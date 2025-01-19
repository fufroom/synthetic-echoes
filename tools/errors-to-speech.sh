#!/bin/bash

# Paths
ERRORS_FILE="../Assets/Resources/errors.json"
OUTPUT_DIR="../Assets/Resources/sounds/errors"

# Ensure the output directory exists
mkdir -p "$OUTPUT_DIR"

# Parse the JSON file and process each error message
jq -c '.[]' "$ERRORS_FILE" | while read -r error; do
  # Extract the ID and message
  id=$(echo "$error" | jq -r '.id')
  message=$(echo "$error" | jq -r '.message')

  # Output file path
  output_file="$OUTPUT_DIR/error-$id.wav"

  # Use espeak-ng to generate the wav file
  espeak-ng -v en-us+m2 -p 20 -s 250 -w "$output_file" "$message"

  echo "Generated: $output_file"
done
