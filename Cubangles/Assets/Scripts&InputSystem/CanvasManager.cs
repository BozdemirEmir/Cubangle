using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using System.Collections;

public class CanvasManager : MonoBehaviour 
{
    [SerializeField] private GameObject[] menus;
    [SerializeField] private GameObject gameOverMenu;
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject movementJoystick;

    [SerializeField] private Slider volumeSlider;
    [SerializeField] private TextMeshProUGUI volumeSliderValueText;






    private void Start()
    {
        ChangeActives("MainMenu");

    }
    public void StartButton()
    {
        AbstractDungeonGenerator.ChangeCanGenerate(true);
        SceneManager.LoadScene(1);
        ChangeActives("LoadMenu");
    }
    public void SettingsButton()
    {
        ChangeActives("SettingsMenu");
    }
    
    public void ExitButton()
    {
        Application.Quit();
    }
    public void UndoButton()
    {
        ChangeActives("MainMenu");
    }
    public void DashButton()
    {
        GameObject.Find("Triangle").GetComponent<PlayerController>().Dash();
    }
    public void HealButton()
    {
        GameObject.Find("Triangle").GetComponent<PlayerController>().Heal(false);
    }
    public void PauseButton()
    {
        pauseMenu.SetActive(true);
        Time.timeScale = 0;
    }
    public void ResumeButton()
    {
        Time.timeScale = 1;
        pauseMenu.SetActive(false);
    }
    public void MainMenuButton()
    {
        SceneManager.LoadScene(0);
    }
    public void GameOverButton()
    {
        foreach (var gameObjects in GameObject.FindGameObjectsWithTag("Enemy"))
        {
            if (!gameObjects.activeSelf)
            {
                gameObjects.SetActive(true);
            }
        }

        if(player != null) 
        {
            PlayerController controller = player.GetComponent<PlayerController>();
            if (controller != null)
            {
                player.transform.position = controller.playerStartPos;
                player.SetActive(true);
                foreach (var hearth in controller.hearths)
                {
                    hearth.gameObject.SetActive(true);
                }
                controller.StartGameAgain();
            }
        }
        gameOverMenu.SetActive(false);
        
    }
    void ChangeActives(string menuName)
    {
        foreach (var menu in menus)
        {
            if(menu.name == menuName)
            {
                menu.SetActive(true);
            }
            else
            {
                menu.SetActive(false);
            }
        }
    }
    public void OnSliderValueChanged()
    {
        volumeSliderValueText.text = Mathf.RoundToInt(volumeSlider.value * 100).ToString();
    }

   
    

   
}
