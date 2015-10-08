using Forcefield.Extensions;
using System.Collections.Generic;
using Terraria;
using TShockAPI;

namespace Forcefield.Forcefields
{
	public sealed class Blackhole : IForcefield
	{
		public string Description
		{
			get
			{
				return "NPC-sucking";
			}
		}

		public string Name
		{
			get
			{
				return "BLACKHOLE";
			}
		}

		public float Radius
		{
			get
			{
				return 150f;
			}
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

				for (int i = 0; i < Main.npc.Length; i++)
				{
					NPC npc = Main.npc[i];
					if (npc.active && !npc.friendly && !npc.townNPC && npc.life != 0 
						&& Vector2.Distance(pos, npc.position) < Radius)
					{
						Main.npc[i].active = false;
						Main.npc[i].type = 0;
						TSPlayer.All.SendData(PacketTypes.NpcUpdate, "", i);
					}
				}
			}
		}
	}
}
