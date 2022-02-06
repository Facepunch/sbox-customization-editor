
using Tools;

internal class AssetRow : Widget
{

	public AssetRow( Asset asset, Widget parent = null )
		: base( parent )
	{
		var l = MakeTopToBottom();
		l.Add( new Label( asset.Path, this ) );
	}

}
