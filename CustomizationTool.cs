using Sandbox;
using System.IO;
using System.Text.Json;
using Tools;

namespace Facepunch.CustomizationTool;

[Tool( "Customization", "science", "Manage a gamemode's customization shit" )]
public class CustomizationTool : Window
{

	private CategoryList categoryList;
	private Widget displayedConfigForm;

	public static CustomizationTool Singleton;

	public CustomizationConfig Config { get; private set; }
	public LocalAddon Addon { get; private set; }

	public CustomizationTool()
	{
		if ( Singleton != null && Singleton.IsValid )
		{
			if(Singleton.IsMinimized)
			{
				Singleton.MakeMaximized();
			}
			Singleton.CreateUI();
			Singleton.Focus();
			Destroy();
			return;
		}

		Title = "Customization Parts";
		Size = new Vector2( 1000, 650 );

		CreateUI();
		Show();

		Singleton = this;
	}

	public override void OnDestroyed()
	{
		base.OnDestroyed();

		if ( Singleton == this ) Singleton = null;
	}

	private void BuildMenuBar()
	{
		var menu = MenuBar.AddMenu( "File" );
		menu.AddOption( "Save" ).Triggered += () =>
		{
			SaveConfig();
			categoryList?.RefreshCategories();
		};
		menu.AddOption( "Quit" ).Triggered += Close;
	}

	public void CreateUI()
	{
		Clear();

		BuildMenuBar();

		var w = new Widget( null );
		var lo = new BoxLayout( BoxLayout.Direction.LeftToRight, w );
		lo.SetContentMargins( 10, 10, 10, 10 );
		lo.Spacing = 10;

		Canvas = w;
		Addon = GetSelectedAddon();

		if( Addon == null )
		{
			lo.Add( new Label( "Select an addon in addon manager", this ) );
			lo.AddStretchCell( 1 );
			return;
		}

		Title = Addon.Config.Title + " Customization Parts";
		Config = LoadConfig();

		var sidebar = lo.Add( new Widget( this ), 1 );
		var sidebarLayout = sidebar.MakeTopToBottom();
		sidebar.MaximumSize = new Vector2( 250, 9999 );

		var content = lo.Add( new Widget( this ), 1 );
		var contentLayout = content.MakeTopToBottom();

		var tbtn = sidebarLayout.Add( new Button( "asset picker test", "science", this ) );
		tbtn.Clicked += () =>
		{
			displayedConfigForm?.Destroy();
			displayedConfigForm = contentLayout.Add( new AssetPicker( this ) );
		};

		categoryList = sidebarLayout.Add( new CategoryList( Config, sidebar ) );
		categoryList.OnCategorySelected += ( cat ) =>
		{
			displayedConfigForm?.Destroy();
			displayedConfigForm = contentLayout.Add( new EditCategory( cat, content ) );
		};

		categoryList.OnPartSelected += ( part ) =>
		{
			displayedConfigForm?.Destroy();
			displayedConfigForm = contentLayout.Add( new EditPart( part, content ) );
		};

		categoryList.OnModified += () =>
		{
			// todo: dirty, press save?
			SaveConfig();
			categoryList.RefreshCategories();
		};

		sidebarLayout.AddSpacingCell( 16 );

		var saveBtn = sidebarLayout.Add( new Button( "Save Changes", "save", this ) );
		saveBtn.SetStyles( "Button { background-color: red; }" );
		saveBtn.Clicked += () =>
		{
			// todo: dirty, press save?
			SaveConfig();
			categoryList.RefreshCategories();
		};

		displayedConfigForm = contentLayout.Add( new AssetPicker( this ) );
	}

	protected override void OnPaint()
	{
		base.OnPaint();

		if ( GetSelectedAddon() != Addon )
		{
			CreateUI();
		}
	}

	[Sandbox.Event.Hotload]
	public void OnHotload()
	{
		CreateUI();
	}

	private void SaveConfig()
	{
		if ( Addon == null || Config == null ) throw new System.Exception( "Addon or config null" );

		var filePath = Path.Combine( Path.GetDirectoryName( Addon.Path ), "config", "customization.json" );
		var json = JsonSerializer.Serialize( Config );
		File.WriteAllText( filePath, json );
	}

	private CustomizationConfig LoadConfig()
	{
		if ( Addon == null ) throw new System.Exception( "Missing addon" );

		var filePath = Path.Combine( Path.GetDirectoryName( Addon.Path ), "config", "customization.json" );
		if ( File.Exists( filePath ) )
		{
			try
			{
				return JsonSerializer.Deserialize<CustomizationConfig>( File.ReadAllText( filePath ) );
			}
			catch ( System.Exception e )
			{
				Log.Error( "Problem deserializing customization config: " + e.Message );
			}
		}

		return new();
	}

	private LocalAddon GetSelectedAddon()
	{
		var defaultAddon = Cookie.GetString( "addonmanager.addon", null );
		return Utility.Addons.GetAll().FirstOrDefault( x => x.Path == defaultAddon );
	}

	public string GetAddonRelativePath( string path )
	{
		var dir = Path.GetDirectoryName( Addon.Path );
		return Path.GetRelativePath( dir, path );
	}

}
