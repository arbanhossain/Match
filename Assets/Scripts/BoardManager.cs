using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BoardManager : MonoBehaviour
{
    [Header("Board Settings")]

    private int tileCount = 20;
    public int horizontalPadding = 50;
    public int verticalPadding = 100;
    public int xOffset = 25;
    public int yOffset = 25;
    private int column = 4;
    private int row = 5;

    [Header("Game Settings")]

    public int difficulty = 1;
    public int difficultyMargin = 25;
    [SerializeField] private int targetRange = 100;
    private bool isTimed = false;

    [Header("Prefabs/GameObjects")]

    [SerializeField] private GameObject tilePrefab;
    public TextMesh targetText;
    public TextMesh signText;
    public TextMesh scoreText;
    public Image timer;
    public PlayableDirector plusOneAnimationTimeline;


    private int a = 0, b = 0;
    private int target, sign;

    private int score = 0;

    void Start()
    {
        isTimed = (PlayerPrefs.GetInt("Timed", 0) == 1) ? true : false;
        if(isTimed) timer.gameObject.SetActive(true);
        PlaceTiles();
        PopulateBoard();
        UpdateScoreText();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (a != 0 && b != 0) CheckPair();
    }

    private void PopulateBoard() {
        target = GetRandomNumber(10, targetRange);
        sign = GetRandomSign();

        targetText.text = System.Convert.ToString(target);
        signText.text = (sign == -1) ? "-" : "+";

        GameObject[] tiles = GameObject.FindGameObjectsWithTag("Tile");

        List<int> numbers = new List<int>();

        for(int i=0; i<tileCount/2; i++) {
            int num, pairNum;
            do {
                num = GetRandomNumber(
                    Mathf.Max(1, target - (difficulty * difficultyMargin)), 
                    target + (difficulty * difficultyMargin)
                );
                pairNum = GetPair(num, sign, target);
            } while (pairNum < 0 || numbers.Contains(num));
            
            if (pairNum == 0 || Random.Range(0f, 1f) > 0.7f) {
                pairNum = GetRandomNumber(
                    Mathf.Max(1, target - (difficulty * difficultyMargin)), 
                    target + (difficulty * difficultyMargin)
                );
            }
            numbers.Add(num);
            numbers.Add(pairNum);
        }

        numbers = numbers.OrderBy( x => Random.value ).ToList();

        for (int i=0; i<tileCount; i++){
            tiles[i].GetComponent<TileScript>().SetNumber(numbers[i]);
        }

        // foreach(var x in numbers) {
        //     Debug.Log(x.ToString());
        // }
    }

    private void PlaceTiles() {
        int screenWidth = Screen.width;
        int screenHeight = Screen.height;
        
        int horizontalGap = (screenWidth - (2 * horizontalPadding)) / column;
        int verticalGap = (screenHeight - (2 * verticalPadding)) / row;

        for (int i=0; i<row; i++) {
            for (int j=0; j<column; j++) {
                int x = horizontalPadding + (j * horizontalGap) + xOffset;
                int y = verticalPadding + (i * verticalGap) + yOffset;
                Vector3 pos = Camera.main.ScreenToWorldPoint(new Vector3(x, y, 10));
                Instantiate(tilePrefab, pos, Quaternion.identity);
            }
        }
    }

    private void UpdateScoreText() {
        scoreText.text = System.Convert.ToString(score);
    }

    private int GetRandomNumber(int min, int max) {
        return Random.Range(min, max + 1);
    }

    private int GetRandomSign() {
        return Random.Range(0f, 1f) > 0.5f ? -1 : 1;
    }

    private int GetPair(int num, int sign, int target) {
        if (sign == 1) {
            return target - num;
        } else {
            if (num < target) return num + target;
            else if (num > target) return num - target;
            else return 0;
        }
    }

    private void CheckPair() {
        GameObject[] tiles = GameObject.FindGameObjectsWithTag("Tile");
        int tileLength = tiles.Length;

        if ((sign == 1 && a + b == target) || (sign == -1 && Mathf.Abs(a-b) == target)) {
            foreach(GameObject tile in tiles) {
                if(tile.GetComponent<TileScript>().selected) {
                    tile.GetComponent<TileScript>().SelfDestruct();
                }
            }
            a = 0;
            b = 0;
            score++;
            plusOneAnimationTimeline.Play();
            UpdateScoreText();
            tileLength -= 2;
        } else {
            scoreText.GetComponent<PlayableDirector>().Play();
            // not correct
            foreach(GameObject tile in tiles) {
                tile.GetComponent<TileScript>().ResetState();
            }
            a = 0;
            b = 0;
        }

        // Check if game is over

        if (tileLength == 0) {
            TriggerGameOver();
        } else {
            tiles = GameObject.FindGameObjectsWithTag("Tile");
            List<int> numbers = new List<int>();
            foreach(GameObject tile in tiles) {
                numbers.Add(tile.GetComponent<TileScript>().GetNumber());
            }
            if(!CheckValidity(numbers)) {
                TriggerGameOver();
            }
        }
    }

    private bool CheckValidity(List<int> numbers) {
        for(int i = 0; i<numbers.Count; i++) {
            for (int j = 0; j<numbers.Count; j++) {
                if(i == j) continue;
                int a = numbers[i], b = numbers[j];
                if ((sign == 1 && a + b == target) || (sign == -1 && Mathf.Abs(a-b) == target)) {
                    return true;
                }
            }
        }
        
        return false;
    }

    public void TriggerGameOver() {
        Debug.Log("Game Over!!!!!");
        PlayerPrefs.SetInt("Current Score", score);
        SceneManager.LoadScene("Game Over");
    }

    public void QueueNumber(int num) {
        if (a == 0) a = num;
        else if (b == 0) b = num;
        Debug.Log($"Numbers on Hold: {a} {b}");
    }

    public void DequeueNumber(int num) {
        if (a == num) a = 0;
        else if (b == num) b = 0;
    }
}
