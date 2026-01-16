using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using Unity.Netcode;

public enum MenuType
{
    Pause,
    Death
}
public class InGameMenu : MonoBehaviour
{
    private PlayerContoller _contoller;
    [SerializeField] private GameObject menuUI;
    [SerializeField] private MenuType menuType;
    void Awake()
    {
        _contoller = GetComponentInParent<PlayerContoller>();
    }

    public void OnOpenMenu(InputAction.CallbackContext context)
    {
        if (menuUI.activeSelf && menuType == MenuType.Pause)
        {
            menuUI.SetActive(false);
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            _contoller.enabled = true;
            _contoller.GetComponentInChildren<Hip>().enabled = true;
        }
        else if(menuType == MenuType.Pause)
        {
            menuUI.SetActive(true);
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            _contoller.enabled = false;
            _contoller.GetComponentInChildren<Hip>().enabled = false;
        }
    }

    public void closeMenu()
    {
        //close menu and unfreze movment on button click
        print("CLose?");
        menuUI.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        _contoller.enabled = true;
        _contoller.GetComponentInChildren<Hip>().enabled = false;
    }
    public void backToMainMenu()
    {
        //goes back to start screen
        NetworkManager.Singleton.DisconnectClient(NetworkManager.Singleton.LocalClientId);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
        
    }

    public void Spectate()
    {

    }
}
