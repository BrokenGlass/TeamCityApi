using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Globalization;

namespace TeamCity
{
    public class Build : TeamCityApiResult
    {
        public bool Succeeded { get; set; }
        public DateTime StartTime { get; set; }


        public Build(TeamCityApi api, XElement xml) : base(api, xml)
        {
            Succeeded = xml.Attribute("status").Value == "SUCCESS";
            var startDate = xml.Attribute("startDate").Value;
            DateTime startTime;
            DateTime.TryParseExact(startDate, "yyyyMMddTHHmmsszz00", CultureInfo.InvariantCulture, DateTimeStyles.None, out startTime);
            StartTime = startTime;
        }
    }
}
