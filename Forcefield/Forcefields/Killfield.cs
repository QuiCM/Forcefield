using System.Collections.Generic;
using System.Linq;
using Forcefield.Extensions;
using Terraria;
using TShockAPI;

namespace Forcefield.Forcefields
{
	internal class Killfield : IForcefield
	{
		public string Name
		{
			get { return "KILL"; }
		}

		public string Description
		{
			get { return "killing"; }
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

				var npcList =
					Main.npc.Where(
						n => n != null &&
						     n.active &&
						     !n.friendly &&
						     !n.townNPC &&
						     n.life != 0 &&
						     Vector2.Distance(pos, n.position) < Radius);

				foreach (var npc in npcList)
				{
					TSPlayer.Server.StrikeNPC(npc.whoAmI, npc.lifeMax + (npc.defense*2) + 200, 0, 0);
				}
			}
		}
	}
}