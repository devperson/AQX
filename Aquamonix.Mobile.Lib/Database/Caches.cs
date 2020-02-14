using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Runtime.Serialization;
using System.IO;

using Aquamonix.Mobile.Lib.Domain;
using Aquamonix.Mobile.Lib.Domain.Responses;
using Aquamonix.Mobile.Lib.Utilities;
using Aquamonix.Mobile.Lib.Services;

namespace Aquamonix.Mobile.Lib.Database
{
    public static class Caches
    {
        public static void ClearCachesForAuthFailure()
        {
            DataCache.Clear();
            //UserCache.Clear(UserCache.DataClearType.Password);
            ServiceContainer.InvalidateCache();
        }

        public static void ClearCachesForLogout()
        {
            DataCache.Clear();
            //UserCache.Clear(UserCache.DataClearType.Password);
            ServiceContainer.InvalidateCache();
        }

        public static void ClearAll()
        {
            DataCache.Clear();
            UserCache.Clear(); 
            ServiceContainer.InvalidateCache();
        }
    }
}
