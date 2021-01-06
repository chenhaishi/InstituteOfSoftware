using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiliconValley.InformationSystem.Entity.ViewEntity.ExaminationSystemView
{
    using SiliconValley.InformationSystem.Entity.MyEntity;
    public class StudentExamView
    {
        public string StudentID { get; set; }

        public string StudentName { get; set; }
        public string DownloadContent { get; set; }
        public bool IsReExam { get; set; }
        public string Paper { get; set; }
        public string ComputerPaper { get; set; }
    }
}
