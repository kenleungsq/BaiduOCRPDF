using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing.Imaging;
using System.Xml.Linq;
using System.IO;

namespace BaiduOCR
{
    class Program
    {
        static void Main(string[] args)
        {

            List<Models> models = new List<Models>();

            for (int i = 7; i < 13; i++)
            {
                BaiDuOcr.Run(@"F:\OneDrive\工作-DESKTOP-HA9E9C4\pdf转图像\河北\1\" + i + "left.png", models);
                BaiDuOcr.Run(@"F:\OneDrive\工作-DESKTOP-HA9E9C4\pdf转图像\河北\1\" + i + "right.png", models);

                Console.WriteLine(i);
            }


            StreamWriter sw = new StreamWriter(@"F:\OneDrive\工作-DESKTOP-HA9E9C4\pdf转图像\河北\1.csv",true,Encoding.UTF8);

            foreach (var model in models)
            {
                sw.Write(model.MajorName + "," + model.MajorNum + "," + model.CollegeName + "," + model.CollegeNum + "\r\n");
            }
            
            sw.Flush();
            sw.Close();

            //ImageHandle.Qie(@"F:\OneDrive\工作-DESKTOP-HA9E9C4\pdf转图像\河北", @"F:\OneDrive\工作-DESKTOP-HA9E9C4\pdf转图像\河北\1");
        }
    }
}
