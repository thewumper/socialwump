# bin/bash
(cd api/wumpapi ; docker build . -t wumpapi:latest ; docker images)
(cd dockercontainers ; docker compose stop ; docker compose start)
