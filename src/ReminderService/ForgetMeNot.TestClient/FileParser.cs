using System;
using System.IO;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace ForgetMeNot.TestClient.Http
{
	public class FileParser
	{
		string _filePath;

		public FileParser (string filePath)
		{
			if (!File.Exists (filePath))
				throw new FileNotFoundException (string.Format("File [{0}] does not exist.", filePath));

			_filePath = filePath;
		}

		public List<ScheduleRequest> Parse()
		{
			return JsonConvert.DeserializeObject<List<ScheduleRequest>> (File.ReadAllText(_filePath));
		}
	}
}

