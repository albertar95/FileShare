using Application.DTO.Folder;
using Application.DTO.User;
using AutoMapper;
using Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Profiles
{
    public class MapperProfile : Profile
    {
        public MapperProfile()
        {
            //user
            CreateMap<User, UserDTO>().ReverseMap();
            CreateMap<User, CreateUserDTO>();
            CreateMap<CreateUserDTO, User>().BeforeMap((s,d) => { d.CreateDate = DateTime.Now;d.Id = Guid.NewGuid(); });
            CreateMap<User, UpdateUserDTO>();
            CreateMap<UpdateUserDTO, User>().BeforeMap((s, d) => { d.LastModified = DateTime.Now; });
            //folder
            CreateMap<Folder, FolderDTO>().ReverseMap();
            CreateMap<Folder, CreateFolderDTO>();
            CreateMap<CreateFolderDTO, Folder>().BeforeMap((s, d) => { d.CreateDate = DateTime.Now; d.Id = Guid.NewGuid(); });
            CreateMap<Folder, UpdateFolderDTO>();
            CreateMap<UpdateFolderDTO, Folder>().BeforeMap((s, d) => { d.LastModified = DateTime.Now; });
        }
    }
}
