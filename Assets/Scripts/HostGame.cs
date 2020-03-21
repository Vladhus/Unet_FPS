using UnityEngine.Networking;
using UnityEngine;

public class HostGame : MonoBehaviour
{
    [SerializeField]
    private uint roomSize = 6;
    private string roomName;
    private NetworkManager networkmanager;

    void Start()
    {
        networkmanager = NetworkManager.singleton;
        if (networkmanager.matchMaker == null)
        {
            networkmanager.StartMatchMaker();
        }
    }
    public void SetRoomName(string _name)
    {
        roomName = _name;
    }

    public void CreateRoom()
    {
        if (roomName != "" && roomName != null)
        {
            Debug.Log("Creating a room: " + roomName + "with room for " + roomSize + "players."); 
            //Create room
            networkmanager.matchMaker.CreateMatch(roomName, roomSize, true,"","", "",0,0, networkmanager.OnMatchCreate);
        }
    }

}
