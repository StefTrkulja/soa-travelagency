package rs.ac.uns.ftn.informatika.jpa.Service;

import org.springframework.stereotype.Service;
import org.springframework.transaction.annotation.Transactional;
import org.springframework.web.multipart.MultipartFile;
import rs.ac.uns.ftn.informatika.jpa.DTO.BlogCreateRequest;
import rs.ac.uns.ftn.informatika.jpa.DTO.BlogResponse;
import rs.ac.uns.ftn.informatika.jpa.DTO.BlogUpdateRequest;
import rs.ac.uns.ftn.informatika.jpa.Model.Blog;
import rs.ac.uns.ftn.informatika.jpa.Model.BlogImage;
import rs.ac.uns.ftn.informatika.jpa.Repository.BlogRepository;
import rs.ac.uns.ftn.informatika.jpa.Util.FileStorageService;
import rs.ac.uns.ftn.informatika.jpa.Util.MarkdownService;

import java.util.ArrayList;
import java.util.List;
import java.util.NoSuchElementException;

@Service
public class BlogServiceImpl implements BlogService {

    private final BlogRepository repo;
    private final MarkdownService md;
    private final FileStorageService storage;

    public BlogServiceImpl(BlogRepository repo, MarkdownService md, FileStorageService storage) {
        this.repo = repo; this.md = md; this.storage = storage;
    }

    @Override @Transactional
    public BlogResponse create(Long authorId, BlogCreateRequest req, List<MultipartFile> images) {
        Blog blog = new Blog();
        blog.setTitle(req.title());
        blog.setDescriptionMd(req.descriptionMarkdown());
        blog.setDescriptionHtml(md.toSafeHtml(req.descriptionMarkdown()));
        blog.setAuthorId(authorId);
        blog = repo.save(blog);

        var stored = storage.storeBlogImages(blog.getId(), images);
        var imgEntities = new ArrayList<BlogImage>();
        for (var s : stored) {
            var img = new BlogImage();
            img.setBlog(blog);
            img.setFileName(s.fileName());
            img.setUrl(s.url());
            img.setContentType(s.contentType());
            img.setSizeBytes(s.size());
            imgEntities.add(img);
        }
        blog.getImages().addAll(imgEntities);
        blog = repo.save(blog);

        return new BlogResponse(
                blog.getId(),
                blog.getTitle(),
                blog.getDescriptionHtml(),
                blog.getCreatedAt(),
                blog.getAuthorId(),
                blog.getImages().stream().map(BlogImage::getUrl).toList()
        );
    }

    @Override
    public List<BlogResponse> getAll() {
        return repo.findAll().stream()
                .map(this::mapToResponse)
                .toList();
    }

    @Override
    public BlogResponse getById(Long id) {
        Blog blog = repo.findById(id)
                .orElseThrow(() -> new NoSuchElementException("Blog not found with id: " + id));
        return mapToResponse(blog);
    }

    @Override @Transactional
    public BlogResponse update(Long id, Long authorId, BlogUpdateRequest req, List<MultipartFile> images) {
        Blog blog = repo.findById(id)
                .orElseThrow(() -> new NoSuchElementException("Blog not found with id: " + id));
        
        if (!blog.getAuthorId().equals(authorId)) {
            throw new IllegalArgumentException("You can only update your own blogs");
        }

        blog.setTitle(req.title());
        blog.setDescriptionMd(req.descriptionMarkdown());
        blog.setDescriptionHtml(md.toSafeHtml(req.descriptionMarkdown()));

        // Handle new images if provided
        if (images != null && !images.isEmpty()) {
            // Clear existing images
            blog.getImages().clear();
            
            // Store new images
            var stored = storage.storeBlogImages(blog.getId(), images);
            var imgEntities = new ArrayList<BlogImage>();
            for (var s : stored) {
                var img = new BlogImage();
                img.setBlog(blog);
                img.setFileName(s.fileName());
                img.setUrl(s.url());
                img.setContentType(s.contentType());
                img.setSizeBytes(s.size());
                imgEntities.add(img);
            }
            blog.getImages().addAll(imgEntities);
        }

        blog = repo.save(blog);
        return mapToResponse(blog);
    }

    @Override @Transactional
    public void delete(Long id, Long authorId) {
        Blog blog = repo.findById(id)
                .orElseThrow(() -> new NoSuchElementException("Blog not found with id: " + id));
        
        if (!blog.getAuthorId().equals(authorId)) {
            throw new IllegalArgumentException("You can only delete your own blogs");
        }

        repo.delete(blog);
    }

    private BlogResponse mapToResponse(Blog blog) {
        return new BlogResponse(
                blog.getId(),
                blog.getTitle(),
                blog.getDescriptionHtml(),
                blog.getCreatedAt(),
                blog.getAuthorId(),
                blog.getImages().stream().map(BlogImage::getUrl).toList()
        );
    }
}
