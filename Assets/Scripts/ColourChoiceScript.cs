using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ColourChoiceScript : MonoBehaviour
{
    GameManagerScript gameManager;

    Color myButtonColour;

    SelectedColourBGScript myBG;
    public void choosePlayerColour(){
        gameManager.playerColour = myButtonColour;
        myBG.updateValues(GetComponent<RectTransform>().position, myButtonColour);

        AudioSource.PlayClipAtPoint(gameManager.NormalClickSound, Camera.main.transform.position);

    }

    // Start is called before the first frame update
    void Start()
    {
        Button button = GetComponent<Button>();
        gameManager = GameObject.Find("GameManager").GetComponent<GameManagerScript>();
        myButtonColour = GetComponent<Image>().color;
        button.onClick.AddListener(choosePlayerColour);
        myBG = GameObject.Find("SelectedColourBG").GetComponent<SelectedColourBGScript>();
    }

}
