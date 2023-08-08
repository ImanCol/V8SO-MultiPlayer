using System;
using UnityEngine;

public class PlayerMemoryReader : MonoBehaviour
{
    public bool Get = false;

    public int ID = 1;
    public Player2[] players;

    private void Start()
    {
        //ReadPlayerData();
    }

    private void Update()
    {
        if (Get)
        {
            ReadPlayerData();
        }
    }

    private void ReadPlayerData()
    {
        for (int i = 0; i < players.Length; i++)
        {
            //int playerInGameID = GameManager.GetPlayerInGameID(ID);
            //int playerInGameID = GetPlayerInGameID(i);
            players[ID].Address = LegacyMemoryReader.ReadUInt32((uint)(-2147072884 + ID * 16));
            players[ID].healthAddr1 = LegacyMemoryReader.ReadUInt32((players[ID].Address + 248));
            players[ID].healthAddr2 = LegacyMemoryReader.ReadUInt32((players[ID].Address + 252));
            players[ID].mgAddr = LegacyMemoryReader.ReadUInt32((players[ID].Address + 284));
            players[ID].Trailer = GameManager.GetTrailerAddress((players[ID].Address));
            players[ID].Camera = LegacyMemoryReader.ReadUInt32((players[ID].Address + 236));

            Debug.Log("Player " + i + " Address: 0x" + players[ID].Address.ToString("X"));
            Debug.Log("Health Address 1: 0x" + players[ID].healthAddr1.ToString("X"));
            Debug.Log("Health Address 2: 0x" + players[ID].healthAddr2.ToString("X"));
            Debug.Log("MG Address: 0x" + players[ID].mgAddr.ToString("X"));
            Debug.Log("Trailer Address: 0x" + players[ID].Trailer.ToString("X"));
            Debug.Log("Camera Address: 0x" + players[ID].Camera.ToString("X"));
        }
    }

    public static int GetPlayerInGameID(int id)
    {
        if (id > Client.instance.myId)
        {
            return id - 1;
        }
        return id;
    }

    // private int GetPlayerInGameID(int index)
    // {
    //     // Lógica para obtener el ID del jugador en el juego
    //     // Implementa tu propia lógica aquí
    //     
    //     return index + 1;
    // }
}

[Serializable]
public class Player2
{
    public uint Address;
    public uint healthAddr1;
    public uint healthAddr2;
    public uint mgAddr;
    public uint Trailer;
    public uint Camera;
}
