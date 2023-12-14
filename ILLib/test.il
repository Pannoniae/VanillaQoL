.assembly extern System.Runtime {
    .publickeytoken = (b0 3f 5f 7f 11 d5 0a 3a )
    .ver 6:0:0:0
}
.assembly ILProj
{
  .ver 1:0:0:0
}

.module ILProj.dll

.class public auto ansi
  ILProj.Util
{

.method public hidebysig static void
    wipeReadonlyFieldIL(
      class [System.Runtime]System.Reflection.FieldInfo 'field'
    ) cil managed
  {
    .maxstack 2
    .locals init (
      [0] object f,
      [1] object* addr
    )

    // [45 69 - 45 70]
    IL_0000: nop

    // [46 9 - 46 38]
    IL_0001: ldarg.0      // 'field'
    IL_0002: ldnull
    IL_0003: callvirt     instance object [System.Runtime]System.Reflection.FieldInfo::GetValue(object)
    IL_0008: stloc.0      // f

    // [47 9 - 47 23]
    IL_0009: ldloca.s     f
    IL_000b: conv.u
    IL_000c: stloc.1      // addr

    // [48 9 - 48 22]
    IL_000d: ldloc.1      // addr
    IL_000e: ldnull
    IL_000f: stind.ref

    // [49 5 - 49 6]
    IL_0010: ret

  }
  .method public hidebysig static void
    setReadonlyFieldIL<T>(
      class [System.Runtime]System.Reflection.FieldInfo 'field',
      !!0/*T*/ 'value'
    ) cil managed
  {
    .maxstack 2
    .locals init (
      [0] !!0/*T*/ f,
      [1] !!0/*T*/* addr
    )

    // [52 80 - 52 81]
    IL_0000: nop

    // [53 8 - 53 38]
    IL_0001: ldarg.0      // 'field'
    IL_0002: ldnull
    IL_0003: callvirt     instance object [System.Runtime]System.Reflection.FieldInfo::GetValue(object)
    IL_0008: unbox.any    !!0/*T*/
    IL_000d: stloc.0      // f

    // [54 8 - 54 22]
    IL_000e: ldloca.s     f
    IL_0010: conv.u
    IL_0011: stloc.1      // addr

    // [55 8 - 55 22]
    IL_0012: ldloc.1      // addr
    IL_0013: ldarg.1      // 'value'
    IL_0014: stobj        !!0/*T*/

    // [56 4 - 56 5]
    IL_0019: ret

  } // end of method Utils::setReadonlyFieldIL

}
