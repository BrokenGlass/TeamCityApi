using System;
using System.Xml.Linq;
using System.Collections.Generic;
using System.Linq;

namespace TeamCity
{
	public class BuildArtifact
	{
		public long Size { get; set;}
		public DateTime ModificationTime {get;set;}
		public string Name {get;set;}
		public string HRef { get; set;}

		public static List<BuildArtifact> Parse (TeamCityApi api, XElement xml)
		{
			var files = xml.Descendants ("file").Where(x=> x.Element("content")!=null);
			return files.Select (x => new BuildArtifact (x)).ToList ();
		}

		public BuildArtifact (XElement xml)
		{
			Size = (long)xml.Attribute ("size");
			ModificationTime = TeamCityUtils.ParseTime(xml.Attribute("modificationTime").Value);
			Name = (string) xml.Attribute("name");
			HRef = (string) xml.Element ("content").Attribute ("href");
		}
	}

	public class BuildArtifactFolder
	{
		public DateTime ModificationTime {get;set;}
		public string Name {get;set;}
		public string HRef { get; set;}

		public BuildArtifactFolder (XElement xml)
		{
			ModificationTime = TeamCityUtils.ParseTime(xml.Attribute("modificationTime").Value);
			Name = (string) xml.Attribute("name");

			var childNode = xml.Element ("children");
			HRef = (string) childNode.Attribute ("href");
		}
	}

}

