using System.Runtime.CompilerServices;
using Terraria;

namespace Forcefield.Extensions
{
	public static class NPCExtensions
	{
		private static ConditionalWeakTable<NPC, NPCPush> PushAttempt = new ConditionalWeakTable<NPC, NPCPush>();
		internal static NPCPush GetForceFieldInfo(this NPC npc)
		{
			return PushAttempt.GetValue(npc, ah => new NPCPush());
		}
	}
}
