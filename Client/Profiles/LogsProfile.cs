using AutoMapper;
using Client.Dtos;
using Client.Models;

namespace Client.Profiles
{
    public class LogsProfile : Profile
    {
        public LogsProfile()
        {
            CreateMap<Log, LogReadDto>();
        }
    }
}