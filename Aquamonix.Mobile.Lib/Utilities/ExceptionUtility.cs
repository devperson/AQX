using System;
using System.Threading.Tasks;

namespace Aquamonix.Mobile.Lib.Utilities
{
    /// <summary>
    /// Utility for automatically wrapping code in a try/catch block with a standard exception handler.
    /// </summary>
	public static class ExceptionUtility
	{
		public static void Try(Action activity)
		{
            try
            {
                if (activity != null)
                    activity();
            }
            catch (Exception e)
            {
                LogUtility.LogException(e);
            }
        }

		public static T Try<T>(Func<T> activity, T defaultValue = default(T))
		{
			try
			{
				if (activity != null)
					return activity();
			}
			catch (Exception e)
			{
				LogUtility.LogException(e);
			}

			return default(T);
		}
	}
}
