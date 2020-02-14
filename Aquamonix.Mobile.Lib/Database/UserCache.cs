using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Runtime.Serialization;
using System.IO;

using Aquamonix.Mobile.Lib.Domain;
using Aquamonix.Mobile.Lib.Domain.Responses;
using Aquamonix.Mobile.Lib.Utilities;

namespace Aquamonix.Mobile.Lib.Database
{
    /// <summary>
    /// Controls caching of user data only.
    /// </summary>
    public static class UserCache
    {
        //location to storage file
        private static string _filePath;

        //used for thread-locking 
        private static readonly object _syncLock = new Object();

        /// <summary>
        /// gets/sets the current user object.
        /// </summary>
        public static User CurrentUser { get; set; }

        /// <summary>
        /// Call on app startup.
        /// </summary>
        public static void Initialize()
        {
            _filePath = Path.Combine(FileUtility.GetUserDirectory(), "User.db");

            ReadFromCache();
        }

        /// <summary>
        /// Read in any values previously saved to permanent storage.
        /// </summary>
        public static void ReadFromCache()
        {
            ExceptionUtility.Try(() =>
            {
                if (FileUtility.FileExists(_filePath))
                {
                    LogUtility.LogMessage("UserCache: restoring from file.");

                    string json = FileUtility.ReadAllText(_filePath);
                    var cachedData = JsonUtility.Deserialize<JsonCacheObject>(json);

                    if (cachedData != null)
                    {
                        CurrentUser = cachedData.User;

                        //decrypt password 
                        if (CurrentUser != null)
                        {
                            if (!String.IsNullOrEmpty(CurrentUser.Password))
                                CurrentUser.Password = new AesUtility().DecryptString(CurrentUser.Password);
                            else
                                CurrentUser.Password = String.Empty;
                        }
                    }
                }
                else
                {
                    LogUtility.LogMessage("UserCache: file not found.");
                }
            });
        }

        /// <summary>
        /// Save current state to permanent storage.
        /// </summary>
        public static void Save()
        {
            ExceptionUtility.Try(() =>
            {
                LogUtility.LogMessage("Saving UserCache.");

                var cacheObj = new JsonCacheObject()
                {
                    User = CurrentUser
                };

                if (cacheObj.User != null)
                {
                    cacheObj.User = (User)cacheObj.User.Clone();
                    cacheObj.User.Password = new AesUtility().EncryptToString(cacheObj.User.Password);
                }

                lock (_syncLock)
                {
                    FileUtility.WriteAllText(_filePath, JsonUtility.Serialize(cacheObj));
                }
            });
        }

        /// <summary>
        /// Clear all data from temporary & permanent storage (from memory and disk)
        /// </summary>
        public static void Clear(DataClearType clearType = DataClearType.All)
        {
            ExceptionUtility.Try(() =>
            {
                LogUtility.LogMessage("Clearing cache.");

                if (CurrentUser != null) {

                    if (((clearType & DataClearType.Username) == DataClearType.Username) || clearType == DataClearType.All)
                        CurrentUser.Username = null;
                    if (((clearType & DataClearType.Password) == DataClearType.Password) || clearType == DataClearType.All)
                        CurrentUser.Password = String.Empty;
                    if (((clearType & DataClearType.ServerUri) == DataClearType.ServerUri) || clearType == DataClearType.All)
                        CurrentUser.ServerUri = String.Empty;
                }

                else
                {
                    CurrentUser = null;
                }

                Save();
            });
        }

        [DataContract]
        private class JsonCacheObject
        {
            [DataMember(Name = PropertyNames.User)]
            public User User { get; set; }
        }

        [Flags]
        public enum DataClearType
        {
            All, 
            Username, 
            Password, 
            ServerUri
        }
    }
}
