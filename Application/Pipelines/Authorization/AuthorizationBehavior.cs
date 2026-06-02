using CrossCuttingConcerns.Exceptions.Types;
using MediatR;
using Microsoft.AspNetCore.Http;
using Security.Constants;
using Security.Extensions;

namespace Application.Pipelines.Authorization;

public class AuthorizationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
     where TRequest : IRequest<TResponse>, ISecuredRequest
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public AuthorizationBehavior(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        List<string>? userRoleClaims = _httpContextAccessor.HttpContext.User.ClaimRoles();

        if (userRoleClaims == null)
        {
            throw new AuthorizationException("You are not authenticated.");
        }

        bool isNotMatchedAUserRoleClaimWithRequestRoles = string.IsNullOrEmpty(userRoleClaims
            .FirstOrDefault(userRoleClaim => userRoleClaim == GeneralOperationClaims.Admin || request.Roles.Any(role => role == userRoleClaim)));

        if (isNotMatchedAUserRoleClaimWithRequestRoles)
        {
            throw new AuthorizationException("You are not authorized to perform this action.");
        }

        TResponse response = await next();
        return response;
    }
}
