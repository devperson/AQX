using System;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Collections.Generic;

using Aquamonix.Mobile.Lib.Database;
using Aquamonix.Mobile.Lib.Utilities;

namespace Aquamonix.Mobile.Lib.Domain 
{
	[DataContract]
	public class User : ICloneable
	{
		[IgnoreDataMember]
		public static User Current
		{
			get { return UserCache.CurrentUser;}
			set { UserCache.CurrentUser = value; }
		}

		[IgnoreDataMember]
		public bool HasConfig
		{
			get
			{
				return (!String.IsNullOrEmpty(this.Username) && !String.IsNullOrEmpty(this.Password) && !String.IsNullOrEmpty(this.ServerUri)); 
			}
		}

		[DataMember]
		public string Username { get; set;}

		[DataMember]
		public string Password { get; set; }

		[DataMember]
		public string ServerUri { get; set; }

		[DataMember]
		public string SessionId { get; set;}

		[DataMember]
		public string Name { get; set; }

        //NOTUSED
        [DataMember]
		public ItemsDictionary<DeviceAccess> DevicesAccess { get; set; }

		public User()
		{
		}

		public void Save()
		{
			UserCache.Save();
		}

		public object Clone()
		{
			User clone = new User()
			{
				Password = this.Password,
				Username = this.Username,
				ServerUri = this.ServerUri,
				SessionId = this.SessionId,
				DevicesAccess = this.DevicesAccess,
				Name = this.Name
            };

			return clone;
		}

		public void Clear()
		{
			this.Username = null;
			this.Password = null;
			this.Name = null;
            this.SessionId = null;
			this.ServerUri = null; 
		}
	}
}
