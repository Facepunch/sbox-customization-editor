using System;
using Tools;

namespace Facepunch.CustomizationTool;

internal class CreatePartDialog : Dialog
{

	private class CreatePartObject 
	{
		[CustomizationObjectForm.CategoryDropdown]
		public int CategoryId { get; set; }
		public string DisplayName { get; set; }
	}

	public CreatePartDialog( Widget parent, CustomizationCategory category, Action<string, int> onSave = null, Action onCancel = null )
		: base( parent )
	{
		Window.Title = "New Part";
		Window.Height = 200;

		SetLayout( LayoutMode.TopToBottom );
		Layout.Margin = 10;

		var obj = new CreatePartObject() { DisplayName = "New Part", CategoryId = category.Id };
		Layout.Add( new CustomizationObjectForm( obj, this, false ) );

		var btns = new Widget( this );
		{
			btns.SetLayout( LayoutMode.RightToLeft );
			btns.Layout.Spacing = 10;

			btns.Layout.Add( new Button( "Cancel", "cancel", btns ) ).Clicked += () =>
			{
				onCancel?.Invoke();
				Close();
			};

			btns.Layout.Add( new Button( "Create", "add", btns ) ).Clicked += () =>
			{
				onSave?.Invoke( obj.DisplayName, obj.CategoryId );
				Close();
			};
		}

		Layout.AddStretchCell( 1 );
		Layout.Add( btns );

		Show();
	}

}
