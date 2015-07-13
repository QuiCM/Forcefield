using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Terraria;
using TerrariaApi.Server;
using TShockAPI;

using Forcefield.Extensions;
using Forcefield.Forcefields;

namespace Forcefield
{
	[ApiVersion(1, 19)]
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
				HelpText = string.Format("Syntax: {0}ff [type] <target> <-p parameters>", TShock.Config.CommandSpecifier)
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
				ffplayer = args.Player.GetForceFieldUser();
				if (ffplayer.Enabled)
				{
					ffplayer.ClearFields();
					ffplayer.Enabled = false;
					args.Player.SendWarningMessage("Forcefield disabled.");
					return;
				}
				args.Player.SendInfoMessage("Usage: forcefield <type> [target]");
				return;
			}

			IForcefield field;
			if (!_forcefields.TryParse(args.Parameters[0], out field))
			{
				args.Player.SendErrorMessage("Error: {0} is an invalid type", args.Parameters[0]);
				args.Player.SendErrorMessage("Valid types: {0}", _forcefields.GetValidFields);
				return;
			}

			int index = args.Parameters.IndexOf("-p");

			if (args.Parameters.Count >= 2 && (index == -1 || index > 1))
			{
				//ff <type> <name> <?params>

				//Grabs all parameters between type and -p
				//eg /ff heal multi part name -p 50
				//this variable will be 'multi part name'
				var plStr = String.Join(" ", args.Parameters.Skip(1).TakeWhile(s => s != "-p"));

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
					TSPlayer player = players[0];
					ffplayer = player.GetForceFieldUser();

					if (field == null)
					{
						ffplayer.ClearFields();
						ffplayer.Enabled = false;

						args.Player.SendSuccessMessage("{0}'s forcefield has been deactivated.", player.Name);
						player.SendSuccessMessage("{0} has deactivated your forcefield.", args.Player.Name);
						return;
					}

					if (ffplayer.HasField(field))
					{
						//remove the field
						ffplayer.RemoveField(field);

						//All field except none have been removed
						if (ffplayer.FieldCount == 0)
						{
							ffplayer.Enabled = false;
						}

						args.Player.SendSuccessMessage("You have removed {0}'s {1} forcefield.",
							player.Name, field.Description);

						player.SendSuccessMessage("{0} has removed your {1} forcefield.",
							args.Player.Name, field.Description);
					}
					else
					{
						//Provides field initialization for the player with all parameters after -p
						//eg /ff heal player -p 50
						//the List<string> sent to the method will be { "50" }
						field.Create(ffplayer, args.Parameters.Skip(index + 1).ToList());

						//add the field
						ffplayer.AddField(field);
						ffplayer.Enabled = true;

						string aOrAn = AOrAn(field.Description);
						args.Player.SendSuccessMessage("You have given {0} {1} {2} forcefield.",
							player.Name, aOrAn, field.Description);

						player.SendSuccessMessage("{0} has given you {1} {2} forcefield.",
							args.Player.Name, aOrAn, field.Description);
					}
				}
			}
			else if (args.Parameters.Count >= 2 && index == 1)
			{
				//ff type -p params

				if (!args.Player.RealPlayer)
				{
					args.Player.SendErrorMessage("You can only add forcefields to in-game players.");
					return;
				}

				ffplayer = args.Player.GetForceFieldUser();

				if (field == null)
				{
					ffplayer.ClearFields();
					ffplayer.Enabled = false;

					args.Player.SendSuccessMessage("Your forcefield has been deactivated.");
					return;
				}

				if (ffplayer.HasField(field))
				{
					//remove the field
					ffplayer.RemoveField(field);

					//All field except none have been removed
					if (ffplayer.FieldCount == 0)
					{
						ffplayer.Enabled = false;
					}

					args.Player.SendSuccessMessage("You have removed your {0} forcefield.", field.Description);
				}
				else
				{
					//Provides field initialization for the player with all parameters after -p
					//eg /ff heal player -p 50
					//the List<string> sent to the method will be { "50" }
					field.Create(ffplayer, args.Parameters.Skip(index + 1).ToList());

					//add the field
					ffplayer.AddField(field);
					ffplayer.Enabled = true;

					args.Player.SendSuccessMessage("You have activated {0} {1} forcefield.",
						AOrAn(field.Description), field.Description);
				}
			}
			else if (args.Parameters.Count < 2)
			{
				if (!args.Player.RealPlayer)
				{
					args.Player.SendErrorMessage("You can only add forcefields to in-game players.");
					return;
				}

				ffplayer = args.Player.GetForceFieldUser();

				if (field == null)
				{
					ffplayer.ClearFields();
					ffplayer.Enabled = false;

					args.Player.SendSuccessMessage("Your forcefield has been deactivated.");
					return;
				}

				if (ffplayer.HasField(field))
				{
					//remove the field
					ffplayer.RemoveField(field);

					//All field except none have been removed
					if (ffplayer.FieldCount == 0)
					{
						ffplayer.Enabled = false;
					}

					args.Player.SendSuccessMessage("You have removed your {0} forcefield.", field.Description);
				}
				else
				{
					//Provides field initialization for the player with all parameters after -p
					//eg /ff heal player -p 50
					//the List<string> sent to the method will be { "50" }
					field.Create(ffplayer, new List<string>());

					//add the field
					ffplayer.AddField(field);
					ffplayer.Enabled = true;

					args.Player.SendSuccessMessage("You have activated {0} {1} forcefield.",
						AOrAn(field.Description), field.Description);
				}
			}
		}

		private string AOrAn(string following, bool isStart = false)
		{
			char[] vowels =
			{
				'a',
				'e',
				'i',
				'o',
				'u'
			};

			if (String.IsNullOrEmpty(following))
			{
				if (isStart)
				{
					return "A";
				}
				return "a";
			}

			if (vowels.Any(v => following[0] == v))
			{
				if (isStart)
				{
					return "An";
				}
				return "an";
			}
			if (isStart)
			{
				return "A";
			}
			return "a";
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