package com.example.follower.follower_microservice.repository;


import com.example.follower.follower_microservice.entity.User;
import org.springframework.data.neo4j.repository.Neo4jRepository;
import org.springframework.data.neo4j.repository.query.Query;
import org.springframework.data.repository.query.Param;
import org.springframework.stereotype.Repository;

import java.util.List;
import java.util.Optional;
import java.util.Set;

@Repository
public interface UserRepository extends Neo4jRepository<User, Long> {
    // Pronadji korisnika sa njegovim pratiteljima i onima koje prati
    @Query("MATCH (u:User {userId: $userId}) " +
            "OPTIONAL MATCH (u)-[:FOLLOWS]->(following) " +
            "OPTIONAL MATCH (follower)-[:FOLLOWS]->(u) " +
            "RETURN u, collect(DISTINCT following) as following, collect(DISTINCT follower) as followers")
    Optional<User> findByUserIdWithRelations(@Param("userId") Long userId);

    // Dobavi sve korisnike koje određeni korisnik prati
    @Query("MATCH (u:User {userId: $userId})-[:FOLLOWS]->(following) " +
            "RETURN following")
    Set<User> findFollowingByUserId(@Param("userId") Long userId);

    // Dobavi sve pratioce odredjenog korisnika
    @Query("MATCH (follower)-[:FOLLOWS]->(u:User {userId: $userId}) " +
            "RETURN follower")
    Set<User> findFollowersByUserId(@Param("userId") Long userId);

    // Proveri da li korisnik A prati korisnika B
    @Query("MATCH (a:User {userId: $followerId})-[:FOLLOWS]->(b:User {userId: $followedId}) " +
            "RETURN COUNT(*) > 0")
    boolean isFollowing(@Param("followerId") Long followerId, @Param("followedId") Long followedId);

    // Preporuke za pracenje (Friends of Friends)
    @Query("MATCH (u:User {userId: $userId})-[:FOLLOWS]->(friend)-[:FOLLOWS]->(recommendation) " +
            "WHERE NOT (u)-[:FOLLOWS]->(recommendation) " +
            "AND recommendation.userId <> $userId " +
            "RETURN DISTINCT recommendation " +
            "LIMIT $limit")
    List<User> findRecommendations(@Param("userId") Long userId, @Param("limit") int limit);

    // Dobavi samo ID-jeve korisnika koje prati
    @Query("MATCH (u:User {userId: $userId})-[:FOLLOWS]->(following) " +
            "RETURN following.userId")
    List<Long> findFollowingUserIds(@Param("userId") Long userId);
}
