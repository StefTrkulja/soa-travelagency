package rs.ac.uns.ftn.informatika.jpa.Controller;

import com.fasterxml.jackson.databind.ObjectMapper;
import jakarta.validation.Valid;
import org.springframework.http.HttpStatus;
import org.springframework.http.MediaType;
import org.springframework.web.bind.annotation.*;
import org.springframework.web.multipart.MultipartFile;
import rs.ac.uns.ftn.informatika.jpa.DTO.BlogCreateRequest;
import rs.ac.uns.ftn.informatika.jpa.DTO.BlogResponse;
import rs.ac.uns.ftn.informatika.jpa.Service.BlogService;


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
            @RequestPart("payload") @Valid String payloadJson,
            @RequestPart(value = "images", required = false) List<MultipartFile> images
    ) throws Exception {
        var payload = objectMapper.readValue(payloadJson, BlogCreateRequest.class);
        return service.create(payload, images);
    }
}