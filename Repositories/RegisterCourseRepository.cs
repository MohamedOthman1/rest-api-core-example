using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using schoolRegistration.Data;
using schoolRegistration.DTO;
using schoolRegistration.Models;

namespace schoolRegistration.Controllers {
    public class RegisterCourseRepository : IRegisterCourseRepository {
        private readonly DataContext _context;

        public RegisterCourseRepository (DataContext context) {
            _context = context;
        }

        public void Add<T> (T entity) where T : class {
            _context.Add (entity);
        }

        public void Delete<T> (T entity) where T : class {
            _context.Remove (entity);
        }

        public async Task<bool> SaveAll () {

            return await _context.SaveChangesAsync () > 0;
        }

        public async Task<Student> GetStudent (int id) {

            return await _context.Student.FirstOrDefaultAsync (x => x.id == id);

        }

        public List<Student> GetSudents () {

            return _context.Student.ToList ();
        }

        public async Task<Student> CheckStudentExist (string fullName) {
            return await _context.Student.FirstOrDefaultAsync (x => x.FullName == fullName);
        }

        public async Task<Course> GetCourse (int id) {
            return await _context.Courses.FirstOrDefaultAsync (x => x.id == id);
        }

        public List<Course> GetCourses () {
            return _context.Courses.ToList ();
        }

        public async Task<Course> CheckCourseExist (string code) {
            return await _context.Courses.FirstOrDefaultAsync (x => x.code == code);
        }

        public async Task<List<int>> CourseCountLessThanThree (int[] coursesId) {
            var courseApplied = new List<int> ();
            try {
                foreach (var courseId in coursesId) {
                    var checkCourse = _context.Registrations.Where (x => x.course_id == courseId).Select (x => x.course_id);
                    if (checkCourse.Count () >= 3) {
                        var courseCode = await _context.Courses.FirstOrDefaultAsync (x => x.id == checkCourse.First ());
                        return null;
                        // throw new Exception ($"Course {courseCode.code} can't be registered more than three times !!");
                    } else {
                        courseApplied.Add (courseId);
                    }
                }
            } catch (Exception e) {
                throw new Exception ("Error :" + e.Message);
            }
            return courseApplied;
        }

        public async Task<List<int>> CourseAlreadyExist (int studentId, List<int> coursesId) {
            var courseNotExist = new List<int> ();
            try {
                foreach (var courseId in coursesId) {
                    var rulesByCondition= await _context.Registrations.Where(a=>coursesId.Any(b=>b==a.course_id) && a.student_id == studentId).ToListAsync();  // new update
                    // var checkCourse = await _context.Registrations.FirstOrDefaultAsync (x => x.course_id == courseId && x.student_id == studentId);
                    if (rulesByCondition != null) {
                        return null;
                        //var courseCode = await _context.Courses.FirstOrDefaultAsync (x => x.id == checkCourse.course_id);
                        //throw new Exception ($"Course {courseCode.code} already exist !!");
                    } else {
                        courseNotExist.Add (courseId);
                    }
                }
            } catch (Exception e) {
                throw new Exception ("Error :" + e.Message);
            }
            return courseNotExist;
        }

        public async Task<List<int>> CheckCourseCorequisite (int studentId, List<int> coursesId) {

            var coursesForRegistration = new List<int> ();
            try {
                foreach (var courseId in coursesId) {
                    bool corequisite = false;
                    var checkCourse = await _context.Courses.FirstOrDefaultAsync (x => x.id == courseId && x.corequisite_course != null);
                    if (checkCourse == null) {
                        coursesForRegistration.Add (courseId);
                    } else {
                        foreach (var course_Id in coursesId) {
                            var courseCorequisite = await _context.Courses.FirstOrDefaultAsync (x => x.id == course_Id);
                            if (courseCorequisite.code == checkCourse.corequisite_course) {
                                coursesForRegistration.Add (course_Id);
                                corequisite = true;
                                continue;
                            }
                        }
                        if (corequisite == false)
                            return null;
                        //throw new Exception ($"You should add {checkCourse.code} both corequisite together!!");
                    }
                }
            } catch (Exception e) {
                throw new Exception ("Error :" + e.Message);
            }
            return coursesForRegistration;
        }

        public async Task<Registration> GetRegistration (int studentId, int courseId) {
            return await _context.Registrations.FirstOrDefaultAsync (x => x.student_id == studentId && x.course_id == courseId);
        }

        public bool AddRegisterList (int studentId, List<int> coursesId) {
            try {
                foreach (var courseId in coursesId) {
                    Registration registerCourse = new Registration {
                        student_id = studentId,
                        course_id = courseId,

                    };
                    _context.Registrations.AddAsync (registerCourse);
                }
                _context.SaveChangesAsync ();
            } catch (Exception e) {
                throw new Exception ("Error :" + e.Message);
            }
            return true;
        }

        public async Task<Registration> isMainExist(int studentId, string courseName)
        {
            var courseId = await _context.Courses.FirstOrDefaultAsync(x=>x.code == courseName);
            if(courseId !=null)
            {
                var checkIfDeleted = await _context.Registrations.FirstOrDefaultAsync(x=>x.course_id == courseId.id && x.student_id == studentId);
                return checkIfDeleted;

            }
            return null;
        }
    }
}