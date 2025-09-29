package com.example.follower.follower_microservice.dto;

import lombok.AllArgsConstructor;
import lombok.Builder;
import lombok.Data;
import lombok.NoArgsConstructor;

import java.util.List;


public class UserFollowersDto {
    private Long userId;
    private List<Long> followingIds;
    private List<Long> followerIds;
    private int followingCount;
    private int followersCount;

    public UserFollowersDto() {}

    public UserFollowersDto(Long userId, List<Long> followingIds, List<Long> followerIds,
                            int followingCount, int followersCount) {
        this.userId = userId;
        this.followingIds = followingIds;
        this.followerIds = followerIds;
        this.followingCount = followingCount;
        this.followersCount = followersCount;
    }

    // Builder pattern bez Lombok-a
    public static Builder builder() {
        return new Builder();
    }

    public static class Builder {
        private Long userId;
        private List<Long> followingIds;
        private List<Long> followerIds;
        private int followingCount;
        private int followersCount;

        public Builder userId(Long userId) {
            this.userId = userId;
            return this;
        }

        public Builder followingIds(List<Long> followingIds) {
            this.followingIds = followingIds;
            return this;
        }

        public Builder followerIds(List<Long> followerIds) {
            this.followerIds = followerIds;
            return this;
        }

        public Builder followingCount(int followingCount) {
            this.followingCount = followingCount;
            return this;
        }

        public Builder followersCount(int followersCount) {
            this.followersCount = followersCount;
            return this;
        }

        public UserFollowersDto build() {
            return new UserFollowersDto(userId, followingIds, followerIds,
                    followingCount, followersCount);
        }
    }

    public Long getUserId() {
        return userId;
    }

    public List<Long> getFollowingIds() {
        return followingIds;
    }

    public List<Long> getFollowerIds() {
        return followerIds;
    }

    public int getFollowingCount() {
        return followingCount;
    }

    public int getFollowersCount() {
        return followersCount;
    }

    public void setUserId(Long userId) {
        this.userId = userId;
    }

    public void setFollowingIds(List<Long> followingIds) {
        this.followingIds = followingIds;
    }

    public void setFollowerIds(List<Long> followerIds) {
        this.followerIds = followerIds;
    }

    public void setFollowingCount(int followingCount) {
        this.followingCount = followingCount;
    }

    public void setFollowersCount(int followersCount) {
        this.followersCount = followersCount;
    }
}
