#nullable enable
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Popcron.LuaScript
{
    public class LuaScriptAsset : ScriptableObject
    {
        private static readonly List<LuaScriptAsset> assets = new List<LuaScriptAsset>();

        public static IReadOnlyCollection<LuaScriptAsset> All => assets;

        [SerializeField]
        private string? text;

        /// <summary>
        /// Invoked when modified in editor.
        /// </summary>
        public Action? OnModified { get; set; }

        public ReadOnlySpan<char> SourceCode => (text ?? string.Empty).AsSpan();

        private void OnEnable()
        {
            assets.Add(this);
        }

        private void OnDisable()
        {
            assets.Remove(this);
        }
    }
}