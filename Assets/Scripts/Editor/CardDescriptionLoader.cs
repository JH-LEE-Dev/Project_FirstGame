using UnityEngine;
using UnityEditor; // 이게 핵심입니다. 에디터 전용 기능
using System.IO;   // 파일 입출력
using System;
using System.Collections.Generic;

[Serializable]
public class RawCardData
{
    public int id;
    public int nameId;
    public int descriptionId;
}

[Serializable]
public class RawCardList
{
    public List<RawCardData> cards;
}

public class CardImporter : EditorWindow
{
    [MenuItem("Tools/Card Data/Import from JSON")]
    public static void ImportJsonData()
    {
        // 1. JSON 파일 경로
        string jsonPath = Path.Combine(Application.dataPath, "StreamingAssets/CardDescription.json");

        if (!File.Exists(jsonPath))
        {
            Debug.LogError("JSON 파일을 찾을 수 없습니다: " + jsonPath);
            return;
        }

        // 2. 파일 읽기 & 파싱
        string jsonString = File.ReadAllText(jsonPath);
        RawCardList rawData = JsonUtility.FromJson<RawCardList>(jsonString);

        string assetPath = "Assets/Scripts/Application Layer/Scriptable Objects/Card/CardDataBase.asset";

        // [핵심 1] 이미 존재하는 에셋인지 확인하고 불러오기
        CardDataBase asset = AssetDatabase.LoadAssetAtPath<CardDataBase>(assetPath);

        if (asset == null)
        {
            Debug.LogWarning("CardDataBase가 없습니다.");
            return;
        }

        int cnt = 0;
        foreach (var rawCard in rawData.cards)
        {
            if (cnt >= asset.cardData.Count)
                return;

            asset.cardData[cnt].cardNameId = rawCard.nameId;
            asset.cardData[cnt].cardDescriptionId = rawCard.descriptionId;

            ++cnt;
        }

        // 4. 변경사항 디스크에 쓰기
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log("CardDataBase Text Data Cooking Completed");
    }
}