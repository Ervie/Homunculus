using System.Threading.Tasks;

namespace MarekMotykaBot.Services.External.Interfaces
{
	public interface IImgFlipService
	{
		Task<string> CreateMarekFace(string topText, string bottomText);

		Task<string> CreateLaughingMarekMeme(string topText, string bottomText);

		Task<string> CreateSkeletorMarekMeme(string topText, string bottomText);

		Task<string> CreateDrakeMarekMeme(string topText, string bottomText);
	}
}