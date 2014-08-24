using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace check1n
{
    class SimpleDate
    {
        private int year;
        private int month;
        private int day;
        public int Year
        {
            get
            {
                return year;
            }
            set
            {
                year = value;
            }
        }

        public int Month
        {
            get
            {
                return month;
            }
            set
            {
                month = value;
            }
        }

        public int Day
        {
            get
            {
                return day;
            }
            set
            {
                day = value;
            }
        }

        public bool isBefore(SimpleDate sd)
        {
            if (sd.Year < Year ||
                (sd.Year == Year && sd.Month < Month) ||
                (sd.Year == Year && sd.Month == Month && sd.Day < Day))
            {
                return true;
            }
            return false;
        }

        public bool isAfter(SimpleDate sd)
        {
            if (sd.Year > Year ||
                (sd.Year == Year && sd.Month > Month) ||
                (sd.Year == Year && sd.Month == Month && sd.Day > Day))
            {
                return true;
            }
            return false;
        }

        public bool equals(SimpleDate sd)
        {
            if (sd.Year == Year && sd.Month == Month && sd.Day == Day)
            {
                return true;
            }
            return false;
        }

        /**
         * -1 before; 0 equal ; 1 after
        */
        public int compare(SimpleDate sd)
        {
            if (isBefore(sd))
            {
                return -1;
            }
            else if (equals(sd))
            {
                return 0;
            }
            else
            {
                return 1;
            }
        }
    }
}
