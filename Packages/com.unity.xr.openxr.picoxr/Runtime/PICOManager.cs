using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using Unity.XR.CoreUtils;
using UnityEngine.XR;

namespace Unity.XR.OpenXR.Features.PICOSupport
{
    public class PICOManager : MonoBehaviour
    {
        private const string TAG = "[PICOManager]";
        private static PICOManager instance = null;
        private Camera[] eyeCamera;
        private XROrigin _xrOrigin;
        private XROrigin _xrOriginT;
        static List<XRInputSubsystem> s_InputSubsystems = new List<XRInputSubsystem>();
        private float cameraYOffset;
        private float cameraY;
        private bool isTrackingOriginMode = false;
        private TrackingOriginModeFlags currentTrackingOriginMode = TrackingOriginModeFlags.Unknown;
        private Vector3 _xrOriginPos = Vector3.zero;
        private Vector3 _xrOriginTPos = Vector3.zero;
        private Quaternion _xrOriginRot = Quaternion.identity;
        private Quaternion _xrOriginTRot = Quaternion.identity;
        private static GameObject local=null;
        public static PICOManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindObjectOfType<PICOManager>();
                    if (instance == null)
                    {
                        GameObject go = new GameObject("[PICOManager]");
                        DontDestroyOnLoad(go);
                        instance = go.AddComponent<PICOManager>();
                        PLog.e("PXRLog instance is not initialized!");
                    }
                }

                return instance;
            }
        }

        void Awake()
        {
            eyeCamera = new Camera[3];
            _xrOrigin = gameObject.GetComponent<XROrigin>();
            _xrOriginPos =new Vector3(Camera.main.transform.position.x,_xrOrigin.transform.position.y,Camera.main.transform.position.z) ;
            _xrOriginRot = Camera.main.transform.parent.rotation;
            Camera[] cam = gameObject.GetComponentsInChildren<Camera>();
            for (int i = 0; i < cam.Length; i++)
            {
                if (cam[i].stereoTargetEye == StereoTargetEyeMask.Both && cam[i] == Camera.main)
                {
                    eyeCamera[0] = cam[i];
                }
                else if (cam[i].stereoTargetEye == StereoTargetEyeMask.Left)
                {
                    eyeCamera[1] = cam[i];
                }
                else if (cam[i].stereoTargetEye == StereoTargetEyeMask.Right)
                {
                    eyeCamera[2] = cam[i];
                }
            }

            cameraY = this.transform.position.y;
            cameraYOffset = _xrOrigin.CameraYOffset;
            if (local==null)
            {
                local= new GameObject();
            }
        }

        public float getCameraYOffset()
        {
            if (currentTrackingOriginMode == TrackingOriginModeFlags.Floor)
            {
                return cameraY;
            }

            return cameraY + cameraYOffset;
        }

        private void Update()
        {
            if (!isTrackingOriginMode)
            {
                XRInputSubsystem subsystem = null;
                SubsystemManager.GetInstances(s_InputSubsystems);
                if (s_InputSubsystems.Count > 0)
                {
                    subsystem = s_InputSubsystems[0];
                }

                var mCurrentTrackingOriginMode = subsystem?.GetTrackingOriginMode();
                if (mCurrentTrackingOriginMode != null)
                {
                    isTrackingOriginMode = true;
                    currentTrackingOriginMode = (TrackingOriginModeFlags)mCurrentTrackingOriginMode;
                }
            }
        }

        private void OnEnable()
        {
            if (Camera.main.gameObject.GetComponent<CompositeLayerManager>() == null)
            {
                Camera.main.gameObject.AddComponent<CompositeLayerManager>();
            }
            foreach (var layer in CompositeLayerFeature.Instances)
            {
                if (eyeCamera[0] != null && eyeCamera[0].enabled)
                {
                    layer.RefreshCamera(eyeCamera[0], eyeCamera[0]);
                }
                else if (eyeCamera[1] != null && eyeCamera[1].enabled)
                {
                    layer.RefreshCamera(eyeCamera[1], eyeCamera[2]);
                }
            }
        }


        public Camera[] GetEyeCamera()
        {
            return eyeCamera;
        }
        public float GetOriginY()
        {
            return _xrOrigin.transform.position.y;
        }

       
        public bool GetOrigin(ref Vector3 pos,ref Quaternion rotation,ref Transform origin)
        {
            Transform transform=local.GetComponent<Transform>();
            transform.rotation = Quaternion.identity;
            origin = transform;
            XROrigin xrOrigin = FindObjectOfType<XROrigin>();
            if (!xrOrigin)
            {
                pos = Vector3.zero;
                rotation=Quaternion.identity;
                return false;
            }

            if (xrOrigin == _xrOrigin)
            {
                pos = _xrOriginPos;
                rotation=_xrOriginRot;
                return true;
            }
            else if (xrOrigin == _xrOriginT)
            {
                pos = _xrOriginTPos;
                rotation=_xrOriginTRot;
                return true;
            }

            _xrOriginT = xrOrigin;
            _xrOriginTPos  =new Vector3( Camera.main.transform.parent.position.x,xrOrigin.transform.position.y, Camera.main.transform.parent.position.z); 
            _xrOriginTRot =  Camera.main.transform.parent.rotation;
            pos = _xrOriginTPos;
            rotation=_xrOriginTRot;
            return true;
        }
        public float GetRefreshRate()
        {
            float i = -1;
            DisplayRefreshRateFeature.GetDisplayRefreshRate(ref i);
            return i;
        }

        public XrExtent2Df GetReferenceSpaceBoundsRect()
        {
            XrExtent2Df extent2D = new XrExtent2Df();
            OpenXRExtensions.GetReferenceSpaceBoundsRect(XrReferenceSpaceType.Stage, ref extent2D);
            return extent2D;
        }
    }
}