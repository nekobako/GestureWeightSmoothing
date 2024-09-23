#if GWS_VRCSDK3_AVATARS

using System;
using System.Collections.Generic;
using UnityEditor.Animations;
using VRC.SDK3.Avatars.Components;
using nadena.dev.ndmf;
using nadena.dev.modular_avatar.animation;

namespace net.nekobako.GestureWeightSmoothing.Editor
{
    using Runtime;

    internal class GestureWeightSmoothingPass : Pass<GestureWeightSmoothingPass>
    {
        protected override void Execute(BuildContext context)
        {
            var components = context.AvatarRootObject.GetComponentsInChildren<GestureWeightSmoothing>();
            if (components.Length == 0)
            {
                return;
            }

            foreach (var component in components)
            {
                var controller = context.GetOrInitializeController(component.LayerType);
                if (controller == null)
                {
                    continue;
                }

                var combiner = new AnimatorCombiner(controller.name, context.AssetContainer);

                combiner.AddController(string.Empty, controller, null);
                controller = combiner.Finish();

                if (RemapParameters(component.ParameterMappings, controller, out bool writeDefaults) && component.AnimatorController is AnimatorController ac)
                {
                    combiner.AddController(string.Empty, ac, component.MatchWriteDefaults ? writeDefaults : component.WriteDefaults);
                    controller = combiner.Finish();
                }

                context.AvatarDescriptor.customizeAnimationLayers = true;
                SetController(context.AvatarDescriptor.baseAnimationLayers);
                SetController(context.AvatarDescriptor.specialAnimationLayers);

                void SetController(VRCAvatarDescriptor.CustomAnimLayer[] layers)
                {
                    for (int i = 0; i < layers.Length; i++)
                    {
                        if (layers[i].type == component.LayerType)
                        {
                            layers[i].isDefault = false;
                            layers[i].animatorController = controller;
                        }
                    }
                }
            }

            foreach (var component in components)
            {
                UnityEngine.Object.DestroyImmediate(component);
            }
        }

        private static bool RemapParameters(IEnumerable<GestureWeightSmoothing.ParameterMapping> mappings, AnimatorController controller, out bool writeDefaults)
        {
            bool remapped = false;
            writeDefaults = false;

            foreach (var state in AnimationUtil.States(controller))
            {
                remapped |= RemapParameters(mappings, state);

                if (state.motion is not BlendTree blendTree || blendTree.blendType != BlendTreeType.Direct)
                {
                    writeDefaults |= state.writeDefaultValues;
                }
            }

            return remapped;
        }

        private static bool RemapParameters(IEnumerable<GestureWeightSmoothing.ParameterMapping> mappings, AnimatorState state)
        {
            bool remapped = false;

            if (state.speedParameterActive)
            {
                remapped |= RemapParameter(mappings, () => state.speedParameter, v => state.speedParameter = v);
            }
            if (state.cycleOffsetParameterActive)
            {
                remapped |= RemapParameter(mappings, () => state.cycleOffsetParameter, v => state.cycleOffsetParameter = v);
            }
            if (state.mirrorParameterActive)
            {
                remapped |= RemapParameter(mappings, () => state.mirrorParameter, v => state.mirrorParameter = v);
            }
            if (state.timeParameterActive)
            {
                remapped |= RemapParameter(mappings, () => state.timeParameter, v => state.timeParameter = v);
            }

            if (state.motion is BlendTree blendTree)
            {
                remapped |= RemapParameters(mappings, blendTree);
            }

            return remapped;
        }

        private static bool RemapParameters(IEnumerable<GestureWeightSmoothing.ParameterMapping> mappings, BlendTree blendTree)
        {
            bool remapped = false;

            switch (blendTree.blendType)
            {
                case BlendTreeType.Simple1D:
                    remapped |= RemapParameter(mappings, () => blendTree.blendParameter, v => blendTree.blendParameter = v);
                    break;

                case BlendTreeType.SimpleDirectional2D:
                case BlendTreeType.FreeformDirectional2D:
                case BlendTreeType.FreeformCartesian2D:
                    remapped |= RemapParameter(mappings, () => blendTree.blendParameter, v => blendTree.blendParameter = v);
                    remapped |= RemapParameter(mappings, () => blendTree.blendParameterY, v => blendTree.blendParameterY = v);
                    break;

                case BlendTreeType.Direct:
                    var children = blendTree.children;
                    for (int i = 0; i < children.Length; i++)
                    {
                        remapped |= RemapParameter(mappings, () => children[i].directBlendParameter, v => children[i].directBlendParameter = v);
                    }
                    blendTree.children = children;
                    break;

                default:
                    throw new Exception($"Unknown BlendTreeType: {blendTree.blendType}");
            }

            foreach (var child in blendTree.children)
            {
                if (child.motion is BlendTree bt)
                {
                    remapped |= RemapParameters(mappings, bt);
                }
            }

            return remapped;
        }

        private static bool RemapParameter(IEnumerable<GestureWeightSmoothing.ParameterMapping> mappings, Func<string> getter, Action<string> setter)
        {
            foreach (var mapping in mappings)
            {
                if (getter() == mapping.Src)
                {
                    setter(mapping.Dst);
                    return true;
                }
            }

            return false;
        }
    }
}

#endif
