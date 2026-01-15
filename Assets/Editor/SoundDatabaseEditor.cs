using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using MyMetroidVania.Data.ScriptableObjects;

namespace MyMetroidVania
{
    [CustomEditor(typeof(SoundDatabase))]
    public class SoundDatabaseEditor : Editor
    {
        private ReorderableList _reorderableList;

        private void OnEnable()
        {
            // _soundDataList プロパティを探す
            var prop = serializedObject.FindProperty("_soundDataList");
            _reorderableList = new ReorderableList(serializedObject, prop, true, true, true, true);

            // ヘッダーの描画
            _reorderableList.drawHeaderCallback = (Rect rect) => {
                EditorGUI.LabelField(rect, "Sound Data List");
            };

            // リストの要素（SoundDataの中身）をどう描画するか
            _reorderableList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) => {
                var element = prop.GetArrayElementAtIndex(index);
                rect.y += 2;

                // 各要素のフィールドを1行で表示する（Clip, Volume, Loop）
                float spacing = 5;
                float clipWidth = rect.width * 0.4f;
                float volWidth = rect.width * 0.35f;
                float loopWidth = rect.width * 0.15f;

                // _clipを表示
                EditorGUI.PropertyField(
                    new Rect(rect.x, rect.y, clipWidth, EditorGUIUtility.singleLineHeight),
                    element.FindPropertyRelative("_clip"), GUIContent.none);

                // _volume
                EditorGUI.PropertyField(
                    new Rect(rect.x + clipWidth + spacing, rect.y, volWidth, EditorGUIUtility.singleLineHeight),
                    element.FindPropertyRelative("_volume"), GUIContent.none);
                element.FindPropertyRelative("_volume").floatValue = 0.5f;


                // _vsLoop
                EditorGUI.LabelField(new Rect(rect.x + clipWidth + volWidth + spacing * 2, rect.y, 40, rect.height), "Loop");
                EditorGUI.PropertyField(
                    new Rect(rect.x + clipWidth + volWidth + spacing * 2 + 35, rect.y, loopWidth, EditorGUIUtility.singleLineHeight),
                    element.FindPropertyRelative("_isLoop"), GUIContent.none);
                element.FindPropertyRelative("_isLoop").boolValue = false;
            };


            // 各要素の高さ
            _reorderableList.elementHeight = EditorGUIUtility.singleLineHeight + 4;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.Space();

            _reorderableList.DoLayoutList();

            serializedObject.ApplyModifiedProperties();
        }
    }
}