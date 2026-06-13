using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlotMachine : MonoBehaviour
{
    [SerializeField] private Animator animator;

    private void Start()
    {
        gameObject.SetActive(false);        
    }

    public IEnumerator PlayAnimation(bool reveal)
    {
        AudioManager.Instance.Play("SlotMachineAppear");

        Quaternion quaternion = transform.rotation;

        Sequence seq = DOTween.Sequence();
        seq.Append(transform.DORotate(Vector3.zero, 0.8f).SetEase(Ease.OutBounce));
        yield return seq.WaitForCompletion();

        // TODO: 显示随机引爆装置爆炸动画
        if (reveal)
        {
            animator.Play("Win");
        }
        else
        {
            animator.Play("Lose");
        }
        yield return null; // 等一帧保证播放
        // 获取当前 clip 时长
        var clips = animator.GetCurrentAnimatorClipInfo(0);
        if (clips.Length > 0)
        {
            float clipLength = clips[0].clip.length;
            yield return new WaitForSeconds(clipLength);
        }

        transform.DORotateQuaternion(quaternion, 0.3f).SetEase(Ease.InBack);
        yield return new WaitForSeconds(0.3f);
        animator.Play("Start");
        yield return null; // 等一帧保证播放
    }
}
