using System;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.Match;
using UnityEngine.UI;

public class CustomNetworkManager : NetworkManager
{
    //public void StartUpHost()
    //{
    //    SetPort();
    //    NetworkManager.singleton.StartHost();
    //}

    //public void JoinGame()
    //{
    //    SetIPAddress();
    //    SetPort();
    //    NetworkManager.singleton.StartClient();
    //}

    //void SetIPAddress()
    //{
    //    string ipAddress = GameObject.Find("InputFieldIPAddress").transform.GetComponentInChildren<Text>().text;
    //    NetworkManager.singleton.networkAddress = ipAddress;
    //}

    //void SetPort()
    //{
    //    NetworkManager.singleton.networkPort = 7777;
    //}

    //private void OnLevelWasLoaded(int level)
    //{
    //    Debug.Log(level);
    //    if (level == 2)
    //    {
    //        SetupMenuSceneButtons();
    //    }
    //    else if (level == 3)
    //    {
    //        SetupGameSceneButtons();
    //    }
    //    else
    //    {
    //        SetupNothing();
    //    }
    //}

    //private void SetupNothing()
    //{
    //    throw new NotImplementedException();
    //}

    //private void SetupMenuSceneButtons()
    //{
    //    GameObject.Find("ButtonStartHost").GetComponent<Button>().onClick.RemoveAllListeners();
    //    GameObject.Find("ButtonStartHost").GetComponent<Button>().onClick.AddListener(StartUpHost);

    //    GameObject.Find("ButtonJoinGame").GetComponent<Button>().onClick.RemoveAllListeners();
    //    GameObject.Find("ButtonJoinGame").GetComponent<Button>().onClick.AddListener(JoinGame);
    //}

    //private void SetupGameSceneButtons()
    //{
    //    GameObject.Find("ButtonDisconnect").GetComponent<Button>().onClick.RemoveAllListeners();
    //    GameObject.Find("ButtonDisconnect").GetComponent<Button>().onClick.AddListener(NetworkManager.singleton.StopHost);
    //}
}
