using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Aquamonix.Mobile.Lib.Domain.Responses
{
	[DataContract]
	public class ErrorResponse : ApiResponse<ErrorResponseBody>
	{
		public ErrorResponse() : base()
		{
		}
	}

	[DataContract]
	public class ErrorResponseBody : ResponseBodyBase
	{
		[DataMember(Name=PropertyNames.Process)]
		public string Process { get; set; }

		[DataMember(Name = PropertyNames.Location)]
		public string Location { get; set; }

        [DataMember(Name = PropertyNames.ResponseMessageShort)]
		public string ResponseMessageShort { get; set; }

		[DataMember(Name = PropertyNames.ResponseMessageLong)]
		public string ResponseMessageLong { get; set; }

		[IgnoreDataMember]
		public ErrorResponseType ErrorType { get; set;}
	}

	public enum ErrorResponseType
	{
		General, 
		ConnectionTimeout, 
		ResponseTimeout, 
		AuthFailure 
	}
}
