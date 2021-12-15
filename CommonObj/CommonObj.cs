using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KCSDK;

using System.IO;
using System.Globalization;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using NLog;



namespace CommonObj
{
    public struct strKerfData
    {
        public int Shift;
        public int Width;
        public int MaxWidth;
        public int ChippingArea;
        public int ChippingSize;
        public int ChippingHalfWidth;
        public int LeftDistance;
        public int RightDistance;

        public strKerfData(int mShift = 0, int mWidth = 0, int mMaxWidth = 0, int mChippingArea = 0,
            int mChippingSize = 0, int mChippingHalfWidth = 0, int mLeftDistance = 0, int mRightDistance = 0)
        {
            Shift = mShift;
            Width = mWidth;
            MaxWidth = mMaxWidth;
            ChippingArea = mChippingArea;
            ChippingSize = mChippingSize;
            ChippingHalfWidth = mChippingHalfWidth;
            LeftDistance = mLeftDistance;
            RightDistance = mRightDistance;
        }
    }

    public struct strKnifeData
    {
        public string sToolName;//刀片序號
        public string sType;//刀片型號
        public double dWidth;//刀寬
        public double dKnifeDiameter;//刀外徑
        public double dKnifeRemain;//刀露量
        public double dRealKnifeRemain;//實際刀露量
        public int iRealMeasureZPos;//接觸式量測高度
        public int iSimMeasureZPos;//非接觸式量測高度
        public double dRealMotorDistance;//實際行走距離
        public double dRealSawDistance;//實際切割距離
        public double dRemainMotorDistance;//剩餘行走距離
        public double dRemainSawDistance;//剩餘切割距離
        public double dLimit;//里程極限
        public double dFlange;//法藍盤外徑

        public strKnifeData(string msToolName = "", string msType = "", double mdWidth = 0, double mdKnifeDiameter = 0,
        double mdKnifeRemain = 0, double mdRealKnifeRemain = 0, int miRealMeasureZPos = 0, int miSimMeasureZPos = 0,
        double mdRealMotorDistance = 0, double mdRealSawDistance = 0, double mdRemainMotorDistance = 0,
            double mdRemainSawDistance = 0, double mdLimit = 0, double mdFlange = 0)
        {
            sToolName = msToolName;
            sType = msType;
            dWidth = mdWidth;
            dKnifeDiameter = mdKnifeDiameter;
            dKnifeRemain = mdKnifeRemain;
            dRealKnifeRemain = mdRealKnifeRemain;
            iRealMeasureZPos = miRealMeasureZPos;
            iSimMeasureZPos = miSimMeasureZPos;
            dRealMotorDistance = mdRealMotorDistance;
            dRealSawDistance = mdRealSawDistance;
            dRemainMotorDistance = mdRemainMotorDistance;
            dRemainSawDistance = mdRemainSawDistance;
            dLimit = mdLimit;
            dFlange = mdFlange;
        }
    }

    public enum SparkType
    {
        enZ1 = 0,
        enZ2,
    };

    public delegate void HandlerVacuumAction(bool bOn);

    public delegate void HandlerDestroyAction(bool bOn);

    //By Kent 20170729
    public delegate bool HandlerGetVacaStatusAction();

    public delegate void HandlerUpdateKerfData(String Channel, String Pos, int LineNo, int KerfShift, bool Reset);

    public delegate void SafeDoorDelegateActivated();

    #region NLog
    public enum enLoggerType
    {
        TRACE,
        DEBUG,
        INFO,
        WARN,
        ERROR,
        FATAL
    }

    public class NLogger
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        public static void Debug(string strMsg,
            [System.Runtime.CompilerServices.CallerMemberName] string strMemberName = "",
            [System.Runtime.CompilerServices.CallerFilePath] string strSrcFilePath = "",
            [System.Runtime.CompilerServices.CallerLineNumber] int nSourceLineNumber = 0)
        {
            OutputToDebugLog(enLoggerType.DEBUG, strMsg, strMemberName, strSrcFilePath, nSourceLineNumber);
        }

        public static void Info(string strMsg,
            [System.Runtime.CompilerServices.CallerMemberName] string strMemberName = "",
            [System.Runtime.CompilerServices.CallerFilePath] string strSrcFilePath = "",
            [System.Runtime.CompilerServices.CallerLineNumber] int nSourceLineNumber = 0)
        {
            OutputToDebugLog(enLoggerType.INFO, strMsg, strMemberName, strSrcFilePath, nSourceLineNumber);
        }

        public static void Warn(string strMsg,
            [System.Runtime.CompilerServices.CallerMemberName] string strMemberName = "",
            [System.Runtime.CompilerServices.CallerFilePath] string strSrcFilePath = "",
            [System.Runtime.CompilerServices.CallerLineNumber] int nSourceLineNumber = 0)
        {
            OutputToDebugLog(enLoggerType.WARN, strMsg, strMemberName, strSrcFilePath, nSourceLineNumber);
        }

        public static void Error(string strMsg,
            [System.Runtime.CompilerServices.CallerMemberName] string strMemberName = "",
            [System.Runtime.CompilerServices.CallerFilePath] string strSrcFilePath = "",
            [System.Runtime.CompilerServices.CallerLineNumber] int nSourceLineNumber = 0)
        {
            OutputToDebugLog(enLoggerType.ERROR, strMsg, strMemberName, strSrcFilePath, nSourceLineNumber);
        }

        public static void OutputToDebugLog(
            enLoggerType logType,
            string strMsg,
            string strMemberName,
            string strSrcFilePath,
            int nSourceLineNumber = 0)
        {
            var culture = new CultureInfo("en-US");
            string strFormat = "yyyy/MM/dd HH:mm:ss.fff";

            string strFilename = Path.GetFileName(strSrcFilePath);
            string strNowTime = DateTime.Now.ToString(strFormat, culture);

            StackTrace st = new StackTrace();
            StackFrame sf = st.GetFrame(1);

            string strCurrentMethodName = sf.GetMethod().Name;

            if (strCurrentMethodName.Length < 5) strCurrentMethodName += " ";

            string strOutput = string.Format("{0} {1} - <{2}({3})({4})><{5}>",
                strNowTime, strCurrentMethodName, strFilename, nSourceLineNumber, strMemberName, strMsg);

            if (logType == enLoggerType.TRACE)
            {
                logger.Trace(strOutput);
            }
            else if (logType == enLoggerType.DEBUG)
            {
                logger.Debug(strOutput);
            }
            else if (logType == enLoggerType.INFO)
            {
                logger.Info(strOutput);
            }
            else if (logType == enLoggerType.WARN)
            {
                logger.Warn(strOutput);
            }
            else if (logType == enLoggerType.ERROR)
            {
                logger.Error(strOutput);
            }
            else
            {
                logger.Fatal(strOutput);
            }
        }
    }
    #endregion

    public class ServerEventArgs : EventArgs
    {
        public string strResult;
    }

    public enum enMsgDialogType
    {
        NOTIFY = 0,
        WARNING,
        ERROR
    }
}
