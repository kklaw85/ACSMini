using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using MvCamCtrl.NET;

namespace JptCamera
{
    public class HikSdkUsb3Cam : Camera
    {

        public enum HIKPixelFormatEnum
        {
            Mono8 = 0x01080001,
            Mono10 = 0x01100003,
            Mono10Packed = 0x010C0004,
            Mono12 = 0x01100005,
            Mono12Packed = 0x010C0006,
            Mono16 = 0x01100007,
            RGB8Packed = 0x02180014,
            YUV422_8 = 0x02100032,
            YUV422_8_UYVY = 0x0210001F,
            YUV8_UYV = 0x02180020,
            YUV411_8_UYYVYY = 0x020C001E,
            BayerGR8 = 0x01080008,
            BayerRG8 = 0x01080009,
            BayerGB8 = 0x0108000A,
            BayerBG8 = 0x0108000B,
            BayerGR10 = 0x0110000c,
            BayerRG10 = 0x0110000d,
            BayerGB10 = 0x0110000e,
            BayerBG10 = 0x0110000f,
            BayerBG10Packed = 0x010C0029,
            BayerGR10Packed = 0x010C0026,
            BayerRG10Packed = 0x010C0027,
            BayerGB10Packed = 0x010C0028,
            BayerGR12 = 0x01100010,
            BayerRG12 = 0x01100011,
            BayerGB12 = 0x01100012,
            BayerBG12 = 0x01100013,
            BayerBG12Packed = 0x010C002D,
            BayerGR12Packed = 0x010C002A,
            BayerRG12Packed = 0x010C002B,
            BayerGB12Packed = 0x010C002C,
            BayerGR16 = 0x0110002E,
            BayerRG16 = 0x0110002F,
            BayerGB16 = 0x01100030,
            BayerBG16 = 0x01100031,
        }

        public enum TriggerEnableEnum
        {
            TRIGGER_OFF = 0,
            TRIGGER_ON = 1,
        }

        public enum HikTriggerSourceEnum
        {
            Line0 = 0,
            Line1 = 1,
            Line2 = 2,
            Line3 = 3,
            Counter = 4,
            Software = 7,
        }

        MyCamera.MV_CC_DEVICE_INFO_LIST m_pDeviceList;

        CameraOperator m_Operator = new CameraOperator();

        public int CameraCount;

        MyCamera.cbOutputdelegate ImageCallback;
        MyCamera.cbOutputExdelegate ImageCallbackEx;

        //public Action<IntPtr, MyCamera.MV_FRAME_OUT_INFO, IntPtr> OutImageCallBack;

        Object _lockable = new object();
        private uint m_nBufSizeForSaveImage = 56625152u;
        private byte[] m_pBufForSaveImage = new byte[56625152];
        NewImageEventArgs imgEventArgs = new NewImageEventArgs();

        bool _isNewImageReceived = false;

        public HikSdkUsb3Cam()
        { }
        
        public override string ConnectCamera()
        {
            JPTUtility.Logger.doLog("HikSdkUsb3Cam - Connect ...");
            string sErr = string.Empty;
            int nRet;
            try
            {
                if (CameraCount != 0)
                {
                    return "Camera Connected";
                }
                System.GC.Collect();
                nRet = CameraOperator.EnumDevices(MyCamera.MV_USB_DEVICE, ref m_pDeviceList);
                if (0 != nRet)
                {
                    return "No Device";
                }
                for (int i = 0; i < m_pDeviceList.nDeviceNum; i++)
                {
                    MyCamera.MV_CC_DEVICE_INFO device = (MyCamera.MV_CC_DEVICE_INFO)Marshal.PtrToStructure(m_pDeviceList.pDeviceInfo[i], typeof(MyCamera.MV_CC_DEVICE_INFO));

                    nRet = m_Operator.Open(ref device);
                    if (0 != nRet)
                    {
                        if (i < m_pDeviceList.nDeviceNum)
                        { continue; }
                        else
                        { return "Open Device Fail"; }
                    }

                    //ImageCallback = new MyCamera.cbOutputdelegate(NewImageReceived);
                    //nRet = m_Operator.RegisterImageCallBack(ImageCallback, IntPtr.Zero);
                    ImageCallbackEx = new MyCamera.cbOutputExdelegate(NewImageReceivedEx);
                    nRet = m_Operator.RegisterImageCallBackEx(ImageCallbackEx, IntPtr.Zero);
                    if (CameraOperator.CO_OK != nRet)
                    {
                        return sErr = "Register callback fail";
                    }

                    CameraCount++;
                    break;
                }
                if (CameraCount == 0)
                {
                    sErr = "No Camera Found";
                }

            }
            catch (Exception ex)
            {
                sErr = string.Format("HikSdkUsb3Cam_Connect error: " + ex.Message);
                JPTUtility.Logger.doLog(sErr);
            }
            return sErr;
        }

        public override string ConnectCamera(string userDefineName)
        {
            JPTUtility.Logger.doLog("HikSdkUsb3Cam - Connect ...");
            string sErr = string.Empty;
            int nRet;
            try
            {
                if (CameraCount != 0)
                {
                    return "Camera Connected";
                }
                System.GC.Collect();
                nRet = CameraOperator.EnumDevices(MyCamera.MV_USB_DEVICE, ref m_pDeviceList);
                if (0 != nRet)
                {

                    return "No Device";
                }
                for (int i = 0; i < m_pDeviceList.nDeviceNum; i++)
                {
                    MyCamera.MV_CC_DEVICE_INFO device = (MyCamera.MV_CC_DEVICE_INFO)Marshal.PtrToStructure(m_pDeviceList.pDeviceInfo[i], typeof(MyCamera.MV_CC_DEVICE_INFO));
                    
                    nRet = m_Operator.Open(ref device);
                    if (0 != nRet)
                    {
                        if (i < m_pDeviceList.nDeviceNum)
                        { continue; }
                        else
                        { return "Open Device Fail"; }
                    }
                    IntPtr buffer = Marshal.UnsafeAddrOfPinnedArrayElement(device.SpecialInfo.stUsb3VInfo, 0);
                    MyCamera.MV_USB3_DEVICE_INFO usbInfo = (MyCamera.MV_USB3_DEVICE_INFO)Marshal.PtrToStructure(buffer, typeof(MyCamera.MV_USB3_DEVICE_INFO));
                    if (usbInfo.chUserDefinedName == userDefineName)
                    {

                        //ImageCallback = new MyCamera.cbOutputdelegate(NewImageReceived);
                        //nRet = m_Operator.RegisterImageCallBack(ImageCallback, IntPtr.Zero);
                        ImageCallbackEx = new MyCamera.cbOutputExdelegate(NewImageReceivedEx);
                        nRet = m_Operator.RegisterImageCallBackEx(ImageCallbackEx, IntPtr.Zero);
                        if (CameraOperator.CO_OK != nRet)
                        {
                            return sErr = "Register callback fail";
                        }

                        CameraCount++;
                        break;
                    }
                    else
                    { m_Operator.Close(); }
                }
                if (CameraCount == 0)
                {
                    sErr = "No Camera";
                }

            }
            catch (Exception ex)
            {
                sErr = string.Format("HikSdkUsb3Cam_Connect error: " + ex.Message);
                JPTUtility.Logger.doLog(sErr);
            }
            return sErr;
        }

        public override string SetDisplayWinForm(IntPtr displayHost)
        {
            JPTUtility.Logger.doLog("HikSdkUsb3Cam - SetDisplayWinForm ...");
            string sErr = string.Empty;
            int nRet;
            try
            {
                if (CameraCount == 0)
                {
                    return sErr = "No Camera";
                }
                nRet = m_Operator.Display(displayHost);
                if (nRet != 0)
                {
                    sErr = "show fail";
                }
            }
            catch (Exception ex)
            {
                sErr = string.Format("HikSdkUsb3Cam_SetDisplayWinForm error: " + ex.Message);
                JPTUtility.Logger.doLog(sErr);
            }
            return sErr;
        }

        public override string DisconnectCamera()
        {
            JPTUtility.Logger.doLog("HikSdkUsb3Cam - RemoveCamera ...");
            string sErr = string.Empty;
            try
            {
                int nRet = 0;

                if (CameraCount == 0)
                {
                    return sErr = "No Camera";
                }
                CameraCount = 0;
                nRet = m_Operator.StopGrabbing();
                nRet = m_Operator.Close();

                if (nRet != 0)
                {
                    throw new Exception("Close failed.");
                }

            }
            catch (Exception ex)
            {
                sErr = string.Format("HikSdkUsb3Cam_RemoveCamera error: " + ex.Message);
                JPTUtility.Logger.doLog(sErr);
                return sErr;
            }
            return sErr;
        }

        public override string GrabSingle()
        {
            JPTUtility.Logger.doLog("HikSdkUsb3Cam - GrabSingle ...");
            string sErr = string.Empty;
            int nRet;
            try
            {
                if (CameraCount == 0)
                {
                    return sErr = "No Camera";
                }

                var currentTrigType = this.TriggerType;

                this.TriggerType = TriggerTypeEnum.TRIGGER_SOFTWARE;

                nRet = m_Operator.StartGrabbing();
                if (MyCamera.MV_OK != nRet)
                {

                    return "StartGrabbing Fail";
                }

                _isNewImageReceived = false;

                nRet = m_Operator.CommandExecute("TriggerSoftware");
                if (nRet != 0)
                {
                    sErr = "GrabSingle Fail";
                }

                int c = 0;
                int interval = 50;
                int timeout = 2000;
                bool isTimeout = false;

                while (_isNewImageReceived == false && isTimeout == false)
                {
                    c++;
                    System.Threading.Thread.Sleep(interval);

                    if (interval * c > timeout)
                    {
                        isTimeout = true;
                    }
                }

                nRet = m_Operator.StopGrabbing();
                if (MyCamera.MV_OK != nRet)
                {

                    return "StopGrabbing Fail";
                }

                this.TriggerType = currentTrigType;

                if (isTimeout)
                { throw new Exception("Timeout 2s"); }
            }
            catch (Exception ex)
            {
                sErr = string.Format("HikSdkUsb3Cam_GrabSingle error: " + ex.Message);
                JPTUtility.Logger.doLog(sErr);
            }
            return sErr;
        }

        public override string StartGrab()
        {
            JPTUtility.Logger.doLog("HikSdkUsb3Cam - GrabContinuous ...");
            string sErr = string.Empty;
            int nRet;
            try
            {
                if (CameraCount == 0)
                {
                    return sErr = "No Camera";
                }
                nRet = m_Operator.StartGrabbing();
                if (MyCamera.MV_OK != nRet)
                {

                    return "StartGrabbing Fail";
                }
                // CameraCount++;
            }
            catch (Exception ex)
            {
                sErr = string.Format("HikSdkUsb3Cam_GrabContinuous error: " + ex.Message);
                JPTUtility.Logger.doLog(sErr);
            }
            return sErr;
        }

        public override string StopGrab()
        {
            JPTUtility.Logger.doLog("HikSdkUsb3Cam - StopContinuous ...");
            string sErr = string.Empty;
            int nRet;
            try
            {
                if (CameraCount == 0)
                {
                    return sErr = "No Camera";
                }
                nRet = m_Operator.StopGrabbing();
                if (MyCamera.MV_OK != nRet)
                {

                    return "StopGrabbing Fail";
                }
                // CameraCount++;
            }
            catch (Exception ex)
            {
                sErr = string.Format("HikSdkUsb3Cam_StopContinuous error: " + ex.Message);
                JPTUtility.Logger.doLog(sErr);
            }
            return sErr;
        }

        //private void GetImage(IntPtr pData, ref MyCamera.MV_FRAME_OUT_INFO pFrameInfo, IntPtr pUser)
        //{
        //    if (OutImageCallBack != null)
        //    {
        //        OutImageCallBack(pData, pFrameInfo, pUser);
        //    }

        //}


        private void NewImageReceived(IntPtr pData, ref MyCamera.MV_FRAME_OUT_INFO stFrameInfo, IntPtr pUser)
        {
            try
            {
                lock (_lockable)
                {
                    _isNewImageReceived = true;

                    if (3u * stFrameInfo.nFrameLen + 2048u > this.m_nBufSizeForSaveImage)
                    {
                        this.m_nBufSizeForSaveImage = 3u * stFrameInfo.nFrameLen + 2048u;
                        this.m_pBufForSaveImage = new byte[this.m_nBufSizeForSaveImage];
                    }
                    IntPtr pImageBuffer = Marshal.UnsafeAddrOfPinnedArrayElement(this.m_pBufForSaveImage, 0);
                    MyCamera.MV_SAVE_IMAGE_PARAM_EX mV_SAVE_IMAGE_PARAM_EX = default(MyCamera.MV_SAVE_IMAGE_PARAM_EX);
                    mV_SAVE_IMAGE_PARAM_EX.enImageType = (MyCamera.MV_SAVE_IAMGE_TYPE)1;
                    mV_SAVE_IMAGE_PARAM_EX.enPixelType = stFrameInfo.enPixelType;
                    mV_SAVE_IMAGE_PARAM_EX.pData = pData;
                    mV_SAVE_IMAGE_PARAM_EX.nDataLen = stFrameInfo.nFrameLen;
                    mV_SAVE_IMAGE_PARAM_EX.nHeight = stFrameInfo.nHeight;
                    mV_SAVE_IMAGE_PARAM_EX.nWidth = stFrameInfo.nWidth;
                    mV_SAVE_IMAGE_PARAM_EX.pImageBuffer = pImageBuffer;
                    mV_SAVE_IMAGE_PARAM_EX.nBufferSize = this.m_nBufSizeForSaveImage;
                    mV_SAVE_IMAGE_PARAM_EX.nJpgQuality = 80u;
                    int num = SaveImage(ref mV_SAVE_IMAGE_PARAM_EX);
                    if (num != 0)
                    {
                        JPTUtility.Logger.doLog("HikSdkUsb3Cam NewImageReceived: Save failed！");
                    }
                    else
                    {
                        using (MemoryStream memoryStream = new MemoryStream())
                        {
                            memoryStream.Write(this.m_pBufForSaveImage, 0, (int)mV_SAVE_IMAGE_PARAM_EX.nImageLen);
                            Bitmap bitmap = (Bitmap)Image.FromStream(memoryStream);
                            imgEventArgs.Height = bitmap.Height;
                            imgEventArgs.Width = bitmap.Width;
                            BitmapData bitmapData = bitmap.LockBits(new Rectangle(0, 0, imgEventArgs.Width, imgEventArgs.Height), ImageLockMode.ReadOnly, bitmap.PixelFormat);
                            int stride = bitmapData.Stride;
                            imgEventArgs.bImageArray = new byte[imgEventArgs.Height * stride];
                            Marshal.Copy(bitmapData.Scan0, imgEventArgs.bImageArray, 0, imgEventArgs.Height * stride);
                            bitmap.UnlockBits(bitmapData);
                            imgEventArgs.ImageBMP = bitmap;
                            //_bitmapLoadedsdk = MonoCameraorRgbCamera ? ConvertByteArrayToImageMonoFormat(_data, _width, _height) : ConvertByteArrayToImageRgbFormat(_data, _width, _height);
                        }
                        
                        OnNewImageReceived(imgEventArgs);
                    }
                }
            }
            catch (Exception ex)
            {
                JPTUtility.Logger.doLog("HikSdkUsb3Cam NewImageReceived error: " + ex.Message);
            }
        }

        private void NewImageReceivedEx(IntPtr pData, ref MyCamera.MV_FRAME_OUT_INFO_EX stFrameInfo, IntPtr pUser)
        {
            try
            {
                lock (_lockable)
                {
                    _isNewImageReceived = true;

                    imgEventArgs.bImageArray = null;
                    imgEventArgs.ImageArray = null;
                    imgEventArgs.ImageBMP = null;

                    imgEventArgs.Height = stFrameInfo.nHeight;
                    imgEventArgs.Width = stFrameInfo.nWidth;

                    var pixType = stFrameInfo.enPixelType;

                    if (pixType == MyCamera.MvGvspPixelType.PixelType_Gvsp_Mono8)
                    {
                        imgEventArgs.bImageArray = new byte[stFrameInfo.nFrameLen];
                        Marshal.Copy(pData, imgEventArgs.bImageArray, 0, (int)stFrameInfo.nFrameLen);
                    }
                    else if (pixType == MyCamera.MvGvspPixelType.PixelType_Gvsp_Mono10 
                        || pixType == MyCamera.MvGvspPixelType.PixelType_Gvsp_Mono12)
                    {
                        imgEventArgs.ImageArray = new short[stFrameInfo.nFrameLen / 2];
                        Marshal.Copy(pData, imgEventArgs.ImageArray, 0, (int)stFrameInfo.nFrameLen / 2);
                    }


                    OnNewImageReceived(imgEventArgs);
                }
            }
            catch (Exception ex)
            {
                JPTUtility.Logger.doLog("HikSdkUsb3Cam NewImageReceivedEx error: " + ex.Message);
            }
        }

        public int SaveImage(ref MyCamera.MV_SAVE_IMAGE_PARAM_EX pSaveParam)
        {
            int result;
            try
            {
                int num = this.m_Operator.SaveImage(ref pSaveParam);
                if (num != 0)
                {
                    result = -1;
                }
                else
                {
                    result = 0;
                }
            }
            catch (Exception ex)
            {
                JPTUtility.Logger.doLog("HikSdkUsb3Cam SaveImage error: " + ex.Message);
                JPTUtility.Logger.doLog(ex.StackTrace);
                result = -1;
            }
            return result;
        }

        public override bool IsCameraConnected
        {
            get
            {
                int nRet = -1;
                try
                {
                    bool result = false;
                    if (CameraCount == 0) return result;
                    if (m_Operator == null) return result;
                    float fVal = 0;
                    nRet = m_Operator.GetFloatValue("ExposureTime", ref fVal);
                    if (nRet != 0) return result;
                    if (fVal > 0) result = true;
                    return result;
                }
                catch (Exception ex)
                {
                    JPTUtility.Logger.doLog("HikSdkUsb3Cam IsCameraConnected error " + ex.Message);
                    return false;
                }
            }
        }

        public override string CameraBrand
        {
            get
            {
                int nRet = -1;
                string result = string.Empty;
                try
                {
                    if (CameraCount == 0)
                    {
                        return "";
                    }
                    nRet = m_Operator.GetStringValue("DeviceVendorName", ref result);
                    if (nRet != 0)
                    {
                        //throw new Exception();
                    }
                }
                catch (Exception ex) { throw new Exception("HikSdkUsb3Cam DeviceVendorName error: " + ex.Message); }
                return result;
            }
        }

        public override string CameraModel
        {
            get
            {
                int nRet = -1;
                string result = string.Empty;
                try
                {
                    if (CameraCount == 0)
                    {
                        return "";
                    }
                    nRet = m_Operator.GetStringValue("DeviceModelName", ref result);
                    if (nRet != 0)
                    {
                        //throw new Exception();
                    }
                }
                catch (Exception ex) { throw new Exception("HikSdkUsb3Cam DeviceModelName error: " + ex.Message); }
                return result;
            }
        }

        public override string CameraUserID
        {
            get
            {
                int nRet = -1;
                string result = string.Empty;
                try
                {
                    if (CameraCount == 0)
                    {
                        return "";
                    }
                    nRet = m_Operator.GetStringValue("DeviceUserID", ref result);
                    if (nRet != 0)
                    {
                        //throw new Exception();
                    }
                }
                catch (Exception ex) { throw new Exception("HikSdkUsb3Cam DeviceUserID error: " + ex.Message); }
                return result;
            }
        }

        public override string CameraSerialName
        {
            get
            {
                int nRet = -1;
                string result = string.Empty;
                try
                {
                    if (CameraCount == 0)
                    {
                        return "";
                    }
                    nRet = m_Operator.GetStringValue("DeviceSerialNumber", ref result);
                    if (nRet != 0)
                    {
                        //throw new Exception();
                    }
                }
                catch (Exception ex) { throw new Exception("HikSdkUsb3Cam DeviceSerialNumber error: " + ex.Message); }
                return result;
            }
        }

        public override double Exposure
        {
            get
            {
                int nRet;
                string sErr = string.Empty;
                double result = 0.0;
                try
                {
                    if (CameraCount == 0)
                    {
                        return result;
                    }
                    float fVal = 0;
                    nRet = m_Operator.GetFloatValue("ExposureTime", ref fVal);
                    if (nRet == 0)
                    {
                        result = Convert.ToDouble(fVal);
                    }
                }
                catch (Exception ex)
                {
                    sErr = string.Format("HikSdkUsb3Cam GetExposureTime error: " + ex.Message);
                    JPTUtility.Logger.doLog(sErr);
                }
                return result;
            }
            set
            {
                string sErr = string.Empty;
                int nRet;
                try
                {
                    float fVal = Convert.ToSingle(value);
                    if (CameraCount == 0)
                    {
                        return;
                    }
                    nRet = m_Operator.SetEnumValue("ExposureAuto", 0);
                    if (nRet != 0)
                    {
                        sErr = "SetExposureTime Fail";
                        return;
                    }
                    nRet = m_Operator.SetFloatValue("ExposureTime", fVal);
                    if (nRet != 0)
                    {
                        sErr = "SetExposureTime Fail";
                    }

                }
                catch (Exception ex)
                {
                    sErr = string.Format("HikSdkUsb3Cam SetExposureTime error: " + ex.Message);
                    JPTUtility.Logger.doLog(sErr);
                }
            }
        }

        public override double ExposureMinimum
        {
            get
            {
                int nRet;
                string sErr = string.Empty;
                double result = 0.0;
                try
                {
                    if (CameraCount == 0)
                    {
                        return result;
                    }
                    float fVal = 0;
                    nRet = m_Operator.GetFloatValueMin("ExposureTime", ref fVal);
                    if (nRet == 0)
                    {
                        result = Convert.ToDouble(fVal);
                    }
                }
                catch (Exception ex)
                {
                    sErr = string.Format("HikSdkUsb3Cam ExposureTimeMinimum error: " + ex.Message);
                    JPTUtility.Logger.doLog(sErr);
                }
                return result;
            }
        }

        public override double ExposureMaximum
        {
            get
            {
                int nRet;
                string sErr = string.Empty;
                double result = 0.0;
                try
                {
                    if (CameraCount == 0)
                    {
                        return result;
                    }
                    float fVal = 0;
                    nRet = m_Operator.GetFloatValueMax("ExposureTime", ref fVal);
                    if (nRet == 0)
                    {
                        result = Convert.ToDouble(fVal);
                    }
                }
                catch (Exception ex)
                {
                    sErr = string.Format("HikSdkUsb3Cam ExposureTimeMaximum error: " + ex.Message);
                    JPTUtility.Logger.doLog(sErr);
                }
                return result;
            }
        }

        public override ushort Binning { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public override PixelFormatEnum PixelFormat
        {
            get
            {
                int nRet = -1;
                PixelFormatEnum result = PixelFormatEnum.UNDEFINED;
                try
                {
                    uint pixFormat = 0;
                    if (CameraCount == 0)
                    {
                        return result;
                    }
                    nRet = m_Operator.GetEnumValue("PixelFormat", ref pixFormat);
                    if (nRet == 0)
                    {
                        switch ((HIKPixelFormatEnum)pixFormat)
                        {
                            case HIKPixelFormatEnum.Mono8:
                                {
                                    result = PixelFormatEnum.MONO8;
                                    break;
                                }
                            case HIKPixelFormatEnum.Mono10:
                                {
                                    result = PixelFormatEnum.MONO10;
                                    break;
                                }
                            case HIKPixelFormatEnum.Mono12:
                                {
                                    result = PixelFormatEnum.MONO12;
                                    break;
                                }
                            case HIKPixelFormatEnum.RGB8Packed:
                                {
                                    result = PixelFormatEnum.RGB8PACKED;
                                    break;
                                }
                            default:
                                {
                                    result = PixelFormatEnum.UNDEFINED;
                                    break;
                                }
                        }
                    }
                }
                catch (Exception ex) { throw new Exception("PixelFormat GET error: " + ex.Message); }
                return result;
            }
            set
            {
                int nRet = -1;
                string result = string.Empty;
                try
                {
                    uint pixFormat = 0;
                    HIKPixelFormatEnum inputValue = HIKPixelFormatEnum.Mono8;

                    switch (value)
                    {
                        case PixelFormatEnum.MONO8:
                            {
                                inputValue = HIKPixelFormatEnum.Mono8;
                                break;
                            }
                        case PixelFormatEnum.MONO10:
                            {
                                inputValue = HIKPixelFormatEnum.Mono10;
                                break;
                            }
                        case PixelFormatEnum.MONO12:
                            {
                                inputValue = HIKPixelFormatEnum.Mono12;
                                break;
                            }
                        case PixelFormatEnum.RGB8PACKED:
                            {
                                inputValue = HIKPixelFormatEnum.RGB8Packed;
                                break;
                            }
                        default:
                            {
                                inputValue = HIKPixelFormatEnum.Mono8;
                                break;
                            }
                    }

                    pixFormat = (uint)inputValue;
                    nRet = m_Operator.SetEnumValue("PixelFormat", pixFormat);
                    if (nRet == 0)
                    {
                        //Success
                    }
                }
                catch (Exception ex) { throw new Exception("PixelFormat(SET) error: " + ex.Message); }
            }
        }

        public override int Gain
        {
            get
            {
                int nRet;
                string sErr = string.Empty;
                int result = 0;
                try
                {
                    if (CameraCount == 0)
                    {
                        return result;
                    }
                    float fVal = 0;
                    nRet = m_Operator.GetFloatValue("Gain", ref fVal);
                    if (nRet == 0)
                    {
                        result = Convert.ToInt32(fVal);
                    }
                }
                catch (Exception ex)
                {
                    sErr = string.Format("HikSdkUsb3Cam GetGain error: " + ex.Message);
                    JPTUtility.Logger.doLog(sErr);
                }
                return result;
            }
            set
            {
                string sErr = string.Empty;
                int nRet;
                try
                {
                    float fVal = Convert.ToSingle(value);
                    if (CameraCount == 0)
                    {
                        return;
                    }

                    nRet = m_Operator.SetFloatValue("Gain", fVal);
                    if (nRet != 0)
                    {
                        sErr = "SetGain Fail";
                    }

                }
                catch (Exception ex)
                {
                    sErr = string.Format("HikSdkUsb3Cam SetGain error: " + ex.Message);
                    JPTUtility.Logger.doLog(sErr);
                }
            }
        }

        public override int GainMinimum
        {
            get
            {
                int nRet;
                string sErr = string.Empty;
                int result = 0;
                try
                {
                    if (CameraCount == 0)
                    {
                        return result;
                    }
                    float fVal = 0;
                    nRet = m_Operator.GetFloatValueMin("Gain", ref fVal);
                    if (nRet == 0)
                    {
                        result = Convert.ToInt32(fVal);
                    }
                }
                catch (Exception ex)
                {
                    sErr = string.Format("HikSdkUsb3Cam GainMinimum error: " + ex.Message);
                    JPTUtility.Logger.doLog(sErr);
                }
                return result;
            }
        }

        public override int GainMaximum
        {
            get
            {
                int nRet;
                string sErr = string.Empty;
                int result = 0;
                try
                {
                    if (CameraCount == 0)
                    {
                        return result;
                    }
                    float fVal = 0;
                    nRet = m_Operator.GetFloatValueMax("Gain", ref fVal);
                    if (nRet == 0)
                    {
                        result = Convert.ToInt32(fVal);
                    }
                }
                catch (Exception ex)
                {
                    sErr = string.Format("HikSdkUsb3Cam GainMaximum error: " + ex.Message);
                    JPTUtility.Logger.doLog(sErr);
                }
                return result;
            }
        }
        public override bool GammaEnable
        {
            get
            {
                int nRet = -1;
                bool result = false;
                try
                {
                    if (CameraCount == 0)
                    {
                        return result;
                    }
                    nRet = m_Operator.GetBoolValue("GammaEnable", ref result);
                    if (nRet != 0)
                    {
                        //
                    }
                }
                catch (Exception ex) { throw new Exception("GammaEnable Get error: " + ex.Message); }
                return result;
            }
            set
            {
                int nRet = -1;
                string result = string.Empty;
                try
                {
                    nRet = m_Operator.SetBoolValue("GammaEnable", value);
                    if (nRet == 0)
                    {
                        //Success
                    }
                }
                catch (Exception ex) { throw new Exception("GammaEnable(SET) error: " + ex.Message); }
            }
        }

        public override double Gamma
        {
            get
            {
                string sErr = string.Empty;
                int nRet = -1;
                double result = 0.0;
                try
                {
                    float tempV = 0;
                    if (CameraCount == 0)
                    {
                        return result;
                    }
                    nRet = m_Operator.GetFloatValue("Gamma", ref tempV);
                    if (nRet == 0)
                    {
                        result = tempV;
                    }
                }
                catch (Exception ex)
                {
                    sErr = string.Format("HikSdkUsb3Cam GetGamma error: " + ex.Message);
                    JPTUtility.Logger.doLog(sErr);
                }
                return result;
            }
            set
            {
                string sErr = string.Empty;
                int nRet;
                try
                {
                    float fVal = Convert.ToSingle(value);
                    if (CameraCount == 0)
                    {
                        return;
                    }
                    nRet = m_Operator.SetFloatValue("Gamma", fVal);
                    if (nRet != 0)
                    {
                        sErr = "SetGamma Fail";
                    }

                }
                catch (Exception ex)
                {
                    sErr = string.Format("HikSdkUsb3Cam SetGamma error: " + ex.Message);
                    JPTUtility.Logger.doLog(sErr);
                }
            }
        }

        public override double GammaMinimum
        {
            get
            {
                int nRet;
                string sErr = string.Empty;
                double result = 0.0;
                try
                {
                    if (CameraCount == 0)
                    {
                        return result;
                    }
                    float fVal = 0;
                    nRet = m_Operator.GetFloatValueMin("Gamma", ref fVal);
                    if (nRet == 0)
                    {
                        result = Convert.ToDouble(fVal);
                    }
                }
                catch (Exception ex)
                {
                    sErr = string.Format("HikSdkUsb3Cam GammaMinimum error: " + ex.Message);
                    JPTUtility.Logger.doLog(sErr);
                }
                return result;
            }
        }

        public override double GammaMaximum
        {
            get
            {
                int nRet;
                string sErr = string.Empty;
                double result = 0.0;
                try
                {
                    if (CameraCount == 0)
                    {
                        return result;
                    }
                    float fVal = 0;
                    nRet = m_Operator.GetFloatValueMax("Gamma", ref fVal);
                    if (nRet == 0)
                    {
                        result = Convert.ToDouble(fVal);
                    }
                }
                catch (Exception ex)
                {
                    sErr = string.Format("HikSdkUsb3Cam GammaMaximum error: " + ex.Message);
                    JPTUtility.Logger.doLog(sErr);
                }
                return result;
            }
        }

        public override TriggerTypeEnum TriggerType
        {
            get
            {
                int nRet = -1;
                TriggerTypeEnum result = TriggerTypeEnum.TRIGGER_CONTINUOUS;
                uint trigMode = 0;
                uint trigSource = 0;
                try
                {
                    if (CameraCount == 0)
                    {
                        return result;
                    }
                    nRet = m_Operator.GetEnumValue("TriggerMode", ref trigMode);
                    nRet = m_Operator.GetEnumValue("TriggerSource", ref trigSource);
                    if (nRet != 0)
                    {
                        if ((TriggerEnableEnum)trigMode == TriggerEnableEnum.TRIGGER_ON)
                        {
                            switch ((HikTriggerSourceEnum)trigSource)
                            {
                                case HikTriggerSourceEnum.Software:
                                    {
                                        result = TriggerTypeEnum.TRIGGER_SOFTWARE;
                                        break;
                                    }
                                case HikTriggerSourceEnum.Line0:
                                case HikTriggerSourceEnum.Line1:
                                case HikTriggerSourceEnum.Line2:
                                    {
                                        result = TriggerTypeEnum.TRIGGER_HW_RISING;
                                        break;
                                    }
                                default:
                                    {
                                        result = TriggerTypeEnum.TRIGGER_CONTINUOUS;
                                        break;
                                    }
                            }
                        }
                        else
                        {
                            result = TriggerTypeEnum.TRIGGER_CONTINUOUS;
                        }
                    }
                }
                catch (Exception ex) { throw new Exception("TriggerType Get error: " + ex.Message); }
                return result;
            }
            set
            {
                int nRet = -1;
                string result = string.Empty;
                try
                {
                    TriggerEnableEnum trigMode = TriggerEnableEnum.TRIGGER_OFF;
                    HikTriggerSourceEnum trigSource = HikTriggerSourceEnum.Software;

                    switch (value)
                    {
                        case TriggerTypeEnum.TRIGGER_SOFTWARE:
                            {
                                trigMode = TriggerEnableEnum.TRIGGER_ON;
                                trigSource = HikTriggerSourceEnum.Software;
                                break;
                            }
                        case TriggerTypeEnum.TRIGGER_HW_RISING:
                        case TriggerTypeEnum.TRIGGER_HW_FALLING:
                            {
                                trigMode = TriggerEnableEnum.TRIGGER_ON;
                                trigSource = HikTriggerSourceEnum.Line0;
                                break;
                            }
                        case TriggerTypeEnum.TRIGGER_CONTINUOUS:
                        case TriggerTypeEnum.TRIGGER_OFF:
                        default:
                            {
                                trigMode = TriggerEnableEnum.TRIGGER_OFF;
                                trigSource = HikTriggerSourceEnum.Software;
                                break;
                            }
                    }


                    nRet = m_Operator.SetEnumValue("TriggerMode", (uint)trigMode);
                    nRet = m_Operator.SetEnumValue("TriggerSource", (uint)trigSource);
                    if (nRet != 0)
                    {
                        throw new Exception("SetTriggerMode Fail");
                    }
                }
                
                catch (Exception ex) { throw new Exception("TriggerType(SET) error: " + ex.Message); }
            }
        }

        //public bool TriggerMode
        //{
        //    get
        //    {
        //        int nRet = -1;
        //        bool result = false;
        //        uint tm = 0;
        //        try
        //        {
        //            if (CameraCount == 0)
        //            {
        //                return result;
        //            }
        //            nRet = m_Operator.GetEnumValue("TriggerMode", ref tm);
        //            if (nRet != 0)
        //            {
        //                if ((TriggerModeEnum)tm == TriggerModeEnum.TRIGGER_ON)
        //                { result = true; }
        //            }
        //        }
        //        catch (Exception ex) { throw new Exception("TriggerMode Get error: " + ex.Message); }
        //        return result;
        //    }
        //    set
        //    {
        //        int nRet = -1;
        //        string result = string.Empty;
        //        try
        //        {
        //            TriggerModeEnum tm = TriggerModeEnum.TRIGGER_OFF;
        //            if (value) tm = TriggerModeEnum.TRIGGER_ON;
        //            nRet = m_Operator.SetEnumValue("TriggerMode", (uint)tm);
        //            if (nRet != 0)
        //            {
        //                throw new Exception("SetTriggerMode Fail");
        //            }
        //        }
        //        catch (Exception ex) { throw new Exception("TriggerMode(SET) error: " + ex.Message); }
        //    }
        //}

        //public CameraSettings.TriggerSourceEnum TriggerSource
        //{
        //    get
        //    {
        //        int nRet = -1;
        //        CameraSettings.TriggerSourceEnum result = CameraSettings.TriggerSourceEnum.M_AUX_IO0;
        //        uint trigSource = 0;
        //        try
        //        {
        //            if (CameraCount == 0)
        //            {
        //                return result;
        //            }
        //            nRet = m_Operator.GetEnumValue("TriggerSource", ref trigSource);
        //            if (nRet != 0)
        //            {
        //                switch ((HikTriggerSourceEnum)trigSource)
        //                {
        //                    case HikTriggerSourceEnum.Software:
        //                        {
        //                            result = CameraSettings.TriggerSourceEnum.M_SOFTWARE;
        //                            break;
        //                        }
        //                    case HikTriggerSourceEnum.Line0:
        //                        {
        //                            result = CameraSettings.TriggerSourceEnum.M_AUX_IO0;
        //                            break;
        //                        }
        //                    case HikTriggerSourceEnum.Line1:
        //                        {
        //                            result = CameraSettings.TriggerSourceEnum.M_AUX_IO1;
        //                            break;
        //                        }
        //                    case HikTriggerSourceEnum.Line2:
        //                        {
        //                            result = CameraSettings.TriggerSourceEnum.M_AUX_IO2;
        //                            break;
        //                        }
        //                    default: break;
        //                }
        //            }
        //        }
        //        catch (Exception ex) { throw new Exception("TriggerSource Get error: " + ex.Message); }
        //        return result;
        //    }
        //    set
        //    {
        //        int nRet = -1;
        //        string result = string.Empty;
        //        try
        //        {
        //            HikTriggerSourceEnum ts = HikTriggerSourceEnum.Software;
        //            switch (value)
        //            {
        //                case CameraSettings.TriggerSourceEnum.M_SOFTWARE:
        //                case CameraSettings.TriggerSourceEnum.M_SOFTWARE1:
        //                case CameraSettings.TriggerSourceEnum.M_SOFTWARE2:
        //                case CameraSettings.TriggerSourceEnum.M_SOFTWARE3:
        //                    {
        //                        ts = HikTriggerSourceEnum.Software;
        //                        break;
        //                    }
        //                case CameraSettings.TriggerSourceEnum.M_AUX_IO0:
        //                    {
        //                        ts = HikTriggerSourceEnum.Line0;
        //                        break;
        //                    }
        //                case CameraSettings.TriggerSourceEnum.M_AUX_IO1:
        //                    {
        //                        ts = HikTriggerSourceEnum.Line1;
        //                        break;
        //                    }
        //                case CameraSettings.TriggerSourceEnum.M_AUX_IO2:
        //                    {
        //                        ts = HikTriggerSourceEnum.Line2;
        //                        break;
        //                    }
        //                case CameraSettings.TriggerSourceEnum.M_AUX_IO3:
        //                    {
        //                        ts = HikTriggerSourceEnum.Line3;
        //                        break;
        //                    }
        //                default: break;
        //            }
        //            nRet = m_Operator.SetEnumValue("TriggerSource", (uint)ts);
        //            if (nRet != 0)
        //            {
        //                throw new Exception("SetTriggerSource Fail");
        //            }
        //        }
        //        catch (Exception ex) { throw new Exception("TriggerSource(SET) error: " + ex.Message); }
        //    }
        //}



        public override int TriggerDelay
        {
            get
            {
                int nRet;
                string sErr = string.Empty;
                int result = 0;
                try
                {
                    if (CameraCount == 0)
                    {
                        return result;
                    }
                    float fVal = 0;
                    nRet = m_Operator.GetFloatValue("TriggerDelay", ref fVal);
                    if (nRet == 0)
                    {
                        result = Convert.ToInt32(fVal);
                    }
                }
                catch (Exception ex)
                {
                    sErr = string.Format("HikSdkUsb3Cam TriggerDelay Get error: " + ex.Message);
                    JPTUtility.Logger.doLog(sErr);
                }
                return result;
            }
            set
            {
                string sErr = string.Empty;
                int nRet;
                float fVal = Convert.ToSingle(value);
                try
                {
                    if (CameraCount == 0)
                    {
                        return;
                    }

                    nRet = m_Operator.SetFloatValue("TriggerDelay", fVal);
                    if (nRet != 0)
                    {
                        sErr = "SetTriggerDelay Fail";
                    }

                }
                catch (Exception ex)
                {
                    sErr = string.Format("HikSdkUsb3Cam TriggerDelay Set error: " + ex.Message);
                    JPTUtility.Logger.doLog(sErr);
                }
            }
        }

        public override int TriggerDelayMin
        {
            get
            {
                int nRet;
                string sErr = string.Empty;
                int result = 0;
                try
                {
                    if (CameraCount == 0)
                    {
                        return result;
                    }
                    float fVal = 0;
                    nRet = m_Operator.GetFloatValueMin("TriggerDelay", ref fVal);
                    if (nRet == 0)
                    {
                        result = Convert.ToInt32(fVal);
                    }
                }
                catch (Exception ex)
                {
                    sErr = string.Format("HikSdkUsb3Cam TriggerDelayMinimum error: " + ex.Message);
                    JPTUtility.Logger.doLog(sErr);
                }
                return result;
            }
        }

        public override int TriggerDelayMax
        {
            get
            {
                int nRet;
                string sErr = string.Empty;
                int result = 0;
                try
                {
                    if (CameraCount == 0)
                    {
                        return result;
                    }
                    float fVal = 0;
                    nRet = m_Operator.GetFloatValueMax("TriggerDelay", ref fVal);
                    if (nRet == 0)
                    {
                        result = Convert.ToInt32(fVal);
                    }
                }
                catch (Exception ex)
                {
                    sErr = string.Format("HikSdkUsb3Cam TriggerDelayMaximum error: " + ex.Message);
                    JPTUtility.Logger.doLog(sErr);
                }
                return result;
            }
        }

        public override int Width
        {
            get
            {
                string sErr = string.Empty;
                int nRet = -1;
                int result = 0;
                try
                {
                    uint tempV = 0;
                    if (CameraCount == 0)
                    {
                        return result;
                    }
                    nRet = m_Operator.GetIntValue("Width", ref tempV);
                    if (nRet == 0)
                    {
                        result = Convert.ToInt32(tempV);
                    }
                }
                catch (Exception ex)
                {
                    sErr = string.Format("HikSdkUsb3Cam Width error: " + ex.Message);
                    JPTUtility.Logger.doLog(sErr);
                }
                return result;
            }
            set
            {
                string sErr = string.Empty;
                int nRet;
                try
                {
                    uint iVal = Convert.ToUInt32(value);
                    if (CameraCount == 0)
                    {
                        return;
                    }
                    nRet = m_Operator.SetIntValue("Width", iVal);
                    if (nRet != 0)
                    {
                        sErr = "SetWidth Fail";
                    }

                }
                catch (Exception ex)
                {
                    sErr = string.Format("HikSdkUsb3Cam Width (SET) error: " + ex.Message);
                    JPTUtility.Logger.doLog(sErr);
                }
            }
        }

        public override int WidthMinimum
        {
            get
            {
                int nRet;
                string sErr = string.Empty;
                int result = 0;
                try
                {
                    if (CameraCount == 0)
                    {
                        return result;
                    }
                    uint fVal = 0;
                    nRet = m_Operator.GetIntValueMin("Width", ref fVal);
                    if (nRet == 0)
                    {
                        result = Convert.ToInt32(fVal);
                    }
                }
                catch (Exception ex)
                {
                    sErr = string.Format("HikSdkUsb3Cam WidthMinimum error: " + ex.Message);
                    JPTUtility.Logger.doLog(sErr);
                }
                return result;
            }
        }

        public override int WidthMaximum
        {
            get
            {
                int nRet;
                string sErr = string.Empty;
                int result = 0;
                try
                {
                    if (CameraCount == 0)
                    {
                        return result;
                    }
                    uint iValue = 0;
                    nRet = m_Operator.GetIntValueMax("Width", ref iValue);
                    if (nRet == 0)
                    {
                        result = Convert.ToInt32(iValue);
                    }
                }
                catch (Exception ex)
                {
                    sErr = string.Format("HikSdkUsb3Cam WidthMaximum error: " + ex.Message);
                    JPTUtility.Logger.doLog(sErr);
                }
                return result;
            }
        }

        public override int Height
        {
            get
            {
                string sErr = string.Empty;
                int nRet = -1;
                int result = 0;
                try
                {
                    uint tempV = 0;
                    if (CameraCount == 0)
                    {
                        return result;
                    }
                    nRet = m_Operator.GetIntValue("Height", ref tempV);
                    if (nRet == 0)
                    {
                        result = Convert.ToInt32(tempV);
                    }
                }
                catch (Exception ex)
                {
                    sErr = string.Format("HikSdkUsb3Cam Height error: " + ex.Message);
                    JPTUtility.Logger.doLog(sErr);
                }
                return result;
            }
            set
            {
                string sErr = string.Empty;
                int nRet;
                try
                {
                    uint iVal = Convert.ToUInt32(value);
                    if (CameraCount == 0)
                    {
                        return;
                    }
                    nRet = m_Operator.SetIntValue("Height", iVal);
                    if (nRet != 0)
                    {
                        sErr = "SetHeight Fail";
                    }

                }
                catch (Exception ex)
                {
                    sErr = string.Format("HikSdkUsb3Cam Height (SET) error: " + ex.Message);
                    JPTUtility.Logger.doLog(sErr);
                }
            }
        }

        public override int HeightMinimum
        {
            get
            {
                int nRet;
                string sErr = string.Empty;
                int result = 0;
                try
                {
                    if (CameraCount == 0)
                    {
                        return result;
                    }
                    uint iVal = 0;
                    nRet = m_Operator.GetIntValueMin("Height", ref iVal);
                    if (nRet == 0)
                    {
                        result = Convert.ToInt32(iVal);
                    }
                }
                catch (Exception ex)
                {
                    sErr = string.Format("HikSdkUsb3Cam HeightMinimum error: " + ex.Message);
                    JPTUtility.Logger.doLog(sErr);
                }
                return result;
            }
        }

        public override int HeightMaximum
        {
            get
            {
                int nRet;
                string sErr = string.Empty;
                int result = 0;
                try
                {
                    if (CameraCount == 0)
                    {
                        return result;
                    }
                    uint iValue = 0;
                    nRet = m_Operator.GetIntValueMax("Height", ref iValue);
                    if (nRet == 0)
                    {
                        result = Convert.ToInt32(iValue);
                    }
                }
                catch (Exception ex)
                {
                    sErr = string.Format("HikSdkUsb3Cam HeightMaximum error: " + ex.Message);
                    JPTUtility.Logger.doLog(sErr);
                }
                return result;
            }
        }

        public override int OffsetX
        {
            get
            {
                string sErr = string.Empty;
                int nRet = -1;
                int result = 0;
                try
                {
                    uint tempV = 0;
                    if (CameraCount == 0)
                    {
                        return result;
                    }
                    nRet = m_Operator.GetIntValue("OffsetX", ref tempV);
                    if (nRet == 0)
                    {
                        result = Convert.ToInt32(tempV);
                    }
                }
                catch (Exception ex)
                {
                    sErr = string.Format("HikSdkUsb3Cam OffSetX error: " + ex.Message);
                    JPTUtility.Logger.doLog(sErr);
                }
                return result;
            }
            set
            {
                string sErr = string.Empty;
                int nRet;
                try
                {
                    uint iVal = Convert.ToUInt32(value);
                    if (CameraCount == 0)
                    {
                        return;
                    }
                    nRet = m_Operator.SetIntValue("OffsetX", iVal);
                    if (nRet != 0)
                    {
                        sErr = "SetOffSetX Fail";
                    }

                }
                catch (Exception ex)
                {
                    sErr = string.Format("HikSdkUsb3Cam OffSetX (SET) error: " + ex.Message);
                    JPTUtility.Logger.doLog(sErr);
                }
            }
        }

        public override int OffsetY
        {
            get
            {
                string sErr = string.Empty;
                int nRet = -1;
                int result = 0;
                try
                {
                    uint tempV = 0;
                    if (CameraCount == 0)
                    {
                        return result;
                    }
                    nRet = m_Operator.GetIntValue("OffsetY", ref tempV);
                    if (nRet == 0)
                    {
                        result = Convert.ToInt32(tempV);
                    }
                }
                catch (Exception ex)
                {
                    sErr = string.Format("HikSdkUsb3Cam OffSetY error: " + ex.Message);
                    JPTUtility.Logger.doLog(sErr);
                }
                return result;
            }
            set
            {
                string sErr = string.Empty;
                int nRet;
                try
                {
                    uint iVal = Convert.ToUInt32(value);
                    if (CameraCount == 0)
                    {
                        return;
                    }
                    nRet = m_Operator.SetIntValue("OffsetY", iVal);
                    if (nRet != 0)
                    {
                        sErr = "SetOffSetY Fail";
                    }

                }
                catch (Exception ex)
                {
                    sErr = string.Format("HikSdkUsb3Cam OffSetY (SET) error: " + ex.Message);
                    JPTUtility.Logger.doLog(sErr);
                }
            }
        }

        public override bool ReverseX
        {
            get
            {
                int nRet = -1;
                bool result = false;
                try
                {
                    if (CameraCount == 0)
                    {
                        return result;
                    }
                    nRet = m_Operator.GetBoolValue("ReverseX", ref result);
                    if (nRet != 0)
                    {
                        //
                    }
                }
                catch (Exception ex) { throw new Exception("ReverseX Get error: " + ex.Message); }
                return result;
            }
            set
            {
                int nRet = -1;
                try
                {
                    nRet = m_Operator.SetBoolValue("ReverseX", value);
                    if (nRet == 0)
                    {
                        //Success
                    }
                }
                catch (Exception ex) { throw new Exception("ReverseX(SET) error: " + ex.Message); }
            }
        }

        public override bool ReverseY
        {
            get
            {
                int nRet = -1;
                bool result = false;
                try
                {
                    if (CameraCount == 0)
                    {
                        return result;
                    }
                    nRet = m_Operator.GetBoolValue("ReverseY", ref result);
                    if (nRet != 0)
                    {
                        //
                    }
                }
                catch (Exception ex) { throw new Exception("ReverseY Get error: " + ex.Message); }
                return result;
            }
            set
            {
                int nRet = -1;
                try
                {
                    nRet = m_Operator.SetBoolValue("ReverseY", value);
                    if (nRet == 0)
                    {
                        //Success
                    }
                }
                catch (Exception ex) { throw new Exception("ReverseY(SET) error: " + ex.Message); }
            }
        }


    }
}
