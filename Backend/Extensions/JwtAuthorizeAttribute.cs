using Backend.Models;
using Backend.Models.Enums;
using Backend.Services;
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
                    if (jWToken != null)
                    {
                        context.HttpContext.Items.TryAdd("user_id", jWToken.user_id);

                        if (_checkRole)
                        {
                            var userService = (IUserService)context.HttpContext.RequestServices.GetService(typeof(IUserService))!;
                            if (userService == null)
                            {
                                context.Result = new StatusCodeResult(StatusCodes.Status500InternalServerError);
                                return;
                            }
                            User? user = userService.GetByIdAsync(jWToken.user_id).Result;
                            
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