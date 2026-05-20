using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartPlaySceneAnim : MonoBehaviour
{
    void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        StartCoroutine(StartAnim());
    }

    private IEnumerator StartAnim()
    {
        yield return new WaitForSeconds(1f);
        int seed = MatchData.Instance.matchSeed;
        ClientCommand.StartMatch(seed);
    }
}
