package com.example.follower.follower_microservice.service;

import com.example.follower.follower_microservice.dto.UserFollowersDto;
import com.example.follower.follower_microservice.entity.User;
import com.example.follower.follower_microservice.exception.UserNotFoundException;
import com.example.follower.follower_microservice.repository.UserRepository;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;
import org.springframework.stereotype.Service;
import org.springframework.transaction.annotation.Transactional;

import java.util.List;
import java.util.Set;
import java.util.stream.Collectors;

@Service
@Transactional
public class FollowerServiceImpl implements FollowerService {
    private final UserRepository userRepository;
    private static final Logger log = LoggerFactory.getLogger(FollowerServiceImpl.class);

    public FollowerServiceImpl(UserRepository userRepository) {
        this.userRepository = userRepository;
    }

    @Override
    public boolean followUser(Long followerId, Long followedId) {
        // Zabrani samopraćenje kao 400 Bad Request
        if (followerId.equals(followedId)) {
            log.warn("User {} tried to follow themselves", followerId);
            throw new IllegalArgumentException("User cannot follow themselves");
        }

        // Ako vec prati, vrati 409 Conflict
        if (isFollowing(followerId, followedId)) {
            log.info("User {} already follows user {}", followerId, followedId);
            throw new IllegalStateException("User already follows the target user");
        }

        // Pronadji ili kreiraj oba korisnika
        User follower = userRepository.findById(followerId)
                .orElseGet(() -> createOrUpdateUser(followerId, "User" + followerId));

        User followed = userRepository.findById(followedId)
                .orElseGet(() -> createOrUpdateUser(followedId, "User" + followedId));

        // Dodaj pracenje
        follower.follow(followed);
        userRepository.save(follower);

        log.info("User {} now follows user {}", followerId, followedId);
        return true;
    }

    @Override
    public boolean unfollowUser(Long followerId, Long followedId) {
        log.info("=== UNFOLLOW REQUEST START ===");
        log.info("Follower ID: {}, Followed ID: {}", followerId, followedId);
        
        try {
            // Zabrani samopraćenje
            if (followerId.equals(followedId)) {
                log.warn("User {} tried to unfollow themselves", followerId);
                return false;
            }

            // Proveri da li prati korisnika
            boolean isCurrentlyFollowing = isFollowing(followerId, followedId);
            log.info("Is user {} currently following user {}? {}", followerId, followedId, isCurrentlyFollowing);
            
            if (!isCurrentlyFollowing) {
                log.info("User {} does not follow user {}", followerId, followedId);
                return false;
            }

            // Direktno brisanje FOLLOWS relacije izmedju dva korisnika
            log.info("Deleting FOLLOWS relationship between {} and {}", followerId, followedId);
            userRepository.deleteFollowRelationship(followerId, followedId);
            log.info("Successfully deleted FOLLOWS relationship");

            log.info("User {} successfully unfollowed user {}", followerId, followedId);
            log.info("=== UNFOLLOW REQUEST END ===");
            return true;

        } catch (Exception e) {
            log.error("Error while unfollowing user: {}", e.getMessage(), e);
            log.info("=== UNFOLLOW REQUEST END (ERROR) ===");
            return false;
        }
    }

    @Override
    @Transactional(readOnly = true)
    public List<Long> getFollowingIds(Long userId) {
        return userRepository.findFollowingUserIds(userId);
    }

    @Override
    @Transactional(readOnly = true)
    public List<Long> getFollowerIds(Long userId) {
        Set<User> followers = userRepository.findFollowersByUserId(userId);
        return followers.stream()
                .map(User::getUserId)
                .collect(Collectors.toList());
    }

    @Override
    @Transactional(readOnly = true)
    public Set<User> getFollowing(Long userId) {
        Set<User> following = userRepository.findFollowingByUserId(userId);
        // Za svakog korisnika, učitaj broj followers i following
        for (User user : following) {
            user.setFollowersCount(userRepository.findFollowersByUserId(user.getUserId()).size());
            user.setFollowingCount(userRepository.findFollowingByUserId(user.getUserId()).size());
        }
        return following;
    }

    @Override
    @Transactional(readOnly = true)
    public Set<User> getFollowers(Long userId) {
        Set<User> followers = userRepository.findFollowersByUserId(userId);
        // Za svakog korisnika, učitaj broj followers i following
        for (User user : followers) {
            user.setFollowersCount(userRepository.findFollowersByUserId(user.getUserId()).size());
            user.setFollowingCount(userRepository.findFollowingByUserId(user.getUserId()).size());
        }
        return followers;
    }

    @Override
    @Transactional(readOnly = true)
    public boolean isFollowing(Long followerId, Long followedId) {
        return userRepository.isFollowing(followerId, followedId);
    }

    @Override
    @Transactional(readOnly = true)
    public UserFollowersDto getUserFollowInfo(Long userId) {
        List<Long> followingIds = getFollowingIds(userId);
        List<Long> followerIds = getFollowerIds(userId);

        return UserFollowersDto.builder()
                .userId(userId)
                .followingIds(followingIds)
                .followerIds(followerIds)
                .followingCount(followingIds.size())
                .followersCount(followerIds.size())
                .build();
    }

    @Override
    @Transactional(readOnly = true)
    public List<User> getRecommendations(Long userId, int limit) {
        List<User> recommendations = userRepository.findRecommendations(userId, limit);
        // Za svakog korisnika, učitaj broj followers i following
        for (User user : recommendations) {
            user.setFollowersCount(userRepository.findFollowersByUserId(user.getUserId()).size());
            user.setFollowingCount(userRepository.findFollowingByUserId(user.getUserId()).size());
        }
        return recommendations;
    }

    @Override
    public User createOrUpdateUser(Long userId, String username) {
        User user = userRepository.findById(userId)
                .orElse(User.builder()
                        .userId(userId)
                        .username(username)
                        .build());

        if (username != null && !username.equals(user.getUsername())) {
            user.setUsername(username);
        }

        return userRepository.save(user);
    }
}
