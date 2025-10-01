package rs.ac.uns.ftn.informatika.jpa.Service;

import org.springframework.web.multipart.MultipartFile;
import rs.ac.uns.ftn.informatika.jpa.DTO.BlogCreateRequest;
import rs.ac.uns.ftn.informatika.jpa.DTO.BlogResponse;
import rs.ac.uns.ftn.informatika.jpa.DTO.BlogUpdateRequest;

import java.util.List;

public interface BlogService {
    BlogResponse create(Long authorId, BlogCreateRequest req, List<MultipartFile> images);
    List<BlogResponse> getAll();
    BlogResponse getById(Long id);
    BlogResponse update(Long id, Long authorId, BlogUpdateRequest req, List<MultipartFile> images);
    void delete(Long id, Long authorId);
}
