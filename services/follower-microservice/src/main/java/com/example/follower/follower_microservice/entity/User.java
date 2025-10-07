package com.example.follower.follower_microservice.entity;

import org.springframework.data.neo4j.core.schema.Id;
import org.springframework.data.neo4j.core.schema.Node;
import org.springframework.data.neo4j.core.schema.Relationship;

import java.util.HashSet;
import java.util.Set;

@Node("User")
public class User {
    @Id
    private Long userId;

    private String username;

    @Relationship(type = "FOLLOWS", direction = Relationship.Direction.OUTGOING)
    private Set<User> following = new HashSet<>();

    @Relationship(type = "FOLLOWS", direction = Relationship.Direction.INCOMING)
    private Set<User> followers = new HashSet<>();

    // Transient fields for counts (not stored in Neo4j)
    private transient Integer followersCount = 0;
    private transient Integer followingCount = 0;

    public User() {
        this.following = new HashSet<>();
        this.followers = new HashSet<>();
    }

    public User(Long userId, String username) {
        this.userId = userId;
        this.username = username;
        this.following = new HashSet<>();
        this.followers = new HashSet<>();
    }

    // Builder pattern bez Lombok-a
    public static Builder builder() {
        return new Builder();
    }

    public static class Builder {
        private Long userId;
        private String username;
        private Set<User> following = new HashSet<>();
        private Set<User> followers = new HashSet<>();

        public Builder userId(Long userId) {
            this.userId = userId;
            return this;
        }

        public Builder username(String username) {
            this.username = username;
            return this;
        }

        public Builder following(Set<User> following) {
            this.following = following;
            return this;
        }

        public Builder followers(Set<User> followers) {
            this.followers = followers;
            return this;
        }

        public User build() {
            User user = new User();
            user.userId = userId;
            user.username = username;
            user.following = following;
            user.followers = followers;
            return user;
        }
    }

    public void follow(User user) {
        if (user != null && !user.getUserId().equals(this.userId)) {
            this.following.add(user);
        }
    }

    public void unfollow(User user) {
        if (user != null) {
            this.following.remove(user);
        }
    }

    public Long getUserId() {
        return userId;
    }

    public String getUsername() {
        return username;
    }

    public Set<User> getFollowing() {
        return following;
    }

    public Set<User> getFollowers() {
        return followers;
    }

    public void setUserId(Long userId) {
        this.userId = userId;
    }

    public void setUsername(String username) {
        this.username = username;
    }

    public void setFollowing(Set<User> following) {
        this.following = following;
    }

    public void setFollowers(Set<User> followers) {
        this.followers = followers;
    }

    public Integer getFollowersCount() {
        return followersCount;
    }

    public void setFollowersCount(Integer followersCount) {
        this.followersCount = followersCount;
    }

    public Integer getFollowingCount() {
        return followingCount;
    }

    public void setFollowingCount(Integer followingCount) {
        this.followingCount = followingCount;
    }
}
