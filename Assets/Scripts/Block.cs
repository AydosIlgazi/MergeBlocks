using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour
{
    public int blockLevel { get; set; }
    public Material[] materials;
    private bool isBlockClicked= false;
    private Vector3 screenPoint;
    private Vector3 offset;
    private GameObject gameStatus;
    public float animationTime = 0.3f;
    public int blockScore = 0;
    Rigidbody rb;
    // Start is called before the first frame update
    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody>();
        gameStatus = GameObject.Find("GameStatus");
        SetBlockLevel(1);
        SetBlockScore(1);
    }

    // Update is called once per frame
    void Update()
    {
        if (!isBlockClicked)
        {
            OnMouseDrag();
        }
        else
        {
        }
        if (Input.GetMouseButtonDown(0) && !isBlockClicked)
        {
            gameObject.GetComponent<Rigidbody>().constraints &= ~RigidbodyConstraints.FreezePositionY;
            SetBlockClicked();
            gameStatus.GetComponent<GameStatus>().StartBlockCoroutine();
        }

    }
    void OnMouseDrag()
    {
        float distance_to_screen = Camera.main.WorldToScreenPoint(gameObject.transform.position).z;
        Vector3 pos_move = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, distance_to_screen));
        transform.position = new Vector3(pos_move.x, transform.position.y, pos_move.z);

    }
    public void SetBlockLevel(int level)
    {
        blockLevel = level;
        gameObject.GetComponent<Renderer>().sharedMaterial = materials[blockLevel-1];
    }
    public void IncreaseBlockLevel(bool increaseScore)
    {
        blockLevel++;
        if (increaseScore)
        {
            SetBlockScore(blockLevel);
        }
        gameObject.GetComponent<Renderer>().sharedMaterial = materials[blockLevel-1];
        gameObject.transform.localScale += new Vector3(0.2f, 0.2f, 0.2f);
    }
    public void SetBlockClicked()
    {
        isBlockClicked = true;
    }
    void OnCollisionEnter(Collision collision)
    {
        Block targetBlock = collision.collider.GetComponent<Block>();
        int targetBlockLevel=0;
        if (targetBlock != null)
        {
            targetBlockLevel = targetBlock.blockLevel;
        }
        Vector3 target = new Vector3();
        target = Vector3.Lerp(gameObject.transform.position, collision.transform.position, 0.5f);
        if (collision.gameObject.tag == "Block" && blockLevel == targetBlockLevel)
        {
            if (targetBlock.GetInstanceID() > GetInstanceID())
            {
                StartCoroutine(BlockColliderAnimation(target,true));
            }
            else
            {
                StartCoroutine(BlockColliderAnimation(target,false));
                Destroy(gameObject);
            }
        }

    }


    IEnumerator BlockColliderAnimation(Vector3 target,bool increaseScore)
    {
        Vector3 current = gameObject.transform.position;

        float elapsedTime = 0;

        while (elapsedTime < animationTime)
        {
            rb.MovePosition(Vector3.Lerp(current, target, (elapsedTime / animationTime)));
            //transform.position = Vector3.Lerp(current, target, (elapsedTime / animationTime));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        IncreaseBlockLevel(increaseScore);

        yield return null;
        StartCoroutine(RotateBlock(0.1f));

    }
    IEnumerator RotateBlock(float inTime)
    {
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        float angle = 0;
        Vector3 axis;
        if (transform.rotation.y > 45)
        {
            angle = transform.rotation.y;
            axis = Vector3.down;
        }
        else
        {
            angle = 90 - transform.rotation.y;
            axis = Vector3.up;
        }


        // calculate rotation speed
        float rotationSpeed = angle / inTime;


        // save starting rotation position
        Quaternion startRotation = transform.rotation;

        float deltaAngle = 0;

        // rotate until reaching angle
        while (deltaAngle < angle)
        {
            deltaAngle += rotationSpeed * Time.deltaTime;
            deltaAngle = Mathf.Min(deltaAngle, angle);

            rb.rotation = startRotation * Quaternion.AngleAxis(deltaAngle, axis);

            yield return null;
        }
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.AddRelativeForce(Vector3.down);

    }
    public void SetBlockScore(int level)
    {
        var currScore = GetBlockScore();
        blockScore = level * (level + 1) / 2;

        gameStatus.GetComponent<GameStatus>().IncreseGameScore(blockScore - (currScore*2));
    }
    public int GetBlockScore()
    {
        return blockScore;
    }
}
