﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Xml.Linq;

namespace TeamCity
{
    public class TeamCityApi
    {
        private TeamCityConfiguration _configuration;
		private TeamCityVersionInfo _versionInfo = null;
		public int TimeoutSeconds { get; set; }

		public TeamCityApi(TeamCityConfiguration config): this(config.ServerUrl, config.UserName, config.Password, config.UseGuestLogin, config.TimeoutSeconds)
        { }

		public TeamCityApi(string server, string userName, string password, bool useGuestLogin, int timeoutSeconds)
        {
            ServicePointManager.ServerCertificateValidationCallback = (a, b, c, d) => true;
            _configuration = new TeamCityConfiguration()
            {
                ServerUrl = server.TrimEnd('/'),
                UserName = userName,
				Password = password,
				UseGuestLogin = useGuestLogin
            };
			TimeoutSeconds = timeoutSeconds;
        }

        public bool Authenticate()
        {
            //test API connection
            try
            {
				var serverElement = TeamCityRestApiCall(TeamCityEndpoint.ApiRoot + "server");
				_versionInfo = new TeamCityVersionInfo()
				{
					Version = (string)serverElement.Attribute("version"),
					MajorVersion =  (int)serverElement.Attribute("versionMajor"),
					MinorVersion = (int)serverElement.Attribute("versionMinor")
				};
              	return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

		public bool TriggerBuild(string buildTypeId, Dictionary<string,string> buildParameters = null)
		{
			string url = string.Format (TeamCityEndpoint.TriggerBuild, buildTypeId);
			if (buildParameters != null)
			{
				foreach (var bp in buildParameters)
				{
					url += string.Format ("&name={0}&value={1}", bp.Key, bp.Value);
				}
			}

			bool wasSuccessful = TeamCityHttpCall (url) != null;
			return wasSuccessful;
		}

		public TeamCityVersionInfo GetVersion()
		{
			if (_versionInfo == null)
			{
				Authenticate ();
			}
			return _versionInfo;
		}
			

		public List<Build> GetBuilds()
		{
			var xml = TeamCityRestApiCall(TeamCityEndpoint.AllBuilds);
			return xml != null ? xml.Descendants ("build").Select (b => new Build (this, b)).ToList () : new List<Build>();
		}

		public List<Build> GetBuildsSince(DateTime startTime)
		{
			var xml = TeamCityRestApiCall(string.Format(TeamCityEndpoint.AllBuildsSince, startTime.ToTeamCityTime()));
			return xml != null ? xml.Descendants ("build").Select (b => new Build (this, b)).ToList () : null;
		}

		public List<Build> GetRunningBuilds(string buildTypeId = null)
		{
			var xml = buildTypeId == null ? TeamCityRestApiCall(TeamCityEndpoint.RunningBuilds)
										  : TeamCityRestApiCall(string.Format(TeamCityEndpoint.RunningBuildsForBuildType, buildTypeId));
			return xml != null ? xml.Descendants ("build").Select (b => new Build (this, b)).ToList () : null;
		}

		public List<Build> GetBuilds(Project p)
		{
			var xml = TeamCityRestApiCall(string.Format(TeamCityEndpoint.BuildsForProject, p.Id));
			return xml != null ? xml.Descendants ("build").Select (b => new Build (this, b)).ToList () : null;
		}

		public List<Build> GetBuilds(string buildTypeId)
		{
			var xml = TeamCityRestApiCall(string.Format(TeamCityEndpoint.BuildsForBuildType, buildTypeId));
			return xml != null ? xml.Descendants ("build").Select (b => new Build (this, b)).ToList () : null;
		}

		public IList<BuildResult> GetBuildHistory(string buildTypeId)
		{
			var results =  TeamCityHttpCall (string.Format (TeamCityEndpoint.BuildHistoryForBuildType, buildTypeId));
			return BuildResult.ParseCsv (results);
		}

		public IList<TestResult> GetTestResults(string buildId)
		{
			var results =  TeamCityHttpCall (string.Format (TeamCityEndpoint.TestsForBuildId, buildId));
			return TestResult.ParseCsv (results);
		}

		public string GetBuildLog(string buildId)
		{
			return TeamCityHttpCall (string.Format (TeamCityEndpoint.BuildLog, buildId));
		}

		public byte[] GetBuildLogZipped(string buildId)
		{
			return TeamCityHttpDataCall (string.Format (TeamCityEndpoint.BuildLogZipped, buildId));
		}

		public string GetTextResult(string relativeUrl)
		{
			return TeamCityHttpCall (relativeUrl);
		}

		public List<Project> GetProjects(string projectId)
        {
			string endpoint = projectId == null ? TeamCityEndpoint.Projects : string.Format (TeamCityEndpoint.ContainedProjects, projectId);

			var xml = TeamCityRestApiCall(endpoint);
			return xml != null ? xml.Descendants("project").Select(p => new Project(this, p)).ToList() : null;
        }

		public List<Project> GetTopLevelProjects()
		{
			var xml = TeamCityRestApiCall(TeamCityEndpoint.ProjectsRoot);
			return xml != null ? xml.Descendants("project").Select(p => new Project(this, p)).ToList() : null;
		}

        public List<BuildType> GetBuildTypes()
        {
            var xml = TeamCityRestApiCall(TeamCityEndpoint.BuildTypes);
			return xml != null ? xml.Descendants ("buildType").Select (bt => new BuildType (this, bt)).ToList () : null;
        }

		/*
		public List<BuildDetails> GetLatestBuildsForProject(string projectId)
		{
			var xml = TeamCityRestApiCall (string.Format(TeamCityEndpoint.LatestBuildsForProject, projectId));
			return xml != null ? xml.Descendants("build").Select(p => new BuildDetails(this, p)).ToList() : null;
		}
		*/

		public BuildDetails GetLatestBuildForBuildType(string buildTypeId)
		{
			var xml = TeamCityRestApiCall (string.Format(TeamCityEndpoint.LatestBuildForBuildType, buildTypeId));
			return xml != null ? new BuildDetails (this, xml) : null;
		}

		public BuildDetails GetBuildDetails(string id)
		{
			var xml = TeamCityRestApiCall (string.Format(TeamCityEndpoint.BuildDetails, id));
			return xml != null ? new BuildDetails (this, xml) : null;
		}

		public BuildTypeDetails GetBuildTypeDetails(string id)
		{
			var xml = TeamCityRestApiCall (string.Format(TeamCityEndpoint.BuildTypeDetails, id));
			return xml != null ? new BuildTypeDetails (this, xml) : null;
		}

		public List<BuildArtifact> GetBuildArtifacts(string id)
		{
			var xml = TeamCityRestApiCall (string.Format(TeamCityEndpoint.Artifacts, id));
			return xml != null ? BuildArtifact.Parse(this, xml) : null;
		}

		public List<Change> GetChanges(string id)
		{
			var xml = TeamCityRestApiCall (string.Format(TeamCityEndpoint.Changes, id));
			return xml != null ? Change.Parse(this, xml) : null;
		}

		public ChangeDetail GetChangeDetails(string id)
		{
			var xml = TeamCityRestApiCall (string.Format(TeamCityEndpoint.ChangeDetails, id));
			return xml != null ? new ChangeDetail (this, xml) : null;
		}
			
		internal string TeamCityHttpCall(string endpointUrl)
		{
			try
			{
				Console.WriteLine ("HTTP request to " + endpointUrl);
				using (TimeOutWebClient wc = new TimeOutWebClient(TimeoutSeconds))
				{
					string userName  = _configuration.UseGuestLogin ? "guest" : _configuration.UserName;
					string password =  _configuration.UseGuestLogin ? "" : _configuration.Password;

					wc.SetCredentials(userName, password);
					string url = _configuration.ServerUrl +  endpointUrl;
					string result = wc.DownloadString(url);
					return result;
				}
			}
			catch(WebException)
			{
				return null;
			}
		}

		internal byte[] TeamCityHttpDataCall(string endpointUrl)
		{
			try
			{
				Console.WriteLine ("HTTP request to " + endpointUrl);
				using (TimeOutWebClient wc = new TimeOutWebClient(TimeoutSeconds))
				{
					string userName  = _configuration.UseGuestLogin ? "guest" : _configuration.UserName;
					string password =  _configuration.UseGuestLogin ? "" : _configuration.Password;

					wc.SetCredentials(userName, password);
					string url = _configuration.ServerUrl +  endpointUrl;
					var result = wc.DownloadData(url);
					return result;
				}
			}
			catch(WebException)
			{
				return null;
			}
		}

        internal XElement TeamCityRestApiCall(string endpointUrl)
        {
			if (_versionInfo == null && !endpointUrl.EndsWith(TeamCityEndpoint.Server))
			{
				Authenticate ();
			}

			endpointUrl = TeamCityEndpoint.GetEndPoint (endpointUrl, _versionInfo);

			try
			{
				Console.WriteLine ("HTTP request to " + endpointUrl);
				using (TimeOutWebClient wc = new TimeOutWebClient(TimeoutSeconds))
	            {
					string userName  = _configuration.UseGuestLogin ? "guest" : _configuration.UserName;
					string password =  _configuration.UseGuestLogin ? "" : _configuration.Password;
					
					wc.SetCredentials(userName, password);  

	                string url = _configuration.ServerUrl +  endpointUrl;
	                string resultXml = wc.DownloadString(url);
	                return XElement.Parse(resultXml);
	            }
			}
			catch(Exception ex)
			{
				return null;
			}
        }

    }

	public class TeamCityVersionInfo
	{
		public string Version;
		public int MajorVersion;
		public int MinorVersion;
	}
}
