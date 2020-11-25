using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuCtrl : MonoBehaviour
{
    public static Color btnColor = Color.red;

    public void Red()
    {
        btnColor = Color.red;
    }
    public void Grey()
    {
        btnColor = Color.grey;
    }
    public void Green()
    {
        btnColor = Color.green;
    }
    public void Yellow()
    {
        btnColor = Color.yellow;
    }
    public void Blue()
    {
        btnColor = Color.blue;
    }
    public void White()
    {
        btnColor = Color.white;
        btnColor.a = 0.6f;
    }
    public void ReloadLevel()
    {
        SceneManager.LoadScene(0);
    }
    public void BackToMainMenu()
    {
        SceneManager.LoadScene(0);
    }
    public void Quit()
    {
        Application.Quit();
    }
}
