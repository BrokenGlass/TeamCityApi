using System;
using System.Collections.Generic;
using System.Linq;

namespace TeamCity
{
	public class TestResult
	{
		public int Order { get; set;}
		public string TestName { get; set;}
		public string TestResultStatus { get; set;}
		public bool Succeeded { get; set;}
		public bool Muted { get; set;}
		public bool Ignored { get; set;}
		public int DurationMs { get; set;}

		public TestResult ()
		{
		}

		public static IList<TestResult> ParseCsv(string csv)
		{
			List<TestResult> results = new List<TestResult> ();
			var rows = csv.Split ( new[] {'\n'}, StringSplitOptions.RemoveEmptyEntries);

			foreach (var row in rows.Skip(1))
			{
				var columns = row.Trim().Split (',');
				var order = int.Parse(columns [0]);
				var testName = string.Join(",", columns.Skip(1).Take(columns.Length-3));
				testName = testName.Trim ('"', ' ');
				var testResultStatus = columns [columns.Length-2];
				var succeeded = columns [columns.Length-2] == "OK";
				var muted = columns [columns.Length-2] == "Muted failure";
				var ignored = columns [columns.Length-2] == "Ignored";
				var duration = int.Parse(columns [columns.Length-1]);

				results.Add (new TestResult () 
				{
					Order = order,
					TestName = testName,
					TestResultStatus = testResultStatus,
					Succeeded = succeeded,
					Muted = muted,
					Ignored = ignored,
					DurationMs = duration
				});
			}
			return results;
		}
	}
}

