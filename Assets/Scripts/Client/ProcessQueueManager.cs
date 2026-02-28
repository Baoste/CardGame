using Game.Domain;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProcessQueueManager : MonoBehaviour
{
    private static ProcessQueueManager instance;
    private Queue<IEventProcess> processQueue = new Queue<IEventProcess>();
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
    public void Enqueue(IEventProcess processable)
    {
        processQueue.Enqueue(processable);

        if (!isProcessing)
        {
            StartCoroutine(ProcessQueue());
        }
    }

    // 处理队列的协程
    private System.Collections.IEnumerator ProcessQueue()
    {
        isProcessing = true;

        while (processQueue.Count > 0)
        {
            IEventProcess current = processQueue.Dequeue();
            current.Process();

            yield return null;
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
