# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Overview

PF.UI is a WPF UI component library and application shell repository. It houses the contract layer (`PF.Core`), a custom control library (`PF.UI.Controls`), a theme resource dictionary (`PF.UI.Resources`), shared WPF utilities (`PF.UI.Shared`), and a minimal Prism application entry point (`PF.UI`).

**Solution file**: `PF.UI.slnx` (requires Visual Studio 2022 or JetBrains Rider)

## Build & Run

```powershell
cd source\repos\PF.UI
dotnet restore
dotnet build
# Run the shell app
dotnet run --project PF.UI
```

## Project Structure & Dependency Chain

Dependencies flow downward — no cycles:

```
PF.UI                → App entry point (Prism bootstrapper + MainWindow with ContentRegion)
PF.UI.Resources      → Theme dictionaries, fonts, colors; depends on PF.Core + PF.UI.Controls
PF.UI.Controls       → Custom controls (Button, Drawer, Growl, StepBar, etc.); depends on PF.UI.Shared
PF.UI.Shared         → Converters, helpers, drawing, media, interop utilities (no UI.Controls dep)
PF.Core              → Interfaces, enums, attributes, entities — zero external deps; NuGet-packaged
```

## Key Architectural Details

### PF.Core
Contract-only layer shared with `PF.AutoFramework`. Packaged as NuGet on Release builds. Contains:
- **Interfaces** — grouped by domain: `Alarm/`, `Device/Hardware/`, `Device/Mechanisms/`, `Station/`, `SecsGem/`, `Configuration/`, `Identity/`, `Logging/`, `Production/`
- **Attributes** — `[AlarmInfo]`, `[MechanismUI]`, `[StationUI]`, `[ModuleNavigation]`, `[ParamView]`
- **Constants** — `AlarmCodes.cs` (enum + extension pattern), `NavigationConstants.cs`, `LogCategories.cs`

### PF.UI.Controls
Custom WPF control library. Controls are grouped in `Controls/` by category: `Attach/` (attached properties), `Base/`, `Block/`, `Button/`, `Drawer/`, `Growl/`, `Input/` (AutoCompleteTextBox, CheckComboBox, SearchComboBox), `Loading/`, `Panel/`, `PropertyGrid/`, `Ribbon/`, `SideMenu/`, `Slider/` (RangeSlider, CompareSlider), `StepBar/`, `TabControl/`, `Time/` (Clock, FlipClock, TimeBar), `Window/` (GlowWindow), and more.

`Interactivity/` contains ported Expression.Interactions and Windows.Interactivity behaviors, plus `EventToCommand` and built-in commands (`CloseWindowCommand`, `ShutdownAppCommand`, etc.).

### PF.UI.Resources
Theme resource library. Key behavior:
- `XAMLTools.MSBuild` merges all `Themes/**/**/*.xaml` → `Themes/Default.xaml` at build time. Edit individual XAML files under `Themes/`, not `Default.xaml` directly.
- `Themes/Basic/` defines numbered primitives: `01_Brushes`, `02_Converters`, `03_Effects`, `04_Fonts`, `05_Geometries`, `06_DrawingImages`, `07_Paths`, `09_Sizes`, `10_Behaviors`, `11_ControlBaseStyle`.
- `Colors/Default.xaml` and `Colors/Dark.xaml` define the light/dark color schemes.
- Ships Source Han Sans SC in 7 weights as embedded `Resource` items (required for NuGet packaging).
- Packaged as NuGet on Release.

### PF.UI.Shared
Pure utility layer (no dependency on `PF.UI.Controls`). Key namespaces:
- `Tools/Converter/` — 30+ `IValueConverter` implementations
- `Tools/Extension/` — extension methods for `DependencyObject`, `FrameworkElement`, `UIElement`, `Color`, geometry
- `Tools/Helper/` — `AnimationHelper`, `ColorHelper`, `DpiHelper`, `VisualHelper`, `WindowHelper`, etc.
- `Tools/Hook/` — global `KeyboardHook`, `MouseHook`, `ClipboardHook`
- `Tools/Interop/` — `InteropMethods`, `InteropValues`, safe handle wrappers
- `Drawing/` — geometry flattening, path helpers, Bezier utilities
- `Media/` — `GeometryEffect`, `GeometrySource<T>`, Arc geometry

## Build Properties

| File | Purpose |
|------|---------|
| `Common.props` | `net8.0`, nullable, implicit usings — for non-WPF projects (PF.Core) |
| `Common.Desktop.props` | `net8.0-windows`, WPF enabled — for all UI projects |
| `Directory.Packages.props` | Central NuGet version management (Prism 9, DryIoc 6, EF Core 9, log4net 3, etc.) |

## NuGet Packaging

`PF.Core`, `PF.UI.Controls`, `PF.UI.Resources`, and `PF.UI.Shared` are all `<IsPackable>true</IsPackable>` and generate NuGet packages on Release builds. When modifying public APIs in these projects, ensure backwards compatibility with consuming projects in `PF.AutoFramework`.
