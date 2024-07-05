using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Unity.Netcode;
[System.Serializable]
public class AdvButtonEvent : UnityEvent
{
}
public class AdvButton : MonoBehaviour
{
    public int id;
    public MeshRenderer meshRenderer;
    public GridBuilder gridBuilder;
    public bool is_required;
    private bool is_ok;
    private static Color32 goldColor = new Color32(255, 190, 0, 255);
    private static Color32 defaultColor = new Color32(255, 0, 0, 255);
    private static Color32 rightColor = new Color32(0, 255, 0, 255);
    private Box currentBox;


    public static AdvButtonEvent OnCheckButtonAction = new AdvButtonEvent();

    void Start()
    {
        OnCheckButtonAction.AddListener(OnCheckButton);
    }
    void OnCheckButton()
    {
        Fix();
    }
    public void Fix()
    {
        if (gridBuilder.gridType != GridBuilder.GridType.buttons)
        return;
        if (currentBox != null)
        {
            if (!currentBox.Picked)
            {
                if (is_required)
                {
                    meshRenderer.materials[1].SetColor("_EmissionColor", rightColor);
                    currentBox.meshRenderer.material.SetColor("_EmissionColor", rightColor);
                    is_ok = true;
                }
                else
                {
                    meshRenderer.materials[1].SetColor("_EmissionColor", defaultColor);
                    currentBox.meshRenderer.material.SetColor("_EmissionColor", defaultColor);
                    is_ok = false;
                }
                List<AdvButton> advs = gridBuilder.buttons.FindAll(x => x.is_required && !x.is_ok);
                if (advs.Count == 0)
                {
                    meshRenderer.materials[1].SetColor("_EmissionColor", goldColor);
                    currentBox.meshRenderer.material.SetColor("_EmissionColor", goldColor);
                }
                Vector3 newPos = Vector3.Lerp(currentBox.transform.position, transform.position + currentBox.transform.up * 0.75f, Time.deltaTime * 15);
                currentBox.transform.position = (newPos);
                currentBox.transform.rotation = (transform.rotation);
            }
        }
        else
        {
            meshRenderer.materials[1].SetColor("_EmissionColor", defaultColor);
            is_ok = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Box box = other.GetComponent<Box>();

        if (box != null && currentBox == null)
        {
            currentBox = box;
            gridBuilder.FixButton(this);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        Box box = other.GetComponent<Box>();
        if (box != null && currentBox == box)
        {
            currentBox.meshRenderer.material.SetColor("_EmissionColor", Box.defaultColor);
            currentBox = null;
        }
    }
}
