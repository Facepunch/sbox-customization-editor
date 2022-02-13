using System;
using System.Reflection;
using System.Text.RegularExpressions;
using Tools;

namespace Facepunch.CustomizationTool;

internal class CustomizationObjectForm : Widget
{

	public Action OnDirty;
	public Action OnSave;

	private Button saveButton;

	public CustomizationObjectForm( object obj, Widget parent )
		: base( parent )
	{
		if ( obj == null ) throw new NullReferenceException();

		SetLayout( LayoutMode.TopToBottom );
		Layout.Spacing = 4;

		foreach( var prop in obj.GetType().GetProperties() )
		{
			if ( !CanDisplayProperty( prop ) ) continue;

			var t = prop.PropertyType;

			var row = Layout.Add( new Widget( this ) );
			row.SetLayout( LayoutMode.LeftToRight );
			row.MinimumSize = 24;

			var label = row.Layout.Add( new Label( GetFriendlyPropertyName( prop.Name ), this ) );
			label.MinimumSize = new Vector2( 150, 24 );

			var lineedit = row.Layout.Add( new LineEdit( this ) );
			lineedit.Text = prop.GetValue( obj )?.ToString();
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
				var btn = row.Layout.Add( new Button( "Open File Dialog", this ), -1 );
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
				var btn = row.Layout.Add( new Button( "Open Asset Picker", this ), -1 );
				btn.Clicked += () =>
				{
					var w = new AssetPickerWindow( this, ( asset ) =>
					 {
						 lineedit.Text = asset.Path;
					 } );
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
