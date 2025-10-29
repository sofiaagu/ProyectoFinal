using UnityEngine;

public class Controlador : MonoBehaviour
{
    public float HorizontalMove;
    public float VerticalMove;
    private Vector3 playerInput;

    public CharacterController Player;

    public float Speed;
    private Vector3 MovePlayer;

    public Camera MainCam;
    public Vector3 CameraAdelante;
    public Vector3 CameraRight;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Player = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        HorizontalMove = Input.GetAxis("Horizontal");
        VerticalMove = Input.GetAxis("Vertical");

        playerInput = new Vector3(HorizontalMove, 0, VerticalMove);
        playerInput = Vector3.ClampMagnitude(playerInput, 1);

        DireccionCamara();
            MovePlayer = playerInput.x * CameraRight + playerInput.z * CameraAdelante;

        Player.transform.LookAt(Player.transform.position + MovePlayer);

        Player.Move(MovePlayer * Speed * Time.deltaTime);
        Debug.Log(Player.velocity.magnitude);   
    }

    void DireccionCamara()
    {
       CameraAdelante = MainCam.transform.forward;
       CameraRight = MainCam.transform.right;

        CameraAdelante.y = 0;
        CameraRight.y = 0;

        CameraAdelante = CameraAdelante.normalized;
        CameraRight = CameraRight.normalized;
    }
}

