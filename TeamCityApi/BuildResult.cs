using System;
using System.Collections.Generic;
using System.Linq;

namespace TeamCity
{
	public class BuildResult
	{
		public string BuildNumber { get; set;}
		public bool Succeeded { get; set;}
		public DateTime StartTime { get; set;}
		public int DurationMs { get; set;}

		public BuildResult ()
		{
		}

		public static IList<BuildResult> ParseCsv(string csv)
		{
			List<BuildResult> results = new List<BuildResult> ();
			var rows = csv.Split ( new[] {'\n'}, StringSplitOptions.RemoveEmptyEntries);

			foreach (var row in rows.Skip(1))
			{
				var columns = row.Trim()
					             .Split (',')
					             .Select(x=> x.Trim('\"'))
					             .ToArray();
				var buildNumber = columns [0];//int.Parse(columns [0]);
				var succeeded = columns [1] == "SUCCESS";
				var startTime = DateTime.Parse(columns [2]);
				var duration = int.Parse(columns [4]);

				results.Add (new BuildResult () 
				{
					BuildNumber = buildNumber,
					Succeeded = succeeded,
					StartTime = startTime,
					DurationMs = duration
				});
			}
			return results.OrderBy(x=> x.StartTime).ToList();
		}
	}
}

