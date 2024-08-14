using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace StuPro
{

    public class RobotCollider : MonoBehaviour
    {
        public RobotAI robot;

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        void OnTriggerEnter(Collider collider)
        {
            if (collider.name == "Goal" && robot.isPickedUp)
            {
                robot.OnTargetCollected();
            }
        }
    }

}
