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
		w.SetLayout( LayoutMode.TopToBottom );
		w.Layout.Margin = 10;

		Canvas = w;

		var layout = w.Layout.Add( new Label( message, this ) );

		var btns = new Widget( w );
		{
			btns.SetLayout( LayoutMode.LeftToRight );
			btns.Layout.AddStretchCell( 100 );
			btns.Layout.Margin = 10;
			var cancel = btns.Layout.Add( new Button( "Cancel", "cancel", btns ) );
			var confirm = btns.Layout.Add( new Button( "Yes", "check", btns ) );

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

		w.Layout.AddStretchCell( 1 );
		w.Layout.Add( btns );
		//

		Show();
	}

}
