using System;

namespace Forcefield
{
	public class NPCPush
	{
		public DateTime LastPush { get; set; }
		public NPCPush()
		{
			LastPush = DateTime.MinValue;
		}
	}
}
