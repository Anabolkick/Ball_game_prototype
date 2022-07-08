using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    public Transform PlayerTrans;
    public Transform DoorTrans;
    void OpenDoor()
    {
        JointSpring spring = GetComponent<HingeJoint>().spring;
        spring.targetPosition = 110;
        GetComponent<HingeJoint>().spring = spring;
    }

    void CheckDistance()
    {
        var distance = Vector3.Distance(PlayerTrans.position, DoorTrans.position);
        if (distance <= 5)
        {
            OpenDoor();
        }
    }

    void Update()
    {
        CheckDistance();
    }
}
