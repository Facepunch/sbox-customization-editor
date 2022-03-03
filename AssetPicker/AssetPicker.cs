using Sandbox;
using System;
using System.Collections.Generic;
using System.IO;
using Tools;

namespace CustomizationEditor;

public class AssetPicker : Widget
{

	public Action<Asset> OnAssetPicked;

	private Widget Canvas;
	private LineEdit FilterInput;

	private int assetSize => 100;
	private AssetType selectedAssetType;
	private string filterText = string.Empty;

	public AssetPicker( Widget parent = null )
		: base( parent )
	{
		CreateUI();
	}

	private void CreateUI()
	{
		SetLayout( LayoutMode.TopToBottom );
		Layout.Spacing = 10;

		var typerow = Layout.Add( new Widget( this ) );
		{
			typerow.SetLayout( LayoutMode.LeftToRight );
			typerow.Layout.Spacing = 15;
			typerow.Layout.Add( new Label( "Type", this ) );
			var combo = typerow.Layout.Add( new ComboBox( this ), 1 );
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


		var searchrow = Layout.Add( new Widget( this ) );
		{
			searchrow.SetLayout( LayoutMode.LeftToRight );
			searchrow.Layout.Spacing = 15;
			searchrow.Layout.Add( new Label( "Filter", this ) );
			FilterInput = searchrow.Layout.Add( new LineEdit( this ) );
			FilterInput.TextEdited += ( v ) =>
			{
				filterText = v;
				RebuildAssetList( GetSelectedAddon(), AssetType.All, v );
			};
		}

		var scrollArea = Layout.Add( new ScrollArea( this ), 1 );
		scrollArea.SetLayout( LayoutMode.TopToBottom );

		Canvas = scrollArea.Layout.Add( new Widget( this ), 1 );
		Canvas.SetLayout( LayoutMode.TopToBottom );
		Canvas.Layout.Spacing = 3;

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

		var row = Canvas.Layout.Add( new Widget( Canvas ), 0 );
		row.SetLayout( LayoutMode.LeftToRight );
		var idx = 0;

		// todo: qt grid layout

		row.Layout.Spacing = 3;

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
			var assetBtn = row.Layout.Add( new AssetButton( asset, assetSize, row ) );
			assetBtn.MouseClick += () =>
			{
				OnAssetPicked?.Invoke( asset );
			};

			idx++;
			if( idx % cols == 0 )
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
		Particle = 2,
		Material = 3,
	}

	private static Dictionary<AssetType, string> AssetExtensions = new()
	{
		{ AssetType.Model, ".vmdl" },
		{ AssetType.Particle, ".vpcf" },
		{ AssetType.Material, ".vmat" },
	};

}
