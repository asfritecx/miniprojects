# Calculator Application

A simple calculator application built with Python and PySide6 that performs basic arithmetic operations.

## Features

- Basic arithmetic operations (+, -, *, /)
- Clear display (CE)
- Decimal point support
- Error handling for invalid expressions

## Technical Details

The application consists of three main components:

1. `calculator.ui` - Qt Designer UI file defining the calculator interface
2. `calculator_ui.py` - Auto-generated Python code from the UI file
3. `calculator.py` - Main application logic

### Implementation

- Built using PySide6 for the GUI components
- Uses Qt Designer for the interface layout
- Main calculator logic implemented in the `Calculator` class
- Event handling for button clicks and calculations
- Dynamic button connection using object names

### Key Components

- Display field for showing input and results
- Numeric keypad (0-9)
- Operation buttons (+, -, *, /)
- Clear (CE) and equals (=) buttons
- Decimal point button

## Requirements

- Python 3.10+
- PySide6
- Qt Designer (for UI modifications)

## Usage

Run the application using:

```bash
python calculator.py
```

## Development
Created by Jason Tan (Asfritecx) using:
- DeepSeek-R1
- ChatGPT-o1
- Claude-Sonnet 3.5
- VSCode
- Python 3.10.2
- PySide6
- Simpleeval
- Qt Designer
- Windows 11

## Set Up
1. Load Project with VSCode ensure extensions installed
    - Python
    - Pylance
    - Qt for Python (for .ui file preview and integration).
2. Install Pyside6 with `pip install pyside6`
3. Editing UI `.venv\Scripts\pyside6-designer.exe`
4. Updating UI `pyside6-uic.exe calculator.ui -o ui_calculator.py`
5. Run python project

## Compiling
1. Load Project to VSCode
2. Run `pip install pyinstaller`
3. Create the .spec file by running `pyinstaller --name=Calculator --onefile --windowed calculator.py`
4. Edit the options required in the `calculator.spec` file and run `pyinstaller calculator.spec`
