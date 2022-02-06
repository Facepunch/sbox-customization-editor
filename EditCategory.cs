using Tools;

namespace Facepunch.CustomizationTool;

internal class EditCategory : Widget
{

	// just using property sheet for now

	public EditCategory( CustomizationCategory cat, Widget parent = null )
		: base( parent )
	{
		var l = MakeTopToBottom();
		l.Spacing = 10;

		var ps = l.Add( new PropertySheet( this ) );
		ps.Target = cat;

		l.Add( new Label( "Choose Category Icon" ) );
		var openIconPicker = l.Add( new Button( "Find Icon", "image", this ) );
		openIconPicker.Clicked += () =>
		{
			var fd = new FileDialog( this );
			fd.Title = "Find Icon";
			fd.SetFindFile();

			if ( fd.Execute() )
			{
				cat.IconPath = CustomizationTool.Singleton.GetAddonRelativePath( fd.SelectedFile );
				ps.Target = null;
				ps.Target = cat;
			}
		};

		l.Add( new Label( "Choose Default Part" ) );
		var defaultPartCombo = l.Add( new ComboBox( this ) );
		defaultPartCombo.AddItem( "None", null, () =>
		 {
			 cat.DefaultPartId = -1;
			 ps.Target = null;
			 ps.Target = cat;
		 } );

		var selectedIdx = 0;
		var idx = 1;

		foreach( var part in CustomizationTool.Singleton.Config.Parts.Where(x => x.CategoryId == cat.Id) )
		{
			defaultPartCombo.AddItem( part.DisplayName, null, () =>
			 {
				 cat.DefaultPartId = part.Id;
				 ps.Target = null;
				 ps.Target = cat;
			 } );
			if( cat.DefaultPartId == part.Id ) selectedIdx = idx;
			idx++;
		}

		defaultPartCombo.CurrentIndex = selectedIdx;

		l.AddStretchCell( 1 );
	}

}
