using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using Network;
using System;

public class Lobby : MonoBehaviour, IMessageReceiver
{
    //Set through Unity
    public GameObject MainPage;
    public GameObject HostPage;
    public GameObject ClientPage;
    //

    Allegiance allegiance;

    void Start()
    {
        this.allegiance = Allegiance.Security;

        CoreNetwork.Instance.Client_ConnectedToHosts += this.OnClientConnected;
        CoreNetwork.Instance.Host_ClientDisconnected += this.OnHostClientDisconnected;
        CoreNetwork.Instance.Subscribe(this);
    }

    void Update()
    {

    }

    void OnDestroy()
    {
        CoreNetwork.Instance.Client_ConnectedToHosts -= this.OnClientConnected;
        CoreNetwork.Instance.Host_ClientDisconnected -= this.OnHostClientDisconnected;

        CoreNetwork.Instance.Unsubscribe(this);
    }

    public void HostAsSecurity()
    {
        CoreNetwork.Instance.HostAsSecurityAndBroadcast();
        this.ShowHostPage();
    }

    public void HostAsPirates()
    {
        CoreNetwork.Instance.HostAsPiratesAndBroadcast();
        this.ShowHostPage(true);
        this.allegiance = Allegiance.Pirates;
    }

    public void ConnectAsClient()
    {
        CoreNetwork.Instance.ListenAsClientAndConnectToHosts();
        this.ShowClientPage();
    }

    #region Server

    //Set through Unity
    public Image[] SecurityStationsIndicators;
    public Image[] PiratesStationsIndicatiors;

    public Image SecurityShipIndicator;
    public Image PirateShipIndicator;
    //

    int[] securityStationsTaken;
    int[] piratesStationsTaken;

    void ShowHostPage(bool isPirate = false)
    {
        this.MainPage.SetActive(false);
        this.HostPage.SetActive(true);

        if (isPirate)
        {
            this.PirateShipIndicator.color = Color.green;
        }
        else
        {
            this.SecurityShipIndicator.color = Color.green;
        }

        this.securityStationsTaken = new int[] { -1, -1, -1};
        this.piratesStationsTaken = new int[] { -1, -1, -1};
    }

    void ReceivedStationSelect(int connectionId, StationSelectMsg stationSelect)
    {
        this.RemoveCurrentSelectionOfClient(connectionId);

        this.TakeStation(stationSelect.Allegiance, stationSelect.Station, connectionId);
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
        SceneManager.LoadScene("BattleScene");
    }

    #endregion

    #region Client

    //Set through Unity
    public GameObject TryingToConnect;
    public GameObject SelectionPage;
    //

    Stations stationSelected;

    void ShowClientPage()
    {
        this.MainPage.SetActive(false);
        this.ClientPage.SetActive(true);
    }

    void OnClientConnected()
    {
        this.TryingToConnect.SetActive(false);
        this.SelectionPage.SetActive(true);

        this.OnStationDropdownChanged(0);
    }

    public void OnStationDropdownChanged(int changedTo)
    {
        this.stationSelected = (Stations) changedTo;
        CoreNetwork.Instance.Send( new StationSelectMsg( this.allegiance,  this.stationSelected ));
    }

    public void OnAllegianceDropdownChanged(int changedTo)
    {
        this.allegiance = (Allegiance) changedTo;
        CoreNetwork.Instance.Send( new StationSelectMsg( this.allegiance,  this.stationSelected ));
    }

    #endregion

    #region IMessageReceiver implementation

    public void ReceiveMsg(int connectionId, MessageBase msg)
    {
        var stationSelect = msg as StationSelectMsg;
        if (stationSelect != null)
        {
            this.ReceivedStationSelect(connectionId, stationSelect);
        }

    }
    #endregion
}
