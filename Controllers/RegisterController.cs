using System;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using schoolRegistration.Models;

namespace schoolRegistration.Controllers {
    [Authorize]
    [Route ("api/[controller]")]
    public class RegisterController : ControllerBase {
        private readonly IRegisterCourseRepository _repo;
        private readonly IMapper _mapper;
        public RegisterController (IRegisterCourseRepository repo, IMapper mapper) {
            _mapper = mapper;
            _repo = repo;

        }

        [HttpPost ("{studentId}/{courseId}")]
        public async Task<IActionResult> RegisterCourse (int studentId, int[] courseId) {
            var courseCount = await _repo.CourseCountLessThanThree (courseId);
            if (courseCount == null)
                return BadRequest ("Course can't be registered more than three times");
            if (courseCount.Count > 0) {
                var courseExist = await _repo.CourseAlreadyExist (studentId, courseCount);
                if (courseExist == null)
                    return BadRequest ("Course Already Exist");
                if (courseExist.Count > 0) {
                    var checkCorequisite = await _repo.CheckCourseCorequisite (studentId, courseExist);
                    if (checkCorequisite == null)
                        return BadRequest ("You should both corequisite together");
                    var registerCourse = _repo.AddRegisterList (studentId, checkCorequisite);
                    if (registerCourse)
                        return NoContent ();

                }

            }

            return BadRequest ("Registration failed on save !!");

        }

        [HttpGet ("{studentId}/{courseId}", Name = "GetRegistration")]
        public async Task<IActionResult> GetRegistration (int studentId, int courseId) {
            var student = await _repo.GetRegistration (studentId, courseId);
            if (student == null) return NotFound ();

            return Ok (student);
        }

        [HttpDelete ("{studentId}/{courseId}")]
        public async Task<IActionResult> DeleteRegistration (int studentId, int courseId) {
            var course = await _repo.GetCourse (courseId);
            if (!course.isMain)
                return BadRequest ($"You should delete {course.corequisite_course} first");

            var studentCourse = await _repo.GetRegistration (studentId, courseId);
            if (studentCourse != null) {
                _repo.Delete (studentCourse);
                if (await _repo.SaveAll ())
                    return NoContent ();
            }
            return NoContent ();

            throw new Exception ("Error while deleting this student");
        }
    }

}