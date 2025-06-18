using System;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace VanillaQoL.Gameplay;

public class Damage : ModSystem {
    public override void Load() {
        //if (QoLConfig.Instance.damageSpread != DamageSpread.Off) {
        //    Main.DefaultDamageVariationPercent = QoLConfig.Instance.damageSpreadAmount;
        //}

        // If it's completely off, we nuke the damage variation code.
        // If it's only disabled for the player, we only use globalitem/globalproj to disable it for the player only.
        if (QoLConfig.Instance.damageSpread == DamageSpread.Off) {
            IL_Main.DamageVar_float_int_float += damageVarPatch;
        }

        if (QoLConfig.Instance.damageSpread != DamageSpread.Off) {
            if (QoLConfig.Instance.damageSpreadAmount != 15) {
                Main.DefaultDamageVariationPercent = QoLConfig.Instance.damageSpreadAmount;
            }
        }

        if (QoLConfig.Instance.critsBypassDefense) {
            IL_NPC.HitModifiers.GetDamage += critsBypassDefense;
        }
    }

    public override void Unload() {
        IL_Main.DamageVar_float_int_float -= damageVarPatch;
        IL_NPC.HitModifiers.GetDamage -= critsBypassDefense;
    }

    // [53 13 - 53 42]
    // IL_002a: ldloca.s     a
    // IL_002c: ldflda       valuetype [tModLoader]Terraria.ModLoader.MultipliableFloat [tModLoader]Terraria.NPC/HitModifiers::DefenseEffectiveness
    // IL_0031: dup
    // IL_0032: ldobj        [tModLoader]Terraria.ModLoader.MultipliableFloat
    // IL_0037: ldc.r4       0.0
    // IL_003c: call         valuetype [tModLoader]Terraria.ModLoader.MultipliableFloat [tModLoader]Terraria.ModLoader.MultipliableFloat::op_Multiply(valuetype [tModLoader]Terraria.ModLoader.MultipliableFloat, float32)
    // IL_0041: stobj        [tModLoader]Terraria.ModLoader.MultipliableFloat


    private static void critsBypassDefense(ILContext il) {
        var ilCursor = new ILCursor(il);
        //if (crit) {
        //    self.DefenseEffectiveness *= 0f;
        //}
        var label = ilCursor.DefineLabel();
        ilCursor.EmitLdarg(2); // crit
        ilCursor.Emit(OpCodes.Brfalse_S, label);
        
        ilCursor.EmitLdarg(0); // self
        ilCursor.Emit<NPC.HitModifiers>(OpCodes.Ldflda, "DefenseEffectiveness");
        ilCursor.EmitDup();
        ilCursor.Emit(OpCodes.Ldobj, typeof(MultipliableFloat));
        ilCursor.Emit(OpCodes.Ldc_R4, 0f);
        ilCursor.EmitCall(typeof(MultipliableFloat).GetMethod("op_Multiply", [typeof(MultipliableFloat), typeof(float)])!);
        ilCursor.EmitStobj(typeof(MultipliableFloat));

        ilCursor.MarkLabel(label);
    }


    //  [21 9 - 21 76]
    // IL_0007: ldsfld       class VanillaQoL.Config.QoLConfig VanillaQoL.Config.QoLConfig::Instance
    // IL_000c: callvirt     instance valuetype VanillaQoL.Config.DamageSpread VanillaQoL.Config.QoLConfig::get_damageSpread()
    // IL_0011: ldc.i4.2
    // IL_0012: bne.un.s     IL_001c

    // [22 13 - 22 46]
    // IL_0014: ldarg.2      // dmg
    // IL_0015: conv.r8
    // IL_0016: call         float64 [System.Runtime]System.Math::Round(float64)
    // IL_001b: pop
    private static void damageVarPatch(ILContext il) {
        var ilCursor = new ILCursor(il);

        var label = ilCursor.DefineLabel();
        ilCursor.EmitLdsfld<QoLConfig>("Instance");
        ilCursor.EmitCallvirt<QoLConfig>("get_damageSpread");
        ilCursor.Emit(OpCodes.Ldc_I4_2);
        ilCursor.Emit(OpCodes.Bne_Un_S, label);
        ilCursor.EmitLdarg2();
        ilCursor.EmitConvR8();
        ilCursor.EmitCall(typeof(Math).GetMethod("Round", [typeof(double)])!);
        ilCursor.EmitConvI4();
        ilCursor.EmitRet();
        ilCursor.MarkLabel(label);
    }
}

public class DamageSpreadItem : GlobalItem {
    public override bool IsLoadingEnabled(Mod mod) {
        return QoLConfig.Instance.damageSpread == DamageSpread.Enemies;
    }

    public override void ModifyHitNPC(Item item, Player player, NPC target, ref NPC.HitModifiers modifiers) {
        modifiers.DamageVariationScale *= 0f;
    }
}

public class DamageSpreadProj : GlobalProjectile {
    public override bool IsLoadingEnabled(Mod mod) {
        return QoLConfig.Instance.damageSpread == DamageSpread.Enemies;
    }

    public override void ModifyHitNPC(Projectile projectile, NPC target, ref NPC.HitModifiers modifiers) {
        modifiers.DamageVariationScale *= 0f;
    }
}

public class DamagePlayer : ModPlayer {
    public override bool IsLoadingEnabled(Mod mod) {
        return QoLConfig.Instance.superCrits;
    }

    public int mostCrit;

    public override void ModifyHitNPCWithItem(Item item, NPC target, ref NPC.HitModifiers modifiers) {
        var critChance = Player.GetWeaponCrit(item);
        if (critChance <= 100) {
            return;
        }

        mostCrit = 0;

        // 0 if normal crit, 1 if super crit, 2 if 2x super crit, etc.
        int critTier = critChance / 100 - 1;
        if (critChance % 100 > Main.rand.Next(101)) {
            critTier++;
        }

        modifiers.CritDamage += critTier;
        mostCrit = critTier;

        SoundEngine.PlaySound(in SoundID.Item70, target.position);
        for (int i = 0; i < 4; ++i) {
            Dust.NewDustDirect(target.position, 2, 2, DustID.Flare, SpeedY: 0.5f, Alpha: 150, Scale: 1.55f);
        }

        for (int i = 0; i < 3; ++i) {
            Dust.NewDustDirect(target.position, 2, 2, DustID.UltraBrightTorch, SpeedY: 0.5f, Alpha: 150, Scale: 2.05f);
        }

        if (mostCrit < 0) {
            mostCrit = 0;
        }
    }

    public override void ModifyHitNPCWithProj(
        Projectile proj,
        NPC target,
        ref NPC.HitModifiers modifiers) {
        var critChance = proj.CritChance;
        if (critChance <= 100) {
            return;
        }

        mostCrit = 0;

        // 0 if normal crit, 1 if super crit, 2 if 2x super crit, etc.
        int critTier = critChance / 100 - 1;
        if (critChance % 100 > Main.rand.Next(101)) {
            critTier++;
        }

        modifiers.CritDamage += critTier;
        mostCrit = critTier;

        SoundEngine.PlaySound(in SoundID.Item70, target.position);
        for (int i = 0; i < 4; ++i) {
            Dust.NewDustDirect(target.position, 2, 2, DustID.Flare, SpeedY: 0.5f, Alpha: 150, Scale: 1.55f);
        }

        for (int i = 0; i < 3; ++i) {
            Dust.NewDustDirect(target.position, 2, 2, DustID.UltraBrightTorch, SpeedY: 0.5f, Alpha: 150, Scale: 2.05f);
        }


        if (mostCrit < 0) {
            mostCrit = 0;
        }
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) {
        if (!QoLConfig.Instance.superCritsColour || !hit.Crit) {
            return;
        }

        // search for crit text (60 is base lifetime, *2 is crit = 120)
        int ct = -1;
        for (var i = 99; i >= 0; --i) {
            if (Main.combatText[i].lifeTime == 120) {
                ct = i;
                break;
            }
        }

        // if not found || already applied somehow?
        if (ct == -1 || Main.combatText[ct].color == QoLConfig.Instance.superCritsColourValue) {
            return;
        }

        if (mostCrit > 0) {
            Main.combatText[ct].color = QoLConfig.Instance.superCritsColourValue;
        }
    }
}