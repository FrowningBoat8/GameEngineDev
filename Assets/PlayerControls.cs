using System.Collections;
using System;
using UnityEngine;

public class PlayerControls : MonoBehaviour
{
    float horizontalInput;
    float verticalInput;
    Rigidbody physics;
    [SerializeField] int movementSpeed = 1500;
    [SerializeField] BuildingManager buildingManager;
    [SerializeField] GameObject buildGuide;
    PlayerMenus playerMenus;
    public enum buildings{NOTHING, TREE_PLANTER, STORAGE_TIER_1, CRAFTING_TIER_1, BASIC_TOWER}
    public Joystick joystick;
    ToggleMobileControls mobileControls;
    [SerializeField] GameObject uiBlock;
    [SerializeField] GameObject inventoryUIBlock;
    public GameObject menuUIBlock;

    //Buildings
    [Header("Building Objects")]
    [SerializeField] GameObject treePlanter;
    [SerializeField] GameObject storageTier1;
    [SerializeField] GameObject crafterTier1;
    [SerializeField] GameObject basicTower1;

    //Meshs
    [Header("Building Meshes")]
    [SerializeField] Mesh treePlanterMesh;
    [SerializeField] Mesh storageTier1Mesh;
    [SerializeField] Mesh crafterTier1Mesh;
    [SerializeField] Mesh basicTowerMesh;
    void Start()
    {
        physics = GetComponent<Rigidbody>();
        playerMenus = GetComponent<PlayerMenus>();
        mobileControls = GetComponent<ToggleMobileControls>();
        //Sets the uiObject to block unwanted inputs
        int aspect = (int)(Camera.main.aspect * 10f);
        if (aspect == 17)
        {
            uiBlock.transform.localScale = new Vector3(12F, 1F, 1.48F);
            inventoryUIBlock.transform.localScale = new Vector3(2.85F, 1F, 2.25F);
        } else if (aspect == 20){
            uiBlock.transform.localScale = new Vector3(12F, 1F, 1.65F);
            inventoryUIBlock.transform.localScale = new Vector3(3.2F, 1F, 2.61F);
        } else if (aspect == 21){
            uiBlock.transform.localScale = new Vector3(12F, 1F, 1.75F);
            inventoryUIBlock.transform.localScale = new Vector3(3.45F, 1F, 2.89F);
        } else if (aspect > 21){
            uiBlock.transform.localScale = new Vector3(12F, 1F, 1.8F);
            inventoryUIBlock.transform.localScale = new Vector3(3.75F, 1F, 3.16F);
        }
    }

    void Update()
    {
        Movement();
        OnMouse();
        if (!playerMenus.buildingToBeBuilt.Equals(buildings.NOTHING))
        {
            ShowBuildPos();
            RotateBuildPos();
        }
    }

    void Movement()
    {
        if (mobileControls.isMobileControls)
        {
            horizontalInput = joystick.Horizontal;
            verticalInput = joystick.Vertical;
            movementSpeed = 4500;
        }
        else 
        {
            horizontalInput = Input.GetAxis("Horizontal");
            verticalInput = Input.GetAxis("Vertical");
            movementSpeed = 4200;
        }
        physics.AddRelativeForce(movementSpeed * horizontalInput * Time.deltaTime, 0, movementSpeed * verticalInput * Time.deltaTime);
    }

    void OnMouse()
    {
        if (Input.GetMouseButtonDown(0) && !mobileControls)
        {
            OnClick();
        } else if  (Input.GetMouseButtonUp(0) && mobileControls)
        {
            OnClick();
        }

    }

    void OnClick()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, 100, -5, QueryTriggerInteraction.Ignore)) {
            if (hit.collider.CompareTag("UIArea")) {return;}
            if (hit.collider.CompareTag("Citizen")) {return;}
            if (hit.collider.CompareTag("Buildable") && !playerMenus.buildingToBeBuilt.Equals(buildings.NOTHING))
            {
                if (playerMenus.CheckCanBuild())
                {
                    Build(hit);
                }
            }
            else
            {
                playerMenus.CheckClicked(hit);
            }
                
        }
    }

    void Build(RaycastHit hit)
    {
        //Vector3 buildPos = new Vector3(RoundToNearest(hit.point.x,2), RoundToNearest(hit.point.y,2), RoundToNearest(hit.point.z,2));
        Vector3 buildPos = buildGuide.transform.position;
        GameObject objectBuilt;
        switch(playerMenus.buildingToBeBuilt)
        {
            case buildings.TREE_PLANTER: objectBuilt = Instantiate(treePlanter,buildPos,buildGuide.transform.rotation,buildingManager.transform); break;
            case buildings.STORAGE_TIER_1: objectBuilt = Instantiate(storageTier1,buildPos,buildGuide.transform.rotation,buildingManager.transform); break;
            case buildings.CRAFTING_TIER_1: objectBuilt = Instantiate(crafterTier1,buildPos,buildGuide.transform.rotation,buildingManager.transform); break;
            case buildings.BASIC_TOWER: objectBuilt = Instantiate(basicTower1,buildPos,buildGuide.transform.rotation,buildingManager.transform); break;
            default: return;
        }
        objectBuilt.name = objectBuilt.name.Replace("(Clone)", "");
        objectBuilt.GetComponent<BuildingScript>().buildingType = playerMenus.buildingToBeBuilt;
        buildingManager.AddBuildingToList(objectBuilt, playerMenus.buildingToBeBuilt);
    }

    void ShowBuildPos()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, 100, -5, QueryTriggerInteraction.Ignore)) {
                if (hit.collider.CompareTag("Buildable") && !playerMenus.buildingToBeBuilt.Equals(buildings.NOTHING))
                {
                    Vector3 buildPos = new Vector3();
                    switch(playerMenus.buildingToBeBuilt)
                    {
                        case buildings.TREE_PLANTER: buildPos = new Vector3(RoundToNearest(hit.point.x,2), RoundToNearest(hit.point.y,2), RoundToNearest(hit.point.z,2)); break;
                        case buildings.STORAGE_TIER_1: buildPos = new Vector3(RoundToNearest(hit.point.x,2), RoundToNearest(hit.point.y,2), (RoundToNearest(hit.point.z-1,2)+1)); break;
                        case buildings.CRAFTING_TIER_1: buildPos = new Vector3(RoundToNearest(hit.point.x,2), RoundToNearest(hit.point.y,2), (RoundToNearest(hit.point.z-1,2)+1)); break;
                        case buildings.BASIC_TOWER: buildPos = new Vector3(RoundToNearest(hit.point.x,2), RoundToNearest(hit.point.y,2), (RoundToNearest(hit.point.z,2))); break;
                    }
                    buildGuide.transform.position = buildPos;
                }
            }
    }
    
    void RotateBuildPos()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            buildGuide.transform.Rotate(new Vector3(0,90,0), Space.Self);
        }
    }

    float RoundToNearest(float value, float num)
    {
        return (Mathf.RoundToInt(value/num) * num);
    }

    public void DisplayBuildGuide(bool isActive, buildings buildingToBeBuilt)
    {
        buildGuide.SetActive(isActive);
        SetBuildGuideMesh(buildingToBeBuilt);
    }

    void SetBuildGuideMesh(buildings buildingToBeBuilt)
    {
        switch(buildingToBeBuilt)
        {
            case buildings.TREE_PLANTER: buildGuide.GetComponent<MeshFilter>().mesh = treePlanterMesh; break;
            case buildings.STORAGE_TIER_1: buildGuide.GetComponent<MeshFilter>().mesh = storageTier1Mesh; break;
            case buildings.CRAFTING_TIER_1: buildGuide.GetComponent<MeshFilter>().mesh = crafterTier1Mesh; break;
            case buildings.BASIC_TOWER: buildGuide.GetComponent<MeshFilter>().mesh = basicTowerMesh; break;
        }
    }

    public void MenuEnabled(bool enabled)
    {
        inventoryUIBlock.SetActive(enabled);
    }
}
