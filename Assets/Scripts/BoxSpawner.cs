using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class BoxSpawner : NetworkBehaviour
{
    [SerializeField] private MeshRenderer meshRenderer;
    private static Color32 defaultColor = new Color32(255, 255, 255, 255);
    private Box boxPrefab;
    private Box currentBox;
    private List<Box> boxes = new List<Box>();
    [SerializeField] private bool isCanSpawn;
    void Start()
    {
        meshRenderer.materials[1].SetColor("_EmissionColor", defaultColor);
        boxPrefab = PrefabsManager.LoadPrefab<Box>();
        
        if (!NetworkManager.Singleton.IsHost)
            return;
        NetworkObject no = GetComponent<NetworkObject>();
        if (!no.IsSpawned)
        {
            no.Spawn();
        }
        
        if (boxes.Count <= GridBuilder.maxBoxes && isCanSpawn)
        {
            StartCoroutine("DoCheck");
        }
    }
    IEnumerator DoCheck()
    {
        yield return new WaitForSeconds(1f);
        currentBox = GameObject.Instantiate(boxPrefab, transform);
        NetworkObject no = currentBox.GetComponent<NetworkObject>();
        if (no != null)
        {
            no.Spawn();
        }
        boxes.Add(currentBox);
    }
    void Update()
    {

    }

    void FixedUpdate()
    {
        if (!NetworkManager.Singleton.IsHost)
            return;
        if (currentBox != null)
        {
            if (currentBox.Picked)
            {
                currentBox = null;
                if (boxes.Count <= GridBuilder.maxBoxes && isCanSpawn)
                {
                    StartCoroutine("DoCheck");
                }
                else
                {
                    Disable();
                    DisableClientRpc();
                }
            }
            else
            {
                Vector3 newPos = Vector3.Lerp(currentBox.transform.position, transform.position + currentBox.transform.up * 2f, Time.deltaTime * 15);
                currentBox.transform.position = (newPos);
                currentBox.transform.rotation = (transform.rotation);
            }
        }
    }

    public void Disable()
    {
        isCanSpawn = false;
    }

    [ClientRpc]
    public void DisableClientRpc()
    {
        isCanSpawn = false;
    }
}
