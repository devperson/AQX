using System;
using System.Collections.Generic;

using Aquamonix.Mobile.Lib.Utilities;

namespace Aquamonix.Mobile.Lib.Environment
{
    /// <summary>
    /// Some server urls can be 'named' rather than specified by actual url. This class is a utility for translating between 
    /// urls and their aliases.
    /// </summary>
	public static class ServerAliases
	{
		private static List<AliasSpec> _aliases = new List<AliasSpec>(); 

		static ServerAliases()
		{
			//TODO: get these from config file, not hard-code
			_aliases.Add(new AliasSpec("EngServer", "wss://raincloud.aquamonix.com.au:60000/service"));
			_aliases.Add(new AliasSpec("DevServer", "wss://raincloud.aquamonix.com.au:60001/service"));
			_aliases.Add(new AliasSpec("RainCloud_demo2", "ws://mce-appdemo.dyndns.info:60444/service"));
            _aliases.Add(new AliasSpec("RainCloud_demo", "ws://172.16.2.12:60444/service"));
            _aliases.Add(new AliasSpec("RainCloud", "wss://raincloud.aquamonix.com.au:444/service"));
            _aliases.Add(new AliasSpec("Dev Server", "ws://mce-appdemo.dyndns.info:40003/service"));
            _aliases.Add(new AliasSpec("Upton", "wss://raincloud.aquamonix.com.au:60446/service"));
            _aliases.Add(new AliasSpec("PivotPro", "wss://raincloud.aquamonix.com.au:60447/service"));

            //local nodejs mock server (for testing) 
            _aliases.Add(new AliasSpec("LocalMock", "ws://192.168.1.40:8086"));
        }

        /// <summary>
        /// Converts an alias to a url.
        /// </summary>
        /// <param name="alias">The case-insensitive alias</param>
        /// <returns>A url or, if none is found, the given string.</returns>
		public static string AliasToUri(string alias)
		{
			return ExceptionUtility.Try<string>(() =>
			{
				if (String.IsNullOrEmpty(alias))
					alias = String.Empty;

				alias = alias.Trim().RemoveWhitespace().ToLower();

				foreach (var a in _aliases)
				{
					if (a.Alias == alias)
						return a.Uri;
				}

				return alias;
			}); 
		}

        /// <summary>
        /// Converts a url to its alias.
        /// </summary>
        /// <param name="uri">The url to convert.</param>
        /// <returns>The corresponding alias, or the original url if none found.</returns>
		public static string UriToAlias(string uri)
		{
			return ExceptionUtility.Try<string>(() =>
			{
				if (String.IsNullOrEmpty(uri))
					uri = String.Empty;

				uri = uri.Trim().ToLower();

				foreach (var a in _aliases)
				{
					if (a.Uri == uri)
						return a.DisplayAlias;
				}

				return uri; 
			});
		}

		private class AliasSpec
		{
			public string Uri { get; set;}
			public string Alias { get; set;}
			public string DisplayAlias { get; private set; }

			public AliasSpec(string displayAlias, string uri)
			{
				this.Uri = uri;
				this.DisplayAlias = displayAlias;
				this.Alias = displayAlias.RemoveWhitespace().Trim().ToLower();
			}
		}
	}
}
