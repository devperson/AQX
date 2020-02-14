using System;

using Aquamonix.Mobile.Lib.Domain;

namespace Aquamonix.Mobile.Lib.Utilities
{
	public static class DateTimeUtil
	{
		public static DateTime? FromUtcDateString(string s)
		{
			DateTime? output = null;

			if (!String.IsNullOrEmpty(s) && s != "0")
			{
				long value;
				if (Int64.TryParse(s, out value))
				{
					output = FromUnixTime(value);
				}
			}

			return output;
		}

		public static string ToUtcDateString(DateTime? value)
		{
			string output = String.Empty;

			if (value != null)
			{
				output = ToUnixTime(value.Value.ToUniversalTime()).ToString(); 
			}

			return output;
		}

		public static DateTime FromUnixTime(long unixTime)
		{
			var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
			return epoch.AddSeconds(unixTime).ToLocalTime();
		}

		public static long ToUnixTime(DateTime? value)
		{
			long output = 0;

			if (value != null)
			{
				var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
				output = (long)DateTime.Now.Subtract(epoch).TotalSeconds;
			}

			return output;
		}

        public static long GetUnixTimestamp()
        {
            return (long)System.Math.Floor((System.DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds);
        }

        public static long SecondsSinceTimestamp(long unixTimestamp)
        {
            return (GetUnixTimestamp() - unixTimestamp); 
        }

        public static string HowLongAgo(DateTime dt)
		{
			string output = String.Empty;

			int minutes = (int)DateTime.Now.Subtract(dt).TotalMinutes;
			int hours = 0;
			int days = 0;
			if (minutes > (60 * 4))
				hours = (int)(minutes / 60);
			if (hours > 24 * 4)
				days = (int)hours / 24;

			output = String.Format("{0}", hours > 0 ? (days > 0 ? days.ToString() + " days" : hours.ToString() + " hours") : minutes.ToString() + " minutes");

			return output;
		}

		public static string HowLongAgo(string utcString)
		{
			string output = String.Empty;
			var dateTime = FromUtcDateString(utcString);
			if (dateTime != null)
				output = HowLongAgo(dateTime.Value);

			return output;
		}

		public static string DateTimeInEnglish(DateTime? value, bool allCaps = true)
		{
			string output = String.Empty;

			//value = DateTime.Now.Subtract(TimeSpan.FromDays(800)); 
			if (value != null)
			{
				var today = DateTime.Today;
				var yesterday = today.Subtract(TimeSpan.FromDays(1));
				string prefix = String.Empty;

				if (today.Year == value.Value.Year && today.Month == value.Value.Month && today.Day == value.Value.Day)
					prefix = StringLiterals.Today;
				else if (yesterday.Year == value.Value.Year && yesterday.Month == value.Value.Month && yesterday.Day == value.Value.Day)
					prefix = StringLiterals.Yesterday;
				else if (DatesInSameWeek(value.Value, today) && today.Year == value.Value.Year)
					prefix = value.Value.DayOfWeek.ToString();
				else if (today.Year == value.Value.Year)
					prefix = value.Value.ToString("d MMMMM");
				else
					prefix = value.Value.ToString("d MMMMM yyyy");

				output = String.Format("{0}, {1}", prefix, value.Value.ToString("HH:mm"));

				if (allCaps)
					output = output.ToUpper();
			}

			return output; 
   		}

		public static bool DatesInSameWeek(DateTime value1, DateTime value2)
		{
			value1 = value1.Date;
			value2 = value2.Date;

			if (value1 == value2)
				return true;
			
			DateTime greaterDate = (value1 > value2 ? value1 : value2).Date;
			DateTime lesserDate = (value1 > value2 ? value2 : value1).Date;

			if (greaterDate == lesserDate)
				return true;

			if (greaterDate.Subtract(lesserDate).TotalDays >= 7)
				return false;

			if (greaterDate.DayOfWeek == DayOfWeek.Sunday)
				return false;

			while (greaterDate > lesserDate)
			{
				greaterDate = greaterDate.AddDays(-1);
				if (greaterDate == lesserDate)
					return true;
				else if (greaterDate.DayOfWeek == DayOfWeek.Sunday)
					return false;
			}

			return false;
		}
	}
}
