############################################################################
# Created: 30 January 2025
# Author: Jason Tan (Asfritecx)
# File: calculator.py
# DevTools Used: DeepSeek R1, VSCode, Python 3.10.2, PySide6, Qt Designer, Windows 10
# Description: A simple calculator application using PySide6 that performs basic arithmetic operations. 
# The application is created using Qt Designer and the UI is loaded using the Ui_MainWindow class. 
# As I'm new to pysense6, I'm using DeepSeek R1 to work on this project. 
# With the aim to create GUI applications in the future using PySide6.
############################################################################
import sys
from PySide6.QtWidgets import QApplication, QMainWindow, QPushButton
from calculator_ui import Ui_MainWindow

class Calculator(QMainWindow):
    def __init__(self):
        super().__init__()
        self.ui = Ui_MainWindow()
        self.ui.setupUi(self)
        self.connect_buttons()

        # Initialize display
        self.ui.calcDisplay.setText("")

    def connect_buttons(self):
        # Connect number buttons (0-9) and operators
        for btn in self.findChildren(QPushButton):
            if btn.objectName().startswith("btn"):
                if btn.text().isdigit() or btn.text() in ['+', '-', '*', '/']:
                    btn.clicked.connect(lambda _, text=btn.text(): self.add_to_display(text))

        # Connect special buttons
        self.ui.btnClear.clicked.connect(self.clear_display)
        self.ui.btnEquals.clicked.connect(self.calculate_result)
        self.ui.btnDecimal.clicked.connect(lambda: self.add_to_display("."))

    def add_to_display(self, text):
        self.ui.calcDisplay.setText(self.ui.calcDisplay.text() + text)

    def clear_display(self):
        self.ui.calcDisplay.setText("")

    def calculate_result(self):
        try:
            expression = self.ui.calcDisplay.text()
            result = str(eval(expression))
            self.ui.calcDisplay.setText(result)
        except Exception as e:
            self.ui.calcDisplay.setText("Error")

if __name__ == "__main__":
    app = QApplication(sys.argv)
    window = Calculator()
    window.show()
    sys.exit(app.exec())