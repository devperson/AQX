using System;

namespace Aquamonix.Mobile.Lib.Utilities
{
	public class MathUtil
	{
		public static int RoundToNearest(int n, int roundTo)
		{
			int nn = n;

			if (n % roundTo == 0)
				return n; 
			
			for (int x = 1; x <= roundTo; x++)
			{
				nn = (n - x);
				if (nn % roundTo == 0)
				{
					return nn;
				}
				nn = (n + x);
				if (nn % roundTo == 0)
				{
					return nn;
				}
			}

			return n;
		}
	}
}
