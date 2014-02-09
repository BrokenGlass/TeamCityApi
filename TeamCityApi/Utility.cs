using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;

namespace TeamCity
{
    public class TimeOutWebClient : WebClient
    {
        /// <summary>
        /// Time in milliseconds
        /// </summary>
        public TimeSpan Timeout { get; set; }

        public TimeOutWebClient() : this(60000) { }
        public TimeOutWebClient(int seconds) : this(TimeSpan.FromSeconds(seconds))
        {
        }

        public TimeOutWebClient(TimeSpan t) 
        {
            Timeout = t;
        }

        protected override WebRequest GetWebRequest(Uri address)
        {
            var request = base.GetWebRequest(address);
            if (request != null)
            {
                request.Timeout = (int)Timeout.TotalSeconds;
            }
            return request;
        }
    }
}
