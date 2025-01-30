# -*- mode: python ; coding: utf-8 -*-


a = Analysis(
    ['calculator.py'],
    pathex=['.'],
    binaries=[],
    datas=[('calculator.ui', '.')],
    hiddenimports=[],
    hookspath=[],
    hooksconfig={},
    runtime_hooks=[],
    excludes=[],
    noarchive=False,
    optimize=0,
    clean=True
)
pyz = PYZ(a.pure)

# Configures the EXE object (Equivalent to --onefile --windowed)
exe = EXE(
    pyz,
    a.scripts,
    a.binaries,
    a.datas,
    [],
    name='Calculator',
    debug=False,
    bootloader_ignore_signals=False,
    strip=False,
    upx=True,   #compression to make the exe smaller
    upx_exclude=[],
    runtime_tmpdir=None,
    console=False,   # Set to False for --windowed (no console)
    icon=None,  # Optional: Add an icon file (e.g., 'icon.ico')    
)
