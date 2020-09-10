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
    public float animationTime = 1f;
    // Start is called before the first frame update
    void Start()
    {
        SetBlockLevel(0);
        gameStatus = GameObject.Find("GameStatus");
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
        //gameObject.GetComponent<Renderer>().sharedMaterial = materials[blockLevel];
    }
    public void IncreaseBlockLevel()
    {
        blockLevel++;
        gameObject.GetComponent<Renderer>().sharedMaterial = materials[blockLevel];
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
        Debug.Log("Collision In: " + gameObject.name);
        if (collision.gameObject.tag == "Block" && blockLevel == targetBlockLevel)
        {
            if (targetBlock.GetInstanceID() > GetInstanceID())
            {
                StartCoroutine(BlockColliderAnimation(target));
            }
            else
            {
                StartCoroutine(BlockColliderAnimation(target));
                Destroy(gameObject);
            }
        }

    }
    void OnCollisionExit(Collision collisionInfo)
    {
        print("Collision Out: " + gameObject.name);
    }

    IEnumerator BlockColliderAnimation(Vector3 target)
    {
        Vector3 current = gameObject.transform.position;

        float elapsedTime = 0;

        while (elapsedTime < animationTime)
        {
            transform.position = Vector3.Lerp(current, target, (elapsedTime / animationTime));
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        IncreaseBlockLevel();
        yield return null;
    }
}
