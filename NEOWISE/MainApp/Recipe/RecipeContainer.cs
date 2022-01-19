using HiPA.Common;
using HiPA.Common.Forms;
using NeoWisePlatform.Recipe.Windows;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace NeoWisePlatform.Recipe
{
	public abstract class RecipeItemBase : RecipeBaseUtility
	{
		public string Name
		{
			get => this.GetValue( () => this.Name );
			set => this.SetValue( () => this.Name, value );
		}
		public DateTime LastModifiedTime
		{
			get => this.GetValue( () => this.LastModifiedTime );
			set => this.SetValue( () => this.LastModifiedTime, value );
		}
		public RecipeItemBase( string name )
		{
			this.Name = name;
		}
		public RecipeItemBase()
		{
		}
		public override bool Equals( object obj )
		{
			if ( obj is RecipeItemBase recipe ) return recipe.Name == this.Name;
			if ( obj is string s ) return s == this.Name;
			return false;
		}

		public virtual void UpdateLastTime() => this.LastModifiedTime = DateTime.Now;

		public override int GetHashCode()
		{
			return this.Name.GetHashCode();
		}
	}

	public class RecipeContainer : RecipeBaseUtility
	{
		public string RecipeFolder { get; protected set; }

		public string RecipeExtName { get; protected set; } = ".rcp";


		[XmlIgnoreAttribute]
		public int RecipeChangeNo
		{
			get => this.GetValue( () => this.RecipeChangeNo );
			set => this.SetValue( () => this.RecipeChangeNo, value );
		}

		protected RecipeContainer( string folderName )
		{
			//this.Log = Logger.GetLogger( folderName );
			_LoadRecipeTypes();

			try
			{
				this.RecipeFolder = Path.Combine( Constructor.GetInstance().StartupPath, folderName );
				if ( Directory.Exists( this.RecipeFolder ) == false )
					Directory.CreateDirectory( this.RecipeFolder );
			}
			catch ( Exception ex )
			{
				//Log.Error( $"RecipeContainers.cs :RecipeContainer: Prepare the Recipe path failure. ", ex );
				MessageBox.Show( $"RecipeContainers.cs :RecipeContainer: Prepare the Recipe path failure. \r\nError:{ex.Message}" );
			}
		}

		public string GetFileNameByRecipe( string recipeName )
		{
			return Path.Combine( this.RecipeFolder, recipeName + this.RecipeExtName );
		}

		RecipeItemBase _appliedRecipe = null;
		RecipeItemBase AppliedRecipe
		{
			get => this._appliedRecipe;
			set
			{
				this._appliedRecipe = value;
				this.RecipeName = this._appliedRecipe != null ? this._appliedRecipe.Name : string.Empty;
			}
		}

		[XmlIgnoreAttribute]
		public string RecipeName
		{
			get => this.GetValue( () => this.RecipeName );
			set => this.SetValue( () => this.RecipeName, value );
		}

		FileStream _fsLocker = null;

		public RecipeItemBase GetAppliedRecipe()
		{
			return this.AppliedRecipe;
		}
		public RecipeItemBase Reload( object sender = null )
		{
			if ( this.IsExists( this.RecipeName ) == false )
				throw new FileNotFoundException( $"Not found the recipe[{this.RecipeName}]" );
			if ( this.AppliedRecipe != null )
				this.LockRecipe( this.AppliedRecipe.Name, false );

			ReApplyRecipe:
			var file = this.GetFileNameByRecipe( this.RecipeName );
			this.LockRecipe( file, true );

			this._fsLocker.Seek( 0, SeekOrigin.Begin );
			var stream = "";
			var reader = new StreamReader( this._fsLocker, Encoding.UTF8 );
			stream = reader.ReadToEnd();

			try
			{
				this.AppliedRecipe = ( RecipeItemBase )XmlHelper.XmlDeserialize( this.RecipeType, stream, s_XmlExtraTypes );
				this.AppliedRecipe.Name = this.RecipeName;
			}
			catch ( Exception ex )
			{
				this.AppliedRecipe = null;
				MessageBox.Show( $"RecipeContainers.cs :Reload: Reload recipe failure. \r\nError:{ex.Message}" );
				this.LockRecipe( file, false );
				if ( this.RetrieveBackupRecipe( this.RecipeName ) )
					goto ReApplyRecipe;
				throw ex;
			}
			Configuration._CheckDefaultValue( this.AppliedRecipe );

			this.RecipeAppliedEvent?.Invoke( sender ?? this, new RecipeItemChangedEventArgs( this.AppliedRecipe ) );
			this.RecipeChangeNo++;
			return this.AppliedRecipe;
		}
		public RecipeItemBase ApplyRecipe( string name, object sender = null )
		{
			if ( this.IsExists( name ) == false )
				throw new FileNotFoundException( $"Not found the recipe[{name}]" );
			if ( this.AppliedRecipe != null &&
				this.AppliedRecipe.Name == name ) return this.AppliedRecipe;

			if ( this.AppliedRecipe != null )
				this.LockRecipe( this.AppliedRecipe.Name, false );

			ReApplyRecipe:
			var file = this.GetFileNameByRecipe( name );
			this.LockRecipe( file, true );

			this._fsLocker.Seek( 0, SeekOrigin.Begin );
			var stream = "";
			var reader = new StreamReader( this._fsLocker, Encoding.UTF8 );
			stream = reader.ReadToEnd();

			try
			{
				this.AppliedRecipe = ( RecipeItemBase )XmlHelper.XmlDeserialize( this.RecipeType, stream, s_XmlExtraTypes );
				this.AppliedRecipe.Name = name;
			}
			catch ( Exception ex )
			{
				this.AppliedRecipe = null;
				MessageBox.Show( $"RecipeContainers.cs :ApplyRecipe: Apply recipe failure. \r\nError:{ex.Message}" );
				this.LockRecipe( file, false );
				if ( this.RetrieveBackupRecipe( name ) )
					goto ReApplyRecipe;
				throw ex;
			}
			Configuration._CheckDefaultValue( this.AppliedRecipe );
			this.UpdateRcpLstStatus( name );
			this.RecipeAppliedEvent?.Invoke( sender ?? this, new RecipeItemChangedEventArgs( this.AppliedRecipe ) );

			this.RecipeChangeNo++;
			return this.AppliedRecipe;
		}
		public RecipeItemBase ApplySelectedRecipe( object sender = null )
		{
			if ( this.RecipeDir.Count < this.LstSelIdx + 1 ) return null;
			return this.ApplyRecipe( this.RecipeDir[ this.LstSelIdx ].Name, sender );
		}
		#region UI side prompt
		public bool CanCopy
		{
			get => this.GetValue( () => this.CanCopy );
			private set => this.SetValue( () => this.CanCopy, value );
		}
		public bool CanRemove
		{
			get => this.GetValue( () => this.CanRemove );
			private set => this.SetValue( () => this.CanRemove, value );
		}
		public void CreateRecipePrompt()
		{
			try
			{
				var dlg = new Win_DialogRecipeName { UseType = Win_DialogRecipeName.DialogType.New };
				if ( dlg.ShowDialog() != true ) return;
				var name = dlg.RecipeName;
				this.CreateRecipe( name );
			}
			catch ( Exception ex )
			{
				this.CatchAndPromptErr( ex );
			}
			finally
			{
				this.RefreshList();
			}
		}
		public void CopyRecipePrompt()
		{
			try
			{
				if ( this.RecipeDir.Count < this.LstSelIdx + 1 ) return;
				var dlg = new Win_DialogRecipeName { UseType = Win_DialogRecipeName.DialogType.Copy, RecipeName = this.RecipeDir[ this.LstSelIdx ]?.Name };
				if ( dlg.ShowDialog() != true ) return;
				var newName = dlg.RecipeName;
				this.CopyRecipe( this.RecipeDir[ this.LstSelIdx ]?.Name, newName );
			}
			catch ( Exception ex )
			{
				this.CatchAndPromptErr( ex );
			}
			finally
			{
				this.RefreshList();
			}
		}
		public void RemoveRecipePrompt()
		{
			try
			{
				if ( this.RecipeDir.Count < this.LstSelIdx + 1 ) return;
				if ( this.RecipeName == this.RecipeDir[ this.LstSelIdx ]?.Name ) return;
				this.RemoveRecipe( this.RecipeDir[ this.LstSelIdx ]?.Name );
			}
			catch ( Exception ex )
			{
				this.CatchAndPromptErr( ex );
			}
			finally
			{
				this.RefreshList();
			}
		}
		#endregion
		#region Recipe dir listing
		public ObservableCollection<RecipeSelection> RecipeDir = new ObservableCollection<RecipeSelection>();
		public int LstSelIdx
		{
			get => this.GetValue( () => this.LstSelIdx );
			set
			{
				if ( value < 0 ) return;
				this.SetValue( () => this.LstSelIdx, value );
				this.CanCopy = this.RecipeDir.Count >= this.LstSelIdx + 1;
				this.CanRemove = this.RecipeDir[ this.LstSelIdx ]?.Name != this.RecipeName;
			}
		}
		private void UpdateRcpLstStatus( string Name )
		{
			foreach ( var rec in this.RecipeDir )
			{
				if ( rec.Name == Name ) rec.Applied = true;
				else rec.Applied = false;
			}
			this.LstSelIdx = this.LstSelIdx;
		}
		public void RefreshList()
		{
			this.ClearErrorFlags();
			try
			{
				var files = Directory.GetFiles( this.RecipeFolder, "*" + this.RecipeExtName, SearchOption.TopDirectoryOnly );
				var recipelist = files.Select( f => Path.GetFileNameWithoutExtension( f ) );
				System.Windows.Application.Current.Dispatcher.Invoke( delegate
				{
					this.RecipeDir.Clear();
					foreach ( var recipe in recipelist )
					{
						this.RecipeDir.Add( new RecipeSelection() { Name = recipe, Applied = recipe == this.RecipeName } );
					}
				} );
				if ( this.RecipeDir.Count > 0 ) this.LstSelIdx = 0;
				else this.LstSelIdx = -1;
			}
			catch ( Exception ex )
			{
				this.CatchAndPromptErr( ex );
			}
		}
		#endregion
		public int GetRecipeCount()
		{
			var files = Directory.GetFiles( this.RecipeFolder, "*" + this.RecipeExtName, SearchOption.TopDirectoryOnly );
			return files.Count();
		}
		public bool IsExists( string name )
		{
			var file = Path.Combine( this.RecipeFolder, name + this.RecipeExtName );
			return File.Exists( file );
		}



		public readonly char[] _invalidNameChars = new char[] { '\\', '/', ':', '?', '*', '<', '>', '|' };
		public bool CheckNameValidation( string name )
		{
			return name.Intersect( Path.GetInvalidFileNameChars() ).Count() == 0;
		}

		protected virtual Type RecipeType => null;
		public void CreateRecipe( string name, Type recipeType = null )
		{
			//if ( recipeType == null )
			//	throw new ArgumentNullException( $"Recipe Type is invalid" );
			if ( string.IsNullOrEmpty( name ) == true )
				throw new ArgumentNullException( $"Recipe name is null" );
			if ( this.IsExists( name ) == true )
				throw new ArgumentException( $"Recipe[{name}] already exist" );
			if ( this.CheckNameValidation( name ) == false )
				throw new ArgumentException( $"Recipe name[{name}] is invalid" );

			var recipe = Activator.CreateInstance( recipeType ?? this.RecipeType, new object[] { name } );
			this.Save( recipe as RecipeItemBase );
		}
		public void CopyRecipe( string srcName, string newName )
		{
			if ( this.IsExists( srcName ) == false )
				throw new FileNotFoundException( $"Not found The source recipe[{srcName}]" );
			if ( this.IsExists( newName ) == true )
				throw new ArgumentException( $"The new recipe[{newName}] already exist" );
			if ( this.CheckNameValidation( newName ) == false )
				throw new ArgumentException( $"The recipe name[{newName}] is invalid" );

			var srcFile = this.GetFileNameByRecipe( srcName );
			var newFile = this.GetFileNameByRecipe( newName );
			File.Copy( srcFile, newFile, false );

			var newRecipe = this.LoadRecipe( newName );
			newRecipe.Name = newName;
			this.Save( newRecipe );
		}
		public void RemoveRecipe( string name )
		{
			if ( this.IsExists( name ) == false )
				throw new FileNotFoundException( $"Not found The source recipe[{name}]" );

			if ( this.AppliedRecipe != null &&
				this.AppliedRecipe.Name == name )
				throw new InvalidOperationException( $"Can not Remove the Applied recipe" );

			var file = this.GetFileNameByRecipe( name );
			File.Delete( file );
		}

		protected void LockRecipe( string fileName, bool toLock )
		{
			if ( toLock )
			{
				this._fsLocker?.Close();
				this._fsLocker = null;

				this._fsLocker = new FileStream( fileName, FileMode.Open, FileAccess.ReadWrite, FileShare.Read );
			}
			else
			{
				this._fsLocker?.Close();
				this._fsLocker = null;
			}
		}

		protected RecipeItemBase LoadRecipe( string name )
		{
			var file = this.GetFileNameByRecipe( name );
			return ( RecipeItemBase )XmlHelper.XmlDeserializeFromFile( this.RecipeType, file, s_XmlExtraTypes );
		}
		public void Save( RecipeItemBase recipe )
		{
			try
			{
				if ( recipe == null )
					throw new ArgumentNullException( $"The recipe is null" );
				if ( string.IsNullOrEmpty( recipe.Name ) == true )
					throw new ArgumentNullException( $"The recipe[{recipe.Name}] is invalid" );

				recipe.UpdateLastTime();
				var fileName = this.GetFileNameByRecipe( recipe.Name );

				if ( recipe == this.AppliedRecipe )
				{
					this._fsLocker.Position = 0;
					var stream = XmlHelper.XmlSerialize( recipe, s_XmlExtraTypes );
					var data = Encoding.UTF8.GetBytes( stream );
					this._fsLocker.Write( data, 0, data.Length );
					this._fsLocker.Flush();
					this._fsLocker.SetLength( data.Length );
				}
				else
				{
					XmlHelper.XmlSerializeToFile( recipe, fileName, s_XmlExtraTypes );
				}
				this.BackUpRecipe( recipe );
			}
			catch ( Exception ex )
			{
				MessageBox.Show( $"RecipeContainer.cs :Save:{ex.Message}" );
			}
			this.RecipeSavedEvent?.Invoke( this, new RecipeItemChangedEventArgs( recipe ) );
		}
		const string BackUpPath = "D:\\RecipeBackUp\\";
		public IEnumerable<string> GetBackUpDates()
		{
			var DirNames = new List<string>();
			try
			{
				if ( !Directory.Exists( BackUpPath ) ) throw new Exception();
				var BackupPaths = Directory.EnumerateDirectories( BackUpPath );
				foreach ( var path in BackupPaths )
				{
					var pathname = path.Split( '\\' );
					if ( DateTime.TryParse( pathname[ 2 ], out DateTime result ) )
						DirNames.Add( pathname[ 2 ] );
				}
				return DirNames;
			}
			catch
			{
				return null;
			}
		}
		private Dictionary<string, string> BackUpDict = new Dictionary<string, string>();
		private void UpdateBackUpDic()
		{
			var DirNames = new List<string>();
			try
			{
				if ( !Directory.Exists( BackUpPath ) ) throw new Exception();
				var BackupPaths = Directory.EnumerateDirectories( BackUpPath );
				foreach ( var path in BackupPaths )
				{
					var pathname = path.Split( '\\' );
					if ( DateTime.TryParse( pathname[ 2 ], out DateTime result ) )
						this.BackUpDict[ pathname[ 2 ] ] = path;
				}
			}
			catch
			{
			}
		}
		public bool isBackUpDone()
		{
			try
			{
				this.UpdateBackUpDic();
				var Now = DateTime.Now.ToString( "yyyy-MM-dd" );
				return this.BackUpDict.ContainsKey( Now );
			}
			catch
			{
				return false;
			}
		}
		public void BackUpRecipe()
		{
			try
			{
				this.UpdateBackUpDic();
				var Now = DateTime.Now.ToString( "yyyy-MM-dd" );
				//return this.BackUpDict.ContainsKey( Now );
			}
			catch
			{
			}
		}
		public void BackUpRecipe( RecipeItemBase recipe )
		{
			try
			{
				if ( !Directory.Exists( BackUpPath ) )
					Directory.CreateDirectory( BackUpPath );
				var BackupFP = BackUpPath + recipe.Name + ".rcp";
				var RecipeOrg = this.GetFileNameByRecipe( recipe.Name );

				if ( File.Exists( BackupFP ) )
					File.Delete( BackupFP );
				File.Copy( RecipeOrg, BackupFP );
			}
			catch
			{
			}
		}
		public bool RetrieveBackupRecipe( string name )
		{
			try
			{
				var RecipeOrg = this.GetFileNameByRecipe( name );

				var RecipeBkUp = BackUpPath + name + ".rcp";
				if ( File.Exists( RecipeBkUp ) )
					if ( MessageBox.Show( "Do you want to revert to Backup Recipe?", "Backup Recipe Found", MessageBoxButtons.YesNo ) == DialogResult.Yes )
					{
						File.Delete( RecipeOrg );
						File.Copy( RecipeBkUp, RecipeOrg );
						return true;
					}
			}
			catch
			{

			}
			return false;
		}
		#region Recipe Event & Handler
		public event RecipeItemChangedEventHandler RecipeAppliedEvent;
		public event RecipeItemChangedEventHandler RecipeSavedEvent;
		#endregion

		#region Looking for all recipe types
		static Type[] s_XmlExtraTypes = null;
		protected static void _LoadRecipeTypes()
		{
			if ( s_XmlExtraTypes == null )
			{
				var validTypes = new Type[] { typeof( RecipeItemBase ) };
				var result = HiPA.Common.Utils.ReflectionTool.QueryConfigurationTypes( validTypes );

				var singleT = new List<Type>();
				foreach ( var item in result.Values )
				{
					singleT.AddRange( item );
				}
				if ( singleT.Any() )
				{
					s_XmlExtraTypes = singleT.ToArray();
				}
			}
		}
		#endregion
	}

	public class RecipeContainer<TRecipe>
		: RecipeContainer
		where TRecipe : RecipeItemBase
	{
		protected RecipeContainer( string folderName ) : base( folderName )
		{
		}

		new public TRecipe GetAppliedRecipe()
		{
			return base.GetAppliedRecipe() as TRecipe;
		}

		new public TRecipe ApplyRecipe( string name, object sender = null )
		{
			return base.ApplyRecipe( name, sender ) as TRecipe;
		}

		protected override Type RecipeType => typeof( TRecipe );
		public void CreateRecipe( string name )
		{
			base.CreateRecipe( name, typeof( TRecipe ) );
		}
	}

	public static class Recipes
	{
		#region Singleton 
		static RecipeContainer<RecipeItemHandler> s_handler = null;
		public static RecipeContainer<RecipeItemHandler> HandlerRecipes()//Package recipe
		{
			try
			{
				if ( s_handler == null )
				{
					var type = typeof( RecipeContainer<RecipeItemHandler> );
					s_handler = Activator.CreateInstance(
						typeof( RecipeContainer<RecipeItemHandler> ),
						System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance, null,
						new object[] { "RecipeHandler" }, System.Globalization.CultureInfo.CurrentCulture ) as RecipeContainer<RecipeItemHandler>;
				}
			}
			catch ( Exception ex )
			{
				return null;
			}
			return s_handler;
		}
		#endregion
	}

	public class RecipeItemChangedEventArgs : EventArgs
	{
		public RecipeItemBase Recipe { get; }
		public RecipeItemChangedEventArgs( RecipeItemBase recipe )
		{
			this.Recipe = recipe;
		}
	}

	public delegate void RecipeItemChangedEventHandler( object sender, RecipeItemChangedEventArgs e );

	public class RecipeSelection : RecipeBaseUtility
	{
		public string Name
		{
			get => this.GetValue( () => this.Name );
			set => this.SetValue( () => this.Name, value );
		}
		public bool Applied
		{
			get => this.GetValue( () => this.Applied );
			set => this.SetValue( () => this.Applied, value );
		}
	}
}
