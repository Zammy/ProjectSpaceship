using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using Networking;
using System;

public class Lobby : MonoBehaviour, IMessageReceiver
{
    //Set through Unity
    public GameObject MainPage;
    public GameObject HostPage;
    public GameObject ClientPage;
    //

    void Start()
    {
        Global.Allegiance = Allegiance.Security;

        CoreNetwork.Instance.Client_ConnectedToHost += this.OnClientConnected;
        CoreNetwork.Instance.Host_ClientDisconnected += this.OnHostClientDisconnected;
        CoreNetwork.Instance.Subscribe(this);
    }

    void Update()
    {

    }

    void OnDestroy()
    {
        CoreNetwork.Instance.Client_ConnectedToHost -= this.OnClientConnected;
        CoreNetwork.Instance.Host_ClientDisconnected -= this.OnHostClientDisconnected;

        CoreNetwork.Instance.Unsubscribe(this);
    }

    public void Host()
    {
        CoreNetwork.Instance.HostAndBroadcast();
        this.ShowHostPage();
    }

    public void ConnectAsClient()
    {
        CoreNetwork.Instance.ListenAsClientAndConnectToHost();
        this.ShowClientPage();
    }

    #region Server

    //Set through Unity
    public Image[] SecurityStationsIndicators;
    public Image[] PiratesStationsIndicatiors;

    public Text IPAddress;
    //

    int[] securityStationsTaken;
    int[] piratesStationsTaken;

    void ShowHostPage()
    {
        this.IPAddress.text = Network.player.ipAddress;

        this.MainPage.SetActive(false);
        this.HostPage.SetActive(true);

        this.securityStationsTaken = new int[] { -1, -1, -1};
        this.piratesStationsTaken = new int[] { -1, -1, -1};
    }

    void ReceivedStationSelect(int connectionId, StationSelectMsg stationSelect)
    {
        this.RemoveCurrentSelectionOfClient(connectionId);

        this.TakeStation(stationSelect.allegiance, stationSelect.station, connectionId);
    }

    void RemoveCurrentSelectionOfClient(int connectionId)
    {
        for (int i = 0; i < securityStationsTaken.Length; i++)
        {
            if (securityStationsTaken[i] == connectionId)
            {
                this.FreeStation(Allegiance.Security, (Stations)i);
                break;
            }
        }
        for (int i = 0; i < piratesStationsTaken.Length; i++)
        {
            if (piratesStationsTaken[i] == connectionId)
            {
                this.FreeStation(Allegiance.Pirates, (Stations)i);
                break;
            }
        }
    }

    void TakeStation(Allegiance all, Stations station, int connectionId)
    {
        Image[] stationIndictators;
        int[] stationsTaken;
        if (all == Allegiance.Pirates)
        {
            stationIndictators = PiratesStationsIndicatiors;
            stationsTaken = piratesStationsTaken;
        }
        else
        {
            stationIndictators = SecurityStationsIndicators;
            stationsTaken = securityStationsTaken;
        }

        stationIndictators[ (int) station].color = Color.green;
        stationsTaken[ (int) station ] = connectionId;
    }

    void FreeStation(Allegiance all, Stations station)
    {
        Image[] stationIndictators;
        int[] stationsTaken;
        if (all == Allegiance.Pirates)
        {
            stationIndictators = PiratesStationsIndicatiors;
            stationsTaken = piratesStationsTaken;
        }
        else
        {
            stationIndictators = SecurityStationsIndicators;
            stationsTaken = securityStationsTaken;
        }

        stationIndictators[ (int) station].color = Color.white;
        stationsTaken[ (int) station ] = -1; 
    }

    void OnHostClientDisconnected(int connectionId)
    {
        this.RemoveCurrentSelectionOfClient(connectionId);
    }

    public void RockOn()
    {
        CoreNetwork.Instance.StopHostBroadcast();

        CoreNetwork.Instance.Send( new StartBattleMsg() );

        SceneManager.LoadScene("BattleScene");
    }

    #endregion

    #region Client

    //Set through Unity
    public GameObject ConnectingPage;
    public GameObject TryingToConnect;
    public InputField HostIP;

    public GameObject SelectionPage;
    //

    Stations stationSelected;

//    bool connectedToHost = false;

    void ShowClientPage()
    {
        this.MainPage.SetActive(false);
        this.ClientPage.SetActive(true);
    }

    void OnClientConnected()
    {
//        this.connectedToHost = true;

        Debug.Log("Lobby.OnClientConnected()");

        this.HostIP.gameObject.SetActive(false);
  
        this.ConnectingPage.SetActive(false);
        this.SelectionPage.SetActive(true);

        this.OnStationDropdownChanged(0);
    }

    public void ManualConnect()
    {
        string ip = this.HostIP.text;
        CoreNetwork.Instance.ListenAsClientAndConnectToHost(ip);
    }

    public void OnStationDropdownChanged(int changedTo)
    {
        this.stationSelected = (Stations) changedTo;
        CoreNetwork.Instance.Send( new StationSelectMsg( Global.Allegiance,  this.stationSelected ));
    }

    public void OnAllegianceDropdownChanged(int changedTo)
    {
        Global.Allegiance = (Allegiance) changedTo;
        CoreNetwork.Instance.Send( new StationSelectMsg( Global.Allegiance,  this.stationSelected ));
    }

    #endregion

    #region IMessageReceiver implementation

    public void ReceiveMsg(INetMsg msg, int connectionId)
    {
        var stationSelect = msg as StationSelectMsg;
        if (stationSelect != null)
        {
            this.ReceivedStationSelect(connectionId, stationSelect);
        }

        var startBattleMsg = msg as StartBattleMsg;
        if (startBattleMsg != null)
        {
            CoreNetwork.Instance.StopHostBroadcast();

            switch (this.stationSelected)
            {
                case Stations.Pilot:
                {
                    SceneManager.LoadScene("PilotStation");
                    break;
                }
                case Stations.SensorsAndWeapons:
                {
                    SceneManager.LoadScene("SensorsAndWeaponsStation");
                    break;
                }
                default:
                    break;
            }
        }

    }
    #endregion
}
