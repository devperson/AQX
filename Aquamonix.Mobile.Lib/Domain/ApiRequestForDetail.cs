using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using System.Threading;

namespace Aquamonix.Mobile.Lib.Domain
{
    public interface IDetailsApiRequest
    {
        RequestResponseHeader Header { get; set; }
        CancellationToken CancellationToken { get; set; }
    }

    public abstract class ApiRequestForDetail<T> : ApiRequestBase
    {
        [DataMember(Name = PropertyNames.RequestResponseBody, Order = 2)]
        public T Body { get; set; }

        public ApiRequestForDetail(string type, string sessionId = null)
        {
            this.Header = new RequestResponseHeader(type, ChannelIdGenerator.GetNext(), sessionId);
        }

        private static class ChannelIdGenerator
        {
            private static long _channelId =37;

            public static long GetNext()
            {
                return System.Threading.Interlocked.Increment(ref _channelId);
            }
        }
    }
}
