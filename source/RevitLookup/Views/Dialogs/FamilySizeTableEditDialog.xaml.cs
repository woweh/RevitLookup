﻿// Copyright 2003-2024 by Autodesk, Inc.
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

using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Extensions.DependencyInjection;
using RevitLookup.ViewModels.Dialogs;
using RevitLookup.Views.Extensions;
using Wpf.Ui;
using Wpf.Ui.Controls;

namespace RevitLookup.Views.Dialogs;

public sealed partial class FamilySizeTableEditDialog
{
    private readonly IServiceProvider _serviceProvider;
    private readonly FamilySizeTableEditDialogViewModel _viewModel;
    private readonly bool _isEditable;

    public FamilySizeTableEditDialog(IServiceProvider serviceProvider, Document document, FamilySizeTableManager manager, string tableName)
    {
        _isEditable = true;
        _serviceProvider = serviceProvider;
        _viewModel = new FamilySizeTableEditDialogViewModel(document, manager, tableName);

        DataContext = _viewModel;
        InitializeComponent();
    }

    public FamilySizeTableEditDialog(IServiceProvider serviceProvider, Document document, FamilySizeTable table)
    {
        _serviceProvider = serviceProvider;

        DataContext = new FamilySizeTableEditDialogViewModel(document, table);
        InitializeComponent();
        SizeTable.IsReadOnly = true;
    }

    public async Task ShowAsync()
    {
        var dialogOptions = new SimpleContentDialogCreateOptions
        {
            Title = "Family size table",
            Content = this,
            CloseButtonText = "Close",
            HorizontalScrollVisibility = ScrollBarVisibility.Disabled,
            VerticalScrollVisibility = ScrollBarVisibility.Disabled
        };

        if (_isEditable)
        {
            dialogOptions.PrimaryButtonText = "Save";
        }

        var dialogResult = await _serviceProvider.GetRequiredService<IContentDialogService>().ShowSimpleDialogAsync(dialogOptions);
        if (dialogResult == ContentDialogResult.Primary && _isEditable)
        {
            _viewModel.SaveData();
        }
    }

    private void OnRightClick(object sender, RoutedEventArgs routedEventArgs)
    {
        if (!_isEditable) return;

        var element = (FrameworkElement) sender;
        var context = (DataRowView) element.DataContext;
        CreateGridRowContextMenu(context.Row, element);
    }

    private void CreateGridRowContextMenu(DataRow dataRow, FrameworkElement dataGridRow)
    {
        var contextMenu = new ContextMenu
        {
            Resources = Resources,
            PlacementTarget = dataGridRow,
            DataContext = _viewModel
        };

        dataGridRow.ContextMenu = contextMenu;

        contextMenu.AddMenuItem("CopyMenuItem")
            .SetHeader("Duplicate row")
            .SetCommand(dataRow, row => _viewModel.DuplicateRow(row))
            .SetShortcut(ModifierKeys.Control, Key.D);

        contextMenu.AddMenuItem("DeleteMenuItem")
            .SetHeader("Delete row")
            .SetCommand(dataRow, row => _viewModel.DeleteRow(row))
            .SetShortcut(Key.Delete);
    }
}