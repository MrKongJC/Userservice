using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Zhaoxi.AgileFramework.Common.Models;
using Zhaoxi.AgileFramework.WebCore.FilterExtend;
using Zhaoxi.MSACommerce.Interface;
using Zhaoxi.MSACommerce.Model;

namespace Zhaoxi.MSACommerce.UserMicroservice.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private IUserService _userService;
        public UserController(IUserService userService)
        {
            _userService = userService;
        }
        [Route("query")]
        [HttpGet]
        public JsonResult QueryUser(string username, string password)
        {
            Console.WriteLine($"This is {typeof(UserController).Name}{nameof(QueryUser)} username={username} password={password}");
            AjaxResult<TbUser> ajaxResult = null;
            TbUser tbUser = _userService.QueryUser(username, password);

            ajaxResult = new AjaxResult<TbUser>()
            {
                Result = true,
                TValue = tbUser
            };
            return new JsonResult(ajaxResult);
        }
        [Route("/api/user/verify")]
        [HttpGet]
        [AllowAnonymousAttribute]//自己校验
        public JsonResult CurrentUser()
        {
            AjaxResult ajaxResult = null;
            IEnumerable<Claim> claimlist = HttpContext.AuthenticateAsync().Result.Principal.Claims;
            if (claimlist != null && claimlist.Count() > 0)
            {
                string username = claimlist.FirstOrDefault(u => u.Type == "username").Value;
                string id = claimlist.FirstOrDefault(u => u.Type == "id").Value;
                ajaxResult = new AjaxResult()
                {
                    Result = true,
                    Value = new
                    {
                        id = id,
                        username = username,
                    }
                };
            }
            else
            {
                ajaxResult = new AjaxResult()
                {
                    Result = false,
                    Message = "Token无效，请重新登陆"
                };
            }
            return new JsonResult(ajaxResult);
        }

        [Route("check/{data}/{type}")]
        [HttpGet]
        public JsonResult CheckData(string data, int type)
        {
            return new JsonResult(_userService.CheckData(data, type));
        }

        [Route("send")]
        [HttpPost]
        public JsonResult SendVerifyCode([FromForm] string phone)
        {
            //检查的时候，需要 ip--

            AjaxResult ajaxResult = this._userService.CheckPhoneNumberBeforeSend(phone);
            if (!ajaxResult.Result)//校验失败
            {
                return new JsonResult(ajaxResult);
            }
            else
            {
                return new JsonResult(_userService.SendVerifyCode(phone));
            }
        }

        [Route("register")]
        [HttpPost]
        [TypeFilter(typeof(CustomAction2CommitFilterAttribute))]
        public JsonResult Register([FromForm] TbUser user, [FromForm] string code)
        {
            _userService.Register(user, code);
            return new JsonResult(new AjaxResult()
            {
                Result = true,
                Message = "注册成功"
            });
        }

    }
}
