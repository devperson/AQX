using System;
using System.IO;

using Aquamonix.Mobile.Lib.Environment;

namespace Aquamonix.Mobile.Lib.Utilities
{
    /// <summary>
    /// Static helper class for reading/writing to the local platform-specific filesystem.
    /// </summary>
	public static class FileUtility
	{
		public static string GetCachesDirectory()
		{
			return Providers.FileUtility?.GetCachesDirectory(); 
		}

        public static string GetUserDirectory()
        {
            return Providers.FileUtility?.GetUserDirectory();
        }

		public static string GetLogDirectory()
		{
			return Providers.FileUtility?.GetLogDirectory();
		}

		public static bool FileExists(string path)
		{
			if (Providers.FileUtility != null)
				return Providers.FileUtility.FileExists(path);

			return false;
		}

		public static void WriteAllBytes(string filePath, byte[] bytes)
		{
			Providers.FileUtility?.WriteAllBytes(filePath, bytes);
		}

		public static void WriteAllText(string filePath, string text)
		{
			Providers.FileUtility?.WriteAllText(filePath, text);
		}

		public static byte[] ReadAllBytes(string filePath)
		{
			return Providers.FileUtility?.ReadAllBytes(filePath);
		}

		public static string ReadAllText(string filePath)
		{
			return Providers.FileUtility?.ReadAllText(filePath);
		}

		public static Stream FileStreamCreate(string filePath, bool overwrite = true)
		{
			return Providers.FileUtility?.FileStreamCreate(filePath, overwrite);
		}

		public static Stream FileStreamOpen(string filePath)
		{
			return Providers.FileUtility?.FileStreamOpen(filePath);
		}

		public static void DeleteFiles(string path, string pattern)
		{
			Providers.FileUtility?.DeleteFiles(path, pattern);
		}

		public static void DeleteFile(string path)
		{
			Providers.FileUtility?.DeleteFile(path);
		}
	}
}
