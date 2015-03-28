using System.Collections.Generic;
using System.Linq;
using Forcefield.Extensions;
using Forcefield.Forcefields;
using TShockAPI;

namespace Forcefield
{
	public class FieldContainer
	{
		private readonly List<IForcefield> _forcefields = new List<IForcefield>();

		public FieldContainer()
		{
			_forcefields.Add(new Healfield());
			_forcefields.Add(new Manafield());
			_forcefields.Add(new Killfield());
			_forcefields.Add(new Pushfield());
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
				forcefield.Update(playerList);
			}
		}
	}
}