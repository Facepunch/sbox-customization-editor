using System;
using Tools;

namespace Facepunch.CustomizationTool;

internal class ConfirmationDialog : Dialog
{

	public string DisplayName { get; set; }

	public ConfirmationDialog( Widget parent, string title, string message, Action onConfirm = null, Action onCancel = null )
		: base( parent )
	{
		SetLayout( LayoutMode.TopToBottom );
		Layout.Margin = 10;

		Window.Title = title;
		Window.Height = 150;

		var label = Layout.Add( new Label( message, this ) );

		var btns = new Widget( this );
		{
			btns.SetLayout( LayoutMode.RightToLeft );
			btns.Layout.Spacing = 10;

			btns.Layout.Add( new Button( "Cancel", "cancel", btns ) ).Clicked += () =>
			{
				onCancel?.Invoke();
				Close();
			};

			btns.Layout.Add( new Button( "Yes", "check", btns ) ).Clicked += () =>
			{
				onConfirm?.Invoke();
				Close();
			};
		}

		Layout.AddStretchCell( 1 );
		Layout.Add( btns );

		Show();
	}

}
