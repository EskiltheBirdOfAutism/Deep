using System.Net.Sockets;
using System.Net;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode.Transports.UTP;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using UnityEditor;

public class NetworkManagerUICode : MonoBehaviour
{
    [SerializeField] private GameObject menu_pos;
    //[SerializeField] private GameObject world_generator;
    //[SerializeField] private GameObject chunk_generator;
    [SerializeField] private GameObject room_generator;
    [SerializeField] private Button host_button;
    [SerializeField] private Button join_button;
    [SerializeField] private Button address_button;
    [SerializeField] private Button play_button;
    public GameObject network_manager;
    private string local_address = "";
    [SerializeField] private GameObject player_prefab;
    private int firewall_break = -1;
    [SerializeField] private string target_scene;
    private string menu_state = "Host/Join";
    private string menu_switch = "NULL";
    private float menu_timer = 0;
    private TextMeshProUGUI menu_text;
    [SerializeField] private LayerMask[] player_layer = new LayerMask[4];
    private void Awake()
    {
        // Det här lägger in listeners för kanpparna, så att vi kan klicka på de
        host_button.onClick.AddListener(HostClicked);
        join_button.onClick.AddListener(JoinClicked);
        address_button.onClick.AddListener(AddressClicked);
        play_button.onClick.AddListener(PlayButton);

        // Det här lägger då in vår ip address i variabeln local_address som en string
        local_address = GetLocalIPAddress();
        if (SceneManager.GetActiveScene().name != target_scene)
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
        // Den här koden hanterar bara timern för när man byter i menyn
        menu_timer -= 10f * Time.deltaTime;
        menu_timer = Mathf.Clamp(menu_timer, 0, 100);

        // Det här stänger av knapparna och ui från menyn
        // Vi gör det här så att vi kan låta objektet komma med i nästa scene
        if (SceneManager.GetActiveScene().name == target_scene)
        {
            menu_pos.SetActive(false);
        }
        else
        {
            menu_pos.SetActive(true);
        }

        // Vi gör ett switch statement här för att enklare få ut vad vi ska göra i varje fall
        // Eftersom flera if statements kan vara långsammare för spelet, är det här lite snabbare
        switch (menu_state)
        {
            case ("Host/Join"):
                {
                    // I den här koden kan man välja mellan host eller join inriktningen, för ifall man vill hosta ett spel
                    // Alltså starta sit egen dator some en server
                    // Eller joina ett spel gå med på en annans server, baserat på deras dator ip address

                    menu_pos.GetComponent<RectTransform>().anchoredPosition += (new Vector2(-240, -30) - menu_pos.GetComponent<RectTransform>().anchoredPosition) * 10f * Time.deltaTime;
                    // (menu pos flyttar positionen på hela menyn, alltså knapparna då)
                    if (menu_switch != "NULL")
                    {
                        menu_state = "Switch";
                    }

                    // Bestämmer vilka knappar som får vara synliga baserat på vilken inriktning/state menyn är inne i
                    // Här är host och join knapparna synliga
                    address_button.gameObject.SetActive(false);
                    play_button.gameObject.SetActive(false);
                    host_button.gameObject.SetActive(true);
                    join_button.gameObject.SetActive(true);
                }
                break;

            // De nästa två är då intriktningarna för både host och join inriktningarna

            case ("Host"):
                {
                    menu_pos.GetComponent<RectTransform>().anchoredPosition += (new Vector2(-240, -30) - menu_pos.GetComponent<RectTransform>().anchoredPosition) * 10f * Time.deltaTime;
                    // (menu pos flyttar positionen på hela menyn, alltså knapparna då)
                    if (menu_switch != "NULL")
                    {
                        menu_state = "Switch";
                    }

                    // Här är firewall break knapparna synliga
                    address_button.gameObject.SetActive(true);
                    play_button.gameObject.SetActive(true);
                    host_button.gameObject.SetActive(false);
                    join_button.gameObject.SetActive(false);

                    if (Input.GetKeyDown(KeyCode.Escape))
                    {
                        if (menu_text == null)
                        {
                            menu_switch = "Host/Join";
                            menu_timer = 5;
                        }
                        else
                        {
                            address_button.GetComponentInChildren<TextMeshProUGUI>().text = "Address";
                            play_button.GetComponentInChildren<TextMeshProUGUI>().text = "Play";
                            menu_text = null;
                        }
                    }
                }
                break;

            case ("Join"):
                {
                    menu_pos.GetComponent<RectTransform>().anchoredPosition += (new Vector2(-240, -30) - menu_pos.GetComponent<RectTransform>().anchoredPosition) * 10f * Time.deltaTime;
                    // (menu pos flyttar positionen på hela menyn, alltså knapparna då)
                    if (menu_switch != "NULL")
                    {
                        menu_state = "Switch";
                    }

                    // Här är firewall break knapparna synliga
                    address_button.gameObject.SetActive(true);
                    play_button.gameObject.SetActive(true);
                    host_button.gameObject.SetActive(false);
                    join_button.gameObject.SetActive(false);

                    if (Input.GetKeyDown(KeyCode.Escape))
                    {
                        if (menu_text == null)
                        {
                            menu_switch = "Host/Join";
                            menu_timer = 5;
                        }
                        else
                        {
                            address_button.GetComponentInChildren<TextMeshProUGUI>().text = "Address";
                            play_button.GetComponentInChildren<TextMeshProUGUI>().text = "Play";
                            menu_text = null;
                        }
                    }
                }
                break;

            case ("Switch"):
                {
                    // Den här koden sker när man byter meny
                    // Framför allt när man byter från start menyn till en av host eller join inriktningarna

                    menu_pos.GetComponent<RectTransform>().anchoredPosition += (new Vector2(-540, -30) - menu_pos.GetComponent<RectTransform>().anchoredPosition) * 10f * Time.deltaTime;
                    if (menu_timer <= 0)
                    {
                        menu_state = menu_switch;
                        menu_switch = "NULL";
                    }
                }
                break;
        }

        if (menu_text != null)
        {
            // Ifall vi tillåter att man kan ändra target ip addressen, så kan man skriva in den som en string
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
            if (Input.GetKey(KeyCode.Delete) && _address.Length > 0) _address = _address.Substring(0, _address.Length - 1);
            network_manager.GetComponent<UnityTransport>().ConnectionData.Address = _address;

            // Sen så skriver vi så addresserna i menyn
            menu_text.text = network_manager.GetComponent<UnityTransport>().ConnectionData.Address;
        }
    }

    // Den här koden sker när man klickar på host knappen i menyn
    private void HostClicked()
    {
        menu_switch = "Host";
        menu_timer = 5;
    }

    // Väldigt liknande till HostClicked faktiskt, men när man klickar på join knappen i menyn
    private void JoinClicked()
    {
        menu_switch = "Join";
        menu_timer = 5;
    }

    // Ifall man trycker på firewall on, kommer det då att öppna porterna för firewallen automatiskt
    // Detta krävs för när man använder connection vid ett annat nätverk eller så
    private void AddressClicked()
    {
        menu_text = address_button.GetComponentInChildren<TextMeshProUGUI>();
        menu_text.fontSize = 12;
        firewall_break = -1;
    }

    // Det här kan man då använda om man bara kör UDP istället för TCP
    private void PlayButton()
    {
        switch (menu_state)
        {
            case ("Host"):
                {
                    // Detta sker när man har tryckt på enter på host inriktningen, efter inskrivning av ip address
                    // Den här koden använder då netcodes StartHost() samt så den sätter connection data, till ip addressen och porten som alla använder 7771
                    // Den kommer sedan att byta scen, genom networkmanagern, när det är laddat in så aktiveras metoden OnSceneLoaded
                    // Sen har jag också på toppen en firewall break ifall det är aktiverat, vilket öppnar porterna 7771 och 7772

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
                break;

            case ("Join"):
                {
                    // Detta sker när man har tryckt på enter på join inriktningen, efter inskrivning av ip address
                    // Istället för StratHost() är det StartClient(), man sätter då connection data till ip addressen av hosten (den skriver man in i menyn) sedan också port som alla använder 7771
                    // När man kopplar till hosten så bör man också komma till scenen som hosten är i, därför använder vi OnSceneLoaded igen för att skapa spelaren
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
                break;
        }
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
            GenerateWorld();

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

    // En metod som bara spawnar världen, specifikt för hosten
    private void GenerateWorld()
    {
        //var _world = Instantiate(world_generator);
        //_world.GetComponent<NetworkObject>().Spawn();
        //var _chunk = Instantiate(chunk_generator);
        //_chunk.GetComponent<NetworkObject>().Spawn();
        var _generator = Instantiate(room_generator);
        _generator.GetComponent<NetworkObject>().Spawn();
    }

    // Det här skapar spelaren med rätt id, tagen från klientens id
    // Vilket innebär att när vi sen tar kontroll över spelaren kan mab bara göra det om klientens id matchar spelaren
    // Annars ifall man hostar eller joinar så finns det risk att man inte kan styra eller att man tar kontroll över en annans spelare
    private void SpawnPlayer(ulong _client_id)
    {
        var _player_instance = Instantiate(player_prefab, new Vector3(0, 0, 0), Quaternion.identity);
        var _net_obj = _player_instance.GetComponent<NetworkObject>();
        _net_obj.SpawnAsPlayerObject(_client_id);
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