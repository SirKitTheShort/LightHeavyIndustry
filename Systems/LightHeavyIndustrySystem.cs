/*

using System;
using System.Collections;
using System.Linq;
using Game;
using Unity.Entities;
using UnityEngine;

namespace LightHeavyIndustry.Systems
{
    public partial class LightHeavyIndustrySystem : GameSystemBase
    {
        private Game.Prefabs.PrefabSystem _prefabSystem;
        private bool _dumped;

        protected override void OnCreate()
        {
            base.OnCreate();

            _prefabSystem = World.GetOrCreateSystemManaged<Game.Prefabs.PrefabSystem>();
            Mod.log.Info("LightHeavyIndustrySystem created (PrefabSystem acquired)");
        }

        protected override void OnUpdate()
        {
            if (_dumped) return;
            _dumped = true;

            try
            {
                DumpIndustrialRelatedPrefabs();
            }
            catch (Exception ex)
            {
                Mod.log.Error(ex, "DumpIndustrialRelatedPrefabs failed");
            }
        }

        private void DumpIndustrialRelatedPrefabs()
        {
            var ps = _prefabSystem;
            var t = ps.GetType();

            var mi = t.GetMethod("GetAvailableContentPrefabs");
            if (mi == null)
            {
                Mod.log.Info("PrefabSystem.GetAvailableContentPrefabs method not found via reflection.");
                return;
            }

            object result = mi.Invoke(ps, null);
            if (result == null)
            {
                Mod.log.Info("GetAvailableContentPrefabs returned null.");
                return;
            }

            if (result is not IEnumerable enumerable)
            {
                Mod.log.Info($"GetAvailableContentPrefabs returned non-enumerable type: {result.GetType().FullName}");
                return;
            }

            Mod.log.Info("Scanning available content prefabs for industrial/zoning/tool keywords...");

            // Keep the filter broad; we’ll tighten once we see real names/types.
            string[] keys =
            {
                "industrial",
                "zone",
                "zoning",
                "tool",
                "industry"
            };

            int matches = 0;
            int total = 0;

            foreach (var obj in enumerable)
            {
                total++;

                if (obj == null) continue;

                string typeName = obj.GetType().FullName ?? obj.GetType().Name;

                // Try to get a meaningful name
                string name = TryGetPrefabName(ps, obj);

                // Fallback to UnityEngine.Object name if it is one
                if (string.IsNullOrWhiteSpace(name) && obj is UnityEngine.Object uo)
                    name = uo.name;

                if (string.IsNullOrWhiteSpace(name))
                    name = "(no-name)";

                // Filter by keywords in either the type name or the prefab name
                string hay = (name + " " + typeName).ToLowerInvariant();
                if (!keys.Any(k => hay.Contains(k)))
                    continue;

                matches++;
                Mod.log.Info($"Prefab match #{matches}: name='{name}' type='{typeName}'");

                // Don’t spam too hard on first run
                if (matches >= 200)
                {
                    Mod.log.Info("Stopping after 200 matches to avoid log spam.");
                    break;
                }
            }

            Mod.log.Info($"Prefab scan done. total_seen={total}, matches_logged={matches}");
        }

        private string TryGetPrefabName(Game.Prefabs.PrefabSystem ps, object prefabObj)
        {
            try
            {
                // Many CS2 prefab APIs let you ask the PrefabSystem for a prefab's name.
                // Signature could vary, so we try a couple of likely reflection calls.

                var pst = ps.GetType();

                // Try: GetPrefabName(prefab)
                var mi1 = pst.GetMethods()
                    .FirstOrDefault(m => m.Name == "GetPrefabName" && m.GetParameters().Length == 1);

                if (mi1 != null)
                {
                    var p0 = mi1.GetParameters()[0].ParameterType;
                    if (p0.IsInstanceOfType(prefabObj))
                    {
                        var s = mi1.Invoke(ps, new object[] { prefabObj }) as string;
                        if (!string.IsNullOrWhiteSpace(s)) return s;
                    }
                }

                // If it doesn’t accept the prefab object directly, we’ll just give up here.
                return null;
            }
            catch
            {
                return null;
            }
        }
    }
}

*/