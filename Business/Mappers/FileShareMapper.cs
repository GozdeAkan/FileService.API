using AutoMapper;
using Business.DTOs.File;
using Domain.Entities;
using Infrastructure;
using FileShare = Domain.Entities.FileShare;

public class FileShareMapper : Profile
{
    public FileShareMapper()
    {
        CreateMap<FileShareDto, FileShare>()
                .ForMember(dest => dest.OwnerUserId, opt => opt.MapFrom(src => WorkContext.CurrentUserId));
    }
}
