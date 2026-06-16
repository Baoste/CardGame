using UnityEngine;
using Cinemachine;
using System.Collections;
using Unity.VisualScripting;

[System.Serializable]
public class Shot
{
    public CinemachineVirtualCamera vcam;
    public Animator[] animators;
    public string animState = "Base Layer.Enter";
}

public class ShotPlayer : MonoBehaviour
{
    public Shot[] shots;
    public CinemachineBrain brain;
    public GameObject mainMenuCanvas;

    public KeyCode nextKey = KeyCode.C;

    public int activePriority = 20;
    public int inactivePriority = 0;

    public int currentIndex = 0;
    private bool isPlayingShot1 = false;

    private void Start()
    {
        InitShots();

        if (GameBootstrap.isLogin)
        {
            FindAnyObjectByType<Cam2>().EnablePlayControll();
        }
        else
        {
            SetActiveShot(0);
            //StartCoroutine(PlayShotWithDelay(1, 1f));
            PlayShotFromStart(0);
            mainMenuCanvas.SetActive(true);
        }
    }

    private void Update()
    {
        //if (Input.GetKeyDown(nextKey))
        //{
        //    PlayNextShot();
        //}
        if (Input.GetMouseButtonDown(0) && currentIndex == 1 && !isPlayingShot1)
        {
            shots[1].animators[0].speed = 6;
            isPlayingShot1 = true;
        }
    }

    public void PlayNextShot()
    {
        int nextIndex = currentIndex + 1;

        if (nextIndex >= shots.Length)
            nextIndex = 0;

        PlayShot(nextIndex);

        Debug.Log("Playing shot: " + nextIndex);
    }

    public void PlayShot(int index)
    {
        if (index < 0 || index >= shots.Length) return;

        PrepareShotAtStart(index);
        SetActiveShot(index);
        PlayShotFromStart(index);

        currentIndex = index;
    }

    private void InitShots()
    {
        for (int i = 0; i < shots.Length; i++)
        {
            shots[i].vcam.Priority = inactivePriority;

            foreach (Animator anim in shots[i].animators)
            {
                if (anim == null) continue;

                anim.enabled = false;
                anim.speed = 1f;
            }
        }
    }

    private void SetActiveShot(int index)
    {
        for (int i = 0; i < shots.Length; i++)
        {
            shots[i].vcam.Priority = i == index ? activePriority : inactivePriority;
        }
    }

    private void PrepareShotAtStart(int index)
    {
        foreach (Animator anim in shots[index].animators)
        {
            if (anim == null) continue;

            anim.enabled = true;
            anim.speed = 0f;

            anim.Rebind();
            anim.Update(0f);

            anim.Play(shots[index].animState, 0, 0f);
            anim.Update(0f);
        }
    }

    private void PlayShotFromStart(int index)
    {
        foreach (Animator anim in shots[index].animators)
        {
            if (anim == null) continue;

            anim.enabled = true;
            anim.speed = 1f;

            anim.Play(shots[index].animState, 0, 0f);
            anim.Update(0f);
        }
    }

    IEnumerator PlayShotWithDelay(int index, float delay)
    {
        yield return new WaitForSeconds(delay);
        PlayShot(index);
    }
}