using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Forcefield.Extensions;
using Terraria;
using TShockAPI;

namespace Forcefield.Forcefields
{
	public class BuffField : IForcefield
	{
		public string Name
		{
			get { return "BUFF"; }
		}

		public string Description
		{
			get { return "buffing"; }
		}

		public float Radius
		{
			get { return 250f; }
		}

		public void Create(ForceFieldUser player, List<string> args)
		{
			if (!player.HasProperty("LastBuffSet"))
			{
				player.SetProperty("LastBuffSet", DateTime.UtcNow);
			}
			if (!player.HasProperty("BuffList"))
			{
				player.SetProperty("BuffList", new List<int>());
			}

			foreach (string arg in args)
			{
				int buff;
				if (!Int32.TryParse(arg, out buff) && (buff < 0 || buff >= Main.maxBuffTypes))
				{
					continue;
				}

				((List<int>) player["BuffList"]).Add(buff);
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

				if ((DateTime.UtcNow - (DateTime) user["LastBuffSet"]).TotalSeconds >= 1)
				{
					foreach (TSPlayer plr in plrList)
					{
						foreach (int buff in (List<int>)user["BuffList"])
						{
							plr.SetBuff(buff, 60, true);
						}
					}
					user["LastBuffSet"] = DateTime.UtcNow;
				}
			}
		}
	}
}
