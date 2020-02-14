using System;

using CoreLocation;

using Aquamonix.Mobile.Lib.Utilities;

namespace Aquamonix.Mobile.IOS.Utilities
{
	public class LocationUtilityIos : ILocationUtility
	{
		private static CLLocation _location = null; 

		public double DistanceInMetersFrom(double latitude, double longitude)
		{
			double output = -1; 
			CLLocationManager locationManager = new CLLocationManager();

			locationManager.RequestWhenInUseAuthorization();

			if (locationManager.Location != null)
				_location = locationManager.Location;

			if (_location != null)
			{
				CLLocation location2 = new CLLocation(_location.Coordinate.Latitude, _location.Coordinate.Longitude);
				output = location2.DistanceFrom(new CLLocation(latitude, longitude));
			}

			return output; 
		}
	}
}
