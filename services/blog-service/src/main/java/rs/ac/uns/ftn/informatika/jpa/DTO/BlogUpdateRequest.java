package rs.ac.uns.ftn.informatika.jpa.DTO;

import jakarta.validation.constraints.NotBlank;
import jakarta.validation.constraints.Size;

public record BlogUpdateRequest (
    @NotBlank
    @Size(max=200) String title,
    @NotBlank String descriptionMarkdown
){}
