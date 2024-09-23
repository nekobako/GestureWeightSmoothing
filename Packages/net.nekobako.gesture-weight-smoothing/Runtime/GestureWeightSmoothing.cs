#if GWS_VRCSDK3_AVATARS

using System;
using System.Collections.Generic;
using UnityEngine;
using VRC.SDK3.Avatars.Components;
using VRC.SDKBase;

namespace net.nekobako.GestureWeightSmoothing.Runtime
{
    internal class GestureWeightSmoothing : MonoBehaviour, IEditorOnly
    {
        [Serializable]
        public struct ParameterMapping
        {
            public string Src;
            public string Dst;
        }

        public VRCAvatarDescriptor.AnimLayerType LayerType = VRCAvatarDescriptor.AnimLayerType.FX;
        public RuntimeAnimatorController AnimatorController = null;
        public bool MatchWriteDefaults = true;
        public bool WriteDefaults = false;
        public List<ParameterMapping> ParameterMappings = new();
    }
}

#endif
