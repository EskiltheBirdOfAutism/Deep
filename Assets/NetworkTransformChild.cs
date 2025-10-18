using Unity.Netcode;
using UnityEngine;
using UnityEngine.UIElements;

public class NetworkTransformChild : NetworkBehaviour
{
    // Det h�r �r varaibler som kr�vs f�r att skicka till children av parent
    // Vi har d� en lista f�r alla transforms som vi ska �ndra
    // (namnen �r lite fel, target_transform �r de transforms vi ska �ndra, medan target_position och target_rotation) �r vad det ska bli
    [SerializeField] private Transform[] target_transform;

    private Vector3[] target_position = new Vector3[99];
    private Quaternion[] target_rotation = new Quaternion[99];

    private void Start()
    {
        // Vi s�tter alla targets till vad de transforms vi har nu
        for (int _i = 0; _i < target_transform.Length; _i++)
        {
            target_position[_i] = target_transform[_i].position;
            target_rotation[_i] = target_transform[_i].rotation;
        }

        // Sen tar vi alla childrens rigidbodies, s� att vi kan s�tta de till kinematic ifall de inte tillh�r �garen
        // Hur det d� funkar �r att f�r alla klienter har de andra klienternas objekt i spelet baserat p� klienternas transforms och s�nt
        // Men vi vill inte p�verka fysiken och transforms fr�n en annan klient, d�rf�r s�tter vi de de representerande klienternas rigidbodies till kinematic s� att de inte kan �ndras
        Rigidbody[] _rb = GetComponentsInChildren<Rigidbody>();

        if (!IsOwner)
        {
            foreach (var _r in _rb)
            {
                _r.isKinematic = true;
            }
        }
    }

    private void FixedUpdate()
    {
        // Vi kollar f�rst ifall denna objekt tillh�r �garen
        if (IsOwner)
        {
            // Ifall det �r s� skickar vi positionerna och rotationerna
            // Vi g�r det genom en for loop av alla childrens transforms, s� skickar vi det d� fr�n klienten till servern
            for (int _i = 0; _i < target_transform.Length; _i++)
            {
                SendTransformServerRpc(target_transform[_i].position, target_transform[_i].rotation, _i);
            }
        }
        else
        {
            // Ifall objektet inte tillh�r �garen �ndrar vi d� positionerna och rotationerna till vad de ska vara
            // Det funkar d� allts� att klienten skickar sin information till servern baserat p� sina egna positioner och rotationer
            // Sedan tar man d� positionerna och rotationerna skickat fr�n andra klienter, s� �ndrar vi deras represenativa objekts positioner till de positionerna och rotationer
            for (int _i = 0; _i < target_transform.Length; _i++)
            {
                target_transform[_i].position = Vector3.Lerp(target_transform[_i].position, target_position[_i], Time.fixedDeltaTime * 10f);
                target_transform[_i].rotation = Quaternion.Slerp(target_transform[_i].rotation, target_rotation[_i], Time.fixedDeltaTime * 10f);
            }
        }
    }

    // Den h�r headern inneb�r att vi skickar information till servern
    [ServerRpc(RequireOwnership = false)]
    private void SendTransformServerRpc(Vector3 _pos, Quaternion _rot, int _index)
    {
        // Sedan uppdaterar vi d� f�r klienten fr�n servern
        UpdateTransformClientRpc(_pos, _rot, _index);
    }

    // Den h�r headern inneb�r att vi skickar information fr�n servern till klienten
    [ClientRpc]
    private void UpdateTransformClientRpc(Vector3 _pos, Quaternion _rot, int _index)
    {
        // S� ifall objektet inte tillh�r �garen, utan en ann klient s� �r den d� ett represeterande objekt av den klienten
        // D� �ndrar vi d� dens positioner till informationen given av servern
        if (!IsOwner)
        {
            target_position[_index] = _pos;
            target_rotation[_index] = _rot;
        }
    }
}