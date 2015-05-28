using System;

namespace Forcefield
{
	public class ForceFieldUser
	{
		public bool Enabled { get; set; }
		public FFType Type { get; set; }
		public int HealthRecoveryAmt { get; set; }
		public int ManaRecoveryAmt { get; set; }
		public int SpeedFactor { get; set; }
		public DateTime LastHealthRecovered { get; set; }
		public DateTime LastManaRecovered { get; set; }

		public ForceFieldUser()
		{
			Enabled = false;
			Type = FFType.None;
			HealthRecoveryAmt = 0;
			ManaRecoveryAmt = 0;
			SpeedFactor = 1;
			LastHealthRecovered = DateTime.MinValue;
			LastManaRecovered = DateTime.MinValue;
		}
	}

	[Flags]
	public enum FFType
	{
		None = 0,
		Kill = 1,
		Push = 2,
		Heal = 4,
		Mana = 8,
		Speed = 16
	}
}