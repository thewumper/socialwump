# bin/bash
(cd api/wumpapi ; docker build . -t wumpapi:latest; docker images)
docker image tag wumpapi:latest roguefirework/wumpapi:latest
(cd dockercontainers ; docker compose stop ; docker compose up --force-recreate -d)
docker image push roguefirework/wumpapi:latest
