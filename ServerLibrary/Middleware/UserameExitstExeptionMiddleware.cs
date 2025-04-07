//using Microsoft.AspNetCore.Http;
//using Microsoft.Extensions.Logging;
//using ServerLibrary.Exceptions;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Net;
//using System.Text;
//using System.Text.Json;
//using System.Threading.Tasks;

//namespace ServerLibrary.Middleware
//{
//    public class UserameExitstExeptionMiddleware
//    {
//        private readonly RequestDelegate _next;
//        private readonly ILogger<UserameExitstExeptionMiddleware> _logger;

//        public UserameExitstExeptionMiddleware(RequestDelegate next, ILogger<UserameExitstExeptionMiddleware> logger)
//        {
//            _next = next;
//            _logger = logger;
//        }

//        public async Task InvokeAsync(HttpContext httpContext)
//        {
//            try
//            {
//                await _next(httpContext);
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "Произошла ошибка при обработке запроса.");
//                await HandleExceptionUserExitsAsync(httpContext, ex);
//            }
//        }

//        private Task HandleExceptionUserExitsAsync(HttpContext context, Exception exception)
//        {
//            switch (exception)
//            {
//                case UsernameAlreadyExitstException:
//                    context.Response.StatusCode = (int)HttpStatusCode.Conflict; //409
//                    break;
//            }

//            //// Настроим тип ошибки в зависимости от исключения
//            //context.Response.ContentType = "application/json";
//            //context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

//            // Пример возвращаемого сообщения
//            var response = new { message = "Пользователь с таким логином уже сущетсвует", details = exception.Message };

//            // Выводим сообщение об ошибке
//            var jsonResponse = JsonSerializer.Serialize(response);
//            return context.Response.WriteAsync(jsonResponse);
//        }
//    }
//}
