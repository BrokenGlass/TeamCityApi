using System;
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

		public TeamCityApi(TeamCityConfiguration config): this(config.ServerUrl, config.UserName, config.Password, config.UseGuestLogin)
        { }

		public TeamCityApi(string server, string userName, string password, bool useGuestLogin)
        {
            ServicePointManager.ServerCertificateValidationCallback += (a, b, c, d) => true;
            _configuration = new TeamCityConfiguration()
            {
                ServerUrl = server.TrimEnd('/'),
                UserName = userName,
				Password = password,
				UseGuestLogin = useGuestLogin
            };
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
			return xml != null ? xml.Descendants ("build").Select (b => new Build (this, b)).ToList () : null;
		}


		public List<Build> GetRunningBuilds()
		{
			var xml = TeamCityRestApiCall(TeamCityEndpoint.RunningBuilds);
			return xml != null ? xml.Descendants ("build").Select (b => new Build (this, b)).ToList () : null;
		}

		public List<Build> GetBuilds(Project p)
		{
			var xml = TeamCityRestApiCall(string.Format(TeamCityEndpoint.BuildsForProject, p.Id));
			return xml != null ? xml.Descendants ("build").Select (b => new Build (this, b)).ToList () : null;
		}


		public List<Project> GetProjects(Project project = null)
        {
			string endpoint = project == null ? TeamCityEndpoint.Projects : string.Format (TeamCityEndpoint.ContainedProjects, project.Id);

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

		//https://ci.zocdoc.com:801/httpAuth/app/rest/builds/?locator=running:true,buildType:bt728,count:1
		

        internal XElement TeamCityRestApiCall(string endpointUrl)
        {
			try
			{
				Console.WriteLine ("HTTP request to " + endpointUrl);
	            using (TimeOutWebClient wc = new TimeOutWebClient())
	            {
					string userName  = _configuration.UseGuestLogin ? "guest" : _configuration.UserName;
					string password =  _configuration.UseGuestLogin ? "" : _configuration.Password;
					
					wc.Credentials = new System.Net.NetworkCredential(userName, password);  

	                string url = _configuration.ServerUrl +  endpointUrl;
	                string resultXml = wc.DownloadString(url);
	                return XElement.Parse(resultXml);
	            }
			}
			catch(WebException)
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
