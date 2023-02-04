using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Based on CustomServer.cs
public class ModuleServer : Server
{
    [SerializeField] private Button _startServerButton;
    [SerializeField] private Button _pingClientButton;

    private bool serverConnected = false;
    private bool clientConnected = false;

    protected virtual void Awake()
    {
        _startServerButton.interactable = true;
        _startServerButton.onClick.AddListener(StartServer);

        _pingClientButton.interactable = false;
        _pingClientButton.onClick.AddListener(SendMessageToClient);

        //Populate Server delegates
        OnClientConnected = () => { clientConnected = true; };
        OnClientDisconnected = () => { clientConnected = false; };
        OnServerClosed = () => { serverConnected = false; };
        OnServerStarted = () => { serverConnected = true; };
    }

    protected override void Update()
    {
        base.Update();

        //Interactables needs to be setted on Update case can be called from a non-main thread
        _startServerButton.interactable = !serverConnected;
        _pingClientButton.interactable = clientConnected;
    }

    private void SendMessageToClient()
    {
        string newMsg = "PING!";
        if (string.IsNullOrEmpty(newMsg))
        {
            Debug.Log("Server: tried to send empty message to client");
            return;
        }
        base.SendMessageToClient(newMsg);
    }
}
