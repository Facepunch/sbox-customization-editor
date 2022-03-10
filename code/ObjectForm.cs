using Sandbox;
using System;
using System.IO;
using System.Linq;
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
			
			// label, stretch
			var leftside = row.Layout.Add( new Widget( this ) );
			leftside.SetLayout( LayoutMode.TopToBottom );

			var label = leftside.Layout.Add( new Label( GetFriendlyPropertyName( prop.Name ), this ) );
			label.MinimumSize = new Vector2( 150, Theme.RowHeight );

			leftside.Layout.AddStretchCell( 1 );

			// line editor, button, icon
			var rightside = row.Layout.Add( new Widget( this ) );
			rightside.SetLayout( LayoutMode.TopToBottom );

			var rightsideTop = rightside.Layout.Add( new Widget( this ) );
			rightsideTop.SetLayout( LayoutMode.LeftToRight );

			var lineedit = rightsideTop.Layout.Add( new LineEdit( this ) );
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

			if( prop.GetCustomAttribute<AssetPickerAttribute>() != null )
			{
				var btn = rightsideTop.Layout.Add( new Button( "Asset Picker", this ), -1 );
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

				rightside.Layout.Add( new SimpleThumb( lineedit, this ) );
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
	public class AssetPickerAttribute : Attribute { }
	public class CategoryDropdownAttribute : Attribute { }
	public class PartDropdownAttribute : Attribute { }

	private class SimpleThumb : Widget
	{

		private LineEdit userinput;

		public SimpleThumb( LineEdit userinput, Widget parent = null )
			: base( parent )
		{
			this.userinput = userinput;

			MinimumSize = new Vector2( 64, 64 );

			userinput.TextChanged += ( v ) =>
			{
				Update();
				Update();
			};
		}

		protected override void OnPaint()
		{
			base.OnPaint();

			var path = Path.Combine( CustomizationTool.Singleton.Addon.Config.Directory.FullName, userinput.Text );
			var asset = AssetSystem.FindByPath( path );
			var thumb = asset?.GetAssetThumb();
			var r = new Rect( 0, 0, 64, 64 );

			Paint.SetPenEmpty();
			Paint.SetBrush( Color.Black );
			Paint.DrawRect( r, 4 );

			if ( thumb == null ) return;

			Paint.Draw( r.Expand( -4, -4 ), thumb );
		}

	}

}
