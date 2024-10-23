using UnityEngine;
using VRC.SDKBase;
using VRC.SDK3.Avatars.Components;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Narazaka.VRChat.AddMaterialSlots
{
    public class AddMaterialSlots : MonoBehaviour, IEditorOnly
    {
        public enum AddMode
        {
            AddCount,
            SetCount,
        }
        public AddMode Mode;
        public int MaterialCount;
        public Material Material;
        [Tooltip("If Renderer is null, apply to this object.")]
        public string RendererPath;
        public Renderer Renderer
        {
            get
            {
                if (string.IsNullOrEmpty(RendererPath))
                {
                    return GetComponent<Renderer>();
                }
                var avatar = GetAvatar();
                if (avatar == null) return null;
                var obj = avatar.Find(RendererPath);
                if (obj == null) return null;
                return obj.GetComponent<Renderer>();
            }
        }

        Transform GetAvatar()
        {
            var avatar = GetComponentInParent<VRCAvatarDescriptor>();
            if (avatar == null) return null;
            return avatar.transform;
        }

#if UNITY_EDITOR
        [CustomEditor(typeof(AddMaterialSlots))]
        public class AddMaterialSlotsEditor : Editor
        {
            SerializedProperty RendererPath;
            GUIContent RendererLabel;

            void OnEnable()
            {
                RendererPath = serializedObject.FindProperty(nameof(RendererPath));
                RendererLabel = new GUIContent("Renderer");
            }

            public override void OnInspectorGUI()
            {
                base.OnInspectorGUI();

                serializedObject.UpdateIfRequiredOrScript();
                EditorGUI.BeginChangeCheck();
                var renderer = EditorGUILayout.ObjectField(RendererLabel, (target as AddMaterialSlots).Renderer, typeof(Renderer), true);
                if (EditorGUI.EndChangeCheck())
                {
                    RendererPath.stringValue = GetPathInAvatar(renderer as Renderer);
                    serializedObject.ApplyModifiedProperties();
                }
            }

            string GetPathInAvatar(Renderer renderer)
            {
                if (renderer == null) return null;
                var addMaterialSlots = target as AddMaterialSlots;
                var avatar = addMaterialSlots.GetAvatar();
                if (avatar == null) return null;
                var path = renderer.name;
                var tr = renderer.transform;
                while (true)
                {
                    tr = tr.parent;
                    if (tr == null || tr == avatar) break;
                    path = tr.name + "/" + path;
                }
                return path;
            }
        }
#endif
    }
}
