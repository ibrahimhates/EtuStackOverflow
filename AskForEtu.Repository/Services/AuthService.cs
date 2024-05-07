using AskForEtu.Core.Dto.Request;
using AskForEtu.Core.Entity;
using AskForEtu.Core.Services;
using AskForEtu.Core.Services.Repo;
using AskForEtu.Repository.UnitofWork;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AskForEtu.Repository.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public AuthService(IUserRepository userRepository, IMapper mapper, IUnitOfWork unitOfWork)
        {
            _userRepository=userRepository;
            _mapper=mapper;
            _unitOfWork=unitOfWork;
        }

        public async Task Register(RegisterDto registerDto)
        {
            var user = _mapper.Map<User>(registerDto);

            await _userRepository.CreateAsync(user);

            await _unitOfWork.SaveAsync();
        }
    }
}
