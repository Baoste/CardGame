using Game.Domain;
using UnityEngine;

public class UIClickSuggest : MonoBehaviour
{
    [SerializeField] private GameObject[] zones;
    [SerializeField] private GameObject[] oddEven;

    public void Show(int id)
    {
        if (id == (int)ParticipantType.MyBoardZone || id == (int)ParticipantType.OppentBoardZone)
        {
            foreach (GameObject obj in zones)
            {
                obj.SetActive(true);
            }
        }
        else
        {
            foreach (GameObject obj in oddEven)
            {
                obj.SetActive(true);
            }
        }
    }

    public void Hide()
    {
        foreach (GameObject obj in zones)
        {
            obj.SetActive(false);
        }
        foreach (GameObject obj in oddEven)
        {
            obj.SetActive(false);
        }
    }
}
