using System;

namespace HiPA.Common.Recipe
{
	public interface IRecipeItem
	{
		string RecipeName { get; }
		string InstrumnetName { get; }
	}

	[Serializable]
	public abstract class RecipeItemBase
		: IRecipeItem
	{
		public string RecipeName { get; set; }
		public string InstrumnetName { get; set; }
	}
}
