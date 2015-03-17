namespace Forcefield
{
	public class ForceFieldUser
	{
		public bool Enabled { get; set; }
		public FFType Type { get; set; }
		public ForceFieldUser()
		{
			Enabled = false;
			Type = FFType.None;
		}
	}

	public enum FFType
	{
		None = 0,
		Kill = 1,
		Push = 2
	}
}
