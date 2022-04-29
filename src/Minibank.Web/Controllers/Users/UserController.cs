using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using Minibank.Core;
using Minibank.Core.Domains.Users;
using Minibank.Core.Domains.Users.Services;
using Minibank.Web.Controllers.Users.Dto;

namespace Minibank.Web.Controllers.Users
{
     [ApiController]
     [Authorize]
     [Route("User")]
     public class UserController
     {
         private readonly IUserService _userService;
    
         public UserController(IUserService userService)
         {
             _userService = userService;
         }
    
         [HttpGet("Get")]
         //[Authorize(Roles = "admin")]
         public async Task<IEnumerable<UserDtoGet>> GetAllUsers()
         {
             var result =  await _userService.GetAllUsers();
             return result.Select(it => new UserDtoGet
                 {
                     Id = it.Id,
                     Login = it.Login,
                     Email = it.Email,
                 });
         }
         
         
         
         [HttpGet("{id}")]
         //[Authorize(Roles = "admin")]
         public async Task<UserDtoGet> GetUser(int id)
         {
             var model = await _userService.GetUser(id);
             return new UserDtoGet
             {
                 Id = model.Id,
                 Login = model.Login,
                 Email = model.Email,
             };
         }

         [HttpPost]
         //[Authorize(Roles = "admin")]
         public async Task CreateUser(UserDtoPostOrPut model) 
         {
             await _userService.CreateUser(new User
             {
                 Login = model.Login,
                 Email = model.Email
             });
         }
         
         [HttpPut("{id}")]
         //[Authorize(Roles = "admin")]
         public async Task UpdateUser(int id, UserDtoPostOrPut userDtoGet)
         {
             await _userService.UpdateUser(new User
             {
                 Id = id,
                 Login = userDtoGet.Login,
                 Email = userDtoGet.Email,
             });
         }
         
         [HttpDelete("{id}")]
         //[Authorize(Roles = "admin")]
         public async Task DeleteUser(int id)
         {
             await _userService.DeleteUser(id);
         }

     }
}