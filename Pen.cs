using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pen : MonoBehaviour
{
    [Header("Pen Properties")]
    public Transform tip;
    public Material drawingMaterial;
    public Material tipMaterial;
    [Range(0.01f, 0.1f)]
    public float penWidth = 0.01f;
    public Color[] penColors;
    public Color eraser;

    [Header("Hands & Grabbable")]
    public OVRGrabber rightHand;
    public OVRGrabber leftHand;
    public OVRGrabbable grabbable;

    private LineRenderer currentDrawing;
    private int index;
    private int currentColorIndex;
    private bool usingPen;

    private void Start()
    {
        currentColorIndex = 0;
        tipMaterial.color = penColors[currentColorIndex];
        usingPen = true;
    }

    private void Update()
    {
        bool isGrabbed = grabbable.isGrabbed;
        bool isRightHandDrawing = isGrabbed && grabbable.grabbedBy == rightHand && OVRInput.Get(OVRInput.Button.SecondaryIndexTrigger);
        bool isLeftHandDrawing = isGrabbed && grabbable.grabbedBy == leftHand && OVRInput.Get(OVRInput.Button.PrimaryIndexTrigger);
        if (isRightHandDrawing || isLeftHandDrawing)
        {
            Draw();
        }
        else if (currentDrawing != null)
        {
            currentDrawing = null;
        }
        else if (OVRInput.GetDown(OVRInput.Button.One))
        {
            SwitchColorForward();
        }
        /*else if (OVRInput.GetDown(OVRInput.Button.Two))
        {
            SwitchColorBackward();
        }*/
        else if (OVRInput.GetDown(OVRInput.Button.Two))
        {
            SwitchTools();
        }
    }

    private void Draw()
    {
        if (currentDrawing == null)
        {
            index = 0;
            currentDrawing = new GameObject().AddComponent<LineRenderer>();
            currentDrawing.material = drawingMaterial;
            if (usingPen == true)
            {
                currentDrawing.startColor = currentDrawing.endColor = penColors[currentColorIndex];
            }
            else if (usingPen == false)
            {
                currentDrawing.startColor = currentDrawing.endColor = eraser;
            }
            currentDrawing.startWidth = currentDrawing.endWidth = penWidth;
            currentDrawing.positionCount = 1;
            currentDrawing.SetPosition(0, tip.position);
        }
        else
        {
            var currentPos = currentDrawing.GetPosition(index);
            if (Vector3.Distance(currentPos, tip.position) > 0.01f)
            {
                index++;
                currentDrawing.positionCount = index + 1;
                currentDrawing.SetPosition(index, tip.position);
            }
        }
    }

    private void SwitchColorForward()
    {
        if (usingPen == true)
        {
            if (currentColorIndex == penColors.Length - 1)
            {
                currentColorIndex = 0;
            }
            else
            {
                currentColorIndex++;
            }
            tipMaterial.color = penColors[currentColorIndex];
        }
    }

    private void SwitchColorBackward()
    {
        if (usingPen == true)
        {
            if (currentColorIndex == 0)
            {
                currentColorIndex = penColors.Length - 1;
            }
            else
            {
                currentColorIndex--;
            }
            tipMaterial.color = penColors[currentColorIndex];
        }
    }

    private void SwitchTools()
    {
        if (usingPen == true) 
        { 
            usingPen = false;
            tipMaterial.color = eraser;
        }
        else if (usingPen == false)
        {
            usingPen = true;
            tipMaterial.color = penColors[currentColorIndex];
        }
    }
}