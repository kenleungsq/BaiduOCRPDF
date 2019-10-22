using System;
using System.CodeDom;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Newtonsoft.Json;

namespace BaiduOCR
{
    public class BaiDuOcr
    {
        // 设置APPID/AK/SK
        static string APP_ID = "你的 App ID";
        static string API_KEY = "n47iC4QO7HztPD26BiDE6vKE";
        static string SECRET_KEY = "St5KQACXpqDacqIQxoHITWbCDuuTGmYn";
        static Baidu.Aip.Ocr.Ocr client = new Baidu.Aip.Ocr.Ocr(API_KEY, SECRET_KEY) { Timeout = 60000 };
        // 修改超时时间

        public BaiDuOcr()
        {

        }

        public static void Run(string path,List<Models> models)
        {
            var image = File.ReadAllBytes(path);
            // 调用通用文字识别, 图片参数为本地图片，可能会抛出网络等异常，请使用try/catch捕获
            //var result = client.AccurateBasic(image);
            //Console.WriteLine(result);
            // 如果有可选参数
            var options = new Dictionary<string, object>{
                {"language_type", "CHN_ENG"},
                {"detect_direction", "true"},
                {"detect_language", "true"},
                //是否显示置信度
                {"probability", "true"}


            };
            // 带参数调用通用文字识别, 图片参数为本地图片
            var result = client.Accurate(image, options);

            Console.WriteLine(result);
            Console.Read();

            //解释Json
            Root rt = JsonConvert.DeserializeObject<Root>(result.ToString());

            //List<Models> models = new List<Models>();


            //右半页OCR后的识别判断,location.left的值只适用于河北省招生计划右半页
            if (path.Contains("right"))
            {
                //要定位left筛选出专业代号和专业名称，只测试过右半部分，有时会误选专业说明，是IEnumerable
                var majors = (from t in rt.words_result
                    where t.location.left > 0 && t.location.left < 250 && Regex.IsMatch(t.words, @"^\d{2}\D")
                              select t).ToList();

                //要定位left筛选出院校代号和院校名称，只测试过右半部分,是IEnumerable
                var colleges = (from t in rt.words_result
                    where t.location.left >= 0 && t.location.left < 60 &&
                          Regex.IsMatch(t.words, @"^\d{4}") &&
                          !t.words.Contains("代号")
                    select t).ToList();

                //要left及与院校名称差不多高度的top定位，筛选出院校招生人数
                var collegeNums = from t in rt.words_result
                    from c in colleges
                    where Math.Abs(t.location.top - c.location.top) < 30 && t.location.left > 600
                    select t.words;

                for (int i = 0; i < majors.Count; i++)
                {
                    //要left及与专业名称差不多高度的top定位，筛选出招生人数、学制和学费
                    string majorNum = (from t in rt.words_result
                        where Math.Abs(t.location.top - majors[i].location.top) < 40 && t.location.left > 600 
                                       select t.words).OrderBy(wordsResultItem => wordsResultItem.Length).LastOrDefault();

                    var college = (from c in colleges
                        where c.location.top - majors[i].location.top < 0
                        select c).LastOrDefault();

                    if (college != null)
                    {
                        string collegeNum = (from t in rt.words_result
                            where Math.Abs(t.location.top - college.location.top) < 40 && t.location.left > 600
                            select t.words).FirstOrDefault();
                        models.Add(new Models(majors[i].words, majorNum, college.words, collegeNum));
                    }
                    else
                    {
                        models.Add(models.Count > 0
                            ? new Models(majors[i].words, majorNum, models.Last().CollegeName,
                                models.Last().CollegeNum)
                            : new Models(majors[i].words, majorNum, string.Empty, string.Empty));
                    }


                    
                }

                //保存出现在页未的学校名，但下面没专业，留待下一页用
                if (!models.Last().Equals(colleges.Last()))
                {
                    models.Add(new Models(string.Empty, string.Empty, colleges.Last().words, collegeNums.Last()));
                }

                //foreach (var model in models)
                //{
                //    Console.WriteLine(model.MajorName + " " + model.MajorNum + " " + model.CollegeName + " " + model.CollegeNum);
                //}
            }
            //左半页OCR后的识别判断,location.left的值只适用于河北省招生计划左半页
            else if (path.Contains("left"))
            {
                var majors = (from t in rt.words_result
                    where t.location.left > 0 && t.location.left <= 250 && Regex.IsMatch(t.words,@"^\d{2}\D")
                    select t).ToList();

                //要定位left筛选出院校代号和院校名称，只适用左半部分,是IEnumerable
                var colleges = rt.words_result.Where(t =>
                    t.location.left >= 130 && t.location.left < 210 && Regex.IsMatch(t.words, @"^\d{4}") &&
                    !t.words.Contains("代号")).ToList();

                //要left及与院校名称差不多高度的top定位，筛选出院校招生人数
                var collegeNums = rt.words_result.SelectMany(t => colleges, (t, c) => new {t, c})
                    .Where(@t1 => Math.Abs(@t1.t.location.top - @t1.c.location.top) < 30 && @t1.t.location.left > 600)
                    .Select(@t1 => @t1.t.words).ToList();

                for (int i = 0; i < majors.Count; i++)
                {
                    //要left及与专业名称差不多高度的top定位，筛选出招生人数、学制和学费
                    string majorNum = (from t in rt.words_result
                                       where Math.Abs(t.location.top - majors[i].location.top) < 40 && t.location.left > 600
                                       select t.words).OrderBy(wordsResultItem => wordsResultItem.Length).LastOrDefault();

                    var college = (from c in colleges
                                   where c.location.top - majors[i].location.top < 0
                                   select c).LastOrDefault();

                    if (college != null)
                    {
                        string collegeNum = (from t in rt.words_result
                                             where Math.Abs(t.location.top - college.location.top) < 40 && t.location.left > 600
                                             select t.words).FirstOrDefault();
                        models.Add(new Models(majors[i].words, majorNum, college.words, collegeNum));
                    }
                    else
                    {
                        models.Add(models.Count > 0
                            ? new Models(majors[i].words, majorNum, models.Last().CollegeName,
                                models.Last().CollegeNum)
                            : new Models(majors[i].words, majorNum, string.Empty, string.Empty));
                    }



                }

                //保存出现在页未的学校名，但下面没专业，留待下一页用
                if (!models.Last().Equals(colleges.Last()))
                {
                    models.Add(new Models(string.Empty, string.Empty,colleges.Last().words, collegeNums.Last()));
                }

                //foreach (var model in models)
                //{
                //    Console.WriteLine(model.MajorName + " " + model.MajorNum + " " + model.CollegeName + " " + model.CollegeNum);
                //}
            }
            





        }
    }

    public class Location
    {
        /// <summary>
        /// 
        /// </summary>
        public int width { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int top { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int left { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int height { get; set; }
    }

    public class Probability
    {
        /// <summary>
        /// 
        /// </summary>
        public double variance { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public double average { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public double min { get; set; }
    }

    public class Words_resultItem
    {
        /// <summary>
        /// 文史本一特殊类型批
        /// </summary>
        public string words { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Location location { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Probability probability { get; set; }
    }

    public class Root
    {
        /// <summary>
        /// 
        /// </summary>
        public long log_id { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int direction { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int words_result_num { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<Words_resultItem> words_result { get; set; }
    }
}