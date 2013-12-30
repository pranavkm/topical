using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.Http;
using System.Web.Security;
using Topical.Services;
using Topical.ViewModel;

namespace Topical.Controllers
{
    public class UsersController : ApiController
    {
        private readonly IUserService _userService;

        public IHttpActionResult Create(RegisterUserViewModel user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Models.User createdUser = _userService.Create(user.UserName, user.Password);
        }
    }
}