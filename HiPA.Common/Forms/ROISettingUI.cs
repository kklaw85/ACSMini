using HiPA.Common.Utils;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows.Forms;

namespace HiPA.Common.Forms
{
	public partial class ROISettingUI
		: UserControl
		, ILockableUI
		, IROISynchronizer
	{
		public ROISettingUI()
		{
			this.InitializeComponent();

			this.numROI_OffsetX.ValueChanged += this.ROI_ValueChanged;
			this.numROI_OffsetY.ValueChanged += this.ROI_ValueChanged;
			this.numROI_Width.ValueChanged += this.ROI_ValueChanged;
			this.numROI_Height.ValueChanged += this.ROI_ValueChanged;
		}

		SynchronizationContext _ui = null;
		protected override void WndProc( ref Message m )
		{
			if ( m.Msg == ( int )WindowMessage.WM_CREATE )
			{
				this._ui = SynchronizationContext.Current;
			}
			base.WndProc( ref m );
		}

		public RectangleD Area
		{
			get => new RectangleD( this.ROIOffsetX, this.ROIOffsetY, this.ROIWidth, this.ROIHeight );
			set
			{
				this.ROIOffsetX = value.X;
				this.ROIOffsetY = value.Y;
				this.ROIWidth = value.Width;
				this.ROIHeight = value.Height;
			}
		}

		bool _readOnly = false;
		public bool ReadOnly
		{
			get => this._readOnly;
			set
			{
				this._readOnly = value;
				this.chkIsEditable.Enabled = !value;
				this.numROI_OffsetX.ReadOnly = value;
				this.numROI_OffsetY.ReadOnly = value;
				this.numROI_Width.ReadOnly = value;
				this.numROI_Height.ReadOnly = value;
			}
		}

		public double ROIOffsetX
		{
			get => ( double )this.numROI_OffsetX.Value;
			set => this.numROI_OffsetX.Value = ( decimal )value;
		}

		public double ROIOffsetY
		{
			get => ( double )this.numROI_OffsetY.Value;
			set => this.numROI_OffsetY.Value = ( decimal )value;
		}

		public double ROIWidth
		{
			get => ( double )this.numROI_Width.Value;
			set => this.numROI_Width.Value = ( decimal )value;
		}

		public double ROIHeight
		{
			get => ( double )this.numROI_Height.Value;
			set => this.numROI_Height.Value = ( decimal )value;
		}

		private void chkIsEditable_CheckedChanged( object sender, EventArgs e )
		{
			this.AreaStateChanged?.Invoke(
				this,
				new ROISynchronizeEventArgs
				{
					ElementName = this._currentElement,
					Area = null,
					ReadOnly = this.ReadOnly,
					IsVisible = null,
				} );
		}

		bool _valueChangeFromMIL = false;
		public void AreaStateChangedFromDisplay( string elementName, RectangleD area )
		{
			if ( this._elementNames.Contains( elementName ) == false ) return;

			void ac( object o )
			{
				var rt = ( RectangleD )o;
				this._valueChangeFromMIL = true;
				this._currentElement = elementName;
				this.Area = rt;
				this._valueChangeFromMIL = false;
			}

			if ( SynchronizationContext.Current != this._ui ) this._ui?.Post( ac, area );
			else ac( area );
		}

		private void ROI_ValueChanged( object sender, EventArgs e )
		{
			if ( this._valueChangeFromMIL == true ) return;
			if ( string.IsNullOrEmpty( this._currentElement ) == true ) return;

			this.AreaStateChanged?.Invoke(
				this,
				new ROISynchronizeEventArgs
				{
					ElementName = this.CurrentElement,
					Area = new RectangleD( this.ROIOffsetX, this.ROIOffsetY, this.ROIWidth, this.ROIHeight ),
					ReadOnly = null,
					IsVisible = null,
				} );
		}

		public event EventHandler<ROISynchronizeEventArgs> AreaStateChanged;

		List<string> _elementNames = new List<string>();
		public IList<string> ElementNames => this._elementNames;
		string _currentElement = "";
		public string CurrentElement => this._currentElement;


		public event EventHandler<bool> LockStateChangedHandler;
		public bool LockUI
		{
			get => this.ReadOnly;
			set
			{
				if ( this.ReadOnly == value ) return;
				this.ReadOnly = value;
				this.chkIsEditable.Enabled = !value;
				this.LockStateChangedHandler?.Invoke( this, value );
			}
		}

		bool _isVisibleElements = true;
		public bool IsVisibleElements
		{
			get => this._isVisibleElements;
			set
			{
				if ( this._isVisibleElements == value ) return;
				this._isVisibleElements = value;

				if ( this.AreaStateChanged != null )
				{
					foreach ( var name in this._elementNames )
					{
						this.AreaStateChanged.Invoke( this, new ROISynchronizeEventArgs
						{
							ElementName = name,
							Area = null,
							ReadOnly = null,
							IsVisible = value,
						} );
					}
				}
			}
		}

		RectangleD IROISynchronizer.Area
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		//public override bool Equals( object obj )
		//{
		//	if ( obj is string ) return this.Name.Equals( obj );
		//	if ( obj is ROISettingUI roi ) return roi.Name.Equals( this.Name );
		//	return false;
		//}
		//public override int GetHashCode()
		//{
		//	return this.Name.GetHashCode();
		//}
	}
}
