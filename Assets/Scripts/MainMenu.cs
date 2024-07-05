using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private Button hostButton;
    [SerializeField] private Button clientButton;
    [SerializeField] private Camera camera;
    void Awake()
    {
        hostButton.onClick.AddListener(() =>
        {
            SceneManager.LoadScene("Level01", LoadSceneMode.Additive);
            NetworkManager.Singleton.StartHost();
            gameObject.SetActive(false);
            camera.gameObject.SetActive(false);
        });
        clientButton.onClick.AddListener(() =>
        {
            NetworkManager.Singleton.StartClient();
            gameObject.SetActive(false);
            camera.gameObject.SetActive(false);
        });
    }
}
