FROM openjdk:17-jdk-slim AS build

RUN apt-get update && apt-get install -y curl && \
    curl -fsSL https://archive.apache.org/dist/maven/maven-3/3.8.6/binaries/apache-maven-3.8.6-bin.tar.gz | tar -xzf - -C /opt && \
    ln -s /opt/apache-maven-3.8.6/bin/mvn /usr/local/bin/mvn

WORKDIR /app

COPY pom.xml .
RUN mvn dependency:go-offline -B

COPY src ./src
RUN mvn clean package -DskipTests

FROM openjdk:17-jdk-slim

WORKDIR /app

RUN mkdir -p uploads

COPY --from=build /app/target/*.jar app.jar

EXPOSE 8080

ENTRYPOINT ["java", "-jar", "app.jar"]