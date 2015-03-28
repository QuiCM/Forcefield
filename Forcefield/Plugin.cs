using System;
using System.Linq;
using System.Reflection;
using Terraria;
using TerrariaApi.Server;
using TShockAPI;

using Forcefield.Extensions;

namespace Forcefield
{
	[ApiVersion(1, 17)]
    public class Plugin : TerrariaPlugin
	{
		public override string Author
		{
			get { return "White, IcyPhoenix"; }
		}

		public override string Description
		{
			get { return "Creates a forcefield around players"; }
		}

		public override string Name
		{
			get { return "Forcefield"; }
		}

		public override Version Version
		{
			get { return Assembly.GetExecutingAssembly().GetName().Version; }
		}

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
			Commands.ChatCommands.Add(new Command("forcefield.use", Forcefield, "forcefield", "ff"));
		}

		private void OnGameUpdate(EventArgs args)
		{
			var PlayerList = TShock.Players.Where(p => p != null && p.IsLoggedIn && p.GetForceFieldUser().Enabled);
			foreach (var ply in PlayerList)
			{
				//Sets inferno buff for 5 seconds
				ply.SetBuff(116, 300, true);
				var pos = ply.TPlayer.position;
				var type = ply.GetForceFieldUser().Type;
				var NpcList =
					Main.npc.Where(
						n => n != null &&
						     n.active &&
						     !n.friendly &&
						     !n.townNPC &&
						     n.life != 0 &&
						     Vector2.Distance(pos, n.position) < 125*((int) type));
				var PlrList = TShock.Players.Where(
					p => p != null &&
						p.Active &&
						p.Team == ply.Team &&
						p.Team != 0 &&
						Vector2.Distance(pos, p.TPlayer.position) < 250); //250 because Wight says 3x is too big for his *****

				
				switch (type)
				{
					case FFType.Kill:
						foreach (var npc in NpcList)
						{
							TSPlayer.Server.StrikeNPC(npc.whoAmI, npc.lifeMax + (npc.defense * 2) + 200, 0, 0);
						}
						return;
					case FFType.Push:
						foreach (var npc in NpcList)
						{
							int xDirection = 1;
							int yDirection = 1;
							if (npc.position.X < ply.TPlayer.position.X)
							{
								xDirection = -1;
							}
							if (npc.position.Y < ply.TPlayer.position.Y)
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
							npc.velocity.X *= xDirection;
						}
						return;
					case FFType.Heal:
						if ((DateTime.Now - ply.GetForceFieldUser().LastRecovered).TotalSeconds >= 1)
						{
							foreach (var plr in PlrList)
							{
								plr.Heal(ply.GetForceFieldUser().RecoveryAmt);
							}
							ply.GetForceFieldUser().LastRecovered = DateTime.Now;
						}
						return;
					case FFType.Mana:
						if ((DateTime.Now - ply.GetForceFieldUser().LastRecovered).TotalSeconds >= 1)
						{
							foreach (var plr in PlrList)
							{
								plr.TPlayer.statMana += ply.GetForceFieldUser().RecoveryAmt;
								plr.SendData(PacketTypes.PlayerMana, "", plr.Index);
							}
							ply.GetForceFieldUser().LastRecovered = DateTime.Now;
						}
						return;
					default:
						return;
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
			ForceFieldUser ffplayer;

			if (args.Parameters.Count < 1)
			{
				args.Player.SendInfoMessage("Usage: forcefield <type> [target]");
				return;
			}

			FFType type;
			if (!Enum.TryParse(args.Parameters[0].ToLower(), true, out type))
			{
				args.Player.SendErrorMessage("Error: {0} is an invalid type, please use - push, kill, heal, mana", args.Parameters[0]);
				return;
			}

			int amount = 5;
			if (args.Parameters.Count > 1 && int.TryParse(args.Parameters.Last(), out amount))
			{
				args.Parameters.RemoveAt(args.Parameters.Count - 1);
			}

			if (args.Parameters.Count >= 2)
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
					ffplayer = player.GetForceFieldUser();
					ffplayer.Enabled = !ffplayer.Enabled;
					ffplayer.Type = type;
					ffplayer.RecoveryAmt = amount;
					args.Player.SendSuccessMessage("{0} is {1} protected by a forcefield",
						player.Name, ffplayer.Enabled ? "now" : "no longer");

					player.SendSuccessMessage("{0} {1}tivated your forcefield",
						player.Name, ffplayer.Enabled ? "ac" : "deac");
				}
				return;
			}

			ffplayer = args.Player.GetForceFieldUser();
			ffplayer.Enabled = !ffplayer.Enabled;
			ffplayer.Type = type;
			ffplayer.RecoveryAmt = amount;
			args.Player.SendSuccessMessage("You are {0} protected by a forcefield.",
				ffplayer.Enabled ? "now" : "no longer");
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