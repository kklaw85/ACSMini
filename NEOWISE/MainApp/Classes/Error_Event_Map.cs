namespace N_Error_Event
{


	public class ErrorMsgMap
	{

		private string _NO_ERROR = "";
		public string NO_ERROR { get { return this._NO_ERROR; } set { this._NO_ERROR = value; } }


		//ITMS related Error
		private string _ITMS_BATCH_UPLOAD_FAILED = "ITMS Batch Data Upload Failed";
		public string ITMS_BATCH_UPLOAD_FAILED { get { return this._ITMS_BATCH_UPLOAD_FAILED; } set { this._ITMS_BATCH_UPLOAD_FAILED = value; } }

		private string _ITMS_MACHINE_STATUS_UPLOAD_FAILED = "ITMS Machine Status Upload Failed";
		public string ITMS_MACHINE_STATUS_UPLOAD_FAILED { get { return this._ITMS_MACHINE_STATUS_UPLOAD_FAILED; } set { this._ITMS_MACHINE_STATUS_UPLOAD_FAILED = value; } }

		private string _ITMS_PING_UPLOAD_FAILED = "ITMS Ping Data Upload Failed";
		public string ITMS_PING_UPLOAD_FAILED { get { return this._ITMS_PING_UPLOAD_FAILED; } set { this._ITMS_PING_UPLOAD_FAILED = value; } }

		private string _ITMS_SUB_UPLOAD_FAILED = "ITMS Substrate Data Upload Failed";
		public string ITMS_SUB_UPLOAD_FAILED { get { return this._ITMS_SUB_UPLOAD_FAILED; } set { this._ITMS_SUB_UPLOAD_FAILED = value; } }

		private string _S_RESULT_SAVE_ERR = "Saving Trimmed Result Error";
		public string S_RESULT_SAVE_ERR { get { return this._S_RESULT_SAVE_ERR; } set { this._S_RESULT_SAVE_ERR = value; } }

		private string _TE_SCORE_COM_ERR = "S_Core Communication Lost";
		public string TE_SCORE_COM_ERR { get { return this._TE_SCORE_COM_ERR; } set { this._TE_SCORE_COM_ERR = value; } }

		private string _TE_SCORE_INIT_FAILED = "S_Core Initialization Failed";
		public string TE_SCORE_INIT_FAILED { get { return this._TE_SCORE_INIT_FAILED; } set { this._TE_SCORE_INIT_FAILED = value; } }

		private string _TE_RV_TO_RS_Error = "Converting Resistance Value to Formatting String Error";
		public string TE_RV_TO_RS_Error { get { return this._TE_RV_TO_RS_Error; } set { this._TE_RV_TO_RS_Error = value; } }

		private string _TE_RS_TO_RV_Error = "Converting Resistance Formatting String to Resistance Value Error";
		public string TE_RS_TO_RV_Error { get { return this._TE_RS_TO_RV_Error; } set { this._TE_RS_TO_RV_Error = value; } }

		private string _TE_UPDATE_TOTAL_RES_ERR = "Total Resistor Update Error";
		public string TE_UPDATE_TOTAL_RES_ERR { get { return this._TE_UPDATE_TOTAL_RES_ERR; } set { this._TE_UPDATE_TOTAL_RES_ERR = value; } }

		private string _TE_UPDATE_CUT_PARA_ERR = "Cut Data Update Error";
		public string TE_UPDATE_CUT_PARA_ERR { get { return this._TE_UPDATE_CUT_PARA_ERR; } set { this._TE_UPDATE_CUT_PARA_ERR = value; } }

		private string _TE_RES_DATA_DNLD = "Trimming Resistor Info Download Error";
		public string TE_RES_DATA_DNLD { get { return this._TE_RES_DATA_DNLD; } set { this._TE_RES_DATA_DNLD = value; } }

		private string _TE_PROBE_MAP_LOAD_FAILED = "Probe-Map Loading Failed";
		public string TE_PROBE_MAP_LOAD_FAILED { get { return this._TE_PROBE_MAP_LOAD_FAILED; } set { this._TE_PROBE_MAP_LOAD_FAILED = value; } }

		private string _TE_GO_TO_RES_FAILED = "Go to Resistor Failed";
		public string TE_GO_TO_RES_FAILED { get { return this._TE_GO_TO_RES_FAILED; } set { this._TE_GO_TO_RES_FAILED = value; } }

		private string _TE_TRIM_ALL_FAILED = "Tirm Column Failed";
		public string TE_TRIM_ALL_FAILED { get { return this._TE_TRIM_ALL_FAILED; } set { this._TE_TRIM_ALL_FAILED = value; } }

		private string _TE_TRIM_ONE_FAILED = "Tirm Res Failed";
		public string TE_TRIM_ONE_FAILED { get { return this._TE_TRIM_ONE_FAILED; } set { this._TE_TRIM_ONE_FAILED = value; } }

		private string _TE_MEASURE_ONE_FAILED = "Measure Res Failed";
		public string TE_MEASURE_ONE_FAILED { get { return this._TE_MEASURE_ONE_FAILED; } set { this._TE_MEASURE_ONE_FAILED = value; } }

		private string _TE_MEASURE_COL_FAILED = "Measure Col Failed";
		public string TE_MEASURE_COL_FAILED { get { return this._TE_MEASURE_COL_FAILED; } set { this._TE_MEASURE_COL_FAILED = value; } }

		private string _TE_Collect_Trim_Result_Timeout_Error = "TE_Collect_Trim_Result_Timeout_Error";
		public string TE_Collect_Trim_Result_Timeout_Error { get { return this._TE_Collect_Trim_Result_Timeout_Error; } set { this._TE_Collect_Trim_Result_Timeout_Error = value; } }

		private string _TE_TRIM_ALL_RESULT_GET_FAILED = "Tirm Column getting Result Failed";
		public string TE_TRIM_ALL_RESULT_GET_FAILED { get { return this._TE_TRIM_ALL_RESULT_GET_FAILED; } set { this._TE_TRIM_ALL_RESULT_GET_FAILED = value; } }

		private string _TE_MESURE_ALL_RESULT_GET_FAILED = "Measure Column getting Result Failed";
		public string TE_MESURE_ALL_RESULT_GET_FAILED { get { return this._TE_MESURE_ALL_RESULT_GET_FAILED; } set { this._TE_MESURE_ALL_RESULT_GET_FAILED = value; } }

		private string _TE_SUB_LAYOUT_DATA_BIND_FAILED = "Sub Layout data display Failed";
		public string TE_SUB_LAYOUT_DATA_BIND_FAILED { get { return this._TE_SUB_LAYOUT_DATA_BIND_FAILED; } set { this._TE_SUB_LAYOUT_DATA_BIND_FAILED = value; } }

		private string _TE_SUB_HISTOGRAM_RESET_FAILED = "Histogram Data Reset Failed";
		public string TE_SUB_HISTOGRAM_RESET_FAILED { get { return this._TE_SUB_HISTOGRAM_RESET_FAILED; } set { this._TE_SUB_HISTOGRAM_RESET_FAILED = value; } }

		private string _TE_SUB_HISTOGRAM_DISPLAY_BINDING_FAILED = "Histogram Data Binding Failed";
		public string TE_SUB_HISTOGRAM_DISPLAY_BINDING_FAILED { get { return this._TE_SUB_HISTOGRAM_DISPLAY_BINDING_FAILED; } set { this._TE_SUB_HISTOGRAM_DISPLAY_BINDING_FAILED = value; } }

		private string _TE_SUB_HISTOGRAM_UPDATE_FAILED = "Histogram Data Update Failed";
		public string TE_SUB_HISTOGRAM_UPDATE_FAILED { get { return this._TE_SUB_HISTOGRAM_UPDATE_FAILED; } set { this._TE_SUB_HISTOGRAM_UPDATE_FAILED = value; } }

		private string _TE_GALVO_MOVE_TO_FAILED = "Galvo Movement Failed";
		public string TE_GALVO_MOVE_TO_FAILED { get { return this._TE_GALVO_MOVE_TO_FAILED; } set { this._TE_GALVO_MOVE_TO_FAILED = value; } }

		private string _TE_GALVO_FENCE_MOVE_TO_FAILED = "Galvo Global Movement Failed";
		public string TE_GALVO_FENCE_MOVE_TO_FAILED { get { return this._TE_GALVO_FENCE_MOVE_TO_FAILED; } set { this._TE_GALVO_FENCE_MOVE_TO_FAILED = value; } }

		private string _TE_FILE_WRITE_ERR = "Laser Info file Write Failed";
		public string TE_FILE_WRITE_ERR { get { return this._TE_FILE_WRITE_ERR; } set { this._TE_FILE_WRITE_ERR = value; } }

		private string _TE_MSG_WRITE_ERR = "Trimming Message Write Failed";
		public string TE_MSG_WRITE_ERR { get { return this._TE_MSG_WRITE_ERR; } set { this._TE_MSG_WRITE_ERR = value; } }

		private string _TE_SWATCH_PIN_ERR = "TE Swatch Pin Error";
		public string TE_SWATCH_PIN_ERR { get { return this._TE_SWATCH_PIN_ERR; } set { this._TE_SWATCH_PIN_ERR = value; } }

		private string _TE_INIT_ERR = "Smart Core Init Failed";
		public string TE_INIT_ERR { get { return this._TE_INIT_ERR; } set { this._TE_INIT_ERR = value; } }

		private string _TE_NOT_ONLINE = "Smart Core Offline";
		public string TE_NOT_ONLINE { get { return this._TE_NOT_ONLINE; } set { this._TE_NOT_ONLINE = value; } }

		private string _TE_CARD_INIT_ERR = "UNI-Trim Board Init Failed";
		public string TE_CARD_INIT_ERR { get { return this._TE_CARD_INIT_ERR; } set { this._TE_CARD_INIT_ERR = value; } }

		private string _TE_SETTING_UPDATE_FAILED = "Smart Core Setting Update Failed";
		public string TE_SETTING_UPDATE_FAILED { get { return this._TE_SETTING_UPDATE_FAILED; } set { this._TE_SETTING_UPDATE_FAILED = value; } }

		private string _TE_DATA_INIT_FAILED = "Trimming Data Init Failed";
		public string TE_DATA_INIT_FAILED { get { return this._TE_DATA_INIT_FAILED; } set { this._TE_DATA_INIT_FAILED = value; } }

		private string _TE_POWER_UPDATE_FAILED = "Trimming Power Set Failed";
		public string TE_POWER_UPDATE_FAILED { get { return this._TE_POWER_UPDATE_FAILED; } set { this._TE_POWER_UPDATE_FAILED = value; } }

		private string _TE_INIT_CON_DATA_ERR = "Error On Con Data Initalization ";
		public string TE_INIT_CON_DATA_ERR { get { return this._TE_INIT_CON_DATA_ERR; } set { this._TE_INIT_CON_DATA_ERR = value; } }

		private string _TE_NOMINAL_DNLD_ERR = "Nominal Download Error ";
		public string TE_NOMINAL_DNLD_ERR { get { return this._TE_NOMINAL_DNLD_ERR; } set { this._TE_NOMINAL_DNLD_ERR = value; } }

		private string _TE_PRE_NOMINAL_DNLD_ERR = "Pre-Nominal Download Error";
		public string TE_PRE_NOMINAL_DNLD_ERR { get { return this._TE_PRE_NOMINAL_DNLD_ERR; } set { this._TE_PRE_NOMINAL_DNLD_ERR = value; } }

		private string _TE_CUT_NO_UPDATED_FAILED = "Cut Number Update Error";
		public string TE_CUT_NO_UPDATED_FAILED { get { return this._TE_CUT_NO_UPDATED_FAILED; } set { this._TE_CUT_NO_UPDATED_FAILED = value; } }

		private string _TE_LASER_CONTROL_TYPE_UPDATE = "Laser Control Type Update Failed";
		public string TE_LASER_CONTROL_TYPE_UPDATE { get { return this._TE_LASER_CONTROL_TYPE_UPDATE; } set { this._TE_LASER_CONTROL_TYPE_UPDATE = value; } }

		private string _TE_CUT_PATTERN_UPDATE_ERR = "Cut Selected Pattern Update Error";
		public string TE_CUT_PATTERN_UPDATE_ERR { get { return this._TE_CUT_PATTERN_UPDATE_ERR; } set { this._TE_CUT_PATTERN_UPDATE_ERR = value; } }

		private string _TE_CLEAR_GALVO_ERR_OFFSET_FAILED = "Clear Galvo Error Offsets Failed";
		public string TE_CLEAR_GALVO_ERR_OFFSET_FAILED { get { return this._TE_CLEAR_GALVO_ERR_OFFSET_FAILED; } set { this._TE_CLEAR_GALVO_ERR_OFFSET_FAILED = value; } }

		//Laser related errors
		private string _LASER_STATE_NOT_STANDBY_ERR = "Laser is not in Standby state. Please check with manufacturer's software";
		public string LASER_STATE_NOT_STANDBY_ERR { get { return this._LASER_STATE_NOT_STANDBY_ERR; } set { this._LASER_STATE_NOT_STANDBY_ERR = value; } }

		private string _LASER_STATE_00_ERR = "Laser is in Ramp Up oscillator state. Please check with manufacturer's software";
		public string LASER_STATE_00_ERR { get { return this._LASER_STATE_00_ERR; } set { this._LASER_STATE_00_ERR = value; } }

		private string _LASER_STATE_01_ERR = "Laser is in Equilibrating oscillator state. Please check with manufacturer's software";
		public string LASER_STATE_01_ERR { get { return this._LASER_STATE_01_ERR; } set { this._LASER_STATE_01_ERR = value; } }

		private string _LASER_STATE_02_ERR = "Laser is in Checking IR oscillator state. Please check with manufacturer's software";
		public string LASER_STATE_02_ERR { get { return this._LASER_STATE_02_ERR; } set { this._LASER_STATE_02_ERR = value; } }

		private string _LASER_STATE_06_ERR = "Laser is in Initialize Light Regulation state. Please check with manufacturer's software";
		public string LASER_STATE_06_ERR { get { return this._LASER_STATE_06_ERR; } set { this._LASER_STATE_06_ERR = value; } }

		private string _LASER_STATE_07_ERR = "Laser is in Ramp Up Light Regulation state. Please check with manufacturer's software";
		public string LASER_STATE_07_ERR { get { return this._LASER_STATE_07_ERR; } set { this._LASER_STATE_07_ERR = value; } }

		private string _LASER_STATE_10_ERR = "Laser is in Ramp Down oscillator state. Please check with manufacturer's software";
		public string LASER_STATE_10_ERR { get { return this._LASER_STATE_10_ERR; } set { this._LASER_STATE_10_ERR = value; } }

		private string _LASER_STATE_11_ERR = "Laser is in Standby state. Please turn on laser";
		public string LASER_STATE_11_ERR { get { return this._LASER_STATE_11_ERR; } set { this._LASER_STATE_11_ERR = value; } }

		private string _LASER_STATE_12_ERR = "Laser is in WarmUp state. Please check with manufacturer's software";
		public string LASER_STATE_12_ERR { get { return this._LASER_STATE_12_ERR; } set { this._LASER_STATE_12_ERR = value; } }

		private string _LASER_STATE_20_ERR = "Laser is in Initialize shifter state. Please check with manufacturer's software";
		public string LASER_STATE_20_ERR { get { return this._LASER_STATE_20_ERR; } set { this._LASER_STATE_20_ERR = value; } }

		//Motion Related Errors
		private string _M_PROBER_Z_NOT_AT_SAFETY_ERR = "Z_Prober Is Not At Safety Position";
		public string M_PROBER_Z_NOT_AT_SAFETY_ERR { get { return this._M_PROBER_Z_NOT_AT_SAFETY_ERR; } set { this._M_PROBER_Z_NOT_AT_SAFETY_ERR = value; } }

		private string _M_CAR_X_HOME_ERR = "Carriage X Home Error";
		public string M_CAR_X_HOME_ERR { get { return this._M_CAR_X_HOME_ERR; } set { this._M_CAR_X_HOME_ERR = value; } }

		private string _M_CAR_X_PROBE_CLEAN_ERR = "Carriage X Probe Clean Error";
		public string M_CAR_X_PROBE_CLEAN_ERR { get { return this._M_CAR_X_PROBE_CLEAN_ERR; } set { this._M_CAR_X_PROBE_CLEAN_ERR = value; } }

		private string _M_CAR_THETA_HOME_CMD_SEND = "Carriage Theta Home Command Failed";
		public string M_CAR_THETA_HOME_CMD_SEND { get { return this._M_CAR_THETA_HOME_CMD_SEND; } set { this._M_CAR_THETA_HOME_CMD_SEND = value; } }

		private string _M_CAR_Y_HOME_ERR = "Carriage Y Home Error";
		public string M_CAR_Y_HOME_ERR { get { return this._M_CAR_Y_HOME_ERR; } set { this._M_CAR_Y_HOME_ERR = value; } }

		private string _M_CAR_Y_PROBE_CLEAN_ERR = "Carriage Y Probe Clean Error";
		public string M_CAR_Y_PROBE_CLEAN_ERR { get { return this._M_CAR_Y_PROBE_CLEAN_ERR; } set { this._M_CAR_Y_PROBE_CLEAN_ERR = value; } }

		private string _M_CAR_THETA_HOME_ERR = "Carriage THETA Home Error";
		public string M_CAR_THETA_HOME_ERR { get { return this._M_CAR_THETA_HOME_ERR; } set { this._M_CAR_THETA_HOME_ERR = value; } }

		private string _M_PROBER_Z_HOME_ERR = "Prober Z Home Error";
		public string M_PROBER_Z_HOME_ERR { get { return this._M_PROBER_Z_HOME_ERR; } set { this._M_PROBER_Z_HOME_ERR = value; } }

		private string _M_CAR_X_ABS_MOVE_ERR = "Carriage X Absolute Move Error";
		public string M_CAR_X_ABS_MOVE_ERR { get { return this._M_CAR_X_ABS_MOVE_ERR; } set { this._M_CAR_X_ABS_MOVE_ERR = value; } }

		private string _M_CAR_Y_ABS_MOVE_ERR = "Carriage Y Absolute Move Error";
		public string M_CAR_Y_ABS_MOVE_ERR { get { return this._M_CAR_Y_ABS_MOVE_ERR; } set { this._M_CAR_Y_ABS_MOVE_ERR = value; } }


		private string _M_PROBER_Z_ABS_MOVE_ERR = "Prober Z Absolute Move Error";
		public string M_PROBER_Z_ABS_MOVE { get { return this._M_PROBER_Z_ABS_MOVE_ERR; } set { this._M_PROBER_Z_ABS_MOVE_ERR = value; } }

		private string _M_CAR_X_REL_MOVE_ERR = "Carriage X Relative Move Error";
		public string M_CAR_X_REL_MOVE_ERR { get { return this._M_CAR_X_REL_MOVE_ERR; } set { this._M_CAR_X_REL_MOVE_ERR = value; } }

		private string _M_CAR_X_POS_NOT_GOOD = "Carriage X Position is not suitable to teach";
		public string M_CAR_X_POS_NOT_GOOD { get { return this._M_CAR_X_POS_NOT_GOOD; } set { this._M_CAR_X_POS_NOT_GOOD = value; } }

		private string _M_CAR_X_POSITIVE_LIMIT_REACHED = "Carriage X positive limit reached";
		public string M_CAR_X_POSITIVE_LIMIT_REACHED { get { return this._M_CAR_X_POSITIVE_LIMIT_REACHED; } set { this._M_CAR_X_POSITIVE_LIMIT_REACHED = value; } }

		private string _M_CAR_X_NEGATIVE_LIMIT_REACHED = "Carriage X negative limit reached";
		public string M_CAR_X_NEGATIVE_LIMIT_REACHED { get { return this._M_CAR_X_NEGATIVE_LIMIT_REACHED; } set { this._M_CAR_X_NEGATIVE_LIMIT_REACHED = value; } }

		private string _M_CAR_Y_POSITIVE_LIMIT_REACHED = "Carriage Y positive limit reached";
		public string M_CAR_Y_POSITIVE_LIMIT_REACHED { get { return this._M_CAR_Y_POSITIVE_LIMIT_REACHED; } set { this._M_CAR_Y_POSITIVE_LIMIT_REACHED = value; } }

		private string _M_CAR_Y_NEGATIVE_LIMIT_REACHED = "Carriage Y negative limit reached";
		public string M_CAR_Y_NEGATIVE_LIMIT_REACHED { get { return this._M_CAR_Y_NEGATIVE_LIMIT_REACHED; } set { this._M_CAR_Y_NEGATIVE_LIMIT_REACHED = value; } }

		private string _M_CAR_Y_REL_MOVE_ERR = "Carriage Y Relative Move Error";
		public string M_CAR_Y_REL_MOVE_ERR { get { return this._M_CAR_Y_REL_MOVE_ERR; } set { this._M_CAR_Y_REL_MOVE_ERR = value; } }

		private string _M_CAR_Y_POS_NOT_GOOD = "Carriage Y Position is not suitable to teach";
		public string M_CAR_Y_POS_NOT_GOOD { get { return this._M_CAR_Y_POS_NOT_GOOD; } set { this._M_CAR_Y_POS_NOT_GOOD = value; } }

		private string _M_CAR_THETA_REL_MOVE_ERR = "Carriage Theta Relative Move Error";
		public string M_CAR_THETA_REL_MOVE_ERR { get { return this._M_CAR_THETA_REL_MOVE_ERR; } set { this._M_CAR_THETA_REL_MOVE_ERR = value; } }

		private string _M_CAR_THETA_ABS_MOVE_ERR = "Carriage Theta Absolute Move Error";
		public string M_CAR_THETA_ABS_MOVE_ERR { get { return this._M_CAR_THETA_ABS_MOVE_ERR; } set { this._M_CAR_THETA_ABS_MOVE_ERR = value; } }

		private string _M_CAR_THETA_POS_NOT_GOOD = "Carriage Theta Position is not suitable to teach";
		public string M_CAR_THETA_POS_NOT_GOOD { get { return this._M_CAR_THETA_POS_NOT_GOOD; } set { this._M_CAR_THETA_POS_NOT_GOOD = value; } }

		private string _M_CAR_THETA_SET_POSITION_ERR = "Carriage Theta ADLINK set position error";
		public string M_CAR_THETA_SET_POSITION_ERR { get { return this._M_CAR_THETA_SET_POSITION_ERR; } set { this._M_CAR_THETA_SET_POSITION_ERR = value; } }

		private string _M_PROBER_Z_REL_MOVE_ERR = "Prober Z Relative Move Error";
		public string M_PROBER_Z_REL_MOVE_ERR { get { return this._M_PROBER_Z_REL_MOVE_ERR; } set { this._M_PROBER_Z_REL_MOVE_ERR = value; } }

		private string _M_PROBER_Z_POS_NOT_GOOD = "Prober Z Measure Position is not suitable to teach";
		public string M_PROBER_Z_POS_NOT_GOOD { get { return this._M_PROBER_Z_POS_NOT_GOOD; } set { this._M_PROBER_Z_POS_NOT_GOOD = value; } }

		private string _M_PROBER_Z_POS_REGISTER_ERROR = "Prober Positions register to PMAC Error";
		public string M_PROBER_Z_POS_REGISTER_ERROR { get { return this._M_PROBER_Z_POS_REGISTER_ERROR; } set { this._M_PROBER_Z_POS_REGISTER_ERROR = value; } }

		private string _M_CAR_THETA_TRIM_POS_MOVE_ERR = "Carriage Theta Trim Pos Move Error";
		public string M_CAR_THETA_TRIM_POS_MOVE_ERR { get { return this._M_CAR_THETA_TRIM_POS_MOVE_ERR; } set { this._M_CAR_THETA_TRIM_POS_MOVE_ERR = value; } }

		private string _M_CAR_LOAD_POS_MOVE_ERR = "Carriage Load Pos Move Error";
		public string M_CAR_LOAD_POS_MOVE_ERR { get { return this._M_CAR_LOAD_POS_MOVE_ERR; } set { this._M_CAR_LOAD_POS_MOVE_ERR = value; } }

		private string _M_CAR_DOUBLE_SUB_CHK_POS_MOVE_ERR = "Carriage Double Substrate Check Pos Move Error";
		public string M_CAR_DOUBLE_SUB_CHK_POS_MOVE_ERR { get { return this._M_CAR_DOUBLE_SUB_CHK_POS_MOVE_ERR; } set { this._M_CAR_DOUBLE_SUB_CHK_POS_MOVE_ERR = value; } }

		private string _M_CAR_UNLOAD_POS_MOVE_ERR = "Carriage UnLoad Pos Move Error";
		public string M_CAR_UNLOAD_POS_MOVE_ERR { get { return this._M_CAR_UNLOAD_POS_MOVE_ERR; } set { this._M_CAR_UNLOAD_POS_MOVE_ERR = value; } }

		private string _M_CAR_REJECT_POS_MOVE_ERR = "Carriage Reject Pos Move Error";
		public string M_CAR_REJECT_POS_MOVE_ERR { get { return this._M_CAR_REJECT_POS_MOVE_ERR; } set { this._M_CAR_REJECT_POS_MOVE_ERR = value; } }

		private string _M_CAR_CLEAN_POS_MOVE_ERR = "Carriage Clean Pos Move Error";
		public string M_CAR_CLEAN_POS_MOVE_ERR { get { return this._M_CAR_CLEAN_POS_MOVE_ERR; } set { this._M_CAR_CLEAN_POS_MOVE_ERR = value; } }

		private string _M_CAR_REF_1_POS_MOVE_ERR = "Carriage Ref 1 Pos Move Error";
		public string M_CAR_REF_1_POS_MOVE_ERR { get { return this._M_CAR_REF_1_POS_MOVE_ERR; } set { this._M_CAR_REF_1_POS_MOVE_ERR = value; } }

		private string _M_CAR_GALVO_CAL_POS_MOVE_ERR = "Carriage Galvo Calibration Pos Move Error";
		public string M_CAR_GALVO_CAL_POS_MOVE_ERR { get { return this._M_CAR_GALVO_CAL_POS_MOVE_ERR; } set { this._M_CAR_GALVO_CAL_POS_MOVE_ERR = value; } }

		private string _M_CAR_REF_2_POS_MOVE_ERR = "Carriage Ref 2 Pos Move Error";
		public string M_CAR_REF_2_POS_MOVE_ERR { get { return this._M_CAR_REF_2_POS_MOVE_ERR; } set { this._M_CAR_REF_2_POS_MOVE_ERR = value; } }

		private string _M_PICK_DOUBLE_SUBS_ERR = "Detect Double Substrate On Carriage";
		public string M_PICK_DOUBLE_SUBS_ERR { get { return this._M_PICK_DOUBLE_SUBS_ERR; } set { this._M_PICK_DOUBLE_SUBS_ERR = value; } }

		private string _M_CAR_FIRST_REF_POS_MOVE_ERR = "Carriage First Ref Pos Move Error";
		public string M_CAR_FIRST_REF_POS_MOVE_ERR { get { return this._M_CAR_FIRST_REF_POS_MOVE_ERR; } set { this._M_CAR_FIRST_REF_POS_MOVE_ERR = value; } }

		private string _M_CAR_SECOND_REF_POS_MOVE_ERR = "Carriage Second Ref Pos Move Error";
		public string M_CAR_SECOND_REF_POS_MOVE_ERR { get { return this._M_CAR_SECOND_REF_POS_MOVE_ERR; } set { this._M_CAR_SECOND_REF_POS_MOVE_ERR = value; } }

		private string _M_CAR_NOT_IN_MEASURE_POS_ERR = "Carriage is Not In Measure Position";
		public string M_CAR_NOT_IN_MEASURE_POS_ERR { get { return this._M_CAR_NOT_IN_MEASURE_POS_ERR; } set { this._M_CAR_NOT_IN_MEASURE_POS_ERR = value; } }

		private string _M_CAR_IN_MEASURE_POS_ERR = "Carriage is in Measure Position";
		public string M_CAR_IN_MEASURE_POS_ERR { get { return this._M_CAR_IN_MEASURE_POS_ERR; } set { this._M_CAR_IN_MEASURE_POS_ERR = value; } }

		private string _M_PROBER_Z_MEASURE_MOVE_ERR = "Prober Z Measure Pos Move Error";
		public string M_PROBER_Z_MEASURE_MOVE_ERR { get { return this._M_PROBER_Z_MEASURE_MOVE_ERR; } set { this._M_PROBER_Z_MEASURE_MOVE_ERR = value; } }

		private string _M_PROBER_Z_INDEX_MOVE_ERR = "Prober Z Index Pos Move Error";
		public string M_PROBER_Z_INDEX_MOVE_ERR { get { return this._M_PROBER_Z_INDEX_MOVE_ERR; } set { this._M_PROBER_Z_INDEX_MOVE_ERR = value; } }

		private string _M_PROBER_Z_SAFETY_MOVE_ERR = "Prober Z Safety Pos Move Error";
		public string M_PROBER_Z_SAFETY_MOVE_ERR { get { return this._M_PROBER_Z_SAFETY_MOVE_ERR; } set { this._M_PROBER_Z_SAFETY_MOVE_ERR = value; } }

		private string _M_PROBER_Z_CLEAN_MOVE_ERR = "Prober Z Clean Pos Move Error";
		public string M_PROBER_Z_CLEAN_MOVE_ERR { get { return this._M_PROBER_Z_CLEAN_MOVE_ERR; } set { this._M_PROBER_Z_CLEAN_MOVE_ERR = value; } }

		private string _M_SYSTEM_HOMING_ERR = "System Homing Error";
		public string M_SYSTEM_HOMING_ERR { get { return this._M_SYSTEM_HOMING_ERR; } set { this._M_SYSTEM_HOMING_ERR = value; } }

		private string _M_CAR_CLAMPER_OUTPUT_WRITE_ERR = "Carriage Clamper Open Output Write Error";
		public string M_CAR_CLAMPER_OUTPUT_WRITE_ERR { get { return this._M_CAR_CLAMPER_OUTPUT_WRITE_ERR; } set { this._M_CAR_CLAMPER_OUTPUT_WRITE_ERR = value; } }

		private string _M_SIDE_CYLINDER_CLOSED_ERROR = "Carriage Side Clamper Closed Error";
		public string M_SIDE_CYLINDER_CLOSED_ERROR { get { return this._M_SIDE_CYLINDER_CLOSED_ERROR; } set { this._M_SIDE_CYLINDER_CLOSED_ERROR = value; } }

		private string _M_SIDE_CYLINDER_OUTPUT_ERROR = "Carriage Side Clamper Output Error";
		public string M_SIDE_CYLINDER_OUTPUT_ERROR { get { return this._M_SIDE_CYLINDER_OUTPUT_ERROR; } set { this._M_SIDE_CYLINDER_OUTPUT_ERROR = value; } }

		private string _M_AIR_KNIFE_OUTPUT_WRITE_ERR = "Air-Knife Output Write Error";
		public string M_AIR_KNIFE_OUTPUT_WRITE_ERR { get { return this._M_AIR_KNIFE_OUTPUT_WRITE_ERR; } set { this._M_AIR_KNIFE_OUTPUT_WRITE_ERR = value; } }

		private string _M_PROBE_CARD_CLEAN_ERR = "Probe Card Cleaning Process Error";
		public string M_PROBE_CARD_CLEAN_ERR { get { return this._M_PROBE_CARD_CLEAN_ERR; } set { this._M_PROBE_CARD_CLEAN_ERR = value; } }

		private string _M_SUB_ON_CARRIAGE = "Sub. On Carriage to be Cleared";
		public string M_SUB_ON_CARRIAGE { get { return this._M_SUB_ON_CARRIAGE; } set { this._M_SUB_ON_CARRIAGE = value; } }

		private string _M_CAR_COL_POS_MOVE_ERR = "Carriage Column Pos Move Error";
		public string M_CAR_COL_POS_MOVE_ERR { get { return this._M_CAR_COL_POS_MOVE_ERR; } set { this._M_CAR_COL_POS_MOVE_ERR = value; } }

		private string _M_CAR_ROW_POS_MOVE_ERR = "Carriage Row Pos Move Error";
		public string M_CAR_ROW_POS_MOVE_ERR { get { return this._M_CAR_ROW_POS_MOVE_ERR; } set { this._M_CAR_ROW_POS_MOVE_ERR = value; } }

		private string _M_CAR_COL_ROW_POS_MOVE_ERR = "Carriage Column and Row Pos Move Error";
		public string M_CAR_COL_ROW_POS_MOVE_ERR { get { return this._M_CAR_COL_ROW_POS_MOVE_ERR; } set { this._M_CAR_COL_ROW_POS_MOVE_ERR = value; } }

		private string _M_COL_NO_OUT_OF_RANGE = "Column Number is Out of Range";
		public string M_COL_NO_OUT_OF_RANGE { get { return this._M_COL_NO_OUT_OF_RANGE; } set { this._M_COL_NO_OUT_OF_RANGE = value; } }

		private string _M_GROUP_NO_OUT_OF_RANGE = "Group Number is Out of Range";
		public string M_GROUP_NO_OUT_OF_RANGE { get { return this._M_GROUP_NO_OUT_OF_RANGE; } set { this._M_GROUP_NO_OUT_OF_RANGE = value; } }

		private string _M_INTERLEAVE_GRP_CLEAR = "Inleave Groups Clear Failed";
		public string M_INTERLEAVE_GRP_CLEAR { get { return this._M_INTERLEAVE_GRP_CLEAR; } set { this._M_INTERLEAVE_GRP_CLEAR = value; } }

		private string _M_INTERLEAVE_GRP_REMOVE = "Inleave Group remove Failed";
		public string M_INTERLEAVE_GRP_REMOVE { get { return this._M_INTERLEAVE_GRP_REMOVE; } set { this._M_INTERLEAVE_GRP_REMOVE = value; } }

		private string _M_PNP_ARM_NOT_ELE_POS = "PNP Arm is not at Elevator Pos";
		public string M_PNP_ARM_NOT_ELE_POS { get { return this._M_PNP_ARM_NOT_ELE_POS; } set { this._M_PNP_ARM_NOT_ELE_POS = value; } }

		private string _M_PNP_ARM_NOT_CAR_POS = "PNP Arm is not at Car Pos";
		public string M_PNP_ARM_NOT_CAR_POS { get { return this._M_PNP_ARM_NOT_CAR_POS; } set { this._M_PNP_ARM_NOT_CAR_POS = value; } }

		private string _M_PNP_ARM_NOT_NG_TRAY_POS = "PNP Arm is not at NG-Tray Pos";
		public string M_PNP_ARM_NOT_NG_TRAY_POS { get { return this._M_PNP_ARM_NOT_NG_TRAY_POS; } set { this._M_PNP_ARM_NOT_NG_TRAY_POS = value; } }

		private string _M_PNP_ARM_SAFETY_CHECK_FAILED = "PNP Arm safety Check failed";
		public string M_PNP_ARM_SAFETY_CHECK_FAILED { get { return this._M_PNP_ARM_SAFETY_CHECK_FAILED; } set { this._M_PNP_ARM_SAFETY_CHECK_FAILED = value; } }

		private string _M_PNP_ARM_NOT_CAR_POS_OUTPUT_WRITE = "PNP Arm is to Car Pos output Wirte Error";
		public string M_PNP_ARM_NOT_CAR_POS_OUTPUT_WRITE { get { return this._M_PNP_ARM_NOT_CAR_POS_OUTPUT_WRITE; } set { this._M_PNP_ARM_NOT_CAR_POS_OUTPUT_WRITE = value; } }

		private string _M_PNP_SET_MOVE_FLAG_ERR = "PNP Arm Set Move Flag Error";
		public string M_PNP_SET_MOVE_FLAG_ERR { get { return this._M_PNP_SET_MOVE_FLAG_ERR; } set { this._M_PNP_SET_MOVE_FLAG_ERR = value; } }

		private string _M_PNP_RESET_POS_BITS_ERR = "PNP Reset Position Bits Error";
		public string M_PNP_RESET_POS_BITS_ERR { get { return this._M_PNP_RESET_POS_BITS_ERR; } set { this._M_PNP_RESET_POS_BITS_ERR = value; } }

		private string _M_PNP_SET_POS_BITS_ERR = "PNP set Position Bits Error";
		public string M_PNP_SET_POS_BITS_ERR { get { return this._M_PNP_SET_POS_BITS_ERR; } set { this._M_PNP_SET_POS_BITS_ERR = value; } }

		private string _M_PNP_SET_START_MOVE_BIT_ERR = "PNP Set Start Move Bit Error";
		public string M_PNP_SET_START_MOVE_BIT_ERR { get { return this._M_PNP_SET_START_MOVE_BIT_ERR; } set { this._M_PNP_SET_START_MOVE_BIT_ERR = value; } }

		private string _M_PNP_RESET_START_MOVE_BIT_ERR = "PNP Reset Start Move Bit Error";
		public string M_PNP_RESET_START_MOVE_BIT_ERR { get { return this._M_PNP_RESET_START_MOVE_BIT_ERR; } set { this._M_PNP_RESET_START_MOVE_BIT_ERR = value; } }

		private string _M_PNP_MOVE_DONE_CHECK_ERR = "PNP Move Done Check Error";
		public string M_PNP_MOVE_DONE_CHECK_ERR { get { return this._M_PNP_MOVE_DONE_CHECK_ERR; } set { this._M_PNP_MOVE_DONE_CHECK_ERR = value; } }

		private string _M_PNP_MOVE_NOT_COMPLETE = "PNP Move Not Complete";
		public string M_PNP_MOVE_NOT_COMPLETE { get { return this._M_PNP_MOVE_NOT_COMPLETE; } set { this._M_PNP_MOVE_NOT_COMPLETE = value; } }

		private string _M_PNP_SET_SPEED_ERR = "PNP Set Speed Error";
		public string M_PNP_SET_SPEED_ERR { get { return this._M_PNP_SET_SPEED_ERR; } set { this._M_PNP_SET_SPEED_ERR = value; } }

		private string _M_PNP_ARM_HOME_ERR = "PNP Arm Home Error";
		public string M_PNP_ARM_HOME_ERR { get { return this._M_PNP_ARM_HOME_ERR; } set { this._M_PNP_ARM_HOME_ERR = value; } }

		private string _M_PNP_ARM_NOT_ELE_POS_OUTPUT_WRITE = "PNP Arm Move to Ele Pos output Wirte Error";
		public string M_PNP_ARM_NOT_ELE_POS_OUTPUT_WRITE { get { return this._M_PNP_ARM_NOT_ELE_POS_OUTPUT_WRITE; } set { this._M_PNP_ARM_NOT_ELE_POS_OUTPUT_WRITE = value; } }

		private string _M_REJECT_ARM_OUT_OUTPUT_WRITE = "Reject Arm Out Output Write Error";
		public string M_REJECT_ARM_OUT_OUTPUT_WRITE { get { return this._M_REJECT_ARM_OUT_OUTPUT_WRITE; } set { this._M_REJECT_ARM_OUT_OUTPUT_WRITE = value; } }

		private string _M_REJECT_ARM_VACCUM_OUTPUT_WRITE = "Reject Arm Out Output Write Error";
		public string M_REJECT_ARM_VACCUM_OUTPUT_WRITE { get { return this._M_REJECT_ARM_VACCUM_OUTPUT_WRITE; } set { this._M_REJECT_ARM_VACCUM_OUTPUT_WRITE = value; } }

		private string _M_CAR_X_RESET_ERR = "Carriage X Reset Error";
		public string M_CAR_X_RESET_ERR { get { return this._M_CAR_X_RESET_ERR; } set { this._M_CAR_X_RESET_ERR = value; } }

		private string _M_CAR_X_DISABLE_ERR = "Carriage X is Disable";
		public string M_CAR_X_DISABLE_ERR { get { return this._M_CAR_X_DISABLE_ERR; } set { this._M_CAR_X_DISABLE_ERR = value; } }

		private string _M_CAR_Y_RESET_ERR = "Carriage Y Reset Error";
		public string M_CAR_Y_RESET_ERR { get { return this._M_CAR_Y_RESET_ERR; } set { this._M_CAR_Y_RESET_ERR = value; } }

		private string _M_CAR_Y_DISABLE_ERR = "Carriage Y is Disable";
		public string M_CAR_Y_DISABLE_ERR { get { return this._M_CAR_Y_DISABLE_ERR; } set { this._M_CAR_Y_DISABLE_ERR = value; } }

		private string _M_CAR_THETA_RESET_ERR = "Carriage THETA Reset Error";
		public string M_CAR_THETA_RESET_ERR { get { return this._M_CAR_THETA_RESET_ERR; } set { this._M_CAR_THETA_RESET_ERR = value; } }

		private string _M_CAR_THETA_SET_SPEED_ERR = "Carriage THETA Set Speed Error";
		public string M_CAR_THETA_SET_SPEED_ERR { get { return this._M_CAR_THETA_SET_SPEED_ERR; } set { this._M_CAR_THETA_SET_SPEED_ERR = value; } }

		private string _M_CAR_THETA_MOTOR_DISABLE_ERR = "Carriage Theta Motor is Disable";
		public string M_CAR_THETA_MOTOR_DISABLE_ERR { get { return this._M_CAR_THETA_MOTOR_DISABLE_ERR; } set { this._M_CAR_THETA_MOTOR_DISABLE_ERR = value; } }

		private string _M_PROBER_Z_RESET_ERR = "Prober Z Reset Error";
		public string M_PROBER_Z_RESET_ERR { get { return this._M_PROBER_Z_RESET_ERR; } set { this._M_PROBER_Z_RESET_ERR = value; } }

		private string _M_PROBER_Z_DISABLE_ERR = "Prober Z is Disable";
		public string M_PROBER_Z_DISABLE_ERR { get { return this._M_PROBER_Z_DISABLE_ERR; } set { this._M_PROBER_Z_DISABLE_ERR = value; } }

		private string _M_PROBER_Z_STATUS_ERR = "Prober Z status error";
		public string M_PROBER_Z_STATUS_ERR { get { return this._M_PROBER_Z_STATUS_ERR; } set { this._M_PROBER_Z_STATUS_ERR = value; } }

		private string _M_CAR_BROKEN_SUB = "Broken Substrate";
		public string M_CAR_BROKEN_SUB { get { return this._M_CAR_BROKEN_SUB; } set { this._M_CAR_BROKEN_SUB = value; } }

		private string _M_CAR_NO_SUB_PRESENT = "No Substrate Present";
		public string M_CAR_NO_SUB_PRESENT { get { return this._M_CAR_NO_SUB_PRESENT; } set { this._M_CAR_NO_SUB_PRESENT = value; } }

		private string _M_CAR_NO_VACCUM_DETECT = "No Vaccum Detect";
		public string M_CAR_NO_VACCUM_DETECT { get { return this._M_CAR_NO_VACCUM_DETECT; } set { this._M_CAR_NO_VACCUM_DETECT = value; } }

		private string _M_POWERMETER_BLOCKING_FAIL = "Power Meter block failed";
		public string M_POWERMETER_BLOCKING_FAIL { get { return this._M_POWERMETER_BLOCKING_FAIL; } set { this._M_POWERMETER_BLOCKING_FAIL = value; } }
		private string _M_POWERMETER_UNBLOCKING_FAIL = "Power Meter unblock failed";
		public string M_POWERMETER_UNBLOCKING_FAIL { get { return this._M_POWERMETER_UNBLOCKING_FAIL; } set { this._M_POWERMETER_UNBLOCKING_FAIL = value; } }


		private string _M_SHUTTER_BLOCKING_FAIL = "Shutter block failed";
		public string M_SHUTTER_BLOCKING_FAIL { get { return this._M_SHUTTER_BLOCKING_FAIL; } set { this._M_SHUTTER_BLOCKING_FAIL = value; } }
		private string _M_SHUTTER_UNBLOCKING_FAIL = "Shutter unblock failed";
		public string M_SHUTTER_UNBLOCKING_FAIL { get { return this._M_SHUTTER_UNBLOCKING_FAIL; } set { this._M_SHUTTER_UNBLOCKING_FAIL = value; } }

		private string _M_CAR_FAILED_TO_UPDATE_VACUUM_IO = "Failed To Update Carriage Vacuum IO";
		public string M_CAR_FAILED_TO_UPDATE_VACUUM_IO { get { return this._M_CAR_FAILED_TO_UPDATE_VACUUM_IO; } set { this._M_CAR_FAILED_TO_UPDATE_VACUUM_IO = value; } }

		private string _M_FAILED_TO_UPDATE_IO = "ADLINK update IO error";
		public string M_FAILED_TO_UPDATE_IO { get { return this._M_FAILED_TO_UPDATE_IO; } set { this._M_FAILED_TO_UPDATE_IO = value; } }

		private string _M_FAILED_TO_READ_IO = "ADLINK read IO error";
		public string M_FAILED_TO_READ_IO { get { return this._M_FAILED_TO_READ_IO; } set { this._M_FAILED_TO_READ_IO = value; } }

		private string _M_PLC_SYSTEM_BUSY = "PLC System Busy";
		public string M_PLC_SYSTEM_BUSY { get { return this._M_PLC_SYSTEM_BUSY; } set { this._M_PLC_SYSTEM_BUSY = value; } }

		private string _M_PLC_SIGNALS_ABNORMAL = "Abnormal PNP signals, please check wiring connection between PNP system and Trimmer";
		public string M_PLC_SIGNALS_ABNORMAL { get { return this._M_PLC_SIGNALS_ABNORMAL; } set { this._M_PLC_SIGNALS_ABNORMAL = value; } }

		private string _M_PLC_IN_ERROR_STATE = "PNP system in error status. Please check PNP system.";
		public string M_PLC_IN_ERROR_STATE { get { return this._M_PLC_IN_ERROR_STATE; } set { this._M_PLC_IN_ERROR_STATE = value; } }
		private string _M_PLC_NO_NEW_SUB = "PNP system indicates no new substrate.";
		public string M_PLC_NO_NEW_SUB { get { return this._M_PLC_NO_NEW_SUB; } set { this._M_PLC_NO_NEW_SUB = value; } }

		private string _M_PLC_SYSTEM_NO_ACK = "No Acknowledgement from PNP system";
		public string M_PLC_SYSTEM_NO_ACK { get { return this._M_PLC_SYSTEM_NO_ACK; } set { this._M_PLC_SYSTEM_NO_ACK = value; } }

		private string _M_PNP_NEW_ARM_NOT_FULLY_DOWN = "PNP New Arm is not fully down!";
		public string M_PNP_NEW_ARM_NOT_FULLY_DOWN { get { return this._M_PNP_NEW_ARM_NOT_FULLY_DOWN; } set { this._M_PNP_NEW_ARM_NOT_FULLY_DOWN = value; } }

		private string _M_PNP_NEW_ARM_NOT_FULLY_UP = "PNP New Arm is not fully Up!";
		public string M_PNP_NEW_ARM_NOT_FULLY_UP { get { return this._M_PNP_NEW_ARM_NOT_FULLY_UP; } set { this._M_PNP_NEW_ARM_NOT_FULLY_UP = value; } }

		private string _M_PNP_NEW_ARM_POSITION_UPDATE_ERR = "PNP New Arm position update error!";
		public string M_PNP_NEW_ARM_POSITION_UPDATE_ERR { get { return this._M_PNP_NEW_ARM_POSITION_UPDATE_ERR; } set { this._M_PNP_NEW_ARM_POSITION_UPDATE_ERR = value; } }

		private string _M_PNP_NEW_ARM_DISABLE_MOTOR_ERR = "PNP New Arm disable motor error!";
		public string M_PNP_NEW_ARM_DISABLE_MOTOR_ERR { get { return this._M_PNP_NEW_ARM_DISABLE_MOTOR_ERR; } set { this._M_PNP_NEW_ARM_DISABLE_MOTOR_ERR = value; } }

		private string _M_PNP_NEW_ARM_HOME_ERR = "PNP New Arm Home Error";
		public string M_PNP_NEW_ARM_HOME_ERR { get { return this._M_PNP_NEW_ARM_HOME_ERR; } set { this._M_PNP_NEW_ARM_HOME_ERR = value; } }

		private string _M_PNP_NEW_ARM_POS_READ_ERR = "PNP New Arm Position Read Error";
		public string M_PNP_NEW_ARM_POS_READ_ERR { get { return this._M_PNP_NEW_ARM_POS_READ_ERR; } set { this._M_PNP_NEW_ARM_POS_READ_ERR = value; } }

		private string _M_PNP_NEW_ARM_WAIT_HOMING_ERR = "PNP New Arm Wait Homing Error";
		public string M_PNP_NEW_ARM_WAIT_HOMING_ERR { get { return this._M_PNP_NEW_ARM_WAIT_HOMING_ERR; } set { this._M_PNP_NEW_ARM_WAIT_HOMING_ERR = value; } }

		private string _M_PNP_CUT_ARM_DISABLE_MOTOR_ERR = "PNP Cut Arm disable motor error!";
		public string M_PNP_CUT_ARM_DISABLE_MOTOR_ERR { get { return this._M_PNP_CUT_ARM_DISABLE_MOTOR_ERR; } set { this._M_PNP_CUT_ARM_DISABLE_MOTOR_ERR = value; } }

		private string _M_PNP_DISABLE_MOTOR_ERR = "PNP disable motor error!";
		public string M_PNP_DISABLE_MOTOR_ERR { get { return this._M_PNP_DISABLE_MOTOR_ERR; } set { this._M_PNP_DISABLE_MOTOR_ERR = value; } }

		private string _M_PNP_CUT_ARM_NO_SUB_PRESENT = "No Substrate on PNP Cut Arm!";
		public string M_PNP_CUT_ARM_NO_SUB_PRESENT { get { return this._M_PNP_CUT_ARM_NO_SUB_PRESENT; } set { this._M_PNP_CUT_ARM_NO_SUB_PRESENT = value; } }

		private string _M_PNP_CUT_VACUUM_ERROR = "Cut Arm Vacuum Error";
		public string M_PNP_CUT_VACUUM_ERROR { get { return this._M_PNP_CUT_VACUUM_ERROR; } set { this._M_PNP_CUT_VACUUM_ERROR = value; } }

		private string _M_FAILED_TO_UNLOAD_FROM_CARRIAGE = "Failed To Unload From Carriage";
		public string M_FAILED_TO_UNLOAD_FROM_CARRIAGE { get { return this._M_FAILED_TO_UNLOAD_FROM_CARRIAGE; } set { this._M_FAILED_TO_UNLOAD_FROM_CARRIAGE = value; } }

		private string _M_PNP_CUT_ARM_NOT_FULLY_DOWN = "PNP Cut Arm is not fully down!";
		public string M_PNP_CUT_ARM_NOT_FULLY_DOWN { get { return this._M_PNP_CUT_ARM_NOT_FULLY_DOWN; } set { this._M_PNP_CUT_ARM_NOT_FULLY_DOWN = value; } }

		private string _M_PNP_CUT_ARM_NOT_FULLY_UP = "PNP Cut Arm is not fully Up!";
		public string M_PNP_CUT_ARM_NOT_FULLY_UP { get { return this._M_PNP_CUT_ARM_NOT_FULLY_UP; } set { this._M_PNP_CUT_ARM_NOT_FULLY_UP = value; } }

		private string _M_PNP_CUT_ARM_POSITION_UPDATE_ERR = "PNP Cut Arm position update error!";
		public string M_PNP_CUT_ARM_POSITION_UPDATE_ERR { get { return this._M_PNP_CUT_ARM_POSITION_UPDATE_ERR; } set { this._M_PNP_CUT_ARM_POSITION_UPDATE_ERR = value; } }

		private string _M_PNP_CUT_ARM_HOME_ERR = "PNP Cut Arm Home Error";
		public string M_PNP_CUT_ARM_HOME_ERR { get { return this._M_PNP_CUT_ARM_HOME_ERR; } set { this._M_PNP_CUT_ARM_HOME_ERR = value; } }

		private string _M_PNP_CUT_ARM_POS_READ_ERR = "PNP Cut Arm Position Read Error";
		public string M_PNP_CUT_ARM_POS_READ_ERR { get { return this._M_PNP_CUT_ARM_POS_READ_ERR; } set { this._M_PNP_CUT_ARM_POS_READ_ERR = value; } }

		private string _M_PNP_CUT_ARM_WAIT_HOMING_ERR = "PNP Cut Arm Wait Homing Error";
		public string M_PNP_CUT_ARM_WAIT_HOMING_ERR { get { return this._M_PNP_CUT_ARM_WAIT_HOMING_ERR; } set { this._M_PNP_CUT_ARM_WAIT_HOMING_ERR = value; } }

		private string _M_PNP_CUT_ARM_SET_SPEED_ERR = "PNP Cut Arm Set Speed Error";
		public string M_PNP_CUT_ARM_SET_SPEED_ERR { get { return this._M_PNP_CUT_ARM_SET_SPEED_ERR; } set { this._M_PNP_CUT_ARM_SET_SPEED_ERR = value; } }


		private string _M_SUB_ON_CAR_NEED_TO_CLEAR = "Substrate on the Carriage is needed to clear manually.";
		public string M_SUB_ON_CAR_NEED_TO_CLEAR { get { return this._M_SUB_ON_CAR_NEED_TO_CLEAR; } set { this._M_SUB_ON_CAR_NEED_TO_CLEAR = value; } }

		private string _M_REJECT_NOT_FULLY_OUT = "Reject Tray not fully out";
		public string M_REJECT_NOT_FULLY_OUT { get { return this._M_REJECT_NOT_FULLY_OUT; } set { this._M_REJECT_NOT_FULLY_OUT = value; } }

		private string _M_REJECT_NOT_FULLY_IN = "Reject Tray not fully in";
		public string M_REJECT_NOT_FULLY_IN { get { return this._M_REJECT_NOT_FULLY_IN; } set { this._M_REJECT_NOT_FULLY_IN = value; } }

		private string _M_PNP_CUT_ARM_DOWN_OUTPUT_WRITE_ERR = "PNP Cut Arm is Down Write Error!";
		public string M_PNP_CUT_ARM_DOWN_OUTPUT_WRITE_ERR { get { return this._M_PNP_CUT_ARM_DOWN_OUTPUT_WRITE_ERR; } set { this._M_PNP_CUT_ARM_DOWN_OUTPUT_WRITE_ERR = value; } }

		private string _M_PNP_CUT_ARM_VACCUM_OUTPUT_WRITE_ERR = "PNP Cut Arm is Vaccum Write Error!";
		public string M_PNP_CUT_ARM_VACCUM_OUTPUT_WRITE_ERR { get { return this._M_PNP_CUT_ARM_VACCUM_OUTPUT_WRITE_ERR; } set { this._M_PNP_CUT_ARM_VACCUM_OUTPUT_WRITE_ERR = value; } }

		private string _M_PNP_NEW_ARM_DOWN_OUTPUT_WRITE_ERR = "PNP New Arm is Down Write Error!";
		public string M_PNP_NEW_ARM_DOWN_OUTPUT_WRITE_ERR { get { return this._M_PNP_NEW_ARM_DOWN_OUTPUT_WRITE_ERR; } set { this._M_PNP_NEW_ARM_DOWN_OUTPUT_WRITE_ERR = value; } }

		private string _M_PNP_NEW_ARM_VACCUM_OUTPUT_WRITE_ERR = "PNP New Arm is Vaccum Write Error!";
		public string M_PNP_NEW_ARM_VACCUM_OUTPUT_WRITE_ERR { get { return this._M_PNP_NEW_ARM_VACCUM_OUTPUT_WRITE_ERR; } set { this._M_PNP_NEW_ARM_VACCUM_OUTPUT_WRITE_ERR = value; } }

		private string _M_PNP_NEW_ARM_VACCUM_NOT_DETECTED = "PNP New Arm Vacuum Not Detected!";
		public string M_PNP_NEW_ARM_VACCUM_NOT_DETECTED { get { return this._M_PNP_NEW_ARM_VACCUM_NOT_DETECTED; } set { this._M_PNP_NEW_ARM_VACCUM_NOT_DETECTED = value; } }

		private string _M_PNP_FAIL_TO_PICK_FROM_NEW = "PNP New Arm Failed to pick up from New Magzine";
		public string M_PNP_FAIL_TO_PICK_FROM_NEW { get { return this._M_PNP_FAIL_TO_PICK_FROM_NEW; } set { this._M_PNP_FAIL_TO_PICK_FROM_NEW = value; } }

		private string _M_NEW_ELE_SNGU_OUTPUT_WRITE_ERR = "New Elevator Snug Out Error";
		public string M_NEW_ELE_SNGU_OUTPUT_WRITE_ERR { get { return this._M_NEW_ELE_SNGU_OUTPUT_WRITE_ERR; } set { this._M_NEW_ELE_SNGU_OUTPUT_WRITE_ERR = value; } }

		private string _M_NEW_ELE_NEW_BLOW_WRITE_ERR = "New Elevator Blow Output Error";
		public string M_NEW_ELE_NEW_BLOW_WRITE_ERR { get { return this._M_NEW_ELE_NEW_BLOW_WRITE_ERR; } set { this._M_NEW_ELE_NEW_BLOW_WRITE_ERR = value; } }

		private string _M_PNP_NEW_ARM_UP_DN_WRITE_OUPPUT_ERR = "New Arm Up Down Error";
		public string M_PNP_NEW_ARM_UP_DN_WRITE_OUPPUT_ERR { get { return this._M_PNP_NEW_ARM_UP_DN_WRITE_OUPPUT_ERR; } set { this._M_PNP_NEW_ARM_UP_DN_WRITE_OUPPUT_ERR = value; } }

		private string _M_PNP_NEW_ARM_NOT_AT_STANDBY_POS = "New Arm Not At Standby Position";
		public string M_PNP_NEW_ARM_NOT_AT_STANDBY_POS { get { return this._M_PNP_NEW_ARM_NOT_AT_STANDBY_POS; } set { this._M_PNP_NEW_ARM_NOT_AT_STANDBY_POS = value; } }

		private string _M_PNP_NEW_ARM_NOT_AT_DOWN_POS = "New Arm Not At Down Position";
		public string M_PNP_NEW_ARM_NOT_AT_DOWN_POS { get { return this._M_PNP_NEW_ARM_NOT_AT_DOWN_POS; } set { this._M_PNP_NEW_ARM_NOT_AT_DOWN_POS = value; } }

		private string _M_PNP_NEW_ARM_SET_SPEED_ERR = "PNP New Arm Set Speed Error";
		public string M_PNP_NEW_ARM_SET_SPEED_ERR { get { return this._M_PNP_NEW_ARM_SET_SPEED_ERR; } set { this._M_PNP_NEW_ARM_SET_SPEED_ERR = value; } }

		private string _M_PNP_NEW_ARM_STOPPER_UP_DN_WRITE_OUPPUT_ERR = "New Arm Stopper Up Down Error";
		public string M_PNP_NEW_ARM_STOPPER_UP_DN_WRITE_OUPPUT_ERR { get { return this._M_PNP_NEW_ARM_STOPPER_UP_DN_WRITE_OUPPUT_ERR; } set { this._M_PNP_NEW_ARM_STOPPER_UP_DN_WRITE_OUPPUT_ERR = value; } }

		private string _M_NEW_ELE_AUTO_OUPPUT_ERR = "New Elevator Auto-Mode Set Error";
		public string M_NEW_ELE_AUTO_OUPPUT_ERR { get { return this._M_NEW_ELE_AUTO_OUPPUT_ERR; } set { this._M_NEW_ELE_AUTO_OUPPUT_ERR = value; } }

		private string _M_NEW_ELE_MANUAL_DOWN_ERR = "New Elevator Manual Down Error";
		public string M_NEW_ELE_MANUAL_DOWN_ERR { get { return this._M_NEW_ELE_MANUAL_DOWN_ERR; } set { this._M_NEW_ELE_MANUAL_DOWN_ERR = value; } }

		private string _M_NEW_ELE_MANUAL_STOP_ERR = "New Elevator Manual Stop Error";
		public string M_NEW_ELE_MANUAL_STOP_ERR { get { return this._M_NEW_ELE_MANUAL_STOP_ERR; } set { this._M_NEW_ELE_MANUAL_STOP_ERR = value; } }

		private string _M_NEW_ELE_HOME_ERR = "New Elevator Home Error";
		public string M_NEW_ELE_HOME_ERR { get { return this._M_NEW_ELE_HOME_ERR; } set { this._M_NEW_ELE_HOME_ERR = value; } }

		private string _M_NEW_ELE_EMPTY = "New Elevator Empty";
		public string M_NEW_ELE_EMPTY { get { return this._M_NEW_ELE_EMPTY; } set { this._M_NEW_ELE_EMPTY = value; } }

		private string _M_NEW_ELE_WAIT_HOMING_ERR = "New Elevator Wait Homing Error";
		public string M_NEW_ELE_WAIT_HOMING_ERR { get { return this._M_NEW_ELE_WAIT_HOMING_ERR; } set { this._M_NEW_ELE_WAIT_HOMING_ERR = value; } }

		private string _M_PNP_NEW_ARM_PICKUP_FAILED = "Pick From New Magazine Failed";
		public string M_PNP_NEW_ARM_PICKUP_FAILED { get { return this._M_PNP_NEW_ARM_PICKUP_FAILED; } set { this._M_PNP_NEW_ARM_PICKUP_FAILED = value; } }

		private string _M_NEW_ELE_SET_SPEED_ERR = "New Elevator Set Speed Error";
		public string M_NEW_ELE_SET_SPEED_ERR { get { return this._M_NEW_ELE_SET_SPEED_ERR; } set { this._M_NEW_ELE_SET_SPEED_ERR = value; } }

		private string _M_CUT_ELE_HOME_ERR = "Cut Elevator Home Error";
		public string M_CUT_ELE_HOME_ERR { get { return this._M_CUT_ELE_HOME_ERR; } set { this._M_CUT_ELE_HOME_ERR = value; } }

		private string _M_CUT_ELE_WAIT_HOMING_ERR = "Cut Elevator Wait Homing Error";
		public string M_CUT_ELE_WAIT_HOMING_ERR { get { return this._M_CUT_ELE_WAIT_HOMING_ERR; } set { this._M_CUT_ELE_WAIT_HOMING_ERR = value; } }

		private string _M_CUT_ELE_SET_SPEED_ERR = "Cut Elevator Set Speed Error";
		public string M_CUT_ELE_SET_SPEED_ERR { get { return this._M_CUT_ELE_SET_SPEED_ERR; } set { this._M_CUT_ELE_SET_SPEED_ERR = value; } }


		private string _M_PNP_NEW_STOPPER_NOT_FULLY_DOWN = "PNP New Stopper is not fully down!";
		public string M_PNP_NEW_STOPPER_NOT_FULLY_DOWN { get { return this._M_PNP_NEW_STOPPER_NOT_FULLY_DOWN; } set { this._M_PNP_NEW_STOPPER_NOT_FULLY_DOWN = value; } }

		private string _M_PNP_NEW_STOPPER_NOT_FULLY_UP = "PNP New Stopper is not fully Up!";
		public string M_PNP_NEW_STOPPER_NOT_FULLY_UP { get { return this._M_PNP_NEW_STOPPER_NOT_FULLY_UP; } set { this._M_PNP_NEW_STOPPER_NOT_FULLY_UP = value; } }

		private string _M_MEGA_OHM_SWITCH_NOT_ON = "Mega-Ohm Switch Cannot On!";
		public string M_MEGA_OHM_SWITCH_NOT_ON { get { return this._M_MEGA_OHM_SWITCH_NOT_ON; } set { this._M_MEGA_OHM_SWITCH_NOT_ON = value; } }

		private string _M_MEGA_OHM_SWITCH_NOT_OFF = "Mega-Ohm Switch Cannot Off!";
		public string M_MEGA_OHM_SWITCH_NOT_OFF { get { return this._M_MEGA_OHM_SWITCH_NOT_OFF; } set { this._M_MEGA_OHM_SWITCH_NOT_OFF = value; } }

		private string _M_INIT_MOTION_ERR = "Initialization Motion Error";
		public string M_INIT_MOTION_ERR { get { return this._M_INIT_MOTION_ERR; } set { this._M_INIT_MOTION_ERR = value; } }

		private string _M_INIT_ARDUINO_ERR = "Initialization Serial Arduino Error";
		public string M_INIT_ARDUINO_ERR { get { return this._M_INIT_ARDUINO_ERR; } set { this._M_INIT_ARDUINO_ERR = value; } }

		private string _M_ENABLE_MOTORS_ERR = "Enable Motors Error";
		public string M_ENABLE_MOTORS_ERR { get { return this._M_ENABLE_MOTORS_ERR; } set { this._M_ENABLE_MOTORS_ERR = value; } }

		private string _M_INIT_IO_ERR = "Initialization IO Error";
		public string M_INIT_IO_ERR { get { return this._M_INIT_IO_ERR; } set { this._M_INIT_IO_ERR = value; } }

		private string _M_LOAD_IO_CONFIG_FAILED = "Load IO Configuration Failed";
		public string M_LOAD_IO_CONFIG_FAILED { get { return this._M_LOAD_IO_CONFIG_FAILED; } set { this._M_LOAD_IO_CONFIG_FAILED = value; } }

		private string _M_LOAD_MOTION_CONFIG_FAILED = "Load Motion Configuration Failed";
		public string M_LOAD_MOTION_CONFIG_FAILED { get { return this._M_LOAD_MOTION_CONFIG_FAILED; } set { this._M_LOAD_MOTION_CONFIG_FAILED = value; } }

		private string _M_ACS_INIT_FAILED = "ACS Motion Initialization Failed";
		public string M_ACS_INIT_FAILED { get { return this._M_ACS_INIT_FAILED; } set { this._M_ACS_INIT_FAILED = value; } }

		private string _M_ADLINK_INIT_FAILED = "Adlink Motion Initialization Failed";
		public string M_ADLINK_INIT_FAILED { get { return this._M_ADLINK_INIT_FAILED; } set { this._M_ADLINK_INIT_FAILED = value; } }

		private string _M_ELMO_INIT_FAILED = "ELMO Motion Initialization Failed";
		public string M_ELMO_INIT_FAILED { get { return this._M_ELMO_INIT_FAILED; } set { this._M_ELMO_INIT_FAILED = value; } }

		private string _M_PMAC_INIT_FAILED = "PMAC Motion Initialization Failed";
		public string M_PMAC_INIT_FAILED { get { return this._M_PMAC_INIT_FAILED; } set { this._M_PMAC_INIT_FAILED = value; } }

		private string _M_CAR_THETA_SERIAL_COM_INIT_FAILED = "Theta Motor Serial Com Initialization Failed";
		public string M_CAR_THETA_SERIAL_COM_INIT_FAILED { get { return this._M_CAR_THETA_SERIAL_COM_INIT_FAILED; } set { this._M_CAR_THETA_SERIAL_COM_INIT_FAILED = value; } }

		private string _M_ACS_MOVE_ERR = "ACS Motion Move Error";
		public string M_ACS_MOVE_ERR { get { return this._M_ACS_MOVE_ERR; } set { this._M_ACS_MOVE_ERR = value; } }

		private string _M_ACS_X_AXIS_SET_SPEED_ERR = "ACS X-Axis Set Speed Error";
		public string M_ACS_X_AXIS_SET_SPEED_ERR { get { return this._M_ACS_X_AXIS_SET_SPEED_ERR; } set { this._M_ACS_X_AXIS_SET_SPEED_ERR = value; } }

		private string _M_ACS_Y_AXIS_SET_SPEED_ERR = "ACS Y-Axis Set Speed Error";
		public string M_ACS_Y_AXIS_SET_SPEED_ERR { get { return this._M_ACS_Y_AXIS_SET_SPEED_ERR; } set { this._M_ACS_Y_AXIS_SET_SPEED_ERR = value; } }

		private string _M_ACS_Z_AXIS_SET_SPEED_ERR = "ACS Z-Axis Set Speed Error";
		public string M_ACS_Z_AXIS_SET_SPEED_ERR { get { return this._M_ACS_Z_AXIS_SET_SPEED_ERR; } set { this._M_ACS_Z_AXIS_SET_SPEED_ERR = value; } }

		private string _M_ADLINK_Z_AXIS_SET_SPEED_ERR = "ADLINK Z-Axis Set Speed Error";
		public string M_ADLINK_Z_AXIS_SET_SPEED_ERR { get { return this._M_ADLINK_Z_AXIS_SET_SPEED_ERR; } set { this._M_ADLINK_Z_AXIS_SET_SPEED_ERR = value; } }

		private string _M_ADLINK_MOVE_ERR = "ADLINK Motion Move Error";
		public string M_ADLINK_MOVE_ERR { get { return this._M_ADLINK_MOVE_ERR; } set { this._M_ADLINK_MOVE_ERR = value; } }

		private string _M_ELMO_MOVE_ERR = "ELMO Motion Move Error";
		public string M_ELMO_MOVE_ERR { get { return this._M_ELMO_MOVE_ERR; } set { this._M_ELMO_MOVE_ERR = value; } }

		private string _M_ACS_DISCONNECT_FAILED = "ACS Motion Disconnected Failed";
		public string M_ACS_DISCONNECT_FAILED { get { return this._M_ACS_DISCONNECT_FAILED; } set { this._M_ACS_DISCONNECT_FAILED = value; } }

		private string _M_ADLINK_DISCONNECT_FAILED = "ACS Motion Disconnected Failed";
		public string M_ADLINK_DISCONNECT_FAILED { get { return this._M_ADLINK_DISCONNECT_FAILED; } set { this._M_ADLINK_DISCONNECT_FAILED = value; } }

		private string _M_ELMO_DISCONNECT_FAILED = "ELMO Motion Disconnected Failed";
		public string M_ELMO_DISCONNECT_FAILED { get { return this._M_ELMO_DISCONNECT_FAILED; } set { this._M_ELMO_DISCONNECT_FAILED = value; } }

		private string _M_PMAC_DISCONNECT_FAILED = "PMAC Motion Disconnected Failed";
		public string M_PMAC_DISCONNECT_FAILED { get { return this._M_PMAC_DISCONNECT_FAILED; } set { this._M_PMAC_DISCONNECT_FAILED = value; } }

		private string _M_ARCUS_DISCONNECT_FAILED = "ARCUS Motion Disconnected Failed";
		public string M_ARCUS_DISCONNECT_FAILED { get { return this._M_ARCUS_DISCONNECT_FAILED; } set { this._M_ARCUS_DISCONNECT_FAILED = value; } }

		private string _M_CAR_VACUUM_CHECK_ERR = "Carriage Vacuum Check Error";
		public string M_CAR_VACUUM_CHECK_ERR { get { return this._M_CAR_VACUUM_CHECK_ERR; } set { this._M_CAR_VACUUM_CHECK_ERR = value; } }

		private string _IO_DOUBLE_SUB_DET_ERR = "Double substrate detection Error";
		public string IO_DOUBLE_SUB_DET_ERR { get { return this._IO_DOUBLE_SUB_DET_ERR; } set { this._IO_DOUBLE_SUB_DET_ERR = value; } }

		//MES related events
		private string _MES_CONNECTION_FAILED = "Connection to MES system failed.";
		public string MES_CONNECTION_FAILED { get { return this._MES_CONNECTION_FAILED; } set { this._MES_CONNECTION_FAILED = value; } }

		private string _MES_DEVICE_IP_ERR = "Device IP error";
		public string MES_DEVICE_IP_ERR { get { return this._MES_DEVICE_IP_ERR; } set { this._MES_DEVICE_IP_ERR = value; } }

		private string _MES_WORKORDER_CANNOT_BE_PRODUCED_ERR = "Workorder cannot be produced on machine";
		public string MES_WORKORDER_CANNOT_BE_PRODUCED_ERR { get { return this._MES_WORKORDER_CANNOT_BE_PRODUCED_ERR; } set { this._MES_WORKORDER_CANNOT_BE_PRODUCED_ERR = value; } }

		private string _MES_ABNORMAL_NETWORK_OR_MES_ERR = "Network or MES system error";
		public string MES_ABNORMAL_NETWORK_OR_MES_ERR { get { return this._MES_ABNORMAL_NETWORK_OR_MES_ERR; } set { this._MES_ABNORMAL_NETWORK_OR_MES_ERR = value; } }

		private string _MES_CHECKOUT_ERR = "Workorder checkout failed.";
		public string MES_CHECKOUT_ERR { get { return this._MES_CHECKOUT_ERR; } set { this._MES_CHECKOUT_ERR = value; } }

		private string _MES_PART_NUMBER_INVALID = "Part Number is Invalid.";
		public string MES_PART_NUMBER_INVALID { get { return this._MES_PART_NUMBER_INVALID; } set { this._MES_PART_NUMBER_INVALID = value; } }

		private string _DATABASE_PART_NUMBER_NOT_EXIST = "Part Number does not exist in database.";
		public string DATABASE_PART_NUMBER_NOT_EXIST { get { return this._DATABASE_PART_NUMBER_NOT_EXIST; } set { this._DATABASE_PART_NUMBER_NOT_EXIST = value; } }

		private string _PRO_FT_REMEASUREMENT_NG_LIMIT_REACHED = "FT remeasurement NG limit reached.";
		public string PRO_FT_REMEASUREMENT_NG_LIMIT_REACHED { get { return this._PRO_FT_REMEASUREMENT_NG_LIMIT_REACHED; } set { this._PRO_FT_REMEASUREMENT_NG_LIMIT_REACHED = value; } }

		private string _PRO_FT_REMEASUREMENT_FILE_DOES_NOT_EXIST = "FT remeasurement rules file does not exist.";
		public string PRO_FT_REMEASUREMENT_FILE_DOES_NOT_EXIST { get { return this._PRO_FT_REMEASUREMENT_FILE_DOES_NOT_EXIST; } set { this._PRO_FT_REMEASUREMENT_FILE_DOES_NOT_EXIST = value; } }
		private string _PRO_PRODUCTION_OPTION_FILE_DOES_NOT_EXIST = "Production Option file does not exist.";
		public string PRO_PRODUCTION_OPTION_FILE_DOES_NOT_EXIST { get { return this._PRO_PRODUCTION_OPTION_FILE_DOES_NOT_EXIST; } set { this._PRO_PRODUCTION_OPTION_FILE_DOES_NOT_EXIST = value; } }
		private string _PRO_PRODUCTION_OPTION_ENTRY_DOES_NOT_EXIST = "Production Option for Resistor/Tolerance/Size does not exist.";
		public string PRO_PRODUCTION_OPTION_ENTRY_DOES_NOT_EXIST { get { return this._PRO_PRODUCTION_OPTION_ENTRY_DOES_NOT_EXIST; } set { this._PRO_PRODUCTION_OPTION_ENTRY_DOES_NOT_EXIST = value; } }
		//Staticstics Graph Related Error
		private string _S_UPDATE_PT_SETTING_DATA = "PT Graph Data Update Failed";
		public string S_UPDATE_PT_SETTING_DATA { get { return this._S_UPDATE_PT_SETTING_DATA; } set { this._S_UPDATE_PT_SETTING_DATA = value; } }

		private string _S_UPDATE_FT_SETTING_DATA = "FT Graph Data Update Failed";
		public string S_UPDATE_FT_SETTING_DATA { get { return this._S_UPDATE_FT_SETTING_DATA; } set { this._S_UPDATE_FT_SETTING_DATA = value; } }

		private string _S_GRAPH_CONFIG_LOAD = "Load Graph Config Data Failed";
		public string S_GRAPH_CONFIG_LOAD { get { return this._S_GRAPH_CONFIG_LOAD; } set { this._S_GRAPH_CONFIG_LOAD = value; } }

		private string _S_GRAPH_DATA_INIT_ERR = "Load Graph Config Data Failed";
		public string S_GRAPH_DATA_INIT_ERR { get { return this._S_GRAPH_DATA_INIT_ERR; } set { this._S_GRAPH_DATA_INIT_ERR = value; } }

		private string _S_GRAPH_DATA_RESET_ERROR = "Graph Data Reset Error";
		public string S_GRAPH_DATA_RESET_ERROR { get { return this._S_GRAPH_DATA_RESET_ERROR; } set { this._S_GRAPH_DATA_RESET_ERROR = value; } }

		private string _S_GRAPH_DATA_SEMI_AUTO_RESET_ERROR = "Semi-Auto Graph Reset Error";
		public string S_GRAPH_DATA_SEMI_AUTO_RESET_ERROR { get { return this._S_GRAPH_DATA_SEMI_AUTO_RESET_ERROR; } set { this._S_GRAPH_DATA_SEMI_AUTO_RESET_ERROR = value; } }

		private string _S_DATA_LIMIT_REACH = "Data Limit Reach";
		public string S_DATA_LIMIT_REACH { get { return this._S_DATA_LIMIT_REACH; } set { this._S_DATA_LIMIT_REACH = value; } }

		private string _V_CAM_INIT_FAILED = "Camera Initialization Failed";
		public string V_CAM_INIT_FAILED { get { return this._V_CAM_INIT_FAILED; } set { this._V_CAM_INIT_FAILED = value; } }

		private string _V_CAM_SN_NOT_FOUND = "Camera Serial Number Not Found";
		public string V_CAM_SN_NOT_FOUND { get { return this._V_CAM_SN_NOT_FOUND; } set { this._V_CAM_SN_NOT_FOUND = value; } }

		private string _V_CAM_NOT_CONNECTED = "Camera Not Connected";
		public string V_CAM_NOT_CONNECTED { get { return this._V_CAM_NOT_CONNECTED; } set { this._V_CAM_NOT_CONNECTED = value; } }

		private string _V_CONNECT_TO_CAM_FAILED = "Connection To Camera Failed";
		public string V_CONNECT_TO_CAM_FAILED { get { return this._V_CONNECT_TO_CAM_FAILED; } set { this._V_CONNECT_TO_CAM_FAILED = value; } }

		private string _V_PR_IMG_GRABBING_FAILED = "PR Image Grabbing Failed";
		public string V_PR_IMG_GRABBING_FAILED { get { return this._V_PR_IMG_GRABBING_FAILED; } set { this._V_PR_IMG_GRABBING_FAILED = value; } }

		private string _V_PICOLO_CARD_INIT_FAILED = "BP Vision card Init Failed";
		public string V_PICOLO_CARD_INIT_FAILED { get { return this._V_PICOLO_CARD_INIT_FAILED; } set { this._V_PICOLO_CARD_INIT_FAILED = value; } }

		private string _V_DOMINO_CARD_INIT_FAILED = "PR Vision card Init Failed";
		public string V_DOMINO_CARD_INIT_FAILED { get { return this._V_DOMINO_CARD_INIT_FAILED; } set { this._V_DOMINO_CARD_INIT_FAILED = value; } }

		private string _V_BP_INIT_FAILED = "BP Camera Init Failed";
		public string V_BP_INIT_FAILED { get { return this._V_BP_INIT_FAILED; } set { this._V_BP_INIT_FAILED = value; } }

		private string _V_PR_FIND_FAILED = "PR Pattern Finding Failed";
		public string V_PR_FIND_FAILED { get { return this._V_PR_FIND_FAILED; } set { this._V_PR_FIND_FAILED = value; } }

		private string _V_PR_MODEL_NOT_EXIST = "PR Model does not exist";
		public string V_PR_MODEL_NOT_EXIST { get { return this._V_PR_MODEL_NOT_EXIST; } set { this._V_PR_MODEL_NOT_EXIST = value; } }

		private string _V_PR_MODEL_LOADING_FAIL = "PR Model load Failed";
		public string V_PR_MODEL_LOADING_FAIL { get { return this._V_PR_MODEL_LOADING_FAIL; } set { this._V_PR_MODEL_LOADING_FAIL = value; } }

		private string _V_PR_THETA_CAL_FAILED = "PR Theta Calculation Failed";
		public string V_PR_THETA_CAL_FAILED { get { return this._V_PR_THETA_CAL_FAILED; } set { this._V_PR_THETA_CAL_FAILED = value; } }

		private string _V_PR_THETA_OFFSET_LIMIT = "PR Theta Offset reach limit";
		public string V_PR_THETA_OFFSET_LIMIT { get { return this._V_PR_THETA_OFFSET_LIMIT; } set { this._V_PR_THETA_OFFSET_LIMIT = value; } }

		private string _V_PR_POS_OFFSET_LIMIT = "PR Position Offset reach limit";
		public string V_PR_POS_OFFSET_LIMIT { get { return this._V_PR_POS_OFFSET_LIMIT; } set { this._V_PR_POS_OFFSET_LIMIT = value; } }

		private string _V_PR_PROCESS_FAILED = "PR finding Process Failed";
		public string V_PR_PROCESS_FAILED { get { return this._V_PR_PROCESS_FAILED; } set { this._V_PR_PROCESS_FAILED = value; } }

		private string _V_PR_FINAL_PR_PROCESS_FAILED = "Final PR Vision Checking Failed";
		public string V_PR_FINAL_PR_PROCESS_FAILED { get { return this._V_PR_FINAL_PR_PROCESS_FAILED; } set { this._V_PR_FINAL_PR_PROCESS_FAILED = value; } }

		private string _V_PR_LEARN_FAIL = "PR Learnning Processed Failed";
		public string V_PR_LEARN_FAIL { get { return this._V_PR_LEARN_FAIL; } set { this._V_PR_LEARN_FAIL = value; } }

		private string _V_PR_SAVE_FAIL = "PR Save Failed";
		public string V_PR_SAVE_FAIL { get { return this._V_PR_SAVE_FAIL; } set { this._V_PR_SAVE_FAIL = value; } }

		private string _V_PR_SCORE_LEVEL_IS_LOW = "PR Score Level is Very Low";
		public string V_PR_SCORE_LEVEL_IS_LOW { get { return this._V_PR_SCORE_LEVEL_IS_LOW; } set { this._V_PR_SCORE_LEVEL_IS_LOW = value; } }

		private string _V_PR_INIT_FAILED = "PR Camera Init Failed";
		public string V_PR_INIT_FAILED { get { return this._V_PR_INIT_FAILED; } set { this._V_PR_INIT_FAILED = value; } }

		private string _V_PR_INVALID_LEARN_ROI = "Learn ROI position/size is invalid";
		public string V_PR_INVALID_LEARN_ROI { get { return this._V_PR_INVALID_LEARN_ROI; } set { this._V_PR_INVALID_LEARN_ROI = value; } }

		private string _V_GRAB_STOP = "Camera Stop Grabbing";
		public string V_GRAB_STOP { get { return this._V_GRAB_STOP; } set { this._V_GRAB_STOP = value; } }

		private string _V_GRAB_START = "Camera Start Grabbing";
		public string V_GRAB_START { get { return this._V_GRAB_START; } set { this._V_GRAB_START = value; } }

		private string _V_PR_CAM_GET_REF_1_ERR = "PR Calibration Get Ref 1 Pos Error";
		public string V_PR_CAM_GET_REF_1_ERR { get { return this._V_PR_CAM_GET_REF_1_ERR; } set { this._V_PR_CAM_GET_REF_1_ERR = value; } }

		private string _V_PR_CAM_GET_REF_2_ERR = "PR Calibration Get Ref 2 Pos Error";
		public string V_PR_CAM_GET_REF_2_ERR { get { return this._V_PR_CAM_GET_REF_2_ERR; } set { this._V_PR_CAM_GET_REF_2_ERR = value; } }

		private string _V_PR_CAM_CAL_ERR = "PR Calibration Calculation Error";
		public string V_PR_CAM_CAL_ERR { get { return this._V_PR_CAM_CAL_ERR; } set { this._V_PR_CAM_CAL_ERR = value; } }

		private string _V_DEFAULT_IMAGE_LOAD_FAILED = "Default Image Load Failed";
		public string V_DEFAULT_IMAGE_LOAD_FAILED { get { return this._V_DEFAULT_IMAGE_LOAD_FAILED; } set { this._V_DEFAULT_IMAGE_LOAD_FAILED = value; } }

		private string _SYS_LASER_CAL_FILE_LOAD = "Laser Calibration File Load Failed";
		public string SYS_LASER_CAL_FILE_LOAD { get { return this._SYS_LASER_CAL_FILE_LOAD; } set { this._SYS_LASER_CAL_FILE_LOAD = value; } }

		private string _SYS_LASER_CAL_FILE_SAVE = "Laser Calibration File Save Failed";
		public string SYS_LASER_CAL_FILE_SAVE { get { return this._SYS_LASER_CAL_FILE_SAVE; } set { this._SYS_LASER_CAL_FILE_SAVE = value; } }

		private string _SYS_CROSSHAIR_SETT_SAVE_ERR = "Corsshair Setting Save failed";
		public string SYS_CROSSHAIR_SETT_SAVE_ERR { get { return this._SYS_CROSSHAIR_SETT_SAVE_ERR; } set { this._SYS_CROSSHAIR_SETT_SAVE_ERR = value; } }

		private string _SYS_SWITCH_CAMERA = "PB-PR Camera Switch Error";
		public string SYS_SWITCH_CAMERA { get { return this._SYS_SWITCH_CAMERA; } set { this._SYS_SWITCH_CAMERA = value; } }

		private string _SYS_RECIPE_SAVE_ERROR = "Recipe File Save Error";
		public string SYS_RECIPE_SAVE_ERROR { get { return this._SYS_RECIPE_SAVE_ERROR; } set { this._SYS_RECIPE_SAVE_ERROR = value; } }

		private string _SYS_RECIPE_LOAD_ERROR = "Recipe File load Error..";
		public string SYS_RECIPE_LOAD_ERROR { get { return this._SYS_RECIPE_LOAD_ERROR; } set { this._SYS_RECIPE_LOAD_ERROR = value; } }

		private string _SYS_DIODE_HOUR_READ_ERROR = "Diode Hours Reading Failed";
		public string SYS_DIODE_HOUR_READ_ERROR { get { return this._SYS_DIODE_HOUR_READ_ERROR; } set { this._SYS_DIODE_HOUR_READ_ERROR = value; } }

		private string _SYS_DIODE_CURRENT_READ_ERROR = "Diode Current Reading Failed";
		public string SYS_DIODE_CURRENT_READ_ERROR { get { return this._SYS_DIODE_CURRENT_READ_ERROR; } set { this._SYS_DIODE_CURRENT_READ_ERROR = value; } }

		private string _SYS_LASER_STATUS_READ_FAILED = "Laser Status Reading Failed";
		public string SYS_LASER_STATUS_READ_FAILED { get { return this._SYS_LASER_STATUS_READ_FAILED; } set { this._SYS_LASER_STATUS_READ_FAILED = value; } }

		private string _SYS_LASER_INFO_READ_FAILED = "Laser Info Reading Failed";
		public string SYS_LASER_INFO_READ_FAILED { get { return this._SYS_LASER_INFO_READ_FAILED; } set { this._SYS_LASER_INFO_READ_FAILED = value; } }

		private string _SYS_INIT_LASER_ERR = "Laser Initialization Failed";
		public string SYS_INIT_LASER_ERR { get { return this._SYS_INIT_LASER_ERR; } set { this._SYS_INIT_LASER_ERR = value; } }

		private string _SYS_UPDATE_BARCODE_DATA_ERR = "Update Barcode Data Failed";
		public string SYS_UPDATE_BARCODE_DATA_ERR { get { return this._SYS_UPDATE_BARCODE_DATA_ERR; } set { this._SYS_UPDATE_BARCODE_DATA_ERR = value; } }

		private string _SYS_INIT_MAIN_MANAGER_ERR = "Init Main Manager Error";
		public string SYS_INIT_MAIN_MANAGER_ERR { get { return this._SYS_INIT_MAIN_MANAGER_ERR; } set { this._SYS_INIT_MAIN_MANAGER_ERR = value; } }

		private string _SYS_READ_SYS_INFO_FAILED = "Read System Info Failed";
		public string SYS_READ_SYS_INFO_FAILED { get { return this._SYS_READ_SYS_INFO_FAILED; } set { this._SYS_READ_SYS_INFO_FAILED = value; } }

		private string _SYS_READ_GALVO_CAL_FILE_FAILED = "Read Galvo Calibration File Failed";
		public string SYS_READ_GALVO_CAL_FILE_FAILED { get { return this._SYS_READ_GALVO_CAL_FILE_FAILED; } set { this._SYS_READ_GALVO_CAL_FILE_FAILED = value; } }

		private string _SYS_GALVO_CAL_PROCESS_FAILED = "Read Calibration Process Failed";
		public string SYS_GALVO_CAL_PROCESS_FAILED { get { return this._SYS_GALVO_CAL_PROCESS_FAILED; } set { this._SYS_GALVO_CAL_PROCESS_FAILED = value; } }

		private string _SYS_GALVO_CAL_DATA_RESET_FAILED = "Read Calibration Data Reset Failed";
		public string SYS_GALVO_CAL_DATA_RESET_FAILED { get { return this._SYS_GALVO_CAL_DATA_RESET_FAILED; } set { this._SYS_GALVO_CAL_DATA_RESET_FAILED = value; } }

		private string _SYS_READ_LANGUAGE_FILE_FAILED = "Read Language File Failed";
		public string SYS_READ_LANGUAGE_FILE_FAILED { get { return this._SYS_READ_LANGUAGE_FILE_FAILED; } set { this._SYS_READ_LANGUAGE_FILE_FAILED = value; } }

		private string _SYS_CHK_SYS_FOLDER_FAILED = "Check System Folders Failed";
		public string SYS_CHK_SYS_FOLDER_FAILED { get { return this._SYS_CHK_SYS_FOLDER_FAILED; } set { this._SYS_CHK_SYS_FOLDER_FAILED = value; } }

		private string _SYS_CHK_SYS_LOG_FOLDER_FAILED = "Check System Log Folders Failed";
		public string SYS_CHK_SYS_LOG_FOLDER_FAILED { get { return this._SYS_CHK_SYS_LOG_FOLDER_FAILED; } set { this._SYS_CHK_SYS_LOG_FOLDER_FAILED = value; } }
	}

	public class WarningMsgMap
	{
		private string _START_AUTO_MODE = "Do You Want To Start Auto Mode?";
		public string START_AUTO_MODE { get { return this._START_AUTO_MODE; } set { this._START_AUTO_MODE = value; } }

		private string _SAVE_RECIPE_AFTER_CHANGES = "Setup Recipe File has not been saved yet after modification. Save Recipe File?";
		public string SAVE_RECIPE_AFTER_CHANGES { get { return this._SAVE_RECIPE_AFTER_CHANGES; } set { this._SAVE_RECIPE_AFTER_CHANGES = value; } }

		private string _SUBSTRATE_CHECKING_BYPASSED_MOVE_TO_TRIMMING_AREA = "Substrate Checking has been Bypass, Do you want to move carriage Trim Area?";
		public string SUBSTRATE_CHECKING_BYPASSED_MOVE_TO_TRIMMING_AREA { get { return this._SUBSTRATE_CHECKING_BYPASSED_MOVE_TO_TRIMMING_AREA; } set { this._SUBSTRATE_CHECKING_BYPASSED_MOVE_TO_TRIMMING_AREA = value; } }

		private string _TRIMCUT_NO_SETTING_MORE_THAN_TRIMTYPE_ALLOWED = "Trim Cuts Number Setting is more than TrimType Allowed";
		public string TRIMCUT_NO_SETTING_MORE_THAN_TRIMTYPE_ALLOWED { get { return this._TRIMCUT_NO_SETTING_MORE_THAN_TRIMTYPE_ALLOWED; } set { this._TRIMCUT_NO_SETTING_MORE_THAN_TRIMTYPE_ALLOWED = value; } }

		private string _MULTIPLE_CUT_FEATURE_DISABLED = "Multiple Cut Feature is Disable, Pls Contact to Machine Vendor for more Information";
		public string MULTIPLE_CUT_FEATURE_DISABLED { get { return this._MULTIPLE_CUT_FEATURE_DISABLED; } set { this._MULTIPLE_CUT_FEATURE_DISABLED = value; } }

		private string _GALVO_POS_ADJ_NOT_ALLOWED_AT_GROUP = "Galvo Position Adjustment Not Allow At Group ";
		public string GALVO_POS_ADJ_NOT_ALLOWED_AT_GROUP { get { return this._GALVO_POS_ADJ_NOT_ALLOWED_AT_GROUP; } set { this._GALVO_POS_ADJ_NOT_ALLOWED_AT_GROUP = value; } }

		private string _CHANGING_TO_2T_LOSE_4T_DATA = "Changing to 2T will lost 4T channel data! Change to 2T?";
		public string CHANGING_TO_2T_LOSE_4T_DATA { get { return this._CHANGING_TO_2T_LOSE_4T_DATA; } set { this._CHANGING_TO_2T_LOSE_4T_DATA = value; } }

		private string _SYSTEM_RESET_CALIBRATION_DATA = "System will reset Calibration Data. Clear Offset?";
		public string SYSTEM_RESET_CALIBRATION_DATA { get { return this._SYSTEM_RESET_CALIBRATION_DATA; } set { this._SYSTEM_RESET_CALIBRATION_DATA = value; } }

		private string _UPDATING_GEOMETRY_RESET_OFFSET_CALIBRATION_DATA = "Updating Geometry will reset offset calibration data. Update Geometry with new values?";
		public string UPDATING_GEOMETRY_RESET_OFFSET_CALIBRATION_DATA { get { return this._UPDATING_GEOMETRY_RESET_OFFSET_CALIBRATION_DATA; } set { this._UPDATING_GEOMETRY_RESET_OFFSET_CALIBRATION_DATA = value; } }

		private string _RELOAD_RECIPE_DISCARD_UNSAVED_DATA = "Reloading recipe will discard unsave data. Want to reload recipe anyway?";
		public string RELOAD_RECIPE_DISCARD_UNSAVED_DATA { get { return this._RELOAD_RECIPE_DISCARD_UNSAVED_DATA; } set { this._RELOAD_RECIPE_DISCARD_UNSAVED_DATA = value; } }

		private string _UPDATING_Y_SPACING_RESET_OFFSET_RESISTOR_CALIBRATION_POS_DATA = "Updating Y-Spacing will reset offset Resistor Calibration Position data. Update Y-Spacing with new values?";
		public string UPDATING_Y_SPACING_RESET_OFFSET_RESISTOR_CALIBRATION_POS_DATA { get { return this._UPDATING_Y_SPACING_RESET_OFFSET_RESISTOR_CALIBRATION_POS_DATA; } set { this._UPDATING_Y_SPACING_RESET_OFFSET_RESISTOR_CALIBRATION_POS_DATA = value; } }

		private string _SYSTEM_RECALCULATE_SPACING_CARRIAGE_XY_POS_COLUMNS = "System will recalculate spacing value Carriage X, Y postions for Columns. Calculate new column spacing?";
		public string SYSTEM_RECALCULATE_SPACING_CARRIAGE_XY_POS_COLUMNS { get { return this._SYSTEM_RECALCULATE_SPACING_CARRIAGE_XY_POS_COLUMNS; } set { this._SYSTEM_RECALCULATE_SPACING_CARRIAGE_XY_POS_COLUMNS = value; } }

		private string _CLEAR_SUBS_IN_NG_TRAY = "Clear Sub in NG tray. Make sure all the sub is clear. Software will reset NG count.";
		public string CLEAR_SUBS_IN_NG_TRAY { get { return this._CLEAR_SUBS_IN_NG_TRAY; } set { this._CLEAR_SUBS_IN_NG_TRAY = value; } }

		private string _RESET_CLAMP_BARING_USAGE = "Reset Clamp Baring Usage Counts?";
		public string RESET_CLAMP_BARING_USAGE { get { return this._RESET_CLAMP_BARING_USAGE; } set { this._RESET_CLAMP_BARING_USAGE = value; } }

		private string _CLAMP_BARING_USAGE_RESET_COMPLETE = "Clamp Baring Usage Counts Reset Completed";
		public string CLAMP_BARING_USAGE_RESET_COMPLETE { get { return this._CLAMP_BARING_USAGE_RESET_COMPLETE; } set { this._CLAMP_BARING_USAGE_RESET_COMPLETE = value; } }

		private string _PRO_FT_REMEASUREMENT_NG = "FT remeasurement NG.";
		public string PRO_FT_REMEASUREMENT_NG { get { return this._PRO_FT_REMEASUREMENT_NG; } set { this._PRO_FT_REMEASUREMENT_NG = value; } }

		private string _RESET_NEW_VACUUM_CAP_USAGE = "Reset New Vacuum Cap Usage Counts?";
		public string RESET_NEW_VACUUM_CAP_USAGE { get { return this._RESET_NEW_VACUUM_CAP_USAGE; } set { this._RESET_NEW_VACUUM_CAP_USAGE = value; } }

		private string _NEW_VACUUM_CAP_USAGE_RESET_COMPLETE = "New Vacuum Cap Usage Counts Reset Completed";
		public string NEW_VACUUM_CAP_USAGE_RESET_COMPLETE { get { return this._NEW_VACUUM_CAP_USAGE_RESET_COMPLETE; } set { this._NEW_VACUUM_CAP_USAGE_RESET_COMPLETE = value; } }

		private string _RESET_CUT_VACUUM_CAP_USAGE = "Reset Cut Vacuum Cap Usage Counts?";
		public string RESET_CUT_VACUUM_CAP_USAGE { get { return this._RESET_CUT_VACUUM_CAP_USAGE; } set { this._RESET_CUT_VACUUM_CAP_USAGE = value; } }

		private string _CUT_VACUUM_CAP_USAGE_RESET_COMPLETE = "Cut Vacuum Cap Usage Counts Reset Completed";
		public string CUT_VACUUM_CAP_USAGE_RESET_COMPLETE { get { return this._CUT_VACUUM_CAP_USAGE_RESET_COMPLETE; } set { this._CUT_VACUUM_CAP_USAGE_RESET_COMPLETE = value; } }



		private string _REMOVE_INTERLEAVE = "Remove Interleave ";
		public string REMOVE_INTERLEAVE { get { return this._REMOVE_INTERLEAVE; } set { this._REMOVE_INTERLEAVE = value; } }

		private string _RECALCULATE_SPACING_CARRIAGE_XY_FOR_COLUMNS = "System will recalculate spacing value Carriage X, Y postions for Columns. Calculate new column spacing?";
		public string RECALCULATE_SPACING_CARRIAGE_XY_FOR_COLUMNS { get { return this._RECALCULATE_SPACING_CARRIAGE_XY_FOR_COLUMNS; } set { this._RECALCULATE_SPACING_CARRIAGE_XY_FOR_COLUMNS = value; } }

		private string _RECALCULATE_SPACING_CARRIAGE_XY_FOR_ROWS = "System will recalculate spacing value Carriage X, Y positions for row. Calculate new row spacing?";
		public string RECALCULATE_SPACING_CARRIAGE_XY_FOR_ROWS { get { return this._RECALCULATE_SPACING_CARRIAGE_XY_FOR_ROWS; } set { this._RECALCULATE_SPACING_CARRIAGE_XY_FOR_ROWS = value; } }

		private string _TO_DO_CON_TEST = "Click on Connector Button to upate each Connection channel.";
		public string TO_DO_CON_TEST { get { return this._TO_DO_CON_TEST; } set { this._TO_DO_CON_TEST = value; } }

		private string _PASSWORD_TRY_AGAIN = "Try Again";
		public string PASSWORD_TRY_AGAIN { get { return this._PASSWORD_TRY_AGAIN; } set { this._PASSWORD_TRY_AGAIN = value; } }

		private string _PASSWORD_TRY_LIMIT = "Try Limit";
		public string PASSWORD_TRY_LIMIT { get { return this._PASSWORD_TRY_LIMIT; } set { this._PASSWORD_TRY_LIMIT = value; } }

		private string _PASSWORD_WRONG_PASSWORD = "Wrong Old Password";
		public string PASSWORD_WRONG_PASSWORD { get { return this._PASSWORD_WRONG_PASSWORD; } set { this._PASSWORD_WRONG_PASSWORD = value; } }

		private string _PASSWORD_TOO_SHORT = "Too short, at least 3 Char";
		public string PASSWORD_TOO_SHORT { get { return this._PASSWORD_TOO_SHORT; } set { this._PASSWORD_TOO_SHORT = value; } }

		private string _PASSWORD_COMFIRM_NOT_MATCH = "New passwrod not match";
		public string PASSWORD_COMFIRM_NOT_MATCH { get { return this._PASSWORD_COMFIRM_NOT_MATCH; } set { this._PASSWORD_COMFIRM_NOT_MATCH = value; } }

		private string _V_PR_CALIBRATION_FACTOR_SAVING = "Calibration Factor Saving.";
		public string V_PR_CALIBRATION_FACTOR_SAVING { get { return this._V_PR_CALIBRATION_FACTOR_SAVING; } set { this._V_PR_CALIBRATION_FACTOR_SAVING = value; } }

		private string _V_BP_CROSSHAIR_CALIBRATION_CLEARED = "Crosshair calibration cleared.";
		public string V_BP_CROSSHAIR_CALIBRATION_CLEARED { get { return this._V_BP_CROSSHAIR_CALIBRATION_CLEARED; } set { this._V_BP_CROSSHAIR_CALIBRATION_CLEARED = value; } }

		private string _V_PR_MASTER_LEARNING_FAILED = "Learning Process Failed.\rMake sure license dongle is present and PR camera is working properly.";
		public string V_PR_MASTER_LEARNING_FAILED { get { return this._V_PR_MASTER_LEARNING_FAILED; } set { this._V_PR_MASTER_LEARNING_FAILED = value; } }

		private string _V_PR_MASTER_SAVING_FAILED = "Saving PR-Master Process Failed.\rMake sure license dongle is present and PR camera is working properly.";
		public string V_PR_MASTER_SAVING_FAILED { get { return this._V_PR_MASTER_SAVING_FAILED; } set { this._V_PR_MASTER_SAVING_FAILED = value; } }

		private string _M_CARR_DOUBLE_PART_CHECK_POS_NOT_GOOD = "Current Position is not suitable for Double Part Check";
		public string M_CARR_DOUBLE_PART_CHECK_POS_NOT_GOOD { get { return this._M_CARR_DOUBLE_PART_CHECK_POS_NOT_GOOD; } set { this._M_CARR_DOUBLE_PART_CHECK_POS_NOT_GOOD = value; } }

		private string _M_CARR_LOADING_POS_NOT_GOOD = "Current Position is not suitable for Loading/Unloading";
		public string M_CARR_LOADING_POS_NOT_GOOD { get { return this._M_CARR_LOADING_POS_NOT_GOOD; } set { this._M_CARR_LOADING_POS_NOT_GOOD = value; } }

		private string _M_CARR_CLEANING_POS_NOT_GOOD = "Current Position is not suitable for Probing Cleaning";
		public string M_CARR_CLEANING_POS_NOT_GOOD { get { return this._M_CARR_CLEANING_POS_NOT_GOOD; } set { this._M_CARR_CLEANING_POS_NOT_GOOD = value; } }

		private string _M_CARR_PR_POS_NOT_GOOD = "Current Position is not suitable for PR Position";
		public string M_CARR_PR_POS_NOT_GOOD { get { return this._M_CARR_PR_POS_NOT_GOOD; } set { this._M_CARR_PR_POS_NOT_GOOD = value; } }

		private string _M_CARR_GALVO_CAL_POS_NOT_GOOD = "Current Position is not suitable for Galvo Calibration";
		public string M_CARR_GALVO_CAL_POS_NOT_GOOD { get { return this._M_CARR_GALVO_CAL_POS_NOT_GOOD; } set { this._M_CARR_GALVO_CAL_POS_NOT_GOOD = value; } }

		private string _M_CARR_THETA_CENTER_POS_NOT_GOOD = "Current Position is not suitable for Theta Center";
		public string M_CARR_THETA_CENTER_POS_NOT_GOOD { get { return this._M_CARR_THETA_CENTER_POS_NOT_GOOD; } set { this._M_CARR_THETA_CENTER_POS_NOT_GOOD = value; } }

		private string _M_CARR_TRIMMING_POS_NOT_GOOD = "Current Position is not suitable for Trimming";
		public string M_CARR_TRIMMING_POS_NOT_GOOD { get { return this._M_CARR_TRIMMING_POS_NOT_GOOD; } set { this._M_CARR_TRIMMING_POS_NOT_GOOD = value; } }

		private string _M_GALVO_GLOBAL_POS_NOT_GOOD = "Global Galvo Offset is too much, it is not suitable for Trimming \r" + "Adjust Resistor Position First";
		public string M_GALVO_GLOBAL_POS_NOT_GOOD { get { return this._M_GALVO_GLOBAL_POS_NOT_GOOD; } set { this._M_GALVO_GLOBAL_POS_NOT_GOOD = value; } }

		private string _M_CARR_Y_POSITION_IS_NOT_GOOD = "Difference in Y-Position for two position is too much. Adjust Theta First.";
		public string M_CARR_Y_POSITION_IS_NOT_GOOD { get { return this._M_CARR_Y_POSITION_IS_NOT_GOOD; } set { this._M_CARR_Y_POSITION_IS_NOT_GOOD = value; } }

		private string _M_CARR_X_POSITION_IS_NOT_GOOD = "Difference in X-Position for two position is too much. Adjust Theta First.";
		public string M_CARR_X_POSITION_IS_NOT_GOOD { get { return this._M_CARR_X_POSITION_IS_NOT_GOOD; } set { this._M_CARR_X_POSITION_IS_NOT_GOOD = value; } }

		private string _MEAS_NORM_VAL_GREATER_THAN_ULO = "Norminal Value is greater than ULO system allowed.";
		public string MEAS_NORM_VAL_GREATER_THAN_ULO { get { return this._MEAS_NORM_VAL_GREATER_THAN_ULO; } set { this._MEAS_NORM_VAL_GREATER_THAN_ULO = value; } }

		private string _MES_WORK_ORDER_QUERY_NOT_DONE = "Work Order Query Not Done.";
		public string MES_WORK_ORDER_QUERY_NOT_DONE { get { return this._MES_WORK_ORDER_QUERY_NOT_DONE; } set { this._MES_WORK_ORDER_QUERY_NOT_DONE = value; } }

		private string _MES_CONNECTION_TO_SERVER_NOT_DONE = "Connection To MES Server Not Done.";
		public string MES_CONNECTION_TO_SERVER_NOT_DONE { get { return this._MES_CONNECTION_TO_SERVER_NOT_DONE; } set { this._MES_CONNECTION_TO_SERVER_NOT_DONE = value; } }

		private string _MES_WORKORDER_NO_EMPTY = "Workorder Number is Empty.";
		public string MES_WORKORDER_NO_EMPTY { get { return this._MES_WORKORDER_NO_EMPTY; } set { this._MES_WORKORDER_NO_EMPTY = value; } }

		private string _MES_TRIM_PARA_QUERY_NOT_DONE = "Trim Parameter Query Not Done.";
		public string MES_TRIM_PARA_QUERY_NOT_DONE { get { return this._MES_TRIM_PARA_QUERY_NOT_DONE; } set { this._MES_TRIM_PARA_QUERY_NOT_DONE = value; } }

		private string _MES_CHECK_IN_NOT_DONE = "Check-In Not Done.";
		public string MES_CHECK_IN_NOT_DONE { get { return this._MES_CHECK_IN_NOT_DONE; } set { this._MES_CHECK_IN_NOT_DONE = value; } }

		private string _MES_BADGE_NO_EMPTY = "Badge Number is Empty.";
		public string MES_BADGE_NO_EMPTY { get { return this._MES_BADGE_NO_EMPTY; } set { this._MES_BADGE_NO_EMPTY = value; } }

		private string _MES_PROBE_NO_EMPTY = "Probe Number is Empty.";
		public string MES_PROBE_NO_EMPTY { get { return this._MES_PROBE_NO_EMPTY; } set { this._MES_PROBE_NO_EMPTY = value; } }

		private string _MES_EQUIPMENTID_EMPTY = "Equipment ID is Empty.";
		public string MES_EQUIPMENTID_EMPTY { get { return this._MES_EQUIPMENTID_EMPTY; } set { this._MES_EQUIPMENTID_EMPTY = value; } }

		private string _TE_CONTEST_WARNING = "Doing Con Test will lost all unsave trimming related data \rSave the current File Before doing Con-Test" + "\rContinue to do Con-Test??";
		public string TE_CONTEST_WARNING { get { return this._TE_CONTEST_WARNING; } set { this._TE_CONTEST_WARNING = value; } }

		private string _SEQ_NEW_EMPTY = "New Sub Magzine is Empty. Fill Up Substrate and start again";
		public string SEQ_NEW_EMPTY { get { return this._SEQ_NEW_EMPTY; } set { this._SEQ_NEW_EMPTY = value; } }

		private string _SEQ_CUT_FULL = "Cut Sub Magzine is Full. Take Out Substrates and start again";
		public string SEQ_CUT_FULL { get { return this._SEQ_CUT_FULL; } set { this._SEQ_CUT_FULL = value; } }

		private string _SEQ_COL_SD_LIMIT_HIT = "Col SD value hit the limit.\rChange the limit setting or stop production.\rStop Running?";
		public string SEQ_COL_SD_LIMIT_HIT { get { return this._SEQ_COL_SD_LIMIT_HIT; } set { this._SEQ_COL_SD_LIMIT_HIT = value; } }

		private string _SEQ_PART_SENSOR_CHK_ERROR = "Carriage Part Sensor Check Error";
		public string SEQ_PART_SENSOR_CHK_ERROR { get { return this._SEQ_PART_SENSOR_CHK_ERROR; } set { this._SEQ_PART_SENSOR_CHK_ERROR = value; } }

		private string _SEQ_COL_YLD_LIMIT_HIT = "Col Yield value hit the limit.\rChange the limit setting or stop production.\rStop Running?";
		public string SEQ_COL_YLD_LIMIT_HIT { get { return this._SEQ_COL_YLD_LIMIT_HIT; } set { this._SEQ_COL_YLD_LIMIT_HIT = value; } }

		private string _SEQ_SUB_SD_LIMIT_HIT = "Substrate SD value hit the limit.\rChange the limit setting or stop production.\rStop Running?";
		public string SEQ_SUB_SD_LIMIT_HIT { get { return this._SEQ_SUB_SD_LIMIT_HIT; } set { this._SEQ_SUB_SD_LIMIT_HIT = value; } }

		private string _SEQ_SUB_YLD_LIMIT_HIT = "Substrate Yield value hit the limit.\rChange the limit setting or stop production.\rStop Running?";
		public string SEQ_SUB_YLD_LIMIT_HIT { get { return this._SEQ_SUB_YLD_LIMIT_HIT; } set { this._SEQ_SUB_YLD_LIMIT_HIT = value; } }

		private string _SEQ_SUB_PR_PROCESS_FAILED = "PR process failed. \rCheck Failure detail on PR Setup page.";
		public string SEQ_SUB_PR_PROCESS_FAILED { get { return this._SEQ_SUB_PR_PROCESS_FAILED; } set { this._SEQ_SUB_PR_PROCESS_FAILED = value; } }

		private string _SEQ_FRONT_SAFETY_DOOR_IS_OPEN = "Front Safety Door is Open! Please Closed the Door to continue Process";
		public string SEQ_FRONT_SAFETY_DOOR_IS_OPEN { get { return this._SEQ_FRONT_SAFETY_DOOR_IS_OPEN; } set { this._SEQ_FRONT_SAFETY_DOOR_IS_OPEN = value; } }

		private string _SEQ_DOOR_IS_OPEN = "Door is Open! Please Closed the Door to continue Process";
		public string SEQ_DOOR_IS_OPEN { get { return this._SEQ_DOOR_IS_OPEN; } set { this._SEQ_DOOR_IS_OPEN = value; } }

		private string _SEQ_SUB_PR_FAILED_LIMIT_HIT = "PR Failure Substrates Count hit the limit.\rCheck Sub, Reset the count and Re-Start Production.";
		public string SEQ_SUB_PR_FAILED_LIMIT_HIT { get { return this._SEQ_SUB_PR_FAILED_LIMIT_HIT; } set { this._SEQ_SUB_PR_FAILED_LIMIT_HIT = value; } }

		private string _SEQ_SUB_IN_REJECT_TRAY_LIMIT_HIT = "Substrates Count in reject tray hit the limit.\rCheck Sub, Clear NG-Tray and Re-Start Production.";
		public string SEQ_SUB_IN_REJECT_TRAY_LIMIT_HIT { get { return this._SEQ_SUB_IN_REJECT_TRAY_LIMIT_HIT; } set { this._SEQ_SUB_IN_REJECT_TRAY_LIMIT_HIT = value; } }

		private string _SEQ_BTH_SD_LIMIT_HIT = "Batch SD value hit the limit.\rChange the limit setting or stop production.\rStop Running?";
		public string SEQ_BTH_SD_LIMIT_HIT { get { return this._SEQ_BTH_SD_LIMIT_HIT; } set { this._SEQ_BTH_SD_LIMIT_HIT = value; } }

		private string _SEQ_BTH_YLD_LIMIT_HIT = "Batch Yield value hit the limit.\rChange the limit setting or stop production.\rStop Running?";
		public string SEQ_BTH_YLD_LIMIT_HIT { get { return this._SEQ_BTH_YLD_LIMIT_HIT; } set { this._SEQ_BTH_YLD_LIMIT_HIT = value; } }

		private string _SYS_CROSSHAIR_DRIFF_CAL_POS = "Current Galvo Y Position is not wide enough/too much to calculate driff factor.\rThe best range is Galvo Y = 20~35";
		public string SYS_CROSSHAIR_DRIFF_CAL_POS { get { return this._SYS_CROSSHAIR_DRIFF_CAL_POS; } set { this._SYS_CROSSHAIR_DRIFF_CAL_POS = value; } }

		private string _SYS_CROSSHAIR_ORI_POS = "Current Galvo Y, X Position is not correct to save origin.\rThe best range is Galvo X = 0, Y= 0";
		public string SYS_CROSSHAIR_ORI_POS { get { return this._SYS_CROSSHAIR_ORI_POS; } set { this._SYS_CROSSHAIR_ORI_POS = value; } }

		private string _SEQ_PRO_SUB_HIT_LIMIT = "Production Substrates Hit Limit Setting for Probe Cleaning Services";
		public string SEQ_PRO_SUB_HIT_LIMIT { get { return this._SEQ_PRO_SUB_HIT_LIMIT; } set { this._SEQ_PRO_SUB_HIT_LIMIT = value; } }

		private string _SEQ_SEMI_AUTO_COMPLETED = "Semi Auto Completed";
		public string SEQ_SEMI_AUTO_COMPLETED { get { return this._SEQ_SEMI_AUTO_COMPLETED; } set { this._SEQ_SEMI_AUTO_COMPLETED = value; } }

		private string _SEQ_PRO_SUB_HIT_LIMIT_2 = "Production Substrates Hit Limit Setting for trim cut checking";
		public string SEQ_PRO_SUB_HIT_LIMIT_2 { get { return this._SEQ_PRO_SUB_HIT_LIMIT_2; } set { this._SEQ_PRO_SUB_HIT_LIMIT_2 = value; } }

		private string _SEQ_PRO_STAT_HIT_LIMIT = "Production Statistics Data Hit Limit";
		public string SEQ_PRO_STAT_HIT_LIMIT { get { return this._SEQ_PRO_STAT_HIT_LIMIT; } set { this._SEQ_PRO_STAT_HIT_LIMIT = value; } }

		private string _SEQ_PRO_FAILD_TO_PICKUP_NEW_SUB = "Failed To Pick Up New Substrate";
		public string SEQ_PRO_FAILD_TO_PICKUP_NEW_SUB { get { return this._SEQ_PRO_FAILD_TO_PICKUP_NEW_SUB; } set { this._SEQ_PRO_FAILD_TO_PICKUP_NEW_SUB = value; } }

		private string _SEQ_PRO_FORCED_STOP = "Production Forced Stop";
		public string SEQ_PRO_FORCED_STOP { get { return this._SEQ_PRO_FORCED_STOP; } set { this._SEQ_PRO_FORCED_STOP = value; } }

		private string _CLAMP_BARING_USAGE_HIT_LIMIT = "Clamper Baring Usage Lifetime Limit Hit";
		public string CLAMP_BARING_USAGE_HIT_LIMIT { get { return this._CLAMP_BARING_USAGE_HIT_LIMIT; } set { this._CLAMP_BARING_USAGE_HIT_LIMIT = value; } }

		private string _NEW_VACUUM_CAP_USAGE_HIT_LIMIT = "New Vacuum Caps Usage Lifetime Limit Hit";
		public string NEW_VACUUM_CAP_USAGE_HIT_LIMIT { get { return this._NEW_VACUUM_CAP_USAGE_HIT_LIMIT; } set { this._NEW_VACUUM_CAP_USAGE_HIT_LIMIT = value; } }

		private string _CUT_VACUUM_CAP_USAGE_HIT_LIMIT = "Cut Vacuum Caps Usage Lifetime Limit Hit";
		public string CUT_VACUUM_CAP_USAGE_HIT_LIMIT { get { return this._CUT_VACUUM_CAP_USAGE_HIT_LIMIT; } set { this._CUT_VACUUM_CAP_USAGE_HIT_LIMIT = value; } }

		private string _MACHINE_AIR_PRESSURE_LOW = "Machine Air Pressure Low";
		public string MACHINE_AIR_PRESSURE_LOW { get { return this._MACHINE_AIR_PRESSURE_LOW; } set { this._MACHINE_AIR_PRESSURE_LOW = value; } }

		private string _DEDUST_PRESSURE_LOW = "Dedust Air Pressure Low";
		public string DEDUST_PRESSURE_LOW { get { return this._DEDUST_PRESSURE_LOW; } set { this._DEDUST_PRESSURE_LOW = value; } }

	}

	public class EventMsgMap
	{
		private string _M_CAR_X_HOME_DONE = "Carriage X Home Done";
		public string M_CAR_X_HOME_DONE { get { return this._M_CAR_X_HOME_DONE; } set { this._M_CAR_X_HOME_DONE = value; } }

		private string _M_CAR_Y_HOME_DONE = "Carriage Y Home Done";
		public string M_CAR_Y_HOME_DONE { get { return this._M_CAR_Y_HOME_DONE; } set { this._M_CAR_Y_HOME_DONE = value; } }

		private string _M_PROBER_Z_HOME_DONE = "Prober Z Home Done";
		public string M_PROBER_Z_HOME_DONE { get { return this._M_PROBER_Z_HOME_DONE; } set { this._M_PROBER_Z_HOME_DONE = value; } }

		private string _M_CARRIAGE_THETA_HOME_DONE = "Carriage THETA Home Done";
		public string M_CARRIAGE_THETA_HOME_DONE { get { return this._M_CARRIAGE_THETA_HOME_DONE; } set { this._M_CARRIAGE_THETA_HOME_DONE = value; } }

		private string _M_CAR_THETA_TRIM_POS_MOVE = "Carriage THETA  to Trim Pos";
		public string M_CAR_THETA_TRIM_POS_MOVE { get { return this._M_CAR_THETA_TRIM_POS_MOVE; } set { this._M_CAR_THETA_TRIM_POS_MOVE = value; } }

		private string _M_CAR_THETA_TRIM_POS_UPDATED = "Carriage THETA  to Trim Pos Updated";
		public string M_CAR_THETA_TRIM_POS_UPDATED { get { return this._M_CAR_THETA_TRIM_POS_UPDATED; } set { this._M_CAR_THETA_TRIM_POS_UPDATED = value; } }

		private string _M_CAR_DOUBLE_PART_CHECK_POS_MOVE = "Carriage Moved to Double Part Check Pos";
		public string M_CAR_DOUBLE_PART_CHECK_POS_MOVE { get { return this._M_CAR_DOUBLE_PART_CHECK_POS_MOVE; } set { this._M_CAR_DOUBLE_PART_CHECK_POS_MOVE = value; } }

		private string _M_CAR_LOAD_POS_MOVE = "Carriage Moved to Load Pos";
		public string M_CAR_LOAD_POS_MOVE { get { return this._M_CAR_LOAD_POS_MOVE; } set { this._M_CAR_LOAD_POS_MOVE = value; } }

		private string _M_CAR_UNLOAD_POS_MOVE = "Carriage Moved to UnLoad Pos";
		public string M_CAR_UNLOAD_POS_MOVE { get { return this._M_CAR_UNLOAD_POS_MOVE; } set { this._M_CAR_UNLOAD_POS_MOVE = value; } }

		private string _M_CAR_REJECT_POS_MOVE = "Carriage Moved to Reject Pos";
		public string M_CAR_REJECT_POS_MOVE { get { return this._M_CAR_REJECT_POS_MOVE; } set { this._M_CAR_REJECT_POS_MOVE = value; } }

		private string _M_CAR_CLEAN_POS_MOVE = "Carriage Moved to Clean Pos";
		public string M_CAR_CLEAN_POS_MOVE { get { return this._M_CAR_CLEAN_POS_MOVE; } set { this._M_CAR_CLEAN_POS_MOVE = value; } }

		private string _M_CAR_PR_1_POS_MOVE = "Carriage Moved to Ref 1 Pos";
		public string M_CAR_PR_1_POS_MOVE { get { return this._M_CAR_PR_1_POS_MOVE; } set { this._M_CAR_PR_1_POS_MOVE = value; } }

		private string _M_CAR_PR_2_POS_MOVE = "Carriage Moved to Ref 2 Pos";
		public string M_CAR_PR_2_POS_MOVE { get { return this._M_CAR_PR_2_POS_MOVE; } set { this._M_CAR_PR_2_POS_MOVE = value; } }

		private string _M_CAR_GALVO_CAL_POS_MOVE = "Carriage Moved to Galvo Calibration Pos";
		public string M_CAR_GALVO_CAL_POS_MOVE { get { return this._M_CAR_GALVO_CAL_POS_MOVE; } set { this._M_CAR_GALVO_CAL_POS_MOVE = value; } }

		private string _M_CAR_FIRST_REF_POS_MOVE = "Carriage Moved to First Ref Pos";
		public string M_CAR_FIRST_REF_POS_MOVE { get { return this._M_CAR_FIRST_REF_POS_MOVE; } set { this._M_CAR_FIRST_REF_POS_MOVE = value; } }

		private string _M_CAR_SECOND_REF_POS_MOVE = "Carriage Moved to Second Ref Pos";
		public string M_CAR_SECOND_REF_POS_MOVE { get { return this._M_CAR_SECOND_REF_POS_MOVE; } set { this._M_CAR_SECOND_REF_POS_MOVE = value; } }

		private string _M_PROBER_Z_MEASURE_MOVE = "Prober Z Moved to Measure Pos";
		public string M_PROBER_Z_MEASURE_MOVE { get { return this._M_PROBER_Z_MEASURE_MOVE; } set { this._M_PROBER_Z_MEASURE_MOVE = value; } }

		private string _M_PROBER_Z_INDEX_MOVE = "Prober Z Moved to Index Pos";
		public string M_PROBER_Z_INDEX_MOVE { get { return this._M_PROBER_Z_INDEX_MOVE; } set { this._M_PROBER_Z_INDEX_MOVE = value; } }

		private string _M_INDEXING_ST = "Indexing Start";
		public string M_INDEXING_ST { get { return this._M_INDEXING_ST; } set { this._M_INDEXING_ST = value; } }

		private string _M_NEW_ELE_GOING_DN = "New Elevator Going Down...";
		public string M_NEW_ELE_GOING_DN { get { return this._M_NEW_ELE_GOING_DN; } set { this._M_NEW_ELE_GOING_DN = value; } }

		private string _M_NEW_ELE_STOP = "New Elevator Stop";
		public string M_NEW_ELE_STOP { get { return this._M_NEW_ELE_STOP; } set { this._M_NEW_ELE_STOP = value; } }

		private string _M_NEW_ELE_GOING_UP = "New Elevator Going Up...";
		public string M_NEW_ELE_GOING_UP { get { return this._M_NEW_ELE_GOING_UP; } set { this._M_NEW_ELE_GOING_UP = value; } }

		private string _M_CUT_ELE_GOING_DN = "Cut Elevator Goin Down...";
		public string M_CUT_ELE_GOING_DN { get { return this._M_CUT_ELE_GOING_DN; } set { this._M_CUT_ELE_GOING_DN = value; } }

		private string _M_CUT_ELE_STOP = "Cut Elevator Stop...";
		public string M_CUT_ELE_STOP { get { return this._M_CUT_ELE_STOP; } set { this._M_CUT_ELE_STOP = value; } }

		private string _M_CUT_ELE_GOING_UP = "Cut Elevator Goin Up...";
		public string M_CUT_ELE_GOING_UP { get { return this._M_CUT_ELE_GOING_UP; } set { this._M_CUT_ELE_GOING_UP = value; } }

		private string _M_CAR_COL_POS_MOVE = "Carriage Moved to Col ";
		public string M_CAR_COL_POS_MOVE { get { return this._M_CAR_COL_POS_MOVE; } set { this._M_CAR_COL_POS_MOVE = value; } }

		private string _M_CAR_ROW_POS_MOVE = "Carriage Moved to Row ";
		public string M_CAR_ROW_POS_MOVE { get { return this._M_CAR_ROW_POS_MOVE; } set { this._M_CAR_ROW_POS_MOVE = value; } }

		private string _M_CAR_COL_ROW_POS_MOVE = "Carriage Moved to Col,Row:";
		public string M_CAR_COL_ROW_POS_MOVE { get { return this._M_CAR_COL_ROW_POS_MOVE; } set { this._M_CAR_COL_ROW_POS_MOVE = value; } }

		private string _M_CAR_COL_POS_UPDATED = "Carriage Col Pos Updated ";
		public string M_CAR_COL_POS_UPDATED { get { return this._M_CAR_COL_POS_UPDATED; } set { this._M_CAR_COL_POS_UPDATED = value; } }

		private string _M_CAR_ROW_POS_UPDATED = "Carriage Row Pos Updated ";
		public string M_CAR_ROW_POS_UPDATED { get { return this._M_CAR_ROW_POS_UPDATED; } set { this._M_CAR_ROW_POS_UPDATED = value; } }

		private string _M_CAR_COL_GALVO_POS_UPDATED = "Carriage Col Galvo Global Pos Updated ";
		public string M_CAR_COL_GALVO_POS_UPDATED { get { return this._M_CAR_COL_GALVO_POS_UPDATED; } set { this._M_CAR_COL_GALVO_POS_UPDATED = value; } }

		private string _M_NO_SUB_ON_NEW_ARM = "No Sub Present On New PNP Arm";
		public string M_NO_SUB_ON_NEW_ARM { get { return this._M_NO_SUB_ON_NEW_ARM; } set { this._M_NO_SUB_ON_NEW_ARM = value; } }

		private string _M_SEQ_DONE = "Sequence Done in ";
		public string M_SEQ_DONE { get { return this._M_SEQ_DONE; } set { this._M_SEQ_DONE = value; } }

		private string _M_SEQ_CENCEL = "Sequence Abort in ";
		public string M_SEQ_CENCEL { get { return this._M_SEQ_CENCEL; } set { this._M_SEQ_CENCEL = value; } }

		private string _M_NEW_ELEVATOR_EMPTY = "New Ele Empty";
		public string M_NEW_ELEVATOR_EMPTY { get { return this._M_NEW_ELEVATOR_EMPTY; } set { this._M_NEW_ELEVATOR_EMPTY = value; } }

		private string _M_COL_POSITION_UPATED = "Column Positions Updated";
		public string M_COL_POSITION_UPATED { get { return this._M_COL_POSITION_UPATED; } set { this._M_COL_POSITION_UPATED = value; } }

		private string _M_COL_ROW_POSITION_UPATED = "Column and Row Positions Updated";
		public string M_COL_ROW_POSITION_UPATED { get { return this._M_COL_ROW_POSITION_UPATED; } set { this._M_COL_ROW_POSITION_UPATED = value; } }

		private string _M_SYSTEM_HOMEING = "System Homing...";
		public string M_SYSTEM_HOMEING { get { return this._M_SYSTEM_HOMEING; } set { this._M_SYSTEM_HOMEING = value; } }

		private string _M_SYSTEM_HOMEING_DONE = "System Homing Complete";
		public string M_SYSTEM_HOMEING_DONE { get { return this._M_SYSTEM_HOMEING_DONE; } set { this._M_SYSTEM_HOMEING_DONE = value; } }

		private string _M_SYSTEM_RECIPE_SAVING = "Recipe File Saving..";
		public string M_SYSTEM_RECIPE_SAVING { get { return this._M_SYSTEM_RECIPE_SAVING; } set { this._M_SYSTEM_RECIPE_SAVING = value; } }

		private string _M_SYSTEM_RECIPE_SAVED = "Recipe File Saved.";
		public string M_SYSTEM_RECIPE_SAVED { get { return this._M_SYSTEM_RECIPE_SAVED; } set { this._M_SYSTEM_RECIPE_SAVED = value; } }

		private string _M_SYSTEM_RECIPE_LOADED = "Recipe File Loaded.";
		public string M_SYSTEM_RECIPE_LOADED { get { return this._M_SYSTEM_RECIPE_LOADED; } set { this._M_SYSTEM_RECIPE_LOADED = value; } }

		private string _M_SYSTEM_RECIPE_LOADING = "Recipe File Loading...";
		public string M_SYSTEM_RECIPE_LOADING { get { return this._M_SYSTEM_RECIPE_LOADING; } set { this._M_SYSTEM_RECIPE_LOADING = value; } }

		private string _M_SAVE_POSITION_CONFIRM = "Save Position Confirm?";
		public string M_SAVE_POSITION_CONFIRM { get { return this._M_SAVE_POSITION_CONFIRM; } set { this._M_SAVE_POSITION_CONFIRM = value; } }

		private string _M_TEACH_PNP_POSITION = "Teach PnP Position";
		public string M_TEACH_PNP_POSITION { get { return this._M_TEACH_PNP_POSITION; } set { this._M_TEACH_PNP_POSITION = value; } }

		private string _M_TEACH_X_AXIS_POSITION = "Teach X-Axis Position";
		public string M_TEACH_X_AXIS_POSITION { get { return this._M_TEACH_X_AXIS_POSITION; } set { this._M_TEACH_X_AXIS_POSITION = value; } }

		private string _M_TEACH_Y_AXIS_POSITION = "Teach Y-Axis Position";
		public string M_TEACH_Y_AXIS_POSITION { get { return this._M_TEACH_Y_AXIS_POSITION; } set { this._M_TEACH_Y_AXIS_POSITION = value; } }

		private string _M_TEACH_Z_AXIS_POSITION = "Teach Z-Axis Position";
		public string M_TEACH_Z_AXIS_POSITION { get { return this._M_TEACH_Z_AXIS_POSITION; } set { this._M_TEACH_Z_AXIS_POSITION = value; } }


		private string _TE_INIT_CON_DATA = "Initializing Con Data.. Please Wait.. ";
		public string TE_INIT_CON_DATA { get { return this._TE_INIT_CON_DATA; } set { this._TE_INIT_CON_DATA = value; } }

		private string _TE_INIT_CON_DATA_DONE = "Con Data Updated ";
		public string TE_INIT_CON_DATA_DONE { get { return this._TE_INIT_CON_DATA_DONE; } set { this._TE_INIT_CON_DATA_DONE = value; } }

		private string _TE_TRIMMING = "Trimming... ";
		public string TE_TRIMMING { get { return this._TE_TRIMMING; } set { this._TE_TRIMMING = value; } }

		private string _TE_MEASURE = "Measuring... ";
		public string TE_MEASURE { get { return this._TE_MEASURE; } set { this._TE_MEASURE = value; } }

		private string _TE_GALVO_RES_POS_MOVE = "Galvo Moved to Res ";
		public string TE_GALVO_RES_POS_MOVE { get { return this._TE_GALVO_RES_POS_MOVE; } set { this._TE_GALVO_RES_POS_MOVE = value; } }

		private string _TE_UPDATED = "Trimming Setting Updated";
		public string TE_UPDATED { get { return this._TE_UPDATED; } set { this._TE_UPDATED = value; } }

		private string _TE_UPDATING_PARAMETERS = "Updating Parameters";
		public string TE_UPDATING_PARAMETERS { get { return this._TE_UPDATING_PARAMETERS; } set { this._TE_UPDATING_PARAMETERS = value; } }

		private string _TE_TRIM_COLUMN_DONE = "Trim Column Done.";
		public string TE_TRIM_COLUMN_DONE { get { return this._TE_TRIM_COLUMN_DONE; } set { this._TE_TRIM_COLUMN_DONE = value; } }

		private string _TE_RES_INFO_DNLD = "Trimming Resistor Info Downloading..., please wait.. ";
		public string TE_RES_INFO_DNLD { get { return this._TE_RES_INFO_DNLD; } set { this._TE_RES_INFO_DNLD = value; } }

		private string _TE_TRIM_RES_DONE = "Trim Resistor Done";
		public string TE_TRIM_RES_DONE { get { return this._TE_TRIM_RES_DONE; } set { this._TE_TRIM_RES_DONE = value; } }

		private string _TE_TRIM_PROBE_CLEAN_DONE = "Trim Probe Pins Done";
		public string TE_TRIM_PROBE_CLEAN_DONE { get { return this._TE_TRIM_PROBE_CLEAN_DONE; } set { this._TE_TRIM_PROBE_CLEAN_DONE = value; } }

		private string _TE_MEAS_COLUMN_DONE = "Measure Column Done.";
		public string TE_MEAS_COLUMN_DONE { get { return this._TE_MEAS_COLUMN_DONE; } set { this._TE_MEAS_COLUMN_DONE = value; } }

		private string _TE_MEAS_RES_DONE = "Measure Resistor Done";
		public string TE_MEAS_RES_DONE { get { return this._TE_MEAS_RES_DONE; } set { this._TE_MEAS_RES_DONE = value; } }

		private string _TE_LASER_CALIBRATION_UPADTED = "Laser Calibration Updated";
		public string TE_LASER_CALIBRATION_UPADTED { get { return this._TE_LASER_CALIBRATION_UPADTED; } set { this._TE_LASER_CALIBRATION_UPADTED = value; } }


		private string _V_PR_TEACH_TEST_AND_LOCATE = "PR-testing and searching positon";
		public string V_PR_TEACH_TEST_AND_LOCATE { get { return this._V_PR_TEACH_TEST_AND_LOCATE; } set { this._V_PR_TEACH_TEST_AND_LOCATE = value; } }

		private string _V_PR_TEACH_TEST_AND_LOCATE_DONE = "PR tested and position updated";
		public string V_PR_TEACH_TEST_AND_LOCATE_DONE { get { return this._V_PR_TEACH_TEST_AND_LOCATE_DONE; } set { this._V_PR_TEACH_TEST_AND_LOCATE_DONE = value; } }

		private string _SYS_READY = "Ready";
		public string SYS_READY { get { return this._SYS_READY; } set { this._SYS_READY = value; } }

		private string _SYS_RST_COMPLETED = "System Reset Completed";
		public string SYS_RST_COMPLETED { get { return this._SYS_RST_COMPLETED; } set { this._SYS_RST_COMPLETED = value; } }

		private string _SYS_LASER_CAL_ABORT = "Laser Calibration Abort";
		public string SYS_LASER_CAL_ABORT { get { return this._SYS_LASER_CAL_ABORT; } set { this._SYS_LASER_CAL_ABORT = value; } }

		private string _SYS_LASER_CAL_DONE = "Laser Calibration Completed";
		public string SYS_LASER_CAL_DONE { get { return this._SYS_LASER_CAL_DONE; } set { this._SYS_LASER_CAL_DONE = value; } }

		private string _SYS_GALVO_CAL_PROCESS_DONE = "Read Calibration Process Done";
		public string SYS_GALVO_CAL_PROCESS_DONE { get { return this._SYS_GALVO_CAL_PROCESS_DONE; } set { this._SYS_GALVO_CAL_PROCESS_DONE = value; } }

		private string _SYS_GALVO_CAL_PROCESS_START = "Galvo Calibration Process Starts";
		public string SYS_GALVO_CAL_PROCESS_START { get { return this._SYS_GALVO_CAL_PROCESS_START; } set { this._SYS_GALVO_CAL_PROCESS_START = value; } }

		private string _SYS_GALVO_CAL_DATA_RESET_DONE = "Read Calibration Data Reset Done";
		public string SYS_GALVO_CAL_DATA_RESET_DONE { get { return this._SYS_GALVO_CAL_DATA_RESET_DONE; } set { this._SYS_GALVO_CAL_DATA_RESET_DONE = value; } }

		private string _SYS_LASER_LAST_FILE_LOAD = "Laser Last Calibration File Loaded";
		public string SYS_LASER_LAST_FILE_LOAD { get { return this._SYS_LASER_LAST_FILE_LOAD; } set { this._SYS_LASER_LAST_FILE_LOAD = value; } }

		private string _SYS_XHAIR_STT_SAVED = "Crosshair Setting Saved";
		public string SYS_XHAIR_STT_SAVED { get { return this._SYS_XHAIR_STT_SAVED; } set { this._SYS_XHAIR_STT_SAVED = value; } }

		private string _SEQ_LOADING_SUB_TO_CARRIAGE = "Loading Substrate to Carriage...";
		public string SEQ_LOADING_SUB_TO_CARRIAGE { get { return this._SEQ_LOADING_SUB_TO_CARRIAGE; } set { this._SEQ_LOADING_SUB_TO_CARRIAGE = value; } }

		private string _SEQ_UNLOADING_SUB_FROM_CARRIAGE = "UnLoading Substrate from Carriage...";
		public string SEQ_UNLOADING_SUB_FROM_CARRIAGE { get { return this._SEQ_UNLOADING_SUB_FROM_CARRIAGE; } set { this._SEQ_UNLOADING_SUB_FROM_CARRIAGE = value; } }

		private string _SEQ_UNLOADING_SUB_TO_NG_TRAY = "UnLoading Substrate to NG Tray...";
		public string SEQ_UNLOADING_SUB_TO_NG_TRAY { get { return this._SEQ_UNLOADING_SUB_TO_NG_TRAY; } set { this._SEQ_UNLOADING_SUB_TO_NG_TRAY = value; } }

		private string _SEQ_NEW_MAG_EMPTY = "New Elevator Empty";
		public string SEQ_NEW_MAG_EMPTY { get { return this._SEQ_NEW_MAG_EMPTY; } set { this._SEQ_NEW_MAG_EMPTY = value; } }

		private string _SEQ_PICK_FROM_NEW_MAG = "Pick Substrate from New Magzine...";
		public string SEQ_PICK_FROM_NEW_MAG { get { return this._SEQ_PICK_FROM_NEW_MAG; } set { this._SEQ_PICK_FROM_NEW_MAG = value; } }

		private string _SEQ_PERFORM_DOUBLE_SUB_CHK = "Performing Double Substrate Check...";
		public string SEQ_PERFORM_DOUBLE_SUB_CHK { get { return this._SEQ_PERFORM_DOUBLE_SUB_CHK; } set { this._SEQ_PERFORM_DOUBLE_SUB_CHK = value; } }

		private string _SEQ_PLACE_CUT_SUB = "Place Cut Substrate to Cut Magzine...";
		public string SEQ_PLACE_CUT_SUB { get { return this._SEQ_PLACE_CUT_SUB; } set { this._SEQ_PLACE_CUT_SUB = value; } }

		private string _P_MOVING_TO_TRIMMED_AREA = "Moving to Trimming Area";
		public string P_MOVING_TO_TRIMMED_AREA { get { return this._P_MOVING_TO_TRIMMED_AREA; } set { this._P_MOVING_TO_TRIMMED_AREA = value; } }

		private string _P_INDEXING_SUB = "Indexing Substrate";
		public string P_INDEXING_SUB { get { return this._P_INDEXING_SUB; } set { this._P_INDEXING_SUB = value; } }

		private string _P_WAIT_FOR_LOAD_UNLOAD = "Waiting For Loading/Unloading";
		public string P_WAIT_FOR_LOAD_UNLOAD { get { return this._P_WAIT_FOR_LOAD_UNLOAD; } set { this._P_WAIT_FOR_LOAD_UNLOAD = value; } }

		private string _P_LOADING_UNLOADING_TIME = "Loading/Unloading Time Taken : ";
		public string P_LOADING_UNLOADING_TIME { get { return this._P_LOADING_UNLOADING_TIME; } set { this._P_LOADING_UNLOADING_TIME = value; } }

		private string _P_START_AUTO_RUNNING = "Start Auto Running..";
		public string P_START_AUTO_RUNNING { get { return this._P_START_AUTO_RUNNING; } set { this._P_START_AUTO_RUNNING = value; } }

		private string _P_STOP_AUTO_RUNNING = "Stop Auto Running..";
		public string P_STOP_AUTO_RUNNING { get { return this._P_STOP_AUTO_RUNNING; } set { this._P_STOP_AUTO_RUNNING = value; } }

		private string _P_INDEXING_DONE = "Substrate Indexing has done in . ";
		public string P_INDEXING_DONE { get { return this._P_INDEXING_DONE; } set { this._P_INDEXING_DONE = value; } }

		private string _P_UNLOADING_FROM_CARR = "Unloading Sub from Carriage..";
		public string P_UNLOADING_FROM_CARR { get { return this._P_UNLOADING_FROM_CARR; } set { this._P_UNLOADING_FROM_CARR = value; } }

		private string _P_LOADING_TO_CARR = "Loading Sub to Carriage...";
		public string P_LOADING_TO_CARR { get { return this._P_LOADING_TO_CARR; } set { this._P_LOADING_TO_CARR = value; } }

		private string _P_PAUSE_AUTO_RUNNING = "Pause Auto Running..";
		public string P_PAUSE_AUTO_RUNNING { get { return this._P_PAUSE_AUTO_RUNNING; } set { this._P_PAUSE_AUTO_RUNNING = value; } }

		private string _P_RESUME_AUTO_RUNNING = "Resume Auto Running..";
		public string P_RESUME_AUTO_RUNNING { get { return this._P_RESUME_AUTO_RUNNING; } set { this._P_RESUME_AUTO_RUNNING = value; } }

		private string _P_PR_CHECKIND_DONE = "PR Checking Done..";
		public string P_PR_CHECKIND_DONE { get { return this._P_PR_CHECKIND_DONE; } set { this._P_PR_CHECKIND_DONE = value; } }

		private string _P_PR_CHECKIND_FAILED = "PR Checking Failed..";
		public string P_PR_CHECKIND_FAILED { get { return this._P_PR_CHECKIND_FAILED; } set { this._P_PR_CHECKIND_FAILED = value; } }

		private string _P_PR_CHECKIND_START = "PR Checking..";
		public string P_PR_CHECKIND_START { get { return this._P_PR_CHECKIND_START; } set { this._P_PR_CHECKIND_START = value; } }

		private string _P_PR_1_CHECKIND_START = "PR Ref 1 Checking..";
		public string P_PR_1_CHECKIND_START { get { return this._P_PR_1_CHECKIND_START; } set { this._P_PR_1_CHECKIND_START = value; } }

		private string _P_PR_2_CHECKIND_START = "PR Ref 2 Checking..";
		public string P_PR_2_CHECKIND_START { get { return this._P_PR_2_CHECKIND_START; } set { this._P_PR_2_CHECKIND_START = value; } }

		private string _P_PR_FINAL_CHECK_START = "PR Final Checking..";
		public string P_PR_FINAL_CHECK_START { get { return this._P_PR_FINAL_CHECK_START; } set { this._P_PR_FINAL_CHECK_START = value; } }

		private string _P_PR_THETA_CALCULATED = "PR Theta Calculated";
		public string P_PR_THETA_CALCULATED { get { return this._P_PR_THETA_CALCULATED; } set { this._P_PR_THETA_CALCULATED = value; } }

		private string _P_START_SEMI_AUTO_RUNNING = "Start Semi-Auto Running..";
		public string P_START_SEMI_AUTO_RUNNING { get { return this._P_START_SEMI_AUTO_RUNNING; } set { this._P_START_SEMI_AUTO_RUNNING = value; } }

		private string _P_STOP_SEMI_AUTO_RUNNING = "Stop Semi-Auto Running..";
		public string P_STOP_SEMI_AUTO_RUNNING { get { return this._P_STOP_SEMI_AUTO_RUNNING; } set { this._P_STOP_SEMI_AUTO_RUNNING = value; } }

		private string _P_PAUSE_SEMI_AUTO_RUNNING = "Pause Semi-Auto Running..";
		public string P_PAUSE_SEMI_AUTO_RUNNING { get { return this._P_PAUSE_SEMI_AUTO_RUNNING; } set { this._P_PAUSE_SEMI_AUTO_RUNNING = value; } }

		private string _P_RESUME_SEMI_AUTO_RUNNING = "Resume Semi-Auto Running..";
		public string P_RESUME_SEMI_AUTO_RUNNING { get { return this._P_RESUME_SEMI_AUTO_RUNNING; } set { this._P_RESUME_SEMI_AUTO_RUNNING = value; } }

		private string _S_Autorun_Not_Start = "Autorun Not Start";
		public string S_Autorun_Not_Start { get { return this._S_Autorun_Not_Start; } set { this._S_Autorun_Not_Start = value; } }

		private string _S_Autorun_Start = "Autorun Start";
		public string S_Autorun_Start { get { return this._S_Autorun_Start; } set { this._S_Autorun_Start = value; } }

		private string _S_Load_Unload_Substrate = "Load and Unload substrate";
		public string S_Load_Unload_Substrate { get { return this._S_Load_Unload_Substrate; } set { this._S_Load_Unload_Substrate = value; } }

		private string _S_Start_PR_Check = "Start PR Check";
		public string S_Start_PR_Check { get { return this._S_Start_PR_Check; } set { this._S_Start_PR_Check = value; } }

		private string _S_Double_Part_Check = "Double Part Check";
		public string S_Double_Part_Check { get { return this._S_Double_Part_Check; } set { this._S_Double_Part_Check = value; } }

		private string _S_PR_CHECK_REF_1 = "PR Ref 1 Check";
		public string S_PR_CHECK_REF_1 { get { return this._S_PR_CHECK_REF_1; } set { this._S_PR_CHECK_REF_1 = value; } }

		private string _S_PR_CHECK_REF_2 = "PR Ref 2 Check";
		public string S_PR_CHECK_REF_2 { get { return this._S_PR_CHECK_REF_2; } set { this._S_PR_CHECK_REF_2 = value; } }

		private string _S_PR_THETA_COMPENSATE = "PR Theta Angle Compensation";
		public string S_PR_THETA_COMPENSATE { get { return this._S_PR_THETA_COMPENSATE; } set { this._S_PR_THETA_COMPENSATE = value; } }

		private string _S_FINAL_PR_CHECK = "Final PR Check";
		public string S_FINAL_PR_CHECK { get { return this._S_FINAL_PR_CHECK; } set { this._S_FINAL_PR_CHECK = value; } }

		private string _S_GOTO_TRIM_POS = "Go To Trim Pos";
		public string S_GOTO_TRIM_POS { get { return this._S_GOTO_TRIM_POS; } set { this._S_GOTO_TRIM_POS = value; } }

		private string _S_PERFORM_TRIMMING = "Trim Column";
		public string S_PERFORM_TRIMMING { get { return this._S_PERFORM_TRIMMING; } set { this._S_PERFORM_TRIMMING = value; } }

		private string _S_INDEX_TABLE = "Indexing";
		public string S_INDEX_TABLE { get { return this._S_INDEX_TABLE; } set { this._S_INDEX_TABLE = value; } }

		private string _S_LOAD_UNLOAD_SUB = "Load and Unload Substrate";
		public string S_LOAD_UNLOAD_SUB { get { return this._S_LOAD_UNLOAD_SUB; } set { this._S_LOAD_UNLOAD_SUB = value; } }

		private string _S_GOTO_LOAD_POS = "Go To Load Position";
		public string S_GOTO_LOAD_POS { get { return this._S_GOTO_LOAD_POS; } set { this._S_GOTO_LOAD_POS = value; } }

		private string _S_AUTORUN_STOP = "Autorun Stop";
		public string S_AUTORUN_STOP { get { return this._S_AUTORUN_STOP; } set { this._S_AUTORUN_STOP = value; } }

		private string _S_DOUBLE_CLAMP_CHECK = "Double Clamp Check";
		public string S_DOUBLE_CLAMP_CHECK { get { return this._S_DOUBLE_CLAMP_CHECK; } set { this._S_DOUBLE_CLAMP_CHECK = value; } }

		private string _S_CARRIAGE_SUB_CONDITION_CHECK = "Carriage Substrate Condition Check";
		public string S_CARRIAGE_SUB_CONDITION_CHECK { get { return this._S_CARRIAGE_SUB_CONDITION_CHECK; } set { this._S_CARRIAGE_SUB_CONDITION_CHECK = value; } }

		private string _S_CARRIAGE_MOVE_TO_DOUBLE_SUB_POS = "Carriage Move To Double Substrate Checking Position";
		public string S_CARRIAGE_MOVE_TO_DOUBLE_SUB_POS { get { return this._S_CARRIAGE_MOVE_TO_DOUBLE_SUB_POS; } set { this._S_CARRIAGE_MOVE_TO_DOUBLE_SUB_POS = value; } }

		private string _S_DOUBLE_SUBSTRATE_DETECTION = "Double Substrate Detection";
		public string S_DOUBLE_SUBSTRATE_DETECTION { get { return this._S_DOUBLE_SUBSTRATE_DETECTION; } set { this._S_DOUBLE_SUBSTRATE_DETECTION = value; } }

		private string _S_CARRIAGE_MOVE_TO_PR1_POS = "Carriage Move To PR1 Position";
		public string S_CARRIAGE_MOVE_TO_PR1_POS { get { return this._S_CARRIAGE_MOVE_TO_PR1_POS; } set { this._S_CARRIAGE_MOVE_TO_PR1_POS = value; } }

		private string _S_CARRIAGE_MOVE_TO_PR2_POS = "Carriage Move To PR2 Position";
		public string S_CARRIAGE_MOVE_TO_PR2_POS { get { return this._S_CARRIAGE_MOVE_TO_PR2_POS; } set { this._S_CARRIAGE_MOVE_TO_PR2_POS = value; } }

		private string _S_LOAD_MODEL_AND_FIND = "Load PR model and Find";
		public string S_LOAD_MODEL_AND_FIND { get { return this._S_LOAD_MODEL_AND_FIND; } set { this._S_LOAD_MODEL_AND_FIND = value; } }

		private string _S_THETA_OFFSET_CALCULATION = "Theta Offset Calculation";
		public string S_THETA_OFFSET_CALCULATION { get { return this._S_THETA_OFFSET_CALCULATION; } set { this._S_THETA_OFFSET_CALCULATION = value; } }

		private string _S_CARRIAGE_THETA_MOVE = "Carriage Theta Move";
		public string S_CARRIAGE_THETA_MOVE { get { return this._S_CARRIAGE_THETA_MOVE; } set { this._S_CARRIAGE_THETA_MOVE = value; } }

		private string _S_CALCULATE_XY_OFFSET_AND_CARRIAGE_MOVE = "XY Offset Calculation and Carriage Movement";
		public string S_CALCULATE_XY_OFFSET_AND_CARRIAGE_MOVE { get { return this._S_CALCULATE_XY_OFFSET_AND_CARRIAGE_MOVE; } set { this._S_CALCULATE_XY_OFFSET_AND_CARRIAGE_MOVE = value; } }

		private string _S_CARRIAGE_MOVE_TO_TRIMMING_POS = "Carriage Move To Trimming Position";
		public string S_CARRIAGE_MOVE_TO_TRIMMING_POS { get { return this._S_CARRIAGE_MOVE_TO_TRIMMING_POS; } set { this._S_CARRIAGE_MOVE_TO_TRIMMING_POS = value; } }

		private string _S_RESET_SUB_STATISTIC = "Reset Substrate Statistic";
		public string S_RESET_SUB_STATISTIC { get { return this._S_RESET_SUB_STATISTIC; } set { this._S_RESET_SUB_STATISTIC = value; } }

		private string _S_DOWNLOAD_INFORMATION_TO_S_CORE = "Download Starting Information To S-Core";
		public string S_DOWNLOAD_INFORMATION_TO_S_CORE { get { return this._S_DOWNLOAD_INFORMATION_TO_S_CORE; } set { this._S_DOWNLOAD_INFORMATION_TO_S_CORE = value; } }

		private string _S_TRIMALL_TO_S_CORE = "Send TrimAll command To S-Core";
		public string S_TRIMALL_TO_S_CORE { get { return this._S_TRIMALL_TO_S_CORE; } set { this._S_TRIMALL_TO_S_CORE = value; } }

		private string _S_UPDATING_UI_AND_SAVE_PRODUCTION_INFO = "Update UI and Save Production Statistic";
		public string S_UPDATING_UI_AND_SAVE_PRODUCTION_INFO { get { return this._S_UPDATING_UI_AND_SAVE_PRODUCTION_INFO; } set { this._S_UPDATING_UI_AND_SAVE_PRODUCTION_INFO = value; } }

		private string _S_TRIMALL_DATA_COLLECTION = "TRIMALL Data Collection";
		public string S_TRIMALL_DATA_COLLECTION { get { return this._S_TRIMALL_DATA_COLLECTION; } set { this._S_TRIMALL_DATA_COLLECTION = value; } }

		private string _S_PROBE_CLEANING = "Probe Cleaning";
		public string S_PROBE_CLEANING { get { return this._S_PROBE_CLEANING; } set { this._S_PROBE_CLEANING = value; } }

		private string _S_SAVE_SUB_DETAIL = "Save Substrate Detail";
		public string S_SAVE_SUB_DETAIL { get { return this._S_SAVE_SUB_DETAIL; } set { this._S_SAVE_SUB_DETAIL = value; } }

		private string _S_WRITE_PRODUCTION_INFO = "Write Production Information";
		public string S_WRITE_PRODUCTION_INFO { get { return this._S_WRITE_PRODUCTION_INFO; } set { this._S_WRITE_PRODUCTION_INFO = value; } }

		private string _S_CARR_INTERLEAVE_MOVEMENT = "Carriage Interleave Movement";
		public string S_CARR_INTERLEAVE_MOVEMENT { get { return this._S_CARR_INTERLEAVE_MOVEMENT; } set { this._S_CARR_INTERLEAVE_MOVEMENT = value; } }

		private string _S_CARR_INDEX_MOVEMENT = "Carriage Indexing Movement";
		public string S_CARR_INDEX_MOVEMENT { get { return this._S_CARR_INDEX_MOVEMENT; } set { this._S_CARR_INDEX_MOVEMENT = value; } }

		//ITMS related events
		private string _ITMS_PING_UPLOAD_DONE = "ITMS Ping Data Upload Done";
		public string ITMS_PING_UPLOAD_DONE { get { return this._ITMS_PING_UPLOAD_DONE; } set { this._ITMS_PING_UPLOAD_DONE = value; } }

		private string _ITMS_SUB_UPLOAD_DONE = "ITMS Substrate Data Upload Done";
		public string ITMS_SUB_UPLOAD_DONE { get { return this._ITMS_SUB_UPLOAD_DONE; } set { this._ITMS_SUB_UPLOAD_DONE = value; } }

		private string _ITMS_BATCH_UPLOAD_DONE = "ITMS Batch Data Upload Done";
		public string ITMS_BATCH_UPLOAD_DONE { get { return this._ITMS_BATCH_UPLOAD_DONE; } set { this._ITMS_BATCH_UPLOAD_DONE = value; } }

		private string _ITMS_MACHINE_STATUS_UPLOAD_DONE = "ITMS Machine Status Upload Done";
		public string ITMS_MACHINE_STATUS_UPLOAD_DONE { get { return this._ITMS_MACHINE_STATUS_UPLOAD_DONE; } set { this._ITMS_MACHINE_STATUS_UPLOAD_DONE = value; } }

		private string _ITMS_MACHINE_DATA_UPLOAD_DONE = "ITMS Machine Data Upload Done";
		public string ITMS_MACHINE_DATA_UPLOAD_DONE { get { return this._ITMS_MACHINE_DATA_UPLOAD_DONE; } set { this._ITMS_MACHINE_DATA_UPLOAD_DONE = value; } }
	}



}
