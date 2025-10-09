using Gateway.Grpc;
using Grpc.Core;
using Microsoft.Extensions.Logging;

namespace Gateway.Services;

public interface IFollowerGrpcClient
{
    Task<FollowUserResponse> FollowUserAsync(long followerId, long followedId);
    Task<UnfollowUserResponse> UnfollowUserAsync(long followerId, long followedId);
}

public class FollowerGrpcClient : IFollowerGrpcClient
{
    private readonly FollowerService.FollowerServiceClient _client;
    private readonly ILogger<FollowerGrpcClient> _logger;

    public FollowerGrpcClient(FollowerService.FollowerServiceClient client, ILogger<FollowerGrpcClient> logger)
    {
        _client = client;
        _logger = logger;
    }

    public async Task<FollowUserResponse> FollowUserAsync(long followerId, long followedId)
    {
        _logger.LogInformation("=== GRPC CLIENT FOLLOW USER START ===");
        _logger.LogInformation("Calling gRPC FollowUser: followerId={}, followedId={}", followerId, followedId);

        try
        {
            var request = new FollowUserRequest
            {
                FollowerId = followerId,
                FollowedId = followedId
            };

            var response = await _client.FollowUserAsync(request);
            
            _logger.LogInformation("gRPC FollowUser response: success={}, message={}", 
                response.Success, response.Message);
            _logger.LogInformation("=== GRPC CLIENT FOLLOW USER END ===");
            
            return response;
        }
        catch (RpcException ex)
        {
            _logger.LogError(ex, "gRPC error calling FollowUser: {Status}", ex.Status);
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error calling FollowUser");
            throw;
        }
    }

    public async Task<UnfollowUserResponse> UnfollowUserAsync(long followerId, long followedId)
    {
        _logger.LogInformation("=== GRPC CLIENT UNFOLLOW USER START ===");
        _logger.LogInformation("Calling gRPC UnfollowUser: followerId={}, followedId={}", followerId, followedId);

        try
        {
            var request = new UnfollowUserRequest
            {
                FollowerId = followerId,
                FollowedId = followedId
            };

            var response = await _client.UnfollowUserAsync(request);
            
            _logger.LogInformation("gRPC UnfollowUser response: success={}, message={}", 
                response.Success, response.Message);
            _logger.LogInformation("=== GRPC CLIENT UNFOLLOW USER END ===");
            
            return response;
        }
        catch (RpcException ex)
        {
            _logger.LogError(ex, "gRPC error calling UnfollowUser: {Status}", ex.Status);
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error calling UnfollowUser");
            throw;
        }
    }
}
