using System;
using System.Linq;
using Terraria;
using TerrariaApi.Server;
using TShockAPI;

namespace Forcefield
{
	[ApiVersion(1, 17)]
    public class Plugin : TerrariaPlugin
	{
		private readonly bool[] _players = new bool[TShock.Players.Length];

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
			for (var i = 0; i < _players.Length; i++)
			{
				if (!_players[i])
				{
					continue;
				}

				var ply = TShock.Players[i];
				if (ply == null)
				{
					continue;
				}

				ply.SetBuff(116, 300, true);
				var pos = ply.TPlayer.position;

				for (var j = 0; j < Main.npc.Length; j++)
				{
					var npc = Main.npc[j];
					if (npc == null || !npc.active || npc.friendly || npc.townNPC)
					{
						continue;
					}

					var npcPos = npc.position;

					var dist = Math.Sqrt(Math.Pow(pos.X - npcPos.X, 2) + Math.Pow(pos.Y - npcPos.Y, 2));

					if (dist < 125)
					{
						TSPlayer.Server.StrikeNPC(j, 99999, 0, 0);
					}
				}
			}
		}

		private void Forcefield(CommandArgs args)
		{
			if (args.Parameters.Count > 0)
			{
				var plStr = string.Join(" ", args.Parameters);
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
					_players[player.Index] = !_players[player.Index];
					args.Player.SendSuccessMessage("{0} is {1} protected by a forcefield",
						player.Name, _players[player.Index] ? "now" : "no longer");

					player.SendSuccessMessage("{0} {1}tivated your forcefield",
						player.Name, _players[player.Index] ? "ac" : "deac");
				}
				return;
			}

			_players[args.Player.Index] = !_players[args.Player.Index];

			args.Player.SendSuccessMessage("You are {0} protected by a forcefield.",
				_players[args.Player.Index] ? "now" : "no longer");
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
