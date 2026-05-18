using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSwitcher : MonoBehaviour
{
    public void SwitchScene()
    {
        SceneManager.LoadScene("ClientTest_Yifan_v4");
    }
}
