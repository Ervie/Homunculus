using MarekMotykaBot.DataTypes;

namespace MarekMotykaBot.Services.External.Interfaces
{
	public interface IUnrealTournamentService
	{
		void ChangeRotation(UTRotationConfiguration rotationConfiguration);
	}
}