﻿using Microsoft.Extensions.Logging;
using RevitLookup.Abstractions.Services.Presentation;
using RevitLookup.Abstractions.Services.Settings;
using RevitLookup.Abstractions.ViewModels.Visualization;
using RevitLookup.Core.Visualization;
using RevitLookup.Core.Visualization.Events;
using Color = System.Windows.Media.Color;

namespace RevitLookup.ViewModels.Visualization;

[UsedImplicitly]
public sealed partial class XyzVisualizationViewModel(
    ISettingsService settingsService,
    INotificationService notificationService,
    ILogger<XyzVisualizationViewModel> logger)
    : ObservableObject, IXyzVisualizationViewModel
{
    private readonly XyzVisualizationServer _server = new();

    [ObservableProperty] private double _axisLength = settingsService.VisualizationSettings.XyzSettings.AxisLength;
    [ObservableProperty] private double _transparency = settingsService.VisualizationSettings.XyzSettings.Transparency;

    [ObservableProperty] private Color _xColor = settingsService.VisualizationSettings.XyzSettings.XColor;
    [ObservableProperty] private Color _yColor = settingsService.VisualizationSettings.XyzSettings.YColor;
    [ObservableProperty] private Color _zColor = settingsService.VisualizationSettings.XyzSettings.ZColor;

    [ObservableProperty] private bool _showPlane = settingsService.VisualizationSettings.XyzSettings.ShowPlane;
    [ObservableProperty] private bool _showXAxis = settingsService.VisualizationSettings.XyzSettings.ShowXAxis;
    [ObservableProperty] private bool _showYAxis = settingsService.VisualizationSettings.XyzSettings.ShowYAxis;
    [ObservableProperty] private bool _showZAxis = settingsService.VisualizationSettings.XyzSettings.ShowZAxis;

    public double MinAxisLength => settingsService.VisualizationSettings.XyzSettings.MinAxisLength;

    public void RegisterServer(object xyzObject)
    {
        if (xyzObject is not XYZ point)
        {
            throw new ArgumentException($"Argument must be of type {nameof(XYZ)}", nameof(xyzObject));
        }

        UpdateShowPlane(ShowPlane);
        UpdateShowXAxis(ShowXAxis);
        UpdateShowYAxis(ShowYAxis);
        UpdateShowZAxis(ShowZAxis);

        UpdateXColor(XColor);
        UpdateYColor(YColor);
        UpdateZColor(ZColor);

        UpdateAxisLength(AxisLength);
        UpdateTransparency(Transparency);

        _server.RenderFailed += HandleRenderFailure;
        _server.Register(point);
    }

    public void UnregisterServer()
    {
        _server.RenderFailed -= HandleRenderFailure;
        _server.Unregister();
    }

    private void HandleRenderFailure(object? sender, RenderFailedEventArgs args)
    {
        logger.LogError(args.ExceptionObject, "Render error");
        notificationService.ShowError("Render error", args.ExceptionObject);
    }

    partial void OnXColorChanged(Color value)
    {
        settingsService.VisualizationSettings.XyzSettings.XColor = value;
        UpdateXColor(value);
    }

    partial void OnYColorChanged(Color value)
    {
        settingsService.VisualizationSettings.XyzSettings.YColor = value;
        UpdateYColor(value);
    }

    partial void OnZColorChanged(Color value)
    {
        settingsService.VisualizationSettings.XyzSettings.ZColor = value;
        UpdateZColor(value);
    }

    partial void OnAxisLengthChanged(double value)
    {
        settingsService.VisualizationSettings.XyzSettings.AxisLength = value;
        UpdateAxisLength(value);
    }

    partial void OnTransparencyChanged(double value)
    {
        settingsService.VisualizationSettings.XyzSettings.Transparency = value;
        UpdateTransparency(value);
    }

    partial void OnShowPlaneChanged(bool value)
    {
        settingsService.VisualizationSettings.XyzSettings.ShowPlane = value;
        UpdateShowPlane(value);
    }

    partial void OnShowXAxisChanged(bool value)
    {
        settingsService.VisualizationSettings.XyzSettings.ShowXAxis = value;
        UpdateShowXAxis(value);
    }

    partial void OnShowYAxisChanged(bool value)
    {
        settingsService.VisualizationSettings.XyzSettings.ShowYAxis = value;
        UpdateShowYAxis(value);
    }

    partial void OnShowZAxisChanged(bool value)
    {
        settingsService.VisualizationSettings.XyzSettings.ShowZAxis = value;
        UpdateShowZAxis(value);
    }

    private void UpdateXColor(Color value)
    {
        _server.UpdateXColor(new Autodesk.Revit.DB.Color(value.R, value.G, value.B));
    }

    private void UpdateYColor(Color value)
    {
        _server.UpdateYColor(new Autodesk.Revit.DB.Color(value.R, value.G, value.B));
    }

    private void UpdateZColor(Color value)
    {
        _server.UpdateZColor(new Autodesk.Revit.DB.Color(value.R, value.G, value.B));
    }

    private void UpdateAxisLength(double value)
    {
        _server.UpdateAxisLength(value / 12);
    }

    private void UpdateTransparency(double value)
    {
        _server.UpdateTransparency(value / 100);
    }

    private void UpdateShowPlane(bool value)
    {
        _server.UpdatePlaneVisibility(value);
    }

    private void UpdateShowXAxis(bool value)
    {
        _server.UpdateXAxisVisibility(value);
    }

    private void UpdateShowYAxis(bool value)
    {
        _server.UpdateYAxisVisibility(value);
    }

    private void UpdateShowZAxis(bool value)
    {
        _server.UpdateZAxisVisibility(value);
    }
}