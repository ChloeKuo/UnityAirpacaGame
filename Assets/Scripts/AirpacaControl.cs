using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class AirpacaControl : MonoBehaviour
{
    public delegate void PlayerDelegate();
    public static event PlayerDelegate OnPlayerDied; // Player touches BombBerry
    public static event PlayerDelegate OnPlayerScored; // Player touches AppleBerry
    public static event PlayerDelegate OnPlayerSpit;
    public static event PlayerDelegate OnPlayerHitBottom; // Player goes below bottom of screen

    public Vector3 pz; // position of mouse click

    Rigidbody2D body;
    public Vector3 clampedPos;
    public float tapForce;
    public int degrees; // degrees for FallSpin rotation
    public Vector3 defaultPosition;

    private SpriteRenderer player;
    private float height;

    GameManager game;
    BlockControl blockScript;

    void TapJump()
    {
        if (Input.GetMouseButtonDown(0) && pz.y >= transform.position.y) // 0 indicates left click, 1 indicates right click; GetMouseButtonDown translates as tap notification on mobile
        {
            float sqrOriginDis = Mathf.Pow(pz.x, 2) + Mathf.Pow(pz.y, 2); // square of the distance from origin to mouse at click  
            float hPush = (Mathf.Sqrt(1 - Mathf.Pow(pz.y, 2) / sqrOriginDis));
            float vPush = (Mathf.Sqrt(1 - Mathf.Pow(pz.x, 2) / sqrOriginDis));
            body.velocity = Vector3.zero;
            if (pz.x < transform.position.x)
            {
                hPush = -hPush;
            }


            //{
            //    vPush = -vPush;
            //}
            // I want the tap/mouse click below the llama (currently the ship) to cause the llama to spit
            body.AddForce(new Vector2(hPush, vPush) * tapForce, ForceMode2D.Force);
        }
        clampedPos = transform.position;
        clampedPos.x = Mathf.Clamp(clampedPos.x, -blockScript.topRightCoor.x + height * 4, blockScript.topRightCoor.x - height * 4);
        transform.position = clampedPos;
    }

    void Spit()
    {
        if (Input.GetMouseButtonDown(0) && pz.y < transform.position.y)
        {
            OnPlayerSpit();
        }
    }

    void FallSpin()
    {
        transform.Rotate(Vector3.forward *degrees);
    }

    void OnEnable()
    {
        CountdownText.OnCountdownFinished += MoveAllow;
    }

    void OnDisable()
    {
        CountdownText.OnCountdownFinished -= MoveAllow;
    }

    // Start is called before the first frame update
    void Start()
    {
        body = GetComponent<Rigidbody2D>();
        player = GetComponent<SpriteRenderer>();
        game = GameManager.Instance;
        body.simulated = false;
        GameObject Blocks = GameObject.Find("Blocks");
        blockScript = Blocks.GetComponent<BlockControl>();
        height = blockScript.blockHeight / 4;
        Vector3 scale = new Vector3(height, height, 0);
        transform.localScale = scale;
        player.sortingLayerName = "Game";
        player.sortingOrder = -1;
    }

    // Update is called once per frame
    void Update()
    {
        if (game.InitGameOver)
        {
            FallSpin();
            if (transform.position.y < -blockScript.topRightCoor.y)
            {
                OnPlayerHitBottom();
            }
        }
        pz = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        pz.z = 0;
        if (game.InGame == true)
        {
            body.simulated = true;
            TapJump();
            Spit();
            if (transform.position.y < -blockScript.topRightCoor.y)
            {
                OnPlayerHitBottom();
            }

        }


    }




    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.tag == "AppleBerry" && game.InGame)
        {
            Destroy(col.gameObject);
            OnPlayerScored(); // event sent to GameManager
            blockScript.IncreaseSpeed();
            //play a sound
        }

        if (col.gameObject.tag == "BombBerry" && game.InGame)
        {
            //body.simulated = false;
            Destroy(col.gameObject);
            OnPlayerDied(); //
            //play a sound


        }
    }

    public void Reset() //when replay button is clicked
    {
        transform.position = defaultPosition;
        body.velocity = Vector3.zero;
        transform.localRotation = Quaternion.identity;
        body.simulated = false;
        player.sortingOrder = 10;
    }

    void MoveAllow()
    {
        body.simulated = true;
    }
}
