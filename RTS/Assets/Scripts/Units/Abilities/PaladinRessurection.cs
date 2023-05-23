using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaladinRessurection : NetworkBehaviour
{
    [Server]
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag!="Corpse")
            return;

        if(other.TryGetComponent<NetworkIdentity>(out NetworkIdentity networkIdentity))
        {
            if (networkIdentity.connectionToClient.identity.GetComponent<RTSPlayer>().GetTeamNumber() != 
                connectionToClient.identity.GetComponent<RTSPlayer>().GetTeamNumber())
            {
                return;
            }

            if(other.TryGetComponent<CorpseRessur>(out CorpseRessur corpse))
            {
                corpse.Ressurection();
            }

        }
    }

}
