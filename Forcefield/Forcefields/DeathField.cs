using System;
using System.Collections.Generic;
using System.Linq;
using Forcefield.Extensions;
using TShockAPI;

namespace Forcefield.Forcefields
{
	public class DeathField : IForcefield
	{
		public string Name
		{
			get { return "DEATH"; }
		}

		public string Description
		{
			get { return "super evil"; }
		}

		public float Radius
		{
			get { return 150f; }
		}

		public void Create(ForceFieldUser player, List<string> args)
		{
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
					     p != player &&
					     p.Active &&
					     Vector2.Distance(pos, p.TPlayer.position) < 250);

				foreach (TSPlayer plr in plrList)
				{
					plr.DamagePlayer(Math.Max(plr.TPlayer.statLifeMax, plr.TPlayer.statLifeMax2));
				}
			}
		}
	}
}
