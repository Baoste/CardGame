using UnityEngine;
using Cinemachine;

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

    public KeyCode nextKey = KeyCode.C;

    public int activePriority = 20;
    public int inactivePriority = 0;

    private int currentIndex = 0;

    private void Start()
    {
        InitShots();

        SetActiveShot(0);
        //PlayShotFromStart(0);
    }

    private void Update()
    {
        if (Input.GetKeyDown(nextKey))
        {
            PlayNextShot();
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
}