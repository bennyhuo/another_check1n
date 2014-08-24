using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace check1n
{
    class LogItem
    {
        private int index;
        private String starttime;
        private String endtime;
        private int lasttime;

        public int Index
        {
            get
            {
                return index;
            }
            set
            {
                index = value;
            }
        }

        public String StartTime
        {
            get
            {
                return starttime;
            }
            set
            {
                starttime = value;
            }
        }

        public String EndTime
        {
            get
            {
                return endtime;
            }
            set
            {
                endtime = value;
            }
        }

        public int LastTime
        {
            get
            {
                if (lasttime == -1)
                {
                    DateTime st = DateTime.Parse(starttime);
                    DateTime et = DateTime.Parse(endtime);
                    lasttime = et.Hour - st.Hour;
                    lasttime +=(int) Math.Round((et.Minute - st.Minute) / 60.0);
                }
                return lasttime;
            }
            set
            {
                lasttime = value;
            }
        }

        public LogItem()
        {
            lasttime = -1;
        }

        public LogItem(int index, String starttime, String endtime, int lasttime)
        {
            this.index = index;
            this.endtime = endtime;
            this.starttime = starttime;
            this.lasttime = lasttime;
        }

        override public  string ToString() 
        {
            return index + "\t" + starttime + "\t" + endtime + "\t" + lasttime;
        }

        public static String getHeadString()
        {
            return "序号\t签到时间\t离开时间\t持续时间";
        }
    }
}
