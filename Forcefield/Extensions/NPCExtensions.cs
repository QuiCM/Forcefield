using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

using Terraria;
using Forcefield.Extensions;

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
