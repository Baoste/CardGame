using System.Collections;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(TMP_Text))]
public class SlotNumberTMP : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private TMP_Text tmpText;

    [Header("Number")]
    [SerializeField] private int minDigits = 1;
    [SerializeField] private string prefix = "";
    [SerializeField] private string suffix = "";

    [Header("Slot Effect")]
    [SerializeField] private float rollDuration = 0.6f;
    [SerializeField] private float digitStopDelay = 0.08f;
    [SerializeField] private float tickInterval = 0.03f;
    [SerializeField] private bool useUnscaledTime = true;

    [Header("Events")]
    public UnityEvent onTick;
    public UnityEvent onFinish;

    private int currentValue;
    private Coroutine rollCoroutine;
    private readonly StringBuilder builder = new StringBuilder();

    private void Awake()
    {
        if (tmpText == null)
            tmpText = GetComponent<TMP_Text>();

        SetImmediate(ChipSkinConfig.myAccountData.ChipCount);
    }

    public void SetImmediate(int value)
    {
        currentValue = value;

        if (rollCoroutine != null)
        {
            StopCoroutine(rollCoroutine);
            rollCoroutine = null;
        }

        tmpText.text = FormatFinalNumber(value);
    }

    public void RollTo(int newValue)
    {
        if (currentValue == newValue)
            return;

        if (rollCoroutine != null)
            StopCoroutine(rollCoroutine);

        rollCoroutine = StartCoroutine(RollRoutine(currentValue, newValue));
        currentValue = newValue;
    }

    private IEnumerator RollRoutine(int fromValue, int toValue)
    {
        int absFrom = Mathf.Abs(fromValue);
        int absTo = Mathf.Abs(toValue);

        string fromStr = absFrom.ToString();
        string toStr = absTo.ToString();

        int digitCount = Mathf.Max(minDigits, fromStr.Length, toStr.Length);

        toStr = toStr.PadLeft(digitCount, '0');

        float totalDuration = rollDuration + digitStopDelay * digitCount;
        float timer = 0f;
        float tickTimer = 0f;

        while (timer < totalDuration)
        {
            float delta = useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;

            timer += delta;
            tickTimer += delta;

            if (tickTimer >= tickInterval)
            {
                tickTimer = 0f;
                UpdateSlotText(toValue, toStr, digitCount, timer);
                onTick?.Invoke();
            }

            yield return null;
        }

        tmpText.text = FormatFinalNumber(toValue);

        rollCoroutine = null;
        onFinish?.Invoke();
    }

    private void UpdateSlotText(int targetValue, string targetStr, int digitCount, float timer)
    {
        builder.Clear();

        builder.Append(prefix);

        if (targetValue < 0)
            builder.Append("-");

        for (int i = 0; i < digitCount; i++)
        {
            float stopTime = rollDuration + digitStopDelay * i;

            if (timer >= stopTime)
            {
                builder.Append(targetStr[i]);
            }
            else
            {
                int randomDigit = Random.Range(0, 10);
                builder.Append(randomDigit);
            }
        }

        builder.Append(suffix);

        tmpText.text = builder.ToString();
    }

    private string FormatFinalNumber(int value)
    {
        bool isNegative = value < 0;
        int absValue = Mathf.Abs(value);

        string number = absValue.ToString().PadLeft(minDigits, '0');

        if (isNegative)
            number = "-" + number;

        return prefix + number + suffix;
    }
}