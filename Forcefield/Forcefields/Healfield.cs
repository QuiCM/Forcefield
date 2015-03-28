using System;
using System.Collections.Generic;
using System.Linq;
using Forcefield.Extensions;
using TShockAPI;

namespace Forcefield.Forcefields
{
	public class Healfield : IForcefield
	{
		public FFType Type
		{
			get { return FFType.Heal; }
		}

		public float Radius
		{
			get { return 250f; }
		}

		public void Update(IEnumerable<TSPlayer> shieldedPlayers)
		{
			foreach (var player in shieldedPlayers)
			{
				var info = player.GetForceFieldUser();
				if (!info.Type.HasFlag(Type))
				{
					continue;
				}

				var pos = player.TPlayer.position;

				var plrList = TShock.Players.Where(
					p => p != null &&
					     p.Active &&
					     p.Team == player.Team &&
					     p.Team != 0 &&
					     Vector2.Distance(pos, p.TPlayer.position) < 250);

				if ((DateTime.UtcNow - info.LastHealthRecovered).TotalSeconds >= 1)
				{
					foreach (var plr in plrList)
					{
						plr.Heal(info.HealthRecoveryAmt);
					}
					player.GetForceFieldUser().LastHealthRecovered = DateTime.UtcNow;
				}
			}
		}
	}
}