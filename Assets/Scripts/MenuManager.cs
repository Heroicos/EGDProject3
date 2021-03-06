using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;  //Remove later, reserve scene changes for TRANSITIONMANAGER

public class MenuManager : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private InputManager inputMan;
    [SerializeField] private AudioSource menuSource;
    [SerializeField] private Graphic bg;

    public List<Button> mainButtons;
    public List<Button> playButtons;
    public List<Button> optionButtons;
    public List<Button> helpButtons;
    public List<Button> creditsButtons;
    public List<Button> quitButtons;
    public List<GameObject> panels;
    public AudioClip choiceClip;
    public AudioClip confirmClip;
    public AudioSource seSource;
    public AudioSource musicSource;
    

    [Header("Variables")]
    public int mainIndex;
    public int playIndex;
    public int optionsIndex;
    public int helpIndex;
    public int creditsIndex;
    public int quitIndex;
    public States.MenuSection currentSection;

    [Header("Button Input")]
    [SerializeField] public float inputX;      //A-D
    [SerializeField] public float inputY;      //W-S
    [SerializeField] public float inputSubmit; //Enter
    [SerializeField] public float inputCancel; //Escape
    [SerializeField] public float inputAlt1;   //Space
    [SerializeField] public float inputAlt2;   //Tab
    [SerializeField] public float inputAlt3;   //Shift
    [SerializeField] public float inputAlt4;   //Ctrl
    [SerializeField] public bool inputButton;  //Check if button pressed

    /*============================================================================
     * DEFAULT UNITY METHODS
     ============================================================================*/
    public void Start()
    {
        inputMan = GameObject.Find("[MANAGER]").GetComponent<InputManager>();
        SetActiveMenuPanel(States.MenuSection.Main);
        mainIndex = 0;
        musicSource.clip = Resources.Load<AudioClip>("Audio/Music/doodle");
        musicSource.Play();
    }

    private void Update()
    {
        if (inputMan.inputY_D || inputMan.inputCancel_D)
        {
            NavUpdate();
            seSource.clip = choiceClip;
            seSource.Play();
        }
        if (inputMan.inputSubmit_D)
        {
            seSource.clip = confirmClip;
            seSource.Play();
        }
        ButtonUpdate();
        /*
        if (currentSection == States.MenuSection.Main)
        {
            panels[0].transform.GetChild(mainIndex-1).GetComponent<Image>().color = new Color(0.0f, 1.0f, 0.0f);
            for (int i = 0; i < panels[0].transform.childCount-1; i++)
            {
                if (i != (mainIndex-1))
                {
                    panels[0].transform.GetChild(i).GetComponent<Image>().color = new Color(1.0f, 1.0f, 1.0f);
                }
            }
            if (!inputButton)
            {
                //Down/S
                if (inputY < 0.0f && mainIndex < 5)
                {
                    mainIndex += 1;
                    inputButton = true;
                }
                //Up/W
                else if (inputY > 0.0f && mainIndex > 1)
                {
                    mainIndex -= 1;
                    inputButton = true;
                }
                //Enter/Submit
                if (inputSubmit != 0.0f)
                {
                    MenuChoice(mainIndex);
                    inputButton = true;
                }
                //Esc/Cancel
                if (inputCancel != 0.0f)
                {
                    SetActiveMenuPanel(States.MenuSection.Main);
                    inputButton = true;
                }
            }
        }
        else if (currentSection == States.MenuSection.Play)
        {
            panels[1].transform.GetChild(playIndex).GetComponent<Image>().color = new Color(0.0f, 1.0f, 0.0f);
            for (int i = 0; i < panels[1].transform.childCount - 1; i++)
            {
                if (i != (playIndex))
                {
                    if (i >= 0 || i <= 2)
                    {
                        panels[1].transform.GetChild(i).GetComponent<Image>().color = new Color(102/255f, 221/255f, 1.0f, 100/255f);
                    }
                    else
                    {
                        panels[1].transform.GetChild(i).GetComponent<Image>().color = new Color(1.0f, 1.0f, 1.0f);
                    }
                }
            }
            if (!inputButton)
            {
                //Down/S
                if (inputY < 0.0f && playIndex < 4)
                {
                    if (playIndex >= 0 || playIndex <= 2)
                    {
                        playIndex = 4;
                    }
                    else
                    {
                        playIndex = 3;
                    }
                    inputButton = true;
                }
                //Up/W
                else if (inputY > 0.0f && playIndex != 3)
                {
                    if (playIndex >= 0 || playIndex <= 2)
                    {
                        playIndex = 3;
                    }
                    else
                    {
                        playIndex = 0;
                    }
                    inputButton = true;
                }
                //Left/A
                if (inputX < 0.0f && playIndex > 0)
                {
                    playIndex -= 1;
                    inputButton = true;
                }
                //Right/D
                else if (inputX > 0.0f && playIndex < 2)
                {
                    playIndex += 1;
                    inputButton = true;
                }
                //Enter/Submit
                if (inputSubmit != 0.0f)
                {
                    MenuChoice(mainIndex);
                    inputButton = true;
                }
                //Esc/Cancel
                if (inputCancel != 0.0f)
                {
                    SetActiveMenuPanel(States.MenuSection.Main);
                    playIndex = 0;
                    inputButton = true;
                }
            }
        }
        else if (currentSection == States.MenuSection.Options)
        {

        }
        else if (currentSection == States.MenuSection.Help)
        {

        }
        else if (currentSection == States.MenuSection.Options)
        {

        }
        else if (currentSection == States.MenuSection.Credits)
        {

        }
        else if (currentSection == States.MenuSection.Quit)
        {

        }
        */

        inputButton = false;
    }

    /*============================================================================
     * MENU METHODS
     ============================================================================*/
    private void NavUpdate()
    {
        int navDiff = (inputMan.inputY > 0) ? -1 : 0 + ((inputMan.inputY < 0) ? 1 : 0);
        if (currentSection == States.MenuSection.Main)
        {
            mainIndex += ((navDiff < 0 && mainIndex > 0) || (navDiff > 0 && mainIndex < mainButtons.Count - 1)) ? navDiff : 0;
            mainButtons[mainIndex].Select();
        }
        else if (currentSection == States.MenuSection.Play)
        {
            playIndex += ((navDiff < 0 && playIndex > 0) || (navDiff > 0 && playIndex < playButtons.Count - 1)) ? navDiff : 0;
            playButtons[playIndex].Select();
        }
        else if (currentSection == States.MenuSection.Options)
        {
            optionsIndex += ((navDiff < 0 && optionsIndex > 0) || (navDiff > 0 && optionsIndex < optionButtons.Count - 1)) ? navDiff : 0;
            optionButtons[optionsIndex].Select();
        }
        else if (currentSection == States.MenuSection.Help)
        {
            helpIndex += ((navDiff < 0 && helpIndex > 0) || (navDiff > 0 && helpIndex < helpButtons.Count - 1)) ? navDiff : 0;
            helpButtons[helpIndex].Select();
        }
        else if (currentSection == States.MenuSection.Credits)
        {
            creditsIndex += ((navDiff < 0 && creditsIndex > 0) || (navDiff > 0 && creditsIndex < creditsButtons.Count - 1)) ? navDiff : 0;
            creditsButtons[creditsIndex].Select();
        }
        else if (currentSection == States.MenuSection.Quit)
        {
            quitIndex += ((navDiff < 0 && quitIndex > 0) || (navDiff > 0 && quitIndex < quitButtons.Count - 1)) ? navDiff : 0;
            quitButtons[quitIndex].Select();
        }
    }

    public void openMenu(int num)
    {
        if (num >= 0 && num <= 5) closeCurrentMenu();
        switch (num)
        {
            case 0:
                currentSection = States.MenuSection.Main;
                mainIndex = 0;
                panels[0].SetActive(true);
                break;
            case 1:
                currentSection = States.MenuSection.Play;
                playIndex = 0;
                panels[1].SetActive(true);
                string f1 = "File1.txt";
                string f2 = "File2.txt";
                string f3 = "File3.txt";
                string fPath1 = Application.streamingAssetsPath + "/Saves/" + f1;
                string fPath2 = Application.streamingAssetsPath + "/Saves/" + f2;
                string fPath3 = Application.streamingAssetsPath + "/Saves/" + f3;
                if (File.Exists(fPath1))
                {
                    Debug.Log("T1");
                    string json = File.ReadAllText(fPath1);
                    SaveFile sf = JsonUtility.FromJson<SaveFile>(json);
                    if (sf.gameStart)
                    {
                        panels[1].transform.GetChild(0).GetChild(0).GetComponent<Text>().text = "Slot 1/nActive Save File";
                    }
                }
                if (File.Exists(fPath2))
                {
                    Debug.Log("T2");
                    string json = File.ReadAllText(fPath2);
                    SaveFile sf = JsonUtility.FromJson<SaveFile>(json);
                    if (sf.gameStart)
                    {
                        panels[1].transform.GetChild(1).GetChild(0).GetComponent<Text>().text = "Slot 2/nActive Save File";
                    }
                }
                if (File.Exists(fPath3))
                {
                    Debug.Log("T3");
                    string json = File.ReadAllText(fPath3);
                    SaveFile sf = JsonUtility.FromJson<SaveFile>(json);
                    if (sf.gameStart)
                    {
                        panels[1].transform.GetChild(2).GetChild(0).GetComponent<Text>().text = "Slot 3/nActive Save File";
                    }
                }
                break;
            case 2:
                currentSection = States.MenuSection.Options;
                optionsIndex = 0;
                panels[2].SetActive(true);
                break;
            case 3:
                currentSection = States.MenuSection.Help;
                helpIndex = 0;
                panels[3].SetActive(true);
                break;
            case 4:
                currentSection = States.MenuSection.Credits;
                creditsIndex = 0;
                panels[4].SetActive(true);
                break;
            case 5:
                currentSection = States.MenuSection.Quit;
                quitIndex = 0;
                panels[5].SetActive(true);
                break;
        }
    }

    public void openSaveSlot(int save)
    {
        SaveFile tmp;
        switch (save)
        {
            case 0:
                PlayerPrefs.SetInt("SaveNum", 1);
                tmp = SaveManager.Load();
                SceneManager.LoadScene(tmp.lastScene);
                break;
            case 1:
                PlayerPrefs.SetInt("SaveNum", 2);
                tmp = SaveManager.Load();
                SceneManager.LoadScene(tmp.lastScene);
                break;
            case 2:
                PlayerPrefs.SetInt("SaveNum", 3);
                tmp = SaveManager.Load();
                SceneManager.LoadScene(tmp.lastScene);
                break;
            case 3:
                SceneManager.LoadScene("Testing/[TestScene]");
                break;
        }
    }

    public void closeCurrentMenu()
    {
        switch (currentSection)
        {
            case States.MenuSection.Main:
                panels[0].SetActive(false);
                break;
            case States.MenuSection.Play:
                panels[1].SetActive(false);
                break;
            case States.MenuSection.Options:
                panels[2].SetActive(false);
                break;
            case States.MenuSection.Help:
                panels[3].SetActive(false);
                break;
            case States.MenuSection.Credits:
                panels[4].SetActive(false);
                break;
            case States.MenuSection.Quit:
                panels[5].SetActive(false);
                break;
        }
    }
    
    public void quitGame()
    {
        Application.Quit();
    }

    public void MenuChoice(int index)
    {
        switch (index)
        {
            case -2:
                SceneManager.LoadScene("[TestScene]");
                break;
            case -1:
                Application.Quit();
                break;
            case 0:
                SetActiveMenuPanel(States.MenuSection.Main);
                mainIndex = 1;
                break;
            case 1:
                SetActiveMenuPanel(States.MenuSection.Play);
                playIndex = 0;
                break;
            case 2:
                SetActiveMenuPanel(States.MenuSection.Options);
                break;
            case 3:
                SetActiveMenuPanel(States.MenuSection.Help);
                break;
            case 4:
                SetActiveMenuPanel(States.MenuSection.Credits);
                break;
            case 5:
                SetActiveMenuPanel(States.MenuSection.Quit);
                break;
        }
    }
    
    private void SetActiveMenuPanel(States.MenuSection section)
    {
        panels[(int)currentSection].SetActive(false);
        currentSection = (States.MenuSection)section;
        panels[(int)currentSection].SetActive(true);
    }

    private void ButtonUpdate()
    {
        inputX = Input.GetAxisRaw("Horizontal");
        inputY = Input.GetAxisRaw("Vertical");
        inputSubmit = Input.GetAxisRaw("Submit");
        inputCancel = Input.GetAxisRaw("Cancel");
        inputAlt1 = Input.GetAxisRaw("Alt1");
        inputAlt2 = Input.GetAxisRaw("Alt2");
        inputAlt3 = Input.GetAxisRaw("Alt3");
        inputAlt4 = Input.GetAxisRaw("Alt4");
    }

    /*============================================================================
     * MISC METHODS
     ============================================================================*/
    [ContextMenu("Reset to Default")]
    private void SetDefaultValues()
    {
        mainIndex = 0;
        optionsIndex = 0;
        currentSection = States.MenuSection.Main;
    }

    public void AudioTrigger(AudioClip tClip)
    {
        menuSource.PlayOneShot(tClip);
    }

    public void testSceneLoader(int num)
    {
        switch(num)
        {
            case 0:
                SceneManager.LoadScene("PlatformerWorld");
                break;
            case 1:
                SceneManager.LoadScene("ShooterWorld");
                break;
            case 2:
                SceneManager.LoadScene("RPGBattle");
                break;
        }
    }
}

