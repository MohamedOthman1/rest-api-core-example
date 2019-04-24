using System.Collections.Generic;
using System.Threading.Tasks;
using schoolRegistration.DTO;
using schoolRegistration.Models;

namespace schoolRegistration.Controllers
{
    public interface IRegisterCourseRepository
    {
         void Add<T>(T entity) where T : class;
         void Delete<T> (T entity) where T:class;

        Task<bool> SaveAll();

        Task<Student> GetStudent(int id);

        Task<Course> GetCourse(int id);

        List<Student> GetSudents();

        List<Course> GetCourses();

        Task<Student> CheckStudentExist(string fullName);


        Task<Course> CheckCourseExist(string code);

        Task<List<int>> CourseCountLessThanThree(int [] courseId);

        Task<List<int>> CourseAlreadyExist(int studentId , List<int> coursesId);

        Task<List<int>> CheckCourseCorequisite(int studentId , List<int> coursesId);

        Task<Registration> GetRegistration(int studentId, int courseId);

        Task<Registration> isMainExist(int studentId, string courseName);

        bool AddRegisterList(int studentId , List<int> coursesId);

    }
}