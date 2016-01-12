using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using System;

public class ExNetworkManager : NetworkManager 
{
    public event Action<NetworkConnection> ServerConnected;

    public event Action<NetworkConnection> ClientConnected;

    public event Action<NetworkConnection> ClientDisconnected;

    public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId, NetworkReader extraMessageReader)
    {
//        base.OnServerAddPlayer(conn, playerControllerId, extraMessageReader);
        Debug.Log("OnServerAddPlayer " + conn.address);
    }

    #region Server
    public override void OnStartServer()
    {
        Debug.Log("OnStartServer");
        base.OnStartServer();
    }

    // called when a client connects 
    public override void OnServerConnect(NetworkConnection conn)
    {
        Debug.Log("OnServerConnect " + conn.address);
        base.OnServerConnect(conn);

        if (this.ServerConnected != null)
        {
            this.ServerConnected(conn);
        }
    }

    // called when a client disconnects
    public override void OnServerDisconnect(NetworkConnection conn)
    {
        Debug.Log("OnServerDisconnect " + conn.address);
        base.OnServerDisconnect(conn);
    }

    // called when a client is ready
    public override void OnServerReady(NetworkConnection conn)
    {
        Debug.Log("OnServerReady " + conn.address);

        base.OnServerReady(conn);
    }

    // called when a new player is added for a client
    public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId)
    {
        Debug.Log("OnServerAddPlayer " + conn.address);
        base.OnServerAddPlayer(conn, playerControllerId);
    }

    public override void OnServerRemovePlayer(NetworkConnection conn, PlayerController player)
    {
        Debug.Log("OnServerRemovePlayer " + conn.address);

        base.OnServerRemovePlayer(conn, player);
    }

    // called when a network error occurs
    public override void OnServerError(NetworkConnection conn, int errorCode)
    {
        Debug.Log("OnServerError " + conn.address + " errorCode: " + errorCode);
        base.OnServerError(conn, errorCode);
    }
    #endregion

    #region Client
    public override void OnClientConnect(NetworkConnection conn)
    {
        Debug.Log("Client connected");

        base.OnClientConnect(conn);

        if (this.ClientConnected != null)
        {
            this.ClientConnected(conn);
        }
    }

    public override void OnClientDisconnect(NetworkConnection conn)
    {
        Debug.Log("Client disconnected");

        base.OnClientDisconnect(conn);

        if (this.ClientDisconnected != null)
        {
            this.ClientDisconnected(conn);
        }
    }
   
    // called when a network error occurs
    public override void OnClientError(NetworkConnection conn, int errorCode)
    {
        Debug.Log("OnClientError " + errorCode);

        base.OnClientError(conn, errorCode);
    }

    // called when told to be not-ready by a server
    public override void OnClientNotReady(NetworkConnection conn)
    {
        Debug.Log("OnClientNotReady");
        base.OnClientNotReady(conn);
    }
    #endregion
}