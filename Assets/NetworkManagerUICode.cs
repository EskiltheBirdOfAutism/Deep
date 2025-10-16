using System.Net.Sockets;
using System.Net;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode.Transports.UTP;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class NetworkManagerUICode : MonoBehaviour
{
    [SerializeField] private GameObject ui_image;
    [SerializeField] private Button server_button;
    [SerializeField] private Button host_button;
    [SerializeField] private Button join_button;
    private int ip_string_check = -1;
    public GameObject network_manager;
    [SerializeField] private TextMeshProUGUI address_text;
    [SerializeField] private TextMeshProUGUI own_address_text;
    [SerializeField] private TextMeshProUGUI firewall_break_text;
    private string local_address = "";
    [SerializeField] private GameObject player_prefab;
    private int firewall_break = -1;
    [SerializeField] private string target_scene;

    private void Awake()
    {
        server_button.onClick.AddListener(ServerClicked);
        host_button.onClick.AddListener(HostClicked);
        join_button.onClick.AddListener(JoinClicked);

        local_address = GetLocalIPAddress();
        if(SceneManager.GetActiveScene().name != target_scene)
        {
            network_manager.GetComponent<UnityTransport>().ConnectionData.Address = local_address;
        }
    }

    private void Start()
    {
        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
    }

    private void Update()
    {
        if (SceneManager.GetActiveScene().name == target_scene)
        {
            ui_image.SetActive(false);
        }
        else
        {
            ui_image.SetActive(true);
        }

        if (Input.GetKeyDown(KeyCode.I))
        {
            ip_string_check = -ip_string_check;
        }

        if (Input.GetKeyDown(KeyCode.F))
        {
            firewall_break = -firewall_break;
            if (firewall_break == 1)
            {
                firewall_break_text.text = "Firewall Break: On";
            }
            else
            {
                firewall_break_text.text = "Firewall Break: Off";
            }
        }

        if (ip_string_check == 1)
        {
            string _address = network_manager.GetComponent<UnityTransport>().ConnectionData.Address;
            if (Input.GetKeyDown("1")) _address += "1";
            if (Input.GetKeyDown("2")) _address += "2";
            if (Input.GetKeyDown("3")) _address += "3";
            if (Input.GetKeyDown("4")) _address += "4";
            if (Input.GetKeyDown("5")) _address += "5";
            if (Input.GetKeyDown("6")) _address += "6";
            if (Input.GetKeyDown("7")) _address += "7";
            if (Input.GetKeyDown("8")) _address += "8";
            if (Input.GetKeyDown("9")) _address += "9";
            if (Input.GetKeyDown("0")) _address += "0";
            if (Input.GetKeyDown(".")) _address += ".";
            if (Input.GetKeyDown(KeyCode.Delete)) _address = _address.Substring(0, _address.Length - 1);
            network_manager.GetComponent<UnityTransport>().ConnectionData.Address = _address;
        }

        address_text.text = "Target Address: " + network_manager.GetComponent<UnityTransport>().ConnectionData.Address;
        own_address_text.text = "Own Address: " + local_address;
    }

    private void ServerClicked()
    {
        NetworkManager.Singleton.StartServer();
    }

    private void HostClicked()
    {
        if (firewall_break == 1)
        {
            #if UNITY_STANDALONE_WIN && !UNITY_EDITOR
                        WindowsFirewallHelper.OpenHostPorts();
            #endif
        }

        UnityTransport _transport = network_manager.GetComponent<UnityTransport>();
        _transport.SetConnectionData(_transport.ConnectionData.Address, _transport.ConnectionData.Port);
        NetworkManager.Singleton.StartHost();
        NetworkManager.Singleton.SceneManager.OnLoadEventCompleted += OnSceneLoaded;
        NetworkManager.Singleton.SceneManager.LoadScene(target_scene, LoadSceneMode.Single);
    }

    private void JoinClicked()
    {
        if (firewall_break == 1)
        {
            #if UNITY_STANDALONE_WIN && !UNITY_EDITOR
                        WindowsFirewallHelper.OpenClientPorts();
            #endif
        }

        UnityTransport _transport = network_manager.GetComponent<UnityTransport>();
        _transport.SetConnectionData(_transport.ConnectionData.Address, _transport.ConnectionData.Port);
        NetworkManager.Singleton.StartClient();
        NetworkManager.Singleton.SceneManager.OnLoadEventCompleted += OnSceneLoaded;
    }

    private void OnClientConnected(ulong _client_id)
    {
        if (!NetworkManager.Singleton.IsServer) return;

        if (SceneManager.GetActiveScene().name == target_scene)
        {
            if (NetworkManager.Singleton.ConnectedClients[_client_id].PlayerObject == null)
            {
                SpawnPlayer(_client_id);
            }
        }
    }

    private void OnSceneLoaded(string _scene_name, LoadSceneMode _mode, List<ulong> _clients_completed, List<ulong> _clients_timed_out)
    {
        if (NetworkManager.Singleton.IsServer && _scene_name == target_scene)
        {
            foreach (var _client_id in _clients_completed)
            {
                if (NetworkManager.Singleton.ConnectedClients[_client_id].PlayerObject == null)
                {
                    SpawnPlayer(_client_id);
                }
            }
        }

        NetworkManager.Singleton.SceneManager.OnLoadEventCompleted -= OnSceneLoaded;
    }
    // This spawns the player with its correct id
    // Meaning that when a player hosts or joins, it will be able to control their own play since that player matches its id
    private void SpawnPlayer(ulong _client_id)
    {
        var _player_instance = Instantiate(player_prefab);
        _player_instance.GetComponent<NetworkObject>().SpawnAsPlayerObject(_client_id);
    }

    public static string GetLocalIPAddress()
    {
        foreach (var ip in Dns.GetHostEntry(Dns.GetHostName()).AddressList)
        {
            if (ip.AddressFamily == AddressFamily.InterNetwork)
            {
                return ip.ToString();
            }
        }
        throw new System.Exception("No network adapters with an IPv4 address in the system!");
    }
}