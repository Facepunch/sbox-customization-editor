using System;
using System.Collections.Generic;
using Tools;

namespace Facepunch.CustomizationTool;

internal class CreatePartDialog : Dialog
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

		Window.Title = "New Part";
		Window.Height = 200;

		SetLayout( LayoutMode.TopToBottom );
		Layout.Margin = 10;

		var ps = Layout.Add( new PropertySheet( this ) );
		var pr = new PropertyRow( this );
		var catLineEdit = new LineEdit( this );
		catLineEdit.Text = category.DisplayName;
		catLineEdit.ReadOnly = true;
		pr.SetLabel( "Category" );
		pr.SetWidget( catLineEdit );

		ps.AddRow( pr );
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
				onSave?.Invoke( DisplayName, category.Id );
				Close();
			};
		}

		Layout.AddStretchCell( 1 );
		Layout.Add( btns );

		Show();

		singleton = this;
	}

}
