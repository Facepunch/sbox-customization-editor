using System;
using System.Collections.Generic;
using System.Linq;
using Tools;

namespace Facepunch.CustomizationTool;

internal class CategoryList : Widget
{

	public Action<CustomizationCategory> OnCategorySelected;
	public Action<CustomizationPart> OnPartSelected;
	public Action OnModified;

	private Widget Canvas;
	private BoxLayout CanvasLayout;
	private BoxLayout ScrollLayout;
	private CustomizationConfig config;

	public CategoryList( CustomizationConfig config, Widget parent = null )
		: base( parent )
	{
		this.config = config;

		var lo = MakeTopToBottom();

		var newCategoryButton = lo.Add( new Button( "New Category", "add", this ) );
		newCategoryButton.Clicked += () =>
		{
			new CreateCategoryDialog( this, ( catName ) =>
			{
				config.CategoryIdAccumulator++;
				var cat = new CustomizationCategory()
				{
					DefaultPartId = 0,
					DisplayName = catName,
					Id = config.CategoryIdAccumulator
				};
				config.Categories.Add( cat );
				OnModified?.Invoke();
			} );
		};

		lo.AddSpacingCell( 10 );

		Canvas = lo.Add( new Widget( this ), 1 );
		CanvasLayout = Canvas.MakeTopToBottom();

		RefreshCategories();
	}

	private void AddCategoryWithParts( CustomizationCategory cat, IEnumerable<CustomizationPart> parts )
	{
		{
			var fuck = ScrollLayout.Add( new Fuck( true, Canvas ) );
			var rtl = fuck.MakeLeftToRight();
			rtl.Spacing = 2;

			var catBtn = rtl.Add( new Button( cat.DisplayName, "category", Canvas ) );
			catBtn.SetStyles( CategoryStyle );
			catBtn.Cursor = CursorShape.Finger;
			catBtn.Clicked += () =>
			{
				fuck.SetActive( true );
				OnCategorySelected?.Invoke( cat );
			};

			var addBtn = rtl.Add( new Button( "", "add", Canvas ) );
			addBtn.MaximumSize = new Vector2( 24, 24 );
			addBtn.SetStyles( MiniBtnStyle );
			addBtn.Cursor = CursorShape.Finger;
			addBtn.Clicked += () =>
			{
				new CreatePartDialog( this, cat, ( partName, categoryId ) =>
				{
					config.PartIdAccumulator++;
					var part = new CustomizationPart()
					{
						CategoryId = categoryId,
						DisplayName = partName,
						Id = config.PartIdAccumulator
					};
					config.Parts.Add( part );
					OnModified?.Invoke();
				} );
			};

			var delBtn = rtl.Add( new Button( "", "delete", Canvas ) );
			delBtn.MaximumSize = new Vector2( 24, 99 );
			delBtn.SetStyles( RedMiniBtnStyle );
			delBtn.Cursor = CursorShape.Finger;
			delBtn.Clicked += () =>
			{
				new ConfirmationDialog( Parent, "Delete Category", "Are you sure you wanna delete category: " + cat.DisplayName, () =>
				{
					config.Categories.Remove( cat );
					OnModified?.Invoke();
				} );
			};
		}

		foreach ( var part in parts )
		{
			var fuck = ScrollLayout.Add( new Fuck( false, Canvas ) );
			var rtl = fuck.MakeLeftToRight();
			rtl.Spacing = 2;

			var partBtn = rtl.Add( new Button( part.DisplayName, null, Canvas ) );
			partBtn.SetStyles( PartStyle );
			partBtn.Clicked += () =>
			{
				fuck.SetActive( true );
				OnPartSelected?.Invoke( part );
			};
			partBtn.Cursor = CursorShape.Finger;

			var delBtn = rtl.Add( new Button( "", "delete", Canvas ) );
			delBtn.MaximumSize = new Vector2( 24, 99 );
			delBtn.SetStyles( RedMiniBtnStyle );
			delBtn.Cursor = CursorShape.Finger;
			delBtn.Clicked += () =>
			{
				new ConfirmationDialog( Parent, "Delete Part", "Are you sure you wanna delete part: " + part.DisplayName, () =>
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

		var scroll = CanvasLayout.Add( new ScrollArea( Canvas ), 1 );
		scroll.Canvas = CanvasLayout.Add( new Widget( scroll ), 1 );

		ScrollLayout = scroll.Canvas.MakeTopToBottom();

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

		ScrollLayout.AddStretchCell( 1 );
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
