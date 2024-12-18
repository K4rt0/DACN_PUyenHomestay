using Backend.Models;
using Backend.Models.Enums;
using Backend.Services;
using DotEnv.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Backend.Extensions
{
    public class JwtAuthorizeAttribute : ActionFilterAttribute
    {
        private readonly UserRole? _userRole;
        private readonly bool _checkRole;

        public JwtAuthorizeAttribute()
        {
            _checkRole = false;
        }

        public JwtAuthorizeAttribute(UserRole userRole)
        {
            _userRole = userRole;
            _checkRole = true;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            ReturnApi returnApi = new();
            string? authorization = context.HttpContext.Request.Headers.Authorization.FirstOrDefault();
            if (authorization != null)
            {
                string[] strings = authorization.Split(' ');
                if (strings.Length == 2 && strings[0] == "Bearer")
                {
                    JWToken? jWToken = Utils.ParseJWT(strings[1]);
                    if (jWToken != null && jWToken.is_expired == false)
                    {
                        var customDataValue = jWToken.custom_data?.ToList()[0].Value;
                        int user_id;
                        if (customDataValue != null)
                        {
                            user_id = int.Parse(customDataValue.ToString()!);
                            context.HttpContext.Items.TryAdd("user_id", user_id);
                        }
                        else
                        {
                            returnApi.message = "Phiên đăng nhập không thể xác minh, vui lòng thử lại!";
                            context.Result = new ObjectResult(returnApi);
                            return;
                        }
                        context.HttpContext.Items.TryAdd("user_id", jWToken.custom_data?.ToList()[0].Value);

                        if (_checkRole)
                        {
                            context.HttpContext.Items.TryAdd("is_root", false);
                            var userService = (IUserService)context.HttpContext.RequestServices.GetService(typeof(IUserService))!;
                            if (userService == null)
                            {
                                context.Result = new StatusCodeResult(StatusCodes.Status500InternalServerError);
                                return;
                            }
                            if (user_id == 0 && (string)jWToken.custom_data!.ToList()[1].Value == EnvReader.Instance["ROOT_ADMIN_USER"])
                            {
                                context.HttpContext.Items["is_root"] = true;
                                return;
                            }
                            User? user = userService.GetByIdAsync(user_id).Result;

                            if (user == null || ((_userRole == UserRole.Manager || _userRole == UserRole.Staff) && user.role == UserRole.Customer))
                            {
                                returnApi.message = "Bạn không có quyền truy cập!";
                                context.Result = new ObjectResult(returnApi);
                            }
                        }
                        return;
                    }
                }
            }
            returnApi.message = "Phiên đăng nhập không thể xác minh, vui lòng thử lại!";
            context.Result = new ObjectResult(returnApi);
        }
    }
}