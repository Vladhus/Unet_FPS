using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;



[RequireComponent(typeof(PlayerSetup))]
public class Player : NetworkBehaviour
{
    
    
    [SyncVar]
    private bool _isDead = false;
    public bool isDead
    {
        get { return _isDead; }
        protected set { _isDead = value; }
    }

    [SerializeField]
    private int maxHealth = 100;

    [SyncVar]
    private int currentHealth;

    [SerializeField]
    private Behaviour[] disableOnDeath;

    

    private bool[] wasEnabled;

    [SerializeField]
    private GameObject[] disableGameObjectsOnDeath;

    [SerializeField]
    private GameObject deathEffect;

    [SerializeField]
    private GameObject spawnEffect;

    private bool firstSetup = true;


    public void SetupPlayer ()
    {
        if (isLocalPlayer)
        {
            GameManeger.instance.SetSceneCameraActive(false);
            GetComponent<PlayerSetup>().playerUIInstance.SetActive(true);
        }
        
        
        CmdBroadcastNewPlayerSetup();

    }
    [Command]
    private void CmdBroadcastNewPlayerSetup()
    {
        RpcSetupPlayerOnAllClients();
    }
    [ClientRpc]
    private void RpcSetupPlayerOnAllClients()
    {
        if (firstSetup)
        {
            wasEnabled = new bool[disableOnDeath.Length];
            for (int i = 0; i < wasEnabled.Length; i++)
            {
                wasEnabled[i] = disableOnDeath[i].enabled;
            }
            firstSetup = false;
        }
        
        SetDefaults();

    }

    /*void Update()
    {
        if (!isLocalPlayer)
        {
            return;
        }
        if (Input.GetKeyDown(KeyCode.K))
        {
            RpcTakeDamage(9999);
        }
    }*/

    [ClientRpc]
    public void RpcTakeDamage(int _amount)
    {
        if (_isDead)
        {
            return;
        }
        currentHealth -= _amount;

        Debug.Log(transform.name + "Now have" + currentHealth + " health.");
        if (currentHealth <= 0)
        {
            Die();
        } 
    }

    private void Die()
    {
        isDead = true;

        //Disable components
        for (int i = 0; i < disableOnDeath.Length; i++)
        {
            disableOnDeath[i].enabled = false;
        }
        //Disable Gameobjects
        for (int i = 0; i < disableGameObjectsOnDeath.Length; i++)
        {
            disableGameObjectsOnDeath[i].SetActive(false);
        }
        //Disable collider
        Collider _col = GetComponent<Collider>();
        if (_col != null)
        {
            _col.enabled = false;
        }

        //Spawn death effect
        GameObject _gfxins=(GameObject) Instantiate(deathEffect, transform.position, Quaternion.identity);
        Destroy(_gfxins, 3f);

        if (isLocalPlayer)
        {
            GameManeger.instance.SetSceneCameraActive(true);
            GetComponent<PlayerSetup>().playerUIInstance.SetActive(false);
        }
        Debug.Log(transform.name + "IS DEAD");
        StartCoroutine(Respawn());
        
    }
    private IEnumerator Respawn()
    {
        yield return new WaitForSeconds(GameManeger.instance.matchSettings.respawnTime);

        
        Transform _spawnPoint = NetworkManager.singleton.GetStartPosition();
        transform.position = _spawnPoint.position;
        transform.rotation = _spawnPoint.rotation;

        yield return new WaitForSeconds(0.1f);
        

        SetupPlayer();
    }

    public void SetDefaults()
    {
        isDead = false;
        currentHealth = maxHealth;

        //Set components active
        for (int i = 0; i < disableOnDeath.Length; i++)
        {
            disableOnDeath[i].enabled = wasEnabled[i];
        }

        //Set gameobjects active
        for (int i = 0; i < disableGameObjectsOnDeath.Length; i++)
        {
            disableGameObjectsOnDeath[i].SetActive(true);
        }


        //Enable collider
        Collider _col = GetComponent<Collider>();
        if (_col != null)
        {
            _col.enabled = true;
        }

       

        //Create spawn effect
        GameObject _gfxins = (GameObject)Instantiate(spawnEffect, transform.position, Quaternion.identity);
        Destroy(_gfxins, 3f);

    }
}
