using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;
using Unity.Collections;

public class Game : MonoBehaviour
{
    public Text text;

    private NetworkManager network;
    private bool connected = false;

    void Start()
    {
        network = FindObjectOfType<NetworkManager>();
        text.text = "";
    }

    public void OnClickOK()
    {
        if (!connected)
            Connect();
        else
            Send();
    }

    private void Connect()
    {
        Debug.Log("Connecting...");
        text.text = "Connecting...";

        network.StartClient();
        connected = true;

        network.CustomMessagingManager.RegisterNamedMessageHandler("refresh", (ulong client_id, FastBufferReader reader) =>
        {
            OnReceive("test", client_id, reader);
        });
    }

    private void Send()
    {
        Debug.Log("Sending...");
        text.text = "Sending...";

        FastBufferWriter writer = new FastBufferWriter(0, Allocator.Temp, 1024);
        network.CustomMessagingManager.SendNamedMessage("action", NetworkManager.ServerClientId, writer, NetworkDelivery.Reliable);
    }
    
    private void OnReceive(string type, ulong client_id, FastBufferReader reader)
    {
        Debug.Log("Receiving...");
        text.text = "Receiving...";

        reader.ReadNetworkSerializable<Data>(out Data data);

        Debug.Log("Data: " + data.value);
        text.text = "Data: " + data.value;

    }
}
