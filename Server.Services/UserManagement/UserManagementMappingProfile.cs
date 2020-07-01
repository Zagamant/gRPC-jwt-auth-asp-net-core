using AutoMapper;
using Google.Protobuf;
using Origin = Server.DAL.Models;
using Server.Services.Models.UserManagement;

namespace Server.Services.UserManagement
{
    public class UserManagementMappingProfile : Profile
    {
        public UserManagementMappingProfile()
        {
            CreateMap<User, UserSignUp>();
            CreateMap<UserSignUp, User>();
            CreateMap<User, UserSignIn>();
            CreateMap<UserSignIn, User>();
            CreateMap<User, UserUpdate>();
            CreateMap<UserUpdate, User>();

            CreateMap<Origin.User, User>()
                .ForMember(src => src.PasswordSalt,
                    dest => dest.MapFrom(
                        src => ByteString.CopyFrom(src.PasswordSalt)));

            CreateMap<User, Origin.User>()
                .ForMember(user => user.PasswordSalt,
                    opt => opt.MapFrom(
                        src => src.PasswordSalt.ToByteArray()));
        }
    }
}