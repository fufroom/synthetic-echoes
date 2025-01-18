import json
import os
import re
import hashlib
import subprocess

# Define constants
INPUT_JSON_FILE = "../Assets/Resources/dialogue.json"
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

def generate_voice(line, node_id, speaker):
    """Generate a voice file for the given line and speaker."""
    filename = f"voice_{node_id}.wav"
    filepath = os.path.join(OUTPUT_DIRECTORY, filename)

    # Skip if the file already exists
    if os.path.exists(filepath):
        print(f"Skipped existing file: {filepath}")
        return

    print(f"Processing: {line}")

    # Use Mozilla TTS for "gloria" and a default voice otherwise
    if speaker == "gloria":
        command = f'tts --text "{line}" --model_name "tts_models/en/ljspeech/tacotron2-DDC" --speaker_idx 1 --out_path "{filepath}"'
    else:
        command = f'tts --text "{line}" --model_name "tts_models/en/ljspeech/tacotron2-DDC" --out_path "{filepath}"'

    # Execute the command
    subprocess.run(command, shell=True, check=True)

    print(f"Saved: {filepath}")

def process_dialogue_nodes(dialogue_nodes):
    """Process all dialogue nodes and generate voice files."""
    for node in dialogue_nodes:
        if node.get("text_to_speech", False):
            line = node.get("body_text", "")
            speaker = node.get("speaker", None)
            generate_voice(line, node["id"], speaker)

def main():
    """Main function to process the dialogue JSON and generate voice files."""
    # Load the dialogue JSON file
    with open(INPUT_JSON_FILE, "r", encoding="utf-8-sig") as f:
        dialogue_nodes = json.load(f)

    # Process each node for TTS
    process_dialogue_nodes(dialogue_nodes)

if __name__ == "__main__":
    main()
