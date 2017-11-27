using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Reflection;

namespace Launcher
{
	public class ModXml : ModDocuments.ModBase
    {
		const string MOD_DOCUMENT_NAME = "mod.xml";

		public delegate void DocumentNodeDelegate( ModXml TargetMod, XElement Node );
		private Dictionary<string, DocumentNodeDelegate> DocumentNodeActions = new Dictionary<string, DocumentNodeDelegate>()
		{
			{ "Name", WriteAsFieldString },
			{ "Description", WriteAsFieldString },
			{ "Authors", WriteToFieldStringList },
			{ "Contacts", WriteToFieldStringList },
		};

		public delegate void DocumentFileDelegate( ref File Target, XElement Node );
		private Dictionary<string, DocumentFileDelegate> DocumentFileActions = new Dictionary<string, DocumentFileDelegate>()
		{
			{ "ComparisonString", WriteAsFieldString },
			{ "ReplacedCodeFile", WriteAsFieldString },
			{ "Directory", WriteAsFieldString },
		};

		public override void Load(string ModPath)
		{
			base.Load( ModPath );
				
			XDocument ModDocument = XDocument.Load( $"{Path}{System.IO.Path.DirectorySeparatorChar}{MOD_DOCUMENT_NAME}" );
			foreach ( var Node in ModDocument.Root.Elements() )
			{
				Debug.WriteLine( $"Node: {Node.Name} ({Node.Value})" );
				string NodeLocalName = Node.Name.LocalName;
				if ( DocumentNodeActions.ContainsKey( NodeLocalName ) )
				{
					DocumentNodeActions[ NodeLocalName ]( this, Node );
				}
				if( NodeLocalName == "Files" )
				{
					Files.Add( CreateFileFromXMLElement( Node ) );
				}
			}


			foreach(File f in Files)
			{
				Debug.WriteLine( $"f: {f}" );
			}
		}

		protected File CreateFileFromXMLElement( XElement FromElement )
		{
			File NewFile = new File();
			NewFile.ComparisonString = string.Empty;
			NewFile.ReplacedCodeFile = string.Empty;
			NewFile.AddressOffset = 0;

			foreach ( var Child in FromElement.Descendants() )
			{
				if ( Child.Name.LocalName == "ComparisonString" )
				{
					NewFile.ComparisonString = Child.Value;
				}
				if ( Child.Name.LocalName == "ReplacedCodeFile" )
				{
					NewFile.ReplacedCodeFile = Child.Value;
				}
				if ( Child.Name.LocalName == "AddressOffset" )
				{
					NewFile.AddressOffset = long.Parse( Child.Value );
				}
			}

			return NewFile;
		}

		public static void WriteAsFieldString( ModXml TargetMod, XElement Node )
		{
			FieldInfo Field = typeof( ModXml ).GetField( Node.Name.LocalName, BindingFlags.Instance | BindingFlags.NonPublic );
			Field.SetValue( TargetMod, Node.Value );
		}

		public static void WriteToFieldStringList( ModXml TargetMod, XElement Node )
		{
			FieldInfo Field = typeof( ModXml ).GetField( Node.Name.LocalName, BindingFlags.Instance | BindingFlags.NonPublic );
			List<string> TargetField = Field.GetValue( TargetMod ) as List<string>;
			TargetField.Add( Node.Value );
		}

		public static void WriteAsFieldString( ref File Target, XElement Node )
		{
			FieldInfo Field = typeof( ModXml ).GetField( Node.Name.LocalName, BindingFlags.Instance | BindingFlags.NonPublic );
			Field.SetValue( Target, Node.Value );
		}

	}
}
