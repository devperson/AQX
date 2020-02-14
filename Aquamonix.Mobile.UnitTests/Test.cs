using NUnit.Framework;
using System;
using System.IO;

using Aquamonix.Mobile.Lib.Domain;
using Aquamonix.Mobile.Lib.Extensions;
using Aquamonix.Mobile.Lib.Environment;
using Aquamonix.Mobile.Lib.Services;
using Aquamonix.Mobile.Lib.Utilities;

namespace Aquamonix.Mobile.UnitTests
{
	[TestFixture()]
	public class Test
	{
		[Test()]
		public void UserSerializeTest()
		{
			Providers.FileUtility = new Aquamonix.Mobile.UnitTests.TestFileUtility(); 
			//User.Delete();

			//get user and verify that there isn't one 
			User user = null; //User.GetCached();

			Assert.IsNull(user);

			//create new user file 
			const string username = "user";
			const string password = "pwd";
			const string server = "server";

			user.Username = username;
			user.Password = password;
			user.ServerUri = server;

			user.Save();

			//read back and verify values 
			user = null; //User.GetCached();

			Assert.IsNotNull(user);
			Assert.AreEqual(user.Username, username);
			Assert.AreEqual(user.Password, password);
			Assert.AreEqual(user.ServerUri, server);

			//modify the user and verify the changes 
			user.Username += "1";
			user.Password += "1";
			user.ServerUri += "1";

			user.Save();

			user = null; //User.GetCached();
			Assert.IsNotNull(user);
			Assert.AreEqual(user.Username, username + "1"); 
			Assert.AreEqual(user.Password, password + "1");
			Assert.AreEqual(user.ServerUri, server + "1");

			//delete the user & verify that it's gone 
			//User.Delete();
			//user = User.GetCached();

			Assert.IsNull(user);
		}
	}

	public class TestFileUtility : Aquamonix.Mobile.Lib.Utilities.IFileUtility
	{
		public string GetCachesDirectory()
		{
			return "/Users"; 
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
