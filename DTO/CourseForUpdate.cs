namespace schoolRegistration.DTO {
    public class CourseForUpdate {
        public string code { get; set; }
        public string corequisite_course { get; set; }
        public bool isMain { get; set; }

    }
}