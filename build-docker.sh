#!/bin/bash

set -e

GIT_SHA=$(git rev-parse HEAD)
IMAGE_NAME="planetariumhq/hand-royal"

docker build -t "$IMAGE_NAME:$GIT_SHA" .
docker tag "$IMAGE_NAME:$GIT_SHA" "$IMAGE_NAME:latest"

echo "Docker image built and tagged as:"
echo "  $IMAGE_NAME:$GIT_SHA"
echo "  $IMAGE_NAME:latest"
