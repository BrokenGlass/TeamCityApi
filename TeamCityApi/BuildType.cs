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

        public List<Build> GetBuilds(int count = 0)
        {
            return GetBuildTypeDetails().GetBuilds(count);
        }
    }


    public class BuildTypeDetails : TeamCityApiResult
    {
        public BuildTypeDetails(TeamCityApi api, XElement xml) : base(api, xml)
        {
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
