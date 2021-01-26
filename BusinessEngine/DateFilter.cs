using System;
using System.Collections.Generic;
using System.Text;

namespace BusinessEngine
{
    public class DateFilter
    {
        //from == to : select only one
        //from & to == 0 : no filter = anytime
        public int YearFrom, YearTo;
        public int MonthFrom, MonthTo;
        public int DayFrom, DayTo;



        public DateFilter SetYear(int year) {YearFrom = YearTo = year; return this;}
        public DateFilter SetMonth(int month) { MonthFrom = MonthTo = month; return this; }
        public DateFilter SetDay(int day) { DayFrom = DayTo = day; return this; }

        public DateFilter SetYearTerm(int yearFrom, int yearTo)
        {
            this.YearFrom = yearFrom;
            this.YearTo = yearTo;
            return this;
        }
        public DateFilter SetMonthTerm(int monthFrom, int monthTo)
        {
            this.MonthFrom = monthFrom;
            this.MonthTo = monthTo;
            return this;
        }
        public DateFilter SetDayTerm(int dayFrom, int dayTo)
        {
            this.DayFrom = dayFrom;
            this.DayTo= dayTo;
            return this;
        }
        public int[][] ToArray()
        {
            return new int[][]
            {
                new int[]{DayFrom,  DayTo},
                new int[]{MonthFrom, MonthTo },
                new int[]{YearFrom, YearTo}
            };
        }
        public void Clear()
        {
            YearFrom = YearTo = MonthFrom = MonthTo = DayFrom = DayTo = 0;
        }


    }
}
