using System;
using System.IO;
using Foundation;

namespace Aquamonix.Mobile.IOS.Utilities
{
    /// <summary>
    /// IOS-specific implementation of IFileUtility.
    /// </summary>
	public class FileUtilityIOS : Aquamonix.Mobile.Lib.Utilities.IFileUtility
	{
		public string GetCachesDirectory()
		{
			string[] paths = NSSearchPath.GetDirectories(NSSearchPathDirectory.CachesDirectory, NSSearchPathDomain.User);

			if (paths != null && paths.Length > 0)
				return paths[0];

			throw new Exception("Unable to locate Caches directory.");
		}

        public string GetUserDirectory()
        {
            string[] paths = NSSearchPath.GetDirectories(NSSearchPathDirectory.DocumentDirectory, NSSearchPathDomain.User);

            if (paths != null && paths.Length > 0)
                return paths[0];

            throw new Exception("Unable to locate Caches directory.");
        }

		public string GetLogDirectory()
		{
			return GetCachesDirectory(); 
		}

		public bool FileExists(string path)
		{
			return File.Exists(path);
		}

		public void WriteAllBytes(string filePath, byte[] bytes)
		{
			File.WriteAllBytes(filePath, bytes);
		}

		public void WriteAllText(string filePath, string text)
		{
			File.WriteAllBytes(filePath, System.Text.Encoding.UTF8.GetBytes(text));
		}

		public Stream FileStreamCreate(string filePath, bool overwrite = true)
		{
			var mode = FileMode.Create;
			if (!overwrite)
				mode = FileMode.Append;
			return new FileStream(filePath, mode);
		}

		public Stream FileStreamOpen(string filePath)
		{
			return new FileStream(filePath, FileMode.Open);
		}

		public byte[] ReadAllBytes(string filePath)
		{
			return File.ReadAllBytes(filePath);
		}

		public string ReadAllText(string filePath)
		{
			return System.Text.Encoding.UTF8.GetString(ReadAllBytes(filePath)); 
		}

		public void DeleteFiles(string path, string pattern)
		{
			var files = Directory.GetFiles(path, pattern);

			foreach (var f in files)
			{
				File.Delete(f);
			}
		}

		public void DeleteFile(string path)
		{
			File.Delete(path);
		}
	}
}
