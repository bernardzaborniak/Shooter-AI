using UnityEngine;

namespace FIMSpace.FLook
{
    /// <summary>
    /// FC: In this partial class we store helper methods
    /// </summary>
    public partial class FLookAnimator
    {
        #region Hierarchy window icon
        public string EditorIconPath { get { return "Look Animator/LookAnimator_SmallIcon"; } }
        public void OnDrop(UnityEngine.EventSystems.PointerEventData data) { }

        #endregion

        /// <summary> Variable helping eyes animator implementation drawing </summary>
        public bool _editor_hideEyes = false;

#if UNITY_EDITOR
        /// <summary>
        /// Validating changes for custom inspector window from editor script
        /// </summary>
        public void UpdateForCustomInspector()
        {
            OnValidate();
        }
#endif


        /// <summary>
        /// Computing elastic clamp angle for given parameters
        /// </summary>
        private float GetClampedAngle(float current, float limit, float elastic, float sign = 1f)
        {
            if (elastic <= 0f) return limit;
            else
            {
                float elasticRange = 0f;

                if (elastic > 0f)
                {
                    elasticRange = FEasing.EaseOutCubic(0f, elastic, (current * sign - limit * sign) / (180f + limit * sign));
                }

                return limit + elasticRange * sign;
            }
        }


        /// <summary>
        /// Computing variables making Quaternion.Look universal for different skeletonal setups
        /// </summary>
        private void ComputeBonesRotationsFixVariables()
        {
            if (BaseTransform != null)
            {
                Quaternion preRot = BaseTransform.rotation;
                BaseTransform.rotation = Quaternion.identity;

                FromAuto = LeadBone.rotation * -Vector3.forward;

                float angl = Quaternion.Angle(Quaternion.identity, LeadBone.rotation);
                Quaternion rotateAxis = (LeadBone.rotation * Quaternion.Inverse(Quaternion.FromToRotation(FromAuto, ModelForwardAxis)));

                OffsetAuto = Quaternion.AngleAxis(angl, rotateAxis.eulerAngles.normalized).eulerAngles;

                BaseTransform.rotation = preRot;

                parentalReferenceLookForward = Quaternion.Inverse(LeadBone.parent.rotation) * BaseTransform.rotation * ModelForwardAxis.normalized;
                parentalReferenceUp = Quaternion.Inverse(LeadBone.parent.rotation) * BaseTransform.rotation * ModelUpAxis.normalized;

                headForward = Quaternion.FromToRotation(LeadBone.InverseTransformDirection(BaseTransform.TransformDirection(ModelForwardAxis.normalized)), Vector3.forward) * Vector3.forward;
            }
            else
            {
                Debug.LogWarning("Base Transform isn't defined, so we can't use auto correction!");
            }
        }


        /// <summary>
        /// Returning forward direction of head bone in main transform space
        /// </summary>
        public Vector3 GetCurrentHeadForwardDirection()
        {
            return (LeadBone.rotation * Quaternion.FromToRotation(headForward, Vector3.forward)) * Vector3.forward;
        }

    }
}