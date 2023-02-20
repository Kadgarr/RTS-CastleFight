using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class BuildingSelectionHandler : MonoBehaviour
{

    [SerializeField] private RectTransform buildingSelectionArea = null;
    [SerializeField] private Image infoImageBuilding = null;

    [SerializeField] private LayerMask layerMask = new LayerMask();

    private Vector2 startPosition;
    private RTSPlayer player;
    private Camera mainCamera;

    public List<Building> SelectedBuildings { get; } = new List<Building> ();

    private void Start()
    {
        mainCamera = Camera.main;

        Building.AuthorityOnBuildingDespawned += AuthorityHadnleBuildingDespawned;
        player = NetworkClient.connection.identity.GetComponent<RTSPlayer>();
        GameOverHandler.ClientOnGameOver += ClientHandleGameOver;
    }

    private void OnDestroy()
    {
        Building.AuthorityOnBuildingDespawned -= AuthorityHadnleBuildingDespawned;
        GameOverHandler.ClientOnGameOver -= ClientHandleGameOver;
    }
    private bool IsOverUI()
    {

        if (EventSystem.current.IsPointerOverGameObject())
        {
            PointerEventData pointerData = new PointerEventData(EventSystem.current)
            {
                pointerId = -1,

            };


            pointerData.position = Input.mousePosition;

            List<RaycastResult> results = new List<RaycastResult>();

            EventSystem.current.RaycastAll(pointerData, results);
            if (results.Count > 0)
            {
                for (int i = 0; i < results.Count; ++i)
                {
                    if (results[i].gameObject.CompareTag("UI"))
                        return true;
                }

            }


            return false;
        }

        return false;

    }
    private void Update()
    {
      
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            if (IsOverUI()) return;
            StartSelectionArea();
        }
     
        else if (Mouse.current.leftButton.wasReleasedThisFrame) ClearSelectionArea();
        else if (Mouse.current.leftButton.isPressed) UpdateSelectionArea(); 
    }

    private void StartSelectionArea()
    {
        if (!Keyboard.current.leftShiftKey.isPressed)
        {
            foreach (Building selectedBuilding in SelectedBuildings)
            {
                selectedBuilding.Deselect();
                infoImageBuilding.gameObject.SetActive(false);  
            }
            SelectedBuildings.Clear();
        }


        buildingSelectionArea.gameObject.SetActive(true);

        startPosition = Mouse.current.position.ReadValue();

        UpdateSelectionArea();

    }

    private void ClientHandleGameOver(string winnerName)
    {
        enabled = false;
    }

    private void UpdateSelectionArea()
    {
        Vector2 mousePosition = Mouse.current.position.ReadValue ();

        float areaWidth = mousePosition.x - startPosition.x;
        float areaHeight = mousePosition.y - startPosition.y;

        buildingSelectionArea.sizeDelta = new Vector2(Mathf.Abs(areaWidth), Mathf.Abs(areaHeight));
        buildingSelectionArea.anchoredPosition = startPosition + new Vector2(areaWidth/2, areaHeight/2);


    }


    private void ClearSelectionArea()
    {
        buildingSelectionArea.gameObject.SetActive(false);

        if (buildingSelectionArea.sizeDelta.magnitude == 0)
        {
            Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());

            if (!Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, layerMask)) return;

            if (!hit.collider.TryGetComponent<Building>(out Building building)) return;

            if (!building.hasAuthority) return;

            SelectedBuildings.Add(building);

            foreach (Building selectedBuilding in SelectedBuildings)
            {
                selectedBuilding.Select();
                infoImageBuilding.gameObject.SetActive(true);
            }

            return;
        }

        Vector2 min = buildingSelectionArea.anchoredPosition - (buildingSelectionArea.sizeDelta / 2);
        Vector2 max = buildingSelectionArea.anchoredPosition + (buildingSelectionArea.sizeDelta / 2);

        foreach(Building buildings in player.GetMyBuildings())
        {
            if (SelectedBuildings.Contains(buildings)) continue;

            Vector3 screenPosition = mainCamera.WorldToScreenPoint(buildings.transform.position);

            if(screenPosition.x>min.x 
                && screenPosition.x<max.x 
                && screenPosition.y>min.y 
                && screenPosition.y<max.y)
            {
                SelectedBuildings.Add(buildings);
                buildings.Select();
                infoImageBuilding.gameObject.SetActive(true);
            }
        }
    }

    //удаляет строение из списка выбранных строений
    private void AuthorityHadnleBuildingDespawned(Building building)
    {
        SelectedBuildings.Remove(building);
        infoImageBuilding.gameObject.SetActive(false);
    }
}
