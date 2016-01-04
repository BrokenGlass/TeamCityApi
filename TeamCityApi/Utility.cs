using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Globalization;
using System.Net.Http;
using System.Threading.Tasks;
using System.Net.Http.Headers;
using ModernHttpClient;


namespace TeamCity
{
	public class TimeOutWebClient : HttpClient
    {
        public TimeOutWebClient() : this(60) { }
        public TimeOutWebClient(int seconds) : this(TimeSpan.FromSeconds(seconds))
        {
        }

		public TimeOutWebClient(TimeSpan t) : base(new NativeMessageHandler())
        {
			Timeout = TimeSpan.FromSeconds (60000);// t;
        }

		public string DownloadString(string requestUri)
		{
			var task = this.GetStringAsync (requestUri);
			//need to do this so we don't try to marshall back to original context and potentially deadlock
			task.ConfigureAwait (false);
			task.Wait ();
			return task.Result;
		}

		public byte[] DownloadData(string requestUri)
		{
			var task = this.GetByteArrayAsync (requestUri);
			//need to do this so we don't try to marshall back to original context and potentially deadlock
			task.ConfigureAwait (false);
			task.Wait ();
			return task.Result;
		}

		public void SetCredentials(string userName, string password)
		{
			var byteArray = Encoding.ASCII.GetBytes(string.Format("{0}:{1}", userName,password));
			var header = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
			this.DefaultRequestHeaders.Authorization = header;
		}
    }

    public static class TeamCityUtils
    {
        public static DateTime ParseTime(string s)
        {
            DateTime time;
			DateTime.TryParseExact(s, "yyyyMMdd'T'HHmmsszzz", CultureInfo.InvariantCulture, DateTimeStyles.None, out time);
            return time;
        }
		public static string FormatTime(DateTime time)
		{
			string result = time.ToString ("yyyyMMdd'T'HHmmsszzz");
			return result;
		}
    }


	public static class DateTimeHelper
	{
		public static string ToTeamCityTime(this DateTime time)
		{
			string result = time.ToString ("yyyyMMdd'T'HHmmsszzz");
			return result;
		}
	}

}
