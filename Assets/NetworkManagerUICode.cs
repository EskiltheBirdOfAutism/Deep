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
    [SerializeField] private GameObject menu_pos;
    [SerializeField] private Button host_button;
    [SerializeField] private Button join_button;
    [SerializeField] private Button firewallon_button;
    [SerializeField] private Button firewalloff_button;
    public GameObject network_manager;
    private string local_address = "";
    [SerializeField] private GameObject player_prefab;
    private int firewall_break = -1;
    [SerializeField] private string target_scene;
    private string menu_state = "Host/Join";
    private string menu_switch = "NULL";
    private float menu_timer = 0;
    private TextMeshProUGUI menu_text;

    private void Awake()
    {
        // Det h�r l�gger in listeners f�r kanpparna, s� att vi kan klicka p� de
        host_button.onClick.AddListener(HostClicked);
        join_button.onClick.AddListener(JoinClicked);
        firewallon_button.onClick.AddListener(FirewallOn);
        firewalloff_button.onClick.AddListener(FirewallOff);

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
        // Den h�r koden hanterar bara timern f�r n�r man byter i menyn
        menu_timer -= 10f * Time.deltaTime;
        menu_timer = Mathf.Clamp(menu_timer, 0, 100);

        // Det h�r st�nger av knapparna och ui fr�n menyn
        // Vi g�r det h�r s� att vi kan l�ta objektet komma med i n�sta scene
        if (SceneManager.GetActiveScene().name == target_scene)
        {
            menu_pos.SetActive(false);
        }
        else
        {
            menu_pos.SetActive(true);
        }

        // Vi g�r ett switch statement h�r f�r att enklare f� ut vad vi ska g�ra i varje fall
        // Eftersom flera if statements kan vara l�ngsammare f�r spelet, �r det h�r lite snabbare
        switch(menu_state)
        {
            case ("Host/Join"):
            {
                // I den h�r koden kan man v�lja mellan host eller join inriktningen, f�r ifall man vill hosta ett spel
                // Allts� starta sit egen dator some en server
                // Eller joina ett spel g� med p� en annans server, baserat p� deras dator ip address

                menu_pos.GetComponent<RectTransform>().anchoredPosition += (new Vector2(-240, -30) - menu_pos.GetComponent<RectTransform>().anchoredPosition) * 10f * Time.deltaTime;
                if(menu_switch != "NULL")
                {
                    menu_state = "Switch";
                }

                firewallon_button.gameObject.SetActive(false);
                firewalloff_button.gameObject.SetActive(false);
                host_button.gameObject.SetActive(true);
                join_button.gameObject.SetActive(true);
            }
            break;

            // De n�sta tv� �r d� intriktningarna f�r b�de host och join inriktningarna

            case ("Host"):
            {
                menu_pos.GetComponent<RectTransform>().anchoredPosition += (new Vector2(-240, -30) - menu_pos.GetComponent<RectTransform>().anchoredPosition) * 10f * Time.deltaTime;
                if (menu_switch != "NULL")
                {
                    menu_state = "Switch";
                }

                firewallon_button.gameObject.SetActive(true);
                firewalloff_button.gameObject.SetActive(true);
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
                        firewallon_button.GetComponentInChildren<TextMeshProUGUI>().text = "Firewall On";
                        firewalloff_button.GetComponentInChildren<TextMeshProUGUI>().text = "Firewall Off";
                        firewallon_button.GetComponentInChildren<TextMeshProUGUI>().fontSize = 20;
                        firewalloff_button.GetComponentInChildren<TextMeshProUGUI>().fontSize = 20;
                        menu_text = null;
                    }
                }
            }
            break;

            case ("Join"):
            {
                menu_pos.GetComponent<RectTransform>().anchoredPosition += (new Vector2(-240, -30) - menu_pos.GetComponent<RectTransform>().anchoredPosition) * 10f * Time.deltaTime;
                if (menu_switch != "NULL")
                {
                    menu_state = "Switch";
                }

                firewallon_button.gameObject.SetActive(true);
                firewalloff_button.gameObject.SetActive(true);
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
                        firewallon_button.GetComponentInChildren<TextMeshProUGUI>().text = "Firewall On";
                        firewalloff_button.GetComponentInChildren<TextMeshProUGUI>().text = "Firewall Off";
                        firewallon_button.GetComponentInChildren<TextMeshProUGUI>().fontSize = 20;
                        firewalloff_button.GetComponentInChildren<TextMeshProUGUI>().fontSize = 20;
                        menu_text = null;
                    }
                }
            }
            break;

            case ("Switch"):
            {
                // Den h�r koden sker n�r man byter meny
                // Framf�r allt n�r man byter fr�n start menynb till en av host eller join inriktningarna

                menu_pos.GetComponent<RectTransform>().anchoredPosition += (new Vector2(-540, -30) - menu_pos.GetComponent<RectTransform>().anchoredPosition) * 10f * Time.deltaTime;
                if(menu_timer <= 0)
                {
                    menu_state = menu_switch;
                    menu_switch = "NULL";
                }
            }
            break;
        }

        if (menu_text != null)
        {
            // Ifall vi till�ter att man kan �ndra target ip addressen, s� kan man skriva in den som en string
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

            // Sen s� skriver vi s� addresserna i menyn
            menu_text.text = network_manager.GetComponent<UnityTransport>().ConnectionData.Address;

            if(Input.GetKeyDown(KeyCode.Return))
            {
                switch(menu_state)
                {
                    case ("Host"):
                    {
                        // Detta sker n�r man har tryckt p� enter p� host inriktningen, efter inskrivning av ip address
                        // Den h�r koden anv�nder d� netcodes StartHost() samt s� den s�tter connection data, till ip addressen och porten som alla anv�nder 7771
                        // Den kommer sedan att byta scen, genom networkmanagern, n�r det �r laddat in s� aktiveras metoden OnSceneLoaded
                        // Sen har jag ocks� p� toppen en firewall break ifall det �r aktiverat, vilket �ppnar porterna 7771 och 7772

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
                        // Detta sker n�r man har tryckt p� enter p� join inriktningen, efter inskrivning av ip address
                        // Ist�llet f�r StratHost() �r det StartClient(), man s�tter d� connection data till ip addressen av hosten (den skriver man in i menyn) sedan ocks� port som alla anv�nder 7771
                        // N�r man kopplar till hosten s� b�r man ocks� komma till scenen som hosten �r i, d�rf�r anv�nder vi OnSceneLoaded igen f�r att skapa spelaren
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
        }
    }

    // Den h�r koden sker n�r man klickar p� host knappen i menyn

    private void HostClicked()
    {
        menu_switch = "Host";
        menu_timer = 5;
    }

    // V�ldigt liknande till HostClicked faktiskt, men n�r man klickar p� join knappen i menyn
    
    private void JoinClicked()
    {
        menu_switch = "Join";
        menu_timer = 5;
    }

    // Ifall man trycker p� firewall on, kommer det d� att �ppna porterna f�r firewallen automatiskt
    // Detta kr�vs f�r n�r man anv�nder connection vid ett annat n�tverk eller s�
    private void FirewallOn()
    {
        firewalloff_button.GetComponentInChildren<TextMeshProUGUI>().text = "Firewall Off";
        firewalloff_button.GetComponentInChildren<TextMeshProUGUI>().fontSize = 20;
        menu_text = firewallon_button.GetComponentInChildren<TextMeshProUGUI>();
        menu_text.fontSize = 12;
        firewall_break = 1;
    }

    // Det h�r kan man d� anv�nda om man bara k�r UDP ist�llet f�r TCP
    private void FirewallOff()
    {
        firewallon_button.GetComponentInChildren<TextMeshProUGUI>().text = "Firewall On";
        firewallon_button.GetComponentInChildren<TextMeshProUGUI>().fontSize = 20;
        menu_text = firewalloff_button.GetComponentInChildren<TextMeshProUGUI>();
        menu_text.fontSize = 12;
        firewall_break = -1;
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