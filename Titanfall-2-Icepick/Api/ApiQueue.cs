using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Icepick.Api
{
	public class ApiResult
	{
		public string status;
		public string message;
		public Dictionary<string, string> data;
		public string rawData;
	}

    public static class ApiQueue
    {
		public delegate void ApiRequestDelegate( string apiPath );
		public delegate void ApiRequestResultDelegate( string apiPath, bool success, ApiResult result );
		public static event ApiRequestDelegate OnApiRequestIssued;
		public static event ApiRequestResultDelegate OnApiRequestResult;

		private static readonly HttpClient httpClient = new HttpClient();
		private static Queue<string> queuedRequests = new Queue<string>();

		public static async Task<ApiResult> ApiRequest( string routeName, params string[] args )
		{
			string apiPath = ApiRoutes.GetApiRoute( routeName, args );
			if( !string.IsNullOrEmpty( apiPath ) )
			{
				OnApiRequestIssued( apiPath );

				string responseString = await httpClient.GetStringAsync( apiPath );
				ApiResult result = JsonConvert.DeserializeObject<ApiResult>( responseString );
				if( result == null || string.IsNullOrEmpty( result.status ) )
				{
					return null;
				}

				bool success = result.status == "success";
				result.rawData = responseString;
				OnApiRequestResult( apiPath, success, result );
				return result;
			}
			else
			{
				OnApiRequestResult( apiPath, false, null );
				return null;
			}
		}

	}
}
