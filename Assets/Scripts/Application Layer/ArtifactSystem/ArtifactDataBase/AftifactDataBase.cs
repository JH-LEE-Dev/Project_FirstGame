using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ArtifactDataBase", menuName = "Game/Artifact DataBase")]
public class ArtifactDataBase : ScriptableObject
{
    public List<ArtifactData> artifactDatas;

    public ArtifactData GetArtifactData(int id)
    {
        return artifactDatas[id];
    }
}