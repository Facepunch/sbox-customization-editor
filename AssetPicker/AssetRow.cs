
using Tools;

internal class AssetRow : Widget
{

	private Asset asset;
	private bool mouseDown;

	public AssetRow( Asset asset, Widget parent = null )
		: base( parent )
	{
		this.asset = asset;
		this.MinimumSize = 96;
		this.Cursor = CursorShape.Finger;
	}

	protected override void OnMouseEnter()
	{
		base.OnMouseEnter();

		Update();
	}

	protected override void OnMouseLeave()
	{
		base.OnMouseLeave();

		Update();
	}

	protected override void OnMousePress( MouseEvent e )
	{
		base.OnMousePress( e );

		mouseDown = true;

		Update();
	}

	protected override void OnMouseReleased( MouseEvent e )
	{
		base.OnMouseReleased( e );

		mouseDown = false;

		Update();
	}

	protected override void OnPaint()
	{
		base.OnPaint();

		var r = new Rect( 0, 0, 96, 96 );
		var border = IsUnderMouse ? Theme.Green : Color.Transparent;
		border = mouseDown ? Theme.White : border;

		Paint.Antialiasing = true;
		Paint.BilinearFiltering = true;
		Paint.SetPen( border, 2 );
		Paint.SetBrush( Theme.Black );
		Paint.DrawRect( r, 8 );
		Paint.Draw( r, asset.GetAssetThumb() );
	}

}
