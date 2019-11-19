using System.Collections.Generic;

namespace MarekMotykaBot.Services.Core.Interfaces
{
	public interface IJSONSerializerService
	{
		T LoadSingleFromFile<T>(string fileName);

		List<T> LoadFromFile<T>(string fileName);

		void SaveSingleToFile<T>(string fileName, T dataToSave);

		void SaveToFile<T>(string fileName, List<T> dataToSave);
	}
}