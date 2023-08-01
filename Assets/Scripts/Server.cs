using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Unity.Collections;

public class Server : MonoBehaviour
{

    private NetworkManager network;

    void Start()
    {
        network = FindObjectOfType<NetworkManager>();
        network.OnClientConnectedCallback += OnConnected;
        network.StartServer();

        network.CustomMessagingManager.RegisterNamedMessageHandler("action", (ulong client_id, FastBufferReader reader) =>
        {
            OnReceive(client_id, reader);
        });

        Debug.Log("Server Listening...");
    }

    private void OnConnected(ulong client_id)
    {
        Send(client_id);
    }

    private void OnReceive(ulong client_id, FastBufferReader reader)
    {
         Send(client_id);
    }

    private void Send(ulong client_id)
    {
        Debug.Log("Server Sending...");

        Data data = new Data();
        data.success = true;
        data.value = "test";

        FastBufferWriter writer = new FastBufferWriter(8, Allocator.Temp, 1024);
        writer.WriteNetworkSerializable(data);
        network.CustomMessagingManager.SendNamedMessage("refresh", client_id, writer);
        writer.Dispose();
    }
}

//Just some placeholder data
public class Data : INetworkSerializable
{
    public bool success;
    public string value;

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref success);
        serializer.SerializeValue(ref value);
    }
}