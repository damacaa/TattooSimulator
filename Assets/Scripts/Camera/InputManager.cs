using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InputManager : MonoBehaviour
{
    public static InputManager instance;
    public bool choosingWhereToBuild = false; //A structure card has been selected
    bool zooming = false;//Is zooming
    bool isMobile = false;
    public bool forceMobile = false;
    public Vector3 offset;
    public Vector3 mousePosition;
    Vector3 lastMousePosition;
    public Color wrongColor;

    [SerializeField]
    private float pinchSensitivity = 15.0f;
    //Gameobject that will be placed where structure is about to be built



    private void Awake()
    {
        instance = this;
    }
    void Start()
    {
        isMobile = forceMobile;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            CameraBehaviour.instance.Reset();
        }

        if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.Z))
        {
            TattooManager.instance.Undo();
        }

        if (Helpers.IsOverUI())
            return;

        CameraBehaviour.instance.Rotate(-Input.GetAxis("Horizontal") * Time.deltaTime, -Input.GetAxis("Vertical") * Time.deltaTime);

        lastMousePosition = mousePosition;
        if (isMobile && Input.touchCount > 0)
        {
            mousePosition = Input.touches[0].position;
            CheckPinch();
        }
        else
        {
            mousePosition = Input.mousePosition;
            //Zoom
            CameraBehaviour.instance.Zoom(Input.mouseScrollDelta.y); //Zoom with mouse wheel
        }

        //Click
        if (Input.GetMouseButtonDown(0))
        {
            MouseDown();
        }

        //Click release
        if (Input.GetMouseButtonUp(0) && !isMobile)
        {
            MouseUp();
        }

        //Drag
        if (Input.GetMouseButton(0))
        {
            MouseDrag();
        }
        else
        {
            MouseHover();
        }

        //Drag
        if (Input.GetMouseButton(2))
        {
            CameraBehaviour.instance.MoveCenter(Input.GetAxis("Mouse X") * Time.deltaTime, -Input.GetAxis("Mouse Y") * Time.deltaTime);
        }


    }

    private void MouseHover()
    {
        Ray ray;
        if (isMobile)
            ray = Camera.main.ScreenPointToRay(Input.mousePosition + offset);
        else
            ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit = new RaycastHit();

        if (Physics.Raycast(ray, out hit, Mathf.Infinity))
        {
            TattooManager.instance.Preview(hit.point, hit.normal);
        }
        else
            TattooManager.instance.HideCursor();
    }


    private void MouseDrag()
    {
        if (!zooming && Input.mousePosition.x <= Screen.width * 0.9f)
        {
            //If not zooming, camera will be moved
            if (Mathf.Abs(Input.GetAxis("Mouse X")) > 30 || Mathf.Abs(Input.GetAxis("Mouse Y")) > 30)
                return;

            CameraBehaviour.instance.Rotate(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
        }
    }

    private void MouseDown()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity))
        {
            switch (hit.collider.tag)
            {
                case "Tattoo":
                    if (!hit.collider.GetComponent<SmartTattoo>().enabled)
                        TattooManager.instance.PrintTattoo(hit.point, hit.normal);
                    break;
                case "Man":
                    TattooManager.instance.PrintTattoo(hit.point, hit.normal);
                    TattooManager.instance.Deselect();
                    break;
                default:
                    TattooManager.instance.Deselect();
                    break;
            }
        }
    }

    public void MouseUp()
    {

    }

    bool CheckPinch()
    {
        if (choosingWhereToBuild)
            return false;

        int activeTouches = Input.touchCount;

        if (activeTouches < 2)//If less than two touches, can't zoom
        {
            zooming = false;
            return false;
        }

        zooming = true;

        Vector2 touch0 = Input.GetTouch(0).position;
        Vector2 touch1 = Input.GetTouch(1).position;
        //Deltas are the position change since las frame
        Vector2 delta0 = Input.GetTouch(0).deltaPosition;
        Vector2 delta1 = Input.GetTouch(1).deltaPosition;

        float currentDist = Vector2.Distance(touch0, touch1);
        float previousDist = Vector2.Distance(touch0 - delta0, touch1 - delta1);
        float difference = previousDist - currentDist;

        if (Vector2.Dot(delta0, delta1) < 0)//If deltas form an angle greater than 90 degrees
        {
            if (difference < 50)
            {
                CameraBehaviour.instance.Zoom(-difference * Time.deltaTime * pinchSensitivity);
            }
        }
        else
        {
            Vector2 avgDelta = (delta0 + delta1) / 2f;
            if (difference < 50)
            {
                CameraBehaviour.instance.MoveCenter(0.01f * avgDelta.x * Time.deltaTime, -0.01f *avgDelta.y * Time.deltaTime);
            }
        }

        return true;
    }
}
//https://answers.unity.com/questions/1698508/detect-mobile-client-in-webgl.html?childToView=1698985#answer-1698985
