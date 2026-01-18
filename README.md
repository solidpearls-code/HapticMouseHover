# Haptic Mouse Hover Plugin

A plugin for Logi/Loupedeck devices (such as MX MASTER 4) that provides haptic feedback when the mouse cursor hovers over clickable controls.

## Features

- Generates haptic (tactile) feedback when the mouse cursor passes over buttons and other clickable UI elements.
- Automatically detects clickable elements using Windows UIAutomation.
- Toggle haptic feedback ON/OFF via plugin command.

## Requirements

- Windows OS
- .NET 8.0 Runtime
- Logi Plugin Service 6.0 or later
- C# 11.0 or later (for development)

## Project Structure

```
LogiPluginExamplePlugin/
├── src/
│   ├── Actions/
│   │   ├── HoverHapticCommand.cs      # Haptic feedback command
│   │   └── MouseHoverDetector.cs      # Mouse hover detection logic
│   ├── Helpers/
│   │   ├── PluginLog.cs               # Logging functionality
│   │   └── PluginResources.cs         # Resource management
│   ├── package/
│   │   ├── metadata/
│   │   │   ├── LoupedeckPackage.yaml  # Plugin configuration
│   │   │   └── Icon256x256.png        # Plugin icon
│   │   └── events/
│   │       ├── DefaultEventSource.yaml # Event definitions
│   │       └── extra/
│   │           └── eventMapping.yaml   # Event mapping
│   ├── HapticOnButtonsPlugin.cs       # Plugin main class
│   ├── HapticOnButtonsApplication.cs  # Application integration
│   └── LogiPluginExamplePlugin.csproj # Project file
└── README.md
```

## Technical Details

### Core Implementation

- **MouseHoverDetector**: Polls the element at the mouse cursor position using Windows UIAutomation.
- **Clickable Element Detection**: Supports the following patterns:
  - InvokePattern (buttons, etc.)
  - TogglePattern (checkboxes, etc.)
  - SelectionItemPattern (list items, etc.)
  - ExpandCollapsePattern (tree views, etc.)
  - ValuePattern (sliders, etc.)

## Author

solidpearls

## Notes

- This plugin is Windows-only.
- Some applications may not work properly due to the use of UIAutomation.
- The polling interval is set to 10ms but can be adjusted according to system performance.

## License

MIT License

Copyright © 2026 solidpearls. All rights reserved.

See [LICENSE] for details.
