using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Game_Manager : MonoBehaviour {

	public static Game_Manager Instance { set; get; }

    public GameObject mainMenu;
    public GameObject serverMenu;
    public GameObject connetMenu;

    public GameObject serverPrefab;
    public GameObject clientPrefab;

    public InputField nameInput;

    private void Start()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);
        serverMenu.SetActive(false);
        connetMenu.SetActive(false);
    }

    public void ConnectButton()
    {
        mainMenu.SetActive(false);
        connetMenu.SetActive(true);
    }
    public void HostButton()
    {
        try
        {
            Server s = Instantiate(serverPrefab).GetComponent<Server>();
            s.Init();

            Client c = Instantiate(clientPrefab).GetComponent<Client>();
            c.clientName = nameInput.text;
            c.isHost = true;
            if (c.clientName == "")
                c.clientName = "Host";
            c.ConnectToServer("127.0.0.1", 6321);
            
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
        }
        mainMenu.SetActive(false);
        serverMenu.SetActive(true);
    }
    public void ConnectToServerButton()
    {
        Debug.Log("here");
        string hostAddress = GameObject.Find("HostInput").GetComponent<InputField>().text;
        if (hostAddress == "")
        {
            hostAddress = "127.0.0.1";
        }
        try
        {
            Client c = Instantiate(clientPrefab).GetComponent<Client>();
            c.clientName = nameInput.text;
            if (c.clientName == "")
                c.clientName = "anonymous";
            c.ConnectToServer(hostAddress, 6321);
            connetMenu.SetActive(false);
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
        }
    }
    public void BackButton()
    {
        mainMenu.SetActive(true);
        serverMenu.SetActive(false);
        connetMenu.SetActive(false);

        Server s = FindObjectOfType<Server>();
        if (s != null)
        {
            Destroy(s.gameObject);
        }
        Client c = FindObjectOfType<Client>();
        if (c != null)
        {
            Destroy(c.gameObject);
        }
    }
    public void StartGame()
    {
        SceneManager.LoadScene("Game Multiplayer");
    }
}
