using System;
using System.Xml.Linq;
using System.Collections.Generic;
using System.Linq;

namespace TeamCity
{
	public class Change : TeamCityApiResult
	{
		public string Version {get;set;}

		public static List<Change> Parse (TeamCityApi api, XElement xml)
		{
			var changes = xml.Descendants ("change");
			return changes.Select (x => new Change (api, x)).ToList ();
		}

		public Change (TeamCityApi api, XElement xml) : base(api, xml)
		{
			Version = (string) xml.Attribute ("version");
		}
	}

	public class ChangeDetail  : Change
	{
		public string UserName { get; set;}
		public DateTime Date { get; set;}
		public string Comment { get; set;}
		public string WebLink {get;set;}
		public List<string> Files { get; set;}
		public string Repository { get; set; }

		public ChangeDetail (TeamCityApi api, XElement xml) : base(api, xml)
		{
			if(xml.Element("comment")!=null)
			{
				Comment = (string)xml.Element ("comment");
			}
			UserName = (string) xml.Attribute ("username");
			Date = TeamCityUtils.ParseTime (xml.Attribute ("date").Value);
			WebLink = (string)xml.Attribute ("webLink") ?? (string)xml.Attribute ("webUrl");
			Files = xml.Descendants ("file").Select (x => (string)x.Attribute ("file")).ToList ();

			if (xml.Element ("vcsRootInstance") != null)
			{
				Repository = (string)xml.Element ("vcsRootInstance").Attribute("name");
			}
		}
	}
}

