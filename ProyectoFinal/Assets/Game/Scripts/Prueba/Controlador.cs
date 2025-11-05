using UnityEngine;

public class Controlador : MonoBehaviour
{
    public float HorizontalMove;
    public float VerticalMove;
    private Vector3 playerInput;

    public CharacterController Player;

    public float Speed;
    private Vector3 MovePlayer;
    public float gravity = 9.8f;
    public float Caida;

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
        MovePlayer = MovePlayer * Speed;

        Player.transform.LookAt(Player.transform.position + MovePlayer);

        setGravity();

        Player.Move(MovePlayer * Time.deltaTime);
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
    void setGravity()
    {
        if (Player.isGrounded)
        {
            Caida = -gravity * Time.deltaTime;
            MovePlayer.y = Caida;
        }
        else
        {
            Caida -= gravity * Time.deltaTime;
            MovePlayer.y = Caida;
        }
    }
}

