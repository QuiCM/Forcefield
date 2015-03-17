using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
