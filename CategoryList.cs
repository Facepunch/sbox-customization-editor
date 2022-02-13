using System;
using System.Collections.Generic;
using System.Linq;
using Tools;

namespace Facepunch.CustomizationTool;

internal class CategoryList : Widget
{

	private record CreateCategoryRecord( string DisplayName );
	private record CreatePartRecord( int CategoryId, string DisplayName );

	public Action<CustomizationCategory> OnCategorySelected;
	public Action<CustomizationPart> OnPartSelected;
	public Action OnModified;

	private Widget Canvas;
	private ScrollArea Scroll;
	private CustomizationConfig config;

	public CategoryList( CustomizationConfig config, Widget parent = null )
		: base( parent )
	{
		this.config = config;

		SetLayout( LayoutMode.TopToBottom );

		var newCategoryButton = Layout.Add( new Button( "New Category", "add", this ) );
		newCategoryButton.Clicked += () =>
		{
			CreateCategoryRecord obj = new( "New Category" );
			CustomizationObjectForm widget = new( obj, this, false );

			new ConfirmationDialog( this )
				.WithWidget( widget )
				.WithTitle( "Create a Category" )
				.WithConfirm( () =>
				 {
					 config.CategoryIdAccumulator++;
					 var cat = new CustomizationCategory()
					 {
						 DefaultPartId = 0,
						 DisplayName = obj.DisplayName,
						 Id = config.CategoryIdAccumulator
					 };
					 config.Categories.Add( cat );
					 OnModified?.Invoke();
				 }, "Create" );
		};

		Layout.AddSpacingCell( 10 );

		Canvas = Layout.Add( new Widget( this ), 1 );
		Canvas.SetLayout( LayoutMode.TopToBottom );

		RefreshCategories();
	}

	private void AddCategoryWithParts( CustomizationCategory cat, IEnumerable<CustomizationPart> parts )
	{
		{
			var fuck = Scroll.Canvas.Layout.Add( new Fuck( true, Canvas ) );
			fuck.SetLayout( LayoutMode.LeftToRight );
			fuck.Layout.Spacing = 2;

			var catBtn = fuck.Layout.Add( new Button( cat.DisplayName, "category", Canvas ) );
			catBtn.SetStyles( CategoryStyle );
			catBtn.Cursor = CursorShape.Finger;
			catBtn.Clicked += () =>
			{
				fuck.SetActive( true );
				OnCategorySelected?.Invoke( cat );
			};

			var addBtn = fuck.Layout.Add( new Button( "", "add", Canvas ) );
			addBtn.MaximumSize = new Vector2( 24, 24 );
			addBtn.SetStyles( MiniBtnStyle );
			addBtn.Cursor = CursorShape.Finger;
			addBtn.Clicked += () =>
			{
				CreatePartRecord obj = new( cat.Id, "New Category" );
				CustomizationObjectForm widget = new( obj, this, false );

				new ConfirmationDialog( this )
					.WithWidget( widget )
					.WithTitle( "Create a Category" )
					.WithConfirm( () =>
					{
						config.PartIdAccumulator++;
						var part = new CustomizationPart()
						{
							CategoryId = cat.Id,
							DisplayName = obj.DisplayName,
							Id = config.PartIdAccumulator
						};
						config.Parts.Add( part );
						OnModified?.Invoke();
					}, "Create" );
			};

			var delBtn = fuck.Layout.Add( new Button( "", "delete", Canvas ) );
			delBtn.MaximumSize = new Vector2( 24, 99 );
			delBtn.SetStyles( RedMiniBtnStyle );
			delBtn.Cursor = CursorShape.Finger;
			delBtn.Clicked += () =>
			{
				new ConfirmationDialog( Parent )
					.WithTitle( "Delete Category" )
					.WithMessage( "Are you sure you wanna delete category: " + cat.DisplayName )
					.WithConfirm( () =>
					{
						config.Categories.Remove( cat );
						OnModified?.Invoke();
					} );
			};
		}

		foreach ( var part in parts )
		{
			var fuck = Scroll.Canvas.Layout.Add( new Fuck( false, Canvas ) );
			fuck.SetLayout( LayoutMode.LeftToRight );
			fuck.Layout.Spacing = 2;

			var partBtn = fuck.Layout.Add( new Button( part.DisplayName, null, Canvas ) );
			partBtn.SetStyles( PartStyle );
			partBtn.Clicked += () =>
			{
				fuck.SetActive( true );
				OnPartSelected?.Invoke( part );
			};
			partBtn.Cursor = CursorShape.Finger;

			var delBtn = fuck.Layout.Add( new Button( "", "delete", Canvas ) );
			delBtn.MaximumSize = new Vector2( 24, 99 );
			delBtn.SetStyles( RedMiniBtnStyle );
			delBtn.Cursor = CursorShape.Finger;
			delBtn.Clicked += () =>
			{
				new ConfirmationDialog( Parent )
					.WithTitle( "Delete Part" )
					.WithMessage( "Are you sure you wanna delete part: " + part.DisplayName )
					.WithConfirm( () =>
					{
						config.Parts.Remove( part );
						OnModified?.Invoke();
					} );
			};
		}
	}

	public void RefreshCategories()
	{
		foreach ( var child in Canvas.Children )
			child.Destroy();

		Scroll = Canvas.Layout.Add( new ScrollArea( Canvas ), 1 );
		Scroll.SetLayout( LayoutMode.TopToBottom );
		Scroll.Canvas = Canvas.Layout.Add( new Widget( Scroll ), 1 );
		Scroll.Canvas.SetLayout( LayoutMode.TopToBottom );

		var uncategorizedParts = config.Parts.Where( x => config.Categories.FirstOrDefault( y => y.Id == x.CategoryId ) == null );
		if( uncategorizedParts.Count() > 0 )
		{
			var cat = new CustomizationCategory() { DisplayName = "Uncategorized", Id = -1 };
			AddCategoryWithParts( cat, uncategorizedParts );
		}

		foreach ( var cat in config.Categories )
		{
			var parts = config.Parts.Where( x => x.CategoryId == cat.Id );
			AddCategoryWithParts( cat, parts );
		}

		Scroll.Layout.AddStretchCell( 1 );
	}

	private const string CategoryStyle = @"
Button {
	padding: 6px;
	border: 0;
	background: none;
	text-align: left;
	padding-left: 10px;
}
";

	private const string PartStyle = @"
Button {
	padding: 5px;
	padding-left: 12px;
	padding-bottom: 8px;
	text-align: left;
	border: 0;
}
";

	private const string MiniBtnStyle = @"
Button {
	background-color: transparent; 
	text-align: center;
	border-radius: 4px;
	padding: 0;
}
Button:hover {
	background-color: #3c576b;
}
";

	private const string RedMiniBtnStyle = MiniBtnStyle + @"
Button:hover {
	background-color: #6b3c3c;
}
";

}

internal class Fuck : Widget
{

	private bool isHeader;
	private static Fuck activeFuck;

	public Fuck( bool isHeader, Widget parent = null ) : base( parent )
	{
		this.isHeader = isHeader;

		SetActive( false );
	}

	public override void OnDestroyed()
	{
		base.OnDestroyed();

		if ( activeFuck == this ) activeFuck = null;
	}

	public void SetActive( bool active )
	{
		if ( activeFuck != null && activeFuck.IsValid && activeFuck != this ) 
			activeFuck.SetActive( false );

		var d = isHeader ? HeaderFuckStyle : "";

		SetStyles( ( active ? ActiveFuckStyle : FuckStyle ) + d );

		activeFuck = this;
	}

	private const string FuckStyle = @"
Fuck {
	border-radius: 4px;
	margin-bottom: 2px;
	padding:8px;
}

Fuck:hover {
	border: 1px solid #3c576b;
}
";

	private const string HeaderFuckStyle = @"
Fuck {
	background-color: black;
}
";

	private const string ActiveFuckStyle = FuckStyle + @"
Fuck {
	border: 1px solid #40874b;
	background-color: #3c576b;
}

Fuck:hover {
	border: 1px solid #40874b;
}
";

}
