using System;
using UnityEngine;

namespace FIMSpace.FLook
{
    /// <summary>
    /// FC: In this partial class we handling correct rotation for bone transforms
    /// </summary>
    public partial class FLookAnimator
    {


        /// <summary>
        /// Optional and basic calculations before main animation computing code
        /// Called every frame
        /// </summary>
        private void CalibrateBones()
        {
            if (RotationSpeed >= 2.5f) instantRotation = true; else instantRotation = false;

            // Referenec pose refresh trigger
            if (refreshReferencePose) RefreshReferencePose();


            #region Dynamic refresh for bone count changes

            if (_preBackBonesCount != BackBonesCount)
            {
                if (BackBonesCount > _preBackBonesCount)
                    for (int i = _preBackBonesCount; i < LookBones.Count; i++)
                        LookBones[i].RefreshStaticRotation(true);

                preWeightFaloff = FaloffValue - 0.001f;
                _preBackBonesCount = BackBonesCount;
            }

            #endregion


            // Backbones falloff update 
            if (!BigAngleAutomation)
            {
                if (AutoBackbonesWeights)
                {
                    if (FaloffValue != preWeightFaloff) SetAutoWeightsDefault();
                }
                else RefreshBoneMotionWeights();

                LookBones[0].motionWeight = LookBones[0].lookWeight;
            }
            else
                UpdateAutomationWeights();


            // Compensation bones update
            if ( DetectZeroKeyframes)
            {
                for (int i = 0; i < CompensationBones.Count; i++)
                {
                    if (CompensationBones[i].Transform == null) continue;
                    CompensationBones[i].RefreshCompensationFrame();
                    CompensationBones[i].CheckForZeroKeyframes();
                }
            }
            else
            {
                for (int i = 0; i < CompensationBones.Count; i++)
                { 
                    if (CompensationBones[i].Transform == null) continue; 
                    CompensationBones[i].RefreshCompensationFrame(); 
                }
            }


            // Target delta time update
            switch (DeltaType)
            {
                case EFDeltaType.DeltaTime: delta = Time.deltaTime; break;
                case EFDeltaType.SmoothDeltaTime: delta = Time.smoothDeltaTime; break;
                case EFDeltaType.UnscaledDeltaTime: delta = Time.unscaledDeltaTime; break;
                case EFDeltaType.FixedDeltaTime: delta = Time.fixedDeltaTime; break;
            }

            delta *= SimulationSpeed;

            // Monitoring animator for capturing bones rotation states
            //if (MonitorAnimator)
            //{
            //    if (animator == null) animator = GetComponentInChildren<Animator>();
            //    MonitorAnimatorCalculations();
            //}

            // Checking if there are not keyframed bones in look chain
            if (UseBoneOffsetRotation)
            {
                if (DetectZeroKeyframes)
                {
                    for (int i = 0; i < LookBones.Count; i++)
                    {
                        // Null keyframe detected 
                        if (Equals( LookBones[i].lastFinalLocalRotation, LookBones[i].Transform.localRotation) )
                            LookBones[i].Transform.localRotation = LookBones[i].lastKeyframeRotation;
                        else // Remembering rotation to check for null keyframe
                            LookBones[i].lastKeyframeRotation = LookBones[i].Transform.localRotation;
                    }
                }
            }


            if (RefreshStartLookPoint) RefreshLookStartPositionAnchor();

            // Changing switch target smoothness speed factor, very basic calculation, slowly going up to value = 1f
            changeTargetSmootherWeight = Mathf.Min(1f, changeTargetSmootherWeight + delta * 0.6f);
            changeTargetSmootherBones = Mathf.Min(1f, changeTargetSmootherBones + delta * 0.6f);
        }



        /// <summary>
        /// Applying calculated bones look rotations and bones compensation
        /// Applying final computed rotations and others 
        /// </summary>
        private void ChangeBonesRotations()
        {
            // Setting bones target calculated and smoothly animated rotations
            for (int i = 0; i < LookBones.Count; i++)
                LookBones[i].Transform.rotation = LookBones[i].finalRotation;

            // Rotate head bone towards desired rotation independly from backbones rotation
            LookBones[0].Transform.rotation = LookBones[0].finalRotation;

            // Compensating bones
            // Automation support
            if (BigAngleAutomationCompensation)
            {
                float f = Mathf.InverseLerp(45f, 170f, Mathf.Abs(unclampedLookAngles.y));
                targetCompensationWeight = Mathf.Lerp(CompensationWeight, CompensationWeightB, f);
                targetCompensationPosWeight = Mathf.Lerp(CompensatePositions, CompensatePositionsB, f);
            }
            else
            {
                targetCompensationWeight = CompensationWeight;
                targetCompensationPosWeight = CompensatePositions;
            }

            for (int i = 0; i < CompensationBones.Count; i++)
            {
                if (CompensationBones[i].Transform == null) continue;
                CompensationBones[i].SetRotationCompensation(targetCompensationWeight);
                CompensationBones[i].SetPositionCompensation(targetCompensationPosWeight);
            }

            // Remembering local rotation to check in next frame if bones are not keyframed
            if (UseBoneOffsetRotation)
            {
                for (int i = 0; i < LookBones.Count; i++)
                {
                    LookBones[i].lastFinalLocalRotation = LookBones[i].Transform.localRotation;
                }
            }

        }



        /// <summary>
        /// Resetting bones smoothing control variables
        /// </summary>
        private void ResetBones(bool onlyIfNull = false)
        {
            if (UseBoneOffsetRotation)
            {
                for (int i = 0; i < LookBones.Count; i++)
                {
                    LookBones[i].animatedTargetRotation = Quaternion.identity; // No Offset
                    LookBones[i].targetRotation = LookBones[i].animatedTargetRotation;
                    LookBones[i].finalRotation = LookBones[i].animatedTargetRotation;
                }
            }
            else
            {
                for (int i = 0; i < LookBones.Count; i++)
                {
                    LookBones[i].animatedTargetRotation = LookBones[i].Transform.rotation; // Hard targe rotation
                    LookBones[i].targetRotation = LookBones[i].animatedTargetRotation;
                    LookBones[i].finalRotation = LookBones[i].animatedTargetRotation;
                }
            }
        }



        /// <summary>
        /// Refreshing list of bones triggered by changing count of backbones through inspector window
        /// Called in Editor OnValidate() or by custom scripting
        /// </summary>
        internal void RefreshLookBones()
        {
            if (LookBones == null) { LookBones = new System.Collections.Generic.List<LookBone>(); LookBones.Add(new LookBone(null)); }

            if (LookBones.Count == 0) { LookBones.Add(new LookBone(null)); }
            if (LookBones.Count > BackBonesCount + 1) LookBones.RemoveRange(BackBonesCount + 1, LookBones.Count - (BackBonesCount + 1));


            if (LeadBone)
            {
                if (LookBones[0].Transform != LeadBone) { LookBones[0] = new LookBone(LeadBone); if ( BaseTransform ) LookBones[0].RefreshBoneDirections(BaseTransform); }

                for (int i = 1; i < 1 + BackBonesCount; i++)
                {
                    if (i >= LookBones.Count)
                    {
                        LookBone l = new LookBone(LookBones[i - 1].Transform.parent);
                        LookBones.Add(l);
                        if (BaseTransform) l.RefreshBoneDirections(BaseTransform);
                    }
                    else
                    if (LookBones[i] == null || LookBones[i].Transform == null) { LookBones[i] = new LookBone(LookBones[i - 1].Transform.parent); if (BaseTransform) LookBones[i].RefreshBoneDirections(BaseTransform); }
                }
            }
            else
            {
                if (LookBones.Count > 1) LookBones.RemoveRange(1, LookBones.Count);
            }

        }


    }
}