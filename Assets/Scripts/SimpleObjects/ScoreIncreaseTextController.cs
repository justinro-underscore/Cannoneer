using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreIncreaseTextController : MonoBehaviour
{
    private Color textColor; // The text color (used to change alpha value)

    /**
     * Set the text color
     */
	void Start ()
    {
        textColor = new Color(1f, 1f, 0f);
    }

    /**
     * Set the parameters of the text
     * @param score The score to display
     * @param pos The starting position of the text
     */
    public void SetParams(int score, Vector3 pos)
    {
        this.GetComponent<Text>().text = "+" + score;
        transform.position = pos;
    }
	
    /**
     * Move the text up and make it more transparent
     */
	void Update ()
    {
        this.transform.Translate(new Vector3(0, 0.01f)); // Move the text
        // Set the transparency
        textColor.a = textColor.a - 0.01f;
        this.GetComponent<Text>().color = textColor;
        if (textColor.a <= 0f) // If it is invisible, destroy it
            Destroy(gameObject);
    }
}
