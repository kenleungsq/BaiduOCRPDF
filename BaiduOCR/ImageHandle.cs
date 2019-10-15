using System.Drawing;
using System.IO;
using ImageProcessor;

namespace BaiduOCR
{
    public class ImageHandle
    {
        private static readonly Rectangle RectAreaLeft = new Rectangle(133,493,1109,2745);
        private static readonly Rectangle RectAreaRight = new Rectangle(1237, 481, 1093, 2757);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="folderPath">所有需切割图片所在文件夹路径</param>
        /// <param name="saveFolderPath">保存文件夹路径</param>
        public static void Qie(string folderPath,string saveFolderPath)
        {

            string[] filePaths = Directory.GetFiles(folderPath);

            for (int i = 0; i < filePaths.Length; i++)
            {
                //传入需切割图片
                Bitmap img = new Bitmap(filePaths[i]);

                //克隆指定区域
                Bitmap resultLeft = img.Clone(new Rectangle(0, 0, img.Width / 2, img.Height), img.PixelFormat);
                Bitmap resultRight = img.Clone(new Rectangle(img.Width / 2, 0, img.Width / 2, img.Height), img.PixelFormat);

                //用imageFactory增加对比度，删除背面透过来的文字
                var imageFactoryLeft = new ImageFactory().Load(resultLeft);
                var imageFactoryRight = new ImageFactory().Load(resultRight);
                imageFactoryLeft.Contrast(80).Format(new ImageProcessor.Imaging.Formats.PngFormat()).Save(saveFolderPath + @"\" + i + @"left.png");
                imageFactoryRight.Contrast(80).Format(new ImageProcessor.Imaging.Formats.PngFormat()).Save(saveFolderPath + @"\" + i + @"right.png");

                //resultLeft.Save(SavePathLeft);
                resultLeft.Dispose();

                //resultRight.Save(SavePathRight);
                resultRight.Dispose();
            }


        }
    }
}