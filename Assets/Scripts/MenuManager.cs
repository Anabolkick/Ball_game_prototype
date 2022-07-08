using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public GameObject StartMenu;
    public static GameObject EndMenu { get; private set; }
    public static TMP_Text EndText {  get; private set; }

    [SerializeField] private GameObject endMenu;
    [SerializeField] private TMP_Text endText;

    void Awake()
    {
        EndMenu = endMenu;
        EndText = endText;
        Time.timeScale = 0;
    }
    public void StartGame()
    {
        StartMenu.SetActive(false);
        Time.timeScale = 1;
    }
    public void Restart()
    {
        SceneManager.LoadScene(0);
    }

    public static void OpenLoseMenu()
    {
        EndText.text = "You Lose!";
        EndMenu.SetActive(true);
        Time.timeScale = 0;
    }

    public static void OpenWinMenu()
    {
        EndText.text = "You Win!";
        EndMenu.SetActive(true);
        Time.timeScale = 0;
    }
}
