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
    [Tooltip ("The ui image in the menu")]
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
        // Det h�r l�gger in listeners f�r kanpparna, s� att vi kan klicka p� de
        server_button.onClick.AddListener(ServerClicked);
        host_button.onClick.AddListener(HostClicked);
        join_button.onClick.AddListener(JoinClicked);

        // Det h�r l�gger d� in v�r ip address i variabeln local_address som en string
        local_address = GetLocalIPAddress();
        if(SceneManager.GetActiveScene().name != target_scene)
        {
            // Sedan s�tter vi unity transports address till local_address
            network_manager.GetComponent<UnityTransport>().ConnectionData.Address = local_address;
        }
    }

    private void Start()
    {
        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
    }

    private void Update()
    {
        // Det h�r st�nger av knapparna och ui fr�n menyn
        // Vi g�r det h�r s� att vi kan l�ta objektet komma med i n�sta scene
        if (SceneManager.GetActiveScene().name == target_scene)
        {
            ui_image.SetActive(false);
        }
        else
        {
            ui_image.SetActive(true);
        }
        
        // Det h�r bara byter p� ip_string_check, vilket sedan kollar ifall vi kan byta ip
        if (Input.GetKeyDown(KeyCode.I))
        {
            ip_string_check = -ip_string_check;
        }

        // Det h�r s�tter av och p� firewall_break, vilket �ppnar portarna 7771 och 7772
        if (Input.GetKeyDown(KeyCode.F) && SceneManager.GetActiveScene().name == "MenuScene")
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

        // Ifall vi till�ter att man kan �ndra target ip addressen, s� kan man skriva in den som en string
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

        // Sen s� skriver vi s� addresserna i menyn
        address_text.text = "Target Address: " + network_manager.GetComponent<UnityTransport>().ConnectionData.Address;
        own_address_text.text = "Own Address: " + local_address;
    }

    // Den h�r koden anv�nder jag aldrig, men den startar servern
    // Det skulle teoretiskt s�tt kunna funka, om jag l�gger till att den flyttar scen och s�nt
    private void ServerClicked()
    {
        NetworkManager.Singleton.StartServer();
    }

    // Den h�r koden sker n�r man klickar p� host knappen i menyn
    // Den anv�nder d� netcodes StartHost() samt s� den s�tter connection data, till ip addressen och porten som alla anv�nder 7771
    // Den kommer sedan att byta scen, genom networkmanagern, n�r det �r laddat in s� aktiveras metoden OnSceneLoaded
    // Sen har jag ocks� p� toppen en firewall break ifall det �r aktiverat, vilket �ppnar porterna 7771 och 7772

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

    // V�ldigt liknande till HostClicked faktiskt, men n�r man klickar p� join knappen i menyn
    // Ist�llet f�r StratHost() �r det StartClient(), man s�tter d� connection data till ip addressen av hosten (den skriver man in i menyn) sedan ocks� port som alla anv�nder 7771
    // N�r man kopplar till hosten s� b�r man ocks� komma till scenen som hosten �r i, d�rf�r anv�nder vi OnSceneLoaded igen f�r att skapa spelaren
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

    // Den h�r koden anv�nds n�r klienten faktiskt ansluter till en host
    // D� skapar den sin egen spelare f�r sin id
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

    // Den h�r koden anv�nds n�r vi laddar scenen
    // Den kollar d� igenom alla klienter som redan finns och skapar deras objekt p� denna klient
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

    // Det h�r skapar spelaren med r�tt id, tagen fr�n klientens id
    // Vilket inneb�r att n�r vi sen tar kontroll �ver spelaren kan mab bara g�ra det om klientens id matchar spelaren
    // Annars ifall man hostar eller joinar s� finns det risk att man inte kan styra eller att man tar kontroll �ver en annans spelare
    private void SpawnPlayer(ulong _client_id)
    {
        var _player_instance = Instantiate(player_prefab);
        _player_instance.GetComponent<NetworkObject>().SpawnAsPlayerObject(_client_id);
    }

    // Den h�r koden tar ens lokala ip address fr�n datorn
    // Det h�r �r s� att vi kan bara f� ip addressen direkt n�r startar spelet ifall man �r hosten
    // Sj�lvaste ip addressen finns inte som en hel string, utan finns i Dns.GetHostEntry, den h�r koden kollar igenom hela Dns.GetHostEntry f�r det relevanta f�r ip addressen
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