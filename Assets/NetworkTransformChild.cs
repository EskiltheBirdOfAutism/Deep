using Unity.Netcode;
using UnityEngine;
using UnityEngine.UIElements;

public class NetworkTransformChild : NetworkBehaviour
{
    // Det här är varaibler som krävs för att skicka till children av parent
    // Vi har då en lista för alla transforms som vi ska ändra
    // namnen är lite fel, target_transform är de transforms vi ska ändra, medan target_position och target_rotation är vad det ska bli
    [SerializeField] public Transform[] target_transform = new Transform[120];

    public Vector3[] target_position = new Vector3[120];
    public Quaternion[] target_rotation = new Quaternion[120];

    private void Start()
    {
        // Vi sätter alla targets till vad de transforms vi har nu
        for (int _i = 0; _i < target_transform.Length; _i++)
        {
            if (target_transform[_i] != null)
            {
                target_position[_i] = target_transform[_i].position;
                target_rotation[_i] = target_transform[_i].rotation;
            }
        }

        // Sen tar vi alla childrens rigidbodies, så att vi kan sätta de till kinematic ifall de inte tillhör ägaren
        // Hur det då funkar är att för alla klienter har de andra klienternas objekt i spelet baserat på klienternas transforms och sånt
        // Men vi vill inte påverka fysiken och transforms från en annan klient, därför sätter vi de de representerande klienternas rigidbodies till kinematic så att de inte kan ändras
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
        // Vi kollar först ifall denna objekt tillhör ägaren
        if (IsOwner)
        {
            // Ifall det är så skickar vi positionerna och rotationerna
            // Vi gör det genom en for loop av alla childrens transforms, så skickar vi det då från klienten till servern
            for (int _i = 0; _i < target_transform.Length; _i++)
            {
                if (target_transform[_i] != null)
                {
                    SendTransformServerRpc(target_transform[_i].position, target_transform[_i].rotation, _i);
                }
            }
        }
        else
        {
            // Ifall objektet inte tillhör ägaren ändrar vi då positionerna och rotationerna till vad de ska vara
            // Det funkar då alltså att klienten skickar sin information till servern baserat på sina egna positioner och rotationer
            // Sedan tar man då positionerna och rotationerna skickat från andra klienter, så ändrar vi deras represenativa objekts positioner till de positionerna och rotationer
            for (int _i = 0; _i < target_transform.Length; _i++)
            {
                if (target_transform[_i] != null)
                { 
                    target_transform[_i].position = Vector3.Lerp(target_transform[_i].position, target_position[_i], Time.fixedDeltaTime * 10f);
                    target_transform[_i].rotation = Quaternion.Slerp(target_transform[_i].rotation, target_rotation[_i], Time.fixedDeltaTime * 10f);
                }
            }
        }
    }

    // Den här headern innebär att vi skickar information till servern
    [ServerRpc(RequireOwnership = false)]
    private void SendTransformServerRpc(Vector3 _pos, Quaternion _rot, int _index)
    {
        // Sedan uppdaterar vi då för klienten från servern
        UpdateTransformClientRpc(_pos, _rot, _index);
    }

    // Den här headern innebär att vi skickar information från servern till klienten
    [ClientRpc]
    private void UpdateTransformClientRpc(Vector3 _pos, Quaternion _rot, int _index)
    {
        // Så ifall objektet inte tillhör ägaren, utan en ann klient så är den då ett represeterande objekt av den klienten
        // Då ändrar vi då dens positioner till informationen given av servern
        if (!IsOwner && target_transform[_index] != null)
        {
            target_position[_index] = _pos;
            target_rotation[_index] = _rot;
        }
    }
}