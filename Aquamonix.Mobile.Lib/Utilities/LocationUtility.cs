using System;

namespace Aquamonix.Mobile.Lib.Utilities
{
    /// <summary>
    /// Interface for provding platform-specific GPS coordinates.
    /// </summary>
	public interface ILocationUtility
	{
        /// <summary>
        /// Returns the distance in meters of the current user from the given location.
        /// </summary>
        /// <param name="latitude">Latitude of target location</param>
        /// <param name="longitude">Longitude of target location</param>
        /// <returns>A distance in meters</returns>
		double DistanceInMetersFrom(double latitude, double longitude);
	}

    /// <summary>
    /// Static helper class for GPS location. 
    /// </summary>
	public static class LocationUtility
	{
		public static double DistanceInMetersFrom(double latitude, double longitude)
		{
			if (Environment.Providers.LocationUtility != null)
				return Environment.Providers.LocationUtility.DistanceInMetersFrom(latitude, longitude);

			return -1;
		}
	}
}
