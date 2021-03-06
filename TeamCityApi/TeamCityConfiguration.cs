﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TeamCity
{
    public class TeamCityConfiguration
    {
        public string ServerUrl { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
		public bool UseGuestLogin {get; set;}
		public int TimeoutSeconds {get; set;}

		public TeamCityConfiguration()
		{
			TimeoutSeconds = 60;
		}
    }
}
