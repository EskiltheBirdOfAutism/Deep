using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class InGameMenu : MonoBehaviour
{
    private PlayerContoller _contoller;
    [SerializeField] private GameObject menuUI;
    void Awake()
    {
        _contoller = GetComponentInParent<PlayerContoller>();
    }

    public void OnOpenMenu(InputAction.CallbackContext context)
    {
        if (menuUI.activeSelf)
        {
            menuUI.SetActive(false);
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            _contoller.enabled = true;
            _contoller.GetComponentInChildren<Hip>().enabled = true;
        }
        else
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
        menuUI.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        _contoller.enabled = true;
        _contoller.GetComponentInChildren<Hip>().enabled = false;
    }
    public void backToMainMenu()
    {
        //goes back to start screen
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
    }
}
