﻿using SiliconValley.InformationSystem.Entity.Entity;
using SiliconValley.InformationSystem.Entity.MyEntity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiliconValley.InformationSystem.Entity.ViewEntity.ExaminationSystemView
{
   /// <summary>
   /// 考试类型模型视图
   /// </summary>
   public class ExamTypeView
    {
        public int ID { get; set; }
        public  ExamTypeName TypeName { get; set; }
        public Grand GrandID { get; set; }
    }
}