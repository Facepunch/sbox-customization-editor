using System;
using Tools;

namespace Facepunch.CustomizationTool;

internal class ConfirmationDialog : Window
{

	public string DisplayName { get; set; }

	public ConfirmationDialog( Widget parent, string title, string message, Action onConfirm = null, Action onCancel = null )
		: base( parent )
	{
		IsDialog = true;
		CloseButtonVisible = false;
		DeleteOnClose = true;
		ResizeButtonsVisible = false;
		Title = title;
		Size = new Vector2( 350, 128 );

		//
		var w = new Widget( null );
		var box = new BoxLayout( BoxLayout.Direction.TopToBottom, w );
		box.SetContentMargins( 0, 10, 0, 0 );

		Canvas = w;

		var layout = box.Add( new Label( message, this ) );

		var btns = new Widget( w );
		{
			var l = new BoxLayout( BoxLayout.Direction.LeftToRight, btns );
			l.AddStretchCell( 100 );
			l.SetContentMargins( 10, 10, 10, 10 );
			var cancel = l.Add( new Button( "Cancel", "cancel", btns ) );
			var confirm = l.Add( new Button( "Yes", "check", btns ) );

			cancel.Clicked += () =>
			{
				onCancel?.Invoke();
				Close();
			};

			confirm.Clicked += () =>
			{
				onConfirm?.Invoke();
				Close();
			};
		}

		box.AddStretchCell( 1 );
		box.Add( btns );
		//

		Show();
	}

}
