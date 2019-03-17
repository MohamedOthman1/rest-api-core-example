using System.Collections.Generic;

namespace schoolRegistration.Models {
    public class Student {
        public int id { get; set; }

        public string FullName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public virtual ICollection<Registration> Registrations { get; set; }

    }
}