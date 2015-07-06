using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace TeamCity
{
    public class BuildType : TeamCityApiResult
    {
		public string ProjectName { get; set; }
		public string ProjectId { get;set; }

        public BuildType(TeamCityApi api, XElement xml) : base(api, xml)
        {
			ProjectName = (string)xml.Attribute ("projectName");
			ProjectId = (string)xml.Attribute ("projectId");
        }

        private BuildTypeDetails GetBuildTypeDetails()
        {
            var details = Api.TeamCityRestApiCall(string.Format(TeamCityEndpoint.BuildTypeDetails, Id));
            return new BuildTypeDetails(Api, details);
        }

        public List<Build> GetBuilds(int count = 0)
        {
            return GetBuildTypeDetails().GetBuilds(count);
        }
    }

    public class BuildTypeDetails : TeamCityApiResult
    {
		public Dictionary<string,string> BuildParameters;

        public BuildTypeDetails(TeamCityApi api, XElement xml) : base(api, xml)
        {
			BuildParameters = new Dictionary<string, string> ();

			if (xml.Element ("parameters") != null)
			{
				var buildParams = xml.Element ("parameters").Elements ("property");
				foreach (var bp in buildParams)
				{
					BuildParameters.Add ((string)bp.Attribute ("name"), (string)bp.Attribute ("value"));
				}
			}
        }

        public List<Build> GetBuilds(int count = 0)
        {
            var href = Xml.Element("builds").Attribute("href").Value;
            if (count > 0)
            {
                href += "?count=" + count;
            }

            var buildXml = Api.TeamCityRestApiCall(href);
            var builds = buildXml.Descendants("build")
                                 .Select(b => new Build(Api, b))
                                 .OrderByDescending(b => b.StartTime)
                                 .ToList();


            return builds;
        }
    }

}
