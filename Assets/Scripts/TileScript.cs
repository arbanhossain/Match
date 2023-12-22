using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileScript : MonoBehaviour
{
    [Header("Tile Settings")]
    [SerializeField] private Color clickedColor;
    private Color initialColor;

    private int i = 0;
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
        // Debug.Log($"Clicked {i++}, Number is {GetNumber()}");
        ToggleState();
    }

    private void ToggleState() {
        GetComponent<SpriteRenderer>().color = (selected) ? initialColor : clickedColor;
        selected = !selected;

        if (!selected) {
            boardManager.DequeueNumber(GetNumber());
        } else {
            boardManager.QueueNumber(GetNumber());
        }
    }

    public void ResetState() {
        GetComponent<SpriteRenderer>().color = initialColor;
        selected = false;
    }

    public int GetNumber() {
        GameObject numberObject = transform.Find("Number").gameObject;
        return System.Convert.ToInt32(numberObject.GetComponent<TextMesh>().text);
    }

    public void SetNumber(int num) {
        GameObject numberObject = transform.Find("Number").gameObject;
        numberObject.GetComponent<TextMesh>().text = System.Convert.ToString(num);
    }

    public void SelfDestruct() {
        Debug.Log("destroying self");
        Destroy(gameObject);
    }
}
