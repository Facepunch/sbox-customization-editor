
using Sandbox;
using System.Collections.Generic;
using System.IO;
using Tools;

namespace CustomizationEditor;

internal class ImageButton : Widget
{

	private readonly string path;
	private bool mouseDown;
	private ImagePicker picker;

	private static ImageButton picked;

	public ImageButton( string path, int size, ImagePicker picker, Widget parent = null )
		: base( parent )
	{
		this.path = path;
		this.picker = picker;
		this.MinimumSize = size;
		this.Cursor = CursorShape.Finger;
		this.ToolTip = CustomizationTool.Singleton.GetAddonRelativePath( path );
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

		var thumb = picker.GetThumb( path );
		if ( thumb != null )
		{
			Paint.Draw( r.Expand( -4, -4 ), thumb );
		}

		Paint.SetDefaultFont();
		Paint.SetPen( Theme.White, 2 );
		Paint.DrawText( r.Expand( -4, -4 ), Path.GetFileName( path ), TextFlag.LeftBottom );
	}

}
