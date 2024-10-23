using nadena.dev.ndmf;
using System;
using UnityEngine;

[assembly: ExportsPlugin(typeof(Narazaka.VRChat.AddMaterialSlots.Editor.AddMaterialSlotsPlugin))]

namespace Narazaka.VRChat.AddMaterialSlots.Editor
{
    public class AddMaterialSlotsPlugin : Plugin<AddMaterialSlotsPlugin>
    {
        public override string DisplayName => "AddMaterialSlots";

        public override string QualifiedName => "net.narazaka.vrchat.add-material-slots";

        protected override void Configure()
        {
            InPhase(BuildPhase.Resolving).BeforePlugin("nadena.dev.modular-avatar").Run("net.narazaka.vrchat.add-material-slots", (ctx) =>
            {
                var components = ctx.AvatarRootTransform.GetComponentsInChildren<AddMaterialSlots>(true);
                foreach (var component in components)
                {
                    var renderer = component.Renderer;
                    if (renderer == null) continue;

                    var slots = renderer.sharedMaterials;
                    var len = slots.Length;
                    switch (component.Mode)
                    {
                        case AddMaterialSlots.AddMode.AddCount:
                            Array.Resize(ref slots, len + component.MaterialCount);
                            break;
                        case AddMaterialSlots.AddMode.SetCount:
                            Array.Resize(ref slots, component.MaterialCount);
                            break;
                    }
                    for (int i = len; i < slots.Length; i++)
                    {
                        slots[i] = component.Material;
                    }
                    renderer.sharedMaterials = slots;
                    UnityEngine.Object.DestroyImmediate(component);
                }
            });
        }
    }
}
