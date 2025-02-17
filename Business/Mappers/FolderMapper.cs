
    using AutoMapper;
    using Domain.Entities;
    using Business.DTOs.Folder;
using Infrastructure;

    namespace Business.Mappers
    {
        public class FolderMapper : Profile
        {
            public FolderMapper()
            {
                CreateMap<FolderCreateDto, Folder>()
                .ForMember(dest => dest.OwnerId, opt => opt.MapFrom(src => WorkContext.CurrentUserId));
            CreateMap<FolderUpdateDto, Folder>()
                .ForMember(dest => dest.OwnerId, opt => opt.MapFrom(src => WorkContext.CurrentUserId));

        }
    }

}
