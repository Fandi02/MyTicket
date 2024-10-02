using anbk.web.Application.Common.Exceptions;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using MyTicket.Application.Constant;
using MyTicket.Application.Models;
using MyTicket.Domain.Entities;
using MyTicket.WebApi.Services;

namespace MyTicket.WebApi.Controllers;

[ApiController]
public class BaseApiController : Controller
{
    private IMediator _mediator;
    private ApplicationJwtManager _applicationJwtManager;

    protected IMediator Mediator => _mediator ??= HttpContext.RequestServices.GetService<IMediator>();
    protected ApplicationJwtManager JwtManager => _applicationJwtManager ??= HttpContext.RequestServices.GetService<ApplicationJwtManager>();
    protected IMapper Mapper => HttpContext.RequestServices.GetService<IMapper>();

    [NonAction]
    public int GetPageFromQueryString()
    {
        if (Request.Query.ContainsKey("p"))
        {
            try
            {
                Request.Query.TryGetValue("p", out var pStr);

                return int.Parse(pStr);
            }
            catch
            {
                // do nothing, return base page
            }
        }

        return BasePagination.BasePage;
    }

    [NonAction]
    public int GetSizeFromQueryString()
    {
        if (Request.Query.ContainsKey("s"))
        {
            try
            {
                Request.Query.TryGetValue("s", out var pStr);

                return int.Parse(pStr);
            }
            catch
            {
                // do nothing, return base page
            }
        }

        return BasePagination.BaseSize;
    }

    [NonAction]
    public string GetQueryFromQueryString()
    {
        if (Request.Query.ContainsKey("q"))
        {
            try
            {
                Request.Query.TryGetValue("q", out var pStr);

                return pStr;
            }
            catch
            {
                // do nothing
            }
        }

        return null;
    }

    [NonAction]
    public Guid GetCurrentUserId()
    {
        if (!User.Identity.IsAuthenticated) throw new InvalidOperationException("If GetCurrentUserId is called, user must be authenticated");

        var str = User.Claims.FirstOrDefault(x => x.Type == ApplicationClaimConstant.UserId).Value;
        if (string.IsNullOrWhiteSpace(str)) throw new Exception("Claim user id is null");

        try
        {
            return Guid.Parse(str);
        }
        catch
        {
            throw;
        }
    }

    [NonAction]
    public string GetCurrentUserRole()
    {
        if (!User.Identity.IsAuthenticated) throw new InvalidOperationException("If GetCurrentUserRole is called, user must be authenticated");

        var str = User.Claims.FirstOrDefault(x => x.Type == ApplicationClaimConstant.Role).Value;
        if (string.IsNullOrWhiteSpace(str)) throw new Exception("Claim user role is null");

        try
        {
            return str;
        }
        catch
        {
            throw;
        }
    }

    [NonAction]
    public bool ValidateCurrentUserRole(string role, int roleOptions)
    {
        if (!User.Identity.IsAuthenticated) throw new InvalidOperationException("If ValidateCurrentUserRole is called, user must be authenticated");

        switch (roleOptions)
        {
            case 0:
                if (role != UserRoleEnum.Admin.ToString())
                {
                    throw new ForbiddenAccessException();
                }
                break;
            case 1:
                if (role != UserRoleEnum.User.ToString())
                {
                    throw new ForbiddenAccessException();
                }
                break;
            default:
                throw new ForbiddenAccessException();
        }

        return true;
    }

    [NonAction]
    public string GetCurrentUsername()
    {
        if (!User.Identity.IsAuthenticated) throw new InvalidOperationException("If GetCurrentUsername is called, user must be authenticated");

        var str = User.Claims.FirstOrDefault(x => x.Type == ApplicationClaimConstant.UserName).Value;
        if (string.IsNullOrWhiteSpace(str)) throw new Exception("Claim username is null");

        try
        {
            return str;
        }
        catch
        {
            throw;
        }
    }
}
