using System.Threading.Tasks;

namespace MarekMotykaBot.Services.External.Interfaces
{
	public interface IImgurService
	{
		Task<string> GetRandomImageFromGallery(string galleryId);

		Task<string> GetRandomImageFromAlbum(string albumId);
	}
}