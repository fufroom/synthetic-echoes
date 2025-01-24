import json
import os
import subprocess

# Define JSON files and their corresponding output filename prefixes
JSON_FILES = {
    "/mnt/big-home/Projects/Synthetic_Echoes/Assets/Resources/Continue.json": ("Continue", "text"),
    "/mnt/big-home/Projects/Synthetic_Echoes/Assets/Resources/MainPrompts.json": ("MainPrompts", "prompt"),
    "/mnt/big-home/Projects/Synthetic_Echoes/Assets/Resources/PromptResponses.json": ("PromptResponses", "text"),
    "/mnt/big-home/Projects/Synthetic_Echoes/Assets/Resources/dialogue.json": ("voice", "body_text")
}

# Output directory for generated voice files
OUTPUT_DIRECTORY = "/mnt/big-home/Projects/Synthetic_Echoes/Assets/Resources/sounds/voice/"

# Ensure the output directory exists
os.makedirs(OUTPUT_DIRECTORY, exist_ok=True)

def generate_voice(line, filename):
    """Generate a voice file for the given line using Festival and voice_us2_mbrola."""
    filepath = os.path.join(OUTPUT_DIRECTORY, filename)

    if os.path.exists(filepath):
        print(f"Skipped existing file: {filepath}")
        return

    print(f"Generating voice for: {line}")

    command = f'echo "{line}" | text2wave -eval "(voice_us2_mbrola)" -o "{filepath}"'
    result = subprocess.run(command, shell=True, capture_output=True, text=True)

    if result.returncode == 0:
        print(f"Saved: {filepath}")
    else:
        print(f"Error generating speech for {filepath}: {result.stderr}")

def generate_scanner_prompt():
    """Generate the one-off SCANNER_PROMPT.wav file."""
    scanner_text = (
        "Place your left hand on the palm scanner. Then, use your right hand to press the button "
        "for option 1 or option 2—choose the one that feels right to you."
    )
    filename = "SCANNER_PROMPT.wav"
    generate_voice(scanner_text, filename)

def process_json_file(json_path, output_prefix, text_field):
    """Process JSON files with the given text field."""
    try:
        with open(json_path, "r", encoding="utf-8-sig") as f:
            data_nodes = json.load(f)

        for node in data_nodes:
            line = node.get(text_field, "")
            node_id = node.get("id", "unknown")

            if isinstance(line, list):
                for index, text in enumerate(line):
                    filename = f"{output_prefix}_{node_id}_{index}.wav"
                    generate_voice(text, filename)
            else:
                filename = f"{output_prefix}_{node_id}.wav"
                generate_voice(line, filename)

    except Exception as e:
        print(f"Error processing {json_path}: {e}")

def main():
    """Main function to process multiple JSON files and generate voice files."""
    print("Generating SCANNER_PROMPT.wav...")
    generate_scanner_prompt()

    for json_path, (output_prefix, text_field) in JSON_FILES.items():
        print(f"Processing file: {json_path}")
        process_json_file(json_path, output_prefix, text_field)

if __name__ == "__main__":
    main()
