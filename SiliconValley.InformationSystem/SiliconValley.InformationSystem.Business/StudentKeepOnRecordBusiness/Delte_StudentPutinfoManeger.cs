using SiliconValley.InformationSystem.Entity.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiliconValley.InformationSystem.Business.StudentKeepOnRecordBusiness
{
   public class Delte_StudentPutinfoManeger:BaseBusiness<Delte_StudentPutinfo>
    {
        public bool Add_data(Delte_StudentPutinfo data)
        {
            bool s = true;
            try
            {
                this.Insert(data);
            }
            catch (Exception)
            {
                s = false;
            }

            return s;
        }
    }
}
