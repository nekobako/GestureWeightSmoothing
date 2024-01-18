#if GWS_VRCSDK3_AVATARS

using nadena.dev.ndmf;
using net.nekobako.GestureWeightSmoothing.Editor;

[assembly: ExportsPlugin(typeof(GestureWeightSmoothingPlugin))]

namespace net.nekobako.GestureWeightSmoothing.Editor
{
    internal class GestureWeightSmoothingPlugin : Plugin<GestureWeightSmoothingPlugin>
    {
        protected override void Configure()
        {
            InPhase(BuildPhase.Transforming)
                .AfterPlugin("nadena.dev.modular-avatar")
                .Run(GestureWeightSmoothingPass.Instance);
        }
    }
}

#endif
