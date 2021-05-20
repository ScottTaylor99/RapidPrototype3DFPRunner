using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class SceneLoader : MonoBehaviour
{
    public AudioSource buttonClick;

    public void Start()
    {
        //Turn cursor on and usable when returning to the main menu
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }
    public void Tutorial()
    {
        //Sound Wasn't playing Before SceneManger.LoadScene, also tried onMouseDown ended up having to use a Coroutine
        buttonClick.Play();
        StartCoroutine("TutorialLevel");
    }

    public void StartGame()
    {
        buttonClick.Play();
        StartCoroutine("LevelOne");
    }
    IEnumerator TutorialLevel()
    {
        yield return new WaitForSeconds(1);
        SceneManager.LoadScene("SampleScene");
    }

    IEnumerator LevelOne()
    {
        //Coroutine Works!!
        yield return new WaitForSeconds(1);
        SceneManager.LoadScene("Level0");
    }

}
