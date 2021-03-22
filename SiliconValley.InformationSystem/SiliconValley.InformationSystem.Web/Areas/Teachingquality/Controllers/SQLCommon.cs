using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SiliconValley.InformationSystem.Web.Areas.Teachingquality.Controllers
{
    public class SQLCommon
    {
        private string connectionStr { get; set; }
        public SQLCommon(string config)
        {
            this.connectionStr = config;
        }

    }
}