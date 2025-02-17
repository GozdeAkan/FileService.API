using AutoMapper;
using Business.DTOs.File;
using Domain.Entities;
using Infrastructure;
using File = Domain.Entities.File;

namespace Business.Mappers
{
    public class FileMapper : Profile
    {
        public FileMapper()
        {
            CreateMap<FileCreateDto, File>()
                .ForMember(dest => dest.OwnerId, opt => opt.MapFrom(src => WorkContext.CurrentUserId));
            CreateMap<FileUpdateDto, File>()
                .ForMember(dest => dest.OwnerId, opt => opt.MapFrom(src => WorkContext.CurrentUserId));
        }
    }
}
