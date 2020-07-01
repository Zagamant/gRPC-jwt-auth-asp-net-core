using AutoMapper;
using Google.Protobuf;
using Server.Services.Models.UserManagement;
using Origin = Server.DAL.Models;

namespace Server.gRPC.Services
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