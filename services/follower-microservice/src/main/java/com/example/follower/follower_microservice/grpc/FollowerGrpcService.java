package com.example.follower.follower_microservice.grpc;

import com.example.follower.follower_microservice.service.FollowerService;
import com.example.follower.follower_microservice.grpc.FollowerServiceProto.*;
import io.grpc.stub.StreamObserver;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;
import org.springframework.stereotype.Component;

@Component
public class FollowerGrpcService extends FollowerServiceGrpc.FollowerServiceImplBase {

    private static final Logger log = LoggerFactory.getLogger(FollowerGrpcService.class);
    private final FollowerService followerService;

    public FollowerGrpcService(FollowerService followerService) {
        this.followerService = followerService;
    }

    @Override
    public void followUser(FollowUserRequest request, StreamObserver<FollowUserResponse> responseObserver) {
        log.info("=== GRPC FOLLOW USER START ===");
        log.info("Received gRPC follow request: followerId={}, followedId={}", 
                request.getFollowerId(), request.getFollowedId());

        try {
            boolean success = followerService.followUser(request.getFollowerId(), request.getFollowedId());
            
            FollowUserResponse response = FollowUserResponse.newBuilder()
                    .setSuccess(success)
                    .setMessage(success ? "Successfully followed user" : "Failed to follow user")
                    .build();

            log.info("Sending gRPC response: success={}, message={}", 
                    response.getSuccess(), response.getMessage());
            
            responseObserver.onNext(response);
            responseObserver.onCompleted();
            
        } catch (IllegalArgumentException e) {
            log.warn("Invalid follow request: {}", e.getMessage());
            FollowUserResponse response = FollowUserResponse.newBuilder()
                    .setSuccess(false)
                    .setMessage(e.getMessage())
                    .build();
            responseObserver.onNext(response);
            responseObserver.onCompleted();
            
        } catch (IllegalStateException e) {
            log.info("Already following: {}", e.getMessage());
            FollowUserResponse response = FollowUserResponse.newBuilder()
                    .setSuccess(false)
                    .setMessage(e.getMessage())
                    .build();
            responseObserver.onNext(response);
            responseObserver.onCompleted();
            
        } catch (Exception e) {
            log.error("Unexpected error in followUser", e);
            responseObserver.onError(e);
        }
        
        log.info("=== GRPC FOLLOW USER END ===");
    }

    @Override
    public void unfollowUser(UnfollowUserRequest request, StreamObserver<UnfollowUserResponse> responseObserver) {
        log.info("=== GRPC UNFOLLOW USER START ===");
        log.info("Received gRPC unfollow request: followerId={}, followedId={}", 
                request.getFollowerId(), request.getFollowedId());

        try {
            boolean success = followerService.unfollowUser(request.getFollowerId(), request.getFollowedId());
            
            UnfollowUserResponse response = UnfollowUserResponse.newBuilder()
                    .setSuccess(success)
                    .setMessage(success ? "Successfully unfollowed user" : "Failed to unfollow user")
                    .build();

            log.info("Sending gRPC response: success={}, message={}", 
                    response.getSuccess(), response.getMessage());
            
            responseObserver.onNext(response);
            responseObserver.onCompleted();
            
        } catch (Exception e) {
            log.error("Unexpected error in unfollowUser", e);
            responseObserver.onError(e);
        }
        
        log.info("=== GRPC UNFOLLOW USER END ===");
    }
}
