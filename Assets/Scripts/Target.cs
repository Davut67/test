using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StuPro
{
    public class Target : MonoBehaviour
    {
        public RobotAI robot;

        // Calls robot if target was successfully pushed into drop zone
        void OnTriggerEnter(Collider collider)
        {
            if (collider.gameObject.tag == "Player")
            {
                Debug.Log("OnTriggerEnter");
                robot.OnBoxPickedUp(); //new
                this.gameObject.SetActive(false);
                //robot.OnTargetCollected();
            }
            
            if (collider.gameObject.tag == "Zaun")
            {
                Debug.Log("OnTriggerEnterKZ");
                robot.OnCollisionWithWall();
            }
        }
    }
}