## Windows Terminal

For the best chess piece rendering, it is recommended to run Chess CLI using **Windows Terminal** with the **Segoe UI Symbol** font.

### 1. Install Windows Terminal

Windows Terminal is available through the Microsoft Store and is included with recent versions of Windows.

### 2. Configure the font

Open Windows Terminal and go to:

**Settings → Profiles → Windows PowerShell → Appearance → Font face**

Set the font to:

```text
Segoe UI Symbol
```

Alternatively, add the following to your Windows Terminal `settings.json` under the Windows PowerShell profile
or in Windows Terminal Ctrl + Shift + ,

```json
"font": {
    "face": "Segoe UI Symbol"
}
```

### 3. Run Chess CLI

Open Windows Terminal, start PowerShell, navigate to the directory containing Chess CLI, and run:

```powershell
.\Chess.Cli.exe
```

The chess pieces should then be rendered using Unicode chess symbols:

```text
♜ ♞ ♝ ♛ ♚ ♝ ♞ ♜
♟ ♟ ♟ ♟ ♟ ♟ ♟ ♟
```

### Compatibility

Chess CLI uses Unicode chess characters. Rendering depends on the terminal and font being used.

**Recommended on Windows:**

* Windows Terminal
* Segoe UI Symbol

If the chess pieces appear as boxes or otherwise render incorrectly, configure the terminal to use a font with Unicode chess-symbol support,
or default to text rendered pieces for white and black:

```text
R N B Q K B N R         r n b q k b n r
P P P P P P P P         p p p p p p p p
```
