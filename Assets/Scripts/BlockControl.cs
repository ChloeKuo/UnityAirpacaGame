using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockControl : MonoBehaviour
{
    public Camera cam;
    public float blockHeight;
    public float bombHeight;
    public float cloudHeight;
    public float rowSpace;
    public Vector2 topRightCoor;
    private int pixScreenWidth;
    private int pixScreenHeight;
    GameManager game;

    class Blocks // keeps track of all of objects of a certain type
    {
        public Transform t;
        public bool inUse;
        public bool blockExist;
        public GameObject go; //Maybe use this to assign certain sprite to each Blocks object?
        public Collider2D blockCollider;


        public Blocks(GameObject Prefab, float xInit, float yInit, float sideLength) // constructor
        {
            go = Instantiate(Prefab, new Vector3(xInit, yInit, 0), Quaternion.identity) as GameObject;
            t = go.transform;
            Vector3 scale = new Vector3(sideLength, sideLength, 0);
            t.localScale = scale;
            blockExist = true;
        }

        public Blocks()
        {
            blockExist = false;
        }

        public void Use()
        {
            inUse = true;
        }

        public void Dispose()
        {
            inUse = false;
        }
    }

    private int numRow;
    private int numCol;
    public float defSpeed;
    private float shiftSpeed;
    public float shrinkSpeed = 0.1f;
    public Vector3 targetScale;


    public GameObject cloudPrefab;
    public GameObject bombPrefab;
    public GameObject berryPrefab;
    public GameObject Target;
    public Vector2 targetAspectRatio;
    int[] blockGroup;
    //Blocks[] blockPool;
    Blocks[] currentPool;
    List<Blocks[]> blockContainer = new List<Blocks[]>();
    List<GameObject> goContainer = new List<GameObject>();

    float targetAspect;

    void OnEnable()
    {
        //AirpacaControl.OnPlayerDied += PlayerNowDead;
        //CountdownText.OnCountdownFinished += EmptyContainer;
    }

    void OnDisable()
    {
        //CountdownText.OnCountdownFinished -= EmptyContainer;
    }

    void Awake()
    {
        cam = Camera.main;
        pixScreenWidth = cam.pixelWidth;
        pixScreenHeight = cam.pixelHeight;
        targetAspect = targetAspectRatio.x / targetAspectRatio.y;
        topRightCoor = cam.ScreenToWorldPoint(new Vector2(cam.pixelWidth, cam.pixelHeight));// * Camera.main.aspect / targetAspect;
        if (topRightCoor.x < topRightCoor.y) // for portrait mode
        {
            blockHeight = topRightCoor.x / 10;
            bombHeight = topRightCoor.x / 20;
            cloudHeight = topRightCoor.x / 15;
        }
        else // for landscape mode, this needs work though
        {
            blockHeight = topRightCoor.y / 10;
            bombHeight = topRightCoor.y / 20;
            cloudHeight = topRightCoor.y / 15;
        }
        //blockHeight = topRightCoor.x / 10;
        //bombHeight = topRightCoor.x / 20;
        //cloudHeight = topRightCoor.x / 15;
        //rowSpace = topRightCoor.x;
        numRow = (int)(topRightCoor.y / blockHeight);
        numCol = (int)(topRightCoor.x / blockHeight);
        Configure(BlockSetUp(), topRightCoor.x, topRightCoor.y, cloudHeight, bombHeight);
        targetScale = new Vector3(0.01f, 0.01f, 0);
        ResetShiftSpeed();
    }

    // Start is called before the first frame update
    void Start()
    {
        game = GameManager.Instance;
        //GameObject Airpaca = GameObject.Find("Airpaca");
        //AirpacaScript = Airpaca.GetComponent<AirpacaControl>();

    }


    // Update is called once per frame
    void Update()
    {
        if (game.FinalGameOver)
        {
            RemoveAll();
        }
        else if (game.InGame == true)
        {
            if (currentPool[0].t.position.y < topRightCoor.y - blockHeight)
            {
                Configure(BlockSetUp(), topRightCoor.x, topRightCoor.y, cloudHeight, bombHeight); //creates new block row
            }
            Shift(); // moves blocks downards
        }
        //Configure(BlockSetUp(), topRightCoor.x, topRightCoor.y, blockHeight);


    }



    int[] BlockSetUp() // sets up 2D array of 0s and 1s
    {
        blockGroup = new int[numCol];
        for (int col = 0; col < numCol; col++)
        {
            if (col == 0)
            {
                blockGroup[col] = 1;
            }
            else
            {
                blockGroup[col] = Random.Range(0, 5);
            }

        }
        return blockGroup;
    }

    void Configure(int[] group, float xPos, float yPos, float cloudHeight, float bombHeight)
    {
        //targetAspect = targetAspectRatio.x / targetAspectRatio.y;
        float xInitPos = xPos + blockHeight * 2;
        float yInitPos = yPos;
        //targetAspect = targetAspectRatio.x / targetAspectRatio.y;
        Blocks[] blockPool = new Blocks[numCol];
        for (int col = 0; col < numCol; col++)
        {
            if (group[col] == 1 || group[col] == 2) // cloud go
            {
                blockPool[col] = new Blocks(cloudPrefab, xInitPos - col * blockHeight * 3, yInitPos + blockHeight * 2, cloudHeight);
                //goContainer.Add(go);
            }
            else if (group[col] == 3) //bomb go
            {
                blockPool[col] = new Blocks(bombPrefab, xInitPos - col * blockHeight * 3, yInitPos + blockHeight * 2, bombHeight);
                //goContainer.Add(go);
            }
            else if (group[col] == 4) //berry go
            {
                blockPool[col] = new Blocks(berryPrefab, xInitPos - col * blockHeight * 3, yInitPos + blockHeight * 2, bombHeight);
                //goContainer.Add(go);
            }
            else
            {
                blockPool[col] = new Blocks();
            }
        }
        currentPool = blockPool;
        blockContainer.Add(blockPool);

    }


    void RemoveAll() // Shrinks blocks, then deletes them
    {
        for (int row = 0; row < blockContainer.Count; row++)
        {
            Blocks[] currRow = blockContainer[row];
            for (int col = 0; col < numCol; col++)
            {
                if (currRow[col].go == null)
                {
                    currRow[col].blockExist = false;

                }
                if (currRow[col].blockExist == true)
                {
                    if (currRow[col].t.localScale.x >= targetScale.x)
                    {
                        currRow[col].t.localScale -= new Vector3(1, 1, 0) * Time.deltaTime * 0.1f;
                    }
                    else
                    {
                        Destroy(currRow[col].go);
                        //blockContainer.Remove(currRow);
                    }

                }
            }
        }
    }

    void Shift()
    {
        for (int row = 0; row < blockContainer.Count; row++)
        {
            Blocks[] currRow = blockContainer[row];
            for (int col = 0; col < numCol; col++)
            {
                if (currRow[col].go == null)
                {
                    currRow[col].blockExist = false;
                }
                if (currRow[col].blockExist == true)
                {
                    currRow[col].t.position += Vector3.down * shiftSpeed * Time.deltaTime;
                    CheckDisposeBlock(currRow[col]);
                }
            }
        }
    }

    public void IncreaseSpeed() //increase speed of blocks falling every 5 berries collected
    {
        shiftSpeed += 0.05f;
    }


    void CheckDisposeBlock(Blocks row) // maybe use colliders to destroy GOs?
    {

        if (row.t.position.y < -topRightCoor.y)
        {
            DisposeBlock(row);
        }
    }

    void DisposeBlock(Blocks row)
    {
        Destroy(row.go);
        //OnTriggerEnter2D();
        row.blockExist = false;
    }

    public void ResetBlocks() // when game over, destroy all blocks and remove from block container, set shiftSpeed to defSpeed;
    {
        for (int row = 0; row < blockContainer.Count; row++)
        {
            Blocks[] currRow = blockContainer[row];
            for (int col = 0; col < numCol; col++)
            {
                if (currRow[col].go == null)
                {
                    currRow[col].blockExist = false;
                }
                if (currRow[col].blockExist == true)
                {
                    DisposeBlock(currRow[col]);
                }
            }
        }
        blockContainer.Clear();
        Configure(BlockSetUp(), topRightCoor.x, topRightCoor.y, cloudHeight, bombHeight);
        ResetShiftSpeed();
    }

    void ResetShiftSpeed()
    {
        shiftSpeed = defSpeed;
    }


    //void OnTriggerEnter2D(Collider2D col)
    //{
    //    if (col.gameObject.tag == "Spit")
    //    {
    //        Destroy(col.gameObject);
    //    }
    //}

}
