using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.Services
{
    public static class DateTimeString
    {
        public static DateTime? TryParsingDate(string date, bool isTime)
        {
            DateTime? dateTimeNull = null;

            DateTime dateTime = DateTime.Now;
            var canParse = DateTime.TryParse(date, out dateTime);
            if (canParse)
            {
                dateTime = isTime ?
                    new DateTime(2000, 01, 01, dateTime.Hour, dateTime.Minute, 0) :
                    new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, 0, 0, 0);
            }
            return canParse ? dateTime : dateTimeNull;
        }
    }
}
