﻿using System;
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
        public string BuildTypeId { get; set; }
		public bool IsRunning {get;set;}
		public int PercentageComplete { get; set; }


        public Build(TeamCityApi api, XElement xml) : base(api, xml)
        {
            Succeeded = xml.Attribute("status").Value == "SUCCESS";
            LogHtmlHRef = (string)xml.Attribute("webUrl");
            BuildTypeId = (string)xml.Attribute("buildTypeId");

            if (xml.Attribute("startDate") != null)
            {
                var startDate = xml.Attribute("startDate").Value;
                StartTime = TeamCityUtils.ParseTime(startDate);
            }

			if (xml.Attribute ("running") != null)
			{
				IsRunning = bool.Parse (xml.Attribute ("running").Value);
			}
        }

        public BuildDetails GetBuildDetails()
        {
            return new BuildDetails(Api, Api.TeamCityRestApiCall(HRef));
        }
    }


    public class BuildDetails : Build
    {
		public string BuildTypeId { get; set; }
        public string StatusText { get; set; }
        public string ProjectName { get; set; }
        public DateTime? EndTime { get; set; }

        public BuildDetails(TeamCityApi api, XElement xml) : base(api, xml)
        {
            var statusTextElement = xml.Descendants("statusText").FirstOrDefault();
            if (statusTextElement != null)
            {
                StatusText = statusTextElement.Value;
            }

            var startTimeNode = xml.Descendants("startDate").FirstOrDefault();
            if (startTimeNode != null)
            {
                StartTime = TeamCityUtils.ParseTime(startTimeNode.Value);
            }

            var endTimeNode = xml.Descendants("finishDate").FirstOrDefault();
            if (endTimeNode != null)
            {
                EndTime = TeamCityUtils.ParseTime(endTimeNode.Value);
            }

            var buildTypeNode = xml.Descendants("buildType").First();

			BuildTypeId = (string)buildTypeNode.Attribute("id");
            Name = (string)buildTypeNode.Attribute("name");
            ProjectName = (string)buildTypeNode.Attribute("projectName");
        }
    }
}
