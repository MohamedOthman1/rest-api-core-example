using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using schoolRegistration.DTO;
using schoolRegistration.Models;
using schoolRegistration.Repositories;

namespace schoolRegistration.Controllers {
    [AllowAnonymous]

    [Route ("api/[controller]")]
    public class AuthController : Controller {
        private readonly IConfiguration _config;
        private readonly IAuthRepository _repo;
        private readonly IMapper _mapper;
        public AuthController (IAuthRepository repo, IConfiguration config, IMapper mapper) {

            _repo = repo;
            _config = config;
            _mapper = mapper;
        }

        [HttpPost ("register")]
        public async Task<IActionResult> Register ([FromBody] UserForRegister userForRegister) {
            if (!string.IsNullOrEmpty (userForRegister.UserName))
                userForRegister.UserName = userForRegister.UserName.ToLower ();
            //validate request  
            if (!ModelState.IsValid)
                return BadRequest (ModelState);

            if (await _repo.UserExist (userForRegister.UserName))
                return BadRequest ("Username is already taken");

            var userToCreate = _mapper.Map<User> (userForRegister);

            var createUser = await _repo.Register (userToCreate, userForRegister.Password);

            return StatusCode (201);
        }

        [HttpPost ("login")]
        public async Task<IActionResult> Login ([FromBody] UserForLogin userForLogin) {
            var userFromRepo = await _repo.Login (userForLogin.Username.ToLower (), userForLogin.Password);

            if (userFromRepo == null)
                return Unauthorized ();

            //generate token
            var tokenHandler = new JwtSecurityTokenHandler ();
            var key = Encoding.ASCII.GetBytes (_config.GetSection ("AppSettings:Token").Value);
            var tokenDescriptor = new SecurityTokenDescriptor {
                Subject = new ClaimsIdentity (new Claim[] {

                new Claim (ClaimTypes.NameIdentifier, userFromRepo.id.ToString ()),
                new Claim (ClaimTypes.Name, userFromRepo.UserName)

                }),
                Expires = DateTime.Now.AddDays (1),
                SigningCredentials = new SigningCredentials (new SymmetricSecurityKey (key), SecurityAlgorithms.HmacSha512Signature)
            };
            var token = tokenHandler.CreateToken (tokenDescriptor);
            var tokenString = tokenHandler.WriteToken (token);
            return Ok (new { tokenString });
        }
    }
}