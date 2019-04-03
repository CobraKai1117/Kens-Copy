﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class DropperInteraction : MonoBehaviour
{
    [SerializeField] GameObject dropper;
    [SerializeField] Camera cam;
    [SerializeField] Transform indicator;
    [SerializeField] GameObject agitator;
    [SerializeField] Material FlashMat;
    [SerializeField] GameObject Sample;
    [SerializeField] GameObject Dropper2;
    [SerializeField] GameObject H20drop;
    [SerializeField] GameObject WaterDropPos;
    [SerializeField] GameObject TestOb;
    [SerializeField] GameObject SolutionJar;
    [SerializeField] GameObject DropOgPos;
    [Space]
    [Space]

    //[SerializeField, Range(0,360)] float[] PivotAngles;

    bool interactable;

    Vector3 dist;
    Vector3 startPos;
    float posX;
    float posZ;
    float posY;
    float distBetweenTwoPoints;
    float dropZAx;
    bool RotationStart;
    [SerializeField] GameObject DropPos;
    public static Camera mainCam;
    public static Vector2 prevMousePos;
    float speed;
    int dropCount = 0;
    float T = 0f;
    public SolutionScript MoveSolution;






    // Start is called before the first frame update
    void Start()
    {
        speed = 70f;
        interactable = true;
        ColumnAppear();
        dropZAx = 52;






        FlashAppear(Dropper2);
        FlashDisappear(SolutionJar);

        //cam = GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        //GlobalFunctions.ColorFlash(FlashMat, Color.yellow, 0.1f, 0.24f);

        Vector3 screenPos = cam.WorldToScreenPoint(Input.mousePosition);

        // Debug.Log("The target is" + screenPos.x + "pixels from the left");

        /*if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0); // get first touch since touch count is greater than zero

            if (touch.phase == TouchPhase.Stationary || touch.phase == TouchPhase.Moved)
            {
                // get the touch position from the screen touch to world point
                Vector3 touchedPos = Camera.main.ScreenToWorldPoint(new Vector3(touch.position.x, touch.position.y, 10));
                // lerp and set the position of the current object to that of the touch, but smoothly over time.
                transform.position = Vector3.Lerp(transform.position, touchedPos, Time.deltaTime);
            }
        } */

        distancetoSample(dropper.transform.position, agitator.transform.position);
        // T += Time.deltaTime;
        //Debug.Log(T);

    }


    void OnMouseDown()
    {
        if (interactable)
        {
            FlashDisappear(Dropper2);

           
            startPos = transform.position;
            Debug.Log(startPos);
          
            dist = Camera.main.WorldToScreenPoint(transform.position);

           

            posX = Input.mousePosition.x - dist.x;

          
            posY = Input.mousePosition.y - dist.y;

            Debug.Log(posY);

            posZ = Input.mousePosition.z - dist.z;

            Debug.Log(posZ);


        }
    }

    void OnMouseDrag()
    {
        if (interactable)
        {

            float disX = Input.mousePosition.x - posX;
            Debug.Log(disX + "FLOAT");
            float disY = Input.mousePosition.y - posY;
            float disZ = Input.mousePosition.z - posZ;
            Vector3 lastPos = Camera.main.ScreenToWorldPoint(new Vector3(disX, disY, disZ));
            transform.position = new Vector3(lastPos.x, startPos.y, startPos.z);

        }
    }

    /* public static bool SlideObjectHorizontal(Transform itemToMove, Transform pointToLockTo, Transform currentParent, float speedMult, float maxDist, float activationDist)
     {
         if (itemToMove.parent != pointToLockTo)
             itemToMove.parent = pointToLockTo;

         pointToLockTo.LookAt(mainCam.transform);
         pointToLockTo.forward = new Vector3(-pointToLockTo.forward.x, 0, -pointToLockTo.forward.z).normalized;

         float moveDist = 0;
         if (Input.GetMouseButton(0) && !Input.GetMouseButtonDown(0))
             moveDist = (Input.mousePosition.x - prevMousePos.x) * speedMult;

         itemToMove.localPosition = new Vector3(itemToMove.localPosition.x + moveDist, itemToMove.localPosition.y, 0);

         if (Mathf.Abs(itemToMove.localPosition.x) > maxDist)
             itemToMove.localPosition = new Vector3(maxDist * Mathf.Sign(itemToMove.localPosition.x), itemToMove.localPosition.y, 0);

         if (Vector2.Distance(new Vector2(itemToMove.position.x, itemToMove.position.z), new Vector2(pointToLockTo.position.x, pointToLockTo.position.z)) < activationDist)
         {
             itemToMove.parent = currentParent;
             return true;
         }
         return false;
     } */








    public void ColumnAppear() // Makes marker for agitator appear
    {
        indicator.gameObject.SetActive(true);
    }


    public void ColumnDisappear() // Makes marker for agitator disappear
    {
        indicator.gameObject.SetActive(false);
    }

    public void distancetoSample(Vector3 pointA, Vector3 pointB) // Checks to see if distance between point A and B is small enough, if it is you can't move dropper any more
    {

        distBetweenTwoPoints = Vector3.Distance(pointA, pointB);
        //Debug.Log(distBetweenAgDrop);
        //  Vector3 newDropPos = dropper.transform.position + new Vector3(0, .2f, 0);
        if (distBetweenTwoPoints <= 1f)
        {
            ColumnDisappear();
            interactable = false;

            // Invoke("MoveDrop", .2f);  // Waits a small amount of time before running MoveDrop function
        }

        if (distBetweenTwoPoints <= 1 && pointA == dropper.transform.position && pointB == agitator.transform.position)
        {
            interactable = false;
            Invoke("MoveDrop", .2f);
        }

    }

    public void MoveDrop() // Program Lerps dropper closer to solution
    {
        //Vector3 newDropPos = dropper.transform.position + new Vector3(0, .005f, 0);
        dropper.transform.position = Vector3.Lerp(dropper.transform.position, DropPos.transform.position, .05f);
        RotationStart = true;


        if (RotationStart)
        {
            dropper.transform.Rotate(0, 0, speed * Time.deltaTime); // This rotates dropper so it properly faces solution

            float RotPoint = dropper.transform.eulerAngles.z;
            // Debug.Log(RotPoint);

            if (Mathf.Abs(RotPoint - dropZAx) < 1)
            {
                speed = 0;
                RotationStart = false; // Ensures that rotation stops once dropper is rotated at a certain angle to solution
                WaterDrop();

            }


        }

    }

    public void WaterDrop() // Adds drops from dropper to solution
    {

        if (Input.GetMouseButtonDown(0) && dropCount < 4)
        {



            H20drop = Instantiate(H20drop, dropper.transform.position, dropper.transform.rotation);
            H20drop.transform.position = Vector3.Lerp(dropper.transform.position, WaterDropPos.transform.position, 30 * Time.deltaTime);


            Debug.Log(dropCount);

            dropCount++;

        }

       if (dropCount >=4)
        {
           // MoveSolution.interactable = true;
          //  distancetoSample(dropper.transform.position, DropOgPos.transform.position);
        }

        

    }

    public void FlashAppear(GameObject ObjWFlash)
    {
        ObjWFlash.SetActive(true);
    }

    public void FlashDisappear(GameObject FlashRem)
    {
        FlashRem.SetActive(false);
    }
}




