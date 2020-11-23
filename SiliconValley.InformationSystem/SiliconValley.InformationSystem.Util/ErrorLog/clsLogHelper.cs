using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiliconValley.InformationSystem.Util.ErrorLog
{
   public  class clsLogHelper
    {
        #region  创建错误日志
        ///-----------------------------------------------------------------------------
        /// <summary>创建错误日志 在c:\ErrorLog\</summary>
        /// <param name="strFunctionName">strFunctionName,调用方法名</param>
        /// <param name="strErrorNum">strErrorNum,错误号</param>
        /// <param name="strErrorDescription">strErrorDescription,错误内容</param>
        /// <returns></returns>
        /// <history>2009-05-29 Created</history>
        ///-----------------------------------------------------------------------------
        //举例:
        //         try
        //         {    要监视的代码     }
        //      catch()
        //            {  myErrorLog.m_CreateErrorLogTxt("myExecute(" & str执行SQL语句 & ")", Err.Number.ToString, Err.Description)  }     
        //         finally
        //         {            }
        public static void m_CreateErrorLogTxt(string strFunctionName, string strErrorNum, string strErrorDescription)
        {
            string strMatter; //错误内容
            string strPath; //错误文件的路径
            DateTime dt = DateTime.Now;
            try
            {
                //Server.MapPath("./") + "File"; 服务器端路径
                strPath = Directory.GetCurrentDirectory() + "\\ErrorLog";   //winform工程\bin\目录下 创建日志文件夹 
                //strPath = "c:" + "\\ErrorLog";//暂时放在c:下

                if (Directory.Exists(strPath) == false)  //工程目录下 Log目录 '目录是否存在,为true则没有此目录
                {
                    Directory.CreateDirectory(strPath); //建立目录　Directory为目录对象
                }
                strPath = strPath + "\\" + dt.ToString("yyyyMM");

                if (Directory.Exists(strPath) == false)  //目录是否存在  '工程目录下 Log\月 目录   yyyymm
                {
                    Directory.CreateDirectory(strPath);  //建立目录//日志文件，以 日 命名 
                }
                strPath = strPath + "\\" + dt.ToString("dd") + ".txt";

                strMatter = strFunctionName + " , " + strErrorNum + " , " + strErrorDescription;//生成错误信息

                StreamWriter FileWriter = new StreamWriter(strPath, true); //创建日志文件
                FileWriter.WriteLine("Time: " + dt.ToString("HH:mm:ss") + "  Err: " + strMatter);
                FileWriter.Close(); //关闭StreamWriter对象
            }
            catch (Exception ex)
            {
                //("写错误日志时出现问题，请与管理员联系！ 原错误:" + strMatter + "写日志错误:" + ex.Message.ToString());
                string str = ex.Message.ToString();
            }
        }
        #endregion
    }
}
