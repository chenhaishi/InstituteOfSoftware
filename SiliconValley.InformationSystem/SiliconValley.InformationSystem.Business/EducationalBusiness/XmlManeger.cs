using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace SiliconValley.InformationSystem.Business.EducationalBusiness
{

    public class XmlEntity
    {
        /// <summary>
        /// 年份
        /// </summary>
        public string Year { get; set; }

        /// <summary>
        /// 开始单休的月份
        /// </summary>
        public string StartMonth { get; set; }

        /// <summary>
        /// 结束单休的月份
        /// </summary>
        public string EndMonth { get; set; }
    }
   public class XmlManeger
    {
        /// <summary>
        /// 获取这个XML文件的所有数据
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public List<XmlEntity> Getlist(string path)
        {
            XmlDocument document = new XmlDocument();
            document.Load(path);
            List<XmlEntity> entities = new List<XmlEntity>();
            //获取XML根节点
            XmlElement root = document.DocumentElement;

            //获取根节点的所有子节点
            XmlNodeList nodeList=  root.ChildNodes;

            for (int i = 0; i < nodeList.Count; i++)
            {
                XmlEntity entity = new XmlEntity();

                XmlAttribute xml= nodeList[i].Attributes["name"];

                entity.Year = xml.Value;

                XmlNodeList child = nodeList[i].ChildNodes;
               
                for (int j = 0; j < child.Count; j++)
                {
                   string name= child[j].Name;
                    if (name== "startmonth")
                    {
                        entity.StartMonth = child[j].InnerText;
                    }
                    else if(name == "endmonth")
                    {
                        entity.EndMonth = child[j].InnerText;
                    }
                }

                entities.Add(entity);
            }

            return entities;  
        }


        /// <summary>
        /// 添加某个年份的单休月份
        /// </summary>
        /// <param name="xmlEntity"></param>
        /// <returns></returns>
        public bool AddData(XmlEntity xmlEntity,string path)
        {
            bool s = true;
            try
            {
                //创建XML文档对象
                XmlDocument document = new XmlDocument();
                //设置第一行描述信息
                //XmlDeclaration declaration = document.CreateXmlDeclaration("1.0", "utf-8", null);
                //将创建的描述信息加载到文档中
                //document.AppendChild(declaration);
                document.Load(path);
                //获取XML文件的根节点
                XmlElement root = document.DocumentElement;
                //将根目录添加到文档中
                //document.AppendChild(root);

                //给根节点添加子节点

                XmlElement element = document.CreateElement("Year");
                //创建属性
                XmlAttribute attribute = document.CreateAttribute("name");
                //给属性赋值
                attribute.Value = xmlEntity.Year;
                //给子节点添加name属性
                element.Attributes.Append(attribute);

                XmlElement startmonth= document.CreateElement("startmonth");
                startmonth.InnerText = xmlEntity.StartMonth;

                XmlElement endmonth = document.CreateElement("endmonth");
                endmonth.InnerText = xmlEntity.EndMonth;

                element.AppendChild(startmonth);
                element.AppendChild(endmonth);
                

                //将子节点添加到根节点中
                root.AppendChild(element);


                document.Save(path);


               
            }
            catch (Exception e)
            {

                  s = false;
            }
          
           
             return s;
           
        }

        /// <summary>
        /// 编辑某个年份的单休月份
        /// </summary>
        /// <param name="xmlEntity"></param>
        /// <returns></returns>
        public bool EditData(XmlEntity xmlEntity,string path)
        {
            bool s = true;
            try
            {
                //创建XML文档对象
                XmlDocument document = new XmlDocument();
                document.Load(path);
                //获取XML文件的根节点
                XmlElement root = document.DocumentElement;

                XmlNodeList list= root.ChildNodes;

                for (int i = 0; i < list.Count; i++)
                {
                   string value= list[i].Attributes["name"].Value;

                    if (xmlEntity.Year== value)
                    {
                        list[i].ChildNodes[0].InnerText = xmlEntity.StartMonth;

                        list[i].ChildNodes[1].InnerText = xmlEntity.EndMonth;
                    }
                }

                document.Save(path);
            }
            catch (Exception)
            {

                s = false;
            }
            return s;
        }
    }
}
