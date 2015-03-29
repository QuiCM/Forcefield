using System.Collections.Generic;
using Forcefield.Extensions;
using TShockAPI;

namespace Forcefield.Forcefields
{
	public class Speedfield : IForcefield
	{
		public FFType Type
		{
			get { return FFType.Speed; }
		}

		public float Radius
		{
			get { return 150f; }
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

				player.TPlayer.velocity.X = info.SpeedFactor;
				if (player.TPlayer.velocity.Y > 0)
				{
					player.TPlayer.velocity.Y = info.SpeedFactor;
				}
				TSPlayer.All.SendData(PacketTypes.PlayerUpdate, "", player.Index);
			}
		}
	}
}
