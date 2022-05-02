﻿// This Source Code Form is subject to the terms of the MIT License.
// If a copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.
// Copyright (C) Leszek Pomianowski and WPF UI Contributors.
// All Rights Reserved.

using System.Windows;
using RevitLookup.UI.Common;

namespace RevitLookup.UI.Controls;

/// <summary>
///     Modern navigation styled similar to the Task Manager in Windows 11.
/// </summary>
public class NavigationCompact : Navigation
{
    /// <summary>
    ///     Property for <see cref="IsExpanded" />.
    /// </summary>
    public static readonly DependencyProperty IsExpandedProperty = DependencyProperty.Register(
        nameof(IsExpanded),
        typeof(bool), typeof(NavigationCompact),
        new PropertyMetadata(false));

    /// <summary>
    ///     Property for <see cref="TemplateButtonCommand" />.
    /// </summary>
    public static readonly DependencyProperty TemplateButtonCommandProperty =
        DependencyProperty.Register(nameof(TemplateButtonCommand),
            typeof(IRelayCommand), typeof(NavigationCompact), new PropertyMetadata(null));

    /// <summary>
    ///     Creates new instance and sets default <see cref="TemplateButtonCommandProperty" />.
    /// </summary>
    public NavigationCompact()
    {
        SetValue(TemplateButtonCommandProperty, new RelayCommand(o => Button_OnClick(this, o)));
    }

    /// <summary>
    ///     Gets or sets a value indicating whether the menu is expanded.
    /// </summary>
    public bool IsExpanded
    {
        get => (bool) GetValue(IsExpandedProperty);
        set => SetValue(IsExpandedProperty, value);
    }

    /// <summary>
    ///     Command triggered after clicking the button.
    /// </summary>
    public IRelayCommand TemplateButtonCommand => (IRelayCommand) GetValue(TemplateButtonCommandProperty);

    private void Button_OnClick(object sender, object parameter)
    {
        if (parameter == null) return;

        var param = parameter as string ?? string.Empty;
        if (param == "hamburger")
            IsExpanded = !IsExpanded;
    }
}