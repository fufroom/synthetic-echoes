#!/bin/bash

# List of Festival voices to test
voices=(
  "voice_rab_diphone"
  "voice_us1_mbrola"
  "voice_us2_mbrola"
  "voice_us3_mbrola"
  "voice_awb_diphone"
)

# Test message
message="Hello, I am your 1995 AI therapist."

# Output directory for temporary test files
output_dir="/tmp/festival_tests"
mkdir -p "$output_dir"

echo "Testing Festival voices..."
for voice in "${voices[@]}"; do
  output_file="$output_dir/${voice}.wav"
  echo "Testing $voice..."
  echo "$message" | text2wave -o "$output_file" -eval "($voice)"
  aplay "$output_file"
done

echo "All voices tested. Temporary files are in $output_dir."
