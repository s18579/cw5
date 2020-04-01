using System;
using cw5.Services;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using cw5.Models;

namespace cw5.Controllers
{

    [ApiController]
    [Route("api/enrollments")]
    public class EnrollmentsController : ControllerBase
    {
        private readonly IStudentsDbService _stService;

        public EnrollmentsController(IStudentsDbService studentService)
        {
            _stService = studentService;
        }

        [HttpPost]
        public async Task<IActionResult> Register(Enrollv2 input)
        {
            var study = await _stService.GetStudy(input.Studies);
            if (study == null)
            {
                return BadRequest();
            }
            else
            {
                await _stService.Register(input.IndexNumber, input.FirstName, input.LastName, input.BirthDate, study.IdStudy);
                return StatusCode(201);
            }
        }

        [HttpPost("promotions")]
        public async Task<IActionResult> Promote(Enrollv1 input)
        {
            var enrollment = await _stService.GetEnroll(input.Studies, input.Semester);
            if (enrollment == null)
            {
                return NotFound();
            }
            else
            {
                await _stService.Promote(input.Studies, input.Semester);
                return StatusCode(201);
            }
        }
    }
}
