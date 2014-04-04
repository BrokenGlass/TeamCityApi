using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TeamCity
{
    public class TeamCityEndpoint
    {
        public const string ApiRoot = "/httpAuth/app/rest/";
        public const string Projects = "/httpAuth/app/rest/projects";
		public const string ProjectsRoot = "/httpAuth/app/rest/projects/id:_Root";
		public const string ContainedProjects = "/httpAuth/app/rest/projects/id:{0}";
        public const string BuildTypes = "/httpAuth/app/rest/buildTypes";
        public const string ProjectDetails = "/httpAuth/app/rest/projects/id:{0}";
		public const string BuildDetails = "/httpAuth/app/rest/builds/id:{0}";
		public const string BuildTypeDetails = "/httpAuth/app/rest/buildTypes/id:{0}";
		public const string LatestBuildForBuildType = "/httpAuth/app/rest/builds/buildType:{0}";
		public const string BuildsForBuildType = BuildTypeDetails + "/builds/";
		public const string AllBuilds = "/httpAuth/app/rest/builds/?count=100";
		public const string RunningBuilds = "/httpAuth/app/rest/builds/?locator=running:true";
		public const string BuildsForProject = "/httpAuth/app/rest/builds/?locator=project:(id:{0})";
		//public const string BuildsForProject = "/httpAuth/app/rest/projects/id:{0}";
    }
}
