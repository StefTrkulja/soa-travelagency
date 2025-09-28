package rs.ac.uns.ftn.informatika.jpa.DTO;

import java.time.OffsetDateTime;
import java.util.List;

public record BlogResponse (
        Long id, String title, String descriptionHtml, OffsetDateTime createdAt, List<String> imageUrls
){}
