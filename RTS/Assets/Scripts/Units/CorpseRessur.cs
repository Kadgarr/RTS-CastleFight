using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class CorpseRessur : NetworkBehaviour
{
    [SerializeField]
    private GameObject unitToSpawn = null;

    private int fullTimeToDestroy = 30;
    private float currentTimeToDestroy = 0;

    [Server]
    public void Ressurection()
    {
        GameObject unitInstance = null;

        unitInstance = Instantiate(unitToSpawn, this.gameObject.transform.position, this.gameObject.transform.rotation);

        NetworkServer.Spawn(unitInstance, connectionToClient);

        NetworkServer.Destroy(this.gameObject);
    }

    private void Update()
    {
        if (currentTimeToDestroy < fullTimeToDestroy)
        {
            currentTimeToDestroy += Time.deltaTime;
            return;
        }
            

        Debug.LogError("Corpse destroyed");
        NetworkServer.Destroy(this.gameObject);
    }
}
