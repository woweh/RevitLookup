﻿// This Source Code Form is subject to the terms of the MIT License.
// If a copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.
// Copyright (C) Leszek Pomianowski and WPF UI Contributors.
// All Rights Reserved.

using System.Windows;
using RevitLookup.UI.Common;

namespace RevitLookup.UI.Controls;

/// <summary>
///     Lets look for things and other stuff.
/// </summary>
public class SearchBox : TextBox
{
    /// <summary>
    ///     Property override for <see cref="TextBox.Icon" />.
    /// </summary>
    // Static constructor.
    static SearchBox()
    {
        FrameworkPropertyMetadata newIconMetadata = new(
            SymbolRegular.Search24);

        IconProperty.OverrideMetadata(
            typeof(SearchBox),
            newIconMetadata);
    }

    // TODO: Well, could use some ListBox search logic or something similar
}