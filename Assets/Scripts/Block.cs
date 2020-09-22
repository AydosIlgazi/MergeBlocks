using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour
{
    public int blockLevel { get; set; }
    public Material[] materials;
    public bool isBlockReleased= false;
    private Vector3 screenPoint;
    private Vector3 offset;
    private GameObject gameStatus;
    public GameObject electricAnimation;
    public float animationTime = 0.5f;
    public float minX;
    public float maxX;
    public int blockScore = 0;
    Rigidbody rb;
    public GameObject collisionAnimation;

    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody>();
        gameStatus = GameObject.Find("GameStatus");
        SetBlockLevel(1);
        SetBlockScore(1);
    }

    void Update()
    {
        if (!isBlockReleased && Input.GetMouseButton(0))
        {
            OnMouseDrag();
        }

        if (!Input.GetMouseButton(0) && !isBlockReleased)
        {
            rb.isKinematic = false;
            rb.constraints &= ~RigidbodyConstraints.FreezePositionY;
            SetBlockClicked();
            gameStatus.GetComponent<GameStatus>().blockOnHold = false;
        }

    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Block")
        {

            Block targetBlock = collision.collider.GetComponent<Block>();
            int targetBlockLevel = 0;
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
                    Debug.Log(Time.time);
                    gameStatus.GetComponent<GameStatus>().CollisionMultiplier(Time.time);
                    gameObject.GetComponent<Collider>().enabled = false;
                    StartCoroutine(BlockColliderAnimation(target, true));
                }
                else
                {
                    gameObject.GetComponent<Collider>().enabled = false;
                    StartCoroutine(BlockColliderAnimation(target, false));
                }
            }
        }
        else if(collision.gameObject.tag == "Wall")
        {
            var electricAnimationObj = Instantiate(electricAnimation);
            electricAnimationObj.transform.SetParent(gameObject.transform);
        }

    }

    void OnMouseDrag()
    {

        float pos_X = Mathf.Clamp(Input.mousePosition.x, minX, maxX);
        float distance_to_screen = Camera.main.WorldToScreenPoint(gameObject.transform.position).z;
        Vector3 pos_move = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, distance_to_screen));
        transform.position = new Vector3(Mathf.Clamp(pos_move.x, minX, maxX), transform.position.y, pos_move.z);


    }

    IEnumerator BlockColliderAnimation(Vector3 target, bool increaseScore)
    {
        Instantiate(collisionAnimation, new Vector3(target.x,target.y,target.z),Quaternion.identity);
        //rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ ;
        rb.freezeRotation = true;
        Vector3 current = gameObject.transform.position;
        float elapsedTime = 0;

        while (elapsedTime < animationTime)
        {
            rb.MovePosition(Vector3.Lerp(current, target, (elapsedTime / animationTime)));
            //transform.position = Vector3.Lerp(current, target, (elapsedTime / animationTime));
            elapsedTime += Time.fixedDeltaTime;
            yield return null;
        }

        IncreaseBlockLevel(increaseScore);

        yield return null;
        if (increaseScore)
        {
            gameObject.GetComponent<Collider>().enabled = true;

            StartCoroutine(RotateBlockTorque());
        }
        else
        {
            Destroy(gameObject);
        }

    }

    IEnumerator RotateBlockFixed(float inTime)
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
            deltaAngle += rotationSpeed * Time.fixedDeltaTime;
            deltaAngle = Mathf.Min(deltaAngle, angle);

            rb.rotation = startRotation * Quaternion.AngleAxis(deltaAngle, axis);

            yield return null;
        }
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.AddRelativeForce(Vector3.down);
    }
    
    IEnumerator RotateBlockTorque()
    {
        rb.freezeRotation = false;
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        yield return null;
        //rb.AddRelativeForce(Vector3.down*10f);
        rb.AddTorque(Vector3.up *180f, ForceMode.Impulse);

        yield return null;
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
        isBlockReleased = true;
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
