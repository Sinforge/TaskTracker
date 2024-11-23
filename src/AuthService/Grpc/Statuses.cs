using Grpc.Core;

namespace AuthService.Grpc;

public static class Statuses
{
    public static Status NotFound(string details) => new Status(StatusCode.NotFound, details);
    public static Status BadRequest(string details) => new Status(StatusCode.InvalidArgument, details);
}