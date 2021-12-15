using System;

namespace Saw
{
    class CheckBeforeStart
    {
        private bool bZSparkTestDone = false;
        private bool bZNCTestDone = false;
        private bool bZNCTestSecondDone = false;
        private bool bZKerfCheckDone = false;

        public bool ZSparkTestDone
        {
            get { return bZSparkTestDone; }

            set { bZSparkTestDone = value; }
        }

        public bool ZNCTestDone
        {
            get { return bZNCTestDone; }

            set { bZNCTestDone = value; }
        }

        public bool ZNCTestSecondDone
        {
            get { return bZNCTestSecondDone; }

            set { bZNCTestSecondDone = value; }
        }

        public bool ZKerfCheckDone
        {
            get { return bZKerfCheckDone; }

            set { bZKerfCheckDone = value; }
        }

        public bool Finish()
        {
            if (bZSparkTestDone && bZNCTestDone && bZKerfCheckDone)
                return true;
            else
                return false;
        }

        public void Reset()
        {
            bZSparkTestDone = false;
            bZNCTestDone = false;
            bZKerfCheckDone = false;
            bZNCTestSecondDone = false;
        }
    }
}
