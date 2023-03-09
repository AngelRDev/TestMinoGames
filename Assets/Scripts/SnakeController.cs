using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.InputSystem;

public class SnakeController : MonoBehaviour
{
    public GameObject tailPrefab;
    public List<Transform> tail = new List<Transform>();
    public InputActionAsset playerMovement;

    Vector3 dir = Vector3.left;
    Vector3 lastDir;
    bool ate = false;

    /* Could be used in a replay system
     Storing this data somewhere, its own struct for example, an list of that struct
     adding a startposition + movesUsed for each snake inside the game. Then just
     advance the moves the movements each time Move() is called
    */
    List<Vector3> startPositions; 
    List<Vector2> movesUsed;
    int moveCounter = 0;
    bool isReplay = false; // false-> accepts player input; true -> just replays stored moves from a saved start position

    private void OnEnable()
    {
        playerMovement.Enable();
    }

    private void OnDisable()
    {
        playerMovement.Disable();
    }

    private void Start()
    {
        // This can be used to build a simulation/replay of the movements used by a player
        movesUsed = new List<Vector2>();
        startPositions = new List<Vector3>();
        startPositions.Add(transform.position); // add snake head start position
        for(int i = 0; i < tail.Count; i++)
        {
            startPositions.Add(tail[i].position); // add snake tail positions
        }

        InvokeRepeating("Move", 0f, 0.15f);
    }

    private void Update()
    {
        if(!isReplay) {
            lastDir = dir;

            dir = playerMovement.FindAction("Move").ReadValue<Vector2>().normalized;
            if (dir == Vector3.zero || (dir.x != 0 && dir.y != 0)) dir = lastDir;

            if (CheckSelfCollision(dir)) dir = lastDir;
        }
    }

    // Called via InvokeRepeating on Start()
    void Move()
    {
        // Replay moves
        if(isReplay) {
            if (moveCounter < movesUsed.Count)
            {
                dir = movesUsed[moveCounter];
                moveCounter++;
            }
        }

        // Destroy this gameObject if it will collide with itself
        if (CheckSelfCollision(dir)) BoardManager.snakeDeath?.Invoke(this);

        Vector2 v = transform.position;
        transform.position += dir;

        // Add new tail if you ate an apple in the last turn
        if (ate)
        {
            GameObject g = Instantiate(tailPrefab, v, Quaternion.identity);
            g.transform.parent = this.transform.parent;
            tail.Insert(0, g.transform);
            ate = false;
        }

        else if (tail.Count > 0)
        {
            tail.Last().position = v;

            tail.Insert(0, tail.Last());
            tail.RemoveAt(tail.Count - 1);
        }

        // Add movement to moveUsed, this can be used for a replay system
        movesUsed.Add(dir);
    }

    // Checks the currentPosition + a new direction will collide or not with itself
    bool CheckSelfCollision(Vector3 dir)
    {
        Vector3 newPos = transform.position + dir;
        for(int i = 0; i < tail.Count; i++)
        {
            if(newPos == tail[i].transform.position) {
                return true;
            }
        }
        return false;
    }

    void OnTriggerStay2D(Collider2D col)
    {
        if (col.gameObject.tag == "Apple")
        {
            ate = true;
            BoardManager.appleEaten?.Invoke(this);
        }
        else if(col.gameObject.tag == "Snake")
        {
            BoardManager.snakeDeath?.Invoke(this);
        }
    }

    public List<Transform> GetTail()
    {
        return tail;
    }
}