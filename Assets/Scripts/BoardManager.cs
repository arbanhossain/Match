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
        // Enable the timer if Timer is enabled from settings
        isTimed = (PlayerPrefs.GetInt("Timer", 0) == 1) ? true : false;
        if(isTimed) timer.gameObject.SetActive(true);

        PlaceTiles(); // Place the tiles
        PopulateBoard(); // Generate Random Numbers and assign them to the tiles
        UpdateScoreText(); // Initialize the score
    }

    // Use FixedUpdate for controlled updates and allowing the game to display visual changes
    void FixedUpdate()
    {
        if (a != 0 && b != 0) CheckPair();
        CheckForcedEnd(); // At each fixed update frame check if the remaining tiles make a valid target
    }

    private void PopulateBoard() {
        // Generate random target and a sign
        target = GetRandomNumber(10, targetRange);
        sign = GetRandomSign();

        // Update the target and sign texts

        targetText.text = System.Convert.ToString(target);
        signText.text = (sign == -1) ? "-" : "+";

        // Pair generation starts from here

        GameObject[] tiles = GameObject.FindGameObjectsWithTag("Tile");

        List<int> numbers = new List<int>();

        for(int i=0; i<tileCount/2; i++) {
            int num, pairNum;

            // Keep generating a number until the number has not been generated already
            // and its pair is not negative
            do {
                num = GetRandomNumber(
                    Mathf.Max(1, target - (difficulty * difficultyMargin)), // the difficulty margin allows us to generate targets with bigger range, default difficulty is 1
                    target + (difficulty * difficultyMargin)
                );
                pairNum = GetPair(num, sign, target);
            } while (pairNum < 0 || numbers.Contains(num));
            
            // If the number's pair is 0 (which is an edge case in the event the generated number is equal the target),
            // or there is an 8% chance, generate an invalid pair number
            if (pairNum == 0 || Random.Range(0f, 1f) > 0.92f) {
                pairNum = GetRandomNumber(
                    Mathf.Max(1, target - (difficulty * difficultyMargin)), 
                    target + (difficulty * difficultyMargin)
                );
            }

            // Add the numbers to the list
            numbers.Add(num);
            numbers.Add(pairNum);
        }

        numbers = numbers.OrderBy( x => Random.value ).ToList(); // Shuffle the array

        // Assign the numbers to the tiles

        for (int i=0; i<tileCount; i++){
            tiles[i].GetComponent<TileScript>().SetNumber(numbers[i]);
        }

        // foreach(var x in numbers) {
        //     Debug.Log(x.ToString());
        // }
    }

    private void PlaceTiles() {
        // Tile placement keeps track of screen dimensions and distributes them evenly

        int screenWidth = Screen.width;
        int screenHeight = Screen.height;
        
        int horizontalGap = (screenWidth - (2 * horizontalPadding)) / column;
        int verticalGap = (screenHeight - (2 * verticalPadding)) / row;

        // Nested loops add O(n^2) complexity but our loops run for very small counters so the effect is negligible
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
        // This is called whenever two tiles are selected

        GameObject[] tiles = GameObject.FindGameObjectsWithTag("Tile");
        int tileLength = tiles.Length;

        // check if the pairs are valid, then loop over all the tiles and get the selected tiles, then call the destruct method
        if ((sign == 1 && a + b == target) || (sign == -1 && Mathf.Abs(a-b) == target)) {
            foreach(GameObject tile in tiles) {
                if(tile.GetComponent<TileScript>().selected) {
                    tile.GetComponent<TileScript>().SelfDestruct();
                }
            }
            a = 0;
            b = 0;
            score++;
            
            // play a tiny +1 animation for better visibility
            plusOneAnimationTimeline.Play();
            UpdateScoreText();

            tileLength -= 2;
            CheckForcedEnd(); // this method is called multiple times throughout the lifecycle of the board to make sure we don't miss the window
        } else {
            // if the slected tiles do not make a valid pair

            // play the animation for wrong answer

            scoreText.GetComponent<PlayableDirector>().Play();

            // reset the tiles
            foreach(GameObject tile in tiles) {
                tile.GetComponent<TileScript>().ResetState();
            }
            a = 0;
            b = 0;
        }

        CheckForcedEnd();

        // Check if game is over

        if (tileLength == 0) {
            TriggerGameOver();
        }
    }

    private void CheckForcedEnd() {
        // This checks if the remaining tiles fail to make up a valid answer, with the help of CheckValidity function

        GameObject[] tiles = GameObject.FindGameObjectsWithTag("Tile");
        List<int> numbers = new List<int>();
        foreach(GameObject tile in tiles) {
            numbers.Add(tile.GetComponent<TileScript>().GetNumber());
        }
        if(!CheckValidity(numbers)) { // If the numbers on the tiles are not valid
            TriggerGameOver();
        }
    }

    private bool CheckValidity(List<int> numbers) {
        // Validity of numbers is calculated by running all numbers against each other to see if the produce the target

        // following loop instantly returns to if it finds a number
        for(int i = 0; i<numbers.Count; i++) {
            for (int j = 0; j<numbers.Count; j++) {
                if(i == j) continue;
                int a = numbers[i], b = numbers[j];
                if ((sign == 1 && a + b == target) || (sign == -1 && Mathf.Abs(a-b) == target)) {
                    return true;
                }
            }
        }

        // If the loop reaches here, that means we were unable to find any matching pair
        return false;
    }

    public void TriggerGameOver() {
        Debug.Log("Game Over!!!!!");
        PlayerPrefs.SetInt("Current Score", score); // Save the current score
        SceneManager.LoadScene("Game Over"); // Load "Game Over" scene
    }

    // We use two variables for holding the tile numbers where we queue them temporarily,  
    // at each FixedUpdate call, check if those two numbers make a valid pair
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
