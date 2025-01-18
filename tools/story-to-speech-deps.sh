#!/bin/bash

# Define the virtual environment directory
VENV_DIR="venv"

echo "Checking for Python installation..."
if ! command -v python3 &> /dev/null; then
    echo "Python3 is not installed. Please install Python3 and try again."
    exit 1
fi

echo "Python3 found: $(python3 --version)"

# Create a virtual environment if it doesn't exist
if [ ! -d "$VENV_DIR" ]; then
    echo "Creating a virtual environment in ./$VENV_DIR..."
    python3 -m venv "$VENV_DIR"
    echo "Virtual environment created."
fi

# Activate the virtual environment
echo "Activating the virtual environment..."
source "$VENV_DIR/bin/activate"

# Install dependencies
echo "Installing required Python packages..."
pip install --upgrade pip
pip install gtts

echo "Dependencies installed successfully! Virtual environment is now active."
echo "To deactivate, run: deactivate"

# Keep the virtual environment active
exec bash --rcfile <(echo "source $VENV_DIR/bin/activate")
