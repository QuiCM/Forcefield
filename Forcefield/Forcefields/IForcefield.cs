using System.Collections.Generic;
using TShockAPI;

namespace Forcefield.Forcefields
{
	public interface IForcefield
	{
		string Name { get; }
		string Description { get; }
		float Radius { get; }
		void Create(ForceFieldUser player, List<string> args);
		void Update(IEnumerable<TSPlayer> shieldedPlayers);
	}
}