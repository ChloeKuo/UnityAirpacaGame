using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]

public class GroundControl : MonoBehaviour
{
    Rigidbody2D body;
    GameManager game;
    AirpacaControl playerscript;
    private SpriteRenderer ground;

    public Vector3 defaultPos;

    // Start is called before the first frame update
    void Start()
    {
        game = GameManager.Instance;
        body = GetComponent<Rigidbody2D>();
        ground = GetComponent<SpriteRenderer>();
        body.simulated = false;
        GameObject Airpaca = GameObject.Find("Airpaca");
        playerscript = Airpaca.GetComponent<AirpacaControl>();
        defaultPos.x = 0;
        defaultPos.y = playerscript.defaultPosition.y - ground.sprite.rect.height/2;
        defaultPos.z = 0;
        Debug.Log(ground.sprite.rect.height);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Drop()
    {
        transform.position = defaultPos;
        //body.simulated = true; //when play button or replay button hit
    }
}
