using System;
using System.Reflection;
using System.Text.RegularExpressions;
using Tools;

namespace Facepunch.CustomizationTool;

public class ObjectForm : Widget
{

	public Action OnDirty;
	public Action OnSave;

	private Button saveButton;

	public ObjectForm( object obj, Widget parent, bool saveBtn = true )
		: base( parent )
	{
		if ( obj == null ) throw new NullReferenceException();

		SetLayout( LayoutMode.TopToBottom );
		Layout.Spacing = 5;

		foreach( var prop in obj.GetType().GetProperties() )
		{
			if ( !CanDisplayProperty( prop ) ) continue;

			var t = prop.PropertyType;

			var row = Layout.Add( new Widget( this ) );
			row.SetLayout( LayoutMode.LeftToRight );
			row.MinimumSize = Theme.RowHeight;
			row.Layout.Spacing = 5;

			var label = row.Layout.Add( new Label( GetFriendlyPropertyName( prop.Name ), this ) );
			label.MinimumSize = new Vector2( 150, Theme.RowHeight );

			var lineedit = row.Layout.Add( new LineEdit( this ) );
			lineedit.Text = prop.GetValue( obj )?.ToString();
			lineedit.MinimumSize = label.MinimumSize;
			lineedit.TextChanged += ( v ) =>
			{
				prop.SetValue( obj, v );
				MarkDirty();
			};
			lineedit.ReadOnly = prop.GetCustomAttribute<ReadOnlyAttribute>() != null;

			if ( lineedit.ReadOnly )
			{
				lineedit.SetStyles( "border:0;" );
			}

			if ( prop.GetCustomAttribute<FilePickerAttribute>() != null )
			{
				var btn = row.Layout.Add( new Button( "File Dialog", this ), -1 );
				btn.MinimumSize = new Vector2( 100, Theme.RowHeight );
				btn.Clicked += () =>
				{
					var fd = new FileDialog( this );
					fd.Title = "Find " + label.Text;
					fd.SetFindFile();

					if ( fd.Execute() )
					{
						lineedit.Text = CustomizationTool.Singleton.GetAddonRelativePath( fd.SelectedFile );
					}
				};
			}

			if( prop.GetCustomAttribute<AssetPickerAttribute>() != null )
			{
				var btn = row.Layout.Add( new Button( "Asset Picker", this ), -1 );
				btn.MinimumSize = new Vector2( 100, Theme.RowHeight );
				btn.Clicked += () =>
				{
					Asset asset = null;
					var assetPiker = new AssetPicker( this );
					assetPiker.OnAssetPicked += v => asset = v;

					var dialog = new ConfirmDialog( this )
						.WithTitle( "Select an asset" )
						.WithSize( 800, 500 )
						.WithWidget( assetPiker )
						.WithConfirm( () => lineedit.Text = asset?.Path ?? lineedit.Text );
				};
			}
		}

		Layout.AddStretchCell( 1 );

		saveButton = Layout.Add( new Button( "Save Changes", "save", this ) );
		saveButton.Enabled = false;
		saveButton.Clicked += () =>
		{
			OnSave?.Invoke();
			saveButton.Enabled = false;
		};
		saveButton.Visible = saveBtn;
	}

	private void MarkDirty()
	{
		saveButton.Enabled = true;
		OnDirty?.Invoke();
	}

	static bool CanDisplayProperty( PropertyInfo pi )
	{
		var t = pi.PropertyType;

		return t == typeof( string ) || t == typeof( int );
	}

	static string GetFriendlyPropertyName( string source )
	{
		return string.Join( " ", Regex.Split( source, @"(?<!^)(?=[A-Z])" ) );
	}

	public class ReadOnlyAttribute : Attribute { }
	public class FilePickerAttribute : Attribute { }
	public class AssetPickerAttribute : Attribute { }
	public class CategoryDropdownAttribute : Attribute { }
	public class PartDropdownAttribute : Attribute { }

}
