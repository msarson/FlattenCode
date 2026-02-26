# FlattenCode — Clarion IDE Addin

A [Clarion IDE](https://www.softvelocity.com/) addin that flattens Clarion line continuations (`|`) into single logical lines.

## What it does

Clarion source files use a pipe character (`|`) to continue a long line onto the next line. This is great for readability in the IDE, but makes it hard to copy/paste, diff, or grep code. `FlattenCode` joins those continuation lines back into single logical lines.

**Before:**
```clarion
Window  WINDOW('My Window'),AT(,,453,319),FONT('Segoe UI',10,,FONT:regular, |
          CHARSET:ANSI),DOUBLE,AUTO,GRAY
```

**After:**
```clarion
Window  WINDOW('My Window'),AT(,,453,319),FONT('Segoe UI',10,,FONT:regular,CHARSET:ANSI),DOUBLE,AUTO,GRAY
```

It also collapses adjacent string literals that were split across lines:
```clarion
! Before
FORMAT('51L(2)|M~IP Address~@s30@' & |
  '28L(2)|M~Socket~@n7@')

! After
FORMAT('51L(2)|M~IP Address~@s30@28L(2)|M~Socket~@n7@')
```

## Usage

- **Shortcut:** `Ctrl+Shift+F`
- **Menu:** Edit → Format → Flatten Code
- **Context menu:** Right-click in any CLW source editor or Embeditor

If text is selected, only the selection is flattened. Otherwise the entire document is flattened and the caret jumps to the start of the continuation group it was in.

The addin is active in both the CLW source editor and the Embeditor (in writable embed regions only).

## Building

Requires the [.NET Framework 4.0 targeting pack](https://www.microsoft.com/en-us/download/details.aspx?id=17851) and a Clarion installation.

```
dotnet build FlattenCode.sln -c Release
```

By default, references are resolved from `C:\Clarion\Clarion11.1\bin`. Override with:

```
dotnet build /p:ClarionBin=C:\Clarion\Clarion12\bin
```

or set the `CLARION_BIN` environment variable, or create a `Directory.Build.props.user` file:

```xml
<Project>
  <PropertyGroup>
    <ClarionBin>C:\Clarion\Clarion12\bin</ClarionBin>
  </PropertyGroup>
</Project>
```

## Deploying

**Pre-built:** Download the latest `FlattenCode-v*.zip` from the [Releases](https://github.com/msarson/FlattenCode/releases) page.

Extract and copy both files to a subfolder under your Clarion accessory addins directory:

```
C:\Clarion\Clarion11.1\accessory\addins\FlattenCode\
```

Restart the Clarion IDE.

**From source:** build (see above), then copy `bin\Release\net40\FlattenCode.dll` and `FlattenCode.addin` to the same location.

## Compatibility

Works with Clarion 10, 11, 11.1, and 12. All versions ship the same `ICSharpCode.*.dll` addin API (`2.1.0.2447`).

## Feedback & bugs

Found a bug or have a feature request? Please [open an issue](https://github.com/msarson/FlattenCode/issues).
