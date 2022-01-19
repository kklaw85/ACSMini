using HiPA.Common;
using HiPA.Common.Forms;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Runtime.CompilerServices;

namespace HiPA.Communicator
{
	//ShopFloor
	public class CavityDetail
	{
		public string serial { get; set; }
		public string mpn { get; set; }
		public string productCode { get; set; }
		public string notAllowed { get; set; }
		public string nextStationType { get; set; }
		public string length { get; set; }
		public string width { get; set; }
		public string height { get; set; }
		public string deviceColor { get; set; }
	}


	//Hive----------------------------------------------------------------
	//these variables name need to exactly match with Hive's data
	public class UnitData
	{
		public string unit_sn { get; set; }
		public serialdata serials { get; set; }
		public string pass { get; set; }
		public string input_time { get; set; }
		public string output_time { get; set; }
		public AELimitKeys data { get; set; }
		public string product { get; set; }
		public IEnumerable<string> GetPropertyName( object obj )
		{
			var Header = new List<string>();
			try
			{
				var Properties = obj.GetType().GetProperties();
				if ( Properties.Length > 1 )
					foreach ( var property in Properties )
					{
						var subproperty = property.PropertyType.GetProperties();
						if ( subproperty.Length > 1 && property.PropertyType != typeof( string ) )
						{
							Header.AddRange( this.GetPropertyName( property.GetValue( obj, null ) ) );
						}
						else
							Header.Add( property.Name );
					}
			}
			catch
			{ }
			return Header;
		}
		public IEnumerable<string> GetPropertyValue( object obj )
		{
			var Header = new List<string>();
			try
			{
				var Properties = obj.GetType().GetProperties();
				if ( Properties.Length > 1 )
				{
					foreach ( var property in Properties )
					{
						var subproperty = property.PropertyType.GetProperties();
						if ( subproperty.Length > 1 && property.PropertyType != typeof( string ) )
						{
							Header.AddRange( this.GetPropertyValue( property.GetValue( obj, null ) ) );
						}
						else if ( property.PropertyType == typeof( serialdata ) && property.PropertyType != typeof( serialdatainherited ) )
							Header.Add( "{}" );
						else
							Header.Add( property.GetValue( obj, null )?.ToString() );
					}
				}
			}
			catch ( Exception ex )
			{ }
			return Header;
		}
		public string GetHeader()
		{
			var Header = string.Empty;
			try
			{
				var PropertiesNames = this.GetPropertyName( this );
				foreach ( var name in PropertiesNames )
					Header += $"{name},";
			}
			catch
			{ }
			return Header;
		}
		public string GetData()
		{
			var Header = string.Empty;
			try
			{
				var PropertiesNames = this.GetPropertyValue( this );
				foreach ( var name in PropertiesNames )
					Header += $"{name},";
			}
			catch
			{ }
			return Header;
		}
	}

	public class serialdata
	{ }
	public class serialdatainherited : serialdata
	{
		public string top_case { get; set; }
		public string bottom_case { get; set; }
	}

	public class AirPodAELimitKeys : AELimitKeys
	{
		[Order]
		public double Unit_Arc_Width { get; set; }
		[Order]
		public double Height_L { get; set; }
		[Order]
		public double Height_R { get; set; }
		[Order]
		public double Height_C { get; set; }
		[Order]
		public double Height_B { get; set; }

		public override void SetWidth( double Width )
		{
			this.Unit_Arc_Width = Width;
		}
		public void SetHeight( HeightLevelDifference heightlevel )
		{
			this.Height_L = heightlevel.Height_L;
			this.Height_R = heightlevel.Height_R;
			this.Height_C = heightlevel.Height_C;
			this.Height_B = heightlevel.Height_B;
		}

	}

	public abstract class AELimitKeys
	{
		#region Process related parameters
		[Order]
		public string Carrier_SN { get; set; }
		[Order]
		public int Active_Energy_cumulative { get; set; }
		[Order]
		public int Active_energy_current_unit { get; set; }
		[Order]
		public double CYCLE_TIME { get; set; }//
		[Order]
		public double ConveyorClearedTime { get; set; }//pending for catherine's feedback
		[Order]
		public double Door_Open_Time { get; set; }
		[Order]
		public string ErrorCode { get; set; }
		[Order]
		public double FREQUENCY { get; set; }
		[Order]
		public double FirstPalletToPos1Time { get; set; }
		[Order]
		public double FirstPalletToPos2Time { get; set; }
		[Order]
		public double Galvo_X_Offset { get; set; }
		[Order]
		public double Galvo_Y_Offset { get; set; }
		[Order]
		public double HATCH_DISTANCE { get; set; }
		[Order]
		public double Holding_Time { get; set; }
		[Order]
		public int LASER_Bay { get; set; }
		[Order]
		public string LASER_TYPE { get; set; }
		[Order]
		public int LAYERS { get; set; }
		[Order]
		public double LHAxisZValue { get; set; }
		[Order]
		public double LaserDiodeTemperature { get; set; }
		[Order]
		public double LaserHeadTemperature { get; set; }
		[Order]
		public double LiftZStandbyDoorOpenTime { get; set; }
		[Order]
		public double MARK_TIME { get; set; }
		[Order]
		public int Mark_Incorrect { get; set; }
		[Order]
		public int Mark_action { get; set; }
		[Order]
		public double Mark_delta_Angle { get; set; }
		[Order]
		public double Mark_delta_X { get; set; }
		[Order]
		public double Mark_delta_Y { get; set; }
		[Order]
		public double Mark_delta_gray { get; set; }
		[Order]
		public double MotionInPos { get; set; }
		[Order]
		public double MoveProductPresserTime { get; set; }
		[Order]
		public int Online { get; set; }
		[Order]
		public double POWER { get; set; }
		[Order]
		public int Peak_Current { get; set; }
		[Order]
		public int Peak_Voltage { get; set; }
		[Order]
		public double SPEED { get; set; }
		[Order]
		public double ScanBarCodeTime { get; set; }
		[Order]
		public double ScanHeightTime { get; set; }
		[Order]
		public double ThreePalletsInMachineTime { get; set; }
		[Order]
		public double Unit_Exposure { get; set; }
		[Order]
		public double Unit_Grayscale { get; set; }
		[Order]
		public int Unit_Orientation { get; set; }
		[Order]
		public double Unit_Pos_Angle { get; set; }
		[Order]
		public double Unit_Pos_X { get; set; }
		[Order]
		public double Unit_Pos_Y { get; set; }
		[Order]
		public double Unit_Y_Offset { get; set; }
		[Order]
		public double Z_Height_Compensation { get; set; }
		[Order]
		public double Z_Height_Target { get; set; }
		[Order]
		public double Z_Height_Uncompensation { get; set; }
		#endregion
		public abstract void SetWidth( double Width );
		public virtual void Clear()
		{
			var properties = this.GetType().GetProperties();
			foreach ( var property in properties )
			{
				if ( property.PropertyType == typeof( string ) )
					this.GetType().GetProperty( property.Name ).SetValue( this, string.Empty );
				else
					this.GetType().GetProperty( property.Name ).SetValue( this, 0 );
			}
		}
		public void Copy( AELimitMachine source )
		{
			var properties = source.GetType().GetProperties();
			foreach ( var property in properties )
			{
				this.GetType().GetProperty( property.Name ).SetValue( this, property.GetValue( source, null ) );
			}
		}
		public void SimData()
		{
			var properties = this.GetType().GetProperties();
			int count = 0;
			foreach ( var property in properties )
			{
				if ( property.PropertyType == typeof( string ) )
					this.GetType().GetProperty( property.Name ).SetValue( this, ( ++count ).ToString() );
				else
					this.GetType().GetProperty( property.Name ).SetValue( this, ++count );
			}
		}
		public void InMachinePara( MachineParameters source )
		{
			var properties = source.GetType().GetProperties();
			foreach ( var property in properties )
			{
				this.GetType().GetProperty( property.Name ).SetValue( this, property.GetValue( source, null ) );
			}
		}
		public void SetErrorCode( RunErrors sErr )
		{
			this.ErrorCode = sErr.ToString();
		}
	}
	public class AELimitMachine : AELimitKeys
	{
		public override void SetWidth( double Width )
		{
		}
	}

	//Selected AELimitKey to display, Need to seperate from AELimitKey otherwise Json conversion has error
	public class AELimitKeyDisplay : BaseUtility
	{
		private string s_Carrier_SN = string.Empty;
		public string Carrier_SN
		{
			get => this.s_Carrier_SN;
			set
			{
				this.Set( ref this.s_Carrier_SN, value, "Carrier_SN" );
			}
		}

		private int s_Mark_Incorrect = 0;
		public int Mark_Incorrect
		{
			get => this.s_Mark_Incorrect;
			set
			{
				this.Set( ref this.s_Mark_Incorrect, value, "Mark_Incorrect" );
			}
		}

		private double s_Mark_delta_X = 0;
		public double Mark_delta_X
		{
			get => this.s_Mark_delta_X;
			set
			{
				this.Set( ref this.s_Mark_delta_X, value, "Mark_delta_X" );
			}
		}

		private double s_Mark_delta_Y = 0;
		public double Mark_delta_Y
		{
			get => this.s_Mark_delta_Y;
			set
			{
				this.Set( ref this.s_Mark_delta_Y, value, "Mark_delta_Y" );
			}
		}

		private double s_Mark_delta_Angle = 0;
		public double Mark_delta_Angle
		{
			get => this.s_Mark_delta_Angle;
			set
			{
				this.Set( ref this.s_Mark_delta_Angle, value, "Mark_delta_Angle" );
			}
		}

		private double s_Z_Height = 0;
		public double Z_Height
		{
			get => this.s_Z_Height;
			set
			{
				this.Set( ref this.s_Z_Height, value, "Z_Height" );
			}
		}

		private double s_Unit_Width = 0;
		public double Unit_Width
		{
			get => this.s_Unit_Width;
			set
			{
				this.Set( ref this.s_Unit_Width, value, "Unit_Width" );
			}
		}

		private int s_Unit_Orientation = 0;
		public int Unit_Orientation
		{
			get => this.s_Unit_Orientation;
			set
			{
				this.Set( ref this.s_Unit_Orientation, value, "Unit_Orientation" );
			}
		}

		private double s_Unit_Pos_X = 0;
		public double Unit_Pos_X
		{
			get => this.s_Unit_Pos_X;
			set
			{
				this.Set( ref this.s_Unit_Pos_X, value, "Unit_Pos_X" );
			}
		}

		private double s_Unit_Pos_Y = 0;
		public double Unit_Pos_Y
		{
			get => this.s_Unit_Pos_Y;
			set
			{
				this.Set( ref this.s_Unit_Pos_Y, value, "Unit_Pos_Y" );
			}
		}

		private double s_Unit_Pos_Angle = 0;
		public double Unit_Pos_Angle
		{
			get => this.s_Unit_Pos_Angle;
			set
			{
				this.Set( ref this.s_Unit_Pos_Angle, value, "Unit_Pos_Angle" );
			}
		}

		private double s_CYCLE_TIME = 0;
		public double CYCLE_TIME
		{
			get => this.s_CYCLE_TIME;
			set
			{
				this.Set( ref this.s_CYCLE_TIME, value, "CYCLE_TIME" );
			}
		}

		private double s_MARK_TIME = 0;
		public double MARK_TIME
		{
			get => this.s_MARK_TIME;
			set
			{
				this.Set( ref this.s_MARK_TIME, value, "MARK_TIME" );
			}
		}
		private double s_Holding_Time = 0;
		public double Holding_Time
		{
			get => this.s_Holding_Time;
			set
			{
				this.Set( ref this.s_Holding_Time, value, "Holding_Time" );
			}
		}
		public void UpdateDisplay( AELimitKeys source )
		{
			this.Carrier_SN = source.Carrier_SN;
			this.Mark_Incorrect = source.Mark_Incorrect;
			this.Mark_delta_X = source.Mark_delta_X;
			this.Mark_delta_Y = source.Mark_delta_Y;
			this.Mark_delta_Angle = source.Mark_delta_Angle;
			this.Z_Height = source.Z_Height_Compensation;
			if ( source is AirPodAELimitKeys )
				this.Unit_Width = ( source as AirPodAELimitKeys ).Unit_Arc_Width;
			this.Unit_Orientation = source.Unit_Orientation;
			this.Unit_Pos_X = source.Unit_Pos_X;
			this.Unit_Pos_Y = source.Unit_Pos_Y;
			this.Unit_Pos_Angle = source.Unit_Pos_Angle;
			this.CYCLE_TIME = source.CYCLE_TIME;
			this.MARK_TIME = source.MARK_TIME;
			this.Holding_Time = source.Holding_Time;
		}
	}


	public class MachineParameters : RecipeBaseUtility
	{
		private int iOnline = 0;
		public int Online
		{
			get => this.iOnline;
			set => this.Set( ref this.iOnline, value, "Online" );
		}
		private int iPeak_Voltage = 0;
		public int Peak_Voltage
		{
			get => this.iPeak_Voltage;
			set => this.Set( ref this.iPeak_Voltage, value, "Peak_Voltage" );
		}
		private int iPeak_Current = 0;
		public int Peak_Current
		{
			get => this.iPeak_Current;
			set => this.Set( ref this.iPeak_Current, value, "Peak_Current" );
		}
		private int iActive_energy_current_unit = 0;
		public int Active_energy_current_unit
		{
			get => this.iActive_energy_current_unit;
			set => this.Set( ref this.iActive_energy_current_unit, value, "Active_energy_current_unit" );
		}
		private int iActive_Energy_cumulative = 0;
		public int Active_Energy_cumulative
		{
			get => this.iActive_Energy_cumulative;
			set => this.Set( ref this.iActive_Energy_cumulative, value, "Active_Energy_cumulative" );
		}
	}
	public class ErrorData
	{
		public string message { get; set; }

		public string code { get; set; }

		public string servity { get; set; }

		public string resolution_code { get; set; }

		//Format should be YYYY-mm-ddTHH:MM:ss.SSS±hhmm eg. 2016-01-16T13:59:05.06+0800		
		public string occurrence_time { get; set; }


		//Format should be YYYY-mm-ddTHH:MM:ss.SSS±hhmm eg. 2016-01-16T13:59:05.06+0800
		public string resolved_time { get; set; }

		public errordata data { get; set; }

	}

	public class errordata
	{
		public string key { get; set; }

		public errordetail detail { get; set; }
	}

	public class issue
	{
		public string id { get; set; }
		public string code { get; set; }
		public string severity { get; set; } = "Error";
		public string description { get; set; }
	}

	public class errordetail
	{
		public string station { get; set; }
		public string serial_number { get; set; }
	}


	public class MachineState
	{
		//1=running,2=idle,3=engineering,4=planned, down=5
		public int machine_state { get; set; }

		//Format should be YYYY-mm-ddTHH:MM:ss.SSS±hhmm eg. 2016-01-16T13:59:05.06+0800
		public string state_change_time { get; set; }
		public MachineStateData data { get; set; }
	}

	public class MachineStateData
	{
		public string code { get; set; }
		public string error_message { get; set; }
	}


	public enum machine_state
	{
		Running = 1,
		Idle,
		Engineering,
		PlannedDowntime,
		Down
	}
	public enum ResponseCode
	{
		Found = 200,
		Created = 201,
		RequestInvalid = 400,
		NotFound = 404,
		GeneralServerError = 500
	}
	public class HttpMessage
	{
		public ResponseCode code { get; set; }
		public string message { get; set; }
	}



	public class RestfulAPIClient
	{

		const string STATIONID = "stationId";
		const string ACCEPT = "accept";
		const string JSONAPPLICATION = "application/json";
		HttpClient client = new HttpClient();

		public HttpMessage HttpPost( string sURL, object objMessage, string sStationID = null )
		{
			if ( sStationID != null )
			{
				this.client.DefaultRequestHeaders.Clear();
				this.client.DefaultRequestHeaders.Add( ACCEPT, JSONAPPLICATION );
				this.client.DefaultRequestHeaders.Add( STATIONID, sStationID );
			}

			//return this.ConvertToResponse( this.client.PostAsync( sURL, objMessage ).Result );
			return this.ConvertToResponse( this.client.PostAsJsonAsync( sURL, objMessage ).Result );
		}


		public HttpMessage HttpGet( string sURL, string sStationID )
		{
			this.client.DefaultRequestHeaders.Clear();
			this.client.DefaultRequestHeaders.Add( ACCEPT, JSONAPPLICATION );
			this.client.DefaultRequestHeaders.Add( STATIONID, sStationID );
			return this.ConvertToResponse( this.client.GetAsync( sURL ).Result );
		}


		public HttpMessage DeleteAsync( string path, string sStationID )
		{
			this.client.DefaultRequestHeaders.Clear();
			this.client.DefaultRequestHeaders.Add( STATIONID, sStationID );
			return this.ConvertToResponse( this.client.DeleteAsync( path ).Result );
		}

		HttpMessage ConvertToResponse( HttpResponseMessage httpResponseMessage )
		{
			HttpMessage response = new HttpMessage();
			try
			{
				response.code = ( ResponseCode )httpResponseMessage.StatusCode;
				response.message = httpResponseMessage.Content.ReadAsStringAsync().Result;
			}
			catch
			{
			}
			return response;
		}

	}
	public class HeightLevelDifference : BaseUtility
	{
		private double d_Height_L = 0;
		public double Height_L
		{
			get => this.d_Height_L;
			set => this.Set( ref this.d_Height_L, value, "Height_L" );
		}
		private double d_Height_R = 0;
		public double Height_R
		{
			get => this.d_Height_R;
			set => this.Set( ref this.d_Height_R, value, "Height_R" );
		}
		private double d_Height_C = 0;
		public double Height_C
		{
			get => this.d_Height_C;
			set => this.Set( ref this.d_Height_C, value, "Height_C" );
		}
		private double d_Height_B = 0;
		public double Height_B
		{
			get => this.d_Height_B;
			set => this.Set( ref this.d_Height_B, value, "Height_B" );
		}
		public void Copy( HeightLevelDifference source )
		{
			var properties = source.GetType().GetProperties();
			foreach ( var property in properties )
			{
				this.GetType().GetProperty( property.Name ).SetValue( this, property.GetValue( source, null ) );
			}
		}
	}
	[AttributeUsage( AttributeTargets.Property, Inherited = false, AllowMultiple = false )]
	public sealed class OrderAttribute : Attribute
	{
		private readonly int order_;
		public OrderAttribute( [CallerLineNumber]int order = 0 )
		{
			this.order_ = order;
		}

		public int Order { get { return this.order_; } }
	}
}