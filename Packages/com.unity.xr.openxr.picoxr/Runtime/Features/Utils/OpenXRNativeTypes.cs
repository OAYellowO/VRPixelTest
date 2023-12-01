using System;
using System.Runtime.InteropServices;
using UnityEngine;


namespace Unity.XR.OpenXR.Features.PICOSupport
{
    public enum XrResult
    {
        Success = 0,
        TimeoutExpored = 1,
        LossPending = 3,
        EventUnavailable = 4,
        SpaceBoundsUnavailable = 7,
        SessionNotFocused = 8,
        FrameDiscarded = 9,
        ValidationFailure = -1,
        RuntimeFailure = -2,
        OutOfMemory = -3,
        ApiVersionUnsupported = -4,
        InitializationFailed = -6,
        FunctionUnsupported = -7,
        FeatureUnsupported = -8,
        ExtensionNotPresent = -9,
        LimitReached = -10,
        SizeInsufficient = -11,
        HandleInvalid = -12,
        InstanceLOst = -13,
        SessionRunning = -14,
        SessionNotRunning = -16,
        SessionLost = -17,
        SystemInvalid = -18,
        PathInvalid = -19,
        PathCountExceeded = -20,
        PathFormatInvalid = -21,
        PathUnsupported = -22,
        LayerInvalid = -23,
        LayerLimitExceeded = -24,
        SpwachainRectInvalid = -25,
        SwapchainFormatUnsupported = -26,
        ActionTypeMismatch = -27,
        SessionNotReady = -28,
        SessionNotStopping = -29,
        TimeInvalid = -30,
        ReferenceSpaceUnsupported = -31,
        FileAccessError = -32,
        FileContentsInvalid = -33,
        FormFactorUnsupported = -34,
        FormFactorUnavailable = -35,
        ApiLayerNotPresent = -36,
        CallOrderInvalid = -37,
        GraphicsDeviceInvalid = -38,
        PoseInvalid = -39,
        IndexOutOfRange = -40,
        ViewConfigurationTypeUnsupported = -41,
        EnvironmentBlendModeUnsupported = -42,
        NameDuplicated = -44,
        NameInvalid = -45,
        ActionsetNotAttached = -46,
        ActionsetsAlreadyAttached = -47,
        LocalizedNameDuplicated = -48,
        LocalizedNameInvalid = -49,
        AndroidThreadSettingsIdInvalidKHR = -1000003000,
        AndroidThreadSettingsdFailureKHR = -1000003001,
        CreateSpatialAnchorFailedMSFT = -1000039001,
        SecondaryViewConfigurationTypeNotEnabledMSFT = -1000053000,
        MaxResult = 0x7FFFFFFF
    }

    public struct XrExtent2Df
    {
        public float width;
        public float height;

        public XrExtent2Df(float x, float y)
        {
            this.width = x;
            this.height = y;
        }

        public XrExtent2Df(Vector2 value)
        {
            width = value.x;
            height = value.y;
        }

        public override string ToString()
        {
            return $"{nameof(width)}: {width}, {nameof(height)}: {height}";
        }
    };

    // [Flags]
    public enum XrReferenceSpaceType
    {
        View = 1,
        Local = 2,
        Stage = 3,
        UnboundedMsft = 1000038000,
        CombinedEyeVarjo = 1000121000
    }

    public struct xrPose
    {
        public double PosX; // position of x
        public double PosY; // position of y
        public double PosZ; // position of z
        public double RotQx; // x components of Quaternion
        public double RotQy; // y components of Quaternion
        public double RotQz; // z components of Quaternion
        public double RotQw; // w components of Quaternion
    }
        
    public enum SecureContentFlag
    {
        SECURE_CONTENT_OFF = 0,
         SECURE_CONTENT_REPLACE_LAYER =2
    }

    public struct PxrRecti
    {
        public int x;
        public int y;
        public int width;
        public int height;
    };

    public enum PxrBlendFactor
    {
        PxrBlendFactorZero = 0,
        PxrBlendFactorOne = 1,
        PxrBlendFactorSrcAlpha = 2,
        PxrBlendFactorOneMinusSrcAlpha = 3,
        PxrBlendFactorDstAlpha = 4,
        PxrBlendFactorOneMinusDstAlpha = 5
    };

    [StructLayout(LayoutKind.Sequential)]
    public struct PxrLayerParam
    {
        public int layerId;
        public CompositeLayerFeature.OverlayShape layerShape;
        public CompositeLayerFeature.OverlayType layerType;
        public CompositeLayerFeature.LayerLayout layerLayout;
        public UInt64 format;
        public UInt32 width;
        public UInt32 height;
        public UInt32 sampleCount;
        public UInt32 faceCount;
        public UInt32 arraySize;
        public UInt32 mipmapCount;
        public UInt32 layerFlags;
        public UInt32 externalImageCount;
        public IntPtr leftExternalImages;
        public IntPtr rightExternalImages;
    };
    
    [StructLayout(LayoutKind.Sequential)]
    public struct PxrVector4f
    {
        public float x;
        public float y;
        public float z;
        public float w;
    };

    [StructLayout(LayoutKind.Sequential)]
    public struct PxrVector3f
    {
        public float x;
        public float y;
        public float z;
    };

    [StructLayout(LayoutKind.Sequential)]
    public struct PxrPosef
    {
        public PxrVector4f orientation;
        public PxrVector3f position;
    };

    [StructLayout(LayoutKind.Sequential)]
    public struct PxrLayerBlend
    {
        public PxrBlendFactor srcColor;
        public PxrBlendFactor dstColor;
        public PxrBlendFactor srcAlpha;
        public PxrBlendFactor dstAlpha;
    };

    [StructLayout(LayoutKind.Sequential)]
    public struct PxrLayerHeader2
    {
        public int layerId;
        public UInt32 layerFlags;
        public float colorScaleX;
        public float colorScaleY;
        public float colorScaleZ;
        public float colorScaleW;
        public float colorBiasX;
        public float colorBiasY;
        public float colorBiasZ;
        public float colorBiasW;
        public int compositionDepth;
        public int sensorFrameIndex;
        public int imageIndex;
        public PxrPosef headPose;
        public CompositeLayerFeature.OverlayShape layerShape;
        public UInt32 useLayerBlend;
        public PxrLayerBlend layerBlend;
        public UInt32 useImageRect;
        public PxrRecti imageRectLeft;
        public PxrRecti imageRectRight;
        public UInt64 reserved0;
        public UInt64 reserved1;
        public UInt64 reserved2;
        public UInt64 reserved3;
    };

    [StructLayout(LayoutKind.Sequential)]
    public struct PxrVector2f
    {
        public float x;
        public float y;
    };

    [StructLayout(LayoutKind.Sequential)]
    public struct PxrLayerQuad
    {
        public PxrLayerHeader2 header;
        public PxrPosef poseLeft;
        public PxrPosef poseRight;
        public PxrVector2f sizeLeft;
        public PxrVector2f sizeRight;
    };

    public enum PxrLayerSubmitFlags
    {
        PxrLayerFlagNoCompositionDepthTesting = 1 << 3,
        PxrLayerFlagUseExternalHeadPose = 1 << 5,
        PxrLayerFlagLayerPoseNotInTrackingSpace = 1 << 6,
        PxrLayerFlagHeadLocked = 1 << 7,
        PxrLayerFlagUseExternalImageIndex = 1 << 8,
    }
    
    public enum PxrLayerCreateFlags
    {
        PxrLayerFlagAndroidSurface = 1 << 0,
        PxrLayerFlagProtectedContent = 1 << 1,
        PxrLayerFlagStaticImage = 1 << 2,
        PxrLayerFlagUseExternalImages = 1 << 4,
        PxrLayerFlag3DLeftRightSurface = 1 << 5,
        PxrLayerFlag3DTopBottomSurface = 1 << 6,
        PxrLayerFlagEnableFrameExtrapolation = 1 << 7,
        PxrLayerFlagEnableSubsampled = 1 << 8,
        PxrLayerFlagEnableFrameExtrapolationPTW = 1 << 9,
        PxrLayerFlagSharedImagesBetweenLayers = 1 << 10,
    }

    public enum EyeType
    {
        EyeLeft,
        EyeRight,
        EyeBoth
    };
    
    /// <summary>
    /// Information about PICO Motion Tracker's connection state.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct PxrFitnessBandConnectState
    {
        /// <summary>
        /// 
        /// </summary>
        public Byte num;
        /// <summary>
        /// 
        /// </summary>
        public fixed Byte trackerID[12];
    }
}