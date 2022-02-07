using System;
using Tools;

namespace Facepunch.CustomizationTool;

internal class CreateCategoryDialog : Dialog
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

		Window.Title = "Create Category";
		Window.Height = 150;

		SetLayout( LayoutMode.TopToBottom );
		Layout.Margin = 10;

		var ps = Layout.Add( new PropertySheet( this ) );
		ps.AddProperty( this, "DisplayName" );
		ps.AddStretch( 1 );

		var btns = new Widget( this );
		{
			btns.SetLayout( LayoutMode.LeftToRight );
			btns.Layout.AddStretchCell( 100 );
			btns.Layout.Spacing = 10;
			var cancel = btns.Layout.Add( new Button( "Cancel", "cancel", btns ) );
			var save = btns.Layout.Add( new Button( "Save", "save", btns ) );

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

		Layout.AddStretchCell( 1 );
		Layout.Add( btns );
		//

		Show();

		singleton = this;
	}

}
