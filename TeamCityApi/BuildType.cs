using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace TeamCity
{
    public class BuildType : TeamCityApiResult
    {
        public BuildType(TeamCityApi api, XElement xml) : base(api, xml)
        {
        }

        private BuildTypeDetails GetBuildTypeDetails()
        {
            var details = Api.TeamCityRestApiCall(string.Format(TeamCityEndpoint.BuildTypeDetails, Id));
            return new BuildTypeDetails(Api, details);
        }

        public List<Build> GetBuilds()
        {
            return GetBuildTypeDetails().Builds;
        }
    }


    public class BuildTypeDetails : TeamCityApiResult
    {
        public List<Build> Builds;

        public BuildTypeDetails(TeamCityApi api, XElement xml) : base(api, xml)
        {
            var href = Xml.Element("builds").Attribute("href").Value;
            var buildXml = Api.TeamCityRestApiCall(href);

            Builds = buildXml.Descendants("build")
                             .Select(b => new Build(Api, b))
                             .OrderByDescending(b=> b.StartTime)
                             .ToList();
        }
    }

}
