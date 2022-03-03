using Facepunch.CustomizationTool;
using Sandbox;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using Tools;

//
// todo: this is basically the same as AssetPicker,
// some things can be reused
//

public class ImagePicker : Widget
{

	public Action<string> OnImagePicked;

	private Widget Canvas;
	private LineEdit FilterInput;

	private int assetSize => 100;
	private string filterText = string.Empty;

	private Dictionary<string, Pixmap> thumbcache = new();

	public ImagePicker( Widget parent = null )
		: base( parent )
	{
		CreateUI();
	}

	private void CreateUI()
	{
		SetLayout( LayoutMode.TopToBottom );
		Layout.Spacing = 10;

		var searchrow = Layout.Add( new Widget( this ) );
		{
			searchrow.SetLayout( LayoutMode.LeftToRight );
			searchrow.Layout.Spacing = 15;
			searchrow.Layout.Add( new Label( "Filter", this ) );
			FilterInput = searchrow.Layout.Add( new LineEdit( this ) );
			FilterInput.TextEdited += ( v ) =>
			{
				filterText = v;
				RebuildAssetList( GetSelectedAddon(), v );
			};
		}

		var scrollArea = Layout.Add( new ScrollArea( this ), 1 );
		scrollArea.SetLayout( LayoutMode.TopToBottom );

		Canvas = scrollArea.Layout.Add( new Widget( this ), 1 );
		Canvas.SetLayout( LayoutMode.TopToBottom );
		Canvas.Layout.Spacing = 3;

		scrollArea.Canvas = Canvas;

		RebuildAssetList( GetSelectedAddon(), filterText );
	}

	protected override void OnResize()
	{
		base.OnResize();

		RebuildAssetList( GetSelectedAddon(), filterText );
	}

	private void RebuildAssetList( LocalAddon addon, string search = null )
	{
		foreach ( var child in Canvas.Children )
			child.Destroy();

		var row = Canvas.Layout.Add( new Widget( Canvas ), 0 );
		row.SetLayout( LayoutMode.LeftToRight );
		var idx = 0;

		// todo: qt grid layout

		row.Layout.Spacing = 3;

		var addonpath = Path.GetDirectoryName( GetSelectedAddon().Path );
		var files = Directory.GetFiles( addonpath, "*.*", SearchOption.AllDirectories );

		var imageFiles = new List<string>();
		foreach ( string filename in files )
		{
			if ( Regex.IsMatch( filename, @"\.jpg$|\.png$|\.gif$" ) )
				imageFiles.Add( filename );
		}

		if ( !string.IsNullOrEmpty( search ) )
		{
			imageFiles = imageFiles.Where( x => x.Contains( search, StringComparison.OrdinalIgnoreCase ) ).ToList();
		}

		// note: size doesn't set until after a Resize, so set a fixed col count
		// Can't get AdjustSize to work how I want, missing something?
		var cols = (int)(Canvas.Parent.Size.x / (assetSize + 5));
		if ( cols == 0 ) cols = 7;

		foreach ( var image in imageFiles )
		{
			var assetBtn = row.Layout.Add( new ImageButton( image, assetSize, this, row ) );
			assetBtn.MouseClick += () =>
			{
				OnImagePicked?.Invoke( CustomizationTool.Singleton.GetAddonRelativePath( image ) );
			};

			idx++;
			if ( idx % cols == 0 )
			{
				row.Layout.AddStretchCell( 1000 );
				row = Canvas.Layout.Add( new Widget( Canvas ), 0 );
				row.SetLayout( LayoutMode.LeftToRight );
				row.Layout.Spacing = 3;
			}
		}

		row.Layout.AddStretchCell( 1000 );

		Canvas.Layout.Add( new Widget( Canvas ), 1 );
	}

	private LocalAddon GetSelectedAddon()
	{
		var defaultAddon = Cookie.GetString( "addonmanager.addon", null );
		return Utility.Addons.GetAll().FirstOrDefault( x => x.Path == defaultAddon );
	}

	public Pixmap GetThumb( string path )
	{
		if ( !thumbcache.ContainsKey( path ) )
		{
			thumbcache[path] = Pixmap.FromFile( path );
		}
		return thumbcache[path];
	}

}
