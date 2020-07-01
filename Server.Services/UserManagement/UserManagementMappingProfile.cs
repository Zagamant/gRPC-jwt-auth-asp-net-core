using AutoMapper;
using Server.DAL.Models;
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
        }
    }
}