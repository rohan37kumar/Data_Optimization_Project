using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    [SerializeField] private Button restartButton;

    void Start()
    {
        restartButton.onClick.AddListener(RestartLevel);
    }

    void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);   //for this project wrote simple restart, better implementation would be using a GameManager
    }
}
