package rs.ac.uns.ftn.informatika.jpa.Config;

import org.springframework.context.annotation.Configuration;
import org.springframework.web.servlet.config.annotation.*;

@Configuration
public class CorsConfig implements WebMvcConfigurer {
    @Override
    public void addCorsMappings(CorsRegistry reg) {
        reg.addMapping("/api/**")
                .allowedOrigins("http://localhost:3000","http://localhost:5173")
                .allowedMethods("*")
                .allowCredentials(true);
    }
}