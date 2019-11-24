using MarekMotykaBot.DataTypes;
using System.Collections.Generic;

namespace MarekMotykaBot.Services.External.Interfaces
{
	public interface IUnrealTournamentService
	{
		void ChangeRotation(UTRotationConfiguration rotationConfiguration);

		ICollection<string> GetCurrentRotationMapList();
	}
}