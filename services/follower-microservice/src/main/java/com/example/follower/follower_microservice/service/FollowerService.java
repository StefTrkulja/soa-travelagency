package com.example.follower.follower_microservice.service;

import com.example.follower.follower_microservice.dto.UserFollowersDto;
import com.example.follower.follower_microservice.entity.User;

import java.util.List;
import java.util.Set;
public interface FollowerService {

    boolean followUser(Long followerId, Long followedId);
    boolean unfollowUser(Long followerId, Long followedId);

    List<Long> getFollowingIds(Long userId);
    List<Long> getFollowerIds(Long userId);
    Set<User> getFollowing(Long userId);
    Set<User> getFollowers(Long userId);

    boolean isFollowing(Long followerId, Long followedId);
    UserFollowersDto getUserFollowInfo(Long userId);

    List<User> getRecommendations(Long userId, int limit);

    User createOrUpdateUser(Long userId, String username);
}
