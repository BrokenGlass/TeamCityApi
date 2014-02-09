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
        public const string BuildTypes = "/httpAuth/app/rest/buildTypes";
        public const string ProjectDetails = "/httpAuth/app/rest/projects/id:{0}";
        public const string BuildTypeDetails = "/httpAuth/app/rest/buildTypes/id:{0}";
        public const string Builds = BuildTypeDetails + "/builds/";
        //public const string BuildsForProject = "/httpAuth/app/rest/projects/{0}/buildTypes";
        public const string BuildsForProject = "/httpAuth/app/rest/builds/?locator=project:(id:{0})";
    }
}
