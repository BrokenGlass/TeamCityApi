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
        public DateTime? StartTime { get; set; }
        public string LogHtmlHRef { get; set; }


        public Build(TeamCityApi api, XElement xml) : base(api, xml)
        {
            Succeeded = xml.Attribute("status").Value == "SUCCESS";
            LogHtmlHRef = (string)xml.Attribute("webUrl");

            if (xml.Attribute("startDate") != null)
            {
                var startDate = xml.Attribute("startDate").Value;
                DateTime startTime;
                DateTime.TryParseExact(startDate, "yyyyMMddTHHmmsszz00", CultureInfo.InvariantCulture, DateTimeStyles.None, out startTime);
                StartTime = startTime;
            }
        }

        public BuildDetails GetBuildDetails()
        {
            return new BuildDetails(Api, Api.TeamCityRestApiCall(HRef));
        }
    }

    public class BuildDetails : Build
    {
        public string StatusText { get; set; }
        public string ProjectName { get; set; }

        public BuildDetails(TeamCityApi api, XElement xml) : base(api, xml)
        {
            var statusTextElement = xml.Descendants("statusText").FirstOrDefault();
            if (statusTextElement != null)
            {
                StatusText = statusTextElement.Value;
            }

            var buildTypeNode = xml.Descendants("buildType").First();

            Name = (string)buildTypeNode.Attribute("name");
            ProjectName = (string)buildTypeNode.Attribute("projectName");
        }
    }
}
