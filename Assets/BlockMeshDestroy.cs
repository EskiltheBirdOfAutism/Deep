using Unity.Netcode;
using UnityEngine;

public class BlockMeshDestroy : MonoBehaviour
{
    [SerializeField] private float room_size = 14f;
    private bool[] block_exist = new bool[49];
    [SerializeField] private GameObject roomblock;
    public Vector2 index = new Vector2(0, 0);
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        for (int _i = 0; _i < room_size / 2; _i++)
        {
            for (int _j = 0; _j < room_size / 2; _j++)
            {
                block_exist[_i + (_j * ((int)room_size / 2))] = true;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (NetworkManager.Singleton.IsHost == true)
        {
            if (collision.gameObject.GetComponent<Tool>() != null)
            {
                if (collision.gameObject.GetComponent<Tool>().tool == ToolType.Pickaxe && collision.gameObject.GetComponent<Tool>().isEquiped)
                {
                    Vector3 _col_point = collision.contacts[0].point;

                    for (int _i = 0; _i < room_size / 2; _i++)
                    {
                        for (int _j = 0; _j < room_size / 2; _j++)
                        {
                            Vector3 _pos = transform.position + new Vector3(_i, 0, _j);
                            CombineInstance[] _block_id = new CombineInstance[49];
                            Material _material = roomblock.GetComponent<MeshRenderer>().sharedMaterial;

                            if (_col_point.x >= _pos.x && _col_point.x < _pos.x + 1
                            && _col_point.z >= _pos.z && _col_point.z < _pos.z + 1)
                            {
                                block_exist[_i + (_j * (int)room_size / 2)] = false;
                                for (int _k = 0; _k < room_size / 2; _k++)
                                {
                                    for (int _l = 0; _l < room_size / 2; _l++)
                                    {
                                        if (block_exist[_k + (_l * (int)room_size / 2)] == true)
                                        {
                                            roomblock.transform.position = new Vector3(0.5f + _k, 0, 0.5f + _l);
                                            _block_id[_k + (_l * (int)room_size / 2)].mesh = roomblock.GetComponent<MeshFilter>().sharedMesh;
                                            _block_id[_k + (_l * (int)room_size / 2)].transform = roomblock.transform.localToWorldMatrix;
                                        }
                                    }
                                }

                                Mesh _new_mesh = new Mesh();
                                _new_mesh.CombineMeshes(_block_id);
                                GetComponent<MeshFilter>().sharedMesh = _new_mesh;
                                GetComponent<MeshRenderer>().sharedMaterial = _material;
                                GetComponent<MeshCollider>().sharedMesh = _new_mesh;

                                foreach (ulong _client_id in NetworkManager.Singleton.ConnectedClientsIds)
                                {
                                    Mesh _mesh = GetComponent<MeshFilter>().mesh;
                                    if(GameObject.Find("RoomGenerator(Clone)")) GameObject.Find("RoomGenerator(Clone)").GetComponent<RoomGeneratorCode>().UpdateClientMeshIdClientRpc(_client_id,
                                    gameObject.GetComponent<NetworkObject>(), index, _mesh.triangles, _mesh.normals, _mesh.vertices);
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
