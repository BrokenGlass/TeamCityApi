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
		public string StatusText { get; set; }
        public DateTime? StartTime { get; set; }
		public DateTime? EndTime { get; set; }
        public string LogHtmlHRef { get; set; }
        public string BuildTypeId { get; set; }
		public bool IsRunning {get;set;}
		public int PercentageComplete { get; set; }


        public Build(TeamCityApi api, XElement xml) : base(api, xml)
        {
            Succeeded = xml.Attribute("status").Value == "SUCCESS";
            LogHtmlHRef = (string)xml.Attribute("webUrl");
            BuildTypeId = (string)xml.Attribute("buildTypeId");
			PercentageComplete = xml.Attribute("percentageComplete")  != null ? (int)xml.Attribute("percentageComplete") : 0;

			if (xml.Attribute ("startDate") != null)
			{
				var startDate = xml.Attribute ("startDate").Value;
				StartTime = TeamCityUtils.ParseTime (startDate);
			} 
			else if (xml.Element ("startDate") != null)
			{
				var startDate = xml.Element ("startDate").Value;
				StartTime = TeamCityUtils.ParseTime (startDate);
			}

			if(xml.Element ("finishDate") != null)
			{
				var endDate = xml.Element ("finishDate").Value;
				EndTime = TeamCityUtils.ParseTime (endDate);
			}

			if(xml.Element ("statusText") != null)
			{
				StatusText = xml.Element ("statusText").Value;
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
		public string ProjectName { get; set; }
        public RunningInfo RunInfo { get; set; }
		public List<Build> SnapshotDependencies { get; private set; }

        public BuildDetails(TeamCityApi api, XElement xml) : base(api, xml)
        {
            var statusTextElement = xml.Descendants("statusText").FirstOrDefault();
            if (statusTextElement != null)
            {
                StatusText = statusTextElement.Value;
            }

			var runningInfo = xml.Descendants("running-info").FirstOrDefault();
			if (runningInfo != null)
			{
				RunInfo = new RunningInfo (runningInfo);
			}

			var snapshotDependencies = xml.Descendants("snapshot-dependencies").FirstOrDefault();
			if (snapshotDependencies != null)
			{
				SnapshotDependencies = snapshotDependencies.Descendants ("build")
														   .Select (b => new Build (api, b))
														   .ToList ();
			} 
			else
			{
				SnapshotDependencies = new List<Build> ();
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

	public class RunningInfo
	{
		public int PercentageComplete {get; set;}
		public int ElapsedSeconds {get; set;}
		public int EstimatedTotalSeconds {get; set;}
		public string CurrentStageText {get; set;}

		public RunningInfo()
		{
		}

		public RunningInfo(XElement node)
		{
			PercentageComplete = (int)node.Attribute ("percentageComplete");
			ElapsedSeconds = (int)node.Attribute ("elapsedSeconds");
			EstimatedTotalSeconds = (int)node.Attribute ("estimatedTotalSeconds");
			CurrentStageText = (string)node.Attribute ("currentStageText");
		}
	}
}
