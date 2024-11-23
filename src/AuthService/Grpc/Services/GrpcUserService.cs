using AuthService.Data;
using Grpc.Core;
using GrpcAuthService;
using Microsoft.EntityFrameworkCore;

namespace AuthService.Grpc.Services;

public class GrpcUserService(AuthServiceDbContext dbContext) : UserService.UserServiceBase
{
    public override async Task<GetUserInfoResponse> GetUserInfo(GetUserInfoRequest request, ServerCallContext context)
    {
        if (!Guid.TryParse(request.Id, out var id))
            throw new RpcException(Statuses.BadRequest("id must in uuid format"));

        var user = await dbContext.Users.FirstOrDefaultAsync(x => x.Id == id);
        if (user is null)
            throw new RpcException(Statuses.NotFound("user with such id not found"));

        return new GetUserInfoResponse()
        {
            Id = user.Id.ToString(),
            Login = user.Login,
            Name = user.Name,
            Surname = user.Surname
        };
    }
}

