using System;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using CheatEngine;
using System.Numerics;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;
    public GameObject startMenu;
    public TMP_InputField usernameField;
    public TMP_InputField serverIPField;
    public TMP_InputField serverPORTField;
    public Button connectButton;
    public Button openButton;
    public Text console;
    public List<Texture2D> pngImages; //lista de imágenes png
    public List<List<Sprite>> spriteList; //lista de listas de sprites
    public List<Sprite> driverNames;
    public List<GameObject> driverPanel;
    public List<TMP_Text> playerNames;
    public List<Sprite> mapSprites;
    public List<string> mapNames;
    public GameObject mapPanel;
    public Image mapImage;
    public Text mapText;
    int pid;
    public ExternalProgramRunner externalProgramRunner;
    string address;
    string port;
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

    public void FailedToOpenProcess()
    {
        openButton.interactable = true;
        console.text = "Failed to open process!\nEnter emulator PID from 'Details'\ntab of Windows Task Manager";
    }

    public void UnableToFindPID()
    {
        openButton.interactable = true;
        console.text = "Unable to find process ID!\nEnter emulator PID from 'Details'\ntab of Windows Task Manager";
    }

    public void UnableToFindIdentifier()
    {
        openButton.interactable = true;
        console.text = "Unable to find Vig-00868 identifier!\nEnter emulator PID from 'Details'\ntab of Windows Task Manager";
    }

    public void EmulatorClosed()
    {
        usernameField.gameObject.SetActive(value: false);
        mapPanel.SetActive(value: false);
        console.text = "Emulator closed!\nEnter emulator PID from 'Details'\ntab of Windows Task Manager";
    }

    public void ServerDown()
    {
        usernameField.interactable = true;
        connectButton.interactable = true;
        console.text = "Server is down!\nEnter your username and server IP or URL";
    }

    public void UnableToConnect()
    {
        usernameField.interactable = true;
        connectButton.interactable = true;
        console.text = "Could not establish connection to server!\nEnter your username and server IP or URL";
    }
    public void ConnectionTimeout()
    {
        if (!externalProgramRunner.isRunning)
        {
        }
        else
        {


        }
        mapPanel.SetActive(value: false);
        usernameField.gameObject.SetActive(value: true);
        usernameField.interactable = true;
        connectButton.gameObject.SetActive(value: true);
        connectButton.interactable = true;
        console.text = "Connection Timeout!\nEnter your username and server IP or URL";
    }

    public void ConnectionSuccessful()
    {
        usernameField.gameObject.SetActive(value: false);
        connectButton.gameObject.SetActive(value: false);
        mapPanel.SetActive(value: true);
        if (GameManager.players[Client.instance.myId].isHost)
        {
            console.text = "Connected to server\nYou are the host";
        }
        else
        {
            console.text = "Connected to server\nYou are a guest";
        }
    }

    public void UpdateMapPanel(int mapID)
    {
        mapImage.sprite = mapSprites[mapID];
        mapImage.SetNativeSize();
        mapText.text = mapNames[mapID];
    }

    public void UpdateDriverPanel(int mapID)
    {
        mapImage.sprite = mapSprites[mapID];
        mapImage.SetNativeSize();
        mapText.text = mapNames[mapID];
    }

    public void ConnectToServer()
    {
        usernameField.interactable = false;
        connectButton.interactable = false;
        console.text = "Connecting to server...";
        if (serverIPField.text == "")
            serverIPField.text = "127.0.0.1";
        if (serverPORTField.text == "")
            serverPORTField.text = "26950";
        address = serverIPField.text;
        port = serverPORTField.text;
        Debug.Log("Conectando: " + address + ":" + port);
        Client.instance.ConnectToServer(address, port);
    }

    public void OpenProcess()
    {
        if (!externalProgramRunner.isRunning)
        {
            Debug.Log("External program is not running.");
        }
        else
        {
            pid = externalProgramRunner.processId;
            //Debug.Log("External program PID: " + pid);
            //Resto del código que utiliza PID
        }
        openButton.interactable = false;

        if (!ProcessSelect.SelectProcess(pid.ToString()))
        {
            UnableToFindPID();
        }
        else if (LegacyMemoryReader.CheckProcess())
        {
            console.text = "Searching for Vig2 00868 in emulator ram...";
            Debug.Log(console.text);
            IntPtr handle = Scanner.OpenProcess(LegacyMemoryReader.GetPID());
            byte[] bytes = Encoding.ASCII.GetBytes(GameManager.instance.identifier);
            List<IntPtr> list = Scanner.FindData(handle, bytes);
            string concatenatedList = string.Join(", ", list);
            Debug.Log("list: 0x" + concatenatedList);
#if DEBUG
            Debug.Log("Escaneo de Región de Memoria: 0x" + handle.ToString("X") + " - 0x" + list[0].ToString("X"));
#endif
            //Debug.Log("bytes: " + bytes);
            //Debug.Log("Lista: " + list);
            if (list.Count == 0)
            {
                UnableToFindIdentifier();
                return;
            }

            LegacyMemoryReader.SetBaseAddress(list[0]);

            string hexValue = BigInteger.Add(BigInteger.Parse(LegacyMemoryReader.baseAddress.ToString("X"), System.Globalization.NumberStyles.HexNumber), BigInteger.One).ToString("X");

#if DEBUG
            Debug.Log("Memoria Base: 0x" + hexValue);
#endif

            cheatEngine = FindObjectOfType<CheatEngine.CheatEngine>();
            if (!cheatEngine.cheatEnabled)
                cheatEngine.ToggleCheatEnabled();

            openButton.gameObject.SetActive(value: false);
            usernameField.gameObject.SetActive(value: true);
            usernameField.interactable = true;
            connectButton.gameObject.SetActive(value: true);
            connectButton.interactable = true;
            console.text = "Enter your username and server IP or URL";
        }
    }

    public CheatEngine.CheatEngine cheatEngine;

    private void Start()
    {
        spriteList = new List<List<Sprite>>(); // inicializar la lista de listas de sprites

        // recorrer la lista de imágenes png
        for (int i = 0; i < pngImages.Count; i++)
        {
            // inicializar la lista de sprites para esta imagen png
            List<Sprite> sprites = new List<Sprite>();

            // obtener los slices de esta imagen png
            Sprite[] slices = Resources.LoadAll<Sprite>("Images/" + pngImages[i].name);

            // recorrer los slices y agregarlos a la lista de sprites
            for (int j = 0; j < slices.Length; j++)
            {
                sprites.Add(slices[j]);
            }
            // agregar la lista de sprites a la lista de listas de sprites
            spriteList.Add(sprites);
        }

        // ejemplo de cómo acceder a un sprite específico en la lista
        Sprite mySprite = spriteList[0][1]; // el segundo sprite de la primera imagen png
        Debug.Log(mySprite.name);

        externalProgramRunner = FindObjectOfType<ExternalProgramRunner>();
    }
}
