using AskForEtu.Core.Dto.Response;
using AskForEtu.Core.ResultStructure;
using AskForEtu.Core.Services;
using AskForEtu.Core.Services.Repo;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace AskForEtu.Repository.Services
{
    public class DepartmentService : IDepartmentService
    {
        private readonly IFacultRepository _facultRepository;
        private readonly IMajorRepository _majorRepository;
        private readonly IMapper _mapper;

        public DepartmentService(IMajorRepository majorRepository,
            IFacultRepository facultRepository,
            IMapper mapper)
        {
            _majorRepository=majorRepository;
            _facultRepository=facultRepository;
            _mapper=mapper;
        }

        public async Task<Response<List<FacultyDto>>> GetAllFacultyAsync()
        {
            try
            {
                var faculties = await _facultRepository
                    .GetAll(trackChanges: false)
                    .ToListAsync();

                var facultiesDto = _mapper.Map<List<FacultyDto>>(faculties);

                return Response<List<FacultyDto>>.Success(facultiesDto, 200);
            }
            catch (Exception)
            {
                return Response<List<FacultyDto>>.Success("Birseyler ters gitti.", 500);
            }
        }

        public async Task<Response<List<MajorDto>>> GetAllMajowByIdAsync(byte facultyId)
        {
            try
            {
                var majors = await _majorRepository
                    .GetByCondition(x => x.FacultyId == facultyId,trackChanges: false)
                    .ToListAsync();

                var majorsDto = _mapper.Map<List<MajorDto>>(majors);

                return Response<List<MajorDto>>.Success(majorsDto, 200);
            }
            catch (Exception)
            {

                return Response<List<MajorDto>>.Success("Birseyler ters gitti.", 500);
            }
        }
    }
}
