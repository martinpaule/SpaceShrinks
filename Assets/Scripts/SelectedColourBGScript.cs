using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectedColourBGScript : MonoBehaviour
{

    //get my rect transform component
    public RectTransform myRectTransform;

    //get my Image component
    UnityEngine.UI.Image myImage;

    public Color SelectedColor = Color.red;
    public Color TargetColor;

    float colorChangeSpeed = 4.0f;
    float sizeChangeSpeed = 3.0f;

    float timePassed = 0.0f;

    public void updateValues(Vector2 position, Color selectedColor){
        TargetColor = Color.black;
        SelectedColor = selectedColor;
        myImage.color = Color.white;
        myRectTransform.position = position;
    }

    void updateGraphics()
    {
        //myImage.color = Color.Lerp(myImage.color, TargetColor, colorChangeSpeed * Time.deltaTime);

        /* Check if the colors are close enough
        if (Vector4.Distance(myImage.color, TargetColor) < 0.01f)
        {
            if(TargetColor == Color.white)
            {
                TargetColor = Color.black;
            }
            else
            {
                TargetColor = Color.white;
            }
        }*/

        float size = 60 + Mathf.Sin(timePassed * sizeChangeSpeed) * 5;
        //update size
        myRectTransform.sizeDelta = new Vector2(size, size);
    }

    Color GetInvertedColor(Color color)
    {
        float r = 1f - color.r;
        float g = 1f - color.g;
        float b = 1f - color.b;
        float a = color.a;  // Keep alpha the same, or use 1f - color.a to invert alpha
        
        return new Color(r, g, b, a);
    }

    // Start is called before the first frame update
    void Start()
    {
        myImage = GetComponent<UnityEngine.UI.Image>();
        myRectTransform = GetComponent<RectTransform>();
    }

    // Update is called once per frame
    void Update()
    {
        timePassed += Time.deltaTime;
        updateGraphics();
    }
}
