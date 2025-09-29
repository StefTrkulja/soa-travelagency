package com.example.follower.follower_microservice.dto;

import jakarta.validation.constraints.NotNull;

public class FollowRequest {
    @NotNull(message = "Follower ID is required")
    private Long followerId;

    @NotNull(message = "Followed ID is required")
    private Long followedId;

    public FollowRequest() {}
    public FollowRequest(Long followerId, Long followedId) {
        this.followerId = followerId;
        this.followedId = followedId;
    }

    public Long getFollowerId() {
        return followerId;
    }

    public Long getFollowedId() {
        return followedId;
    }

    public void setFollowerId(@NotNull(message = "Follower ID is required") Long followerId) {
        this.followerId = followerId;
    }

    public void setFollowedId(@NotNull(message = "Followed ID is required") Long followedId) {
        this.followedId = followedId;
    }
}
