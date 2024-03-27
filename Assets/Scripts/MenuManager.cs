using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{


    // Import the javascript function that redirects to another URL
    [DllImport("__Internal")]
    private static extern void RedirectTo();
    // Import the javascript function that for start game events   [DllImport("__Internal")]
    private static extern void StartGameEvent();
    // Import the javascript function for start level events
    [DllImport("__Internal")]
    private static extern void StartLevelEvent(int level);
    // Import the javascript for replay events
    [DllImport("__Internal")]
    private static extern void ReplayEvent();



    public enum Menu
    {
        Main,
        Levels,
        Controls,
        Credits
    };
    public Menu currentMenu;
    public GameObject[] menus;
    public Transform[] buttons;
    public Transform selector;
    public GameObject challengeUnlockNotice;
    public int rowSize;
    int selected;
    public bool allowHorizontal;
    InputMain controls;
    int xInput;
    int yInput;

    public void Selected()
    {
        selector.GetComponent<Animator>().SetTrigger("Pop");
        if (currentMenu == Menu.Main)
        {
            if (selected + 1 > menus.Length - 1)
            {
                Application.Quit();
            }
            else
            {
                OpenMenu(selected + 1);
            }
        }
        else if (currentMenu == Menu.Credits || currentMenu == Menu.Controls)
        {
            OpenMenu(0);
        }
        else if (currentMenu == Menu.Levels)
        {

            if (selected == buttons.Length - 1)
            {
                OpenMenu(0);
            }
            else
            {
                // Call level manager with build index
                if (selected < PlayerPrefs.GetInt("LevelsUnlocked"))
                {
                    FMODUnity.RuntimeManager.PlayOneShot("event:/SFX/Menu/Press");
                    GameObject.Find("LevelManager").GetComponent<LevelManager>().OpenLevel(selected + 1);
                }
            }
        }
    }

    public void MouseButtonEnter(int buttonNum)
    {
        selected = buttonNum;
        MoveSelectIcon();
    }

    void MoveSelectIcon()
    {
        selector.position = buttons[selected].transform.position;
        if (currentMenu == Menu.Levels)
        {
            if (selected < buttons.Length - 1)
            {
                selector.GetComponent<Animator>().SetBool("Single", true);
            }
            else
            {
                selector.GetComponent<Animator>().SetBool("Single", false);
            }
        }
        selector.GetComponent<Animator>().SetTrigger("Pop");
        FMODUnity.RuntimeManager.PlayOneShot("event:/SFX/Menu/Scroll");
    }

    void OpenMenu(int menuIndex)
    {
        foreach(GameObject menu in menus)
        {
            menu.SetActive(false);
        }
        menus[menuIndex].SetActive(true);
        FMODUnity.RuntimeManager.PlayOneShot("event:/SFX/Menu/Press");
    }

    void UnlockLevels()
    {
        if (currentMenu == Menu.Levels)
        {
            for (int i = 0; i < buttons.Length - 1; i++)
            {
                if (i < PlayerPrefs.GetInt("LevelsUnlocked"))
                {
                    buttons[i].GetComponent<Image>().color = new Color32(0, 0, 0, 130);
                }
                else
                {
                    buttons[i].GetComponent<Image>().color = new Color32(255, 0, 0, 130);
                }
            }
        }
        else if (currentMenu == Menu.Main && PlayerPrefs.GetInt("LevelsUnlocked") == 17 && challengeUnlockNotice != null)
        {
            challengeUnlockNotice.SetActive(true);
        }
    }


    //-----------------------------Input Actions Setup----------------------
    void Awake()
    {
        //----------------------Setup input events---------------------------
        controls = new InputMain();
        // Select Press
        controls.Menu.Select.performed += ctx => Selected();
 
        // Scroll Horizontal
        controls.Menu.HorizontalScroll.performed += ctx => {
            if (allowHorizontal && xInput != (int)Mathf.Sign(ctx.ReadValue<float>())) 
            {
                xInput = (int)Mathf.Sign(ctx.ReadValue<float>());
                selected = Mathf.Clamp(selected + xInput, 0, buttons.Length - 1);
                MoveSelectIcon();
            }
        };
        controls.Menu.HorizontalScroll.canceled += ctx => xInput = 0;
        // Scroll Vertical
        controls.Menu.VerticalScroll.performed += ctx => {
            if (yInput != (int)Mathf.Sign(ctx.ReadValue<float>()))
            {
                yInput = -(int)Mathf.Sign(ctx.ReadValue<float>());
                selected = Mathf.Clamp(selected + yInput * rowSize, 0, buttons.Length - 1);
                MoveSelectIcon();
            }
        };
        controls.Menu.VerticalScroll.canceled += ctx => yInput = 0;

        if (!PlayerPrefs.HasKey("LevelsUnlocked"))
        {
            PlayerPrefs.SetInt("LevelsUnlocked", 1);
        }
    }
    void OnEnable()
    {
        selected = 0;
        UnlockLevels();
        controls.Menu.Enable();
        MoveSelectIcon();
    }

    void OnDisable()
    {
        controls.Menu.Disable();
    }

    public void StartGame()
    {
#if !UNITY_EDITOR
        StartGameEvent();
#endif
        Debug.Log("start game");
    }

    public void StartLevel(int level)
    {
        StartLevelEvent(level);
    }
}

