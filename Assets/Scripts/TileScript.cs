using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileScript : MonoBehaviour
{
    [Header("Tile Settings")]
    [SerializeField] private Color clickedColor;
    private Color initialColor;

    [HideInInspector] public bool selected = false;
    private BoardManager boardManager;
    // Start is called before the first frame update
    void Start()
    {
        initialColor = GetComponent<SpriteRenderer>().color;
        boardManager = GameObject.Find("BoardManager").GetComponent<BoardManager>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnMouseDown() {
        // On click, toggle between selected and deselected
        ToggleState();
    }

    private void ToggleState() {
        GetComponent<SpriteRenderer>().color = (selected) ? initialColor : clickedColor;
        selected = !selected;

        // If we have selected the number, queue it in the board's memory, otherwise dequeue the number from memory
        if (!selected) {
            boardManager.DequeueNumber(GetNumber());
        } else {
            boardManager.QueueNumber(GetNumber());
        }
    }

    public void ResetState() {
        // Reset in case of a wrong answer
        GetComponent<SpriteRenderer>().color = initialColor;
        selected = false;
    }

    // Get the number associated with the tile
    public int GetNumber() {
        GameObject numberObject = transform.Find("Number").gameObject;
        return System.Convert.ToInt32(numberObject.GetComponent<TextMesh>().text);
    }

    // Set the number in the associated text of tile.
    public void SetNumber(int num) {
        GameObject numberObject = transform.Find("Number").gameObject;
        numberObject.GetComponent<TextMesh>().text = System.Convert.ToString(num);
    }

    public void SelfDestruct() {
        Debug.Log("destroying self");
        Destroy(gameObject);
    }
}
