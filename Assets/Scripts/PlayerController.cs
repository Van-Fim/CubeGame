using Unity.Netcode;
using UnityEngine;

public class PlayerController : NetworkBehaviour
{
    private Vector3 Velocity;
    private Vector3 PlayerMovementInput;
    private Vector2 PlayerMouseInput;
    private bool Sneaking = false;
    private float xRotation;
    public Box pickedObj;

    public static PlayerController instance;
    public static NetworkVariable<int> seed = new NetworkVariable<int>();

    [Header("Components Needed")]
    [SerializeField] public Transform PlayerCamera;
    [SerializeField] private CharacterController Controller;
    [SerializeField] private Transform Player;
    [Space]
    [Header("Movement")]
    [SerializeField] private float Speed;
    [SerializeField] private float JumpForce;
    [SerializeField] private float Sensetivity;
    [SerializeField] private float Gravity = 9.81f;
    [Space]
    [Header("Sneaking")]
    [SerializeField] private bool Sneak = false;
    [SerializeField] private float SneakSpeed;

    void Start()
    {
        if (NetworkManager.Singleton.IsHost)
        {
            if(seed.Value == 0)
                seed.Value = Random.Range(1, 100000);
        }
        if (!IsOwner)
        {
            PlayerCamera.gameObject.SetActive(false);
            return;
        }
        
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        if (!IsOwner)
            return;
        PlayerMovementInput = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical"));
        PlayerMouseInput = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));

        MovePlayer();
        MoveCamera();

        if (Input.GetKey(KeyCode.RightShift) && Sneak)
        {
            Player.localScale = new Vector3(1f, 0.5f, 1f);
            Sneaking = true;
        }
        if (Input.GetKeyUp(KeyCode.RightShift))
        {
            Player.localScale = new Vector3(1f, 1f, 1f);
            Sneaking = false;
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (pickedObj == null)
            {
                float pickUpDistance = 10;
                if (Physics.Raycast(PlayerCamera.transform.position, PlayerCamera.transform.forward, out RaycastHit raycastHit, pickUpDistance, 1 << 6))
                {
                    if (raycastHit.transform.TryGetComponent<Box>(out pickedObj))
                    {
                        pickedObj.PickUpServerRpc(NetworkManager.LocalClientId);
                    }
                }
            }
            else
            {
                pickedObj.DropServerRpc();
                pickedObj = null;
            }
        }
    }
    private void MovePlayer()
    {
        Vector3 MoveVector = transform.TransformDirection(PlayerMovementInput);


        if (Controller.isGrounded)
        {
            Velocity.y = -1f;

            if (Input.GetKeyDown(KeyCode.Space) && Sneaking == false)
            {
                Velocity.y = JumpForce;
            }
        }
        else
        {
            Velocity.y += Gravity * -2f * Time.deltaTime;
        }
        if (Sneaking)
        {
            Controller.Move(MoveVector * SneakSpeed * Time.deltaTime);
        }
        else
        {
            Controller.Move(MoveVector * Speed * Time.deltaTime);
        }
        Controller.Move(Velocity * Time.deltaTime);

    }
    private void MoveCamera()
    {
        xRotation -= PlayerMouseInput.y * Sensetivity;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        transform.Rotate(0f, PlayerMouseInput.x * Sensetivity, 0f);
        PlayerCamera.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
    }

    private void FixedUpdate()
    {
        if (pickedObj != null && IsServer)
        {
            if (pickedObj.Picked)
            {
                Vector3 newPos = Vector3.Lerp(pickedObj.transform.position, PlayerCamera.position + PlayerCamera.forward * 2, Time.deltaTime * 500);
                pickedObj.transform.position = (PlayerCamera.position + PlayerCamera.forward * 2);
                pickedObj.transform.rotation = (PlayerCamera.rotation);
            }
            else
            {
                pickedObj = null;
            }
        }
    }
}
