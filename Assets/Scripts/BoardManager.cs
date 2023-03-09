using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
    [SerializeField] GameObject boardTile;
    [SerializeField] GameObject apple;
    [SerializeField] GameObject snake;
    [SerializeField] GameObject border;

    [SerializeField] int sizeX = 30;
    [SerializeField] int sizeY = 30;

    int halfX;
    int halfY;

    public delegate void SpawnAppleDelegate(SnakeController snake);
    public static SpawnAppleDelegate appleEaten;

    public delegate void SnakeDeathDelegate(SnakeController snake);
    public static SnakeDeathDelegate snakeDeath;

    [SerializeField] List<SnakeController> snakeController;

    void Start()
    {
        halfX = sizeX / 2;
        halfY = sizeY / 2;

        GenerateBoard();
        SpawnApple();
        appleEaten += SpawnApple;
        appleEaten += CheckWin;
        snakeDeath = SnakeDeath;

        // Find the snakes at the beginning of the game
        SnakeController []snakeControllers = GameObject.FindObjectsOfType<SnakeController>();
        for (int i = 0; i < snakeControllers.Length; i++) snakeController.Add(snakeControllers[i]);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P)) SpawnSnake(); // For testing
    }

    void GenerateBoard()
    {
        // Generate playable board
        for(int i = -halfX; i < halfX; i++)
        {
            for (int j = -halfY; j < halfY; j++)
            {
                Instantiate(boardTile, new Vector3(i, j, 0f), Quaternion.identity);
            }
        }

        // Generate borders
        for(int i = -halfX - 1; i < halfX + 1; i++)
        {
            Instantiate(border, new Vector3(i, -halfY - 1, 0f), Quaternion.identity);
            Instantiate(border, new Vector3(i, halfY, 0f), Quaternion.identity);
        }

        for (int i = -halfY - 1; i < halfY + 1; i++)
        {
            Instantiate(border, new Vector3(-halfX - 1, i, 0f), Quaternion.identity);
            Instantiate(border, new Vector3(halfX, i, 0f), Quaternion.identity);
        }
    }

    void SpawnApple(SnakeController snake = null)
    {
        int x, y = 0;
        do
        {
            x = Random.Range(-halfX, halfX);
            y = Random.Range(-halfY, halfY);

        } while (!IsTileEmpty(x,y));

        apple.transform.position = new Vector3(x, y, 0f);
    }

    void CheckWin(SnakeController snake)
    {
        if (snake.GetTail().Count == (sizeX * sizeY)) Debug.Log("WIN!");
    }

    // For testing
    void SpawnSnake()
    {
        int x, y = 0;
        do
        {
            x = Random.Range(-halfX, halfX - 2); // minus 2 to give space for the next tiles to spawn
            y = Random.Range(-halfY, halfY);

        } while (!IsTileEmpty(x, y) || !IsTileEmpty(x+1,y) || !IsTileEmpty(x + 2, y));

        GameObject go = Instantiate(snake, new Vector3(x, y, 0f), Quaternion.identity);
        snakeController.Add(go.transform.GetComponentInChildren<SnakeController>());
    }

    void SnakeDeath(SnakeController snake)
    {
        snakeController.Remove(snake);
        if (snakeController.Count <= 0) Debug.Log("GAME OVER, NO SNAKES LEFT");

        Destroy(snake.gameObject.transform.parent.gameObject);
    }

    bool IsTileEmpty(int x, int y)
    {
        List<Transform> tail;
        for (int j = 0; j < snakeController.Count; j++) {
            tail = snakeController[j].GetTail();
            for(int i = 0; i < tail.Count; i++)
            {
                if (tail[i].position == new Vector3(x, y, 0)) return false;
            }
        }
        return true;
    }
}

