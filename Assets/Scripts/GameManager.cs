using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public static Dictionary<int, PlayerManager> players = new Dictionary<int, PlayerManager>();

    public static Queue<_Action> actions = new Queue<_Action>();

    public GameObject localPlayerPrefab;

    public GameObject playerPrefab;

    public GameObject playersState;

    public string identifier;

    private bool inMatch;

    private bool mapLoaded;

    private const uint globalObjPtr = 2147924016u;

    private int mapID;

    public UIManager uiManacerInstance;

    UIManager uIManager;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            UnityEngine.Debug.Log("Instance already exists, destroying object!");
            UnityEngine.Object.Destroy(this);
        }
    }

    private void Update()
    {
        if (InLevel() && LegacyMemoryReader.ReadByte(2147894407u) == 0 && LegacyMemoryReader.ReadInt16(2147894412u) == 0 && LegacyMemoryReader.ReadByte(2147894409u) == 0 && LegacyMemoryReader.ReadInt16(2147894414u) == 0 && actions.Count > 0)
        {
            _Action action = actions.Dequeue();
            LegacyMemoryReader.WriteByte(2147894403u, action.arg1);
            LegacyMemoryReader.WriteByte(2147894408u, action.arg2);
            LegacyMemoryReader.WriteInt16(2147894410u, action.arg3);
            LegacyMemoryReader.WriteByte(2147894404u, action.arg4);
            LegacyMemoryReader.WriteByte(2147894409u, action.a1);
            LegacyMemoryReader.WriteInt16(2147894414u, action.a2);
#if DEBUG
            Debug.Log(action.arg1 + " " + action.arg2 + " " + action.arg3 + " " + action.arg4 + " " + action.a1 + " " + action.a2);
#endif
        }
    }

    private void FixedUpdate()
    {
        if (LegacyMemoryReader.ReadUInt32(2147923996u) != 0 && LegacyMemoryReader.ReadByte(2147923992u) != 0)
        {
            if (!mapLoaded)
            {
                mapLoaded = true;
#if DEBUG
                Debug.Log("Calcular direccion Player...");
#endif
                SetPlayerAddresses();
                ClientSend.MapLoaded();
            }
        }
        else if (mapLoaded)
        {
            for (int i = 1; i <= players.Count; i++)
            {
                players[i].CompleteHandles();
                players[i].isWrecked = false;
            }
            mapLoaded = false;
            actions.Clear();
        }
        if (LegacyMemoryReader.ReadByte(2147919908u) == 0)
        {
            inMatch = true;
        }
        else
        {
            inMatch = false;
        }
    }

    private void SetPlayerAddresses()
    {
        if (!InLevel())
        {
            return;
        }
        Debug.Log("Cantidad de Players: " + players.Count);
        Debug.Log("myId Client: " + Client.instance.myId);

        for (int i = 1; i <= players.Count; i++)
        {
            Debug.Log("Buscando Player: " + i);
            if (i == Client.instance.myId)
            {
                players[i].Address = LegacyMemoryReader.ReadUInt32(2147923944u);
#if DEBUG
                Debug.Log("4-Pass: playersAddress: " + i + " - 0x8006B7E8: 0x" + players[i].Address.ToString("X"));
#endif
                continue;
            }
            int playerInGameID = GetPlayerInGameID(i);
#if DEBUG
            Debug.Log("1-Pass: playerInGameID: " + playerInGameID);
#endif
            players[i].Address = LegacyMemoryReader.ReadUInt32((uint)(-2147072884 + playerInGameID * 16));
#if DEBUG
            Debug.Log("2-Pass: playersAddress: " + i + " - 0x8006448C: 0x" + players[i].Address.ToString("X"));
#endif
        }
    }

    public void SpawnPlayer(int _id, string _username)
    {
        GameObject gameObject = (_id != Client.instance.myId) ? UnityEngine.Object.Instantiate(playerPrefab) : UnityEngine.Object.Instantiate(localPlayerPrefab);
        gameObject.GetComponent<PlayerManager>().Initialize(_id, _username);
        players.Add(_id, gameObject.GetComponent<PlayerManager>());
        if (_id >= 1 && _id <= 4)
        {
            int index = _id - 1;
            UIManager.instance.playerNames[index].text = _username;
            UIManager.instance.driverPanel[index].SetActive(true);
        }
#if DEBUG
        Debug.Log("Spawn Player: ID: " + _id + " User: " + _username);
#endif
        DiscordManager.instance.partySize = players.Count;
        DiscordManager.instance.partyMax = 5;
    }

    public void DestroyAll()
    {
        for (int i = 1; i <= players.Count; i++)
        {
            UnityEngine.Object.Destroy(players[i].gameObject);
            players.Remove(i);
            UIManager.instance.driverPanel[i].SetActive(false);
        }
    }

    public static void SetMap(int id)
    {
        instance.mapID = id;
    }

    public static int GetMap()
    {
        return instance.mapID;
    }

    public static int GetPlayerInGameID(int id)
    {
        if (id > Client.instance.myId)
        {
            return id - 1;
        }
        return id;
    }

    public static int ConvertGameID(int id)
    {
        if (id < Client.instance.myId)
        {
            return id;
        }
        return id + 1;
    }

    public static bool InMatch()
    {
        return instance.inMatch;
    }

    public static bool InLevel()
    {
        return instance.mapLoaded;
    }

    private void Start()
    {
        //Debug.Log("Load...");
        uiManacerInstance.usernameField.text = PlayerPrefs.GetString("usernameIndex", uiManacerInstance.usernameField.text);
        uiManacerInstance.externalProgramRunner.programPathInputField.text = PlayerPrefs.GetString("programPathIndex", uiManacerInstance.externalProgramRunner.programPathInputField.text);
        uiManacerInstance.externalProgramRunner.programArgumentsInputField.text = PlayerPrefs.GetString("programArgumentsIndex", uiManacerInstance.externalProgramRunner.programArgumentsInputField.text);
        uiManacerInstance.serverIPField.text = PlayerPrefs.GetString("serverIPIndex");
        uiManacerInstance.serverPORTField.text = PlayerPrefs.GetString("serverPORTIndex");
    }

    public void savePrefusername()
    {
        PlayerPrefs.SetString("usernameIndex", UIManager.instance.usernameField.text);
        //Debug.Log("Save...");
    }
    public void savePrefprogramPath()
    {
        //Debug.Log("Save...");
        PlayerPrefs.SetString("programPathIndex", UIManager.instance.externalProgramRunner.programPathInputField.text);
    }

    public void savePrefprogramArguments()
    {
        //Debug.Log("Save...");
        PlayerPrefs.SetString("programArgumentsIndex", UIManager.instance.externalProgramRunner.programArgumentsInputField.text);
    }

    public void savePrefserverIP()
    {
        //Debug.Log("Save...");
        PlayerPrefs.SetString("serverIPIndex", UIManager.instance.serverIPField.text);
    }

    public void savePrefserverPORT()
    {
        //Debug.Log("Save...");
        PlayerPrefs.SetString("serverPORTIndex", UIManager.instance.serverPORTField.text);
    }

    public static uint GetTrailerAddress(uint playerAddress)
    {
        LegacyMemoryReader.ReadInt16(playerAddress + 10);
        if (LegacyMemoryReader.ReadByte(playerAddress + 220) == 4)
        {
            uint num = LegacyMemoryReader.ReadUInt32(2147924016u);
            do
            {
                uint num2 = LegacyMemoryReader.ReadUInt32(num + 8);
                if (num2 == 0)
                {
                    break;
                }
                if (LegacyMemoryReader.ReadUInt32(num2 + 192) == playerAddress)
                {
                    return num2;
                }
                num = LegacyMemoryReader.ReadUInt32(num);
            }
            while (num != 0);
            return 0u;
        }
        return 0u;
    }
}