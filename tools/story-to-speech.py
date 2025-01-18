import json
import os
import re
import hashlib

# Define constants
INPUT_JSON_FILE = "../Assets/Resources/story.json"
OUTPUT_DIRECTORY = "../Assets/Resources/sounds/voice/"
SEED = 12345  # Default seed for deterministic random number generation

# Ensure the output directory exists
os.makedirs(OUTPUT_DIRECTORY, exist_ok=True)

def sanitize_text(text):
    """Sanitize the text by converting to lowercase and removing spaces/punctuation."""
    sanitized = re.sub(r'[^a-zA-Z0-9]', '', text.lower())
    return sanitized

def seeded_random(text, seed=12345):
    """Generate a deterministic random number based on a text seed."""
    sanitized = sanitize_text(text)
    seed_value = sum(ord(c) for c in sanitized) + seed  # Generate seed from sanitized text
    random_number = (seed_value * 1103515245 + 12345) & 0x7FFFFFFF  # LCG algorithm
    return random_number % 100000000  # Limit to 8 digits

def extract_spoken_lines(data, spoken_lines=None):
    """Recursively extract spoken lines (marked with '^') from the JSON structure."""
    if spoken_lines is None:
        spoken_lines = []

    if isinstance(data, dict):
        for key, value in data.items():
            extract_spoken_lines(value, spoken_lines)
    elif isinstance(data, list):
        for item in data:
            extract_spoken_lines(item, spoken_lines)
    elif isinstance(data, str) and data.startswith("^"):
        spoken_lines.append(data[1:])  # Remove the leading '^'

    return spoken_lines

def process_spoken_lines(lines):
    """Convert spoken lines to speech and save them as .wav files."""
    for line in lines:
        # Generate a deterministic filename
        filename = f"voice_{seeded_random(line)}.wav"
        filepath = os.path.join(OUTPUT_DIRECTORY, filename)

        # Skip if the file already exists
        if os.path.exists(filepath):
            print(f"Skipped existing file: {filepath}")
            continue

        # Generate speech with Festival
        print(f"Processing: {line}")
        command = f'echo "{line}" | text2wave -o "{filepath}" -eval "(voice_us2_mbrola)"'
        os.system(command)
        print(f"Saved: {filepath}")

def main():
    # Load the Ink story JSON with UTF-8-sig encoding to handle BOM
    with open(INPUT_JSON_FILE, "r", encoding="utf-8-sig") as f:
        story_data = json.load(f)

    # Extract spoken lines
    spoken_lines = extract_spoken_lines(story_data)

    # Convert to speech and save as .wav files
    process_spoken_lines(spoken_lines)

if __name__ == "__main__":
    main()
