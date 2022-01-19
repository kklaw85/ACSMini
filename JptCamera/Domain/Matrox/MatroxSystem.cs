using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Matrox.MatroxImagingLibrary;

namespace JptCamera
{
    public class MatroxSystem
    {
        private MIL_ID _applicationID = MIL.M_NULL;

        private MIL_ID _defaultSystemID = MIL.M_NULL;
        private MIL_ID _hostSystemID = MIL.M_NULL;
        private MIL_ID _gigeSystemID = MIL.M_NULL;
        private MIL_ID _usb3SystemID = MIL.M_NULL;
        private MIL_ID _rapixoSystemID = MIL.M_NULL;

        public MatroxSystem()
        {
            if (_applicationID == MIL.M_NULL)
            {
                InitialiseApplicationID();
            }
        }

        /// <summary>
        /// Initialising MIL Application can only be done once.
        /// </summary>
        /// <returns></returns>
        public string InitialiseApplicationID()
        {
            JPTUtility.Logger.doLog("MatroxSystem - InitialiseApplicationID ...");
            try
            {
                if (_applicationID == MIL.M_NULL)
                {
                    MIL.MappAlloc(MIL.M_NULL, MIL.M_QUIET, ref _applicationID);

                    MIL.MappControl(MIL.M_DEFAULT, MIL.M_ERROR, MIL.M_THROW_EXCEPTION);

                    double result = 1;
                    MIL.MappInquire(_applicationID, MIL.M_CURRENT_APPLICATION, ref result);

                    if (result == 0)
                    {
                        //throw new Exception("MIL Application can only be initialised once");
                    }
                }
            }
            catch(Exception ex)
            {
                string error = string.Format("MatroxSystem_InitialiseApplicationID error: " + ex.Message);
                JPTUtility.Logger.doLog(error);
                return error;
            }
            return string.Empty;
        }

        public MIL_ID ID
        {
            get { return _applicationID; }
        }


        public MIL_ID DefaultSystemID
        {
            get
            {
                try
                {
                    if (_defaultSystemID == MIL.M_NULL)
                    {
                        MIL.MsysAlloc(_applicationID, "M_SYSTEM_DEFAULT", MIL.M_DEFAULT, MIL.M_DEFAULT, ref _defaultSystemID);
                    }
                }
                catch (Exception ex)
                {
                    string error = string.Format("MatroxSystem_DefaultSystemID error: " + ex.Message);
                    JPTUtility.Logger.doLog(error);
                    return MIL.M_NULL;
                }
                return _defaultSystemID;
            }
        }

        public MIL_ID HostSystemID
        {
            get
            {
                try
                {
                    if (_hostSystemID == MIL.M_NULL)
                    {
                        MIL.MsysAlloc(_applicationID, "M_SYSTEM_HOST", MIL.M_DEFAULT, MIL.M_DEFAULT, ref _hostSystemID);
                    }
                }
                catch (Exception ex)
                {
                    string error = string.Format("MatroxSystem_HostSystemID error: " + ex.Message);
                    JPTUtility.Logger.doLog(error);
                    return MIL.M_NULL;
                }
                return _hostSystemID;
            }
        }

        public MIL_ID GigeSystemID
        {
            get
            {
                try
                {
                    if (_gigeSystemID == MIL.M_NULL)
                    {
                        MIL.MsysAlloc(_applicationID, "M_SYSTEM_GIGE_VISION", MIL.M_DEFAULT, MIL.M_DEFAULT, ref _gigeSystemID);
                    }
                }
                catch (Exception ex)
                {
                    string error = string.Format("MatroxSystem_GigeSystemID error: " + ex.Message);
                    JPTUtility.Logger.doLog(error);
                    return MIL.M_NULL;
                }
                return _gigeSystemID;
            }
        }

        public MIL_ID Usb3SystemID
        {
            get
            {
                try
                {
                    if (_usb3SystemID == MIL.M_NULL)
                    {
                        MIL.MsysAlloc(_applicationID, "M_SYSTEM_USB3_VISION", MIL.M_DEFAULT, MIL.M_DEFAULT, ref _usb3SystemID);
                    }
                }
                catch (Exception ex)
                {
                    string error = string.Format("MatroxSystem_GigeSystemID error: " + ex.Message);
                    JPTUtility.Logger.doLog(error);
                    return MIL.M_NULL;
                }
                return _usb3SystemID;
            }
        }

        public MIL_ID RapixoSystemID
        {
            get
            {
                try
                {
                    if (_rapixoSystemID == MIL.M_NULL)
                    {
                        MIL.MsysAlloc(_applicationID, "M_SYSTEM_RAPIXOCXP", MIL.M_DEFAULT, MIL.M_DEFAULT, ref _rapixoSystemID);
                    }
                }
                catch (Exception ex)
                {
                    string error = string.Format("MatroxSystem_RapixoSystemID error: " + ex.Message);
                    JPTUtility.Logger.doLog(error);
                    return MIL.M_NULL;
                }
                return _rapixoSystemID;
            }
        }

        public void FreeResources()
        {
            JPTUtility.Logger.doLog("MatroxSystem - FreeResources ...");

            if (_defaultSystemID != MIL.M_NULL)
            {
                MIL.MsysFree(_defaultSystemID);
                _defaultSystemID = MIL.M_NULL;
            }

            if (_hostSystemID != MIL.M_NULL)
            {
                MIL.MsysFree(_hostSystemID);
                _hostSystemID = MIL.M_NULL;
            }

            if (_gigeSystemID != MIL.M_NULL)
            {
                MIL.MsysFree(_gigeSystemID);
                _gigeSystemID = MIL.M_NULL;
            }

            if (_usb3SystemID != MIL.M_NULL)
            {
                MIL.MsysFree(_usb3SystemID);
                _usb3SystemID = MIL.M_NULL;
            }

            if (_rapixoSystemID != MIL.M_NULL)
            {
                MIL.MsysFree(_rapixoSystemID);
                _rapixoSystemID = MIL.M_NULL;
            }

            if (_applicationID != MIL.M_NULL)
            {
                MIL.MappFree(_applicationID);
                _applicationID = MIL.M_NULL;
            }

            // The object has been cleaned up.
            // This call removes the object from the finalization queue and 
            // prevent finalization code object from executing a second time.
            GC.SuppressFinalize(this);
        }
    }
}
