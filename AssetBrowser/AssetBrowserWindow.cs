using System;
using Tools;

public class AssetBrowserWindow : Window
{

	public AssetBrowserWindow( Widget parent = null, Action<Asset> onAssetPicked = null )
		: base( parent )
	{
		Size = new Vector2( 756, 500 );
		Title = "Asset Browser";
		Canvas = new Widget( this );
		Canvas.SetLayout( LayoutMode.TopToBottom );
		var browser = Canvas.Layout.Add( new AssetBrowser( this ) );
		browser.OnAssetPicked += ( asset ) =>
		{
			onAssetPicked?.Invoke( asset );
			Close();
		};

		Show();
	}

}
