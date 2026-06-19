using Game.Domain;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class ProcessQueueManager : MonoBehaviour
{
    private static ProcessQueueManager instance;
    private Queue<Func<object[], IEnumerator>> processQueue = new();
    private Queue<object[]> paramsQueue = new Queue<object[]>();
    private Queue<float> delayQueue = new Queue<float>();
    private bool isProcessing = false;

    public static ProcessQueueManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<ProcessQueueManager>();

                if (instance == null)
                {
                    GameObject go = new GameObject("ProcessQueueManager");
                    instance = go.AddComponent<ProcessQueueManager>();
                    DontDestroyOnLoad(go); // 场景切换时不销毁
                }
            }
            return instance;
        }
    }

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    // 添加到队列
    public void Enqueue(Func<object[], IEnumerator> processable, object[] paramList, float delay)
    {
        processQueue.Enqueue(processable);
        paramsQueue.Enqueue(paramList);
        delayQueue.Enqueue(delay);

        if (!isProcessing)
        {
            StartCoroutine(ProcessQueue());
        }
    }

    // 处理队列的协程
    private IEnumerator ProcessQueue()
    {
        isProcessing = true;

        while (processQueue.Count > 0)
        {
            Func<object[], IEnumerator> current = processQueue.Dequeue();
            object[] parameters = paramsQueue.Dequeue();
            float delay = delayQueue.Dequeue();

            if (delay > 0f)
                yield return new WaitForSeconds(delay);

            yield return current(parameters);
        }

        isProcessing = false;
    }

    // 清空队列
    public void ClearQueue()
    {
        processQueue.Clear();
        isProcessing = false;
        StopAllCoroutines();
    }

}
