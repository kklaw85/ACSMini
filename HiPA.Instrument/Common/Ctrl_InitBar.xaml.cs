using HiPA.Common;
using HiPA.Common.UControl;
using System;
using System.Windows;
using System.Windows.Controls;

namespace HiPA.Instrument.Common
{
	public partial class Ctrl_InitBar : PanelBase
	{
		public Ctrl_InitBar()
		{
			this.InitializeComponent();
			this.MachineStateLockable = false;
			this.BindLockUI( this );
		}
		private InstrumentBase _Instrument = null;
		public InstrumentBase Instrument
		{
			get => this._Instrument;
			set
			{
				try
				{
					this._Instrument = value;
					this.DataContext = this._Instrument;
				}
				catch ( Exception ex )
				{
					Equipment.ErrManager.RaiseWarning( this._Instrument, this.FormatErrMsg( this.Name, ex ), ErrorTitle.InvalidOperation );
				}
			}
		}

		private async void Btn_Click( object sender, RoutedEventArgs e )
		{
			try
			{
				if ( this._Instrument == null ) return;
				var btn = sender as Button;
				if ( btn == this.Btn_Initialize )
				{
					var tsk = this._Instrument.Initialize();
					await tsk;
					if ( tsk.Result != string.Empty )
						throw new Exception( tsk.Result );
				}
				if ( btn == this.Btn_Stop )
				{
					var tsk = this._Instrument.Stop();
					await tsk;
					if ( tsk.Result != string.Empty )
						throw new Exception( tsk.Result );
				}
			}
			catch ( Exception ex )
			{
				Equipment.ErrManager.RaiseError( this._Instrument, this.FormatErrMsg( this.Name, ex ), ErrorTitle.InvalidOperation, ErrorClass.E6 );
			}
		}
	}
}
