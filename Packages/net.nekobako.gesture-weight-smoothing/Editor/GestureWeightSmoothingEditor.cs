#if GWS_VRCSDK3_AVATARS

using UnityEditor;

namespace net.nekobako.GestureWeightSmoothing.Editor
{
    using Runtime;

    [CustomEditor(typeof(GestureWeightSmoothing))]
    internal class GestureWeightSmoothingEditor : UnityEditor.Editor
    {
        private SerializedProperty m_LayerTypeProperty = null;
        private SerializedProperty m_AnimatorControllerProperty = null;
        private SerializedProperty m_MatchWriteDefaultsProperty = null;
        private SerializedProperty m_WriteDefaultsProperty = null;
        private SerializedProperty m_ParameterMappingsProperty = null;

        private void OnEnable()
        {
            m_LayerTypeProperty = serializedObject.FindProperty(nameof(GestureWeightSmoothing.LayerType));
            m_AnimatorControllerProperty = serializedObject.FindProperty(nameof(GestureWeightSmoothing.AnimatorController));
            m_MatchWriteDefaultsProperty = serializedObject.FindProperty(nameof(GestureWeightSmoothing.MatchWriteDefaults));
            m_WriteDefaultsProperty = serializedObject.FindProperty(nameof(GestureWeightSmoothing.WriteDefaults));
            m_ParameterMappingsProperty = serializedObject.FindProperty(nameof(GestureWeightSmoothing.ParameterMappings));
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(m_LayerTypeProperty);
            EditorGUILayout.PropertyField(m_AnimatorControllerProperty);

            EditorGUILayout.PropertyField(m_MatchWriteDefaultsProperty);
            if (!m_MatchWriteDefaultsProperty.boolValue)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(m_WriteDefaultsProperty);
                EditorGUI.indentLevel--;
            }

            EditorGUILayout.PropertyField(m_ParameterMappingsProperty);

            serializedObject.ApplyModifiedProperties();
        }
    }
}

#endif
