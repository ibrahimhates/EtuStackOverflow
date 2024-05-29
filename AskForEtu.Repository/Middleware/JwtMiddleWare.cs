using System.Text.Json;
using AskForEtu.Core.JwtGenerator;
using AskForEtu.Core.ResultStructure;
using AskForEtu.Core.ResultStructure.Dto;
using Microsoft.AspNetCore.Http;

namespace MicroBlog.Service.Middleware;

public class JwtMiddleWare
{
    private readonly RequestDelegate _next;
    private const string Separator = "/";
    private const string LoginPathName = "login";

    public JwtMiddleWare(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context, IJwtProvider jwtProvider)
    {
        bool? isPathLogin = context.Request.Path.Value?
            .Split(Separator)
            .Last()
            .Equals(LoginPathName);

        if (isPathLogin.Value)
        {
            await _next(context);
            return;
        }

        var token = context.Request.Headers["Authorization"]
            .FirstOrDefault()?
            .Split(" ")
            .Last();

        if (string.IsNullOrEmpty(token))
        {
            await _next(context);
            return;
        }

        var (result, _) = await jwtProvider.VerifyTokenAsync(token);

        if (!result)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsync(
                JsonSerializer.Serialize(Response<NoContent>.Fail("Invalid Token", StatusCodes.Status401Unauthorized))
            );
            return;
        }

        await _next(context);
    }
}