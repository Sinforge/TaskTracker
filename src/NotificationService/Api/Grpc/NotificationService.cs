using Grpc.Core;
using GrpcNotificationService;
using NotificationService.Data;

namespace NotificationService.Api.Grpc;

public class NotificationService(NotificationServiceDbContext dbContext) : GrpcNotificationService.NotificationService.NotificationServiceBase 
{
    public override async Task<NotificationResponse> SendNotification(NotificationRequest request, ServerCallContext context)
    {
        if (!Guid.TryParse(request.UserId, out var id))
            throw new RpcException(new Status(StatusCode.InvalidArgument, "id must be uuid format"));

        var notification = new Notification
        {
            Id = Guid.NewGuid(),
            Message = request.Message,
            UserId = id
        };
        await dbContext.Notifications.AddAsync(notification);
        await dbContext.SaveChangesAsync();

        return new NotificationResponse();
    }
}