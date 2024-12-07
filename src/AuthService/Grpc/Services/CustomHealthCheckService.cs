using Grpc.Core;
using Grpc.Health.V1;

namespace AuthService.Grpc.Services;

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