using System;
using System.Collections.Generic;
using System.Text;

namespace SSRD.AdminUI.Template.Models
{
    public class TimeRangeStatisticsModel
    {
        public DateTime DateTime { get; set; }
        public int Value { get; set; }

        public TimeRangeStatisticsModel(DateTime dateTime, int value)
        {
            DateTime = dateTime;
            Value = value;
        }
    }
}
