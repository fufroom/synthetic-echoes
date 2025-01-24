./combine_scripts.sh
#!/bin/bash

# Define the target directory
SCRIPT_DIR="..//Assets/Scripts"

# Output file to store concatenated scripts
OUTPUT_FILE="all_scripts_combined.txt"

# Find all script files (C# and Python files)
find "$SCRIPT_DIR" -type f \( -name "*.cs" -o -name "*.py" \) -print0 | while IFS= read -r -d '' file; do
    echo "==== START OF $file ====" >> "$OUTPUT_FILE"
    cat "$file" >> "$OUTPUT_FILE"
    echo -e "\n==== END OF $file ====\n" >> "$OUTPUT_FILE"
done

echo "All scripts have been combined into $OUTPUT_FILE"
