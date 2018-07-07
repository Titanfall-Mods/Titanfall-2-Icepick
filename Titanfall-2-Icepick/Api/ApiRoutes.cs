using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Icepick.Api
{
    public static class ApiRoutes
    {
		private const string TitanfallModsUrl = "http://www.titanfallmods.com";
		private const string TitanfallModsApi = "http://www.titanfallmods.com/api/v1";

		private const string IcepickInfo = "{0}/icepick/info";
		private const string AllPublicMods = "{0}/mods/info/public";
		private const string SpecificMod = "{0}/mods/info/{1}";
		private const string LatestModRelease = "{0}/mods/releases/latest/{1}";
		private const string AllModReleases = "{0}/mods/releases/all/{1}";

		private const string ViewMod = "{0}/mods/{1}";

		public static string GetSite()
		{
			return TitanfallModsUrl;
		}

		public static string GetApiRoute( string routeName, params string[] args )
		{
			return GetWebsiteUrl( TitanfallModsApi, routeName, args );
		}

		public static string GetRoute( string routeName, params string[] args )
		{
			return GetWebsiteUrl( TitanfallModsUrl, routeName, args );
		}

		private static string GetWebsiteUrl( string baseUrl, string routeName, params string[] args )
		{
			FieldInfo routeField = typeof( ApiRoutes ).GetField( routeName, BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.FlattenHierarchy );
			if ( routeField == null )
			{
				return null;
			}

			string routeFormat = (string) routeField.GetRawConstantValue();
			List<string> formatArgs = new List<string>() { baseUrl };
			formatArgs.AddRange( args );
			return string.Format( routeFormat, formatArgs.ToArray() );
		}

	}
}
