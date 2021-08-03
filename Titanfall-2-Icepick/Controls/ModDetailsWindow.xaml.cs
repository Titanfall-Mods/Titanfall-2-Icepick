using Icepick.Mods;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace Icepick.Controls
{
	/// <summary>
	/// Interaction logic for ModDetailsWindow.xaml
	/// </summary>
	public partial class ModDetailsWindow : Window
	{
		public ModDetailsWindow( ModItem parent )
		{
			InitializeComponent();

			this.Title = parent.ModName + " Details";
			ModNameLabel.Content = parent.ModName;
			ModDescriptionLabel.Content = parent.ModDescription;
			ModDisplayImage.Source = parent.ModImageSource;

			if( parent.Mod != null )
			{
				if ( parent.Mod.Definition != null )
				{
					AddAuthors( parent.Mod.Definition );
					AddContacts( parent.Mod.Definition );
					AddVersion( parent.Mod.Definition );
				}

				AddErrors( parent.Mod );
				AddWarnings( parent.Mod );
			}
		}

		protected void AddAuthors( TitanfallModDefinition definition )
		{
			if ( definition.Authors != null && definition.Authors.Count > 0 )
			{
				TextBlock title = new TextBlock();
				title.Inlines.Add( new Bold( new Run( "Authors" ) ) );
				DetailsPanel.Children.Add( title );

				foreach ( string author in definition.Authors )
				{
					TextBlock authorBlock = new TextBlock();
					authorBlock.Text = author;
					DetailsPanel.Children.Add( authorBlock );
				}
			}
		}

		protected void AddContacts( TitanfallModDefinition definition )
		{
			if ( definition.Contacts != null && definition.Contacts.Count > 0 )
			{
				TextBlock title = new TextBlock();
				title.Inlines.Add( new Bold( new Run( "Contact Details" ) ) );
				DetailsPanel.Children.Add( title );

				foreach ( string contact in definition.Contacts )
				{
					TextBlock contactBlock = new TextBlock();
					contactBlock.Text = contact;
					DetailsPanel.Children.Add( contactBlock );
				}
			}
		}

		protected void AddVersion( TitanfallModDefinition definition )
		{
			if ( definition.Contacts != null && definition.Contacts.Count > 0 )
			{
				TextBlock title = new TextBlock();
				title.Inlines.Add( new Bold( new Run( "Version" ) ) );
				DetailsPanel.Children.Add( title );

				TextBlock versionBlock = new TextBlock();
				versionBlock.Text = definition.Version;
				DetailsPanel.Children.Add( versionBlock );
			}
		}

		protected void AddErrors( TitanfallMod mod )
		{
			List<string> errors = mod.GetErrors();
			if( errors.Count > 0 )
			{
				TextBlock title = new TextBlock();
				title.Inlines.Add( new Bold( new Run( "Errors" ) ) );
				DetailsPanel.Children.Add( title );

				foreach ( string error in errors )
				{
					TextBlock errorBlock = new TextBlock();
					errorBlock.Text = error;
					DetailsPanel.Children.Add( errorBlock );
				}
			}
		}

		protected void AddWarnings( TitanfallMod mod )
		{
			List<string> warnings = mod.GetWarnings();
			if ( warnings.Count > 0 )
			{
				TextBlock title = new TextBlock();
				title.Inlines.Add( new Bold( new Run( "Warnings" ) ) );
				DetailsPanel.Children.Add( title );

				foreach ( string warning in warnings )
				{
					TextBlock warningBlock = new TextBlock();
					warningBlock.Text = warning;
					DetailsPanel.Children.Add( warningBlock );
				}
			}
		}

	}
}
