using System.Runtime.CompilerServices;
using TShockAPI;

namespace Forcefield.Extensions
{
	public static class TSPlayerExtensions
	{
		private static ConditionalWeakTable<TSPlayer, ForceFieldUser> players = new ConditionalWeakTable<TSPlayer, ForceFieldUser>();

		internal static ForceFieldUser GetForceFieldUser(this TSPlayer tsplayer)
		{
			return players.GetValue(tsplayer, ah => new ForceFieldUser());
		}
	}
}
