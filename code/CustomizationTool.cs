using Sandbox;
using System.IO;
using System.Linq;
using System.Text.Json;
using Tools;

namespace CustomizationEditor;

[Tool( "Customization", "science", "Manage a gamemode's customization shit" )]
public class CustomizationTool : Window
{

	private CategoryList categoryList;
	private ObjectForm displayedForm;

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
		w.SetLayout( LayoutMode.LeftToRight );
		w.Layout.Margin = 10;
		w.Layout.Spacing = 10;

		Canvas = w;
		Addon = GetSelectedAddon();

		if( Addon == null )
		{
			w.Layout.Add( new Label( "Select an addon in addon manager", this ) );
			w.Layout.AddStretchCell( 1 );
			return;
		}

		Title = Addon.Config.Title + " Customization Parts";
		Config = LoadConfig();

		var sidebar = w.Layout.Add( new Widget( this ), 1 );
		sidebar.SetLayout( LayoutMode.TopToBottom );
		sidebar.MaximumSize = new Vector2( 250, 9999 );

		var content = w.Layout.Add( new Widget( this ), 1 );
		content.SetLayout( LayoutMode.TopToBottom );

		categoryList = sidebar.Layout.Add( new CategoryList( Config, sidebar ) );
		categoryList.OnCategorySelected += ( cat ) =>
		{
			displayedForm?.Destroy();
			displayedForm = content.Layout.Add( new ObjectForm( cat, content ) );
			displayedForm.OnSave += () =>
			{
				SaveConfig();
				categoryList.RefreshCategories();
			};
		};

		categoryList.OnPartSelected += ( part ) =>
		{
			displayedForm?.Destroy();
			displayedForm = content.Layout.Add( new ObjectForm( part, content ) );
			displayedForm.OnSave += () =>
			{
				SaveConfig();
				categoryList.RefreshCategories();
			};
		};

		categoryList.OnModified += () =>
		{
			// todo: dirty, press save?
			SaveConfig();
			categoryList.RefreshCategories();
		};
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
