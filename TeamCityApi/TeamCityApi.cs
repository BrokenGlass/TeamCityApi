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


        public void Connect(string userName, string password)
        {

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

        public List<Project> GetProjects()
        {
            var xml = TeamCityRestApiCall(TeamCityEndpoint.Projects);
            return xml.Descendants("project").Select(p => new Project(this, p)).ToList();
        }

        internal XElement TeamCityRestApiCall(string endpointUrl)
        {
            using (WebClient wc = new WebClient())
            {
                wc.Credentials = new System.Net.NetworkCredential(_configuration.UserName, _configuration.Password);                                                                 
                string url = _configuration.ServerUrl +  endpointUrl;
                string resultXml = wc.DownloadString(url);
                return XElement.Parse(resultXml);
            }
        }



    }
}
