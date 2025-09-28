CREATE TABLE blogs (
                       id               BIGSERIAL PRIMARY KEY,
                       title            VARCHAR(200) NOT NULL,
                       description_md   TEXT NOT NULL,
                       description_html TEXT NOT NULL,
                       created_at       TIMESTAMPTZ NOT NULL DEFAULT now()
);

CREATE TABLE blog_images (
                             id           BIGSERIAL PRIMARY KEY,
                             blog_id      BIGINT NOT NULL REFERENCES blogs(id) ON DELETE CASCADE,
                             file_name    VARCHAR(255) NOT NULL,
                             url          TEXT NOT NULL,
                             content_type VARCHAR(127),
                             size_bytes   BIGINT
);