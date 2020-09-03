#if UNITY_EDITOR

using System.IO;
using UnityEditor;
using UnityEngine;

namespace FIMSpace.FEditor
{
    public static class FGUI_Finders
    {
        public static bool CheckForAnimator(GameObject root, bool needAnimatorBox = true, bool drawInactiveWarning = true)
        {
            Animation animation = null;
            Animator animator = root.GetComponentInChildren<Animator>();
            if (!animator) if (root.transform.parent) if (root.transform.parent.parent) animator = root.transform.parent.parent.GetComponentInChildren<Animator>(); else animator = root.transform.parent.GetComponent<Animator>();

            if (!animator)
            {
                animation = root.GetComponentInChildren<Animation>();
                if (!animation) if (root.transform.parent) if (root.transform.parent.parent) animation = root.transform.parent.parent.GetComponentInChildren<Animation>(); else animation = root.transform.parent.GetComponent<Animation>();
            }

                if (animator)
                {
                    if (animator.runtimeAnimatorController == null)
                    {
                        EditorGUILayout.HelpBox("No 'Animator Controller' inside Animator", MessageType.Warning);
                        animator = null;
                    }
                }

            if (needAnimatorBox)
            {
                if ( animator == null && animation == null)
                {
                    GUILayout.Space(-4);
                    FGUI_Inspector.DrawWarning(" ANIMATOR NOT FOUND! ");
                    GUILayout.Space(2);
                }
                else
                if ( drawInactiveWarning)
                {
                    bool drawInact = false;
                    if (animator != null) if (animator.enabled == false) drawInact = true;
                    if (animation != null) if (animation.enabled == false) drawInact = true;
                    if( drawInact)
                    {
                        GUILayout.Space(-4);
                        FGUI_Inspector.DrawWarning(" ANIMATOR IS DISABLED! ");
                        GUILayout.Space(2);
                    }
                }
            }

                if (animator != null || animation != null)
            {
                if (animator) if (!animator.enabled) return false;
                if (animation) if (!animation.enabled) return false;
                return true;
            }
            else return false;
        }

    }
}

#endif
