
using Tools;

internal class AssetRow : Widget
{

	private readonly Asset asset;
	private bool mouseDown;

	public AssetRow( Asset asset, Widget parent = null )
		: base( parent )
	{
		this.asset = asset;
		this.MinimumSize = 96;
		this.Cursor = CursorShape.Finger;
		this.ToolTip = asset.Path;
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

		if ( !e.LeftMouseButton ) return;

		mouseDown = true;

		Update();
	}

	protected override void OnMouseReleased( MouseEvent e )
	{
		base.OnMouseReleased( e );

		if ( !e.LeftMouseButton ) return;

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
		Paint.DrawRect( r, 4 );
		Paint.Draw( r, asset.GetAssetThumb() );

		Paint.SetDefaultFont();
		Paint.SetPen( Theme.White, 2 );
		Paint.DrawText( r.Expand(-4, -4), asset.Name, TextFlag.LeftBottom );
	}

}
