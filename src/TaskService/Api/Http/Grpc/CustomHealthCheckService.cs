using Grpc.Core;
using Grpc.Health.V1;

namespace TaskService.Api.Http.Grpc;

public class CustomHealthCheckService : Health.HealthBase
{
    public override Task<HealthCheckResponse> Check(HealthCheckRequest request, ServerCallContext context)
    {
        // Логика проверки состояния вашего сервиса
        var response = new HealthCheckResponse
        {
            Status = HealthCheckResponse.Types.ServingStatus.Serving // Вернуть статус сервиса
        };
        return Task.FromResult(response);
    }
}