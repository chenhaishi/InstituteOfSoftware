﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SiliconValley.InformationSystem.Entity.MyEntity;

namespace SiliconValley.InformationSystem.Entity.ViewEntity
{
    /// <summary>
    /// 用于获取哪个校区的教室哪个阶段的教务
    /// </summary>
   public class Recon_Login_Data
    {
        /// <summary>
        /// 教室Id
        /// </summary>
        public int ClassRoom_Id
        {
            get;set;
        }
        /// <summary>
        /// 是否是S1.S2阶段的教务
        /// </summary>
        public bool IsOld
        {
            get;set;
        }
    }
}
