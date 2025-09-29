package rs.ac.uns.ftn.informatika.jpa.Config;

import org.springframework.beans.factory.annotation.Value;
import org.springframework.context.annotation.Configuration;
import org.springframework.web.servlet.config.annotation.*;

import java.nio.file.Path;
import java.nio.file.Paths;

@Configuration
public class StaticFilesConfig implements WebMvcConfigurer {
    @Value("${app.storage.local-root}") String root;
    @Override
    public void addResourceHandlers(ResourceHandlerRegistry reg) {
        Path base = Paths.get(root).toAbsolutePath();
        reg.addResourceHandler("/files/**")
                .addResourceLocations("file:" + base.toString() + "/");
    }
}
