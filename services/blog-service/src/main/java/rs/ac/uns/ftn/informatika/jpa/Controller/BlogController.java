package rs.ac.uns.ftn.informatika.jpa.Controller;

import com.fasterxml.jackson.databind.ObjectMapper;
import jakarta.validation.Valid;
import org.springframework.http.HttpStatus;
import org.springframework.http.MediaType;
import org.springframework.web.bind.annotation.*;
import org.springframework.web.multipart.MultipartFile;
import rs.ac.uns.ftn.informatika.jpa.DTO.BlogCreateRequest;
import rs.ac.uns.ftn.informatika.jpa.DTO.BlogResponse;
import rs.ac.uns.ftn.informatika.jpa.DTO.BlogUpdateRequest;
import rs.ac.uns.ftn.informatika.jpa.Service.BlogService;

import java.util.ArrayList;
import java.util.List;

@RestController
@RequestMapping("/api/blogs")
public class BlogController {
    private final BlogService service;
    private final ObjectMapper objectMapper;
    public BlogController(BlogService service, ObjectMapper objectMapper)
    {
        this.service = service;
        this.objectMapper = objectMapper;
    }

    @PostMapping(consumes = MediaType.MULTIPART_FORM_DATA_VALUE)
    @ResponseStatus(HttpStatus.CREATED)
    public BlogResponse create(
            @RequestHeader("X-User-Id") Long authorId,
            @RequestPart("payload") @Valid String payloadJson,
            @RequestPart(value = "images", required = false) List<MultipartFile> images
    ) throws Exception {
        var payload = objectMapper.readValue(payloadJson, BlogCreateRequest.class);
        return service.create(authorId, payload, images);
    }

    @GetMapping
    public List<BlogResponse> getAll() {
        return service.getAll();
    }

    @PostMapping("/following")
    public List<BlogResponse> getFollowingBlogs(@RequestBody(required = false) List<Long> followingUserIds) {
        //System.out.println("Received followingUserIds: " + followingUserIds);
        if (followingUserIds == null) {
            followingUserIds = new ArrayList<>();
        }
        //System.out.println("Processed followingUserIds: " + followingUserIds);
        return service.getFollowingBlogs(followingUserIds);
    }

    @GetMapping("/my")
    public List<BlogResponse> getMyBlogs(@RequestHeader("X-User-Id") Long userId) {
        //System.out.println("Received userId for my blogs: " + userId);
        return service.getMyBlogs(userId);
    }

    @GetMapping("/{id}")
    public BlogResponse getById(@PathVariable Long id) {
        return service.getById(id);
    }

    @PutMapping(value = "/{id}", consumes = MediaType.MULTIPART_FORM_DATA_VALUE)
    public BlogResponse update(
            @PathVariable Long id,
            @RequestHeader("X-User-Id") Long authorId,
            @RequestPart("payload") @Valid String payloadJson,
            @RequestPart(value = "images", required = false) List<MultipartFile> images
    ) throws Exception {
        var payload = objectMapper.readValue(payloadJson, BlogUpdateRequest.class);
        return service.update(id, authorId, payload, images);
    }

    @DeleteMapping("/{id}")
    @ResponseStatus(HttpStatus.NO_CONTENT)
    public void delete(
            @PathVariable Long id,
            @RequestHeader("X-User-Id") Long authorId
    ) {
        service.delete(id, authorId);
    }
}