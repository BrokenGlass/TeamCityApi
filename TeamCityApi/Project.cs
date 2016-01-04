using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace TeamCity
{
    public class Project : TeamCityApiResult
    {
		public string ParentProjectId { get; set; }
		public string Description { get; set; }

        public Project(TeamCityApi api, XElement xml): base(api, xml)
        {
			if (xml.Attribute ("parentProjectId") != null)
			{
				ParentProjectId = xml.Attribute ("parentProjectId").Value;
			} 

			if (xml.Attribute ("description") != null)
			{
				Description = xml.Attribute ("description").Value;
			} 
        }

        public List<Build> GetBuilds()
        {
            var xml = Api.TeamCityRestApiCall(string.Format(TeamCityEndpoint.BuildsForProject, Id));
            return xml.Descendants("build").Select(b => new Build(Api, b)).ToList();
        }

        public List<Build> GetMostRecentBuilds()
        {
            var allBuilds = GetBuilds();

            var builds = allBuilds.GroupBy(x => x.Id)
                                  .Select(g => g.OrderByDescending(x => x.StartTime).First())
                                  .ToList();

            return builds;
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
