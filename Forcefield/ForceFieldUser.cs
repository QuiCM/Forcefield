using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Forcefield
{
	public class ForceFieldUser
	{
		public bool enabled { get; set; }
		public FFType Type { get; set; }
		public ForceFieldUser()
		{
			enabled = false;
			Type = FFType.None;
		}
	}

	public enum FFType
	{
		None = 0,
		kill = 1,
		push = 2
	}
}
