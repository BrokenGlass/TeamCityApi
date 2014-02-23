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

        public TeamCityApi(TeamCityConfiguration config): this(config.ServerUrl, config.UserName, config.Password)
        { }

        public TeamCityApi(string server, string userName, string password)
        {
            ServicePointManager.ServerCertificateValidationCallback += (a, b, c, d) => true;
            _configuration = new TeamCityConfiguration()
            {
                ServerUrl = server.TrimEnd('/'),
                UserName = userName,
                Password = password
            };
        }

        public bool Authenticate()
        {
            //test API connection
            try
            {
                string version = TeamCityRestApiCall(TeamCityEndpoint.ApiRoot + "server").Value;
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

		public List<Build> GetBuilds()
		{
			var xml = TeamCityRestApiCall(TeamCityEndpoint.AllBuilds);
			return xml.Descendants("build").Select(b => new Build(this, b)).ToList();
		}

        public List<Project> GetProjects()
        {
            var xml = TeamCityRestApiCall(TeamCityEndpoint.Projects);
            return xml.Descendants("project").Select(p => new Project(this, p)).ToList();
        }

        public List<BuildType> GetBuildTypes()
        {
            var xml = TeamCityRestApiCall(TeamCityEndpoint.BuildTypes);
            return xml.Descendants("buildType").Select(bt => new BuildType(this, bt)).ToList();
        }
		public BuildDetails GetLatestBuildForBuildType(string buildTypeId)
		{
			var xml = TeamCityRestApiCall (TeamCityEndpoint.LatestBuildForBuildType);
			return new BuildDetails (this, xml);
		}

        internal XElement TeamCityRestApiCall(string endpointUrl)
        {
			Console.WriteLine ("HTTP request to " + endpointUrl);
            using (TimeOutWebClient wc = new TimeOutWebClient())
            {
                wc.Credentials = new System.Net.NetworkCredential(_configuration.UserName, _configuration.Password);                                                                 
                string url = _configuration.ServerUrl +  endpointUrl;
                string resultXml = wc.DownloadString(url);
                return XElement.Parse(resultXml);
            }
        }



    }
}
