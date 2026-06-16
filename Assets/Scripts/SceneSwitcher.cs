using FishNet;
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
        if (InstanceFinder.NetworkManager == null)
        {
            Debug.LogError("FishNet NetworkManager not found.");
            return;
        }

        InstanceFinder.NetworkManager.TransportManager.Transport.SetClientAddress("49.232.222.222");
        //InstanceFinder.NetworkManager.TransportManager.Transport.SetClientAddress("localhost");
        // nm.TransportManager.Transport.SetClientAddress("localhost");
        InstanceFinder.ClientManager.StartConnection();

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
        AccountData account0 = (AccountData)parameters[0];
        AccountData account1 = (AccountData)parameters[1];

        shotPlayer.PlayShot(4);

        if (account0.AccountId != ChipSkinConfig.myAccountData.AccountId)
        {
            ChipSkinConfig.opponentAccountData = account0;
        }
        else
        {
            ChipSkinConfig.opponentAccountData = account1;
        }

        StartCoroutine(_JoinMatch());
    }

    private IEnumerator _JoinMatch()
    {
        yield return new WaitForSeconds(11f);
        FindAnyObjectByType<StartSceneBootstrap>().SwitchToGameScene("ClientTest_Yifan_v4");
    }
}
