using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class SpitControl : MonoBehaviour
{
    Camera cam;
    BlockControl blockScript;


    public delegate void SpitDelegate();
    //public static event SpitDelegate OnShoot;

    public GameObject Player;

    GameManager game;

    private SpriteRenderer sprite;
    //public bool playerDead = false;
    public bool isSpit;
    public float moveSpeed;

    public int sortingOrder = 0;

    Rigidbody2D spitBody;

    void OnEnable()
    {
        AirpacaControl.OnPlayerSpit += SpitAllow;
    }

    void SpitAllow()
    {
        isSpit = true;
    }

    void SpitMove()
    {
        //SortingLayer Game
        sprite.sortingOrder = 11;
        transform.position += Vector3.up * moveSpeed * Time.deltaTime;
    }


    // Start is called before the first frame update
    void Start()
    {
        spitBody = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
        cam = Camera.main;
        game = GameManager.Instance;
        transform.localPosition = new Vector3(Player.transform.position.x, Player.transform.position.y, 0);
        isSpit = false;
        //Vector2 BlockControl.topRightCoor = cam.ScreenToWorldPoint(new Vector2(cam.pixelWidth, cam.pixelHeight));
        //GameObject thePlayer = GameObject.Find("Airpaca");
        //AirpacaControl playerScript = thePlayer.GetComponent<AirpacaControl>();
        GameObject Blocks = GameObject.Find("Blocks");
        blockScript = Blocks.GetComponent<BlockControl>();

    }

    // Update is called once per frame
    void Update()
    {
        if (game.InGame == false)
        {
            sprite.sortingOrder = 0;
            transform.localPosition = new Vector3(Player.transform.position.x, Player.transform.position.y, 0);
            isSpit = false;
        }
        else if (game.InGame)
        {
            if (isSpit == true && transform.position.y < blockScript.topRightCoor.y)
            {
                SpitMove();
            }
            else
            {
                isSpit = false;
                sprite.sortingOrder = 0;
                transform.localPosition = new Vector3(Player.transform.position.x, Player.transform.position.y, 0);
            }
        }

    }
    //Vector3 pz = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    //pz.z = 0;
    //if (Input.GetMouseButtonDown(0) && pz.y <= transform.position.y) // 0 indicates left click, 1 indicates right click; GetMouseButtonDown translates as tap notification on mobile
    //{

    //}

    //void OnCollisionEnter2D(Collision2D col)
    //{
    //    if (col.gameObject.tag == "DeadZone")
    //    {
    //        Destroy(col.gameObject);
    //        Debug.Log("hit");
    //    }
    //}

    void OnTriggerEnter2D(Collider2D col)
    {
        if (game.InitGameOver == false)
        {
            if (col.gameObject.tag == "AppleBerry" || col.gameObject.tag == "BombBerry")
            {
                Destroy(col.gameObject);
            }
        }

    }
}
