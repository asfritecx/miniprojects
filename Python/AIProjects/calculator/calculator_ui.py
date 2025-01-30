# -*- coding: utf-8 -*-

################################################################################
## Form generated from reading UI file 'calculator.ui'
##
## Created by: Qt User Interface Compiler version 6.8.1
##
## WARNING! All changes made in this file will be lost when recompiling UI file!
################################################################################

from PySide6.QtCore import (QCoreApplication, QDate, QDateTime, QLocale,
    QMetaObject, QObject, QPoint, QRect,
    QSize, QTime, QUrl, Qt)
from PySide6.QtGui import (QBrush, QColor, QConicalGradient, QCursor,
    QFont, QFontDatabase, QGradient, QIcon,
    QImage, QKeySequence, QLinearGradient, QPainter,
    QPalette, QPixmap, QRadialGradient, QTransform)
from PySide6.QtWidgets import (QApplication, QGridLayout, QLayout, QLineEdit,
    QMainWindow, QMenuBar, QPushButton, QSizePolicy,
    QStatusBar, QTabWidget, QWidget)

class Ui_MainWindow(object):
    def setupUi(self, MainWindow):
        if not MainWindow.objectName():
            MainWindow.setObjectName(u"MainWindow")
        MainWindow.resize(350, 520)
        sizePolicy = QSizePolicy(QSizePolicy.Policy.Expanding, QSizePolicy.Policy.Expanding)
        sizePolicy.setHorizontalStretch(0)
        sizePolicy.setVerticalStretch(0)
        sizePolicy.setHeightForWidth(MainWindow.sizePolicy().hasHeightForWidth())
        MainWindow.setSizePolicy(sizePolicy)
        MainWindow.setMinimumSize(QSize(350, 520))
        MainWindow.setMaximumSize(QSize(350, 520))
        MainWindow.setAutoFillBackground(False)
        MainWindow.setTabShape(QTabWidget.TabShape.Rounded)
        self.centralwidget = QWidget(MainWindow)
        self.centralwidget.setObjectName(u"centralwidget")
        self.calcDisplay = QLineEdit(self.centralwidget)
        self.calcDisplay.setObjectName(u"calcDisplay")
        self.calcDisplay.setGeometry(QRect(20, 10, 311, 81))
        font = QFont()
        font.setPointSize(40)
        font.setBold(True)
        self.calcDisplay.setFont(font)
        self.calcDisplay.setLayoutDirection(Qt.LayoutDirection.LeftToRight)
        self.calcDisplay.setAutoFillBackground(True)
        self.gridLayoutWidget = QWidget(self.centralwidget)
        self.gridLayoutWidget.setObjectName(u"gridLayoutWidget")
        self.gridLayoutWidget.setGeometry(QRect(20, 110, 311, 351))
        self.gridLayout = QGridLayout(self.gridLayoutWidget)
        self.gridLayout.setSpacing(15)
        self.gridLayout.setObjectName(u"gridLayout")
        self.gridLayout.setSizeConstraint(QLayout.SizeConstraint.SetNoConstraint)
        self.gridLayout.setContentsMargins(15, 15, 15, 15)
        self.btnAdd = QPushButton(self.gridLayoutWidget)
        self.btnAdd.setObjectName(u"btnAdd")
        self.btnAdd.setEnabled(True)
        sizePolicy.setHeightForWidth(self.btnAdd.sizePolicy().hasHeightForWidth())
        self.btnAdd.setSizePolicy(sizePolicy)
        self.btnAdd.setMinimumSize(QSize(10, 10))
        self.btnAdd.setMaximumSize(QSize(300, 300))
        font1 = QFont()
        font1.setFamilies([u"Calibri"])
        font1.setPointSize(20)
        font1.setBold(True)
        font1.setItalic(False)
        font1.setStyleStrategy(QFont.PreferAntialias)
        self.btnAdd.setFont(font1)

        self.gridLayout.addWidget(self.btnAdd, 1, 3, 1, 1)

        self.btnOne = QPushButton(self.gridLayoutWidget)
        self.btnOne.setObjectName(u"btnOne")
        sizePolicy.setHeightForWidth(self.btnOne.sizePolicy().hasHeightForWidth())
        self.btnOne.setSizePolicy(sizePolicy)
        self.btnOne.setMinimumSize(QSize(10, 10))
        self.btnOne.setMaximumSize(QSize(300, 300))
        self.btnOne.setFont(font1)

        self.gridLayout.addWidget(self.btnOne, 3, 0, 1, 1)

        self.btnEight = QPushButton(self.gridLayoutWidget)
        self.btnEight.setObjectName(u"btnEight")
        sizePolicy.setHeightForWidth(self.btnEight.sizePolicy().hasHeightForWidth())
        self.btnEight.setSizePolicy(sizePolicy)
        self.btnEight.setMinimumSize(QSize(10, 10))
        self.btnEight.setMaximumSize(QSize(300, 300))
        self.btnEight.setFont(font1)

        self.gridLayout.addWidget(self.btnEight, 1, 1, 1, 1)

        self.btnTwo = QPushButton(self.gridLayoutWidget)
        self.btnTwo.setObjectName(u"btnTwo")
        self.btnTwo.setEnabled(True)
        sizePolicy.setHeightForWidth(self.btnTwo.sizePolicy().hasHeightForWidth())
        self.btnTwo.setSizePolicy(sizePolicy)
        self.btnTwo.setMinimumSize(QSize(10, 10))
        self.btnTwo.setMaximumSize(QSize(300, 300))
        self.btnTwo.setFont(font1)

        self.gridLayout.addWidget(self.btnTwo, 3, 1, 1, 1)

        self.btnThree = QPushButton(self.gridLayoutWidget)
        self.btnThree.setObjectName(u"btnThree")
        self.btnThree.setEnabled(True)
        sizePolicy.setHeightForWidth(self.btnThree.sizePolicy().hasHeightForWidth())
        self.btnThree.setSizePolicy(sizePolicy)
        self.btnThree.setMinimumSize(QSize(10, 10))
        self.btnThree.setMaximumSize(QSize(300, 300))
        self.btnThree.setFont(font1)

        self.gridLayout.addWidget(self.btnThree, 3, 2, 1, 1)

        self.btnFour = QPushButton(self.gridLayoutWidget)
        self.btnFour.setObjectName(u"btnFour")
        sizePolicy.setHeightForWidth(self.btnFour.sizePolicy().hasHeightForWidth())
        self.btnFour.setSizePolicy(sizePolicy)
        self.btnFour.setMinimumSize(QSize(10, 10))
        self.btnFour.setMaximumSize(QSize(300, 300))
        self.btnFour.setFont(font1)

        self.gridLayout.addWidget(self.btnFour, 2, 0, 1, 1)

        self.btnSubtract = QPushButton(self.gridLayoutWidget)
        self.btnSubtract.setObjectName(u"btnSubtract")
        self.btnSubtract.setEnabled(True)
        sizePolicy.setHeightForWidth(self.btnSubtract.sizePolicy().hasHeightForWidth())
        self.btnSubtract.setSizePolicy(sizePolicy)
        self.btnSubtract.setMinimumSize(QSize(10, 10))
        self.btnSubtract.setMaximumSize(QSize(300, 300))
        self.btnSubtract.setFont(font1)

        self.gridLayout.addWidget(self.btnSubtract, 2, 3, 1, 1)

        self.btnSeven = QPushButton(self.gridLayoutWidget)
        self.btnSeven.setObjectName(u"btnSeven")
        sizePolicy.setHeightForWidth(self.btnSeven.sizePolicy().hasHeightForWidth())
        self.btnSeven.setSizePolicy(sizePolicy)
        self.btnSeven.setMinimumSize(QSize(10, 10))
        self.btnSeven.setMaximumSize(QSize(300, 300))
        self.btnSeven.setFont(font1)

        self.gridLayout.addWidget(self.btnSeven, 1, 0, 1, 1)

        self.btnFive = QPushButton(self.gridLayoutWidget)
        self.btnFive.setObjectName(u"btnFive")
        sizePolicy.setHeightForWidth(self.btnFive.sizePolicy().hasHeightForWidth())
        self.btnFive.setSizePolicy(sizePolicy)
        self.btnFive.setMinimumSize(QSize(10, 10))
        self.btnFive.setMaximumSize(QSize(300, 300))
        self.btnFive.setFont(font1)

        self.gridLayout.addWidget(self.btnFive, 2, 1, 1, 1)

        self.btnNine = QPushButton(self.gridLayoutWidget)
        self.btnNine.setObjectName(u"btnNine")
        self.btnNine.setEnabled(True)
        sizePolicy.setHeightForWidth(self.btnNine.sizePolicy().hasHeightForWidth())
        self.btnNine.setSizePolicy(sizePolicy)
        self.btnNine.setMinimumSize(QSize(10, 10))
        self.btnNine.setMaximumSize(QSize(300, 300))
        self.btnNine.setFont(font1)

        self.gridLayout.addWidget(self.btnNine, 1, 2, 1, 1)

        self.btnSix = QPushButton(self.gridLayoutWidget)
        self.btnSix.setObjectName(u"btnSix")
        self.btnSix.setEnabled(True)
        sizePolicy.setHeightForWidth(self.btnSix.sizePolicy().hasHeightForWidth())
        self.btnSix.setSizePolicy(sizePolicy)
        self.btnSix.setMinimumSize(QSize(10, 10))
        self.btnSix.setMaximumSize(QSize(300, 300))
        self.btnSix.setFont(font1)

        self.gridLayout.addWidget(self.btnSix, 2, 2, 1, 1)

        self.btnClear = QPushButton(self.gridLayoutWidget)
        self.btnClear.setObjectName(u"btnClear")
        self.btnClear.setEnabled(True)
        sizePolicy.setHeightForWidth(self.btnClear.sizePolicy().hasHeightForWidth())
        self.btnClear.setSizePolicy(sizePolicy)
        self.btnClear.setMinimumSize(QSize(10, 10))
        self.btnClear.setMaximumSize(QSize(300, 300))
        self.btnClear.setFont(font1)

        self.gridLayout.addWidget(self.btnClear, 0, 0, 1, 1)

        self.btnEquals = QPushButton(self.gridLayoutWidget)
        self.btnEquals.setObjectName(u"btnEquals")
        self.btnEquals.setEnabled(True)
        sizePolicy.setHeightForWidth(self.btnEquals.sizePolicy().hasHeightForWidth())
        self.btnEquals.setSizePolicy(sizePolicy)
        self.btnEquals.setMinimumSize(QSize(10, 10))
        self.btnEquals.setMaximumSize(QSize(300, 300))
        self.btnEquals.setFont(font1)

        self.gridLayout.addWidget(self.btnEquals, 3, 3, 1, 1)

        self.btnDivide = QPushButton(self.gridLayoutWidget)
        self.btnDivide.setObjectName(u"btnDivide")
        self.btnDivide.setEnabled(True)
        sizePolicy.setHeightForWidth(self.btnDivide.sizePolicy().hasHeightForWidth())
        self.btnDivide.setSizePolicy(sizePolicy)
        self.btnDivide.setMinimumSize(QSize(10, 10))
        self.btnDivide.setMaximumSize(QSize(300, 300))
        self.btnDivide.setFont(font1)
        self.btnDivide.setIconSize(QSize(40, 40))

        self.gridLayout.addWidget(self.btnDivide, 0, 3, 1, 1)

        self.btnMultiply = QPushButton(self.gridLayoutWidget)
        self.btnMultiply.setObjectName(u"btnMultiply")
        self.btnMultiply.setEnabled(True)
        sizePolicy.setHeightForWidth(self.btnMultiply.sizePolicy().hasHeightForWidth())
        self.btnMultiply.setSizePolicy(sizePolicy)
        self.btnMultiply.setMinimumSize(QSize(10, 10))
        self.btnMultiply.setMaximumSize(QSize(300, 300))
        self.btnMultiply.setFont(font1)
        self.btnMultiply.setIconSize(QSize(40, 40))

        self.gridLayout.addWidget(self.btnMultiply, 0, 2, 1, 1)

        self.btnDecimal = QPushButton(self.gridLayoutWidget)
        self.btnDecimal.setObjectName(u"btnDecimal")
        self.btnDecimal.setEnabled(True)
        sizePolicy.setHeightForWidth(self.btnDecimal.sizePolicy().hasHeightForWidth())
        self.btnDecimal.setSizePolicy(sizePolicy)
        self.btnDecimal.setMinimumSize(QSize(10, 10))
        self.btnDecimal.setMaximumSize(QSize(300, 300))
        self.btnDecimal.setFont(font1)
        self.btnDecimal.setIconSize(QSize(40, 40))

        self.gridLayout.addWidget(self.btnDecimal, 0, 1, 1, 1)

        MainWindow.setCentralWidget(self.centralwidget)
        self.menubar = QMenuBar(MainWindow)
        self.menubar.setObjectName(u"menubar")
        self.menubar.setGeometry(QRect(0, 0, 350, 33))
        MainWindow.setMenuBar(self.menubar)
        self.statusbar = QStatusBar(MainWindow)
        self.statusbar.setObjectName(u"statusbar")
        MainWindow.setStatusBar(self.statusbar)

        self.retranslateUi(MainWindow)

        QMetaObject.connectSlotsByName(MainWindow)
    # setupUi

    def retranslateUi(self, MainWindow):
        MainWindow.setWindowTitle(QCoreApplication.translate("MainWindow", u"MainWindow", None))
        self.calcDisplay.setText("")
        self.btnAdd.setText(QCoreApplication.translate("MainWindow", u"+", None))
        self.btnOne.setText(QCoreApplication.translate("MainWindow", u"1", None))
        self.btnEight.setText(QCoreApplication.translate("MainWindow", u"8", None))
        self.btnTwo.setText(QCoreApplication.translate("MainWindow", u"2", None))
        self.btnThree.setText(QCoreApplication.translate("MainWindow", u"3", None))
        self.btnFour.setText(QCoreApplication.translate("MainWindow", u"4", None))
        self.btnSubtract.setText(QCoreApplication.translate("MainWindow", u"-", None))
        self.btnSeven.setText(QCoreApplication.translate("MainWindow", u"7", None))
        self.btnFive.setText(QCoreApplication.translate("MainWindow", u"5", None))
        self.btnNine.setText(QCoreApplication.translate("MainWindow", u"9", None))
        self.btnSix.setText(QCoreApplication.translate("MainWindow", u"6", None))
        self.btnClear.setText(QCoreApplication.translate("MainWindow", u"CE", None))
        self.btnEquals.setText(QCoreApplication.translate("MainWindow", u"=", None))
        self.btnDivide.setText(QCoreApplication.translate("MainWindow", u"/", None))
        self.btnMultiply.setText(QCoreApplication.translate("MainWindow", u"*", None))
        self.btnDecimal.setText(QCoreApplication.translate("MainWindow", u".", None))
    # retranslateUi

