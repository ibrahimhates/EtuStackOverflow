using AskForEtu.Core.Services;
using EtuStackOverflow.Controllers.Api.CustomControllerBase;
using Microsoft.AspNetCore.Mvc;

namespace EtuStackOverflow.Controllers.Api
{
    [Route("api/[controller]s")]
    [ApiController]
    public class DepartmentController : CustomController
    {
        private readonly IDepartmentService _departmentService;

        public DepartmentController(IDepartmentService departmentService)
        {
            _departmentService=departmentService;
        }

        [HttpGet("faculties")]
        public async Task<IActionResult> GetAllFaculty()
        { 
            var result = await _departmentService.GetAllFacultyAsync();

            return CreateActionResultInstance(result);
        }

        [HttpGet("majors/{id:int}")]
        public async Task<IActionResult> GetAllMajor(int id)
        {
            var result = await _departmentService.GetAllMajowByIdAsync((byte)id);

            return CreateActionResultInstance(result);
        }
    }
}
