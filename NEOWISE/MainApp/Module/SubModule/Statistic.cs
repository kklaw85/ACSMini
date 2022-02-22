using HiPA.Common.Forms;
using System;

namespace NeoWisePlatform.Module
{
	[Serializable]
	public class Statistic
		: BaseUtility
	{
		private PNPModuleConfiguration PNPCfg = null;
		#region timing
		private TicToc Timer = new TicToc();
		public double TotalRunTime
		{
			get => this.GetValue( () => this.TotalRunTime );
			set => this.SetValue( () => this.TotalRunTime, value );
		}
		public double TPU
		{
			get => this.GetValue( () => this.TPU );
			set => this.SetValue( () => this.TPU, value );
		}
		public double UPH
		{
			get => this.GetValue( () => this.UPH );
			set => this.SetValue( () => this.UPH, value );
		}
		#endregion
		#region Mat Qty
		public int TotalProcessedQty
		{
			get => this.GetValue( () => this.TotalProcessedQty );
			set
			{
				this.SetValue( () => this.TotalProcessedQty, value );
				this.TPU = this.TotalRunTime / this.TotalProcessedQty;
				this.UPH = ( double )this.TotalProcessedQty / ( double )this.TotalRunTime * 3600;
				this.NGPerc = ( double )this.NGQty / ( double )this.TotalProcessedQty * 100;
				this.KIVPerc = ( double )this.KIVQty / ( double )this.TotalProcessedQty * 100;
				this.QICPerc = ( double )this.QICQty / ( double )this.TotalProcessedQty * 100;
			}
		}
		public int KIVQty
		{
			get => this.GetValue( () => this.KIVQty );
			set => this.SetValue( () => this.KIVQty, value );
		}
		public int NGQty
		{
			get => this.GetValue( () => this.NGQty );
			set => this.SetValue( () => this.NGQty, value );
		}
		public int QICQty
		{
			get => this.GetValue( () => this.QICQty );
			set => this.SetValue( () => this.QICQty, value );
		}
		public double KIVPerc
		{
			get => this.GetValue( () => this.KIVPerc );
			set => this.SetValue( () => this.KIVPerc, value );
		}
		public double NGPerc
		{
			get => this.GetValue( () => this.NGPerc );
			set => this.SetValue( () => this.NGPerc, value );
		}
		public double QICPerc
		{
			get => this.GetValue( () => this.QICPerc );
			set => this.SetValue( () => this.QICPerc, value );
		}
		#endregion
		public void LinkPNPCfg( PNPModuleConfiguration cfg )
		{
			this.PNPCfg = cfg;
		}
		private void Clear()
		{
			this.TotalRunTime = 0;
			this.KIVQty = 0;
			this.NGQty = 0;
			this.QICQty = 0;
			this.TotalProcessedQty = 0;
		}
		public void Start()//needs to be run when autorun is triggered
		{
			try
			{
				this.Timer.Tic();
			}
			catch ( Exception ex )
			{

			}
		}
		private void Restart()
		{
			try
			{
				this.TotalRunTime += this.Timer.Toc().Elapsed.TotalSeconds;
				this.Timer.Tic();
			}
			catch ( Exception ex )
			{

			}
		}
		public void Pause()//execute when autorun is stopped
		{
			try
			{
				this.TotalRunTime += this.Timer.Toc().Elapsed.TotalSeconds;
				this.Timer.Clear();
			}
			catch ( Exception ex )
			{

			}
		}
		public void Stop()//execute when autorun is stopped
		{
			try
			{
				this.Timer.Clear();
			}
			catch ( Exception ex )
			{

			}
		}
		private void AddQIC()
		{
			this.Restart();//update timer first before qty
			this.QICQty++;
			this.TotalProcessedQty++;
		}
		private void AddNG()
		{
			this.Restart();//update timer first before qty
			this.NGQty++;
			this.TotalProcessedQty++;
		}
		private void AddKIV()
		{
			this.Restart();//update timer first before qty
			this.KIVQty++;
			this.TotalProcessedQty++;
		}
		public void Reset()
		{
			this.Stop();
			this.Clear();
		}
		public void Update( eInspResult Res )
		{
			if ( Res == eInspResult.KIV ) this.AddKIV();
			else if ( Res == eInspResult.NG ) this.AddNG();
			else if ( Res == eInspResult.QIC ) this.AddQIC();
			else
			{
				if ( this.PNPCfg.UnInspResult == UninspResult.KIV ) this.AddKIV();
				else if ( this.PNPCfg.UnInspResult == UninspResult.NG ) this.AddNG();
			}
		}
	}
}
