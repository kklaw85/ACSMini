using HiPA.Common.Forms;
using System;

namespace NeoWisePlatform.Recipe
{
	[Serializable]
	public class PNPPos : RecipeBaseUtility
	{
		public double PickPos//For load/unload arm
		{
			get => this.GetValue( () => this.PickPos );
			set => this.SetValue( () => this.PickPos, value );
		}
		public double LoadPos//For load arm
		{
			get => this.GetValue( () => this.LoadPos );
			set => this.SetValue( () => this.LoadPos, value );
		}
		public double PlaceNGPos//For unload arm
		{
			get => this.GetValue( () => this.PlaceNGPos );
			set => this.SetValue( () => this.PlaceNGPos, value );
		}
		public double PlaceKIVPos//For unload arm
		{
			get => this.GetValue( () => this.PlaceKIVPos );
			set => this.SetValue( () => this.PlaceKIVPos, value );
		}
		public double WaitPos//while waiting inspection to complete
		{
			get => this.GetValue( () => this.WaitPos );
			set => this.SetValue( () => this.WaitPos, value );
		}
		public PNPPos()
		{ }
	}
}