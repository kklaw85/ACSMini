using HiPA.Common.Forms;
using HiPA.Common.Utils;
using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;

namespace HiPA.Common
{
	public class MultilingualErrModule : BaseUtility
	{
		protected static string Name = "Multilingual Error Module";
		protected static XmlDictionary<int, string> ReferenceErrList { get; set; } = new XmlDictionary<int, string>();
		public static XmlDictionary<int, string> RuntimeErrList { get; set; } = new XmlDictionary<int, string>();
		public static XmlDictionary<int, string> RuntimeErrENGList { get; set; } = new XmlDictionary<int, string>();
		public Task<string> InitLoadErrorList()
		{
			return Task<string>.Run( () =>
			{
				var result = string.Empty;
				try
				{
					//Generate ENG Ref
					foreach ( var pair in ReflectionTool.GetEnumValueDesc( typeof( RunErrors ) ) )
					{
						ReferenceErrList.Add( pair.Value, pair.Desc );
						RuntimeErrENGList.Add( pair.Value, pair.Desc );
					}

					//Check Multi-Lang Files
					if ( !File.Exists( Path.Combine( Constructor.GetInstance().SystemFilesPath, "ErrorListen-EN.xml" ) ) )
						if ( ( result = this.GenerateErrEN().Result ) != string.Empty )
							throw new Exception( result );
					if ( !File.Exists( Path.Combine( Constructor.GetInstance().SystemFilesPath, "ErrorListen-CZ.xml" ) ) )
						if ( ( result = this.GenerateErrCZ().Result ) != string.Empty )
							throw new Exception( result );
					if ( !File.Exists( Path.Combine( Constructor.GetInstance().SystemFilesPath, "ErrorListen-CHS.xml" ) ) )
						if ( ( result = this.GenerateErrCHS().Result ) != string.Empty )
							throw new Exception( result );
				}
				catch ( Exception ex )
				{
					result = this.FormatErrMsg( Name, ex );
					Equipment.ErrManager.RaiseError( null, result, ErrorTitle.OperationFailure, ErrorClass.F );
				}
				try
				{
					result = string.Empty;
					//Load Default ENG Error List
					RuntimeErrList.Clear();
					if ( ( result = this.RetrieveErrEN().Result ) != string.Empty )
						throw new Exception( result );

					if ( ( result = this.LoadErrLang( "EN" ).Result ) != string.Empty )
						throw new Exception( result );
				}
				catch ( Exception ex )
				{
					result = this.FormatErrMsg( Name, ex );
					Equipment.ErrManager.RaiseError( null, result, ErrorTitle.OperationFailure, ErrorClass.F );
				}
				return result;
			} );
		}
		public Task<string> LoadErrLang( string lang )
		{
			return Task<string>.Run( () =>
			{
				var result = string.Empty;
				try
				{
					result = string.Empty;
					RuntimeErrList.Clear();
					switch ( lang )
					{
						case "zh-CHS":
							if ( ( result = this.RetrieveErrCHS().Result ) != string.Empty )
								throw new Exception( result );
							break;
						case "cs-CZ":
							if ( ( result = this.RetrieveErrCZ().Result ) != string.Empty )
								throw new Exception( result );
							break;
						default: //ENG
							if ( ( result = this.RetrieveErrEN().Result ) != string.Empty )
								throw new Exception( result );
							break;
					}
					if ( ( result = this.CheckErrList( lang ).Result ) != string.Empty )
						throw new Exception( result );
				}
				catch ( Exception ex )
				{
					result = this.FormatErrMsg( Name, ex );
					Equipment.ErrManager.RaiseError( null, result, ErrorTitle.OperationFailure, ErrorClass.F );
				}
				return result;
			} );
		}
		protected Task<string> CheckErrList( string lang )
		{
			return Task<string>.Run( () =>
			{
				var result = string.Empty;
				try
				{
					result = string.Empty;
					ReferenceErrList.Clear();
					switch ( lang ) //Load Software Default Ref Lang
					{
						case "zh-CHS":
							foreach ( var pair in ReflectionTool.GetEnumValueDesc( typeof( RunErrorsCHS ) ) )
								ReferenceErrList.Add( pair.Value, pair.Desc );
							break;
						case "cs-CZ":
							foreach ( var pair in ReflectionTool.GetEnumValueDesc( typeof( RunErrorsCZ ) ) )
								ReferenceErrList.Add( pair.Value, pair.Desc );
							break;
						default: //ENG
							foreach ( var pair in ReflectionTool.GetEnumValueDesc( typeof( RunErrors ) ) )
								ReferenceErrList.Add( pair.Value, pair.Desc );
							break;
					}

					//Validate the Contents of the loaded Lang File with Software Default Ref Lang.
					var diff = ReferenceErrList.Except( RuntimeErrList );
					if ( diff.Count() != 0 )
					{
						foreach ( var ele in diff )
							Console.WriteLine( $"{ele.Value.ToString()}" );


						//Re-Generate Existing selected Lang's Default Lang Error List
						RuntimeErrList = ReferenceErrList;

						//Notify User
						Equipment.ErrManager.RaiseWarning( $"{ReferenceErrList[ ( int )RunErrors.ERR_LoadLangErr ]}", ErrorTitle.MultiLanguageErr );

						//Replace Current RuntimeErrList with newly Generated Default Lang Error List
						switch ( lang )
						{
							case "zh-CHS":
								if ( ( result = this.GenerateErrCHS().Result ) != string.Empty )
									throw new Exception( result );
								break;
							case "cs-CZ":
								if ( ( result = this.GenerateErrCZ().Result ) != string.Empty )
									throw new Exception( result );
								break;
							default: //ENG
								if ( ( result = this.GenerateErrEN().Result ) != string.Empty )
									throw new Exception( result );
								break;
						}
					}
				}
				catch ( Exception ex )
				{
					result = this.FormatErrMsg( Name, ex );
					Equipment.ErrManager.RaiseError( null, result, ErrorTitle.OperationFailure, ErrorClass.F );
				}
				return result;
			} );
		}
		#region Retrieving and Generate Error List
		#region ENGLISH
		protected Task<string> GenerateErrEN()
		{
			return Task<string>.Run( () =>
			{
				var result = string.Empty;
				try
				{
					var EngFP = Path.Combine( Constructor.GetInstance().SystemFilesPath, "ErrorListen-EN.xml" );
					RuntimeErrList.Clear();
					foreach ( var pair in ReflectionTool.GetEnumValueDesc( typeof( RunErrors ) ) )
						RuntimeErrList.Add( pair.Value, pair.Desc );

					var settings = new XmlWriterSettings();
					settings.ConformanceLevel = ConformanceLevel.Fragment;
					settings.Indent = true;
					settings.NewLineOnAttributes = true;
					var wr = XmlWriter.Create( EngFP, settings );

					RuntimeErrList.WriteXmlGeneric( wr );
					wr.Close();
				}
				catch ( Exception ex )
				{
					result = this.FormatErrMsg( Name, ex );
					Equipment.ErrManager.RaiseError( null, result, ErrorTitle.OperationFailure, ErrorClass.F );
				}
				return result;
			} );
		}
		protected Task<string> RetrieveErrEN()
		{
			return Task<string>.Run( () =>
			{
				var result = string.Empty;
				try
				{
					var EngFP = Path.Combine( Constructor.GetInstance().SystemFilesPath, "ErrorListen-EN.xml" );

					var settings = new XmlReaderSettings();
					settings.ConformanceLevel = ConformanceLevel.Fragment;
					var rd = XmlReader.Create( EngFP, settings );

					RuntimeErrList.ReadXmlGeneric( rd );
					rd.Close();
				}
				catch ( Exception ex )
				{
					result = this.FormatErrMsg( Name, ex );
					Equipment.ErrManager.RaiseError( null, result, ErrorTitle.OperationFailure, ErrorClass.F );
				}
				return result;
			} );
		}
		#endregion
		#region CZECH 
		protected Task<string> GenerateErrCZ()
		{
			return Task<string>.Run( () =>
			{
				var result = string.Empty;
				try
				{

					var EngFP = Path.Combine( Constructor.GetInstance().SystemFilesPath, "ErrorListen-CZ.xml" );
					RuntimeErrList.Clear();
					foreach ( var pair in ReflectionTool.GetEnumValueDesc( typeof( RunErrorsCZ ) ) )
						RuntimeErrList.Add( pair.Value, pair.Desc );

					var settings = new XmlWriterSettings();
					settings.ConformanceLevel = ConformanceLevel.Fragment;
					settings.Indent = true;
					settings.NewLineOnAttributes = true;
					var wr = XmlWriter.Create( EngFP, settings );

					RuntimeErrList.WriteXmlGeneric( wr );
					wr.Close();
				}
				catch ( Exception ex )
				{
					result = this.FormatErrMsg( Name, ex );
					Equipment.ErrManager.RaiseError( null, result, ErrorTitle.OperationFailure, ErrorClass.F );
				}
				return result;
			} );
		}
		protected Task<string> RetrieveErrCZ()
		{
			return Task<string>.Run( () =>
			{
				var result = string.Empty;
				try
				{
					var EngFP = Path.Combine( Constructor.GetInstance().SystemFilesPath, "ErrorListen-CZ.xml" );

					var settings = new XmlReaderSettings();
					settings.ConformanceLevel = ConformanceLevel.Fragment;
					var rd = XmlReader.Create( EngFP, settings );

					RuntimeErrList.ReadXmlGeneric( rd );
					rd.Close();
				}
				catch ( Exception ex )
				{
					result = this.FormatErrMsg( Name, ex );
					Equipment.ErrManager.RaiseError( null, result, ErrorTitle.OperationFailure, ErrorClass.F );
				}
				return result;
			} );
		}
		#endregion
		#region CHINESE Simplified 
		protected Task<string> GenerateErrCHS()
		{
			return Task<string>.Run( () =>
			{
				var result = string.Empty;
				try
				{

					var EngFP = Path.Combine( Constructor.GetInstance().SystemFilesPath, "ErrorListen-CHS.xml" );
					RuntimeErrList.Clear();
					foreach ( var pair in ReflectionTool.GetEnumValueDesc( typeof( RunErrorsCHS ) ) )
						RuntimeErrList.Add( pair.Value, pair.Desc );

					var settings = new XmlWriterSettings();
					settings.ConformanceLevel = ConformanceLevel.Fragment;
					settings.Indent = true;
					settings.NewLineOnAttributes = true;
					var wr = XmlWriter.Create( EngFP, settings );

					RuntimeErrList.WriteXmlGeneric( wr );
					wr.Close();
				}
				catch ( Exception ex )
				{
					result = this.FormatErrMsg( Name, ex );
					Equipment.ErrManager.RaiseError( null, result, ErrorTitle.OperationFailure, ErrorClass.F );
				}
				return result;
			} );
		}
		protected Task<string> RetrieveErrCHS()
		{
			return Task<string>.Run( () =>
			{
				var result = string.Empty;
				try
				{
					var EngFP = Path.Combine( Constructor.GetInstance().SystemFilesPath, "ErrorListen-CHS.xml" );

					var settings = new XmlReaderSettings();
					settings.ConformanceLevel = ConformanceLevel.Fragment;
					var rd = XmlReader.Create( EngFP, settings );

					RuntimeErrList.ReadXmlGeneric( rd );
					rd.Close();
				}
				catch ( Exception ex )
				{
					result = this.FormatErrMsg( Name, ex );
					Equipment.ErrManager.RaiseError( null, result, ErrorTitle.OperationFailure, ErrorClass.F );
				}
				return result;
			} );
		}
		#endregion
		#endregion
	}

	#region CZECH Errors
	public enum RunErrorsCZ
	{
		//0- no error
		//-1 to -100 for timeouts
		//-101 to -200 for thread inconformity error
		//-201 to -300 for input
		//-301 to -400 for output
		//-401 to -500 for cylinder
		//-501 to -600 for adlinkmotion
		//-601 to -700 for vision
		//-701 to -800 for servercom
		//-801 to -900 for conveyor
		//-901 to -1000 for heightsensor
		//-1001 to -1100 for laser marking
		//-1101 to -1200 for barcode
		//-1201 to -1300 for recipelink
		//-1301 to -1400 for image handling
		//-4001 to -5000 for checking of other thread error by Conveyorout.
		//-9001 to -10000 for autorunseq exception
		//-10001 to -20000 for laserhead exception
		//-20001 to -30000 for ConveyorIn exception
		//-30001 to -40000 for ConveyorOut exception
		//-40001 to -50000 for Smemainlet exception
		//-50001 to -60000 for Smemaoutlet exception
		//-60001 to -70000 for Common exception
		//-70001 to -80000 for Tray exception
		//-80001 to -90000 for Shopfloor exception
		//-10000001 to -20000000 B262 Vision Processing
		//-20000001 to -30000000 HiPA Common
		//-30000001 to -40000000 HiPA Instrument
		//-40000001 to -50000000 JPT Camera
		//-50000001 to -60000000 JPT Matrox System
		//-60000001 to -70000000 Neowise
		//....and other exception

		[Description( "Žádná chyba" )]
		ERR_NoError = ( 0 ),      //No Error	

		#region timeouts
		[Description( "Časový limit pohybu" )]
		ERR_MoveTimeout = ( -1 ),   // 

		[Description( "Časový limit IO" )]
		ERR_IOTimeout = ( -2 ),   // 

		[Description( "Časový limit funkce" )]
		ERR_FunctionTimeout = ( -3 ),   // 
		#endregion
		#region Threadstatus inconformity
		[Description( "Neshoda stavu vlákna" )]
		ERR_Inconformity = ( -101 ),
		[Description( "Neočekávaná chyba výjimky" )]
		ERR_UnexpectedException = ( -102 ),
		//[Description( "Neshoda stavu podprocesu komunikačního vlákna" )]
		//ERR_Inconformity_ShopFloorSeq = ( -101 ),   //

		//[Description( "Neshoda stavu se závitem laserové hlavy" )]
		//ERR_Inconformity = ( -102 ),

		//[Description( "Neshoda stavu vlákna dopravníku" )]
		//ERR_Inconformity_ConveyorOutSeq = ( -103 ),   // 

		//[Description( "Neshoda stavu dopravníku ve vlákně" )]
		//ERR_Inconformity_ConveyorInSeq = ( -104 ),   // 

		//[Description( "Neshoda stavu vlákna Smema" )]
		//ERR_Inconformity_SmemaSeq = ( -105 ),   // 

		//[Description( "Neshoda stavu podprocesu zásobníku" )]
		//ERR_Inconformity_TraySeq = ( -106 ),
		#endregion
		#region Input failure
		[Description( "Chyba při čtení Adlinku" )]
		ERR_ReadInputIOErr = ( -201 ),   // 
		#endregion
		#region Output failure
		[Description( "Set Adlink Out Error" )]
		ERR_SetOutputIOErr = ( -301 ),   // 
		#endregion
		#region IO Motion
		[Description( "Chyba pohybu válce" )]
		ERR_CylinderMotionErr = ( -401 ), //
		[Description( "Chyba pohybu dveří válce" )]
		ERR_CylinderDrMotionErr = ( -402 ), //
		[Description( "Chyba pohybu válcového lisu" )]
		ERR_CylinderProdPressMotionErr = ( -403 ), //


		// [Description("Reset All Motion IO Failed")]
		// ERR_ResetAllFail = (-1001), //
		// [Description("Poloha uvolnění zátky vstupu selhala")]]
		// ERR_InletStopperResetFail = (-1002), //
		// [Description("Pozice bloku zarážky vstupu selhala")]]
		// ERR_InletStopperMoveToPosFail = (-1003), //
		// [Description("Uvolnění zátky zásuvky se nezdařilo")]
		// ERR_OutletStopperResetFail = (-1004), //
		// [Description("Pozice bloku výstupního uzávěru selhala")]
		// ERR_OutletStopperMoveToPosFail = (-1005), //
		// [Description("Stažení produktu se nezdařilo")]
		// ERR_ProductPresserResetFail = (-1006), //
		// [Description("Produktový lis se přesunul do pracovní polohy se nezdařil")]
		// ERR_ProductPresserMoveToPosFail = (-1007), //
		// [Description("Chyba zavírání dveří závěrky")]
		// ERR_ShutterDoorClose = (-1008),
		// [Description("Chyba otevřených dveří závěrky")]]
		// ERR_ShutterDoorOpen = (-1009),
		#endregion
		#region adlink motion
		[Description( "Chyba pohybu osy" )]
		ERR_AxisMoveErr = ( -501 ), //
									// [Description("Pomalý start výstupu dopravníku selhal")]
									// ERR_ConveyorOutStartSlowFail = (-3001), //
									// [Description("Rychlé spuštění výstupu dopravníku selhalo")]
									// ERR_ConveyorOutStartFastFail = (-3002), //
									// [Description("Selhání výstupu dopravníku selhalo")]
									// ERR_ConveyorOutStopFail = (-3003), //
									// [Description("LiftZ přesun do pohotovostního režimu selhal")]
									// ERR_LiftZMoveToStandbyPosFailed = (-3004), //
									// [Description("LiftZ Move To Work Pos Failed")]
									// ERR_LiftZMoveToWorkPosFailed = (-3005), //
									// [Description("Laser Head Move XYZ Failed")]]
									// ERR_LHMoveXYZFailed = (-3006), //
									// [Description("Laser Head Move X Failed")]
									// ERR_LHMoveXFailed = (-3007), //
									// [Description("Laserový pohyb hlavy Y selhal")]
									// ERR_LHMoveYFailed = (-3008), //
									// [Description("Laser Head Move Z Failed")]]
									// ERR_LHMoveZFailed = (-3009), //
		#endregion
		#region Vision
		[Description( "Chyba Grab All Camera" )]
		ERR_CameraGrabAllError = ( -601 ), //
		[Description( "Chyba kontroly orientace" )]
		ERR_VisionOrientationCheckErr = ( -602 ),
		[Description( "Orientace: Nenalezeno" )]
		ERR_VisionOrientationNotFoundErr = ( -603 ),
		[Description( "Chyba kontroly zarovnání" )]
		ERR_VisionAlignmentCheckErr = ( -604 ),
		[Description( "Nesprávná šířka produktu" )]
		ERR_ProductWidthErr = ( -605 ),
		[Description( "Chyba kontroly obsahu" )]
		ERR_ContentChkErr = ( -606 ),
		[Description( "Překročení limitu otáčení Y" )]
		ERR_ExceedRotYLimitErr = ( -607 ),
		[Description( "Nesprávný úhel produktu" )]
		ERR_ProductAngleErr = ( -608 ),
		#endregion
		#region ServerComm
		//ShopFloor Communication Seq.
		// ShopFloor Communication Seq.
		[Description( "Při načítání obrazu z Shop Floor došlo k chybě" )]
		ERR_ShopFloor_Retrieve_Img = ( -701 ), //
		[Description( "Při načítání Cavity No došlo k chybě" )]
		ERR_ShopFloor_Retrieve_CavityNo = ( -702 ), //
		[Description( "Při zveřejňování výsledku do Shop Floor došlo k chybě" )]
		ERR_ShopFloor_Post_Result = ( -703 ), //
		[Description( "Žádná data dutiny nevyryjí chybu" )]
		ERR_NoCavityDataToEngrave = ( -704 ), // pouze během simulačního režimu
		#endregion
		#region Conveyor
		[Description( "Deska zaseknutá na vstupu" )]
		ERR_Board_Jammed_Inlet = ( -801 ), //
		[Description( "Board Jammed at Outlet" )]
		ERR_Board_Jammed_Outlet = ( -802 ), //
		[Description( "Dopravník není prázdný" )]
		ERR_ConveyorNotEmpty = ( -803 ), //
		#endregion
		#region HeightSensor
		[Description( "chyba HeightSensor" )]
		ERR_HeightSensorError = ( -901 ), //
		[Description( "Překročení limitu kompenzace" )]
		ERR_ExceededCompensationLimitError = ( -902 ), //
		#endregion
		#region Laser
		[Description( "Chyba ověření kalibrovaného výkonu laseru" )]
		ERR_CalibLaserPwrValidErr = ( -1001 ),
		[Description( "chyba značení" )]
		ERR_LaserMarkErr = ( -1002 ),
		#endregion
		#region Barcode
		[Description( "Chyba čtečky čárových kódů" )]
		ERR_BarCodeReaderErr = ( -1101 ),   // 
		#endregion
		#region Recipesetting
		[Description( "Cavity No. not found in Cavity / WorkPos Idx. Link" )]
		ERR_CavityNoNotFound = ( -1201 ),
		[Description( "Recept nebyl nalezen v odkazu Barva / Typ produktu" )]
		ERR_RecipeNotFound = ( -1202 ),
		#endregion
		#region Imagehandling
		[Description( "Chyba při odeslání obrazového souboru" )]
		ERR_ImageFilePostActionErr = ( -1301 ),
		#endregion

		[Description( "Vstup dopravníku obsahuje chybu" )]
		ERR_InletContainsErr = ( -4001 ), // přípona, není třeba zmínit
		[Description( "SMEMA Outlet obsahuje chybu" )]
		ERR_SMEMAOutContainsErr = ( -4002 ), // přípona, není třeba zmínit
		[Description( "Vstup SMEMA obsahuje chybu" )]
		ERR_SMEMAInContainsErr = ( -4003 ), // rozšíření, není třeba zmínit
		[Description( "ShopFloor obsahuje chybu" )]
		ERR_ShopFloorContainsErr = ( -4004 ), // přípona, není třeba zmínit

		#region autoseq
		// Auto Seq
		[Description( "Počkejte na chybu IO tlačítka Start" )]
		ERR_StartBtnIOErr = ( -9001 ),
		[Description( "Chyba nastavení zámku dveří" )]
		ERR_SetDoorLockErr = ( -9002 ),
		[Description( "Zkontrolovat chybu zámku dveří" )]
		ERR_CheckDoorLockErr = ( -9003 ),
		[Description( "Set All Station to Ready Error" )]
		ERR_SetAllStationReadyErr = ( -9004 ),
		#endregion
		#region laserheadseqexception
		//Laser Head Seq.
		[Description( "Čekání na pracovní chybu" )]
		ERR_WaitForWorkErr = ( -10001 ),
		[Description( "Wait Lift in Pos. Error" )]
		ERR_WaitLiftInPosErr = ( -10002 ),
		[Description( "Při kontrole výšky došlo k chybě" )]
		ERR_ScanHeight = ( -10003 ),
		[Description( "Při kontrole zraku v sekvenci laserové hlavy došlo k chybě" )]
		ERR_VisionCheckError = ( -10004 ),
		[Description( "Došlo k chybě při zavření čekajících dveří v sekvenci laserové hlavy" )]
		ERR_WaitDoorClosedError = ( -10005 ),
		[Description( "Do Marking Error" )]
		ERR_DoMarkingErr = ( -10006 ),
		[Description( "Čekejte na chybu Hotovo CC" )]
		ERR_WaitForCCDoneErr = ( -10007 ),
		[Description( "Do CC Error" )]
		ERR_DoCCErr = ( -10008 ),
		[Description( "Pokračovat v další dutině" )]
		ERR_ContinueNextCavityErr = ( -10009 ),
		[Description( "Počkejte na všechny palety" )]
		ERR_WaitForALLPalletsErr = ( -10010 ),
		[Description( "Chyba IO dveří závěrky; zkontrolujte IO dveří závěrky" )]
		ERR_ShutterDoorIOError = ( -10011 ),
		#endregion
		#region ConveyorInSeqexception
		//ConveyorInlet Seq
		[Description( "Chyba vstupu SMEMA Work" )]
		ERR_InletSMEMAWorkErr = ( -20002 ),
		[Description( "Chyba kontroly vstupu sběrače" )]
		ERR_InletCheckRunConveyorFlagErr = ( -20003 ),
		[Description( "Čekejte na chybu snímače palety 2" )]
		ERR_WaitPalletSen2Err = ( -20004 ),
		[Description( "chyba při kontrole čárového kódu" )]
		ERR_BarCodeCheckErr = ( -20005 ),
		[Description( "Chyba přesunu k senzoru palety 1" )]
		ERR_MovePalletSen1Err = ( -20006 ),
		[Description( "Zkontrolovat chybu snímače palety 1" )]
		ERR_CheckPalletSen1Err = ( -20007 ),
		[Description( "Čekejte na chybu připraveného vstupu SMEMA" )]
		ERR_WaitForSMEMAReadyErr = ( -20008 ),
		[Description( "Chyba kontroly vstupních palet všech senzorů" )]
		ERR_InletCheckPalletAllSenErr = ( -20009 ),
		#endregion
		#region ConveyorOutSeqexception
		[Description( "Chyba stavu kontroly výstupu" )]
		ERR_OutletCheckStateErr = ( -30002 ),
		[Description( "Chyba dopravníku v akci" )]
		ERR_ConveyorInActionErr = ( -30003 ),
		[Description( "Čekejte na chybu palety 1" )]
		ERR_WaitForPallet1Err = ( -30004 ),
		[Description( "Načíst chybu čárového kódu" )]
		ERR_RetrieveBarCodeErr = ( -30005 ),
		[Description( "Chyba při spuštění serveru" )]
		ERR_StartServerCommErr = ( -30006 ),
		[Description( "Čekejte na sériovou chybu produktu" )]
		ERR_WaitProdSNErr = ( -30007 ),
		[Description( "Chyba zvedání Z do pracovní polohy" )]
		ERR_ZWorkPosErr = ( -30008 ),
		[Description( "Čekejte na chybu při dokončení značky" )]
		ERR_WaitMarkCompleteErr = ( -30009 ),
		[Description( "Chyba výsledku odeslání ShopFloor" )]
		ERR_ShopFloorSendResultErr = ( -30010 ),
		[Description( "Chyba zvedání Z do pohotovostní polohy" )]
		ERR_ZStandbyPosErr = ( -30011 ),
		[Description( "Chyba všech senzorů kontroly výstupní palety" )]
		ERR_OutletCheckPalletAllSenErr = ( -30012 ),
		[Description( "Outlet Start Smema to Action Error" )]
		ERR_OutletSMEMAIsActionErr = ( -30013 ),
		[Description( "Uvolnit chybu všech 3 palet" )]
		ERR_Release3PalletsErr = ( -30014 ),
		[Description( "Chyba palety 1 na 1" )]
		ERR_Release1By1PalletErr = ( -30015 ),
		[Description( "Chyba palety vydání 1" )]
		ERR_ReleasePallet1Err = ( -30016 ),
		[Description( "Chyba čekací desky k chybě čidla výstupu" )]
		ERR_BoardToOutletSenErr = ( -30017 ),
		[Description( "Spustit chybu uvolnění" )]
		ERR_StartReleasingErr = ( -30018 ),
		[Description( "Chyba posunu palety 2 na 1" )]
		ERR_ShiftPallet2To1Err = ( -30019 ),
		[Description( "Čekejte na chybu vypnutého senzoru palety 3" )]
		ERR_WaitPallet3SenseOffErr = ( -30020 ),
		[Description( "Počkejte na chybu příznaku Smema Out" )]
		ERR_WaitSmemaFlagErr = ( -30021 ),


		#endregion
		#region Smemainletexception
		//Smema Inlet Seq.
		[Description( "Smema čeká na přijetí chyby" )]
		ERR_WaitingToAcceptErr = ( -40001 ),
		[Description( "Chyba přijímání" )]
		ERR_AcceptErr = ( -40002 ),
		#endregion
		#region Smemaoutletexception
		//Smema Outlet Seq.
		[Description( "Čekání na uvolnění chyby" )]
		ERR_WaitingToReleaseErr = ( -50001 ),
		[Description( "Chyba při uvolnění zásuvky Smema" )]
		ERR_SmemaReleasingErr = ( -50002 ),
		#endregion
		#region Commonexception
		[Description( "Chyba inicializace" )]
		ERR_InitErr = ( -60001 ),
		[Description( "Chyba po inicializaci" )]
		ERR_PostInitErr = ( -60002 ),
		[Description( "chyba IsAction" )]
		ERR_IsActionErr = ( -60003 ),
		[Description( "Dokončit chybu" )]
		ERR_FinishErr = ( -60004 ),
		#endregion
		#region TraySeqException
		//Tray Seq.
		[Description( "OUT OUT Tray Sensor of Error" )]
		ERR_ProxSenOutErr = ( -70001 ),
		[Description( "Chyba snímače přiblížení v zásobníku" )]
		ERR_ProxSenInErr = ( -70002 ),
		[Description( "Chyba zarážky při aktivaci zásobníku" )]
		ERR_ActStopperErr = ( -70003 ),
		[Description( "Chyba při kontrole názvu souboru v zásobníku" )]
		ERR_ChkFileNameErr = ( -70004 ),
		[Description( "Chyba uložení obrázku do zásobníku" )]
		ERR_SaveImageqErr = ( -70005 ),
		[Description( "Chyba nastavení dutiny zásobníku" )]
		ERR_SetCavityErr = ( -70006 ),
		[Description( "Chyba čárového kódu při kontrole zásobníku" )]
		ERR_ChkbarCodeErr = ( -70007 ),
		[Description( "Chyba serveru zásobníku je akce" )]
		ERR_SvrCommIsActionErr = ( -70008 ),
		[Description( "Sériová chyba produktu s příjmem produktu" )]
		ERR_RecvProdSerialErr = ( -70009 ),
		[Description( "Chyba přesunutí zásobníku do pozice NFC" )]
		ERR_ToNPCPosErr = ( -70010 ),
		[Description( "Chyba při provádění zásobníku NFC" )]
		ERR_ExecuteNFCErr = ( -70011 ),
		[Description( "Chyba skenování zásobníku NFC" )]
		ERR_NFCScanningErr = ( -70012 ),
		[Description( "Chyba při přesunu zásobníku do laserové pozice" )]
		ERR_ToLaserPosErr = ( -70013 ),
		[Description( "Chyba laserového značení signálu zásobníku" )]
		ERR_SignalLaserMarkingErr = ( -70014 ),
		[Description( "Chyba zarovnání dna zásobníku" )]
		ERR_TrayBottomALErr = ( -70015 ),
		#endregion
		#region Shopfloor
		[Description( "Čekání na chybu komunikace ShopFloor Comm" )]
		ERR_WaitShopFloorCommErr = ( -80001 ),
		[Description( "Odeslat data ShopFloor Error" )]
		ERR_SendToShopFloorErr = ( -80002 ),

		#endregion


		//-10000001 to -20000000 B262 Vision Processing
		//-20000001 to -30000000 HiPA Common
		//-30000001 to -40000000 HiPA Instrument
		//-40000001 to -50000000 JPT Camera
		//-50000001 to -60000000 JPT Matrox System
		#region -10000001 to -20000000 B262 Vision Processing
		[Description( "Zlatý název souboru PNG je prázdný" )]
		ERR_PNGNameEmptyErr = ( -10000001 ),
		[Description( "Leptaná výška nebo šířka v mm je prázdná" )]
		ERR_EtchedHeightWidthIsEmpty = ( -10000002 ),
		[Description( "Byte Array is null" )]
		ERR_ByteArrIsNull = ( -10000003 ),
		[Description( "Neplatné číslo kavity nebo dolní AL. Výsledek" )]
		ERR_InvalidCavityNoBtmALResult = ( -10000004 ),

		#endregion
		#region -30000001 to -40000000 HiPA Instrument
		//Barcode Scanner Cognex
		[Description( "Zjištění zařízení trvá příliš dlouho" )]
		ERR_DeviceDiscoveryErr = ( -30000001 ),
		[Description( "Cognex Send Command Trigger Error" )]
		ERR_CongexBarSendTriggerErr = ( -30000002 ),

		//Camera - CameraManager
		[Description( "Fotoaparát není inicializován" )]
		ERR_CamNotInitializedErr = ( -30010001 ),
		[Description( "Fotoaparát v nepřetržitém režimu" )]
		ERR_CamInContinuousMode = ( -30010002 ),
		[Description( "Nepřetržitá chyba uchopení" )]
		ERR_ContinuousGrabErr = ( -30010003 ),
		[Description( "Fotoaparát NENÍ v nepřetržitém režimu" )]
		ERR_CamNotInContinuousMode = ( -30010004 ),
		[Description( "Nebyla detekována žádná kamera" )]
		ERR_NoCamDetectedErr = ( -30010005 ),
		//Camera - MatroxProcessing
		[Description( "Vybrán neplatný nebo nesprávný recept produktu" )]
		ERR_InvalidProdRecipe = ( -30011001 ),

		//Height Sensor - Optex
		[Description( "Chyba relace zápisu" )]
		ERR_SetCMDError = ( -30020001 ),
		[Description( "Chyba relace čtení" )]
		ERR_GetCMDError = ( -30020002 ),
		[Description( "chyba kontrolního součtu" )]
		ERR_CheckSumErr = ( -30020003 ),
		[Description( "Selhání načítání dat" )]
		ERR_GetDataErr = ( -30020004 ),
		[Description( "chyba CRC" )]
		ERR_CheckCRCErr = ( -30020005 ),
		[Description( "Selhání aktualizace průměrného počtu" )]
		ERR_UpdateAvgNumErr = ( -30020006 ),
		[Description( "Aktualizace výpadku napájení laseru" )]
		ERR_UpdateLaserPwrErr = ( -30020007 ),
		[Description( "Selhání aktualizací období vzorkování" )]
		ERR_UpdateSamplingPeriodErr = ( -30020008 ),
		[Description( "Selhání aktualizace citlivosti" )]
		ERR_UpdateSenErr = ( -30020009 ),
		[Description( "Překročen počet pokusů" )]
		ERR_RetriesExceeded = ( -30020010 ),

		//LaserController - EzCadCtrl
		[Description( "Nebyla zjištěna žádná karta EzCad" )] //Original => "Could not search any EzCad card"
		ERR_NoEzCadCardErr = ( -30030001 ),
		[Description( "Neplatný počet známek" )]
		ERR_InvalidMarkCountErr = ( -30030002 ),
		[Description( "EzCard je zaneprázdněn" )]
		ERR_EzCardBusy = ( -30030003 ),
		[Description( "Soubor neexistuje" )]
		ERR_FileNotExistErr = ( -30030004 ),
		[Description( "Neplatné ID značky" )]
		ERR_InvalidMarkID = ( -30030005 ),
		[Description( "Neplatný marker nebo ID EzCad" )]
		ERR_InvalidID = ( -30030006 ),
		[Description( "Neplatný počet HatchParam" )]
		ERR_InvalidHatchParamCount = ( -30030007 ),
		[Description( "Entita neexistuje" )]
		ERR_EntityNotExistErr = ( -30030008 ),
		[Description( "Neplatný seznam značek Marker" )]
		ERR_InvalidMarkIDList = ( -30030009 ),

		//LaserController - LaserEzCad
		[Description( "Get PowerLevel Perc Failed" )]
		ERR_GetPwrLvlPercErr = ( -30031001 ),
		[Description( "Get PowerLevel Failed, Please Enter Watt that within Calibrated Range" )]
		ERR_GetPwrLvlErr = ( -30031002 ),
		[Description( "Zahájení značení selhalo" )]
		ERR_MarkingErr = ( -30031003 ),

		//LaserModule - LaserAOC
		[Description( "Žádná odpověď přijata" )]
		ERR_ReadCMDErr = ( -30040001 ),
		[Description( "Sada příkazů selhala" )]
		ERR_SendCMDErr = ( -30040002 ),
		[Description( "Žádná odpověď ze zařízení" )]
		ERR_NoResponseErr = ( -30040003 ),

		//LightSource - CST
		[Description( "Nastavení jasu mimo rozsah" )]
		ERR_BrightnessOutOfRangeErr = ( -30050001 ),
		[Description( "Neplatná návratová hodnota" )]
		ERR_InvalidReturnValErr = ( -30050002 ),

		//Motion - ADLinkIoBoard
		[Description( "Board nebyl nalezen" )]
		ERR_NoBoardDetectErr = ( -30060001 ),

		//Motion - ADLinkMotionBoard
		[Description( "BoardID nebyl nalezen" )]
		ERR_NoBoardIDDetectErr = ( -30061001 ),

		//NFC - Amphenol
		[Description( "Žádná odpověď přijata" )]
		ERR_NoResponseRecvErr = ( -30070001 ),
		[Description( "Nesprávný formát dat" )]
		ERR_InvalidDataFormat = ( -30070002 ),
		[Description( "Nelze najít sériové číslo" )]
		ERR_FindSNErr = ( -30070003 ),
		[Description( "Zařízení NFC nebylo detekováno" )]
		ERR_NoNFCDetectErr = ( -30070004 ),
		[Description( "Data NFC nejsou připravena" )]
		ERR_NFCDataNotReady = ( -30070005 ),

		//OpticalPowerMeter - LaserPowerMeter
		[Description( "Zařízení není připojeno" )]
		ERR_NoDeviceErr = ( -30080001 ),

		//ThermoHygroSensor - ThermoHygro
		[Description( "Žádný výsledek nebyl přijat" )]
		ERR_NoResultErr = ( -30090001 ),

		#endregion
		#region -60000001 to -70000000 Neowise Modules 
		// Alarm Page
		[Description( "Nejprve prosím zavřete kryt vstupu dopravníku." )]
		ERR_CloseConveyorInletCover = ( -60000001 ),
		[Description( "Nejprve prosím zavřete kryt výstupu dopravníku." )]
		ERR_CloseConveyorOutletCover = ( -60000002 ),

		//MainWindow
		[Description( "Objekt receptu null" )]
		ERR_RecipeObjNull = ( -60100001 ),
		[Description( "Import receptu produktu se nezdařil, importovaný recept již existuje" )]
		ERR_RecipeImportFail = ( -60100002 ),

		//ConveyorModuleBase

		[Description( "Neplatná pozice" )]
		ERR_InvalidAxisPos = ( -60200001 ),
		[Description( "Deska detekována v InletDoor" )]
		ERR_BoardOnInletDr = ( -60200002 ),
		[Description( "Deska detekována v OutletDoor" )]
		ERR_BoardOnOutletDr = ( -60200003 ),

		//LaserHeadModule
		[Description( "Inicializace se nezdařila" )]
		ERR_InitFailed = ( -60300001 ),
		[Description( "Neočekávaný výškový rozdíl. Zkontrolujte, zda VýškaSensor ukazuje na správnou pozici." )]
		ERR_HeightDiff = ( -60300002 ),
		[Description( "Překročení počtu kompenzací. Zkontrolujte toleranci přijetí." )]
		ERR_ExceedNoOfCompensationRetries = ( -60300003 ),
		[Description( "Neplatný vstupní rozsah" )]
		ERR_InvalidRange = ( -60300004 ),
		[Description( "Get PowerLevel Perc Failed" )]
		ERR_GetPwrPercFail = ( -60300005 ),
		[Description( "Get PowerLevel Failed, Please Enter Watt that within Calibrated Range" )]
		ERR_GetPwrPercFailEnterWatt = ( -60300006 ),
		[Description( "Kontrolní soubor neexistuje" )]
		ERR_InspectFileNotExist = ( -60300007 ),
		[Description( "Soubor modelu neexistuje" )]
		ERR_ModelFileNotExist = ( -60300008 ),

		//StationModuleBase
		[Description( "Zdrojová cesta není definována" )]
		ERR_PathIsUndefined = ( -60400001 ),
		[Description( "Více než 1 obrázek ve složce" )]
		ERR_ExceedOneImage = ( -60400002 ),
		[Description( "Chyba GetImageFileName." )]
		ERR_GetImgFileNameErr = ( -60400003 ),
		[Description( "Neplatný formát názvu souboru pro noEtch" )]
		ERR_InvalidnoEtchImgFileFormat = ( -60400004 ),
		[Description( "Neplatný formát názvu souboru" )]
		ERR_InvalidFilenameFormat = ( -60400005 ),

		//IoModules
		[Description( "IO Board not Initialized" )]
		ERR_IONotInit = ( -60500001 ),
		[Description( "Osa není v bezpečném rozsahu" )]
		ERR_AxisNotSafe = ( -60500002 ),

		//TrayModuleBase
		[Description( "Zásobník není zavřený" )]
		ERR_TrayNotClose = ( -60600001 ),
		[Description( "NFC Z není v poloze reset" )]
		ERR_NFCZNotInResetPos = ( -60600002 ),
		[Description( "NFC X není v pracovní pozici" )]
		ERR_NFCXNotInWorkPos = ( -60600003 ),
		[Description( "Zátka není v poloze pro reset" )]
		ERR_TrayStopperNotInResetPos = ( -60600004 ),
		[Description( "Soubor vstupního obrázku neexistuje" )]
		ERR_ImgFileNotExist = ( -60600005 ),

		//Ctrl_Ezcad
		[Description( "Chyba načítání souboru" )]
		ERR_LoadFileFail = ( -60700001 ),
		[Description( "Použít chybu nastavení pera" )]
		ERR_ApplyPenErr = ( -60700002 ),
		[Description( "Použít chybu nastavení BMP" )]
		ERR_ApplyBMPErr = ( -60700003 ),
		[Description( "Chyba nastavení pera a BMP" )]
		ERR_ApplyPenBMPErr = ( -60700004 ),
		[Description( "chyba MoveRotate" )]
		ERR_MovRotateErr = ( -60700005 ),

		//Ctrl_WorkPos
		[Description( "Dosáhli jste maximálního povoleného počtu pracovních pozic" )]
		ERR_ExceedNumOfWorkPos = ( -60800001 ),
		[Description( "Nalezena stávající naučená pozice" )]
		ERR_TeachPosFound = ( -60800002 ),

		//Recipe
		[Description( "Obsah souboru je nesprávný" )]
		ERR_IncorrectFileContent = ( -60900001 ),

		//MachineStateMng
		[Description( "Timeout: Fail to acknowledge to Pop Up Window (Laser Module Disconnected)" )]
		ERR_AcknowledgeTimeoutEzcad = ( -61000001 ),
		[Description( "Timeout: Fail to acknowledge to Pop Up Window (Manual Marking)" )]
		ERR_AcknowledgeTimeoutMan = ( -61000002 ),

		#endregion

		[Description( "Nalezen nesprávný seznam jazyků, načítání zálohy vybraného jazyka" )]
		ERR_LoadLangErr = ( -99999998 ),
		[Description( "Neznámá chyba výjimky" )]
		ERR_UnknownExceptionErr = ( -99999999 ),
	}
	#endregion

	#region CHS Errors
	public enum RunErrorsCHS
	{
		//0- no error
		//-1 to -100 for timeouts
		//-101 to -200 for thread inconformity error
		//-201 to -300 for input
		//-301 to -400 for output
		//-401 to -500 for cylinder
		//-501 to -600 for adlinkmotion
		//-601 to -700 for vision
		//-701 to -800 for servercom
		//-801 to -900 for conveyor
		//-901 to -1000 for heightsensor
		//-1001 to -1100 for laser marking
		//-1101 to -1200 for barcode
		//-1201 to -1300 for recipelink
		//-1301 to -1400 for image handling
		//-4001 to -5000 for checking of other thread error by Conveyorout.
		//-9001 to -10000 for autorunseq exception
		//-10001 to -20000 for laserhead exception
		//-20001 to -30000 for ConveyorIn exception
		//-30001 to -40000 for ConveyorOut exception
		//-40001 to -50000 for Smemainlet exception
		//-50001 to -60000 for Smemaoutlet exception
		//-60001 to -70000 for Common exception
		//-70001 to -80000 for Tray exception
		//-80001 to -90000 for Shopfloor exception
		//-10000001 to -20000000 B262 Vision Processing
		//-20000001 to -30000000 HiPA Common
		//-30000001 to -40000000 HiPA Instrument
		//-40000001 to -50000000 JPT Camera
		//-50000001 to -60000000 JPT Matrox System
		//-60000001 to -70000000 Neowise
		//....and other exception

		[Description( "没有错误" )]
		ERR_NoError = ( 0 ),      //No Error	

		#region timeouts
		[Description( "动作超时" )]
		ERR_MoveTimeout = ( -1 ),   // 

		[Description( "IO 超时" )]
		ERR_IOTimeout = ( -2 ),   // 

		[Description( "函数超时" )]
		ERR_FunctionTimeout = ( -3 ),   // 
		#endregion
		#region Threadstatus inconformity
		[Description( "线程状态不一致" )]
		ERR_Inconformity = ( -101 ),
		[Description( "意外异常错误" )]
		ERR_UnexpectedException = ( -102 ),
		//[Description(“车间通信线程状态不一致”)]
		//ERR_Inconformity_ShopFloorSeq = ( -101 ), //

		//[Description("激光头seq线程状态不一致")]
		//ERR_Inconformity = ( -102 ),

		//[Description( "Conveyor Out thread status inconformity" )]
		//ERR_Inconformity_ConveyorOutSeq = ( -103 ), //

		//[Description("传送带线程状态不一致")]
		//ERR_Inconformity_ConveyorInSeq = ( -104 ), //

		//[Description( "Smema 线程状态不一致" )]
		//ERR_Inconformity_SmemaSeq = ( -105 ), //

		//[Description（“托盘线程状态不一致”）]
		//ERR_Inconformity_TraySeq = ( -106 ),
		#endregion
		#region Input failure
		[Description( "读取广告链接错误" )]
		ERR_ReadInputIOErr = ( -201 ),   // 
		#endregion
		#region Output failure
		[Description( "设置广告链接错误" )]
		ERR_SetOutputIOErr = ( -301 ),   // 
		#endregion
		#region IO Motion
		[Description( "气缸运动错误" )]
		ERR_CylinderMotionErr = ( -401 ),   // 
		[Description( "气缸门运动错误" )]
		ERR_CylinderDrMotionErr = ( -402 ),   // 
		[Description( "气缸产品压脚运动错误" )]
		ERR_CylinderProdPressMotionErr = ( -403 ),   // 


		//[Description( "Reset All Motion IO Failed" )]
		//ERR_ResetAllFail = ( -1001 ),   // 
		//[Description( "Inlet Stopper Release Position Failed" )]
		//ERR_InletStopperResetFail = ( -1002 ),   // 
		//[Description( "Inlet Stopper Block Position Failed" )]
		//ERR_InletStopperMoveToPosFail = ( -1003 ),   // 
		//[Description( "Outlet Stopper Release Failed" )]
		//ERR_OutletStopperResetFail = ( -1004 ),   // 
		//[Description( "Outlet Stopper Block Position Failed" )]
		//ERR_OutletStopperMoveToPosFail = ( -1005 ),   // 
		//[Description( "Product Presser Retract Failed" )]
		//ERR_ProductPresserResetFail = ( -1006 ),   // 
		//[Description( "Product Presser Move to Work Position Failed" )]
		//ERR_ProductPresserMoveToPosFail = ( -1007 ),   // 
		//[Description( "Shutter door close error" )]
		//ERR_ShutterDoorClose = ( -1008 ),
		//[Description( "Shutter door open error" )]
		//ERR_ShutterDoorOpen = ( -1009 ),
		#endregion
		#region adlink motion
		[Description( "轴移动错误" )]
		ERR_AxisMoveErr = ( -501 ),   // 
									  //[Description( "Conveyor Outlet Start Slow Failed" )]
									  //ERR_ConveyorOutStartSlowFail = ( -3001 ),   // 
									  //[Description( "Conveyor Outlet Start Fast Failed" )]
									  //ERR_ConveyorOutStartFastFail = ( -3002 ),   // 
									  //[Description( "Conveyor Outlet Stop Failed" )]
									  //ERR_ConveyorOutStopFail = ( -3003 ),   // 
									  //[Description( "LiftZ Move To Standby Pos Failed" )]
									  //ERR_LiftZMoveToStandbyPosFailed = ( -3004 ),   // 
									  //[Description( "LiftZ Move To Work Pos Failed" )]
									  //ERR_LiftZMoveToWorkPosFailed = ( -3005 ),   // 
									  //[Description( "Laser Head Move XYZ Failed" )]
									  //ERR_LHMoveXYZFailed = ( -3006 ),   // 
									  //[Description( "Laser Head Move X Failed" )]
									  //ERR_LHMoveXFailed = ( -3007 ),   // 
									  //[Description( "Laser Head Move Y Failed" )]
									  //ERR_LHMoveYFailed = ( -3008 ),   // 
									  //[Description( "Laser Head Move Z Failed" )]
									  //ERR_LHMoveZFailed = ( -3009 ),   // 
		#endregion
		#region Vision
		[Description( "相机抓取所有错误" )]
		ERR_CameraGrabAllError = ( -601 ),   // 
		[Description( "方向检查错误" )]
		ERR_VisionOrientationCheckErr = ( -602 ),
		[Description( "方向：未找到" )]
		ERR_VisionOrientationNotFoundErr = ( -603 ),
		[Description( "对齐检查错误" )]
		ERR_VisionAlignmentCheckErr = ( -604 ),
		[Description( "错误的产品宽度" )]
		ERR_ProductWidthErr = ( -605 ),
		[Description( "内容检查错误" )]
		ERR_ContentChkErr = ( -606 ),
		[Description( "超出 Y 旋转限制" )]
		ERR_ExceedRotYLimitErr = ( -607 ),
		[Description( "产品角度不正确" )]
		ERR_ProductAngleErr = ( -608 ),
		#endregion
		#region ServerComm
		//ShopFloor Communication Seq.
		[Description( "从车间检索图像时发生错误" )]
		ERR_ShopFloor_Retrieve_Img = ( -701 ),   // 
		[Description( "检索型腔号时发生错误" )]
		ERR_ShopFloor_Retrieve_CavityNo = ( -702 ),   // 
		[Description( "将结果发布到车间时发生错误" )]
		ERR_ShopFloor_Post_Result = ( -703 ),   // 
		[Description( "无模腔数据可雕刻错误" )]
		ERR_NoCavityDataToEngrave = ( -704 ),//only occur during simulation mode
		#endregion
		#region Conveyor
		[Description( "板卡在入口处" )]
		ERR_Board_Jammed_Inlet = ( -801 ),   // 
		[Description( "板卡在插座" )]
		ERR_Board_Jammed_Outlet = ( -802 ),   // 
		[Description( "传送带非空" )]
		ERR_ConveyorNotEmpty = ( -803 ),   // 
		#endregion
		#region HeightSensor
		[Description( "高度传感器错误" )]
		ERR_HeightSensorError = ( -901 ),   // 
		[Description( "超出赔偿限额" )]
		ERR_ExceededCompensationLimitError = ( -902 ),   // 
		#endregion
		#region Laser
		[Description( "校准激光功率验证错误" )]
		ERR_CalibLaserPwrValidErr = ( -1001 ),
		[Description( "标记错误" )]
		ERR_LaserMarkErr = ( -1002 ),
		#endregion
		#region Barcode
		[Description( "条形码阅读器错误" )]
		ERR_BarCodeReaderErr = ( -1101 ),   // 
		#endregion
		#region Recipesetting
		[Description( "在 Cavity/WorkPos Idx 中未找到 Cavity 编号。链接" )]
		ERR_CavityNoNotFound = ( -1201 ),
		[Description( "在颜色/产品类型链接中找不到配方" )]
		ERR_RecipeNotFound = ( -1202 ),
		#endregion
		#region Imagehandling
		[Description( "图像文件发布操作错误" )]
		ERR_ImageFilePostActionErr = ( -1301 ),
		#endregion




		[Description( "传送带入口包含错误" )]
		ERR_InletContainsErr = ( -4001 ),//扩展名，不用提
		[Description( "SMEMA 插座包含错误" )]
		ERR_SMEMAOutContainsErr = ( -4002 ),//扩展名，不用提
		[Description( "SMEMA 入口包含错误" )]
		ERR_SMEMAInContainsErr = ( -4003 ),//扩展名，不用提
		[Description( "ShopFloor 包含错误" )]
		ERR_ShopFloorContainsErr = ( -4004 ),//扩展名，不用说了
		#region autoseq
		// Auto Seq
		[Description( "等待开始按钮IO错误" )]
		ERR_StartBtnIOErr = ( -9001 ),
		[Description( "设置门锁IO错误" )]
		ERR_SetDoorLockErr = ( -9002 ),
		[Description( "检查门锁IO错误" )]
		ERR_CheckDoorLockErr = ( -9003 ),
		[Description( "将所有站设置为就绪错误" )]
		ERR_SetAllStationReadyErr = ( -9004 ),
		#endregion
		#region laserheadseqexception
		//Laser Head Seq.
		[Description( "等待工作错误" )]
		ERR_WaitForWorkErr = ( -10001 ),
		[Description( "在位置等待电梯。错误" )]
		ERR_WaitLiftInPosErr = ( -10002 ),
		[Description( "检查高度时出错" )]
		ERR_ScanHeight = ( -10003 ),
		[Description( "在激光头序列中进行视觉检查时发生错误" )]
		ERR_VisionCheckError = ( -10004 ),
		[Description( "在激光头序列中等待门关闭时发生错误" )]
		ERR_WaitDoorClosedError = ( -10005 ),
		[Description( "做标记错误" )]
		ERR_DoMarkingErr = ( -10006 ),
		[Description( "等待抄送完成错误" )]
		ERR_WaitForCCDoneErr = ( -10007 ),
		[Description( "做CC错误" )]
		ERR_DoCCErr = ( -10008 ),
		[Description( "继续下一个腔" )]
		ERR_ContinueNextCavityErr = ( -10009 ),
		[Description( "等待所有托盘" )]
		ERR_WaitForALLPalletsErr = ( -10010 ),
		[Description( "卷帘门 IO 错误；请检查卷帘门 IO" )]
		ERR_ShutterDoorIOError = ( -10011 ),
		#endregion
		#region ConveyorInSeqexception
		//ConveyorInlet Seq
		[Description( "入口 SMEMA 工作错误" )]
		ERR_InletSMEMAWorkErr = ( -20002 ),
		[Description( "入口检查运行传送带标志错误" )]
		ERR_InletCheckRunConveyorFlagErr = ( -20003 ),
		[Description( "等待托盘传感器2错误" )]
		ERR_WaitPalletSen2Err = ( -20004 ),
		[Description( "条码检查错误" )]
		ERR_BarCodeCheckErr = ( -20005 ),
		[Description( "移动到托盘传感器 1 错误" )]
		ERR_MovePalletSen1Err = ( -20006 ),
		[Description( "检查托盘传感器 1 错误" )]
		ERR_CheckPalletSen1Err = ( -20007 ),
		[Description( "等待入口 SMEMA 就绪错误" )]
		ERR_WaitForSMEMAReadyErr = ( -20008 ),
		[Description( "入口检查托盘所有传感器错误" )]
		ERR_InletCheckPalletAllSenErr = ( -20009 ),
		#endregion
		#region ConveyorOutSeqexception
		[Description( "插座检查状态错误" )]
		ERR_OutletCheckStateErr = ( -30002 ),
		[Description( "传送带运行错误" )]
		ERR_ConveyorInActionErr = ( -30003 ),
		[Description( "等待托盘1错误" )]
		ERR_WaitForPallet1Err = ( -30004 ),
		[Description( "检索条码错误" )]
		ERR_RetrieveBarCodeErr = ( -30005 ),
		[Description( "启动服务器通信错误" )]
		ERR_StartServerCommErr = ( -30006 ),
		[Description( "等待产品序列错误" )]
		ERR_WaitProdSNErr = ( -30007 ),
		[Description( "提升 Z 移动到工作位置。错误" )]
		ERR_ZWorkPosErr = ( -30008 ),
		[Description( "等待标记完成错误" )]
		ERR_WaitMarkCompleteErr = ( -30009 ),
		[Description( "ShopFloor 发送结果错误" )]
		ERR_ShopFloorSendResultErr = ( -30010 ),
		[Description( "提升Z移动到待机位置。错误" )]
		ERR_ZStandbyPosErr = ( -30011 ),
		[Description( "出口检查托盘所有传感器错误" )]
		ERR_OutletCheckPalletAllSenErr = ( -30012 ),
		[Description( "出口开始 Smema 行动错误" )]
		ERR_OutletSMEMAIsActionErr = ( -30013 ),
		[Description( "释放所有 3 个托盘错误" )]
		ERR_Release3PalletsErr = ( -30014 ),
		[Description( "按 1 个托盘错误释放 1 个" )]
		ERR_Release1By1PalletErr = ( -30015 ),
		[Description( "释放 1 个托盘错误" )]
		ERR_ReleasePallet1Err = ( -30016 ),
		[Description( "等待板移动到出口传感器错误" )]
		ERR_BoardToOutletSenErr = ( -30017 ),
		[Description( "开始释放错误" )]
		ERR_StartReleasingErr = ( -30018 ),
		[Description( "移动托盘 2 到 1 错误" )]
		ERR_ShiftPallet2To1Err = ( -30019 ),
		[Description( "等待托盘 3 传感器关闭错误" )]
		ERR_WaitPallet3SenseOffErr = ( -30020 ),
		[Description( "等待 Smema 输出标志错误" )]
		ERR_WaitSmemaFlagErr = ( -30021 ),


		#endregion
		#region Smemainletexception
		//Smema Inlet Seq.
		[Description( "Smema 等待接受错误" )]
		ERR_WaitingToAcceptErr = ( -40001 ),
		[Description( "接受错误" )]
		ERR_AcceptErr = ( -40002 ),
		#endregion
		#region Smemaoutletexception
		//Smema Outlet Seq.
		[Description( "等待释放错误" )]
		ERR_WaitingToReleaseErr = ( -50001 ),
		[Description( "Smema Outlet 释放错误" )]
		ERR_SmemaReleasingErr = ( -50002 ),
		#endregion
		#region Commonexception
		//Smema Outlet Seq.
		[Description( "初始化错误" )]
		ERR_InitErr = ( -60001 ),
		[Description( "后初始化错误" )]
		ERR_PostInitErr = ( -60002 ),
		[Description( "IsAction 错误" )]
		ERR_IsActionErr = ( -60003 ),
		[Description( "完成错误" )]
		ERR_FinishErr = ( -60004 ),
		#endregion
		#region TraySeqException
		//Tray Seq.
		[Description( "托盘接近传感器输出错误" )]
		ERR_ProxSenOutErr = ( -70001 ),
		[Description( "托盘接近传感器出错" )]
		ERR_ProxSenInErr = ( -70002 ),
		[Description( "托盘激活止动器错误" )]
		ERR_ActStopperErr = ( -70003 ),
		[Description( "托盘检查文件名错误" )]
		ERR_ChkFileNameErr = ( -70004 ),
		[Description( "托盘保存图像错误" )]
		ERR_SaveImageqErr = ( -70005 ),
		[Description( "托盘设置腔设置错误" )]
		ERR_SetCavityErr = ( -70006 ),
		[Description( "托盘检查条码错误" )]
		ERR_ChkbarCodeErr = ( -70007 ),
		[Description( "托盘服务器通讯是操作错误" )]
		ERR_SvrCommIsActionErr = ( -70008 ),
		[Description( "托盘接收产品序列错误" )]
		ERR_RecvProdSerialErr = ( -70009 ),
		[Description( "托盘移动到 NFC 位置。错误" )]
		ERR_ToNPCPosErr = ( -70010 ),
		[Description( "托盘执行NFC错误" )]
		ERR_ExecuteNFCErr = ( -70011 ),
		[Description( "托盘NFC扫描错误" )]
		ERR_NFCScanningErr = ( -70012 ),
		[Description( "托盘移动到激光位置。错误" )]
		ERR_ToLaserPosErr = ( -70013 ),
		[Description( "托盘信号激光打标错误" )]
		ERR_SignalLaserMarkingErr = ( -70014 ),
		[Description( "托盘底部对齐错误" )]
		ERR_TrayBottomALErr = ( -70015 ),
		#endregion
		#region Shopfloor
		[Description( "等待车间通信错误" )]
		ERR_WaitShopFloorCommErr = ( -80001 ),
		[Description( "发送数据 ShopFloor 错误" )]
		ERR_SendToShopFloorErr = ( -80002 ),

		#endregion



		//-10000001 to -20000000 B262 Vision Processing
		//-20000001 to -30000000 HiPA Common
		//-30000001 to -40000000 HiPA Instrument
		//-40000001 to -50000000 JPT Camera
		//-50000001 to -60000000 JPT Matrox System
		#region -10000001 to -20000000 B262 Vision Processing
		[Description( "金色PNG文件名为空" )]
		ERR_PNGNameEmptyErr = ( -10000001 ),
		[Description( "蚀刻的高度或宽度以毫米为单位为空" )]
		ERR_EtchedHeightWidthIsEmpty = ( -10000002 ),
		[Description( "字节数组为空" )]
		ERR_ByteArrIsNull = ( -10000003 ),
		[Description( "无效的型腔号或底部 AL。结果" )]
		ERR_InvalidCavityNoBtmALResult = ( -10000004 ),


		#endregion
		#region -30000001 to -40000000 HiPA Instrument
		//Barcode Scanner Cognex
		[Description( "设备发现时间太长" )]
		ERR_DeviceDiscoveryErr = ( -30000001 ),
		[Description( "Cognex Send Command Trigger Error" )]
		ERR_CongexBarSendTriggerErr = ( -30000002 ),

		//Camera - CameraManager
		[Description( "相机未初始化" )]
		ERR_CamNotInitializedErr = ( -30010001 ),
		[Description( "相机​​处于连续模式" )]
		ERR_CamInContinuousMode = ( -30010002 ),
		[Description( "连续抓取错误" )]
		ERR_ContinuousGrabErr = ( -30010003 ),
		[Description( "相机​​不在连续模式" )]
		ERR_CamNotInContinuousMode = ( -30010004 ),
		[Description( "未检测到摄像头" )]
		ERR_NoCamDetectedErr = ( -30010005 ),
		//Camera - MatroxProcessing
		[Description( "选择的产品配方无效或不正确" )]
		ERR_InvalidProdRecipe = ( -30011001 ),

		//Height Sensor - Optex
		[Description( "写入会话错误" )]
		ERR_SetCMDError = ( -30020001 ),
		[Description( "读取会话错误" )]
		ERR_GetCMDError = ( -30020002 ),
		[Description( "校验和错误" )]
		ERR_CheckSumErr = ( -30020003 ),
		[Description( "获取数据失败" )]
		ERR_GetDataErr = ( -30020004 ),
		[Description( "CRC 错误" )]
		ERR_CheckCRCErr = ( -30020005 ),
		[Description( "更新平均次数失败" )]
		ERR_UpdateAvgNumErr = ( -30020006 ),
		[Description( "更新激光电源故障" )]
		ERR_UpdateLaserPwrErr = ( -30020007 ),
		[Description( "更新采样周期失败" )]
		ERR_UpdateSamplingPeriodErr = ( -30020008 ),
		[Description( "更新灵敏度失败" )]
		ERR_UpdateSenErr = ( -30020009 ),
		[Description( "超过重试次数" )]
		ERR_RetriesExceeded = ( -30020010 ),

		//LaserController - EzCadCtrl
		[Description( "No EzCad Card detected" )] //Original => "Could not search any EzCad card"
		ERR_NoEzCadCardErr = ( -30030001 ),
		[Description( "无效标记计数" )]
		ERR_InvalidMarkCountErr = ( -30030002 ),
		[Description( "EzCad 忙" )]
		ERR_EzCardBusy = ( -30030003 ),
		[Description( "文件不存在" )]
		ERR_FileNotExistErr = ( -30030004 ),
		[Description( "无效的标记 ID" )]
		ERR_InvalidMarkID = ( -30030005 ),
		[Description( "无效的标记或 EzCad ID" )]
		ERR_InvalidID = ( -30030006 ),
		[Description( "无效的 HatchParam 计数" )]
		ERR_InvalidHatchParamCount = ( -30030007 ),
		[Description( "实体不存在" )]
		ERR_EntityNotExistErr = ( -30030008 ),
		[Description( "无效的 MarkerIDList" )]
		ERR_InvalidMarkIDList = ( -30030009 ),

		//LaserController - LaserEzCad
		[Description( "获取功率级别百分比失败" )]
		ERR_GetPwrLvlPercErr = ( -30031001 ),
		[Description( "获取功率电平失败，请输入校准范围内的瓦特" )]
		ERR_GetPwrLvlErr = ( -30031002 ),
		[Description( "开始标记失败" )]
		ERR_MarkingErr = ( -30031003 ),

		//LaserModule - LaserAOC
		[Description( "没有收到回复" )]
		ERR_ReadCMDErr = ( -30040001 ),
		[Description( "命令集失败" )]
		ERR_SendCMDErr = ( -30040002 ),
		[Description( "设备无回复" )]
		ERR_NoResponseErr = ( -30040003 ),

		//LightSource - CST
		[Description( "亮度设置超出范围" )]
		ERR_BrightnessOutOfRangeErr = ( -30050001 ),
		[Description( "无效的返回值" )]
		ERR_InvalidReturnValErr = ( -30050002 ),

		//Motion - ADLinkIoBoard
		[Description( "未找到董事会" )]
		ERR_NoBoardDetectErr = ( -30060001 ),

		//Motion - ADLinkMotionBoard
		[Description( "未找到BoardID" )]
		ERR_NoBoardIDDetectErr = ( -30061001 ),

		//NFC - Amphenol
		[Description( "没有收到回复" )]
		ERR_NoResponseRecvErr = ( -30070001 ),
		[Description( "数据格式不正确" )]
		ERR_InvalidDataFormat = ( -30070002 ),
		[Description( "无法找到序列号" )]
		ERR_FindSNErr = ( -30070003 ),
		[Description( "未检测到 NFC 设备" )]
		ERR_NoNFCDetectErr = ( -30070004 ),
		[Description( "NFC 数据未准备好" )]
		ERR_NFCDataNotReady = ( -30070005 ),

		//OpticalPowerMeter - LaserPowerMeter
		[Description( "设备未连接" )]
		ERR_NoDeviceErr = ( -30080001 ),

		//ThermoHygroSensor - ThermoHygro
		[Description( "没有收到结果" )]
		ERR_NoResultErr = ( -30090001 ),

		#endregion
		#region -60000001 to -70000000 Neowise Modules 
		// Alarm Page
		[Description( "请先关闭传送带入口盖。" )]
		ERR_CloseConveyorInletCover = ( -60000001 ),
		[Description( "请先关闭输送机出口盖。" )]
		ERR_CloseConveyorOutletCover = ( -60000002 ),

		//MainWindow
		[Description( "配方对象为空" )]
		ERR_RecipeObjNull = ( -60100001 ),
		[Description( "产品配方导入失败，导入的配方已存在" )]
		ERR_RecipeImportFail = ( -60100002 ),

		//ConveyorModuleBase

		[Description( "无效位置" )]
		ERR_InvalidAxisPos = ( -60200001 ),
		[Description( "在 InletDoor 中检测到板" )]
		ERR_BoardOnInletDr = ( -60200002 ),
		[Description( "在 OutletDoor 中检测到板子" )]
		ERR_BoardOnOutletDr = ( -60200003 ),

		//LaserHeadModule
		[Description( "初始化失败" )]
		ERR_InitFailed = ( -60300001 ),
		[Description( "意外的高度差。请检查高度传感器是否指向正确的位置。" )]
		ERR_HeightDiff = ( -60300002 ),
		[Description( "超出补偿数量。请检查验收容差。" )]
		ERR_ExceedNoOfCompensationRetries = ( -60300003 ),
		[Description( "无效的输入范围" )]
		ERR_InvalidRange = ( -60300004 ),
		[Description( "获取 PowerLevel Perc 失败" )]
		ERR_GetPwrPercFail = ( -60300005 ),
		[Description( "获取功率电平失败，请输入校准范围内的瓦特" )]
		ERR_GetPwrPercFailEnterWatt = ( -60300006 ),
		[Description( "检查文件不存在" )]
		ERR_InspectFileNotExist = ( -60300007 ),
		[Description( "模型文件不存在" )]
		ERR_ModelFileNotExist = ( -60300008 ),

		//StationModuleBase
		[Description( "源路径未定义" )]
		ERR_PathIsUndefined = ( -60400001 ),
		[Description( "文件夹中超过 1 个图像" )]
		ERR_ExceedOneImage = ( -60400002 ),
		[Description( "GetImageFileName 错误。" )]
		ERR_GetImgFileNameErr = ( -60400003 ),
		[Description( "noEtch 的文件名格式无效" )]
		ERR_InvalidnoEtchImgFileFormat = ( -60400004 ),
		[Description( "无效的文件名格式" )]
		ERR_InvalidFilenameFormat = ( -60400005 ),

		//IoModules
		[Description( "IO 板未初始化" )]
		ERR_IONotInit = ( -60500001 ),
		[Description( "轴不在安全范围内" )]
		ERR_AxisNotSafe = ( -60500002 ),

		//TrayModuleBase
		[Description( "托盘未关闭" )]
		ERR_TrayNotClose = ( -60600001 ),
		[Description( "NFC Z 不在复位位置" )]
		ERR_NFCZNotInResetPos = ( -60600002 ),
		[Description( "NFC X 不在工作位置" )]
		ERR_NFCXNotInWorkPos = ( -60600003 ),
		[Description( "塞子不在复位位置" )]
		ERR_TrayStopperNotInResetPos = ( -60600004 ),
		[Description( "输入图像文件不存在" )]
		ERR_ImgFileNotExist = ( -60600005 ),

		//Ctrl_Ezcad
		[Description( "加载文件失败" )]
		ERR_LoadFileFail = ( -60700001 ),
		[Description( "应用笔设置错误" )]
		ERR_ApplyPenErr = ( -60700002 ),
		[Description( "应用 BMP 设置错误" )]
		ERR_ApplyBMPErr = ( -60700003 ),
		[Description( "应用笔和 BMP 设置错误" )]
		ERR_ApplyPenBMPErr = ( -60700004 ),
		[Description( "移动旋转错误" )]
		ERR_MovRotateErr = ( -60700005 ),

		//Ctrl_WorkPos
		[Description( "达到了允许的工作位置数量限制" )]
		ERR_ExceedNumOfWorkPos = ( -60800001 ),
		[Description( "找到现有的教学位置" )]
		ERR_TeachPosFound = ( -60800002 ),

		//Recipe
		[Description( "文件内容不正确" )]
		ERR_IncorrectFileContent = ( -60900001 ),

		//MachineStateMng
		[Description( "Timeout: Fail to acknowledge to Pop Up Window (Laser Module Disconnected)" )]
		ERR_AcknowledgeTimeoutEzcad = ( -61000001 ),
		[Description( "Timeout: Fail to acknowledge to Pop Up Window (Manual Marking)" )]
		ERR_AcknowledgeTimeoutMan = ( -61000002 ),
		#endregion

		[Description( "发现不正确的语言列表，正在加载备份选择的语言" )]
		ERR_LoadLangErr = ( -99999998 ),
		[Description( "未知异常错误" )]
		ERR_UnknownExceptionErr = ( -99999999 ),
	}
	#endregion
}
