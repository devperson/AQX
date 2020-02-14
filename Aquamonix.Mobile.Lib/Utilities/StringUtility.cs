using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace Aquamonix.Mobile.Lib.Utilities
{
	public static class StringUtility
	{
		public static string ToCommaDelimitedList(IEnumerable<string> values, string singularPrefix = null, string pluralPrefix = null)
		{
			var list = values.ToList();
			StringBuilder sb = new StringBuilder();
			string output = String.Empty;

			for (int n = 0; n < list.Count; n++)
			{
				if (list.Count > 1 && (n == list.Count - 1))
				{
					sb.Append("and ");
				}

				sb.Append(list[n]);

				if (list.Count > 2 && n < list.Count - 1)
				{
					sb.Append(", ");
				}
				else {
					sb.Append(" ");
				}
			}

			output = sb.ToString().Trim();

			if (list.Count > 1 && pluralPrefix != null)
				output = pluralPrefix + " " + output;
			else if (list.Count == 1 && singularPrefix != null)
				output = singularPrefix + " " + output;

			return output; 
		}

		public static string RemoveWhitespace(this string s)
		{
			if (s != null)
				s = s.Replace(" ", String.Empty);

			return s; 
		}
	}
}
