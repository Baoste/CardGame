using FishNet.Managing;
using Game.Domain;
using Newtonsoft.Json;
using System.Collections;
using UnityEngine;

public class SceneSwitcher : MonoBehaviour
{
    private NetworkManager nm;
    public ShotPlayer shotPlayer;

    private void Start()
    {
        nm = FindAnyObjectByType<NetworkManager>();
        nm.TransportManager.Transport.SetClientAddress("49.232.222.222");
        //nm.TransportManager.Transport.SetClientAddress("localhost");
        nm.ClientManager.StartConnection();

        ProcessDispatcher.Register("BothJoinMatch", BothJoinMatch);
    }

    public void SwitchScene()
    {
        GetCardDeckCommand cmd = new GetCardDeckCommand { playerId = -1 };
        ClientGameState.gateway.SendCommandServerRpc("GetCardDeck", JsonConvert.SerializeObject(cmd));

        MatchData.Instance.matchSeed = 123;
        MatchData.Instance.matchSeed = Random.Range(0, 9999999);
        ClientCommand.CreateMatch(MatchData.Instance.matchId);
    }

    public void BothJoinMatch(object[] parameters)
    {
        shotPlayer.PlayShot(4);
        StartCoroutine(_JoinMatch());
    }

    private IEnumerator _JoinMatch()
    {
        yield return new WaitForSeconds(11f);
        FindAnyObjectByType<StartSceneBootstrap>().SwitchToGameScene("ClientTest_Yifan_v4");
    }
}
