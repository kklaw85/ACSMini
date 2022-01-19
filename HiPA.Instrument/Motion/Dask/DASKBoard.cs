using System.Collections.Generic;

namespace HiPA.Instrument.Motion.Dask
{
	internal class Dask
	{
		static Dictionary<int, string> _errorsSet;
		public static string GetErrorDesc( short errorCode )
		{
			try
			{
				if ( _errorsSet == null )
				{
					_errorsSet = new Dictionary<int, string>();
					foreach ( var pair in HiPA.Common.Utils.ReflectionTool.GetEnumValueDesc( typeof( DASKError ) ) )
						_errorsSet.Add( pair.Item1, pair.Item2 );
				}

				if ( errorCode == 0 ) return "";

				if ( _errorsSet.TryGetValue( errorCode, out var result ) ) return result;

			}
			catch
			{
				//new Thread( () => MessageBox.Show( $"DASKBoard.cs :GetErrorDesc:{ex.Message}" ) ).Start();
			}
			return "Unknown Error";
		}
	}

	#region AdLink Errors

	#endregion
}
