using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Based on: CustomClient.cs
public class ModuleClient : Client
{
    [SerializeField] private Button _startClientButton;
    [SerializeField] private Button _pingServerButton;
    private void Awake()
    {
        _startClientButton.onClick.AddListener(base.StartClient);

        _pingServerButton.interactable = false;
        _pingServerButton.onClick.AddListener(SendMessageToServer);

        //Populate Client delegates
        OnClientStarted = () =>
        {
            _startClientButton.interactable = false;
            _pingServerButton.interactable = true;
        };

        OnClientClosed = () =>
        {
            _startClientButton.interactable = true;
            _pingServerButton.interactable = false;
        };
    }

    private void SendMessageToServer()
    {
        string newMsg = "I'm a client!";
        if (string.IsNullOrEmpty(newMsg))
        {
            Debug.Log("Client: tried to send empty message");
            return;
        }
        base.SendMessageToServer(newMsg);
    }
}
