using System;
using System.Collections.Generic;
namespace Icepick.Api
{
    public static class ApiRoutes
    {
		private const string TitanfallModsUrl = "https://titanfallmods.com";
		private const string TitanfallModsDiscord = "https://discord.gg/Hw3A6ZKgy7";

		public static string GetSite()
		{
			return TitanfallModsUrl;
		}

		public static string GetDiscord()
		{
			return TitanfallModsDiscord;
		}
	}
}
