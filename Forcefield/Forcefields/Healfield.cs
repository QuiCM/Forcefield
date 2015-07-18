using System;
using System.Collections.Generic;
using System.Linq;
using Forcefield.Extensions;
using TShockAPI;

namespace Forcefield.Forcefields
{
	public class Healfield : IForcefield
	{
		public string Name
		{
			get { return "HEAL"; }
		}

		public string Description
		{
			get { return "healing"; }
		}

		public float Radius
		{
			get { return 250f; }
		}

		public void Create(ForceFieldUser player, List<string> args)
		{
			if (!player.HasProperty("LastHealthRecovered"))
			{
				player.SetProperty("LastHealthRecovered", DateTime.UtcNow);
			}
			if (!player.HasProperty("HealthRecoveryAmount"))
			{
				player.SetProperty("HealthRecoveryAmount", 10);
			}
			if (!player.HasProperty("TimeBetweenHeals"))
			{
				player.SetProperty("TimeBetweenHeals", 5);
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
				int timeBetweenHeals;
				if (Int32.TryParse(args[1], out timeBetweenHeals))
				{
					player.SetProperty("TimeBetweenHeals", timeBetweenHeals);
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

				if ((DateTime.UtcNow - (DateTime)user["LastHealthRecovered"]).TotalSeconds >= (int)user["TimeBetweenHeals"])
				{
					foreach (var plr in plrList)
					{
						plr.Heal((int)user["HealthRecoveryAmount"]);
					}
					user["LastHealthRecovered"] = DateTime.UtcNow;
				}
			}
		}
	}
}