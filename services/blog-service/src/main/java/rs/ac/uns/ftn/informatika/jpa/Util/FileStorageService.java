package rs.ac.uns.ftn.informatika.jpa.Util;

import org.springframework.beans.factory.annotation.Value;
import org.springframework.stereotype.Service;
import org.springframework.web.multipart.MultipartFile;

import java.nio.file.*;
import java.util.ArrayList;
import java.util.List;
import java.util.UUID;

@Service
public class FileStorageService {
    private final Path root;
    private final String publicBaseUrl;

    public FileStorageService(
            @Value("${app.storage.local-root}") String rootDir,
            @Value("${app.storage.public-base-url}") String baseUrl) {
        this.root = Paths.get(rootDir).toAbsolutePath();
        this.publicBaseUrl = baseUrl.endsWith("/") ? baseUrl.substring(0, baseUrl.length()-1) : baseUrl;
    }

    public List<StoredFile> storeBlogImages(Long blogId, List<MultipartFile> files) {
        List<StoredFile> out = new ArrayList<>();
        if (files == null) return out;
        try {
            Path dir = root.resolve("blogs").resolve(String.valueOf(blogId));
            Files.createDirectories(dir);
            for (MultipartFile f : files) {
                if (f.isEmpty()) continue;
                String safeName = UUID.randomUUID() + "_" + Path.of(f.getOriginalFilename()).getFileName().toString();
                Path dest = dir.resolve(safeName);
                Files.copy(f.getInputStream(), dest, StandardCopyOption.REPLACE_EXISTING);
                String url = publicBaseUrl + "/blogs/" + blogId + "/" + safeName;
                out.add(new StoredFile(safeName, url, f.getContentType(), f.getSize()));
            }
            return out;
        } catch (Exception e) {
            throw new RuntimeException("Storing files failed", e);
        }
    }

    public record StoredFile(String fileName, String url, String contentType, long size) {}
}