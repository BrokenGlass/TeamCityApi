using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace TeamCity
{
    public class Project : TeamCityApiResult
    {
        public Project(TeamCityApi api, XElement xml): base(api, xml)
        {
        }

        public List<BuildType> GetBuildTypes()
        {
            var details = Api.TeamCityRestApiCall(string.Format(TeamCityEndpoint.ProjectDetails, Id));
            return new ProjectDetails(Api, details).BuildTypes;
        }
    }


    public class ProjectDetails : TeamCityApiResult
    {
        public List<BuildType> BuildTypes { get; set; }

        public ProjectDetails(TeamCityApi api, XElement xml)
            : base(api, xml)
        {
            BuildTypes = Xml.Descendants("buildTypes")
                            .Elements("buildType")
                            .Select(x => new BuildType(this.Api, x))
                            .ToList();
        }

    }
}
