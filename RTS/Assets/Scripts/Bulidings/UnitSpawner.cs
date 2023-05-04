using Mirror;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UnitSpawner : NetworkBehaviour//, IPointerClickHandler
{
    [SerializeField] private Health health = null;
    [SerializeField] private Unit unitPreab = null;
    [SerializeField] private Transform unitSpawnPoint = null;
    [SerializeField] private TMP_Text remainingUnitsText = null;
    [SerializeField] private Image unitProgressImage;
    [SerializeField] private int maxUnitQueue = 5;
    [SerializeField] private float spawnMoveRange = 7;
    [SerializeField] private float unitSpawnDuration = 5f;

    [SyncVar(hook =nameof(ÑlientHandleQueuedUnitsUpdated))]
    private int queuedUnits;

    [SyncVar]
    private float unitTimer;
    [SyncVar]
    private bool isProducingUnits=true;

    #region Server

    float newProgress;
    private float progressImageVelocity;

    private void Start()
    {
        unitProgressImage.fillAmount = 0f;
    }
    private void Update()
    {
        if (isServer)
        {
            if(isProducingUnits)
            ProduceUnits();
        }

        if (isClient)
        {
            UpdateTimerDisplay();
        }
    }

   
    public override void OnStartServer()
    {
        health.ServerOnDie += ServerHandleOnDie;
    }
    public override void OnStopServer()
    {
        health.ServerOnDie += ServerHandleOnDie;
    }

    [Server]
    private void ServerHandleOnDie()
    {
        NetworkServer.Destroy(gameObject);
    }

    [Server]
    private void ProduceUnits()
    {
        //if (queuedUnits == 0) return;
        unitTimer += Time.deltaTime;

        if (unitTimer < unitSpawnDuration)  return;

        GameObject unitInstance = Instantiate(
          unitPreab.gameObject,
          unitSpawnPoint.position,
          unitSpawnPoint.rotation);

        NetworkServer.Spawn(unitInstance, connectionToClient);

        Vector3 spawnOffSet = Random.insideUnitSphere * spawnMoveRange;

        spawnOffSet.y = unitSpawnPoint.position.y;

        UnitMovement unitMovement=unitInstance.GetComponent<UnitMovement>();
        unitMovement.ServerMove(unitSpawnPoint.position+spawnOffSet);

       // queuedUnits--;
        unitTimer = 0f;
    }

    [Command]
    private void CmdSpawnUnit()
    {
      if(queuedUnits == maxUnitQueue) return;

        RTSPlayer player = connectionToClient.identity.GetComponent<RTSPlayer>();

        if (player.GetGoldResources()< unitPreab.GetResourceCost()) return;

        queuedUnits++;

        player.SetResourcesGold(player.GetGoldResources() - unitPreab.GetResourceCost());
    }

    
    private void ÑlientHandleQueuedUnitsUpdated(int oldUnits, int newUnits)
    {
        remainingUnitsText.text = newUnits.ToString();
    }
    #endregion

    #region Client

    private void UpdateTimerDisplay()
    {
        newProgress = unitTimer / unitSpawnDuration;
        //unitProgressImage.fillAmount = newProgress;
        if (newProgress < unitProgressImage.fillAmount)
        {
            unitProgressImage.fillAmount = newProgress;
        }
        else
        {
            unitProgressImage.fillAmount = Mathf.SmoothDamp(
                unitProgressImage.fillAmount,
                newProgress,
                ref progressImageVelocity,
                0.1f
                );
        }
    }

    //public void OnPointerClick(PointerEventData eventData)
    //{
    //    if (eventData.button != PointerEventData.InputButton.Left) { return; }

    //    if (!hasAuthority) return;

    //    CmdSpawnUnit();
    //}

    #endregion

    #region Client
    public float GetNewProgressDuration()
    {
        return (int)unitTimer+1;
    }

    public int GetSpawnDuration()
    {
        return (int)unitSpawnDuration;
    }

    public GameObject GetUnit()
    {
        return unitPreab.gameObject;
    }

    [Command]
    public void SetTrueProducingUnits()
    {
        isProducingUnits = true;
    }
    [Command]
    public void SetFalseProducingUnits()
    {
        isProducingUnits = false;
        unitProgressImage.fillAmount = 0f;
        newProgress = 0f;
        unitTimer = 0f;
    }
    #endregion

}
