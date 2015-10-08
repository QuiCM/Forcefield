using System;
using System.Collections.Generic;
using System.Linq;
using Forcefield.Extensions;
using Terraria;
using TShockAPI;

namespace Forcefield.Forcefields
{
	public class Manafield : IForcefield
	{
		public string Name
		{
			get { return "MANA"; }
		}

		public string Description
		{
			get { return "mana restoring"; }
		}

		public float Radius
		{
			get { return 250f; }
		}

		public void Create(ForceFieldUser player, List<string> args)
		{
			if (!player.HasProperty("LastManaRecovered"))
			{
				player.SetProperty("LastManaRecovered", DateTime.UtcNow);
			}
			if (!player.HasProperty("ManaRecoveryAmount"))
			{
				player.SetProperty("ManaRecoveryAmount", 10);
			}
			if (!player.HasProperty("TimeBetweenRestores"))
			{
				player.SetProperty("TimeBetweenRestores", 5);
			}

			if (args.Count > 0)
			{
				int recover;
				if (Int32.TryParse(args[0], out recover))
				{
					player.SetProperty("HealthRecoveryAmount", recover);
				}
			}
			if (args.Count > 1)
			{
				int timeBetweenRestores;
				if (Int32.TryParse(args[1], out timeBetweenRestores))
				{
					player.SetProperty("TimeBetweenRestores", timeBetweenRestores);
				}
			}
		}

		public void Update(IEnumerable<TSPlayer> shieldedPlayers)
		{
			foreach (var player in shieldedPlayers)
			{
				ForceFieldUser user = player.GetForceFieldUser();
				if (!user.HasField(this))
				{
					continue;
				}

				var pos = player.TPlayer.position;

				var plrList = TShock.Players.Where(
					p => p != null &&
					     p.Active &&
					     p.Team == player.Team &&
					     p.Team != 0 &&
					     Vector2.Distance(pos, p.TPlayer.position) < Radius);

				if ((DateTime.UtcNow - (DateTime)user["LastManaRecovered"]).TotalSeconds >= (int)user["TimeBetweenRestores"])
				{
					foreach (var plr in plrList)
					{
						var restore = Math.Min((int)user["ManaRecoveryAmount"],
							plr.TPlayer.statManaMax2 - plr.TPlayer.statMana);
						if (Main.ServerSideCharacter)
						{
							plr.TPlayer.statMana += restore;
							plr.SendData(PacketTypes.PlayerMana, "", plr.Index);
							plr.SendData(PacketTypes.EffectMana, "", plr.Index, restore);
						}
					}
					user["LastManaRecovered"] = DateTime.UtcNow;
				}
			}
		}
	}
}