namespace BaiduOCR
{
    public class Models
    {
        public string MajorName { get; set; }
        public string MajorNum { get; set; }
        public string CollegeName { get; set; }
        public string CollegeNum { get; set; }

        public Models(string majorName, string majorNum, string collegeName, string collegeNum)
        {
            MajorName = majorName;
            MajorNum = majorNum;
            CollegeName = collegeName;
            CollegeNum = collegeNum;
        }
    }

    public class ZSJH
    {
        public string MajorName { get; set; }
        public string CollegeNum { get; set; }
        public string CollegeName { get; set; }
        public string Persons { get; set; }
        public string XueNian { get; set; }
        public string Fee { get; set; }

        public ZSJH(string majorName, string collegeNum, string collegeName, string persons,string xueNian,string fee)
        {
            MajorName = majorName;
            CollegeNum = collegeNum;
            CollegeName = collegeName;
            Persons = persons;
            XueNian = xueNian;
            Fee = fee;
        }

        public ZSJH()
        {
            
        }
    }
}