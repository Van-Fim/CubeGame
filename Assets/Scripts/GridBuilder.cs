using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

public class GridBuilder : MonoBehaviour
{
    public enum GridType { buttons, info };
    private Vector2 size = new Vector2(3, 3);
    private AdvButton advButtonPrefab;
    private Transform staticBoxPrefab;
    public List<AdvButton> buttons;
    public GridType gridType;
    public static int maxBoxes;

    void Start()
    {
        advButtonPrefab = PrefabsManager.LoadPrefab<AdvButton>();
        staticBoxPrefab = PrefabsManager.LoadPrefab<Transform>("StaticBox");
        Build();
    }
    void FixedUpdate()
    {
        AdvButton.OnCheckButtonAction.Invoke();
    }
    public void FixButton(AdvButton button)
    {
        button.Fix();
    }
    void Build()
    {
        UnityEngine.Random.InitState(PlayerController.seed.Value);
        int id = 0;
        for (int x = 0; x < size.x; x++)
        {
            for (int y = 0; y < size.y; y++)
            {
                AdvButton btn = GameObject.Instantiate<AdvButton>(advButtonPrefab, transform);
                btn.gridBuilder = this;
                btn.transform.localPosition = new Vector3(x * 2, 0, y * 2);
                btn.id = id;
                buttons.Add(btn);
                id++;
            }
        }

        for (int i = 0; i < buttons.Count; i++)
        {
            AdvButton btn = buttons[UnityEngine.Random.Range(0, buttons.Count)];
            
            if (gridType == GridType.info)
            {
                btn.meshRenderer.materials[1].SetColor("_EmissionColor", new Color32(0, 255, 0, 255));
                GameObject box = GameObject.Instantiate(staticBoxPrefab.gameObject, btn.transform);
                box.transform.localPosition = new Vector3(0, 0.75f, 0);
                box.GetComponent<MeshRenderer>().material.SetColor("_EmissionColor", new Color32(0, 255, 0, 255));
            }
            else
            {
                btn.is_required = true;
                maxBoxes = buttons.FindAll(x => x.is_required).Count - 1;
            }
        }

        
    }
}
