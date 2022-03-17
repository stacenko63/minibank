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
    
         [HttpGet("get")]
         public IEnumerable<UserDto> GetAll()
         {
             return _userService.GetAllUsers()
                 .Select(it => new UserDto
                 {
                     Id = it.Id,
                     Login = it.Login,
                     Email = it.Email,
                     HasBankAccounts = it.HasBankAccounts
                 });
         }
         
         
         
         [HttpGet("{id}")]
         public UserDto Get(string id)
         {
             var model = _userService.GetUser(id);
             return new UserDto
             {
                 Id = model.Id,
                 Login = model.Login,
                 Email = model.Email,
                 HasBankAccounts = model.HasBankAccounts
             };
         
         }

         [HttpPost]
         public void Create(UserDto model) 
         {
             _userService.CreateUser(new User
             {
                 Id = model.Id,
                 Login = model.Login,
                 Email = model.Email
             });
         }
         
         [HttpPut("{id}")]
         public void Update(string id, UserDto userDto)
         {
             _userService.UpdateUser(new User
             {
                 Id = userDto.Id,
                 Login = userDto.Login,
                 Email = userDto.Email,
                 HasBankAccounts = userDto.HasBankAccounts
             });
         }
         
         [HttpDelete("{id}")]
         public void Delete(string id)
         {
             _userService.DeleteUser(id);
         }

         // [HttpPatch("/{id}/activate")]
         // public void SetActive(string id)
         // {
         //     //_userService.SetActive(id);
         // }
        
    }
}