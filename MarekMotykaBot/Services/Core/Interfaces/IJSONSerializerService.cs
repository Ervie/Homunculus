using System.Collections.Generic;
using System.Threading.Tasks;

namespace MarekMotykaBot.Services.Core.Interfaces
{
	public interface IJSONSerializerService
	{
		Task<T> LoadSingleFromFileAsync<T>(string fileName);

		Task<List<T>> LoadFromFileAsync<T>(string fileName);

		List<T> LoadFromFile<T>(string fileName);

		Task SaveSingleToFileAsync<T>(string fileName, T dataToSave);

		Task SaveToFileAsync<T>(string fileName, List<T> dataToSave);
	}
}