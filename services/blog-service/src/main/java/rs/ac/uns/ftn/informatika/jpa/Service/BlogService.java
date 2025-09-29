package rs.ac.uns.ftn.informatika.jpa.Service;

import org.springframework.web.multipart.MultipartFile;
import rs.ac.uns.ftn.informatika.jpa.DTO.BlogCreateRequest;
import rs.ac.uns.ftn.informatika.jpa.DTO.BlogResponse;

import java.util.List;

public interface BlogService {
    BlogResponse create(Long authorId, BlogCreateRequest req, List<MultipartFile> images);
}
