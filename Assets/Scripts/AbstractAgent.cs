using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using Random = System.Random;

namespace StuPro
{


    public class AbstractAgent : Agent
    {
        //position of the objects
        public int agentPosX;
        public int agentPosZ;

        public int boxPosX;
        public int boxPosZ;

        public int goalPosX;
        public int goalPosZ;

        //is box picked up?
        public bool isPickedUp;

        public Random rnd = new Random();

        //timer, similar to the battery capacity in the continuous environment
        public float time = 100f;

        //saves the actions here from the buffer
        int movement;

        //tiles for visualisation
        [SerializeField] public GameObject bot, box, goal;

        //the script used to operate the robot in the continuous environment
        public Abstraction abstraction;


        //here the objects are randomly placed at the beginning of an episode and other things are initialized
        public override void OnEpisodeBegin()
        {
            //on the lower lessons of the curriculum the objects are close to the middle
            if (Academy.Instance.EnvironmentParameters.GetWithDefault("levelDifficulty", 0) < 1)
            {
                agentPosX = rnd.Next(-2, 2);
                agentPosZ = rnd.Next(-3, 3);

                boxPosX = rnd.Next(-2, 2);
                boxPosZ = rnd.Next(-3, 3);

                if (boxPosX == agentPosX && boxPosZ == agentPosZ)
                {
                    boxPosX = rnd.Next(-2, 2);
                    boxPosZ = rnd.Next(-3, 3);
                }

                goalPosX = rnd.Next(-2, 2);
                goalPosZ = rnd.Next(-3, 3);

                if (boxPosX == goalPosX && boxPosZ == goalPosZ | agentPosX == goalPosX && agentPosZ == goalPosZ)
                {
                    goalPosX = rnd.Next(-2, 2);
                    goalPosZ = rnd.Next(-3, 3);
                }
            }

            //in the middle lessons the area is widened
            else if (Academy.Instance.EnvironmentParameters.GetWithDefault("levelDifficulty", 0) < 5)
            {
                agentPosX = rnd.Next(-4, 4);
                agentPosZ = rnd.Next(-6, 6);

                boxPosX = rnd.Next(-4, 4);
                boxPosZ = rnd.Next(-6, 6);

                if (boxPosX == agentPosX && boxPosZ == agentPosZ)
                {
                    boxPosX = rnd.Next(-4, 4);
                    boxPosZ = rnd.Next(-6, 6);
                }

                goalPosX = rnd.Next(-4, 4);
                goalPosZ = rnd.Next(-6, 6);

                if (boxPosX == goalPosX && boxPosZ == goalPosZ | agentPosX == goalPosX && agentPosZ == goalPosZ)
                {
                    goalPosX = rnd.Next(-4, 4);
                    goalPosZ = rnd.Next(-6, 6);
                }
            }

            //in the last lessons the objects are placed across the whole grid
            else
            {

                agentPosX = rnd.Next(-7, 7);
                agentPosZ = rnd.Next(-10, 10);

                boxPosX = rnd.Next(-7, 7);
                boxPosZ = rnd.Next(-10, 10);

                if (boxPosX == agentPosX && boxPosZ == agentPosZ)
                {
                    boxPosX = rnd.Next(-7, 7);
                    boxPosZ = rnd.Next(-10, 10);
                }

                goalPosX = rnd.Next(-7, 7);
                goalPosZ = rnd.Next(-10, 10);

                if (boxPosX == goalPosX && boxPosZ == goalPosZ | agentPosX == goalPosX && agentPosZ == goalPosZ)
                {
                    goalPosX = rnd.Next(-7, 7);
                    goalPosZ = rnd.Next(-10, 10);
                }
            }

            isPickedUp = false;
            time = 100f;
        }

        public override void CollectObservations(VectorSensor sensor)
        {

            //here the positions of the objects in the abstraction are observed during training
            sensor.AddObservation(agentPosX);
            sensor.AddObservation(agentPosZ);

            sensor.AddObservation(boxPosX);
            sensor.AddObservation(boxPosZ);

            sensor.AddObservation(goalPosX);
            sensor.AddObservation(goalPosZ);

            sensor.AddObservation(isPickedUp);

            //here the positions of the actual objects are observed during testing
            /*sensor.AddObservation(abstraction.robotPosX);
            sensor.AddObservation(abstraction.robotPosZ);

            sensor.AddObservation(abstraction.boxPosX);
            sensor.AddObservation(abstraction.boxPosZ);

            sensor.AddObservation(abstraction.goalPosX);
            sensor.AddObservation(abstraction.goalPosZ);

            sensor.AddObservation(abstraction.isPickedUp);*/
        }

        public override void OnActionReceived(ActionBuffers actions)
        {
            //timer is decreased
            time -= 0.1f;

            movement = actions.DiscreteActions[0];

            //actions for training
            if (movement == 1) { agentPosX += 1; }
            if (movement == 3) { agentPosX -= 1; }
            if (movement == 2) { agentPosZ += 1; }
            if (movement == 0) { agentPosZ -= 1; }

            //actions for testing on the continuous env
            /*if (movement == 1) { abstraction.key = 4; }
            if (movement == 3) { abstraction.key = 3; }
            if (movement == 2) { abstraction.key = 2; }
            if (movement == 0) { abstraction.key = 1; }*/

            //box picked up
            if (boxPosX == agentPosX && boxPosZ == agentPosZ)
            {
                float i = time / 70f;

                isPickedUp = true;
                boxPosX = 1000;
                boxPosZ = 1000;
                AddReward(i);
                Debug.Log(this.gameObject.name + "box picked up");
            }

            //episode finished successfully
            if (goalPosX == agentPosX && goalPosZ == agentPosZ && isPickedUp)
            {
                float i = time / 10f;


                AddReward(3);
                AddReward(i);
                isPickedUp = false;
                Debug.Log(this.gameObject.name + "success");

                EndEpisode();
            }

            //robot fell off platform
            if(agentPosZ < -10 | agentPosZ > 10 | agentPosX < -7 | agentPosX > 7)
            {
                
                 SetReward(-4);
                
                
                isPickedUp = false;
                Debug.Log(this.gameObject.name + "fell off");

                EndEpisode();
            }

            //time over
            if (time <= 0f)
            {
                
                SetReward(-3f);
                
                isPickedUp = false;
                Debug.Log(this.gameObject.name + "time over");

                EndEpisode();
            }

            //visualisation
            if (this.tag == "agent")
            {
                bot.transform.position = new Vector3((float)agentPosX, bot.transform.position.y, (float)agentPosZ);
                box.transform.position = new Vector3((float)boxPosX, 81.4f, (float)boxPosZ);
                goal.transform.position = new Vector3((float)goalPosX, goal.transform.position.y, (float)goalPosZ);
            }
        }

    }
}
