using System.Collections.Generic;
using TShockAPI;

namespace Forcefield.Forcefields
{
	public interface IForcefield
	{
		FFType Type { get; }
		float Radius { get; }
		void Update(IEnumerable<TSPlayer> shieldedPlayers);
	}
}