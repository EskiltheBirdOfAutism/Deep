using Unity.Netcode;
using UnityEngine;
using UnityEngine.UIElements;

public class NetworkMeshChild : NetworkBehaviour
{
    /*
    // Det här är varaibler som krävs för att skicka till children av parent
    // Vi har då en lista för alla transforms som vi ska ändra
    // namnen är lite fel, target_transform är de transforms vi ska ändra, medan target_position och target_rotation är vad det ska bli
    [SerializeField] public MeshRenderer[] mesh_current = new MeshRenderer[120];
    public MeshRenderer[] mesh_target = new MeshRenderer[120];
    private void Start()
    {
        // Vi sätter alla targets till vad de transforms vi har nu
        for (int _i = 0; _i < mesh_current.Length; _i++)
        {
            if (mesh_current[_i] != null)
            {
                mesh_target = mesh_current;
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
            for (int _i = 0; _i < mesh_current.Length; _i++)
            {
                if (mesh_current[_i] != null)
                {
                    SendMeshServerRpc(mesh_current[_i], _i);
                }
            }
        }
        else
        {
            // Ifall objektet inte tillhör ägaren ändrar vi då positionerna och rotationerna till vad de ska vara
            // Det funkar då alltså att klienten skickar sin information till servern baserat på sina egna positioner och rotationer
            // Sedan tar man då positionerna och rotationerna skickat från andra klienter, så ändrar vi deras represenativa objekts positioner till de positionerna och rotationer
            for (int _i = 0; _i < mesh_current.Length; _i++)
            {
                if (mesh_current[_i] != null)
                {
                    mesh_current = mesh_target;
                }
            }
        }
    }

    // Den här headern innebär att vi skickar information till servern
    [ServerRpc(RequireOwnership = false)]
    private void SendMeshServerRpc(MeshRenderer _mesh, int _index)
    {
        // Sedan uppdaterar vi då för klienten från servern
        UpdateMeshClientRpc(_mesh, _index);
    }

    // Den här headern innebär att vi skickar information från servern till klienten
    [ClientRpc]
    private void UpdateMeshClientRpc(MeshRenderer _mesh, int _index)
    {
        // Så ifall objektet inte tillhör ägaren, utan en ann klient så är den då ett represeterande objekt av den klienten
        // Då ändrar vi då dens positioner till informationen given av servern
        if (!IsOwner && mesh_current[_index] != null)
        {
            mesh_current[_index] = _mesh;
        }
    }
    */
    
}