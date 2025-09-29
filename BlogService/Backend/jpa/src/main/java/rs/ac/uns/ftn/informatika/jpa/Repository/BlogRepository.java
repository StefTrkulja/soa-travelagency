package rs.ac.uns.ftn.informatika.jpa.Repository;

import org.springframework.data.jpa.repository.JpaRepository;
import rs.ac.uns.ftn.informatika.jpa.Model.Blog;

public interface BlogRepository extends JpaRepository<Blog, Long> {
}
