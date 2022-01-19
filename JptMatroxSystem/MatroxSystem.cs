﻿using Matrox.MatroxImagingLibrary;
using System;

namespace JptMatroxSystem
{
	public sealed class MatroxSystem
	{
		static MatroxSystem _instance;

		private MIL_ID _applicationID = MIL.M_NULL;

		private MIL_ID _defaultSystemID = MIL.M_NULL;
		private MIL_ID _hostSystemID = MIL.M_NULL;
		private MIL_ID _gigeSystemID = MIL.M_NULL;
		private MIL_ID _usb3SystemID = MIL.M_NULL;
		private MIL_ID _rapixoSystemID = MIL.M_NULL;

		public static MatroxSystem Instance
		{
			get { return _instance ?? ( _instance = new MatroxSystem() ); }
		}

		public static void Free()
		{
			_instance.FreeResources();
		}

		private MatroxSystem()
		{
			if ( this._applicationID == MIL.M_NULL )
			{
				this.InitialiseApplicationID();
			}
		}

		/// <summary>
		/// Initialising MIL Application can only be done once.
		/// </summary>
		/// <returns></returns>
		private string InitialiseApplicationID()
		{
			string sErr = string.Empty;
			try
			{
				if ( this._applicationID == MIL.M_NULL )
				{
					MIL.MappAlloc( MIL.M_NULL, MIL.M_QUIET, ref this._applicationID );

					MIL.MappControl( MIL.M_DEFAULT, MIL.M_ERROR, MIL.M_THROW_EXCEPTION );

					double result = 1;
					MIL.MappInquire( this._applicationID, MIL.M_CURRENT_APPLICATION, ref result );

					if ( result == 0 )
					{
						//throw new Exception("MIL Application can only be initialised once");
					}
				}
			}
			catch ( Exception ex ) { sErr = string.Format( "MatroxSystem_InitialiseApplicationID error: " + ex.Message ); }
			return sErr;
		}

		public MIL_ID ID
		{
			get { return this._applicationID; }
		}


		public MIL_ID DefaultSystemID
		{
			get
			{
				try
				{
					if ( this._defaultSystemID == MIL.M_NULL )
					{
						MIL.MsysAlloc( this._applicationID, "M_SYSTEM_DEFAULT", MIL.M_DEFAULT, MIL.M_DEFAULT, ref this._defaultSystemID );
					}
				}
				catch ( Exception ex )
				{
					string error = string.Format( "MatroxSystem_DefaultSystemID error: " + ex.Message );
					return MIL.M_NULL;
				}
				return this._defaultSystemID;
			}
		}

		public MIL_ID HostSystemID
		{
			get
			{
				try
				{
					if ( this._hostSystemID == MIL.M_NULL )
					{
						MIL.MsysAlloc( this._applicationID, "M_SYSTEM_HOST", MIL.M_DEFAULT, MIL.M_DEFAULT, ref this._hostSystemID );
					}
				}
				catch ( Exception ex )
				{
					string error = string.Format( "MatroxSystem_HostSystemID error: " + ex.Message );
					return MIL.M_NULL;
				}
				return this._hostSystemID;
			}
		}

		public MIL_ID GigeSystemID
		{
			get
			{
				try
				{
					if ( this._gigeSystemID == MIL.M_NULL )
					{
						MIL.MsysAlloc( this._applicationID, "M_SYSTEM_GIGE_VISION", MIL.M_DEFAULT, MIL.M_DEFAULT, ref this._gigeSystemID );
					}
				}
				catch ( Exception ex )
				{
					string error = string.Format( "MatroxSystem_GigeSystemID error: " + ex.Message );
					return MIL.M_NULL;
				}
				return this._gigeSystemID;
			}
		}

		public MIL_ID Usb3SystemID
		{
			get
			{
				try
				{
					if ( this._usb3SystemID == MIL.M_NULL )
					{
						MIL.MsysAlloc( this._applicationID, "M_SYSTEM_USB3_VISION", MIL.M_DEFAULT, MIL.M_DEFAULT, ref this._usb3SystemID );
					}
				}
				catch ( Exception ex )
				{
					string error = string.Format( "MatroxSystem_GigeSystemID error: " + ex.Message );
					return MIL.M_NULL;
				}
				return this._usb3SystemID;
			}
		}

		public MIL_ID RapixoSystemID
		{
			get
			{
				try
				{
					if ( this._rapixoSystemID == MIL.M_NULL )
					{
						MIL.MsysAlloc( this._applicationID, "M_SYSTEM_RAPIXOCXP", MIL.M_DEFAULT, MIL.M_DEFAULT, ref this._rapixoSystemID );
					}
				}
				catch ( Exception ex )
				{
					string error = string.Format( "MatroxSystem_RapixoSystemID error: " + ex.Message );
					return MIL.M_NULL;
				}
				return this._rapixoSystemID;
			}
		}

		public MIL_INT LicenseID
		{
			get
			{
				MIL_INT LicenseModules = 0;
				try
				{
					if ( this._applicationID != MIL.M_NULL )
					{
						MIL.MappInquire( this._applicationID, MIL.M_LICENSE_MODULES, ref LicenseModules );
					}
				}
				catch ( Exception ex )
				{
					string error = string.Format( "MatroxSystem_LicenseID error: " + ex.Message );
				}
				return LicenseModules;
			}
		}

		public void FreeResources()
		{

			if ( this._defaultSystemID != MIL.M_NULL )
			{
				MIL.MsysFree( this._defaultSystemID );
				this._defaultSystemID = MIL.M_NULL;
			}

			//if ( this._hostSystemID != MIL.M_NULL )
			//{
			//	MIL.MsysFree( this._hostSystemID );
			//	this._hostSystemID = MIL.M_NULL;
			//}

			if ( this._gigeSystemID != MIL.M_NULL )
			{
				MIL.MsysFree( this._gigeSystemID );
				this._gigeSystemID = MIL.M_NULL;
			}

			if ( this._usb3SystemID != MIL.M_NULL )
			{
				MIL.MsysFree( this._usb3SystemID );
				this._usb3SystemID = MIL.M_NULL;
			}

			if ( this._rapixoSystemID != MIL.M_NULL )
			{
				MIL.MsysFree( this._rapixoSystemID );
				this._rapixoSystemID = MIL.M_NULL;
			}

			if ( this._applicationID != MIL.M_NULL )
			{
				MIL.MappFree( this._applicationID );
				this._applicationID = MIL.M_NULL;
			}

			// The object has been cleaned up.
			// This call removes the object from the finalization queue and 
			// prevent finalization code object from executing a second time.
			GC.SuppressFinalize( this );
		}
	}
}
