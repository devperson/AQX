using System;

using Aquamonix.Mobile.Lib.Domain;
using Aquamonix.Mobile.Lib.Domain.Responses;
using Aquamonix.Mobile.Lib.Utilities;

namespace Aquamonix.Mobile.Lib.Services
{
	public static class ResponseFactory
	{
		public static T ParseResponse<T>(string json) where T: IApiResponse
		{
			var output = default(T);

			ExceptionUtility.Try(() =>
			{
				output = JsonUtility.Deserialize<T>(json);

				if (output != null)
					output.RawResponse = json;

				if (output?.Header != null && output?.Header.Type == ResponseType.SystemError)
				{
					ErrorResponse errorResponse = JsonUtility.Deserialize<ErrorResponse>(json);
					output.ErrorBody = errorResponse.Body;
				}

				output?.ReadyResponse();
			});

			return output; 
		}

		public static IApiResponse ParseResponse(string json)
		{
			var output = JsonUtility.Deserialize<ApiResponseBase>(json);

			if (output != null)
				output.RawResponse = json;

			if (output != null && output.Header != null && !String.IsNullOrEmpty(output.Header.Type))
			{
				switch (output.Header.Type)
				{
					case ResponseType.Connection:
						output = ParseResponse<ConnectionResponse>(json);
						break;

					case ResponseType.Alerts:
						output = ParseResponse<AlertsResponse>(json);
						break;
						
					case ResponseType.Devices:
						output = ParseResponse<DevicesResponse>(json);
						break;

					case ResponseType.Device:
						output = ParseResponse<DevicesResponse>(json);
						break;

					case ResponseType.SystemError:
						output = ParseResponse<ErrorResponse>(json);
						break;

					case ResponseType.Progress:
						output = ParseResponse<ProgressResponse>(json);
						break;
				}
			}

			return output;
		}

		public static T FabricateRequestTimeoutResponse<T>(IApiRequest request) where T : IApiResponse, new()
		{
			var response = new T();

			response.Header = new RequestResponseHeader(ResponseType.TimeoutError, request.Header.Channel, request.Header.SessionId);
			response.ErrorBody = new ErrorResponseBody()
			{
				ResponseMessageShort = "Request Timeout",
				ResponseMessageLong = String.Format("The {0} request timed out on the client side. Please check your connection.", request.Header.Type),
				ErrorType = ErrorResponseType.ResponseTimeout
			};

			return response;
		}

		public static T FabricateLoginFailureResponse<T>(IApiRequest request) where T : IApiResponse, new()
		{
			var response = new T();

			response.Header = new RequestResponseHeader(ResponseType.Connection, request.Header.Channel, request.Header.SessionId);
			response.ErrorBody = new ErrorResponseBody()
			{
				ResponseMessageShort = StringLiterals.AuthFailure, //"Authentication Failure",
				ResponseMessageLong = StringLiterals.UserNotAuthenticated, //"User not authenticated.", 
				ErrorType = ErrorResponseType.AuthFailure
			};

			return response;
		}

		public static T FabricateConnectionTimeoutResponse<T>(IApiRequest request) where T : IApiResponse, new()
		{
			var response = new T();

			response.Header = new RequestResponseHeader(request.Header.Type, request.Header.Channel, request.Header.SessionId);
			response.ErrorBody = new ErrorResponseBody()
			{
				ResponseMessageShort = StringLiterals.ConnectionTimeout, //"Connection Timeout",
				ResponseMessageLong = StringLiterals.UnableToEstablishConnection, //"Unable to establish a connection with the server."),
				ErrorType = ErrorResponseType.ConnectionTimeout
			};

			return response;
		}
	}
}
