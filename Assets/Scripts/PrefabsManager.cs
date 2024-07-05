using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PrefabsManager : MonoBehaviour
{
    public static List<string> list;
    public static Object[] prefabs;

    public static void Init()
    {
        prefabs = Resources.LoadAll("Prefabs");
    }
    
    public static T LoadPrefab<T>(string name = null)
    {
        for (int i = 0; i < prefabs.Length; i++)
        {
            GameObject gm = prefabs[i] as GameObject;
            if (gm.GetComponent<T>() != null && name != null && gm.gameObject.name == name)
            {
                return gm.GetComponent<T>();
            }
            else if (gm.GetComponent<T>() != null && name == null)
            {
                return gm.GetComponent<T>();
            }
        }
        return default(T);
    }
}
