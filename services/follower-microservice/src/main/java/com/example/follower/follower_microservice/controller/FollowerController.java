package com.example.follower.follower_microservice.controller;

import com.example.follower.follower_microservice.dto.FollowRequest;
import com.example.follower.follower_microservice.dto.FollowResponse;
import com.example.follower.follower_microservice.dto.UserFollowersDto;
import com.example.follower.follower_microservice.entity.User;
import com.example.follower.follower_microservice.service.FollowerService;
import jakarta.validation.Valid;
//import lombok.RequiredArgsConstructor;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;
import org.springframework.http.HttpStatus;
import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.*;

import java.util.List;
import java.util.Set;

@RestController
@RequestMapping("/api/followers")
//@RequiredArgsConstructor
public class FollowerController {

    private final FollowerService followerService;
    private static final Logger log = LoggerFactory.getLogger(FollowerController.class);

    public FollowerController(FollowerService followerService) {
        this.followerService = followerService;
    }

    @PostMapping("/follow")
    public ResponseEntity<FollowResponse> followUser(@Valid @RequestBody FollowRequest request) {
        // Guard: zabrani pracenje samog sebe
        if (request.getFollowerId().equals(request.getFollowedId())) {
            FollowResponse response = FollowResponse.builder()
                    .success(false)
                    .message("User cannot follow themselves")
                    .build();
            return ResponseEntity.status(HttpStatus.BAD_REQUEST).body(response);
        }

        // Guard: ako vec prati
        if (followerService.isFollowing(request.getFollowerId(), request.getFollowedId())) {
            FollowResponse response = FollowResponse.builder()
                    .success(false)
                    .message("User already follows the target user")
                    .build();
            return ResponseEntity.status(HttpStatus.CONFLICT).body(response);
        }

        boolean success = followerService.followUser(request.getFollowerId(), request.getFollowedId());

        FollowResponse response = FollowResponse.builder()
                .success(success)
                .message(success ? "Successfully followed user" : "Failed to follow user")
                .build();

        return ResponseEntity.status(success ? HttpStatus.OK : HttpStatus.BAD_REQUEST).body(response);
    }

    @PostMapping("/unfollow")
    public ResponseEntity<FollowResponse> unfollowUser(@Valid @RequestBody FollowRequest request) {
        log.info("=== UNFOLLOW CONTROLLER START ===");
        log.info("Received unfollow request: followerId={}, followedId={}", request.getFollowerId(), request.getFollowedId());
        
        boolean success = followerService.unfollowUser(request.getFollowerId(), request.getFollowedId());
        log.info("Unfollow service returned: {}", success);

        FollowResponse response = FollowResponse.builder()
                .success(success)
                .message(success ? "Successfully unfollowed user" : "Failed to unfollow user")
                .build();

        log.info("Sending response: success={}, message={}", response.isSuccess(), response.getMessage());
        log.info("=== UNFOLLOW CONTROLLER END ===");
        
        return ResponseEntity.status(success ? HttpStatus.OK : HttpStatus.BAD_REQUEST).body(response);
    }

    @GetMapping("/{userId}/following/ids")
    public ResponseEntity<List<Long>> getFollowingIds(@PathVariable Long userId) {
        List<Long> followingIds = followerService.getFollowingIds(userId);
        return ResponseEntity.ok(followingIds);
    }

    @GetMapping("/{userId}/followers/ids")
    public ResponseEntity<List<Long>> getFollowerIds(@PathVariable Long userId) {
        List<Long> followerIds = followerService.getFollowerIds(userId);
        return ResponseEntity.ok(followerIds);
    }

    @GetMapping("/{userId}/following")
    public ResponseEntity<Set<User>> getFollowing(@PathVariable Long userId) {
        Set<User> following = followerService.getFollowing(userId);
        return ResponseEntity.ok(following);
    }

    @GetMapping("/{userId}/followers")
    public ResponseEntity<Set<User>> getFollowers(@PathVariable Long userId) {
        Set<User> followers = followerService.getFollowers(userId);
        return ResponseEntity.ok(followers);
    }

    @GetMapping("/{userId}/info")
    public ResponseEntity<UserFollowersDto> getUserFollowInfo(@PathVariable Long userId) {
        UserFollowersDto info = followerService.getUserFollowInfo(userId);
        return ResponseEntity.ok(info);
    }

    @GetMapping("/check")
    public ResponseEntity<Boolean> checkFollowing(
            @RequestParam Long followerId,
            @RequestParam Long followedId) {
        boolean isFollowing = followerService.isFollowing(followerId, followedId);
        return ResponseEntity.ok(isFollowing);
    }

    @GetMapping("/{userId}/recommendations")
    public ResponseEntity<List<User>> getRecommendations(
            @PathVariable Long userId,
            @RequestParam(defaultValue = "10") int limit) {
        List<User> recommendations = followerService.getRecommendations(userId, limit);
        return ResponseEntity.ok(recommendations);
    }

    @PostMapping("/sync-user")
    public ResponseEntity<FollowResponse> syncUser(
            @RequestParam Long userId,
            @RequestParam(required = false) String username) {
        User user = followerService.createOrUpdateUser(userId, username);

        FollowResponse response = FollowResponse.builder()
                .success(true)
                .message("User synchronized successfully")
                .data(user)
                .build();

        return ResponseEntity.ok(response);
    }
}
