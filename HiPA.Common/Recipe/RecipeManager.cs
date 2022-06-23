using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;

namespace HiPA.Common.Recipe
{
	/// <summary>
	/// TO BE CONTINUED... 
	/// by Howie
	/// </summary>



	[Serializable]
	public class RecipeArchiveItem
	{
		[NonSerialized]
		public object SyncRoot = new object();

		public string RecipeName { get; set; }
		protected XmlDictionary<string, IRecipeItem> RecipeItems { get; set; } = new XmlDictionary<string, IRecipeItem>();
		public IEnumerable<string> InstrumentNames => this.RecipeItems.Keys;

		[NonSerialized]
		public Dictionary<string, InstrumentBase> Instruments;

		#region RecipeItem Manager 
		public IRecipeItem GetRecipeItem( string instrumentName )
		{
			if ( string.IsNullOrEmpty( instrumentName ) == true ) return null;
			try
			{
				Monitor.Enter( this.SyncRoot );
				return this.RecipeItems[ instrumentName ];
			}
			finally
			{
				Monitor.Exit( this.SyncRoot );
			}
		}
		public void AddRecipeItem( IRecipeItem recipeItem )
		{
			if ( recipeItem == null ) return;
			try
			{
				Monitor.Enter( this.SyncRoot );
				if ( this.RecipeItems.TryGetValue( recipeItem.InstrumnetName, out var item ) == true ) return;
				this.RecipeItems.Add( recipeItem.InstrumnetName, recipeItem );
			}
			finally
			{
				Monitor.Exit( this.SyncRoot );
			}
		}
		public bool RemoveRecipeItem( string instrumentName )
		{
			if ( string.IsNullOrEmpty( instrumentName ) == true ) return false;
			try
			{
				Monitor.Enter( this.SyncRoot );
				return this.RecipeItems.Remove( instrumentName );
			}
			finally
			{
				Monitor.Exit( this.SyncRoot );
			}
		}
		#endregion
	}

	//public class RecipeContainer
	//{
	//	const string EQUIPMENT_RECIPE = "Equipment";

	//	public string RecipePath { get; set; }

	//	RecipeArchiveItem _equipmentRecipe = null;
	//	Type[] _xmlExtraTypes = null;

	//	protected RecipeContainer()
	//	{
	//	}

	//	public void InitRecipes( string recipePath )
	//	{
	//		var type = typeof( IRecipeItem );
	//		var types = HiPA.Common.Utils.ReflectionTool.QueryConfigurationTypes( new Type[] { type } );
	//		if ( types.TryGetValue( type, out var list ) == true )
	//			this._xmlExtraTypes = list.ToArray();

	//		this.RecipePath = recipePath;
	//		if ( Directory.Exists( recipePath ) == false )
	//			Directory.CreateDirectory( recipePath );

	//		//this._equipmentRecipePath = Path.Combine( this.RecipePath, EQUIPMENT_RECIPE );

	//		foreach ( var file in Directory.EnumerateFiles( this.RecipePath, "*.rcp" ) )
	//		{
	//			var recipeName = Path.GetFileNameWithoutExtension( file );
	//			var recipeArchiveItem = this.DeserializeRecipe( recipeName );
	//			if ( recipeArchiveItem != null )
	//			{
	//				if ( recipeArchiveItem.RecipeName == EQUIPMENT_RECIPE )
	//				{
	//					this._equipmentRecipe = recipeArchiveItem;
	//				}
	//				else
	//				{
	//					this.AddArchiveItem( recipeArchiveItem );
	//				}

	//			}
	//		}

	//		this.InitInstrumentDependency();
	//	}

	//	private void InitInstrumentDependency()
	//	{
	//		var instance = Constructor.GetInstance();
	//		if ( this._equipmentRecipe == null ) this._equipmentRecipe = new RecipeArchiveItem();

	//		foreach ( var name in this._equipmentRecipe.InstrumentNames )
	//		{
	//			var instrument = instance.GetInstrument( name );
	//			if ( instrument != null ) this._equipmentRecipe.Instruments[ name ] = instrument;
	//		}
	//	}

	//	Dictionary<string, RecipeArchiveItem> Recipes = new Dictionary<string, RecipeArchiveItem>();

	//	public IRecipeItem GetRecipeByName( string instrumentName, string recipeName = "" )
	//	{
	//		return null;
	//	}

	//	private void UpdateRecipeItem( IRecipeItem recipe )
	//	{

	//	}
	//	public void SaveRecipe( object sender, IRecipeItem recipeItem )
	//	{
	//		//var previous = this.GetRecipeByName( recipe.RecipeName, recipe.InstrumnetName );
	//		var recipe = this.GetArchiveItem( recipeItem.RecipeName );
	//		this.UpdateRecipeItem( recipeItem );
	//		this.SerializeRecipe( recipe.RecipeName, recipe );
	//		this.RecipeItemChanged?.Invoke( sender, new RecipeItemChangedEventArgs( recipeItem ) );
	//	}

	//	#region Serialization
	//	void SerializeRecipe( string name, RecipeArchiveItem recipe )
	//	{
	//		var path = Path.Combine( this.RecipePath, name + ".rcp" );
	//		XmlHelper.XmlSerializeToFile( recipe, path, this._xmlExtraTypes );
	//	}
	//	RecipeArchiveItem DeserializeRecipe( string name )
	//	{
	//		var path = Path.Combine( this.RecipePath, name + ".rcp" );
	//		return XmlHelper.XmlDeserializeFromFile<RecipeArchiveItem>( path, this._xmlExtraTypes );
	//	}
	//	#endregion

	//	#region Recipe Archive Item Manager 
	//	object _syncRecipes = new object();
	//	private RecipeArchiveItem GetArchiveItem( string recipeName )
	//	{
	//		if ( string.IsNullOrEmpty( recipeName ) == true ) return null;
	//		try
	//		{
	//			Monitor.Enter( this._syncRecipes );
	//			this.Recipes.TryGetValue( recipeName, out var item );
	//			return item;
	//		}
	//		finally
	//		{
	//			Monitor.Exit( this._syncRecipes );
	//		}
	//	}
	//	private bool AddArchiveItem( RecipeArchiveItem item )
	//	{
	//		if ( item == null ) return false;

	//		try
	//		{
	//			Monitor.Enter( this._syncRecipes );
	//			if ( this.Recipes.TryGetValue( item.RecipeName, out var temp ) == true ) return false;
	//			this.Recipes.Add( item.RecipeName, item );
	//			return true;
	//		}
	//		finally
	//		{
	//			Monitor.Exit( this._syncRecipes );
	//		}
	//	}
	//	private bool RemoveArchiveItem( string recipeName )
	//	{
	//		if ( string.IsNullOrEmpty( recipeName ) == true ) return false;
	//		try
	//		{
	//			Monitor.Enter( this._syncRecipes );
	//			return this.Recipes.Remove( recipeName );
	//		}
	//		finally
	//		{
	//			Monitor.Exit( this._syncRecipes );
	//		}
	//	}
	//	private
	//	#endregion

	//	#region Singleton
	//	static object s_SyncRoot = new object();
	//	static RecipeContainer s_Instance = null;
	//	public static RecipeContainer GetInstance()
	//	{
	//		lock ( s_SyncRoot )
	//		{
	//			if ( s_Instance == null ) s_Instance = new RecipeContainer();
	//			return s_Instance;
	//		}
	//	}
	//	#endregion

	//	#region Event 
	//	public event RecipeItemChangedEventHandler RecipeItemChanged;
	//	public delegate void RecipeItemChangedEventHandler( object sender, RecipeItemChangedEventArgs e );
	//	#endregion
	//}

	//public class RecipeItemChangedEventArgs : EventArgs
	//{
	//	public IRecipeItem Current { get; set; }

	//	public RecipeItemChangedEventArgs( IRecipeItem current )
	//	{
	//		this.Current = current;
	//	}
	//}

	public class RecipeNoticeHandler
	{
		public static event PropertyChangedEventHandler StaticPropertyChanged;
		private static bool bRecipeChanged = false;
		public static bool RecipeChanged
		{
			get => bRecipeChanged;
			set
			{
				bRecipeChanged = value;
				StaticPropertyChanged?.Invoke( null, new PropertyChangedEventArgs( "RecipeChanged" ) );
			}
		}
	}
}
