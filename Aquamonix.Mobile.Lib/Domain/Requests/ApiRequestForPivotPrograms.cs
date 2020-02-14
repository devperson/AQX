using System;
using System.Threading;
using System.Collections.Generic;
using System.Runtime.Serialization;

using ServiceStack.Text;
using Newtonsoft.Json;

namespace Aquamonix.Mobile.Lib.Domain
{
    /// <summary>
    /// Base interface for request objects.
    /// </summary>

    /// <summary>
    /// Common base class for request objects.
    /// </summary>


    /// <summary>
    /// Standard base class for request objects.
    /// </summary>
    /// <typeparam name="T"></typeparam>
	[DataContract]
    public abstract class ApiRequestForPivotPrograms<T> : ApiRequestBase
    {
        [DataMember(Name = PropertyNames.RequestResponseBody, Order = 2)]
        public T Body { get; set; }

        public ApiRequestForPivotPrograms(string type, string sessionId = null)
        {
            this.Header = new RequestResponseHeader(type, ChannelIdGenerator.GetNext(), sessionId);
        }

        private static class ChannelIdGenerator
        {
            private static long _channelId=311;

            public static long GetNext()
            {
                return System.Threading.Interlocked.Increment(ref _channelId);
            }
        }
    }
}
