using UnityEngine;
using Unity.Netcode;

public class GameManager : MonoBehaviour
{
    void Awake()
    {
        Application.targetFrameRate = 60;
        PrefabsManager.Init();
    }
}
