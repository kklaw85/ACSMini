using HiPA.Common.Forms;
using HiPA.Common.Utils;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace HiPA.Common
{
	[Serializable]
	public abstract class Configuration
				: RecipeBaseUtility
	{
		public abstract string Name { get; set; }
		public string Location { get; set; } = "";
		public abstract InstrumentCategory Category { get; }
		public abstract Type InstrumentType { get; }
		public abstract MachineVariant MachineVar { get; set; }

		protected bool b_Enable = true;
		public bool Enable
		{
			get => this.b_Enable;
			set => this.Set( ref this.b_Enable, value, "Enable" );
		}
		public List<string> Children { get; set; }

		public virtual void CheckDefaultValue()
		{
			_CheckDefaultValue( this );
		}

		public static void _CheckDefaultValue( object instance )
		{
			var type = instance.GetType();
			var fields = type.GetFields( BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance );

			var defaultField = type.GetField( "_DEFAULT", BindingFlags.Static | BindingFlags.NonPublic );
			var defaultObject = defaultField?.GetValue( instance );
			if ( defaultObject == null ) return;

			foreach ( var field in fields )
			{
				var value = field.GetValue( instance );
				if ( value is null )
				{
					var f = defaultField.DeclaringType.GetField( field.Name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance );
					value = f.GetValue( defaultObject );

					var newObj = ObjectCopy.Copy( value );

					field.SetValue( instance, newObj );
				}
			}
		}
	}
}
