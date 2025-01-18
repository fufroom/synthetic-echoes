import json
import os
import re
import subprocess

# Define constants
INPUT_JSON_FILE = "../Assets/Resources/dialogue.json"
OUTPUT_DIRECTORY = "../Assets/Resources/sounds/voice/"

# Ensure the output directory exists
os.makedirs(OUTPUT_DIRECTORY, exist_ok=True)

def sanitize_text(text):
    """Sanitize the text by converting to lowercase and removing spaces/punctuation."""
    sanitized = re.sub(r'[^a-zA-Z0-9]', '', text.lower())
    return sanitized

def generate_voice(line, node_id):
    """Generate a voice file for the given line using Festival and voice_us2_mbrola."""
    filename = f"voice_{node_id}.wav"
    filepath = os.path.join(OUTPUT_DIRECTORY, filename)

    # Skip if the file already exists
    if os.path.exists(filepath):
        print(f"Skipped existing file: {filepath}")
        return

    print(f"Processing: {line}")

    # Command to generate speech using Festival with voice_us2_mbrola
    command = f'echo "{line}" | text2wave -eval "(voice_us2_mbrola)" -o "{filepath}"'

    # Execute the command
    subprocess.run(command, shell=True, check=True)

    print(f"Saved: {filepath}")

def process_dialogue_nodes(dialogue_nodes):
    """Process all dialogue nodes and generate voice files."""
    for node in dialogue_nodes:
        if node.get("text_to_speech", False):
            line = node.get("body_text", "")
            generate_voice(line, node["id"])

def main():
    """Main function to process the dialogue JSON and generate voice files."""
    # Load the dialogue JSON file
    with open(INPUT_JSON_FILE, "r", encoding="utf-8-sig") as f:
        dialogue_nodes = json.load(f)

    # Process each node for TTS
    process_dialogue_nodes(dialogue_nodes)

if __name__ == "__main__":
    main()
