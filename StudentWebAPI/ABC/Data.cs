using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABC
{
    public class Data
    {

        public void listen()
        {
            string constr = @"server=MS-QOYXPPYYEHIK\SA;database=test;uid=sa;pwd=pan12208274";
            SqlConnection con = new SqlConnection(constr);
            string sql = "insert into hh values('nihao')";
            con.Open();
            SqlCommand com = new SqlCommand(sql, con);
            SqlDependency dep = new SqlDependency(com);
            SqlDependency.Start(constr);
            com.ExecuteNonQuery();
            dep.OnChange += Dep_OnChange;

        }

        private void Dep_OnChange(object sender, SqlNotificationEventArgs e)
        {
            string S = "";
        }
    }
}
