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
        // Det här lägger in listeners för kanpparna, så att vi kan klicka på de
        server_button.onClick.AddListener(ServerClicked);
        host_button.onClick.AddListener(HostClicked);
        join_button.onClick.AddListener(JoinClicked);

        // Det här lägger då in vår ip address i variabeln local_address som en string
        local_address = GetLocalIPAddress();
        if(SceneManager.GetActiveScene().name != target_scene)
        {
            // Sedan sätter vi unity transports address till local_address
            network_manager.GetComponent<UnityTransport>().ConnectionData.Address = local_address;
        }
    }

    private void Start()
    {
        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
    }

    private void Update()
    {
        // Det här stänger av knapparna och ui från menyn
        // Vi gör det här så att vi kan låta objektet komma med i nästa scene
        if (SceneManager.GetActiveScene().name == target_scene)
        {
            ui_image.SetActive(false);
        }
        else
        {
            ui_image.SetActive(true);
        }
        
        // Det här bara byter på ip_string_check, vilket sedan kollar ifall vi kan byta ip
        if (Input.GetKeyDown(KeyCode.I))
        {
            ip_string_check = -ip_string_check;
        }

        // Det här sätter av och på firewall_break, vilket öppnar portarna 7771 och 7772
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

        // Ifall vi tillåter att man kan ändra target ip addressen, så kan man skriva in den som en string
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

        // Sen så skriver vi så addresserna i menyn
        address_text.text = "Target Address: " + network_manager.GetComponent<UnityTransport>().ConnectionData.Address;
        own_address_text.text = "Own Address: " + local_address;
    }

    // Den här koden använder jag aldrig, men den startar servern
    // Det skulle teoretiskt sätt kunna funka, om jag lägger till att den flyttar scen och sånt
    private void ServerClicked()
    {
        NetworkManager.Singleton.StartServer();
    }

    // Den här koden sker när man klickar på host knappen i menyn
    // Den använder då netcodes StartHost() samt så den sätter connection data, till ip addressen och porten som alla använder 7771
    // Den kommer sedan att byta scen, genom networkmanagern, när det är laddat in så aktiveras metoden OnSceneLoaded
    // Sen har jag också på toppen en firewall break ifall det är aktiverat, vilket öppnar porterna 7771 och 7772

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

    // Väldigt liknande till HostClicked faktiskt, men när man klickar på join knappen i menyn
    // Istället för StratHost() är det StartClient(), man sätter då connection data till ip addressen av hosten (den skriver man in i menyn) sedan också port som alla använder 7771
    // När man kopplar till hosten så bör man också komma till scenen som hosten är i, därför använder vi OnSceneLoaded igen för att skapa spelaren
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

    // Den här koden används när klienten faktiskt ansluter till en host
    // Då skapar den sin egen spelare för sin id
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

    // Den här koden används när vi laddar scenen
    // Den kollar då igenom alla klienter som redan finns och skapar deras objekt på denna klient
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

    // Det här skapar spelaren med rätt id, tagen från klientens id
    // Vilket innebär att när vi sen tar kontroll över spelaren kan mab bara göra det om klientens id matchar spelaren
    // Annars ifall man hostar eller joinar så finns det risk att man inte kan styra eller att man tar kontroll över en annans spelare
    private void SpawnPlayer(ulong _client_id)
    {
        var _player_instance = Instantiate(player_prefab);
        _player_instance.GetComponent<NetworkObject>().SpawnAsPlayerObject(_client_id);
    }

    // Den här koden tar ens lokala ip address från datorn
    // Det här är så att vi kan bara få ip addressen direkt när startar spelet ifall man är hosten
    // Självaste ip addressen finns inte som en hel string, utan finns i Dns.GetHostEntry, den här koden kollar igenom hela Dns.GetHostEntry för det relevanta för ip addressen
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