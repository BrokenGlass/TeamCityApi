using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace TeamCity
{
    public class TeamCityApiResult
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string HRef { get; set; }

        protected XElement Xml {get;set;}
        protected TeamCityApi Api { get; set; }


        public TeamCityApiResult(TeamCityApi api, XElement xml)
        {
            Api = api;
            Xml = xml;
            ParseBaseInfo();
        }

        public void ParseBaseInfo()
        {
            Id = (string)Xml.Attribute("id");
            Name = (string)Xml.Attribute("name");
            HRef = (string)Xml.Attribute("href");
        }
    }
}
