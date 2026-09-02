using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;

namespace VanillaQoL.Gameplay
{
	public class InfoVanityAccessories : ModPlayer
	{
		public override bool IsLoadingEnabled(Mod mod) => QoLConfig.Instance.infoAccWorkInVanity;

		public override void ResetInfoAccessories()
		{
			for (int j = 10; j < Player.armor.Length; j++)
			{
				Player.RefreshInfoAccsFromItemType(Player.armor[j]);
			}
		}
	}
}
