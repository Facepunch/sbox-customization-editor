using System;
using Tools;

namespace Facepunch.CustomizationTool;

internal class CreateCategoryDialog : Dialog
{

	private record CreateObj( string DisplayName );

	public CreateCategoryDialog( Widget parent, Action<string> onSave = null, Action onCancel = null )
		: base( parent )
	{
		Window.Title = "Create Category";
		Window.Height = 150;

		SetLayout( LayoutMode.TopToBottom );
		Layout.Margin = 10;

		var obj = new CreateObj( "New Category" );
		Layout.Add( new CustomizationObjectForm( obj, this, false ) );

		var btns = new Widget( this );
		{
			btns.SetLayout( LayoutMode.RightToLeft );
			btns.Layout.Spacing = 10;
			var cancel = btns.Layout.Add( new Button( "Cancel", "cancel", btns ) );
			var save = btns.Layout.Add( new Button( "Create", "add", btns ) );

			cancel.Clicked += () =>
			{
				onCancel?.Invoke();
				Close();
			};

			save.Clicked += () =>
			{
				onSave?.Invoke( obj.DisplayName );
				Close();
			};
		}
		
		Layout.AddStretchCell( 1 );
		Layout.Add( btns );

		Show();
	}

}
