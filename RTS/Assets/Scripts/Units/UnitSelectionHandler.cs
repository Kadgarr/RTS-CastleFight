using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

public class UnitSelectionHandler : MonoBehaviour
{

    [SerializeField] private RectTransform unitSelectionArea = null;

    [SerializeField] private LayerMask layerMask = new LayerMask();

    private Vector2 startPosition;
    private RTSPlayer player;
    private Camera mainCamera;

    public static event Action ClearBuildingSelectionUpdated;

    public List<Unit> SelectedUnits { get; } = new List<Unit> ();

    private void Start()
    {
        mainCamera = Camera.main;

        Unit.AuthorityOnUnitDespawned += AuthorityHadnleUnitDespawned;
        BuildingSelectionHandler.ClearUnitSelectionUpdated += HandleClearSelectionUpdated;
        GameOverHandler.ClientOnGameOver += ClientHandleGameOver;

        player = NetworkClient.connection.identity.GetComponent<RTSPlayer>();
    }


    private void OnDestroy()
    {
        Unit.AuthorityOnUnitDespawned -= AuthorityHadnleUnitDespawned;
        GameOverHandler.ClientOnGameOver -= ClientHandleGameOver;
        BuildingSelectionHandler.ClearUnitSelectionUpdated -= HandleClearSelectionUpdated;
    }
    private bool IsOverUIandGROUND()
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
                    if (results[i].gameObject.CompareTag("UI") ||  results[i].gameObject.CompareTag("BuildingArea"))
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
            if (IsOverUIandGROUND()) return;
            StartSelectionArea();
        }
        else if (Mouse.current.leftButton.wasReleasedThisFrame) ClearSelectionArea();
        else if (Mouse.current.leftButton.isPressed) UpdateSelectionArea(); 
    }

    private void StartSelectionArea()
    {
        if (!Keyboard.current.leftShiftKey.isPressed)
        {
            foreach (Unit selectedUnit in SelectedUnits)
            {
                selectedUnit.Deselect();
                selectedUnit.SetActiveCanvasInfo(false);
                ClearBuildingSelectionUpdated.Invoke();
            }
            SelectedUnits.Clear();
        }

        unitSelectionArea.gameObject.SetActive(true);

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

        unitSelectionArea.sizeDelta = new Vector2(Mathf.Abs(areaWidth), Mathf.Abs(areaHeight));
        unitSelectionArea.anchoredPosition = startPosition + new Vector2(areaWidth/2, areaHeight/2);
    }


    private void ClearSelectionArea()
    {
        unitSelectionArea.gameObject.SetActive(false);
        
        if (unitSelectionArea.sizeDelta.magnitude == 0)
        {
            Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());

            if (!Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, layerMask)) return;

            if (!hit.collider.TryGetComponent<Unit>(out Unit unit)) return;

            // if (!unit.hasAuthority) return;

            StartSelectionArea();

            SelectedUnits.Add(unit);

            int i = 0;
            foreach (Unit selectedUnit in SelectedUnits)
            {
                selectedUnit.Select();

                if (i != 0)
                    SelectedUnits[i - 1].SetActiveCanvasInfo(false);

                selectedUnit.SetActiveCanvasInfo(true);
                ClearBuildingSelectionUpdated.Invoke();
                i++;
            }

            return;
        }

        Vector2 min = unitSelectionArea.anchoredPosition - (unitSelectionArea.sizeDelta / 2);
        Vector2 max = unitSelectionArea.anchoredPosition + (unitSelectionArea.sizeDelta / 2);


        int j = 0;
        //Debug.Log($"SelectedUnits count: {SelectedUnits.Count};\nplayer.GetMyUnits() count: {player.GetMyUnits().Count}");

        foreach(Unit unit in player.GetMyUnits())
        {
            if (SelectedUnits.Contains(unit)) continue;

            Vector3 screenPosition = mainCamera.WorldToScreenPoint(unit.transform.position);

            if(screenPosition.x>min.x 
                && screenPosition.x<max.x 
                && screenPosition.y>min.y 
                && screenPosition.y<max.y)
            {
                SelectedUnits.Add(unit);

                unit.Select();

                if (j != 0)
                    SelectedUnits[j - 1].SetActiveCanvasInfo(false);
                    
                unit.SetActiveCanvasInfo(true);
                ClearBuildingSelectionUpdated.Invoke();
                j++;
            }
        }
    }

    //удаляет юниты из списка выбранных юнитов
    private void AuthorityHadnleUnitDespawned(Unit unit)
    {
        SelectedUnits.Remove(unit);
        unit.SetActiveCanvasInfo(false);
    }

    private void HandleClearSelectionUpdated()
    {
        foreach (Unit selectedUnit in SelectedUnits)
        {
            selectedUnit.Deselect();
            selectedUnit.SetActiveCanvasInfo(false);
        }
        SelectedUnits.Clear();
    }
}
