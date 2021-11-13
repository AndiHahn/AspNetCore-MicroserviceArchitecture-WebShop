using System;
using System.Threading.Tasks;
using Grpc.Core;
using Grpc.Core.Interceptors;
using Microsoft.Extensions.Logging;

namespace Webshop.ApiGateways.Bff.Aggregator.Infrastructure
{
    public class GrpcExceptionInterceptor : Interceptor
    {
        private readonly ILogger<GrpcExceptionInterceptor> logger;

        public GrpcExceptionInterceptor(
            ILogger<GrpcExceptionInterceptor> logger)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public override AsyncUnaryCall<TResponse> AsyncUnaryCall<TRequest, TResponse>(
            TRequest request,
            ClientInterceptorContext<TRequest, TResponse> context,
            AsyncUnaryCallContinuation<TRequest, TResponse> continuation)
        {
            var call = continuation(request, context);

            return new AsyncUnaryCall<TResponse>(
                HandleResponse(call.ResponseAsync),
                call.ResponseHeadersAsync,
                call.GetStatus,
                call.GetTrailers,
                call.Dispose);
        }

        private async Task<TResponse> HandleResponse<TResponse>(Task<TResponse> task)
        {
            try
            {
                var response = await task;
                return response;
            }
            catch (RpcException ex)
            {
                logger.LogError("Error calling via grpc.\nStatus: {s}\nDetail: {d}", ex.Status, ex.Message);
                return default;
            }
        }
    }
}
