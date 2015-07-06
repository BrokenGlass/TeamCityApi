using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TeamCity
{
    public static class TeamCityEndpoint
    {
        public const string ApiRoot = "/httpAuth/app/rest/";
		public const string Server = ApiRoot + "server";
        public const string Projects = "/httpAuth/app/rest/projects";
		public const string ProjectsRoot = "/httpAuth/app/rest/projects/id:_Root";
		public const string ContainedProjects = "/httpAuth/app/rest/projects/id:{0}";
        public const string BuildTypes = "/httpAuth/app/rest/buildTypes";
        public const string ProjectDetails = "/httpAuth/app/rest/projects/id:{0}";
		public const string BuildDetails = "/httpAuth/app/rest/builds/id:{0}";
		public const string Artifacts = "/httpAuth/app/rest/builds/id:{0}/artifacts/children";
		public const string BuildTypeDetails = "/httpAuth/app/rest/buildTypes/id:{0}";
		public const string BuildHistoryForBuildType = "/exportchart.html?type=text&buildTypeId={0}&%40f_range=MONTH&%40filter.status=ERROR&%40filter.pas=true&_graphKey=g&valueType=BuildDurationNetTime&id=BuildDurationNetTimeStatistics";
		public const string LatestBuildForBuildType = "/httpAuth/app/rest/builds/buildType:{0}";
		public const string BuildsForBuildType =  "/httpAuth/app/rest/builds/?locator=running:any,buildType:{0}";
		public const string TestsForBuildId = "/get/tests/buildId/{0}/testResults.csv";
		public const string TestHistory = "/exportchart.html?type=text&buildTypeId=&projectId={0}&%40f_range=MONTH&%40filter.status=ERROR&testNameId={1}&_graphKey=g&valueType=TestDuration&id=TestDuration";
		public const string AllBuilds = "/httpAuth/app/rest/builds/?count=100";
		public const string AllBuildsSince = "/httpAuth/app/rest/builds/?sinceDate={0}&count=200&locator=running:any";

		public const string RunningBuilds = "/httpAuth/app/rest/builds/?locator=running:true";
		public const string RunningBuildsForBuildType = RunningBuilds + ",buildType:{0}";
		public const string BuildsForProject = "/httpAuth/app/rest/builds/?locator=project:(id:{0})";
		public const string BuildLog = "/httpAuth/downloadBuildLog.html?buildId={0}";
		public const string Changes = "/httpAuth/app/rest/changes?locator=build:(id:{0})";
		public const string ChangeDetails = "/httpAuth/app/rest/changes/id:{0}";
		public const string AddFieldInfo = "fields=$long,build($short,startDate,finishDate,statusText)";

		public const string TriggerBuild = "/httpAuth/action.html?add2Queue={0}";
    
		public static string GetEndPoint(string endpoint, TeamCityVersionInfo version)
		{
			if (version != null  && endpoint.Contains ("/builds/"))
			{
				if(version.MajorVersion >8  || ( version.MajorVersion == 8 && version.MinorVersion >=1))
				{
					if(endpoint.Contains("?"))
					{
						endpoint = endpoint + "&" + AddFieldInfo;
					}
					else
						endpoint = endpoint + "?" + AddFieldInfo;
				}
			} 
			return endpoint;
		}
	
	
	}
}
