package com.example.follower.follower_microservice.config;

import com.example.follower.follower_microservice.grpc.FollowerGrpcService;
import io.grpc.Server;
import io.grpc.ServerBuilder;
import org.springframework.beans.factory.annotation.Value;
import org.springframework.context.annotation.Bean;
import org.springframework.context.annotation.Configuration;

import javax.annotation.PostConstruct;
import javax.annotation.PreDestroy;
import java.io.IOException;

@Configuration
public class ManualGrpcConfig {

    @Value("${grpc.server.port:9091}")
    private int grpcPort;

    private Server grpcServer;

    @Bean
    public Server grpcServer(FollowerGrpcService followerGrpcService) throws IOException {
        grpcServer = ServerBuilder.forPort(grpcPort)
                .addService(followerGrpcService)
                .build()
                .start();
        
        System.out.println("=== MANUAL gRPC Server started on port " + grpcPort + " ===");
        
        Runtime.getRuntime().addShutdownHook(new Thread(() -> {
            if (grpcServer != null) {
                grpcServer.shutdown();
            }
        }));
        
        return grpcServer;
    }
}
