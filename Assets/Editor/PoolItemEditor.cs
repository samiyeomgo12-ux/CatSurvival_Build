using UnityEditor;

[CustomEditor(typeof(NewPoolItemData))]
public class PoolItemEditor : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        var typeProp = serializedObject.FindProperty("type");

        EditorGUILayout.PropertyField(typeProp);

        if((PoolObjType) typeProp.enumValueIndex == PoolObjType.Enemy)
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("enemyKey"));
        }
        else if((PoolObjType) typeProp.enumValueIndex == PoolObjType.Weapon)
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("weaponKey"));
        }
        else
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("itemkey"));
        }

        EditorGUILayout.PropertyField(serializedObject.FindProperty("initSize"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("maxSize"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("prefab"));

        serializedObject.ApplyModifiedProperties();
    }
}
