using System;
using System.Linq;
using Terraria;
using TerrariaApi.Server;
using TShockAPI;

using Forcefield.Extensions;

namespace Forcefield
{
	[ApiVersion(1, 17)]
    public class Plugin : TerrariaPlugin
	{
		public Plugin(Main game) : base(game)
		{
		}

		public override void Initialize()
		{
			ServerApi.Hooks.GameInitialize.Register(this, OnGameInitialize);
			ServerApi.Hooks.GameUpdate.Register(this, OnGameUpdate);
		}

		private void OnGameInitialize(EventArgs args)
		{
			Commands.ChatCommands.Add(new Command("forcefield.use", Forcefield, "forcefield"));
		}

		private void OnGameUpdate(EventArgs args)
		{
			var PlayerList = TShock.Players.Where(x => x != null && x.IsLoggedIn && x.GetForceFieldInfo().enabled);
			foreach (TSPlayer ply in PlayerList)
			{
				ply.SetBuff(116, 300, true);
				var pos = ply.TPlayer.position;
				FFType type = ply.GetForceFieldInfo().Type;
				var NpcList = Main.npc.Where(x => x != null && x.active && !x.friendly && !x.townNPC && x.life != 0 && Vector2.Distance(pos, x.position) < 125 && (DateTime.Now - x.GetForceFieldInfo().LastPush).TotalSeconds >= 0.5);

				foreach(NPC npc in NpcList)
				{
					switch(type)
					{
						case FFType.kill:
							TSPlayer.Server.StrikeNPC(npc.whoAmI, npc.lifeMax + 200, 0, 0);
							return;
						case FFType.push:
							int xDelta = 1;
							int yDelta = 1;
							if (npc.position.X < ply.TPlayer.position.X)
							{
								xDelta = -1;
							}
							if (npc.position.Y < ply.TPlayer.position.Y)
							{
								yDelta = -1;
							}
							if (npc.velocity == Vector2.Zero)
							{
								npc.velocity.X = 3;
								npc.velocity.Y = 3;
							}
							float force = 0;
							if (npc.velocity.X > npc.velocity.Y)
							{
								force = npc.velocity.X / 15f;
								npc.velocity.X = 25;
								npc.velocity.Y *= force;
							}
							else
							{
								force = npc.velocity.Y / 15f;
								npc.velocity.X *= force;
								npc.velocity.Y = 25;
							}
							npc.velocity.X *= xDelta;
							npc.velocity.Y *= yDelta;
							npc.GetForceFieldInfo().LastPush = DateTime.Now;
							return;
						default:
							return;
					}
				}
			}
		}

		/// <summary>
		/// /forcefield <type> [target]
		/// type - kill, push
		/// </summary>
		/// <param name="args"></param>
		private void Forcefield(CommandArgs args)
		{
			ForceFieldUser ffplayer = null;

			if (args.Parameters.Count < 1)
			{
				args.Player.SendInfoMessage("/forcefield <type> [target]");
				return;
			}

			FFType type;
			if (!Enum.TryParse(args.Parameters[0].ToLower(), out type))
			{
				args.Player.SendErrorMessage("Error: {0} is an invalid type, please use - push, kill", args.Parameters[0]);
				return;
			}

			if (args.Parameters.Count == 2)
			{
				var plStr = string.Join(" ", args.Parameters.Skip(1));
				var players = TShock.Utils.FindPlayer(plStr);

				if (players.Count > 1)
				{
					TShock.Utils.SendMultipleMatchError(args.Player, players.Select(p => p.Name));
				}
				else if (players.Count == 0)
				{
					args.Player.SendErrorMessage("No players matched your search '{0}'.", plStr);
				}
				else
				{
					var player = players[0];
					ffplayer = player.GetForceFieldInfo();
					ffplayer.enabled = !ffplayer.enabled;
					ffplayer.Type = type;
					args.Player.SendSuccessMessage("{0} is {1} protected by a forcefield",
						player.Name, ffplayer.enabled ? "now" : "no longer");

					player.SendSuccessMessage("{0} {1}tivated your forcefield",
						player.Name, ffplayer.enabled ? "ac" : "deac");
				}
				return;
			}

			ffplayer = args.Player.GetForceFieldInfo();
			ffplayer.enabled = !ffplayer.enabled;
			ffplayer.Type = type;
			args.Player.SendSuccessMessage("You are {0} protected by a forcefield.",
				ffplayer.enabled ? "now" : "no longer");
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				ServerApi.Hooks.GameInitialize.Deregister(this, OnGameInitialize);
				ServerApi.Hooks.GameUpdate.Deregister(this, OnGameUpdate);
			}
			base.Dispose(disposing);
		}
    }
}
