using System;

namespace Aquamonix.Mobile.Lib.Extensions
{
    /// <summary>
    /// Extension methods for System.String.
    /// </summary>
	public static class StringExtensions
	{
		public static string SubstringIfLonger(this string s, int length)
		{
			if (s == null)
				return null;
			if (s.Length > length)
				return s.Substring(0, length);
			return s;
		}

		public static string ToNormalCase(this string s)
		{
			return s;
		}

		public static string LongestString(this string[] values)
		{
			string output = null;
			if (values != null)
			{
				foreach (string s in values)
				{
					if (!String.IsNullOrEmpty(s))
					{
						if (output == null)
							output = s;
						if (s.Length > output.Length)
							output = s;
					}
				}
			}

			return output; 
		}

		public static string ShortestString(this string[] values)
		{
			string output = null;
			if (values != null)
			{
				foreach (string s in values)
				{
					if (!String.IsNullOrEmpty(s))
					{
						if (output == null)
							output = s;
						if (s.Length < output.Length)
							output = s;
					}
				}
			}

			return output;
		}
	}
}
