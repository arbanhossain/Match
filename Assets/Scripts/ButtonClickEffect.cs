using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonClickEffect : MonoBehaviour
{
    private Color initialColor;
    public Color clickedColor;
    public float duration = 0.1f;

    private bool pressed = false;

    public string sceneToGoTo;
    // Start is called before the first frame update
    void Start()
    {
        initialColor = GetComponent<SpriteRenderer>().color;
        // Debug.Log(initialColor);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnMouseDown() {
        // Debug.Log("Mouse Down Called");
        if (!pressed) {
            GetComponent<AudioSource>().Play(0);
            StartCoroutine(ChangeColorOnClick());
        }
    }

    IEnumerator ChangeColorOnClick() {
        // Debug.Log("Changing");
        pressed = true;
        GetComponent<SpriteRenderer>().color = clickedColor;
        yield return new WaitForSeconds(duration);
        GetComponent<SpriteRenderer>().color = initialColor;
        pressed = false;
        GoToScene(sceneToGoTo);
    }

    public void GoToScene(string sceneName) {
        SceneManager.LoadScene(sceneName);
    }
}
