namespace schoolRegistration.Models {
    public class Registration {
        public int id { get; set; }
        public int student_id { get; set; }
        public int course_id { get; set; }
        public virtual Student Student { get; set; }
        public virtual Course Course { get; set; }
    }
}