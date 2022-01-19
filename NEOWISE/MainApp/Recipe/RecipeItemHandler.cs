using System;

namespace NeoWisePlatform.Recipe
{

	[Serializable]
	public class RecipeItemHandler : RecipeItemBase//package type here
	{
		[NonSerialized]
		private static readonly RecipeItemHandler _DEFAULT = new RecipeItemHandler( "" );
		public PNPPos PNPWorkPos { get; set; } = new PNPPos();// Decimal up down   #1
		#region Recipe Event & Handler
		#endregion
		public RecipeItemHandler( string name ) : base( name )
		{
		}
		public RecipeItemHandler()
		{
		}
	}
}
