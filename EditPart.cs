using System.IO;
using System.Linq;
using Tools;

namespace Facepunch.CustomizationTool;

internal class EditPart : Widget
{

	// just using property sheet for now

	private PropertySheet PropertySheet;

	public EditPart( CustomizationPart part, Widget parent = null )
		: base( parent )
	{
		var l = MakeTopToBottom();
		l.Spacing = 10;

		PropertySheet = l.Add( new PropertySheet( this ) );
		PropertySheet.Target = part;

		var cfg = CustomizationTool.Singleton.Config;

		l.Add( new Label( "Choose Icon/Asset" ) );
		// icon/asset picker buttons
		{
			var w = l.Add( new Widget( this ) );
			var ltr = w.MakeLeftToRight();
			ltr.Spacing = 10;

			var openIconPicker = ltr.Add( new Button( "Find Icon", "image", this ) );
			openIconPicker.Clicked += () =>
			{
				var fd = new FileDialog( this );
				fd.Title = "Find Icon";
				fd.SetFindFile();

				if ( fd.Execute() )
				{
					part.IconPath = CustomizationTool.Singleton.GetAddonRelativePath( fd.SelectedFile );
					RefreshPropertySheet();
				}
			};

			var openAssetPicker = ltr.Add( new Button( "Find Asset", "file_open", this ) );
			openAssetPicker.Clicked += () =>
			{
				var browser = new AssetBrowserWindow( this, ( asset ) =>
				{
					part.AssetPath = asset.Path;
					RefreshPropertySheet();
				} );
			};
		}

		l.Add( new Label( "Choose Category" ) );
		// category combo
		{
			var combo = l.Add( new ComboBox( this ) );
			combo.AddItem( "Uncategorized", null, () =>
			{
				part.CategoryId = -1;
				RefreshPropertySheet();
			} );

			foreach ( var cat in cfg.Categories )
			{
				combo.AddItem( cat.DisplayName, null, () =>
				{
					part.CategoryId = cat.Id;
					RefreshPropertySheet();
				} );
			}

			var partCat = cfg.Categories.FirstOrDefault( x => x.Id == part.CategoryId );
			combo.CurrentIndex = combo.FindIndex( partCat != null ? partCat.DisplayName : "Uncategorized" ) ?? 0;
		}

		l.AddStretchCell( 1 );
	}

	private void RefreshPropertySheet()
	{
		// am I missing something to magically update fields?
		var target = PropertySheet.Target;
		PropertySheet.Target = null;
		PropertySheet.Target = target;
	}

}
