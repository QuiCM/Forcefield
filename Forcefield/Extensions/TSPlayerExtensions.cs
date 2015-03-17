using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

using TShockAPI;
using TShockAPI.DB;
using Forcefield.Extensions;

namespace Forcefield.Extensions
{
	public static class TSPlayerExtensions
	{
		private static ConditionalWeakTable<TSPlayer, ForceFieldUser> players = new ConditionalWeakTable<TSPlayer, ForceFieldUser>();

		internal static ForceFieldUser GetForceFieldInfo(this TSPlayer tsplayer)
		{
			return players.GetValue(tsplayer, ah => new ForceFieldUser());
		}
	}
}
