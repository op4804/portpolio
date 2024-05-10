using System.Collections;
using System.Collections.Generic;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{


    GameObject sm;

    // Start is called before the first frame update
    void Start()
    {
        sm = GameObject.Find("StageManager");
    }

    // Update is called once per frame
    void Update()
    {
        
    }



    public void OnClickStartButton()
    {
        Debug.Log("onclickButton");
        SceneManager.LoadScene("Stage");
        
    }

    public void OnClickExitButton()
    {
        Debug.Log("onclickButton");
        Application.Quit();
    }

    public void OnClickPauseButton()
    {
        if (sm.GetComponent<StageManager>().isPaused)
        {
            sm.GetComponent<StageManager>().isPaused = false;
            Time.timeScale = 0f;

        }
        else
        {
            sm.GetComponent<StageManager>().isPaused = true;
            Time.timeScale = 1f;
        }       

    }
    public void OnClicMainButton()
    {
        SceneManager.LoadScene("Main");
    }



}
