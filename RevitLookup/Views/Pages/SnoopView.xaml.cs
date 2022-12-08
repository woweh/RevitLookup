﻿// Copyright 2003-2022 by Autodesk, Inc.
// 
// Permission to use, copy, modify, and distribute this software in
// object code form for any purpose and without fee is hereby granted,
// provided that the above copyright notice appears in all copies and
// that both that copyright notice and the limited warranty and
// restricted rights notice below appear in all supporting
// documentation.
// 
// AUTODESK PROVIDES THIS PROGRAM "AS IS" AND WITH ALL FAULTS.
// AUTODESK SPECIFICALLY DISCLAIMS ANY IMPLIED WARRANTY OF
// MERCHANTABILITY OR FITNESS FOR A PARTICULAR USE.  AUTODESK, INC.
// DOES NOT WARRANT THAT THE OPERATION OF THE PROGRAM WILL BE
// UNINTERRUPTED OR ERROR FREE.
// 
// Use, duplication, or disclosure by the U.S. Government is subject to
// restrictions set forth in FAR 52.227-19 (Commercial Computer
// Software - Restricted Rights) and DFAR 252.227-7013(c)(1)(ii)
// (Rights in Technical Data and Computer Software), as applicable.

using System.Windows;
using RevitLookup.Services.Contracts;
using RevitLookup.UI.Common.Interfaces;
using RevitLookup.ViewModels.Contracts;
using RevitLookup.ViewModels.Objects;

namespace RevitLookup.Views.Pages;

public sealed partial class SnoopView : INavigableView<ISnoopViewModel>
{
    public SnoopView(ISnoopService viewModel)
    {
        ViewModel = (ISnoopViewModel) viewModel;
        InitializeComponent();
    }

    public ISnoopViewModel ViewModel { get; }

    private void HandleGridRowClick(object sender, RoutedEventArgs routedEventArgs)
    {
        if (DataGrid.SelectedItems.Count == 1)
        {
            var selectedItem = (ISnoopableObject) DataGrid.SelectedItem;
            var members = selectedItem.GetCachedMembers();
            if (members is not null)
            {
                var window = Host.GetService<ILookupInstance>();
                window.ShowWindow();
                window.Navigate(typeof(SnoopView));
            }
        }
    }

    private void SnoopView_OnLoaded(object sender, RoutedEventArgs e)
    {
        var itemTemplate = TreeView.ItemTemplate.Template;
    }
}