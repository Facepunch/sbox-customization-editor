using System;
using System.Linq;
using Tools;

namespace CustomizationEditor;

public class ConfirmDialog : Dialog
{

	private Widget content;
	private Widget footer;
	private Button cancelButton;
	private Button confirmButton;

	private Action onCancel;
	private Action onConfirm;

	public ConfirmDialog( Widget parent )
		: base( parent )
	{
		SetLayout( LayoutMode.TopToBottom );
		Layout.Margin = 10;

		Window.Title = "Confirm";
		Window.Size = new Vector2( 500, 175 );
		
		content = Layout.Add( new Widget( this ) );
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

	public ConfirmDialog WithTitle( string title )
	{
		Window.Title = title;
		return this;
	}

	public ConfirmDialog WithMessage( string message ) => WithWidget( new Label( message ) );
	public ConfirmDialog WithWidget( Widget widget )
	{
		content.Children.FirstOrDefault()?.Destroy();
		content.Layout.Add( widget );
		return this;
	}

	public ConfirmDialog WithConfirm( Action onConfirm, string text = null )
	{
		this.onConfirm = onConfirm;
		confirmButton.Text = text ?? confirmButton.Text;
		return this;
	}

	public ConfirmDialog WithCancel( Action onCancel, string text = null )
	{
		this.onCancel = onCancel;
		cancelButton.Text = text ?? cancelButton.Text;
		return this;
	}

	public ConfirmDialog WithSize( int width, int height )
	{
		Window.Size = new Vector2( width, height );
		return this;
	}

}
