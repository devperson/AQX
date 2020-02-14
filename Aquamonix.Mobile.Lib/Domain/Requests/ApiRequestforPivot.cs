using System;
using System.Runtime.Serialization;
using System.Threading;

namespace Aquamonix.Mobile.Lib.Domain.Requests
{

    public interface IDetailsApiRequest
    {
        RequestResponseHeader Header { get; set; }
        CancellationToken CancellationToken { get; set; }
    }
    //[DataContract]
    //public abstract class ApiRequestBase1 : IDetailsApiRequest
    //{
    //    [DataMember(Name = PropertyNames.RequestResponseHeader, Order = 1)]
    //    public RequestResponseHeader Header { get; set; }

    //    [IgnoreDataMember]
    //    public CancellationToken CancellationToken { get; set; }
    //}





    [DataContract]
    public abstract class ApiRequestforPivot<T> : ApiRequestBase
    {
        [DataMember(Name = PropertyNames.RequestResponseBody, Order = 2)]
        public T Body { get; set; }

        public ApiRequestforPivot(string type, string sessionId = null)
        {
            this.Header = new RequestResponseHeader(type, ChannelIdGenerator.GetNext(), sessionId);
        }

        private static class ChannelIdGenerator
        {
            private static long _channelId = 315;

            public static long GetNext()
            {
                return System.Threading.Interlocked.Increment(ref _channelId);
            }
        }
    }


}