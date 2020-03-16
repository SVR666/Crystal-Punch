using UnityEngine;
using UnityEngine.SceneManagement;

public class scene_changer : MonoBehaviour
{
    public void changescene(string scenename)
    {
        SceneManager.LoadScene(scenename);
    }
}
