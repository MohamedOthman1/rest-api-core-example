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
    public class StudentController : ControllerBase {
        private readonly IMapper _mapper;
        private readonly IRegisterCourseRepository _repo;

        public StudentController (IRegisterCourseRepository repo, IMapper mapper) {
            _repo = repo;
            _mapper = mapper;
        }

        [HttpPost ("addStudent")]
        public async Task<IActionResult> AddStudent ([FromBody] StudentForCreation studentForCreation) {
            if (studentForCreation == null)
                return BadRequest ("There is no student to be added !!");

            var checkStudent = await _repo.CheckStudentExist (studentForCreation.FullName);
            if (checkStudent != null)
                return BadRequest ("Student Already exist !!");
            var student = _mapper.Map<Student> (studentForCreation);

            _repo.Add (student);

            var studentToReturn = _mapper.Map<StudentToReturn> (studentForCreation);

            if (await _repo.SaveAll ())
                return CreatedAtRoute ("GetStudent", new { id = studentForCreation.id }, studentToReturn);

            throw new Exception ("Creating the student failed on save");

        }

        [HttpGet ("{id}", Name = "GetStudent")]
        public async Task<IActionResult> GetStudent (int id) {
            var student = await _repo.GetStudent (id);
            if (student == null) return NotFound ();

            return Ok (student);
        }

        [HttpDelete ("{id}")]
        public async Task<IActionResult> DeleteStudent (int id) {
            var student = await _repo.GetStudent (id);

            if (student != null)
                _repo.Delete (student);

            if (await _repo.SaveAll ())
                return NoContent ();

            throw new Exception ("Error while deleting this student");
        }

        [HttpPut ("{id}")]
        public async Task<IActionResult> UpdateStudent (int id, [FromBody] StudentForUpdate studentForUpdate) {
            var student = await _repo.GetStudent (id);

            _mapper.Map (studentForUpdate, student);

            if (await _repo.SaveAll ())
                return NoContent ();

            throw new Exception ($"Updating student {id} failed on save");

        }

        [HttpGet]
        public IActionResult Get () {

            var studentsList = _repo.GetSudents ();
            if (studentsList != null)
                return Ok (studentsList);

            throw new Exception ($"Error Occure while retrieving students data");

        }
    }
}