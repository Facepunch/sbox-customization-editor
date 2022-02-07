using System;
using System.Collections.Generic;
using Tools;

namespace Facepunch.CustomizationTool;

internal class CreatePartDialog : Window
{

	private static CreatePartDialog singleton;

	public string DisplayName { get; set; }

	public CreatePartDialog( Widget parent, CustomizationCategory category, Action<string, int> onSave = null, Action onCancel = null )
		: base( parent )
	{
		if ( singleton != null && singleton.IsValid )
		{
			singleton.Destroy();
			singleton = null;
		}

		IsDialog = true;
		CloseButtonVisible = false;
		DeleteOnClose = true;
		ResizeButtonsVisible = false;
		Title = "New Part";
		Size = new Vector2( 350, 175 );

		//
		var w = new Widget( null );
		w.SetLayout( LayoutMode.TopToBottom );
		w.Layout.Margin = 10;

		Canvas = w;

		var ps = w.Layout.Add( new PropertySheet( w ) );
		ps.AddSectionHeader( "Creating part in category: " + category.DisplayName );
		ps.AddProperty( this, "DisplayName" );
		ps.AddStretch( 1 );

		var btns = new Widget( w );
		{
			btns.SetLayout( LayoutMode.LeftToRight );
			btns.Layout.AddStretchCell( 100 );
			btns.Layout.Margin = 10;
			var cancel = btns.Layout.Add( new Button( "Cancel", "cancel", btns ) );
			var save = btns.Layout.Add( new Button( "Save", "save", btns ) );

			cancel.Clicked += () =>
			{
				onCancel?.Invoke();
				Close();
			};

			save.Clicked += () =>
			{
				onSave?.Invoke( DisplayName, category.Id );
				Close();
			};
		}

		w.Layout.AddStretchCell( 1 );
		w.Layout.Add( btns );
		//

		Show();

		singleton = this;
	}

}
