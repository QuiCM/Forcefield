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
		private FieldContainer _forcefields;

		public override string Author
		{
			get { return "White, IcyPhoenix, AmoKa"; }
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
			Commands.ChatCommands.Add(new Command("forcefield.use", Forcefield, "forcefield", "ff")
			{
				HelpText = string.Format("Syntax: {0}ff [type] <target> <restore>", TShock.Config.CommandSpecifier)
			});

			_forcefields = new FieldContainer();
		}

		private void OnGameUpdate(EventArgs args)
		{
			_forcefields.Update();
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
				if (args.Player.GetForceFieldUser().Enabled)
				{
					args.Player.GetForceFieldUser().Type = FFType.None;
					args.Player.GetForceFieldUser().Enabled = false;
					args.Player.SendWarningMessage("Forcefield disabled.");
					return;
				}
				args.Player.SendInfoMessage("Usage: forcefield <type> [target]");
				return;
			}

			FFType type;
			if (!Enum.TryParse(args.Parameters[0], true, out type))
			{
				args.Player.SendErrorMessage("Error: {0} is an invalid type, please use push, kill, heal, mana, speed or none.",
					args.Parameters[0]);
				return;
			}

			var amount = 5;
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

					if (type == FFType.None)
					{
						ffplayer.Type = FFType.None;
						ffplayer.Enabled = false;

						args.Player.SendSuccessMessage("{0}'s forcefield has been deactivated.", player.Name);
						player.SendSuccessMessage("{0} has deactivated your forcefield.", args.Player.Name);
						return;
					}

					if (ffplayer.Type.HasFlag(type))
					{
						//remove the flag
						ffplayer.Type &= ~type;

						//All flags except none have been removed
						if (ffplayer.Type == FFType.None)
						{
							ffplayer.Enabled = false;
						}

						args.Player.SendSuccessMessage("You have removed {0}'s {1} forcefield.",
							player.Name,
							type == FFType.Kill
								? "killing"
								: type == FFType.Heal
									? "healing"
									: type == FFType.Mana
										? "mana restoring"
										: type == FFType.Speed
											? "speedy"
											: "pushing");

						player.SendSuccessMessage("{0} has removed your {1} forcefield.",
							args.Player.Name,
							type == FFType.Kill
								? "killing"
								: type == FFType.Heal
									? "healing"
									: type == FFType.Mana
										? "mana restoring"
										: type == FFType.Speed
											? "speedy"
											: "pushing");
					}
					else
					{
						//add the flag
						ffplayer.Type |= type;
						ffplayer.Enabled = true;

						if (type == FFType.Heal)
						{
							ffplayer.HealthRecoveryAmt = amount;
						}
						else if (type == FFType.Mana)
						{
							ffplayer.ManaRecoveryAmt = amount;
						}
						else if (type == FFType.Speed)
						{
							ffplayer.SpeedFactor = amount;
						}

						args.Player.SendSuccessMessage("You have given {0} a {1} forcefield.",
							player.Name,
							type == FFType.Kill
								? "killing"
								: type == FFType.Heal
									? "healing"
									: type == FFType.Mana
										? "mana restoring"
										: type == FFType.Speed
											? "speedy"
											: "pushing");

						player.SendSuccessMessage("{0} has given you a {1} forcefield.",
							args.Player.Name,
							type == FFType.Kill
								? "killing"
								: type == FFType.Heal
									? "healing"
									: type == FFType.Mana
										? "mana restoring"
										: type == FFType.Speed
											? "speedy"
											: "pushing");
					}

					ffplayer.HealthRecoveryAmt = amount;
				}

				return;
			}

			ffplayer = args.Player.GetForceFieldUser();

			if (type == FFType.None)
			{
				ffplayer.Type = FFType.None;
				ffplayer.Enabled = false;

				args.Player.SendSuccessMessage("Your forcefield has been deactivated.");
				return;
			}
			
			if (ffplayer.Type.HasFlag(type))
			{
				ffplayer.Type &= ~type;

				if (ffplayer.Type == FFType.None)
				{
					ffplayer.Enabled = false;
				}

				args.Player.SendSuccessMessage("You have removed your {0} forcefield.",
					type == FFType.Kill
						? "killing"
						: type == FFType.Heal
							? "healing"
							: type == FFType.Mana
								? "mana restoring"
								: type == FFType.Speed
									? "speedy"
									: "pushing");
			}
			else
			{
				ffplayer.Type |= type;
				ffplayer.Enabled = true;

				if (type == FFType.Heal)
				{
					ffplayer.HealthRecoveryAmt = amount;
				}
				else if (type == FFType.Mana)
				{
					ffplayer.ManaRecoveryAmt = amount;
				}
				else if (type == FFType.Speed)
				{
					ffplayer.SpeedFactor = amount;
				}

				args.Player.SendSuccessMessage("You have activated a {0} forcefield.",
					type == FFType.Kill
						? "killing"
						: type == FFType.Heal
							? "healing"
							: type == FFType.Mana
								? "mana restoring"
								: type == FFType.Speed
									? "speedy"
									: "pushing");
			}
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