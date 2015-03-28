using System;

namespace Forcefield
{
	public class ForceFieldUser
	{
		public bool Enabled { get; set; }
		public FFType Type { get; set; }
		public int RecoveryAmt { get; set; }
		public DateTime LastRecovered { get; set; }
		public ForceFieldUser()
		{
			Enabled = false;
			Type = FFType.None;
			RecoveryAmt = 0;
			DateTime LastRecovered = DateTime.MinValue;
		}
	}

	public enum FFType
	{
		None = 0,
		Kill = 1,
		Push = 2,
		Heal = 3,
		Mana = 4
	}
}
