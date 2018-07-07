using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Icepick.Mods
{
	public static class ModDatabase
	{
		public const string MODS_DIRECTORY = "data/mods";

		public delegate void ModDatabaseDelegate();
		public delegate void TitanfallModDelegate( TitanfallMod Mod );

		public static event ModDatabaseDelegate OnStartedLoadingMods;
		public static event TitanfallModDelegate OnModLoaded;
		public static event ModDatabaseDelegate OnFinishedLoadingMods;

		public static List<TitanfallMod> LoadedMods = new List<TitanfallMod>();

		public static void ClearDatabase()
		{
			LoadedMods.Clear();
		}

		public static void LoadAll()
		{
			if ( OnStartedLoadingMods != null )
			{
				OnStartedLoadingMods();
			}

			if ( Directory.Exists( MODS_DIRECTORY ) )
			{
				foreach ( var ModPath in Directory.GetDirectories( MODS_DIRECTORY ) )
				{
					TitanfallMod newMod = new TitanfallMod( ModPath );
					LoadedMods.Add( newMod );
					if ( OnModLoaded != null )
					{
						OnModLoaded( newMod );
					}
				}
			}

			if ( OnFinishedLoadingMods != null )
			{
				OnFinishedLoadingMods();
			}
		}
	}
}
