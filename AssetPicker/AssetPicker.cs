using Sandbox;
using System;
using System.Collections.Generic;
using System.IO;
using Tools;

public class AssetPicker : Widget
{

	private Widget Canvas;
	private BoxLayout CanvasLayout;

	public AssetPicker( Widget parent = null )
		: base( parent )
	{
		CreateUI();
	}

	private void CreateUI()
	{
		var l = this.MakeTopToBottom();

		var combo = l.Add( new ComboBox( this ) );
		foreach( var type in Enum.GetValues<AssetType>() )
		{
			combo.AddItem( type.ToString(), null, () =>
			{
				RebuildAssetList( GetSelectedAddon(), type );
			} );
		}
		combo.CurrentIndex = 0;

		Canvas = l.Add( new Widget( this ), 1 );
		CanvasLayout = Canvas.MakeTopToBottom();

		RebuildAssetList( GetSelectedAddon(), AssetType.All );

		l.AddStretchCell( 100 );
	}

	private void RebuildAssetList( LocalAddon addon, AssetType type )
	{
		foreach ( var child in Canvas.Children )
			child.Destroy();

		foreach ( var asset in AssetSystem.All
			.Where( x => BelongsToAddon( x, addon ) )
			.Where( x => ShouldShow( x ) )
			.Where( x => IsAssetType( x, type ) ) )
		{
			CanvasLayout.Add( new AssetRow( asset, Canvas ) );
		}

		CanvasLayout.AddStretchCell( 1 );
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
