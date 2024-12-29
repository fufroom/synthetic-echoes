#!/bin/bash

# Directory to store the generated samples
OUTPUT_DIR="./tts_all_models_samples"
mkdir -p "$OUTPUT_DIR"

# Text to synthesize
TEXT="I can tell you are holding something back, place your palm on the scanner so I may read your emotional data."

# Array of TTS model names (Filtered for en, uk, aus, nz)
MODELS=(
    "tts_models/en/ek1/tacotron2"
    "tts_models/en/ljspeech/tacotron2-DDC"
    "tts_models/en/ljspeech/tacotron2-DDC_ph"
    "tts_models/en/ljspeech/glow-tts"
    "tts_models/en/ljspeech/speedy-speech"
    "tts_models/en/ljspeech/tacotron2-DCA"
    "tts_models/en/ljspeech/vits"
    "tts_models/en/ljspeech/vits--neon"
    "tts_models/en/ljspeech/fast_pitch"
    "tts_models/en/ljspeech/overflow"
    "tts_models/en/ljspeech/neural_hmm"
    "tts_models/en/vctk/vits"
    "tts_models/en/vctk/fast_pitch"
    "tts_models/en/sam/tacotron-DDC"
    "tts_models/en/blizzard2013/capacitron-t2-c50"
    "tts_models/en/blizzard2013/capacitron-t2-c150_v2"
    "tts_models/en/multi-dataset/tortoise-v2"
    "tts_models/en/jenny/jenny"
    "tts_models/uk/mai/glow-tts"
    "tts_models/uk/mai/vits"
)

# Loop to generate 10 randomized samples
for i in $(seq 1 10); do
    # Randomly select a model
    MODEL=${MODELS[$RANDOM % ${#MODELS[@]}]}

    # Random pitch between -300 and 0 (avoiding high pitch)
    PITCH=$((RANDOM % 301 - 300))

    # Output file with model and pitch in the name
    OUTPUT_FILE="$OUTPUT_DIR/sample_$i_$(echo "$MODEL" | tr '/' '_')_pitch${PITCH}.wav"

    echo "Generating sample for model: $MODEL with pitch: $PITCH"
    
    # Generate audio with random pitch adjustment
    tts --text "$TEXT" --model_name "$MODEL" --pipe_out | sox -t wav - "$OUTPUT_FILE" pitch $PITCH

    echo "Saved to: $OUTPUT_FILE"
done

echo "10 randomized samples have been generated and saved in: $OUTPUT_DIR"
