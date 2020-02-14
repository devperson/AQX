using System;
using System.Threading;
using System.Collections.Generic;
using System.Runtime.Serialization;

using ServiceStack.Text;
using Newtonsoft.Json;

using Aquamonix.Mobile.Lib.Domain.Responses;

namespace Aquamonix.Mobile.Lib.Domain
{
    /// <summary>
    /// Base interface for response objects.
    /// </summary>
	public interface IApiResponse
	{
		RequestResponseHeader Header { get; set; }
		ErrorResponseBody ErrorBody { get; set; }
		string RawResponse { get; set; }
		bool HasError { get; }
		bool IsSuccessful { get; }
		bool IsFinal { get; }
		bool IsTimeout { get; }
		bool IsAuthFailure { get; }
		bool IsGlobalResponse { get; }


		bool IsServerDownResponse { get; }
		bool IsReconnectResponse { get; }

		void ReadyResponse();
        ResponseBodyBase GetBody();
	}

    /// <summary>
    /// Common base class for response objects.
    /// </summary>
	[DataContract]
	public class ApiResponseBase : IApiResponse
	{
		private const int ServerDownResponseCode = 10002;
		private const int ReconnectResponseCode = 20003;

		[DataMember(Name = PropertyNames.RequestResponseHeader)]
		public RequestResponseHeader Header { get; set; }

		[IgnoreDataMember]
		public ErrorResponseBody ErrorBody { get; set;}

		[IgnoreDataMember]
		public bool HasError
		{
			get
			{
				return this.ErrorBody != null;
			}
		}

		[IgnoreDataMember]
		public virtual bool IsSuccessful
		{
			get
			{
				return (this.ErrorBody == null);
			}
		}

		[IgnoreDataMember]
		public virtual bool IsFinal { get { return true;}}

		[IgnoreDataMember]
		public virtual bool IsTimeout
		{
			get
			{
				return (this.Header?.Type == ResponseType.TimeoutError);
			}
		}

		[IgnoreDataMember]
		public virtual bool IsAuthFailure
		{
			get
			{
				return false;
			}
		}

		[IgnoreDataMember]
		public virtual bool IsGlobalResponse
		{
			get
			{
				return this.ErrorBody != null && this.ErrorBody.ResponseCode == ServerDownResponseCode;
			}
		}

		[IgnoreDataMember]
		public virtual bool IsServerDownResponse
		{
			get
			{
				return this.ErrorBody != null && this.ErrorBody.ResponseCode == ServerDownResponseCode;
			}
		}

		[IgnoreDataMember]
		public virtual bool IsReconnectResponse
		{
			get
			{
				return this.ErrorBody != null && this.ErrorBody.ResponseCode == ReconnectResponseCode;
			}
		}

		[IgnoreDataMember]
		public string RawResponse { get; set;}

		public ApiResponseBase() { }

		public virtual void ReadyResponse() { }

        public virtual ResponseBodyBase GetBody() { return null; }
    }

    /// <summary>
    /// Standard base class for response objects.
    /// </summary>
    /// <typeparam name="T"></typeparam>
	[DataContract]
	public abstract class ApiResponse<T> : ApiResponseBase where T: ResponseBodyBase
	{
		[DataMember(Name = PropertyNames.RequestResponseBody)]
		public T Body { get; set; }

		[IgnoreDataMember]
		public override bool IsSuccessful
		{
			get
			{
				return (this.ErrorBody == null && this.Body != null && !this.IsAuthFailure);
			}
		}

		[IgnoreDataMember]
		public override bool IsAuthFailure
		{
			get
			{
				return ((this.Body?.IsAuthenticated != null && this.Body.IsAuthenticated.Value == false) || (this.ErrorBody?.ErrorType == ErrorResponseType.AuthFailure));
			}
		}

		public ApiResponse() { }

		/// <summary>
		/// Do any stuff you need to do here, in order to get the response ready to ready. 
		/// E.g. formatting, editing of values, etc. 
		/// </summary>
		public override void ReadyResponse()
		{
			if (this.Body != null)
				this.Body.ReadyResponse(); 
		}

        public override ResponseBodyBase GetBody()
        {
            return (this.Body == null) ? null : (this.Body as ResponseBodyBase); 
        }
    }
}
