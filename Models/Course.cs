using System.Collections.Generic;

namespace schoolRegistration.Models {
    public class Course {
        public int id { get; set; }
        public string code { get; set; }
        public string corequisite_course { get; set; }
        public bool isMain { get; set; }
        public virtual ICollection<Registration> Registrations { get; set; }

    }
}