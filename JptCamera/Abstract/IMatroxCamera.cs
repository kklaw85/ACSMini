using Matrox.MatroxImagingLibrary;
using System;
using System.Windows.Forms.Integration;

namespace JptCamera
{
	public interface IMatroxCamera
	{

		string InitialiseMatrox( MIL_ID SystemID );

		string SetDisplayWinForm( IntPtr Handler );

		string SetDisplayWPF( WindowsFormsHost DisplayFormHost );

		string SetDisplayWPFOFF();

		string Load2DImageFromFile( string Filename );

		MIL_ID MILImage { get; }

		void OnImageReceive( EventHandler<JptCamera.MatroxDigitizer.NewDataEventArg> NewImageReceived );

		void OnImageReceivedRemove( EventHandler<JptCamera.MatroxDigitizer.NewDataEventArg> NewImageReceived );


	}
}
