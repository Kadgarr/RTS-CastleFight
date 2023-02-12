using Mirror;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.ProBuilder.Shapes;
using UnityEngine.UI;
using UnityEngineInternal;
using Color = UnityEngine.Color;

public class BuildingButton : MonoBehaviour
{
    [SerializeField] private Building building = null;
    [SerializeField] private Image iconImage=null;
    [SerializeField] private TMP_Text priceText = null;
    [SerializeField] private LayerMask floorMask = new LayerMask();

    private Camera mainCamera;
    private BoxCollider buildingCollider;
    private RTSPlayer player;
    private GameObject buildingPreviewInstance;
    private Renderer buildingRendererInstance;
    private Color normColor;

    private void Start()
    {
        mainCamera = Camera.main;
       
        iconImage.sprite = building.GetIcon();
        priceText.text = building.GetPrice().ToString();

        player = NetworkClient.connection.identity.GetComponent<RTSPlayer>();

        buildingCollider = building.GetComponent<BoxCollider>();
    }

    private void Update()
    {

        if (buildingPreviewInstance == null) return;
        
        UpdateBuildingPreview();

        PlaceBuilding();

    }

    public void Build()
    {
       
        //if (!Mouse.current.leftButton.wasPressedThisFrame) return;

        if (player.GetResources() < building.GetPrice()) return;
       
        buildingPreviewInstance = Instantiate(building.GetBuildingPreview());

        buildingRendererInstance = buildingPreviewInstance.GetComponentInChildren<MeshRenderer>();


        buildingPreviewInstance.SetActive(false);


        normColor = buildingRendererInstance.material.color;
    }

    public void PlaceBuilding()
    {
        if (buildingPreviewInstance == null) return;


        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
            
            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, floorMask))
            {
                if(hit.collider.TryGetComponent<TeamNumberArea>(out TeamNumberArea teamNumberArea))
                {
                    if (teamNumberArea.GetTeamNumber() == player.GetTeamNumber() && hit.collider.gameObject.tag!="WallArea")
                    {
                        RTSPlayer player = NetworkClient.connection.identity.GetComponent<RTSPlayer>();
                        player.CmdTryPlaceBuilding(building.GetId(), hit.point);
                    }
                }

            }

            Destroy(buildingPreviewInstance);
        }
       
        if (Mouse.current.rightButton.wasPressedThisFrame)
        {
            Destroy(buildingPreviewInstance);
        }
    }

    private void UpdateBuildingPreview()
    {

        Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());

        if (!Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, floorMask)) { return; }

        buildingPreviewInstance.transform.position = hit.point;

        if (!buildingPreviewInstance.activeSelf)
        {
            buildingPreviewInstance.SetActive(true);
        }

        //����� ����� ��� ���������� ������ (��������� �� �����������)

        //Debug.Log(player.CanPlaceBuilding(buildingCollider, hit.point).ToString());
        //Debug.Log($"Physics.Raycast:{Physics.Raycast(ray, out RaycastHit hitt, Mathf.Infinity, floorMask)}");

        Color color = player.CanPlaceBuilding(buildingCollider, hit.point) ? normColor : Color.red;

        buildingRendererInstance.material.SetColor("_BaseColor", color);
    }

   
}
