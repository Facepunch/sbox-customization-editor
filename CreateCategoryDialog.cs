using System;
using Tools;

namespace Facepunch.CustomizationTool;

internal class CreateCategoryDialog : Window
{

	private static CreateCategoryDialog singleton;

	public string DisplayName { get; set; }

	public CreateCategoryDialog( Widget parent, Action<string> onSave = null, Action onCancel = null )
		: base( parent )
	{
		if( singleton != null && singleton.IsValid )
		{
			singleton.Destroy();
			singleton = null;
		}

		IsDialog = true;
		CloseButtonVisible = false;
		DeleteOnClose = true;
		ResizeButtonsVisible = false;
		Title = "New Category";
		Size = new Vector2( 350, 128 );

		//
		var w = new Widget( null );
		var box = new BoxLayout( BoxLayout.Direction.TopToBottom, w );
		var ps = box.Add( new PropertySheet( w ) );
		box.SetContentMargins( 0, 10, 0, 0 );

		Canvas = w;

		ps.AddProperty( this, "DisplayName" );
		ps.AddStretch( 1 );

		var btns = new Widget( w );
		{
			var l = new BoxLayout( BoxLayout.Direction.LeftToRight, btns );
			l.AddStretchCell( 100 );
			l.SetContentMargins( 10, 10, 10, 10 );
			var cancel = l.Add( new Button( "Cancel", "cancel", btns ) );
			var save = l.Add( new Button( "Save", "save", btns ) );

			cancel.Clicked += () =>
			{
				onCancel?.Invoke();
				Close();
			};

			save.Clicked += () =>
			{
				onSave?.Invoke( DisplayName );
				Close();
			};
		}

		box.AddStretchCell( 1 );
		box.Add( btns );
		//

		Show();

		singleton = this;
	}

}
