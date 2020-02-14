using System;
using System.IO;


namespace Aquamonix.Mobile.Lib.Utilities
{
    /// <summary>
    /// Interface for reading/writing from/to the platform-specific filesystem.
    /// </summary>
	public interface IFileUtility
	{
		string GetCachesDirectory();

        string GetUserDirectory();

		string GetLogDirectory();

		bool FileExists(string path);

		void WriteAllBytes(string filePath, byte[] bytes);

		void WriteAllText(string filePath, string text);

		byte[] ReadAllBytes(string filePath);

		string ReadAllText(string filePath);

		Stream FileStreamCreate(string filePath, bool overwrite = true);

		Stream FileStreamOpen(string filePath);

		void DeleteFiles(string path, string pattern);

		void DeleteFile(string path);
	}
}
