using UnityEngine;
using UnityEditor;

namespace Snorlax.Prototype.AngleEditor
{
    [CustomEditor(typeof(AngleBool))]
    public class AngleBoolEditor : Editor
    {
        SerializedProperty serializedProperty;
        AngleBool cb;
        bool mainFoldout;

        void OnEnable() { EditorApplication.update += Update; cb = (AngleBool)target; serializedProperty = serializedObject.FindProperty(nameof(AngleBool.AngleList)); }
        void OnDisable() { EditorApplication.update -= Update; }

        void Update()
        {
            Repaint();
        }


        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            serializedObject.ApplyModifiedProperties();
            int i = 0;

            //base.OnInspectorGUI();
            
            mainFoldout = (EditorGUILayout.Foldout(mainFoldout, "Angles ", EditorStyles.foldout));

            if (mainFoldout)
            {
                EditorGUI.BeginChangeCheck();

                if (GUILayout.Button("New Angle"))
                {
                    AddItemToArray(ref cb.AngleList);
                }

                EditorGUI.indentLevel++;
                foreach (SerializedProperty A in serializedProperty)
                {
                    EditorGUILayout.BeginVertical("box", GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
                    {
                        A.isExpanded = (EditorGUILayout.Foldout(A.isExpanded, "Angle " + i, EditorStyles.foldout));

                        if (A.isExpanded)
                        {
                            EditorGUILayout.BeginHorizontal();
                            {
                                var rect = GUILayoutUtility.GetRect(128f, 128f);
                                AngleDrawer.DrawBackGround(rect);
                                AngleDrawer.FloatAngle(rect, ref cb.AngleList[i].StartAngle, ref cb.AngleList[i].EndAngle, 10f, 0f, 360f, Vector2.up, cb.AngleList[i].Angle);
                                //Color StartColor = Color.white;
                                EditorGUILayout.BeginVertical();
                                {
                                    EditorGUILayout.PropertyField(A.FindPropertyRelative(nameof(AngleInformation.StartAngle)), true);
                                    //cb.AngleList[i].StartAngle = EditorGUILayout.FloatField(cb.AngleList[i].StartAngle, GUILayout.MinWidth(60f), GUILayout.MaxWidth(60f));
                                    EditorGUILayout.PropertyField(A.FindPropertyRelative(nameof(AngleInformation.EndAngle)), true);
                                    EditorGUILayout.PropertyField(A.FindPropertyRelative(nameof(AngleInformation.Angle)), true);
                                    EditorGUILayout.BeginHorizontal();
                                    if (GUILayout.Button("Invert")) AngleFunctions.InvertedAngle(ref cb.AngleList[i].StartAngle, ref cb.AngleList[i].EndAngle);
                                    if (GUILayout.Button("TestAngle")) AngleFunctions.TestAngleBool(cb.AngleList[i], cb.AngleList[i].Angle);
                                    if (GUILayout.Button("Opposite")) AngleFunctions.OppositeAngle(ref cb.AngleList[i].StartAngle, ref cb.AngleList[i].EndAngle, 180f);
                                    if (GUILayout.Button("-", GUILayout.MaxWidth(20)))
                                    {
                                        serializedProperty.DeleteArrayElementAtIndex(i);
                                        //EditorUtility.SetDirty(cb);
                                        //return;
                                    }
                                    EditorGUILayout.EndHorizontal();

                                    if (EditorGUI.EndChangeCheck())
                                    {
                                        /*
                                        for (int a = 0; a < cb.AngleList.Length; a++)
                                        {
                                            cb.AngleList[a].Angle = CalculateAngle(cb.AngleList[a].StartAngle, cb.AngleList[a].EndAngle);
                                        }*/
                                        serializedObject.ApplyModifiedProperties();
                                        EditorUtility.SetDirty(cb);
                                    }
                                }
                                EditorGUILayout.EndVertical();
                            }
                            EditorGUILayout.EndHorizontal();
                        }
                        i++;
                    }
                    EditorGUILayout.EndVertical();
                }

                EditorGUI.indentLevel--;
            }

            if (EditorGUI.EndChangeCheck())
            {
                /*
                for (int a = 0; a < cb.AngleList.Length; a++)
                {
                    cb.AngleList[a].Angle = CalculateAngle(cb.AngleList[a].StartAngle, cb.AngleList[a].EndAngle);
                }
                */
               //serializedObject.ApplyModifiedProperties();
               EditorUtility.SetDirty(cb);
            }
        }

        private float CalculateAngle(float to, float from)
        {
            return Mathf.Abs(from - to) % 360f;
        }

        /*
        public static void RemoveAt<T>(ref T[] arr, int index)
        {
            for (int a = index; a < arr.Length - 1; a++)
            {
                // moving elements downwards, to fill the gap at [index]
                arr[a] = arr[a + 1] == null ? arr[a + 2] :  arr[a + 1];
            }
            // finally, let's decrement Array's size by one
            Array.Resize(ref arr, arr.Length - 1);
        }*/

        public static void AddItemToArray<T>(ref T[] arr)
        {
            int oldCount = (arr != null) ? arr.Length : 0;

            T[] finalArray = new T[oldCount + 1];

            for (int i = 0; i < oldCount; i++)
            {
                finalArray[i] = arr[i];
            }
           // finalArray[finalArray.Length - 1] = itemToAdd;

            arr = finalArray;
        }

        private void DrawPropertyDrawers(SerializedProperty A, int i)
        {
            EditorGUILayout.BeginHorizontal();
            {
                var rect = GUILayoutUtility.GetRect(64f, 64f);
                AngleDrawer.DrawBackGround(rect);
                //AngleDrawer.FloatAngle(rect, ref cb.AngleList[i].StartAngle, ref cb.AngleList[i].EndAngle, 10f, 0f, 360f, Vector2.up, cb.AngleList[i].StartColor, cb.AngleList[i].EndColor, cb.AngleList[i].FillColor);

                EditorGUILayout.BeginVertical();
                {
                    EditorGUILayout.PropertyField(A.FindPropertyRelative(nameof(AngleInformation.StartAngle)), true);
                    EditorGUILayout.PropertyField(A.FindPropertyRelative(nameof(AngleInformation.EndAngle)), true);
                    EditorGUILayout.PropertyField(A.FindPropertyRelative(nameof(AngleInformation.Angle)), true);
                    EditorGUILayout.BeginHorizontal();
                    if (GUILayout.Button("Invert")) AngleFunctions.InvertedAngle(ref cb.AngleList[i].StartAngle, ref cb.AngleList[i].EndAngle);
                    if (GUILayout.Button("Opposite")) AngleFunctions.OppositeAngle(ref cb.AngleList[i].StartAngle, ref cb.AngleList[i].EndAngle, 180f);
                    if (GUILayout.Button("-", GUILayout.MaxWidth(20)))
                    {
                        serializedProperty.DeleteArrayElementAtIndex(i);
                        //EditorUtility.SetDirty(cb);
                        //return;
                    }
                    EditorGUILayout.EndHorizontal();
                }
                EditorGUILayout.EndVertical();
            }
            EditorGUILayout.EndHorizontal();
        }
    }
}
