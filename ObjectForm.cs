using System;
using System.IO;
using System.Reflection;
using System.Text.Json;
using System.Text.RegularExpressions;
using Tools;

namespace CustomizationEditor;

public class ObjectForm : Widget
{

	public Action OnDirty;
	public Action OnSave;
	public bool AutoSave;

	private Button saveButton;

	public ObjectForm( object obj, Widget parent, bool saveBtn = true )
		: base( parent )
	{
		if ( obj == null ) throw new NullReferenceException();

		var clone = JsonSerializer.Deserialize( JsonSerializer.Serialize( obj ), obj.GetType() );

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
				prop.SetValue( clone, v );
				if ( AutoSave )
				{
					CopyValues( obj, clone );
					return;
				}
				MarkDirty();
			};
			lineedit.ReadOnly = prop.GetCustomAttribute<ReadOnlyAttribute>() != null;

			if ( lineedit.ReadOnly )
			{
				lineedit.SetStyles( "border:0;" );
			}

			if ( prop.GetCustomAttribute<ImagePickerAttribute>() != null )
			{
				var btn = row.Layout.Add( new Button( "Image Picker", this ), -1 );
				btn.MinimumSize = new Vector2( 100, Theme.RowHeight );
				btn.Clicked += () =>
				{
					string img = null;
					var imagePiker = new ImagePicker( this );
					imagePiker.OnImagePicked += v => img = v;

					var dialog = new ConfirmDialog( this )
						.WithTitle( "Select an image" )
						.WithSize( 800, 500 )
						.WithWidget( imagePiker )
						.WithConfirm( () => lineedit.Text = img ?? lineedit.Text );
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
			CopyValues( obj, clone );
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

	static void CopyValues( object target, object source )
	{
		if ( target == null ) throw new ArgumentNullException( nameof( target ) );
		if ( source == null ) throw new ArgumentNullException( nameof( source ) );

		Type t = target.GetType();

		var properties = t.GetProperties(
			  BindingFlags.Instance | BindingFlags.Public ).Where( prop =>
					prop.CanRead
				 && prop.CanWrite
				 && prop.GetIndexParameters().Length == 0 );

		foreach ( var prop in properties )
		{
			var value = prop.GetValue( source, null );
			prop.SetValue( target, value, null );
		}
	}

	public class ReadOnlyAttribute : Attribute { }
	public class FilePickerAttribute : Attribute { }
	public class ImagePickerAttribute : Attribute { }
	public class AssetPickerAttribute : Attribute { }
	public class CategoryDropdownAttribute : Attribute { }
	public class PartDropdownAttribute : Attribute { }

}
