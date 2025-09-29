package rs.ac.uns.ftn.informatika.jpa.Util;

import org.springframework.stereotype.Component;

import com.vladsch.flexmark.html.HtmlRenderer;
import com.vladsch.flexmark.parser.Parser;
import com.vladsch.flexmark.util.ast.Node;
import org.owasp.html.PolicyFactory;
import org.owasp.html.Sanitizers;

@Component
public class MarkdownService {
    private final Parser parser = Parser.builder().build();
    private final HtmlRenderer renderer = HtmlRenderer.builder().build();
    private final PolicyFactory policy = Sanitizers.FORMATTING
            .and(Sanitizers.BLOCKS)
            .and(Sanitizers.LINKS)
            .and(Sanitizers.TABLES)
            .and(Sanitizers.IMAGES);

    public String toSafeHtml(String markdown) {
        Node doc = parser.parse(markdown);
        String html = renderer.render(doc);
        return policy.sanitize(html);
    }
}
