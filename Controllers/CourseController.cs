using System;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using schoolRegistration.DTO;
using schoolRegistration.Models;

namespace schoolRegistration.Controllers {
    [Authorize]
    [Route ("api/[controller]")]
    public class CourseController : ControllerBase {
        private readonly IRegisterCourseRepository _repo;
        private readonly IMapper _mapper;
        public CourseController (IRegisterCourseRepository repo, IMapper mapper) {
            _mapper = mapper;
            _repo = repo;
        }

        [HttpPost ("addCourse")]
        public async Task<IActionResult> AddCourse ([FromBody] CourseForCreation courseForCreation) {
            if (courseForCreation == null)
                return BadRequest ("There is no student to be added !!");

            var checkCourse = await _repo.CheckCourseExist (courseForCreation.code);
            if (checkCourse != null)
                return BadRequest ("Course Already exist !!");

            var course = _mapper.Map<Course>(courseForCreation);
            _repo.Add (course);

            var courseToReturn = _mapper.Map<CourseToReturn> (courseForCreation);

            if (await _repo.SaveAll ())
                return CreatedAtRoute ("GetCourse", new { id = courseForCreation.id }, courseToReturn);

            throw new Exception ("Creating the student failed on save");
        }

        [HttpGet ("{id}", Name = "GetCourse")]
        public async Task<IActionResult> GetCouse (int id) {
            var course = await _repo.GetCourse (id);
            if (course == null) return NotFound ();
            return Ok (course);
        }

        [HttpPut ("{id}")]
        public async Task<IActionResult> UpdateCourse (int id, [FromBody] CourseForUpdate courseForUpdate) {
            var course = await _repo.GetCourse (id);

            _mapper.Map (courseForUpdate, course);

            if (await _repo.SaveAll ())
                return NoContent ();

            throw new Exception ($"Updating Course {id} failed on save");

        }

        [HttpGet]
        public IActionResult Get () {

            var CoursesList = _repo.GetCourses ();
            if (CoursesList != null)
                return Ok (CoursesList);

            throw new Exception ($"Error Occure while retrieving courses data");

        }

        [HttpDelete ("{id}")]
        public async Task<IActionResult> DeleteCourse (int id) {
            var course = await _repo.GetCourse(id);

            if (course != null)
                _repo.Delete(course);

            if (await _repo.SaveAll ())
                return NoContent ();

            throw new Exception ("Error while deleting this course");
        }
    }
}