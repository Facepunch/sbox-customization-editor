using System;
using Tools;

namespace Facepunch.CustomizationTool;

internal class ConfirmationDialog : Dialog
{

	private Widget content;
	private Widget footer;
	private Button cancelButton;
	private Button confirmButton;

	private Action onCancel;
	private Action onConfirm;

	public ConfirmationDialog( Widget parent )
		: base( parent )
	{
		SetLayout( LayoutMode.TopToBottom );
		Layout.Margin = 10;

		Window.Title = "Confirm";
		Window.Height = 150;

		content = Layout.Add( new Widget( this ), 1 );
		content.SetLayout(LayoutMode.TopToBottom);

		footer = new Widget( this );
		{
			footer.SetLayout( LayoutMode.RightToLeft );
			footer.Layout.Spacing = 10;

			cancelButton = footer.Layout.Add( new Button( "Cancel", "cancel", footer ) );
			cancelButton.Clicked += () =>
			{
				onCancel?.Invoke();
				Close();
			};

			confirmButton = footer.Layout.Add( new Button( "Yes", "check", footer ) );
			confirmButton.Clicked += () =>
			{
				onConfirm?.Invoke();
				Close();
			};
		}

		Layout.AddStretchCell( 1 );
		Layout.Add( footer );

		Show();
	}

	public ConfirmationDialog WithTitle( string title )
	{
		Window.Title = title;
		return this;
	}

	public ConfirmationDialog WithWidget( Widget widget )
	{
		content.Children.FirstOrDefault()?.Destroy();
		content.Layout.Add( widget );
		return this;
	}

	public ConfirmationDialog WithMessage( string message )
	{
		content.Children.FirstOrDefault()?.Destroy();
		content.Layout.Add( new Label( message ) );
		return this;
	}

	public ConfirmationDialog WithConfirm( Action onConfirm, string text = null )
	{
		this.onConfirm = onConfirm;
		confirmButton.Text = text ?? confirmButton.Text;
		return this;
	}

	public ConfirmationDialog WithCancel( Action onCancel, string text = null )
	{
		this.onCancel = onCancel;
		cancelButton.Text = text ?? cancelButton.Text;
		return this;
	}

}
