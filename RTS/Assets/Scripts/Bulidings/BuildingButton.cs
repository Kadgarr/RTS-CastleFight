using Mirror;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.ProBuilder.Shapes;
using UnityEngine.UI;

public class BuildingButton : MonoBehaviour//,IPointerUpHandler,IPointerDownHandler
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

        buildingCollider = building.GetComponent<BoxCollider>();
    }

    private void Update()
    {
        if (player == null)
        {
            player = NetworkClient.connection.identity.GetComponent<RTSPlayer>();
        }

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
               player.CmdTryPlaceBuilding(building.GetId(), hit.point);
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

        Debug.Log($"color: {buildingRendererInstance.material.color}");

        
        Color color = player.CanPlaceBuilding(buildingCollider, hit.point) ? normColor : Color.red;

        buildingRendererInstance.material.SetColor("_Color", color);


    }

    //public void OnPointerDown(PointerEventData eventData)
    //{
    //    if (eventData.button != PointerEventData.InputButton.Left) return;

    //    buildingPreviewInstance = Instantiate(building.GetBuildingPreview());
    //    buildingRendererInstance = buildingPreviewInstance.GetComponentInChildren<Renderer>();

    //    buildingPreviewInstance.SetActive(false);
    //}

    //public void OnPointerUp(PointerEventData eventData)
    //{
    //    if (buildingPreviewInstance == null) return;

    //    Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());

    //    if(Physics.Raycast(ray,out RaycastHit hit, Mathf.Infinity, floorMask))
    //    {
    //        //place building
    //    }

    //    Destroy(buildingPreviewInstance);
    //}

}
