﻿using System.Collections;
using System.Diagnostics.CodeAnalysis;
using LookupEngine;
using RevitLookup.Abstractions.ObservableModels.Decomposition;
using RevitLookup.Abstractions.Services.Decomposition;
using RevitLookup.Abstractions.Services.Settings;
using RevitLookup.UI.Playground.Mockups.Core.Decomposition;
using RevitLookup.UI.Playground.Mockups.Mappers;

namespace RevitLookup.UI.Playground.Mockups.Services.Decomposition;

[SuppressMessage("ReSharper", "LoopCanBeConvertedToQuery")]
[SuppressMessage("ReSharper", "ForeachCanBeConvertedToQueryUsingAnotherGetEnumerator")]
public sealed class MockDecompositionService(ISettingsService settingsService) : IDecompositionService
{
    public List<ObservableDecomposedObject> DecompositionStackHistory { get; } = [];

    public async Task<ObservableDecomposedObject> DecomposeAsync(object? obj)
    {
        var options = CreateDecomposeMembersOptions();
        return await Task.Run(() =>
        {
            var result = LookupComposer.Decompose(obj, options);
            return DecompositionResultMapper.Convert(result);
        });
    }

    public async Task<List<ObservableDecomposedObject>> DecomposeAsync(IEnumerable objects)
    {
        var options = CreateDecomposeOptions();
        return await Task.Run(() =>
        {
            var capacity = objects is ICollection collection ? collection.Count : 4;
            var decomposedObjects = new List<ObservableDecomposedObject>(capacity);
            foreach (var obj in objects)
            {
                var decomposedObject = LookupComposer.DecomposeObject(obj, options);
                decomposedObjects.Add(DecompositionResultMapper.Convert(decomposedObject));
            }

            return decomposedObjects;
        });
    }

    public async Task<List<ObservableDecomposedMember>> DecomposeMembersAsync(ObservableDecomposedObject decomposedObject)
    {
        var options = CreateDecomposeMembersOptions();
        return await Task.Run(() =>
        {
            var decomposedMembers = LookupComposer.DecomposeMembers(decomposedObject.RawValue, options);
            var members = new List<ObservableDecomposedMember>(decomposedMembers.Count);

            foreach (var decomposedMember in decomposedMembers)
            {
                members.Add(DecompositionResultMapper.Convert(decomposedMember));
            }

            return members;
        });
    }

    private static DecomposeOptions CreateDecomposeOptions()
    {
        return new DecomposeOptions
        {
            EnableRedirection = true,
            TypeResolver = DescriptorsMap.FindDescriptor
        };
    }

    private DecomposeOptions CreateDecomposeMembersOptions()
    {
        return new DecomposeOptions
        {
            IncludeRoot = settingsService.DecompositionSettings.IncludeRoot,
            IncludeFields = settingsService.DecompositionSettings.IncludeFields,
            IncludeEvents = settingsService.DecompositionSettings.IncludeEvents,
            IncludeUnsupported = settingsService.DecompositionSettings.IncludeUnsupported,
            IncludePrivateMembers = settingsService.DecompositionSettings.IncludePrivate,
            IncludeStaticMembers = settingsService.DecompositionSettings.IncludeStatic,
            EnableExtensions = settingsService.DecompositionSettings.IncludeExtensions,
            EnableRedirection = true,
            TypeResolver = DescriptorsMap.FindDescriptor
        };
    }
}