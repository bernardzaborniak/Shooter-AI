using FIMSpace.FEditor;
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

namespace FIMSpace.FLook
{
    public partial class FLookAnimator_Editor
    {
        public override void OnInspectorGUI()
        {
            Undo.RecordObject(target, "Look Animator Inspector");

            serializedObject.Update();

            FLookAnimator Get = (FLookAnimator)target;
            string title = drawDefaultInspector ? " Default Inspector" : " Look Animator 2";
            if (!drawNewInspector) title = " Old GUI Version";


            //EditorGUILayout.PropertyField(serializedObject.FindProperty("targetLookAngles"));
            //EditorGUILayout.PropertyField(serializedObject.FindProperty("animatedLookAngles"));

            HeaderBoxMain(title, ref Get.drawGizmos, ref drawDefaultInspector, _TexLookAnimIcon, Get, 27);


            #region Default Inspector
            if (drawDefaultInspector)
            {
                #region Exluding from view not needed properties

                List<string> excludedVars = new List<string>();

                if (Get.BackBonesCount < 1)
                {
                    excludedVars.Add("BackBonesTransforms");
                    excludedVars.Add("BackBonesFalloff");
                    excludedVars.Add("m_script");
                }

                #endregion

                GUILayout.Space(5f);

                if (Get.BackBonesCount < 0) Get.BackBonesCount = 0;

                // Draw default inspector without not needed properties
                DrawPropertiesExcluding(serializedObject, excludedVars.ToArray());
            }
            else
            #endregion

            {
                if (drawNewInspector)
                {
                    GUILayout.Space(4f);
                    DrawNewGUI();
                }
                else
                {
                    DrawOldGUI();
                }
            }

            // Apply changed parameters variables
            serializedObject.ApplyModifiedProperties();
        }


        private void DrawNewGUI()
        {
            #region Preparations for unity versions and skin

            c = Color.Lerp(GUI.color * new Color(0.8f, 0.8f, 0.8f, 0.7f), GUI.color, Mathf.InverseLerp(0f, 0.15f, Get.LookAnimatorAmount));

            RectOffset zeroOff = new RectOffset(0, 0, 0, 0);
            float bgAlpha = 0.05f; if (EditorGUIUtility.isProSkin) bgAlpha = 0.1f;

#if UNITY_2019_3_OR_NEWER
        int headerHeight = 22;
#else
            int headerHeight = 25;
#endif

            #endregion


            GUILayout.BeginVertical(FGUI_Resources.BGBoxStyle); GUILayout.Space(1f);
            GUILayout.BeginVertical(FGUI_Inspector.Style(zeroOff, zeroOff, new Color(.7f, .7f, 0.7f, bgAlpha), Vector4.one * 3, 3));

            FGUI_Inspector.HeaderBox(ref drawSetup, Lang("Character Setup"), true, FGUI_Resources.Tex_GearSetup, headerHeight, headerHeight - 1, LangBig());
            if (drawSetup) Tab_DrawSetup();

            GUILayout.EndVertical();

            // ------------------------------------------------------------------------

            GUILayout.BeginVertical(FGUI_Inspector.Style(zeroOff, zeroOff, new Color(.3f, .4f, 1f, bgAlpha), Vector4.one * 3, 3));
            FGUI_Inspector.HeaderBox(ref drawTweaking, Lang("Tweak Animation"), true, FGUI_Resources.Tex_Sliders, headerHeight, headerHeight - 1, LangBig());

            if (drawTweaking) Tab_DrawTweaking();

            GUILayout.EndVertical();

            // ------------------------------------------------------------------------

            GUILayout.BeginVertical(FGUI_Inspector.Style(zeroOff, zeroOff, new Color(.675f, .675f, 0.275f, bgAlpha), Vector4.one * 3, 3));
            FGUI_Inspector.HeaderBox(ref drawLimiting, Lang("Limit Animation Behaviour"), true, FGUI_Resources.Tex_Knob, headerHeight, headerHeight - 1, LangBig());

            Get._gizmosDrawingLimiting = drawLimiting;
            if (drawLimiting) Tab_DrawLimiting();

            GUILayout.EndVertical();

            // ------------------------------------------------------------------------


            GUILayout.BeginVertical(FGUI_Inspector.Style(zeroOff, zeroOff, new Color(.3f, 1f, .7f, bgAlpha), Vector4.one * 3, 3));
            FGUI_Inspector.HeaderBox(ref drawAdditional, Lang("Additional Modules"), true, FGUI_Resources.Tex_Module, headerHeight, headerHeight - 1, LangBig());

            if (drawAdditional) Tab_DrawAdditionalFeatures();

            GUILayout.EndVertical();

            // ------------------------------------------------------------------------

            GUILayout.BeginVertical(FGUI_Inspector.Style(zeroOff, zeroOff, new Color(1f, .4f, .4f, bgAlpha * 0.5f), Vector4.one * 3, 3));
            FGUI_Inspector.HeaderBox(ref drawCorrecting, Lang("Corrections"), true, FGUI_Resources.Tex_Repair, headerHeight, headerHeight - 1, LangBig());

            if (drawCorrecting) Tab_DrawCorrections();

            GUILayout.EndVertical();

            GUILayout.Space(2f);
            GUILayout.EndVertical();

        }


    }
}