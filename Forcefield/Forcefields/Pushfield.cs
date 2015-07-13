using System.Collections.Generic;
using System.Linq;
using Forcefield.Extensions;
using Terraria;
using TShockAPI;

namespace Forcefield.Forcefields
{
	public class Pushfield : IForcefield
	{

		public string Name
		{
			get { return "PUSH"; }
		}

		public string Description
		{
			get { return "pushing"; }
		}

		public float Radius
		{
			get { return 250f; }
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
					var xDirection = 1;
					var yDirection = 1;
					if (npc.position.X < player.TPlayer.position.X)
					{
						xDirection = -1;
					}
					if (npc.position.Y < player.TPlayer.position.Y)
					{
						yDirection = -1;
					}
					if (npc.velocity == Vector2.Zero)
					{
						npc.velocity.X = 3;
						npc.velocity.Y = 3;
					}
					float force;
					if (npc.velocity.X > npc.velocity.Y)
					{
						force = npc.velocity.X/15f;
						npc.velocity.X = 25;
						npc.velocity.Y *= force;
					}
					else
					{
						force = npc.velocity.Y/15f;
						npc.velocity.X *= force;
						npc.velocity.Y = 25;
					}
					npc.velocity.X *= xDirection;
					npc.velocity.Y *= yDirection;
				}
			}
		}
	}
}