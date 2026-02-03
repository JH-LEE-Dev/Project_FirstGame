#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

public class CheatTools : EditorWindow
{
    string inputCardId = "";
    int goldAmount = 1000;
    bool isGodMode = false;
    GameObject specificUnit;

    [MenuItem("Tools/Cheats")]
    public static void ShowWindow()
    {
        GetWindow<CheatTools>("Cheat Tools");
    }

    void OnGUI()
    {
        GUILayout.Label("게임 치트 관리자", EditorStyles.boldLabel);

        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

        EditorGUILayout.HelpBox("게임 실행 중에만 작동하는 기능들입니다.", MessageType.Info);
        EditorGUILayout.Space();

        GUILayout.Label("자원 관리", EditorStyles.label);

        inputCardId = EditorGUILayout.TextField("생성할 카드 ID", inputCardId);

        if (GUILayout.Button($"카드 생성 ({inputCardId})"))
        {
            Debug.Log($"치트 실행: {inputCardId} 카드를 생성합니다.");
            

        }

        EditorGUILayout.Space();

        EditorGUILayout.BeginHorizontal();
        {
            goldAmount = EditorGUILayout.IntField("골드 수량", goldAmount);
            if (GUILayout.Button("골드 지급", GUILayout.Width(100)))
            {
                Debug.Log($"치트 실행: {goldAmount} 골드 획득");
            }
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space();

        isGodMode = EditorGUILayout.Toggle("무적 모드 (God Mode)", isGodMode);
        if (GUI.changed)
        {
            Debug.Log($"무적 모드 상태 변경: {isGodMode}");
        }

        EditorGUILayout.Space();

        GUILayout.Label("특정 유닛 조작", EditorStyles.boldLabel);

        specificUnit = (GameObject)EditorGUILayout.ObjectField("대상 유닛", specificUnit, typeof(GameObject), true);

        if (GUILayout.Button("선택된 유닛 처치") && null != specificUnit)
        {
            
        }
    }
}
#endif