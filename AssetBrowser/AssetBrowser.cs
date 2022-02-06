using Sandbox;
using System;
using System.Collections.Generic;
using System.IO;
using Tools;

public class AssetBrowser : Widget
{

	public Action<Asset> OnAssetPicked;

	private Widget Canvas;
	private BoxLayout CanvasLayout;
	private LineEdit FilterInput;

	private int assetSize => 100;
	private AssetType selectedAssetType;
	private string filterText = string.Empty;

	public AssetBrowser( Widget parent = null )
		: base( parent )
	{
		CreateUI();
	}

	private void CreateUI()
	{
		var l = this.MakeTopToBottom();
		l.Spacing = 10;

		var typerow = l.Add( new Widget( this ) );
		{
			var ltr = typerow.MakeLeftToRight();
			ltr.Spacing = 15;
			ltr.Add( new Label( "Type", this ) );
			var combo = ltr.Add( new ComboBox( this ), 1 );
			foreach ( var type in Enum.GetValues<AssetType>() )
			{
				combo.AddItem( type.ToString(), null, () =>
				{
					selectedAssetType = type;
					RebuildAssetList( GetSelectedAddon(), type, filterText );
				} );
			}
			combo.CurrentIndex = 0;
		}


		var searchrow = l.Add( new Widget( this ) );
		{
			var ltr = searchrow.MakeLeftToRight();
			ltr.Spacing = 15;
			ltr.Add( new Label( "Filter", this ) );
			FilterInput = ltr.Add( new LineEdit( this ) );
			FilterInput.TextEdited += ( v ) =>
			{
				filterText = v;
				RebuildAssetList( GetSelectedAddon(), AssetType.All, v );
			};
		}

		var scrollArea = l.Add( new ScrollArea( this ), 1 );
		var scrollLayout = scrollArea.MakeTopToBottom();

		Canvas = scrollLayout.Add( new Widget( this ), 1 );
		CanvasLayout = Canvas.MakeTopToBottom();
		CanvasLayout.Spacing = 3;

		scrollArea.Canvas = Canvas;

		RebuildAssetList( GetSelectedAddon(), AssetType.All, filterText );
	}

	protected override void OnResize()
	{
		base.OnResize();

		RebuildAssetList( GetSelectedAddon(), selectedAssetType, filterText );
	}

	private void RebuildAssetList( LocalAddon addon, AssetType type, string search = null )
	{
		foreach ( var child in Canvas.Children )
			child.Destroy();

		var row = CanvasLayout.Add( new Widget( Canvas ), 0 );
		var layout = row.MakeLeftToRight();
		var idx = 0;

		// todo: qt grid layout

		layout.Spacing = 3;

		var assets = AssetSystem.All
			.Where( x => BelongsToAddon( x, addon ) )
			.Where( x => ShouldShow( x ) )
			.Where( x => IsAssetType( x, type ) );

		if( !string.IsNullOrEmpty( search ) )
		{
			assets = assets.Where( x => x.Path.Contains( search, StringComparison.OrdinalIgnoreCase ) );
		}

		// note: size doesn't set until after a Resize, so set a fixed col count
		// Can't get AdjustSize to work how I want, missing something?
		var cols = (int)(Canvas.Parent.Size.x / ( assetSize + 5 ));
		if ( cols == 0 ) cols = 7; 

		foreach ( var asset in assets )
		{
			var assetBtn = layout.Add( new AssetButton( asset, assetSize, row ) );
			assetBtn.MouseClick += () =>
			{
				OnAssetPicked?.Invoke( asset );
			};

			idx++;
			if( idx % cols == 0 )
			{
				layout.AddStretchCell( 1000 );
				row = CanvasLayout.Add( new Widget( Canvas ), 0 );
				layout = row.MakeLeftToRight();
				layout.Spacing = 3;
			}
		}

		layout.AddStretchCell( 1000 );

		CanvasLayout.Add( new Widget( Canvas ), 1 );
	}

	private LocalAddon GetSelectedAddon()
	{
		var defaultAddon = Cookie.GetString( "addonmanager.addon", null );
		return Utility.Addons.GetAll().FirstOrDefault( x => x.Path == defaultAddon );
	}

	private static bool BelongsToAddon( Asset asset, LocalAddon addon )
	{
		var relPath = Path.GetRelativePath( Path.GetDirectoryName( addon.Path ), asset.AbsolutePath );
		return !relPath.StartsWith( '.' ) && !Path.IsPathRooted( relPath );
	}

	private static bool ShouldShow( Asset asset )
	{
		var ext = Path.GetExtension( asset.AbsolutePath );

		if ( !AssetExtensions.ContainsValue( ext ) ) return false;

		return true;
	}

	private static bool IsAssetType( Asset asset, AssetType type )
	{
		if ( type == AssetType.All ) return true;

		if ( !AssetExtensions.ContainsKey( type ) ) return false;

		return AssetExtensions[type] == Path.GetExtension( asset.AbsolutePath );
	}

	private enum AssetType
	{
		All = 0,
		Model = 1,
		Particle = 2
	}

	private static Dictionary<AssetType, string> AssetExtensions = new()
	{
		{ AssetType.Model, ".vmdl" },
		{ AssetType.Particle, ".vpcf" },
	};

}
