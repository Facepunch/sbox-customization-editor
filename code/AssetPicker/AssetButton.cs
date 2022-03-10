
using Sandbox;
using Tools;

namespace CustomizationEditor;

internal class AssetButton : Widget
{

	private readonly Asset asset;
	private bool mouseDown;

	private static AssetButton picked;

	public AssetButton( Asset asset, int size, Widget parent = null )
		: base( parent )
	{
		this.asset = asset;
		this.MinimumSize = size;
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

		if ( picked?.IsValid ?? false )
			picked.Update();

		picked = this;

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

		var r = new Rect( 0, 0, MinimumSize.x, MinimumSize.y );
		var border = IsUnderMouse ? Theme.Green : Color.Transparent;
		border = mouseDown ? Theme.White : border;
		border = picked == this ? Theme.Green : border;

		Paint.Antialiasing = true;
		Paint.BilinearFiltering = true;
		Paint.SetPen( border, 2 );
		Paint.SetBrush( Theme.Black );
		Paint.DrawRect( r, 4 );

		var thumb = asset.GetAssetThumb();
		if( thumb != null )
		{
			Paint.Draw( r.Expand( -4, -4 ), thumb );
		}

		Paint.SetDefaultFont();
		Paint.SetPen( Theme.White, 2 );
		Paint.DrawText( r.Expand(-4, -4), asset.Name, TextFlag.LeftBottom );
	}

}
