using AutoMapper;
using schoolRegistration.DTO;
using schoolRegistration.Models;

namespace schoolRegistration.Helpers {
    public class AutoMapperProfiles : Profile {
        public AutoMapperProfiles () {

            CreateMap<StudentForUpdate, Student> ();

            CreateMap<UserForRegister, User> ();

            CreateMap<Student, StudentToReturn> ();

            CreateMap<CourseForUpdate, Course> ();

            CreateMap<CourseForCreation, Course> ();

            CreateMap<StudentForCreation, Student> ();
            
            CreateMap<Course, CourseToReturn> ();
        }
    }
}