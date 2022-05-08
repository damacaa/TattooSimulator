using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InputManager : MonoBehaviour
{
    public static InputManager instance;

    public bool forceMobile = false;
    bool isMobile = false;
    public Vector3 offset;
    public Color wrongColor;
    public float holdTime = 0.5f;

    bool holdingTattoo = false;
    bool movingTattoo = false;
    bool zooming = false;//Is zooming

    public Vector3 mousePosition;

    [SerializeField]
    private float pinchSensitivity = 15.0f;

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

        //CameraBehaviour.instance.Rotate(-Input.GetAxis("Horizontal") * Time.deltaTime, -Input.GetAxis("Vertical") * Time.deltaTime);

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
        if (Input.GetMouseButtonUp(0))
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
        if (movingTattoo)
        {
            Ray ray = Camera.main.ScreenPointToRay(mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity))
            {
                TattooManager.instance.MoveTattoo(hit.point, hit.normal);
            }
        }
        else if (!zooming)
        {
            //If not zooming, camera will be moved
            if (Mathf.Abs(Input.GetAxis("Mouse X")) > 30 || Mathf.Abs(Input.GetAxis("Mouse Y")) > 30)
                return;

            CameraBehaviour.instance.Rotate(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
        }
    }

    private void MouseDown()
    {
        Ray ray = Camera.main.ScreenPointToRay(mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity))
        {
            switch (hit.collider.tag)
            {
                case "Tattoo":
                    SmartTattoo t = hit.collider.GetComponent<SmartTattoo>();
                    if (!t.enabled)
                        TattooManager.instance.PrintTattoo(hit.point, hit.normal);
                    StartCoroutine(CheckHold(mousePosition, t));
                    break;
                case "Man":
                    TattooManager.instance.PrintTattoo(hit.point, hit.normal);
                    break;
                default:
                    TattooManager.instance.Deselect();
                    break;
            }
        }
    }

    IEnumerator CheckHold(Vector3 initialPos, SmartTattoo tattoo)
    {
        print("Check hold");
        float t = 0;
        Vector3 lastPos = initialPos;
        holdingTattoo = true;
        float step = holdTime / 10f;
        while (t < holdTime)
        {
            if (!holdingTattoo || (mousePosition - lastPos).magnitude > 10f)
            {
                print("Hold cancelled");
                TattooManager.instance.SelectTattoo(tattoo);
                CameraBehaviour.instance.MoveTargetTo(tattoo.transform.position);
                yield break;
            }
            lastPos = mousePosition;
            t += step;
            yield return new WaitForSeconds(step);
        }

        movingTattoo = true;
        TattooManager.instance.SelectTattoo(tattoo);
        yield return null;
    }

    public void MouseUp()
    {
        print("Mouse up");
        if (movingTattoo)
            TattooManager.instance.StopMovingTattoo();
        movingTattoo = false;
        holdingTattoo = false;
    }

    bool CheckPinch()
    {
        if (movingTattoo)
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
                CameraBehaviour.instance.MoveCenter(0.01f * avgDelta.x * Time.deltaTime, -0.01f * avgDelta.y * Time.deltaTime);
            }
        }

        return true;
    }
}
//https://answers.unity.com/questions/1698508/detect-mobile-client-in-webgl.html?childToView=1698985#answer-1698985
