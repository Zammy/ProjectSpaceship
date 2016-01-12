using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class StartScene : NetworkBehaviour 
{
    //Set through Unity
    public ExNetworkManager NetworkManager;
    public InputField IP;

    public GameObject MainPage;
    public GameObject HostPage;
    //

    void Start()
    {
        this.NetworkManager.ClientConnected += this.OnClientConnected ;
        this.NetworkManager.ServerConnected += this.OnServerConnected ;
    }

    void OnDestroy()
    {
    }

    public void ConnectAsClient()
    {
        if (this.IP.text != "")
        {
            this.NetworkManager.networkAddress = this.IP.text;
        }
        this.NetworkManager.StartClient();
    }

    public void Host()
    {
        this.NetworkManager.StartServer();

        this.MainPage.SetActive(false);
        this.HostPage.SetActive(true);
    }

    void OnServerConnected(NetworkConnection conn)
    {
        Debug.Log("Loading BattleScene");
        SceneManager.LoadScene("BattleScene");
    }

    void OnClientConnected(NetworkConnection conn)
    {
        Debug.Log("Loading PilotStation");
        SceneManager.LoadScene("PilotStation");
    }
}
