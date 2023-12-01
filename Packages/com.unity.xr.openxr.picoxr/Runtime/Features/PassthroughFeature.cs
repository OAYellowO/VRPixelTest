using System;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.XR.OpenXR;
using UnityEngine.XR.OpenXR.Features;

#if UNITY_EDITOR
using UnityEditor.XR.OpenXR.Features;
#endif

namespace Unity.XR.OpenXR.Features.PICOSupport
{
#if UNITY_EDITOR
    [OpenXRFeature(UiName = "OpenXR Passthrough",
        Hidden = false,
        BuildTargetGroups = new[] { UnityEditor.BuildTargetGroup.Android },
        Company = "PICO",
        OpenxrExtensionStrings = extensionString,
        Version = "0.0.1",
        FeatureId = featureId)]
#endif
    public class PassthroughFeature : OpenXRFeatureBase
    {
        public const string featureId = "com.pico.openxr.feature.passthrough";
        public const string extensionString = "XR_FB_passthrough";
        public static bool isExtensionEnable =false;
        public override void Initialize(IntPtr intPtr)
        {
            isExtensionEnable=_isExtensionEnable;
            initialize(intPtr, xrInstance);
        }

        public override string GetExtensionString()
        {
            return extensionString;
        }
        
        public static bool EnableSeeThroughManual(bool value)
        {
            if (!isExtensionEnable)
            {
                return false;
            }
            return Passthrough_Enable(xrSession,value);
        }
        public static void Destroy()
        {
            if (!isExtensionEnable)
            {
                return ;
            }
            Passthrough_Destroy();
        }
        private void OnDestroy()
        {
            Destroy();
        }

        private const string ExtLib = "openxr_pico";

        [DllImport(ExtLib, EntryPoint = "PICO_initialize_Passthrough", CallingConvention = CallingConvention.Cdecl)]
        private static extern void initialize(IntPtr xrGetInstanceProcAddr, ulong xrInstance);
        [DllImport(ExtLib, EntryPoint = "PICO_Passthrough_Enable", CallingConvention = CallingConvention.Cdecl)]
        private static extern bool Passthrough_Enable(ulong xrSessionId, bool enable);

        [DllImport(ExtLib, EntryPoint = "PICO_Passthrough_Destroy", CallingConvention = CallingConvention.Cdecl)]
        private static extern void Passthrough_Destroy();
       
    }
}