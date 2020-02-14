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
	public interface IApiRequest
	{
        bool IsProgressRequest { get; }
		RequestResponseHeader Header { get; set; }
		CancellationToken CancellationToken { get; set; }
	}

    /// <summary>
    /// Common base class for request objects.
    /// </summary>
	public abstract class ApiRequestBase : IApiRequest
	{
        public virtual bool IsProgressRequest { get { return false; } }

		[DataMember(Name = PropertyNames.RequestResponseHeader, Order=1)]
		public RequestResponseHeader Header { get; set; }

		[IgnoreDataMember]
		public CancellationToken CancellationToken { get; set; }
	}

    /// <summary>
    /// Standard base class for request objects.
    /// </summary>
    /// <typeparam name="T"></typeparam>
	[DataContract]
	public abstract class ApiRequest<T> : ApiRequestBase
	{
		[DataMember(Name = PropertyNames.RequestResponseBody, Order=2)]
		public T Body { get; set; }

		public ApiRequest(string type, string sessionId = null)
		{
			this.Header = new RequestResponseHeader(type, ChannelIdGenerator.GetNext(), sessionId);
		}

		private static class ChannelIdGenerator
		{
			private static long _channelId;

			public static long GetNext()
			{
				return System.Threading.Interlocked.Increment(ref _channelId);
			}
		}
	}
}
