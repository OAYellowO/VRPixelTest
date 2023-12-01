using System;
using UnityEngine;
using UnityEngine.Rendering;

namespace  Unity.XR.OpenXR.Features.PICOSupport
{
    public class CompositeLayerManager : MonoBehaviour
    {
        private void OnEnable()
        {
#if UNITY_2019_1_OR_NEWER
            if (GraphicsSettings.renderPipelineAsset != null)
            {
                RenderPipelineManager.beginFrameRendering += BeginRendering;
                RenderPipelineManager.endFrameRendering += EndRendering;
            }
            else
            {
                Camera.onPreRender += OnPreRenderCallBack;
                Camera.onPostRender += OnPostRenderCallBack;
            }
#endif
        }

        private void OnDisable()
        {
#if UNITY_2019_1_OR_NEWER
            if (GraphicsSettings.renderPipelineAsset != null)
            {
                RenderPipelineManager.beginFrameRendering -= BeginRendering;
                RenderPipelineManager.endFrameRendering -= EndRendering;
            }
            else
            {
                Camera.onPreRender -= OnPreRenderCallBack;
                Camera.onPostRender -= OnPostRenderCallBack;
            }
#endif
        }

        private void Start()
        {
            
        }

        private void BeginRendering(ScriptableRenderContext arg1, Camera[] arg2)
        {
            foreach (Camera cam in arg2)
            {
                if (cam != null && Camera.main == cam)
                {
                    OnPreRenderCallBack(cam);
                }
            }
        }

        private void EndRendering(ScriptableRenderContext arg1, Camera[] arg2)
        {
            foreach (Camera cam in arg2)
            {
                if (cam != null && Camera.main == cam)
                {
                    OnPostRenderCallBack(cam);
                }
            }
        }

        private void OnPreRenderCallBack(Camera cam)
        {
            // There is only one XR main camera in the scene.
            if (null == Camera.main) return;
            if (cam == null || cam != Camera.main || cam.stereoActiveEye == Camera.MonoOrStereoscopicEye.Right) return;

            //CompositeLayers
            if (null == CompositeLayerFeature.Instances) return;
            if (CompositeLayerFeature.Instances.Count > 0)
            {
                foreach (var overlay in CompositeLayerFeature.Instances)
                {
                    if (!overlay.isActiveAndEnabled) continue;
                    if (null == overlay.layerTextures) continue;
                    if (!overlay.isClones && overlay.layerTextures[0] == null && overlay.layerTextures[1] == null ) continue;
                    if (overlay.overlayTransform != null && !overlay.overlayTransform.gameObject.activeSelf) continue;
                    overlay.CreateTexture();
                    OpenXRExtensions.GetLayerNextImageIndex(overlay.overlayIndex, ref overlay.imageIndex);
                }
            }
        }

        private void OnPostRenderCallBack(Camera cam)
        {
            // There is only one XR main camera in the scene.
            if (null == Camera.main) return;
            if (cam == null || cam != Camera.main || cam.stereoActiveEye == Camera.MonoOrStereoscopicEye.Right) return;
            
            if (null == CompositeLayerFeature.Instances) return;
            if (CompositeLayerFeature.Instances.Count > 0 )
            {
                CompositeLayerFeature.Instances.Sort();
                foreach (var compositeLayer in CompositeLayerFeature.Instances)
                {
                    if (null == compositeLayer) continue;
                    compositeLayer.UpdateCoords();
                    if (!compositeLayer.isActiveAndEnabled) continue;
                    if (null == compositeLayer.layerTextures) continue;
                    if (!compositeLayer.isClones && compositeLayer.layerTextures[0] == null && compositeLayer.layerTextures[1] == null ) continue;
                    if (compositeLayer.overlayTransform != null && null == compositeLayer.overlayTransform.gameObject) continue;
                    if (compositeLayer.overlayTransform != null && !compositeLayer.overlayTransform.gameObject.activeSelf) continue;

                    Vector4 colorScale = compositeLayer.GetLayerColorScale();
                    Vector4 colorBias = compositeLayer.GetLayerColorOffset();
                    bool isHeadLocked = compositeLayer.overlayTransform != null && compositeLayer.overlayTransform.parent == transform;
                    
                    if ( !compositeLayer.CopyRT()) continue;
                    if (null == compositeLayer.cameraRotations || null == compositeLayer.modelScales || null == compositeLayer.modelTranslations) continue;

            

                    if (compositeLayer.overlayShape == CompositeLayerFeature.OverlayShape.Quad)
                    {
                        PxrLayerQuad layerSubmit2 = new PxrLayerQuad();
                        layerSubmit2.header.layerId = compositeLayer.overlayIndex;
                        layerSubmit2.header.colorScaleX = colorScale.x;
                        layerSubmit2.header.colorScaleY = colorScale.y;
                        layerSubmit2.header.colorScaleZ = colorScale.z;
                        layerSubmit2.header.colorScaleW = colorScale.w;
                        layerSubmit2.header.colorBiasX = colorBias.x;
                        layerSubmit2.header.colorBiasY = colorBias.y;
                        layerSubmit2.header.colorBiasZ = colorBias.z;
                        layerSubmit2.header.colorBiasW = colorBias.w;
                        layerSubmit2.header.compositionDepth = compositeLayer.layerDepth;
                        layerSubmit2.header.headPose.orientation.x = compositeLayer.cameraRotations[0].x;
                        layerSubmit2.header.headPose.orientation.y = compositeLayer.cameraRotations[0].y;
                        layerSubmit2.header.headPose.orientation.z = -compositeLayer.cameraRotations[0].z;
                        layerSubmit2.header.headPose.orientation.w = -compositeLayer.cameraRotations[0].w;
                        layerSubmit2.header.headPose.position.x = (compositeLayer.cameraTranslations[0].x + compositeLayer.cameraTranslations[1].x) / 2;
                        layerSubmit2.header.headPose.position.y = (compositeLayer.cameraTranslations[0].y + compositeLayer.cameraTranslations[1].y) / 2;
                        layerSubmit2.header.headPose.position.z = -(compositeLayer.cameraTranslations[0].z + compositeLayer.cameraTranslations[1].z) / 2;
                        layerSubmit2.header.layerShape = CompositeLayerFeature.OverlayShape.Quad;
                        layerSubmit2.header.useLayerBlend = (UInt32)(compositeLayer.useLayerBlend ? 1 : 0);
                        layerSubmit2.header.layerBlend.srcColor = compositeLayer.srcColor;
                        layerSubmit2.header.layerBlend.dstColor = compositeLayer.dstColor;
                        layerSubmit2.header.layerBlend.srcAlpha = compositeLayer.srcAlpha;
                        layerSubmit2.header.layerBlend.dstAlpha = compositeLayer.dstAlpha;
                        layerSubmit2.header.useImageRect = (UInt32)(compositeLayer.useImageRect ? 1 : 0);
                        layerSubmit2.header.imageRectLeft = compositeLayer.getPxrRectiLeft(true);
                        layerSubmit2.header.imageRectRight = compositeLayer.getPxrRectiLeft(false);

                        layerSubmit2.sizeLeft.x = compositeLayer.modelScales[0].x;
                        layerSubmit2.sizeLeft.y = compositeLayer.modelScales[0].y;
                        layerSubmit2.sizeRight.x = compositeLayer.modelScales[0].x;
                        layerSubmit2.sizeRight.y = compositeLayer.modelScales[0].y;

                        if (isHeadLocked)
                        {
                            layerSubmit2.poseLeft.orientation.x = compositeLayer.overlayTransform.localRotation.x;
                            layerSubmit2.poseLeft.orientation.y = compositeLayer.overlayTransform.localRotation.y;
                            layerSubmit2.poseLeft.orientation.z = -compositeLayer.overlayTransform.localRotation.z;
                            layerSubmit2.poseLeft.orientation.w = -compositeLayer.overlayTransform.localRotation.w;
                            layerSubmit2.poseLeft.position.x = compositeLayer.overlayTransform.localPosition.x;
                            layerSubmit2.poseLeft.position.y = compositeLayer.overlayTransform.localPosition.y;
                            layerSubmit2.poseLeft.position.z = -compositeLayer.overlayTransform.localPosition.z;

                            layerSubmit2.poseRight.orientation.x = compositeLayer.overlayTransform.localRotation.x;
                            layerSubmit2.poseRight.orientation.y = compositeLayer.overlayTransform.localRotation.y;
                            layerSubmit2.poseRight.orientation.z = -compositeLayer.overlayTransform.localRotation.z;
                            layerSubmit2.poseRight.orientation.w = -compositeLayer.overlayTransform.localRotation.w;
                            layerSubmit2.poseRight.position.x = compositeLayer.overlayTransform.localPosition.x;
                            layerSubmit2.poseRight.position.y = compositeLayer.overlayTransform.localPosition.y;
                            layerSubmit2.poseRight.position.z = -compositeLayer.overlayTransform.localPosition.z;

                            layerSubmit2.header.layerFlags = (UInt32)(
                                PxrLayerSubmitFlags.PxrLayerFlagLayerPoseNotInTrackingSpace |
                                PxrLayerSubmitFlags.PxrLayerFlagHeadLocked);
                        }
                        else
                        {
                            
                            Quaternion quaternion = new Quaternion(compositeLayer.modelRotations[0].x,
                                compositeLayer.modelRotations[0].y, compositeLayer.modelRotations[0].z,
                                compositeLayer.modelRotations[0].w);
                          
                            Vector3 cameraPos = Vector3.zero;
                            Quaternion cameraRot = Quaternion.identity;
                            Transform origin = null;
                            bool ret = PICOManager.Instance.GetOrigin(ref cameraPos, ref cameraRot,ref origin);
                            if (!ret)
                            {
                                return;
                            }
                            // Quaternion lQuaternion =  Quaternion.Euler(-cameraRot.eulerAngles.x,
                            //     -cameraRot.eulerAngles.y, -cameraRot.eulerAngles.z);
                            Quaternion lQuaternion =  new Quaternion(-cameraRot.x,-cameraRot.y,-cameraRot.z,cameraRot.w);
                            Vector3 pos = new Vector3(compositeLayer.modelTranslations[0].x - cameraPos.x,
                                compositeLayer.modelTranslations[0].y - PICOManager.Instance.getCameraYOffset() +
                                PICOManager.Instance.GetOriginY() - cameraPos.y, compositeLayer.modelTranslations[0].z-cameraPos.z);
                  
                            quaternion  *= lQuaternion;
                            origin.rotation *= lQuaternion;
                         
                            pos = origin.TransformPoint(pos);
                          
                           
                           
                            // Quaternion.l
                            layerSubmit2.poseLeft.position.x = pos.x;
                            layerSubmit2.poseLeft.position.y = pos.y;
                            layerSubmit2.poseLeft.position.z = -pos.z;
                            layerSubmit2.poseLeft.orientation.x = -quaternion.x;
                            layerSubmit2.poseLeft.orientation.y = -quaternion.y;
                            layerSubmit2.poseLeft.orientation.z = quaternion.z;
                            layerSubmit2.poseLeft.orientation.w = quaternion.w;
                            
                            layerSubmit2.poseRight.position.x = pos.x;
                            layerSubmit2.poseRight.position.y = pos.y;
                            layerSubmit2.poseRight.position.z = -pos.z;
                            layerSubmit2.poseRight.orientation.x = -quaternion.x;
                            layerSubmit2.poseRight.orientation.y = -quaternion.y;
                            layerSubmit2.poseRight.orientation.z = quaternion.z;
                            layerSubmit2.poseRight.orientation.w = quaternion.w;

                            
                            layerSubmit2.header.layerFlags = (UInt32)(
                                PxrLayerSubmitFlags.PxrLayerFlagUseExternalHeadPose |
                                PxrLayerSubmitFlags.PxrLayerFlagLayerPoseNotInTrackingSpace);
                        }

                        if (compositeLayer.useImageRect)
                        {
                            layerSubmit2.poseLeft.position.x += -0.5f + compositeLayer.dstRectLeft.x + 0.5f * Mathf.Min(compositeLayer.dstRectLeft.width, 1 - compositeLayer.dstRectLeft.x);
                            layerSubmit2.poseLeft.position.y += -0.5f + compositeLayer.dstRectLeft.y + 0.5f * Mathf.Min(compositeLayer.dstRectLeft.height, 1 - compositeLayer.dstRectLeft.y);
                            layerSubmit2.poseRight.position.x += -0.5f + compositeLayer.dstRectRight.x + 0.5f * Mathf.Min(compositeLayer.dstRectRight.width, 1 - compositeLayer.dstRectRight.x);
                            layerSubmit2.poseRight.position.y += -0.5f + compositeLayer.dstRectRight.y + 0.5f * Mathf.Min(compositeLayer.dstRectRight.height, 1 - compositeLayer.dstRectRight.y);

                            layerSubmit2.sizeLeft.x = compositeLayer.modelScales[0].x * Mathf.Min(compositeLayer.dstRectLeft.width, 1 - compositeLayer.dstRectLeft.x);
                            layerSubmit2.sizeLeft.y = compositeLayer.modelScales[0].y * Mathf.Min(compositeLayer.dstRectLeft.height, 1 - compositeLayer.dstRectLeft.y);
                            layerSubmit2.sizeRight.x = compositeLayer.modelScales[0].x * Mathf.Min(compositeLayer.dstRectRight.width, 1 - compositeLayer.dstRectRight.x);
                            layerSubmit2.sizeRight.y = compositeLayer.modelScales[0].y * Mathf.Min(compositeLayer.dstRectRight.height, 1 - compositeLayer.dstRectRight.y);
                        }

                 
     
                        OpenXRExtensions.SubmitLayerQuad(layerSubmit2);
                    }
                }
            }
        }
    }
}