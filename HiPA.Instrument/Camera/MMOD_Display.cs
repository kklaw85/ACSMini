using JptMatroxSystem;
using Matrox.MatroxImagingLibrary;
using System;
using System.Windows.Forms.Integration;

namespace HiPA.Instrument.Camera
{
	public class MMOD_Display : WindowsFormsHost
	{
		private MIL_ID _mdisp = MIL.M_NULL;
		private MIL_ID img = MIL.M_NULL;
		private static MatroxSystem _appID = MatroxSystem.Instance;
		public MIL_ID m_system = _appID.HostSystemID;
		public MMOD_Display()
		{
			try
			{
				if ( this._mdisp == MIL.M_NULL )
				{
					MIL.MdispAlloc( this.m_system, MIL.M_DEFAULT, "M_DEFAULT", MIL.M_WINDOWED, ref this._mdisp );
				}
				MIL.MdispControl( this._mdisp, MIL.M_CENTER_DISPLAY, MIL.M_ENABLE );

				if ( this == null )
					throw new ArgumentNullException( "displayHost", "Must be a valid object." );
			}
			catch
			{ }
		}
		public void DisplayBuffer( MIL_ID milbuf )
		{
			try
			{
				if ( this.img != MIL.M_NULL )
				{
					MIL.MbufFree( this.img );
					this.img = MIL.M_NULL;
				}
				if ( this == null )
					throw new ArgumentNullException( "displayHost", "Must be a valid object." );
				// Set Display to scaled mode
				MIL.MdispControl( this._mdisp, MIL.M_SCALE_DISPLAY, MIL.M_ENABLE );
				MIL.MdispSelectWPF( this._mdisp, milbuf, this );
			}
			catch
			{ }
		}
		protected override void Dispose( bool disposing )
		{
			base.Dispose( disposing );
			if ( disposing )
			{
				if ( this._mdisp != MIL.M_NULL )
				{
					MIL_ID selectedBufferId = ( MIL_ID )MIL.MdispInquire( this._mdisp, MIL.M_SELECTED, MIL.M_NULL );
					if ( selectedBufferId != MIL.M_NULL )
					{

					}
					MIL.MdispFree( this._mdisp );
					this._mdisp = MIL.M_NULL;
				}
			}
		}

	}
}
