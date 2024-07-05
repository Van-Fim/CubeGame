using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Box : NetworkBehaviour
{
    public static Color32 defaultColor = new Color32(255, 255, 255, 11);
    public MeshRenderer meshRenderer;
    Rigidbody objRigidbody;
    private bool picked;

    public bool Picked
    {
        get => picked; set
        {
            picked = value;
        }
    }

    public void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        objRigidbody = GetComponent<Rigidbody>();
    }
    [ServerRpc(RequireOwnership = false)]
    public void PickUpServerRpc(ulong id)
    {
        objRigidbody.useGravity = false;
        Picked = true;

        for (int i = 0; i < NetworkManager.ConnectedClientsList.Count; i++)
        {
            NetworkClient client = NetworkManager.ConnectedClientsList[i];
            if (client.ClientId == id)
            {
                PlayerController fm = client.PlayerObject.GetComponent<PlayerController>();
                fm.pickedObj = this;
                break;
            }
        }
    }
    [ServerRpc(RequireOwnership = false)]
    public void DropServerRpc()
    {
        Picked = false;
        objRigidbody.useGravity = true;
        transform.SetParent(null);
    }
}
