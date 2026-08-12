-- Resu Fit — Database Schema (PostgreSQL)

CREATE TABLE users (
    id            UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    email         VARCHAR(255) UNIQUE NOT NULL,
    password_hash VARCHAR(255) NOT NULL,
    created_at    TIMESTAMPTZ NOT NULL DEFAULT now()
);

CREATE TABLE scans (
    id               UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    user_id          UUID NOT NULL REFERENCES users(id) ON DELETE CASCADE,
    resume_filename  VARCHAR(255),
    job_title        VARCHAR(255),
    match_score      NUMERIC(5,2) NOT NULL,       -- e.g. 78.50
    created_at       TIMESTAMPTZ NOT NULL DEFAULT now()
);

CREATE TABLE scan_red_flags (
    id          UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    scan_id     UUID NOT NULL REFERENCES scans(id) ON DELETE CASCADE,
    flag_type   VARCHAR(100) NOT NULL,   -- e.g. 'missing_keyword', 'no_metrics', 'formatting'
    description TEXT NOT NULL
);

CREATE TABLE scan_suggestions (
    id          UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    scan_id     UUID NOT NULL REFERENCES scans(id) ON DELETE CASCADE,
    suggestion  TEXT NOT NULL
);

-- Index for common lookup pattern (a user's scans)
CREATE INDEX idx_scans_user_id ON scans(user_id);
