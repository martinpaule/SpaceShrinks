using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UImanager : MonoBehaviour
{
    // Start is called before the first frame update

    public GameObject startGameUI_PF;
    public GameObject upgradeUI_PF;

    public  GameObject upgradeUI = null;
    public GameObject selectedPlanet = null;

    OuterEdgesScript outerEdgesScript;


    public GameObject jumpHomeButton;

    public GameObject ExitOptionsPanel;

    bool exitPanelActive = false;

    GameObject congrTXT;

    GameObject ResultTXT;

    public AudioClip music;

    AudioSource audioSource;

    void Start()
    {
        //spawn start game UI and child it to this object
        Instantiate(startGameUI_PF, transform);

        jumpHomeButton = GameObject.Find("JumpHomeButton");
        jumpHomeButton.SetActive(false);

        ExitOptionsPanel = GameObject.Find("ExitOptions");
        ExitOptionsPanel.SetActive(false);

        outerEdgesScript = GameObject.Find("OuterEdgesManager").GetComponent<OuterEdgesScript>();

        congrTXT = GameObject.Find("CongrText");
        congrTXT.SetActive(false);


        ResultTXT = GameObject.Find("GameResultText");
        ResultTXT.SetActive(false);


        audioSource = GetComponent<AudioSource>();

        audioSource.clip = music;
        audioSource.Play();
        audioSource.loop = true;
    }

    // Update is called once per frame
    void Update()
    {
        //if user presses ESC
        if(Input.GetKeyDown(KeyCode.Escape) && outerEdgesScript.startedGame == true){

            toggleExit();
        }
    }

    public void toggleExit(){
        exitPanelActive = !exitPanelActive;
        ExitOptionsPanel.SetActive(exitPanelActive);

        if(exitPanelActive){
            Time.timeScale = 0.0f;
        }else{
            Time.timeScale = 1.0f;
        }

    }

    public void toggleEndgameUI(bool won){
    
        ResultTXT.SetActive(true);
        congrTXT.SetActive(true);
        if(won){
            congrTXT.GetComponentInChildren<TextMeshProUGUI>().text = "THE GALAXY IS YOURS!";
            ResultTXT.GetComponentInChildren<TextMeshProUGUI>().text = "YOU WON !!!";
            //set their colours to green
            congrTXT.GetComponentInChildren<TextMeshProUGUI>().color = Color.green;
            ResultTXT.GetComponentInChildren<TextMeshProUGUI>().color = Color.green;
        }else{
            congrTXT.GetComponentInChildren<TextMeshProUGUI>().text = "YOUR SPECIES WERE WIPED OUT!";
            ResultTXT.GetComponentInChildren<TextMeshProUGUI>().text = "YOU LOST! :(";
            //set their colours to red
            congrTXT.GetComponentInChildren<TextMeshProUGUI>().color = Color.red;
            ResultTXT.GetComponentInChildren<TextMeshProUGUI>().color = Color.red;

        }
    }

    public void LoadScene(){
        UnityEngine.SceneManagement.SceneManager.LoadScene("SampleScene");
        Time.timeScale = 1.0f;

    }

    public void ExitGame(){
        Application.Quit();
    }

}
