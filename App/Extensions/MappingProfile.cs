using App.Dtos;
using App.Dtos.FileDtos;
using App.Dtos.PersonInformationDtos;
using App.Dtos.UserDtos;
using AutoMapper;
using DataAccess.Entities;

namespace App.Extensions;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<UserDto, User>()
            .ReverseMap();
        CreateMap<UserIncludeDataDto, User>()
            .ForMember(dest => dest.PersonInformations, opt => opt
                .MapFrom(src => src.PersonInformationDtos))
            .ReverseMap();
        CreateMap<UserRegisterDto, User>()
            .ReverseMap();
        CreateMap<UserLoginDto, User>()
            .ReverseMap();

        CreateMap<PlaceOfResidenceDto, PlaceOfResidence>().ReverseMap();
        CreateMap<PlaceOfResidenceCreateDto, PlaceOfResidence>().ReverseMap();
        
        CreateMap<PersonInformationDto, PersonInformation>().ReverseMap();
        CreateMap<PersonInformationIncludeDataDto, PersonInformation>()
            .ForMember(dest => dest.PlaceOfResidence, opt => opt
                .MapFrom(src => src.PlaceOfResidenceDto))
            .ForMember(dest => dest.ProfilePhoto, opt => opt
                .MapFrom(src => src.ProfilePhotoDto)).ReverseMap();
        CreateMap<PersonInformationCreateDto, PersonInformation>()
            .ForMember(dest => dest.PlaceOfResidence, opt => opt
                .MapFrom(src => src.PlaceOfResidenceDto)).ReverseMap();
        CreateMap<PersonInformationUpdateDto, PersonInformation>()
            .ForMember(dest => dest.PlaceOfResidence, opt => opt
                .MapFrom(src => src.PlaceOfResidenceDto)).ReverseMap();

        CreateMap<ProfilePhotoCreateDto, ProfilePhoto>().ReverseMap();
        CreateMap<ProfilePhotoDto, ProfilePhoto>().ReverseMap();
    }
}