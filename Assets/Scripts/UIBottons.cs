using UnityEngine;
using UnityEngine.SceneManagement;

public class UIBottons : MonoBehaviour
{

    public void LoadMainScane()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(0);
    }
}
