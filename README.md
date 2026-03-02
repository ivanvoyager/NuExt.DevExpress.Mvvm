# NuExt.DevExpress.Mvvm
[![NuGet](https://img.shields.io/nuget/v/NuExt.DevExpress.Mvvm.svg)](https://www.nuget.org/packages/NuExt.DevExpress.Mvvm)
[![Build](https://github.com/ivanvoyager/NuExt.DevExpress.Mvvm/actions/workflows/ci.yml/badge.svg)](https://github.com/ivanvoyager/NuExt.DevExpress.Mvvm/actions/workflows/ci.yml)
[![License](https://img.shields.io/github/license/ivanvoyager/NuExt.DevExpress.Mvvm?label=license)](https://github.com/ivanvoyager/NuExt.DevExpress.Mvvm/blob/main/LICENSE)
[![Downloads](https://img.shields.io/nuget/dt/NuExt.DevExpress.Mvvm.svg)](https://www.nuget.org/packages/NuExt.DevExpress.Mvvm)

`NuExt.DevExpress.Mvvm` is a NuGet package that offers a suite of extensions and utilities for the [DevExpress MVVM Framework](https://github.com/DevExpress/DevExpress.Mvvm.Free). The focus of this package is on **asynchronous operations**, enhancing the core capabilities of the DevExpress MVVM framework and simplifying the implementation of the Model-View-ViewModel (MVVM) pattern in WPF applications. It provides developers with tools to efficiently handle async tasks, improve application responsiveness, and reduce routine work.

## Migration Note

If you are starting a new project or planning to modernize an existing one, consider using the **NuExt.Minimal.Mvvm** family instead of this package.

`NuExt.Minimal.Mvvm`, `NuExt.Minimal.Behaviors.Wpf`, and `NuExt.Minimal.Mvvm.Wpf` provide a more streamlined and predictable MVVM model with:

- a minimal and dependency‑free core,
- deterministic async command semantics,
- lightweight ViewModel lifecycles,
- explicit view/document/dialog composition,
- clean integration with modern .NET and multi‑UI‑thread WPF scenarios.

This package remains functional and stable for existing applications, but the **Minimal.Mvvm** stack is recommended for new development due to its clearer architecture, smaller surface area, and async‑first design.

Learn more:
- https://www.nuget.org/packages/NuExt.Minimal.Mvvm  
- https://www.nuget.org/packages/NuExt.Minimal.Behaviors.Wpf  
- https://www.nuget.org/packages/NuExt.Minimal.Mvvm.Wpf

### Commonly Used Types

- **`DevExpress.Mvvm.Bindable`**: Base class for creating bindable objects.
- **`DevExpress.Mvvm.ViewModel`**: Base class for ViewModels designed for asynchronous initialization and disposal.
- **`DevExpress.Mvvm.ControlViewModel`**: Base class for control-specific ViewModels.
- **`DevExpress.Mvvm.DocumentContentViewModelBase`**: Base class for ViewModels that represent document content.
- **`DevExpress.Mvvm.WindowViewModel`**: Base class for window-specific ViewModels.
- **`DevExpress.Mvvm.AsyncCommandManager`**: Manages instances of `IAsyncCommand`.
- **`DevExpress.Mvvm.IAsyncDialogService`**: Displays dialog windows asynchronously.
- **`DevExpress.Mvvm.IAsyncDocument`**: Asynchronous document created with `IAsyncDocumentManagerService`.
- **`DevExpress.Mvvm.IAsyncDocumentManagerService`**: Manages asynchronous documents.
- **`DevExpress.Mvvm.UI.OpenWindowsService`**: Manages open window ViewModels within the application.
- **`DevExpress.Mvvm.UI.SettingsService`**: Facilitates saving and loading settings.
- **`DevExpress.Mvvm.UI.TabbedDocumentUIService`**: Manages tabbed documents within a UI.
- **`DevExpress.Mvvm.UI.WindowPlacementService`**: Saves and restores window placement between runs.

### Installation

You can install `NuExt.DevExpress.Mvvm` via [NuGet](https://www.nuget.org/):

```sh
dotnet add package NuExt.DevExpress.Mvvm
```

Or through the Visual Studio package manager:

1. Go to `Tools -> NuGet Package Manager -> Manage NuGet Packages for Solution...`.
2. Search for `NuExt.DevExpress.Mvvm`.
3. Click "Install".

### Usage Examples

For comprehensive examples of how to use the package, refer to the [WpfAppSample](https://github.com/ivanvoyager/NuExt.DevExpress.Mvvm/tree/main/samples/WpfAppSample) and the [NuExt.DevExpress.Mvvm.MahApps.Metro](https://github.com/ivanvoyager/NuExt.DevExpress.Mvvm.MahApps.Metro) repository.

### Contributing

Contributions are welcome! Feel free to submit issues, fork the repository, and send pull requests. Your feedback and suggestions for improvement are highly appreciated.

### License

Licensed under the MIT License. See the LICENSE file for details.
