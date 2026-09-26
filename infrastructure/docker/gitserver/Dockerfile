FROM golang:1.25-bookworm AS gitaly-builder

ARG GITALY_VERSION=v19.4.1

RUN apt-get update && apt-get install -y --no-install-recommends \
        build-essential \
        pkg-config \
        meson \
        ninja-build \
        gettext \
        python3 \
        git \
        libcurl4-openssl-dev \
        libexpat1-dev \
        libpcre2-dev \
        libssl-dev \
        zlib1g-dev \
    && rm -rf /var/lib/apt/lists/*

WORKDIR /src

RUN git clone --branch "${GITALY_VERSION}" --depth 1 \
        https://gitlab.com/gitlab-org/gitaly.git \
        gitaly

WORKDIR /src/gitaly

RUN make build

FROM debian:bookworm-slim

ENV DEBIAN_FRONTEND=noninteractive

RUN apt-get update && apt-get install -y --no-install-recommends \
        git \
        openssh-server \
        nginx-light \
        fcgiwrap \
        spawn-fcgi \
        gettext-base \
        curl \
        ca-certificates \
    && rm -rf /var/lib/apt/lists/*

COPY --from=gitaly-builder /src/gitaly/_build/bin/ /usr/local/bin/

RUN useradd --system --create-home --shell /usr/bin/git-shell --home-dir /var/lib/git git \
    && mkdir -p /var/lib/git/repositories /run/sshd /etc/gitaly \
    && ssh-keygen -A

COPY sshd_config.include /etc/ssh/sshd_config.d/10-forge.conf
COPY authorized-keys-command.sh /usr/local/bin/authorized-keys-command.sh
COPY nginx-git-http.conf.template /etc/nginx/templates/git-http.conf.template
COPY gitaly.toml /etc/gitaly/config.toml
COPY entrypoint.sh /usr/local/bin/entrypoint.sh
COPY gitlab-internal-stub.conf /etc/nginx/sites-enabled/gitlab-internal-stub.conf

RUN echo -n "unused-dummy-secret" > /etc/gitaly/gitlab_shared_secret

RUN chmod +x /usr/local/bin/authorized-keys-command.sh /usr/local/bin/entrypoint.sh \
    && rm -f /etc/nginx/sites-enabled/default

EXPOSE 22 8080

VOLUME ["/var/lib/git"]

ENTRYPOINT ["/usr/local/bin/entrypoint.sh"]
