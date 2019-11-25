using MarekMotykaBot.DataTypes;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MarekMotykaBot.Services.External.Interfaces
{
	public interface IUnrealTournamentService
	{
		Task ChangeRotation(UTRotationConfiguration rotationConfiguration);

		Task<ICollection<string>> GetCurrentRotationMapList();
	}
}