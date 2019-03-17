namespace schoolRegistration.DTO
{
    public class CourseToReturn
    {
        public int id { get; set; }
        public string code { get; set; }
        public string corequisite_course { get; set; }
        public bool isMain { get; set; }

    }
}