using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using Minibank.Core;
using Minibank.Core.Domains.Users;
using Minibank.Core.Domains.Users.Services;
using Minibank.Web.Controllers.Users.Dto;

namespace Minibank.Web.Controllers.Users
{
     [ApiController]
     [Route("User")]
     public class UserController
     {
         private readonly IUserService _userService;
    
         public UserController(IUserService userService)
         {
             _userService = userService;
         }
    
         [HttpGet("Get")]
         public IEnumerable<UserDtoGet> GetAllUsers()
         {
             return _userService.GetAllUsers()
                 .Select(it => new UserDtoGet
                 {
                     Id = it.Id,
                     Login = it.Login,
                     Email = it.Email,
                 });
         }
         
         
         
         [HttpGet("{id}")]
         public UserDtoGet GetUser(int id)
         {
             var model = _userService.GetUser(id);
             return new UserDtoGet
             {
                 Id = model.Id,
                 Login = model.Login,
                 Email = model.Email,
             };
         
         }

         [HttpPost]
         public void CreateUser(UserDtoPostOrPut model) 
         {
             _userService.CreateUser(new User
             {
                 Login = model.Login,
                 Email = model.Email
             });
         }
         
         [HttpPut("{id}")]
         public void UpdateUser(int id, UserDtoPostOrPut userDtoGet)
         {
             _userService.UpdateUser(new User
             {
                 Id = id,
                 Login = userDtoGet.Login,
                 Email = userDtoGet.Email,
             });
         }
         
         [HttpDelete("{id}")]
         public void DeleteUser(int id)
         {
             _userService.DeleteUser(id);
         }

     }
}