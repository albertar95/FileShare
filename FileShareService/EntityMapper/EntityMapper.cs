using Application.DTO.Folder;
using Application.DTO.User;
using Application.Service.EntityMapper;
using AutoMapper;
using Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileShareService.EntityMapper
{
    public class EntityMapper : IEntityMapper
    {
        public Mapper _mapper;
        public EntityMapper()
        {
            _mapper = new AutoMapper.Mapper(new MapperConfiguration(cfg => {
                //user
                cfg.CreateMap<User, UserDTO>().AfterMap((s,d) => { d.Password = ""; });
                cfg.CreateMap<UserDTO, User>();
                cfg.CreateMap<User, CreateUserDTO>();
                cfg.CreateMap<CreateUserDTO, User>().BeforeMap((s, d) => { d.CreateDate = DateTime.Now; d.Id = Guid.NewGuid();d.IncorrectPasswordCount = 0; });
                cfg.CreateMap<User, UpdateUserDTO>();
                cfg.CreateMap<UpdateUserDTO, User>().BeforeMap((s, d) => { d.LastModified = DateTime.Now; });
                //folder
                cfg.CreateMap<Folder, FolderDTO>().BeforeMap((s,d) => { d.Username = s.User.Username; }).AfterMap((s, d) => { d.VirtualPath = ""; });
                cfg.CreateMap<FolderDTO, Folder>();
                cfg.CreateMap<Folder, CreateFolderDTO>();
                cfg.CreateMap<CreateFolderDTO, Folder>().BeforeMap((s, d) => { d.CreateDate = DateTime.Now; });
                cfg.CreateMap<Folder, UpdateFolderDTO>();
                cfg.CreateMap<UpdateFolderDTO, Folder>().BeforeMap((s, d) => { d.LastModified = DateTime.Now; });
            }));
        }

        public T EntityMap<T>(object source, T input = null) where T : class
        {
            if(input == null)
                return _mapper.DefaultContext.Mapper.Map<T>(source);
            else
                return _mapper.DefaultContext.Mapper.Map(source,input);
        }
    }
}
