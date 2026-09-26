#!/usr/bin/env bash
set -euo pipefail

: "${AUTH_API_URL:?AUTH_API_URL must be set}"

# Render the nginx vhost with the current AUTH_API_URL baked in.
envsubst '${AUTH_API_URL}' \
    < /etc/nginx/templates/git-http.conf.template \
    > /etc/nginx/sites-enabled/git-http.conf

mkdir -p /run/sshd /var/run/gitaly
chown -R git:git /var/lib/git

spawn-fcgi -s /run/fcgiwrap.socket -U www-data -G www-data /usr/sbin/fcgiwrap
nginx

/usr/local/bin/gitaly /etc/gitaly/config.toml &

# sshd stays in the foreground so the container's lifecycle follows it and
# its logs go to stdout/stderr like everything else here.
exec /usr/sbin/sshd -D -e 
