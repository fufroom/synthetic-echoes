#!/bin/bash

# Variables
BUILD_DIR="linux-builds" # Local directory containing the Linux build
CABINET_USER="synthetic-echoes"
CABINET_HOST="192.168.1.144"
CABINET_DEST_DIR="/home/synthetic-echoes/builds" # Destination directory on the cabinet server

# Ensure the build directory exists
if [ ! -d "$BUILD_DIR" ]; then
    echo "Error: Build directory '$BUILD_DIR' does not exist."
    exit 1
fi

# Ensure the destination directory exists on the remote server
ssh "${CABINET_USER}@${CABINET_HOST}" "mkdir -p ${CABINET_DEST_DIR}"

# Copy the build files to the cabinet
scp -r "${BUILD_DIR}/" "${CABINET_USER}@${CABINET_HOST}:${CABINET_DEST_DIR}"

if [ $? -eq 0 ]; then
    echo "Build successfully copied to ${CABINET_HOST}:${CABINET_DEST_DIR}"
else
    echo "Error: Failed to copy build to cabinet."
    exit 1
fi
