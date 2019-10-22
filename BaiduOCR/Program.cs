using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing.Imaging;
using System.Xml.Linq;
using System.IO;
using System.Runtime.CompilerServices;

namespace BaiduOCR
{
    class Program
    {
        static void Main(string[] args)
        {

            List<Models> models = new List<Models>();
            List<ZSJH> zsjhs = new List<ZSJH>();

            for (int i = 25; i < 26; i++)
            {
                //BaiDuOcr.Run(@"F:\OneDrive\工作-DESKTOP-HA9E9C4\pdf转图像\河北\1\" + i + "left.png", models);
                BaiDuOcr.Run(@"F:\OneDrive\工作-DESKTOP-HA9E9C4\pdf转图像\河北\1\" + i + "right.png", models);

                Console.WriteLine(i);
            }

            //var query = from m in models
            //            where m.MajorNum.Length >= 6
            //    select new ZSJH
            //    {
            //        CollegeName = m.CollegeName,
            //        Fee = m.MajorNum.Substring(m.MajorNum.Length - 4, 4),
            //        XueNian = m.MajorNum.Substring(m.MajorNum.Length - 5,1 ),
            //        Persons = m.MajorNum.Substring(1,m.MajorNum.Length - 5)
            //    };

            //foreach (var itemZsjh in query)
            //{
            //    Console.WriteLine(itemZsjh.CollegeName + itemZsjh.Persons + " " + itemZsjh.XueNian + " " + itemZsjh.Fee);
            //}

            //Console.Read();

            
            //从models.majorNum 提取出 招生人数、学制、学费，利用substring
            foreach (var model in models)
            {
                var zsjh = new ZSJH();
                zsjh.CollegeNum = model.CollegeNum;
                zsjh.CollegeName = model.CollegeName;
                zsjh.MajorName = model.MajorName;
                if (model.MajorNum != null && model.MajorNum.Length >= 6)
                {
                    zsjh.Fee = model.MajorNum.Substring(model.MajorNum.Length - 4, 4);
                    zsjh.XueNian = model.MajorNum.Substring(model.MajorNum.Length - 5, 1);
                    zsjh.Persons = model.MajorNum.Substring(0, model.MajorNum.Length - 5);
                }
                else
                {
                    zsjh.Fee = null;
                    zsjh.XueNian = null;
                    zsjh.Persons = model.MajorNum;
                }

                zsjhs.Add(zsjh);
              
            }

            StreamWriter sw = new StreamWriter(@"F:\OneDrive\工作-DESKTOP-HA9E9C4\pdf转图像\河北\1.csv", true, Encoding.UTF8);
            foreach (var zsjh in zsjhs)
            {
                sw.Write(zsjh.MajorName + "," + zsjh.Persons + "," + zsjh.XueNian + "," + zsjh.Fee + "," + zsjh.CollegeName + "," + zsjh.CollegeNum + "\r\n");
            }
            sw.Flush();
            sw.Close();

            //ImageHandle.Qie(@"F:\OneDrive\工作-DESKTOP-HA9E9C4\pdf转图像\河北", @"F:\OneDrive\工作-DESKTOP-HA9E9C4\pdf转图像\河北\1");
        }
    }
}
