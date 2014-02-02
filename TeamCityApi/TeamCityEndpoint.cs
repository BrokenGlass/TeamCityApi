using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TeamCity
{
    public class TeamCityEndpoint
    {
        public const string ApiRoot = "/httpAuth/app/rest/";
        public const string ProjectType = "/httpAuth/app/rest/builds/?locator=buildType:{0}";
        public const string Projects = "/httpAuth/app/rest/projects";
        public const string ProjectDetails = "/httpAuth/app/rest/projects/id:{0}";
        public const string BuildTypeDetails = "/httpAuth/app/rest/buildTypes/id:{0}";
        public const string Builds = BuildTypeDetails + "/builds/";
    }
}
