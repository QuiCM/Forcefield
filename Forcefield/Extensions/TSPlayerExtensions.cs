using System.Runtime.CompilerServices;
using TShockAPI;

namespace Forcefield.Extensions
{
	public static class TSPlayerExtensions
	{
		private static readonly ConditionalWeakTable<TSPlayer, ForceFieldUser> Players = new ConditionalWeakTable<TSPlayer, ForceFieldUser>();

		internal static ForceFieldUser GetForceFieldUser(this TSPlayer tsplayer)
		{
			return Players.GetOrCreateValue(tsplayer);
		}
	}
}
