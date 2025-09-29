package rs.ac.uns.ftn.informatika.jpa.Model;

import jakarta.persistence.*;
import java.time.OffsetDateTime;
import java.util.ArrayList;
import java.util.List;

@Entity @Table(name="blogs")
public class Blog {
    @Id @GeneratedValue(strategy = GenerationType.IDENTITY)
    private Long id;

    @Column(nullable=false, length=200)
    private String title;

    @Column(name="description_md", nullable=false, columnDefinition="text")
    private String descriptionMd;

    @Column(name="description_html", nullable=false, columnDefinition="text")
    private String descriptionHtml;

    @Column(name="created_at", nullable=false)
    private OffsetDateTime createdAt = OffsetDateTime.now();

    @Column(name="author_id", nullable=false)
    private Long authorId;

    @OneToMany(mappedBy = "blog", cascade = CascadeType.ALL, orphanRemoval = true)
    private List<BlogImage> images = new ArrayList<>();

    public Long getId() {
        return id;
    }

    public String getTitle() {
        return title;
    }

    public String getDescriptionMd() {
        return descriptionMd;
    }

    public String getDescriptionHtml() {
        return descriptionHtml;
    }

    public OffsetDateTime getCreatedAt() {
        return createdAt;
    }

    public List<BlogImage> getImages() {
        return images;
    }

    public Long getAuthorId() {
        return authorId;
    }

    public void setId(Long id) {
        this.id = id;
    }

    public void setTitle(String title) {
        this.title = title;
    }

    public void setDescriptionMd(String descriptionMd) {
        this.descriptionMd = descriptionMd;
    }

    public void setDescriptionHtml(String descriptionHtml) {
        this.descriptionHtml = descriptionHtml;
    }

    public void setImages(List<BlogImage> images) {
        this.images = images;
    }

    public void setCreatedAt(OffsetDateTime createdAt) {
        this.createdAt = createdAt;
    }

    public void setAuthorId(Long authorId) {
        this.authorId = authorId;
    }
}
