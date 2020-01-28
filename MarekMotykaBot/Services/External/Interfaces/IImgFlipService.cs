using System.Threading.Tasks;

namespace MarekMotykaBot.Services.External.Interfaces
{
	public interface IImgFlipService
	{
		Task<string> CreateMarekFace(params string[] text);

		Task<string> CreateLaughingMarekMeme(params string[] text);

		Task<string> CreateSkeletorMarekMeme(params string[] text);

		Task<string> CreateDrakeMarekMeme(params string[] text);

		Task<string> CreateNosaczMeme(params string[] text);
	}
}