using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class CorpseRessur : NetworkBehaviour
{
    [SerializeField]
    private GameObject unitToSpawn = null;

    [Server]
    public void Ressurection()
    {
        GameObject unitInstance = null;

        unitInstance = Instantiate(unitToSpawn, this.gameObject.transform.position, this.gameObject.transform.rotation);

        NetworkServer.Spawn(unitInstance, connectionToClient);

        NetworkServer.Destroy(this.gameObject);
    }

}
