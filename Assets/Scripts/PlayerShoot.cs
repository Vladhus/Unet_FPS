using UnityEngine.Networking;
using UnityEngine;



[RequireComponent(typeof(WeaponManager))]
public class PlayerShoot : NetworkBehaviour
{
    private const string PLAYER_TAG = "Player";
         

    [SerializeField]
    private Camera cam;

    [SerializeField]
    private LayerMask mask;

    private PlayerWeapon currentWeapon;
    private WeaponManager weaponManager;


     

    void Start()
    {
        if (cam == null)
        {
            Debug.LogError("PlayerShoot: No Camera");
            this.enabled = false;
        }
        weaponManager = GetComponent<WeaponManager>();
       
    }

    void Update()
    {

        if (HusStop.IsHus)
        {
            return;
        }
        currentWeapon = weaponManager.GetCurrentWeapon();
        if (currentWeapon.fireRate <= 0f)
        {
            if (Input.GetButtonDown("Fire1"))
            {
                Shoot();
            }
        }
        else
        {
            if (Input.GetButtonDown("Fire1"))
            {
                InvokeRepeating("Shoot", 0f, 1/currentWeapon.fireRate);
            }else if(Input.GetButtonUp("Fire1"))
            {
                CancelInvoke("Shoot");
            }
        }
        
    }

    //Is called on the server when a player shoots
    [Command]
    void CmdOnShoot()
    {
        RpcDoShootEffect();
    }

    //IS called on all clients when we need to do a shoot efect
    [ClientRpc]
    void RpcDoShootEffect()
    {
        weaponManager.GetCurrentGraphics().muzzleFlash.Play();
    }



    //is called on the server when we hit something
    //Takes the hit point and the normal of the surface 
    [Command]
    void CmdOnHit(Vector3 _pos,Vector3 _normal)
    {
        RpcDoHitEffect(_pos, _normal); 
    }

    //Is call on all clients ,here we can spawn in coll effects
    [ClientRpc]
    void RpcDoHitEffect(Vector3 _pos, Vector3 _normal)
    {
       GameObject _hitEffect=(GameObject) Instantiate(weaponManager.GetCurrentGraphics().hitEffectPrefab, _pos, Quaternion.LookRotation(_normal));
        Destroy(_hitEffect, 2f); 
    }


    [Client]
    void Shoot()
    {


        if(!isLocalPlayer)
        {
            return;
        }
        //WE are shooting call the OnShoot metod on the server
        CmdOnShoot();
        RaycastHit _hit;
        if (Physics.Raycast(cam.transform.position,cam.transform.forward,out _hit, currentWeapon.range,mask))
        {
            
            if (_hit.collider.tag == PLAYER_TAG)
            {
                CmdPlayerShoot(_hit.collider.name, currentWeapon.damage);
            }
            //We hit something ,call the hit metod on the server
            CmdOnHit(_hit.point, _hit.normal);
        }

    }


    [Command]
    void CmdPlayerShoot(string _player_ID,int _damage)
    {
        Debug.Log(_player_ID + "Has been shoot");
       Player _player =  GameManeger.GetPLayer(_player_ID);
        _player.RpcTakeDamage(_damage);
    }
}
