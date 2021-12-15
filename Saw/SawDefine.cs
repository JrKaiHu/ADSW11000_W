using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Saw
{
    internal enum enSpeedMode
    {
        掃瞄速度,
        切割後速度,
        切割速度,
        刀痕速度
    }

    internal enum enUseSpidleType
    {
        雙刀,
        Z1單刀,
        Z2單刀
    }

    internal enum enSpindleStatus
    {
        Stop,
        Starting,
        Run,
        Closing
    };

    internal enum enCleanType
    {
        入料前清洗,
        掃靶前清洗,
        切割後清洗
    }

    internal enum enSpindleParamSel
    {
        實際轉數,
        參考轉數,
        驅動器負載,
        主軸負載
    }

    public enum enSpindleSel
    {
        NONE,
        Z1,
        Z2
    }

    internal enum enSpindleCtrlType
    {
        預設訊號控制_IO,
        進階訊號控制_IO,
        通訊控制_RS232
    }

    internal enum enScanTargetMethod
    {
        四點掃靶,
        半靶掃靶,
        全靶掃靶_口字形,
        全靶掃靶_N字形,
        三點掃靶,
        即看即切
    }

    internal enum enLoadMode
    {
        自動,
        手動
    }

    internal enum enUnLoadMode
    {
        自動,
        手動
    }

    public enum enValveStatus
    {
        RUNNING,
        COMPLETE,
        TIMEOUT
    }

    public enum enValveActMode
    {
        NORMAL,
        NOCHECK,
        AUTOOFF
    }

    internal enum enCheckSel
    {
        無,
        里程數檢查,
        片數檢查
    }

    internal enum enHomeSpindleStatus
    {
        主軸狀態初始,
        開啟雙軸_Z1運轉中_Z2運轉中,
        開啟雙軸_Z1運轉中_Z2停止,
        開啟雙軸_Z1停止_Z2運轉中,
        開啟雙軸_Z1停止_Z2停止,
        開啟單軸_Z1運轉中,
        開啟單軸_Z1停止,
        開啟單軸_Z2運轉中,
        開啟單軸_Z2停止
    }

    internal enum enFocusMode
    {
        首片,
        每片
    }

    internal enum enCheckKerfSel
    {
        NONE,
        EDGE,
        DESIGNATED,
        ALL
    }

    internal enum enTeachSel
    {
        Z1SparkTest = 1,
        Z1NonContactTest,
        Z1BaseLineTeach,
        Z2SparkTest,
        Z2NonContactTest,
        Z2BaseLineTeach,
        CutLineTeach,
        TargetTeach
    }

    internal enum enCutSequenceCheck
    {
        OK,
        CutSequenceOutOfRange,
        CutLineOutOfRange
    }

    internal enum enTeachActiionSel
    {
        無動作,
        接觸測高,
        非接觸測高,
        基準線校正,
        切割道學習,
        靶點學習,
        換刀,
        圓心校正
    }

    internal enum enSecondSawState
    {
        NoUse,
        First,
        Second,
    }

    public enum enCutWaterSel
    {
        SHOWER,
        SPRAY
    }

    internal enum enLimitSel
    {
        MIN,
        MAX
    }

    internal class SpindleStatus
    {
        public int MinRefSpeed;
        public int MinActSpeed;
        public float MaxAMPLoad;
        public float MaxMotorLoad;

        public SpindleStatus()
        {
            MinActSpeed = 60000;
            MinRefSpeed = 60000;
            MaxAMPLoad = 0;
            MaxMotorLoad = 0;
        }

        public void Reset()
        {
            MinActSpeed = 60000;
            MinRefSpeed = 60000;
            MaxAMPLoad = 0;
            MaxMotorLoad = 0;
        }
    }

    internal enum SeeAndCutPointSel
    {
        NEAR,
        FAR,
    }
}
