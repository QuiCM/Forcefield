using System;
using System.Collections.Generic;
using System.Linq;
using Forcefield.Extensions;
using Forcefield.Forcefields;
using TShockAPI;

namespace Forcefield
{
	public class FieldContainer
	{
		private readonly Dictionary<string, IForcefield> _forcefields = new Dictionary<string, IForcefield>();

		public FieldContainer()
		{
			_forcefields.Add("HEAL", new Healfield());
			_forcefields.Add("MANA", new Manafield());
			_forcefields.Add("KILL", new Killfield());
			_forcefields.Add("PUSH", new Pushfield());
		}

		public bool TryParse(string name, out IForcefield field)
		{
			if (name.ToUpper() == "NONE")
			{
				field = null;
				return true;
			}

			return _forcefields.TryGetValue(name.ToUpper(), out field);
		}

		public string GetValidFields
		{
			get { return "none, " + String.Join(", ", _forcefields.Select(f => f.Key.ToLower())); }
		}

		public void Update()
		{
			var playerList = TShock.Players.Where(p => p != null && p.IsLoggedIn && p.GetForceFieldUser().Enabled).ToList();

			foreach (var player in playerList)
			{
				player.SetBuff(116, 300, true);
			}
			foreach (var forcefield in _forcefields)
			{
				forcefield.Value.Update(playerList);
			}
		}
	}
}