using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

namespace StuPro
{
    public class RobotAI : Agent
    {
        // Debugging
        [SerializeField] bool showDebugMessages = false;

        // Articulation bodies
        [SerializeField] public ArticulationBody articulationBody;
        [SerializeField] ArticulationBody leftWheel, rightWheel;
        [SerializeField] public ArticulationBody targetArticulationBody;
        [SerializeField] ArticulationDrive wheelDrive;
        [SerializeField] public GameObject dropZone, box;
        [SerializeField] Randomizer randomizer;

        // Properties for Training
        public float actionM1 = 0;
        public float actionM2 = 0;

        [SerializeField] float batteryCapacity = 0;

        float BatteryCapacity
        {
            get { return batteryCapacity; }
            set { batteryCapacity = Mathf.Clamp(value, 0, 100); }
        }

        //only needed for regular abstraction, for the half step and fully continuous version, disable abstraction and edit the abstraction mapping function accordingly
        public bool abstraction = true;

        //Coordinates of the next direct step to the goal
        public int calcStepX;
        public int calcStepZ;

        //Positions for the abstract environment
        public int robotPosX = 0;
        public int robotPosZ = 0;

        public int boxPosX = 0;
        public int boxPosZ = 0;

        public int goalPosX = 0;
        public int goalPosZ = 0;

        //Real positions (also used for finer grid size)
        public float robotPosXR = 0;
        public float robotPosZR = 0;

        public float boxPosXR = 0;
        public float boxPosZR = 0;

        public float goalPosXR = 0;
        public float goalPosZR = 0;

        //Is Box picked up?
        public bool isPickedUp = false;

        //Settings
        [SerializeField] bool batteryDeathIrrelevant = false;
        [SerializeField] float topSpeed = 1;
        [SerializeField] int maxEnergyCollectedTillEpisodeEnd = 0;
        [SerializeField] int energyCollectedCurrentRun = 0;

        //Randomization
        Vector3 randomTarget;
        int batteryFailure = 0;
        int boundaryFailure = 0;
        int targetFailure = 0;
        int targetCollected = 0;
        float initDistance;
        string folderName = System.DateTime.Now.ToString("yyyyMMdd_hhmmss");

        private float newAction1 = 0f;
        private float newAction2 = 0f;
        public bool heuristic = false;

        float forward = 0.1f;

        void Start()
        {
            if (heuristic) Time.timeScale = 1.0f;
        }

        // Defines what happens at the beginning of a new episode (e.g. reset of robot and obstacles)
        public override void OnEpisodeBegin()
        {
            // Reset battery capacity
            BatteryCapacity = 100;

            // Reset energyCollectedCurrentRun
            energyCollectedCurrentRun = 0;

            // Set position of target randomly
            box.SetActive(true);
            isPickedUp = false;
            Debug.Log("Current level: " + Academy.Instance.EnvironmentParameters.GetWithDefault("levelDifficulty", 0));
            randomizer.RandomizeObstacles(/*heuristic ? 11 : */Academy.Instance.EnvironmentParameters.GetWithDefault("levelDifficulty", 0));
            initDistance = Mathf.Sqrt(
                Mathf.Pow(targetArticulationBody.transform.position.x - dropZone.transform.position.x, 2) +
                Mathf.Pow(targetArticulationBody.transform.position.z - dropZone.transform.position.z, 2)
            );
            SetMaxEnergy(heuristic ? 11 : Academy.Instance.EnvironmentParameters.GetWithDefault("levelDifficulty", 0));

            //calculates the abstract coordinates
            if (abstraction)
            {
                this.AbstractionMapping();

                //calculates the next direct step to the goal
                //this.CalculateSteps();
            }
        }

        //returns the rotation of the robot in degrees (0-360)
        public float GetRot()
        {
            return articulationBody.transform.eulerAngles.y;
        }

        //returns the float coordinate rounded to halfs
        public float AbstractToHalf(float pos)
        {
            float a = 1f;
            if (pos < 0f)
            {
                a = -1f;
            }

            float x = pos;
            if (a < 0f)
            {
                x = pos*a;
            }
            
            int y = (int)x;
            float z = (float)y;

            if ((x - z) < 0.5f)
            {
                return z * a;
            }
            else
            {
                return (z + 0.5f ) * a;
            }
        }


        //Calculates the coordinates as wished
        public void AbstractionMapping()
        {

            //calculates the coordinates as typecast for a rough gridsize
            if (abstraction)
            {
                
                robotPosX = (int)articulationBody.transform.position.x;
                robotPosZ = (int)articulationBody.transform.position.z;

                if (isPickedUp == false)
                {
                    boxPosX = (int)targetArticulationBody.transform.position.x;
                    boxPosZ = (int)targetArticulationBody.transform.position.z;
                }


                goalPosX = (int)dropZone.transform.position.x;
                goalPosZ = (int)dropZone.transform.position.z;
            }

            if (!abstraction)
            {
                 //saves the coordinates in the according variables for fully continuous training
                 robotPosXR = articulationBody.transform.position.x;
                 robotPosZR = articulationBody.transform.position.z;

                 if (isPickedUp == false)
                 {
                    boxPosXR = targetArticulationBody.transform.position.x;
                    boxPosZR = targetArticulationBody.transform.position.z;
                 }


                 goalPosXR = dropZone.transform.position.x;
                 goalPosZR = dropZone.transform.position.z;

                //saves the coordinates in the according variables for the finder grid size
                /*robotPosXR = AbstractToHalf(articulationBody.transform.position.x);
                robotPosZR = AbstractToHalf(articulationBody.transform.position.z);

                if (isPickedUp == false)
                {
                    boxPosXR = AbstractToHalf(targetArticulationBody.transform.position.x);
                    boxPosZR = AbstractToHalf(targetArticulationBody.transform.position.z);
                }


                goalPosXR = AbstractToHalf(dropZone.transform.position.x);
                goalPosZR = AbstractToHalf(dropZone.transform.position.z);*/

            }

        }


        //calculates one direct step to the goal
        public void CalculateSteps()
        {
            //if the box is not picked up, the step is calculated towards the box
            if (!isPickedUp)
            {
                //looks if it has to take a step along the x axis
                if (boxPosX < robotPosX)
                {
                    calcStepX = robotPosX - 1;
                    calcStepZ = robotPosZ;
                }

                else if (boxPosX > robotPosX)
                {
                    calcStepX = robotPosX + 1;
                    calcStepZ = robotPosZ;
                }

                //taskes a step along the z axis, when the x coordinate is the same as the goal
                else if (boxPosZ < robotPosZ)
                {
                    calcStepX = robotPosX;
                    calcStepZ = robotPosZ - 1;
                }

                else if (boxPosZ > robotPosZ)
                {
                    calcStepX = robotPosX;
                    calcStepZ = robotPosZ + 1;
                }


            }

            //if the box is picked up, the step is calculated towards the goal zone
            else
            {
                if (goalPosX < robotPosX)
                {
                    calcStepX = robotPosX - 1;
                    calcStepZ = robotPosZ;
                }

                else if (goalPosX > robotPosX)
                {
                    calcStepX = robotPosX + 1;
                    calcStepZ = robotPosZ;
                }

                else if (goalPosZ < robotPosZ)
                {
                    calcStepX = robotPosX;
                    calcStepZ = robotPosZ - 1;
                }

                else if (goalPosZ > robotPosZ)
                {
                    calcStepX = robotPosX;
                    calcStepZ = robotPosZ + 1;
                }

            }

            //when the curriculum learning lesson is above 10 the calculated step gets abrogated
            if(Academy.Instance.EnvironmentParameters.GetWithDefault("levelDifficulty", 0) > 9)
            {
                calcStepX = 1000;
                calcStepZ = 1000;
            }
        }

        //Here the observations are chosen
        public override void CollectObservations(VectorSensor sensor)
        {

            //Here the coordintes for rough grid size are observed
            if (abstraction)
            {
                sensor.AddObservation(robotPosX);
                sensor.AddObservation(robotPosZ);


                sensor.AddObservation(boxPosX);
                sensor.AddObservation(boxPosZ);

                sensor.AddObservation(goalPosX);
                sensor.AddObservation(goalPosZ);

                //Here the coordintes of the next calculated step are observed
                /*sensor.AddObservation(calcStepX);
                sensor.AddObservation(calcStepZ);*/

            }

            //Here the coordintes for fine grid size or the fully continuous environment are observed
            if (!abstraction)
            {
                sensor.AddObservation(robotPosXR);
                sensor.AddObservation(robotPosZR);


                sensor.AddObservation(boxPosXR);
                sensor.AddObservation(boxPosZR);

                sensor.AddObservation(goalPosXR);
                sensor.AddObservation(goalPosZR);
            }

            //Here it is observed whether the box is picked up or not
            sensor.AddObservation(isPickedUp);
        }
        
        
        // Defines the action that the robot performs each step
        public override void OnActionReceived(ActionBuffers actions)
        {

            //Map the current positions to the abstraction
            this.AbstractionMapping();
            //this.CalculateSteps();

            // Read next action from action buffer
            actionM1 = actions.ContinuousActions[1];
            actionM2 = actions.ContinuousActions[0];

            // Clamp actions
            var leftWheelDrive = Mathf.Clamp(actionM1, -1f, 1f);
            var rightWheelDrive = Mathf.Clamp(actionM2, -1f, 1f);

            // Reduce battery capacity by .2 each step
            BatteryCapacity -= .2f;

            // Add velocity to wheels
            wheelDrive = leftWheel.xDrive;
            wheelDrive.targetVelocity = topSpeed * leftWheelDrive;
            leftWheel.xDrive = wheelDrive;

            wheelDrive = rightWheel.xDrive;
            wheelDrive.targetVelocity = topSpeed * rightWheelDrive;
            rightWheel.xDrive = wheelDrive;

            // If battery capacity is zero, the robot could not finish the episode successfully
            if (!batteryDeathIrrelevant && BatteryCapacity <= 0)
            {
                batteryFailure++;

                // Tensorflow Stats
                Academy.Instance.StatsRecorder.Add("Custom/Failure/Battery", batteryFailure);
                Academy.Instance.StatsRecorder.Add("Custom/Maximal energy collected", energyCollectedCurrentRun);
                float distance = Mathf.Sqrt(
                    Mathf.Pow(targetArticulationBody.transform.position.x - dropZone.transform.position.x, 2) +
                    Mathf.Pow(targetArticulationBody.transform.position.z - dropZone.transform.position.z, 2)
                );
                Academy.Instance.StatsRecorder.Add("Custom/Distance between target and drop area", distance/initDistance);

                // End Episode
                SetReward(-1);
                box.SetActive(true);
                isPickedUp = false;
                EndEpisode();
            }

            // If the target leaves platform, the robot could not finish the episode successfully
            if (targetArticulationBody.transform.localPosition.y < -0.5f)
            {
                targetFailure++;

                // Tensorflow Stats
                Academy.Instance.StatsRecorder.Add("Custom/Failure/Target has left platform", targetFailure);
                Academy.Instance.StatsRecorder.Add("Custom/Maximal energy collected", energyCollectedCurrentRun);

                float distance = Mathf.Sqrt(
                    Mathf.Pow(targetArticulationBody.transform.position.x - dropZone.transform.position.x, 2) +
                    Mathf.Pow(targetArticulationBody.transform.position.z - dropZone.transform.position.z, 2)
                );

                Academy.Instance.StatsRecorder.Add("Custom/Distance between target and drop area", distance/initDistance);

                // End Episode
                SetReward(-1);
                box.SetActive(true);
                isPickedUp = false;
                EndEpisode();
            }
        }

        //when the box is picked up the agent gets a reward relative to the left over battery capacity, the faster the better
        public void OnBoxPickedUp()
        {
            isPickedUp = true;
            AddReward(BatteryCapacity / 100);

        }


        // The robot successfully finishes an episode, if the target cube is collected
        public void OnTargetCollected()
        {
            Debug.Log("Target cube collected!");
            targetCollected++;
            Academy.Instance.StatsRecorder.Add("Custom/Target collected", targetCollected);
            Academy.Instance.StatsRecorder.Add("Custom/Battery capacity after collection", BatteryCapacity);

            // Reward robot for collection target cube
            AddReward(2);
            box.SetActive(true);
            isPickedUp = false;

            // Reward robot for collection target cube fast
            AddReward(BatteryCapacity / 100);

            energyCollectedCurrentRun++;
            Academy.Instance.StatsRecorder.Add("Custom/Maximal energy collected", energyCollectedCurrentRun);
            Academy.Instance.StatsRecorder.Add("Custom/Distance between target and drop area", 0);

            if (energyCollectedCurrentRun < maxEnergyCollectedTillEpisodeEnd)
            {
                AddReward(1);
                box.SetActive(true);
                isPickedUp = false;
                BatteryCapacity = 100;
                randomizer.RandomizeObstacles(Academy.Instance.EnvironmentParameters.GetWithDefault("levelDifficulty", 0));
                SetMaxEnergy(Academy.Instance.EnvironmentParameters.GetWithDefault("levelDifficulty", 0));
            }
            else
            {
                AddReward(1); //extra point for a succesfull run
                box.SetActive(true);
                isPickedUp = false;
                EndEpisode();
            }
        }

        //when the robot leaves the platform it gets a penalty
        public void OnCollisionWithBoundary()
        {
            boundaryFailure++;

            // Tensorflow Stats
            Academy.Instance.StatsRecorder.Add("Custom/Failure/Robot touched boundary", boundaryFailure);
            Academy.Instance.StatsRecorder.Add("Custom/Maximal energy collected", energyCollectedCurrentRun);
            float distance = Mathf.Sqrt(
                Mathf.Pow(targetArticulationBody.transform.position.x - dropZone.transform.position.x, 2) +
                Mathf.Pow(targetArticulationBody.transform.position.z - dropZone.transform.position.z, 2)
            );
            Academy.Instance.StatsRecorder.Add("Custom/Distance between target and drop area", distance/initDistance);

            // End Episode
            SetReward(-2);
            box.SetActive(true);
            isPickedUp = false;
            EndEpisode();
        }


        //Here the discrete actions of the fully abstract agent are translated to the continuous actions of the robot
        public void MoveAbstract(int dir)
        {

            //forward
            if (dir == 1)
            {
                wheelDrive = leftWheel.xDrive;
                wheelDrive.targetVelocity = topSpeed * forward;
                leftWheel.xDrive = wheelDrive;

                wheelDrive = rightWheel.xDrive;
                wheelDrive.targetVelocity = topSpeed * forward;
                rightWheel.xDrive = wheelDrive;
            }

            //backward
            else if(dir == 2)
            {
                wheelDrive = leftWheel.xDrive;
                wheelDrive.targetVelocity = topSpeed * forward * -1f;
                leftWheel.xDrive = wheelDrive;

                wheelDrive = rightWheel.xDrive;
                wheelDrive.targetVelocity = topSpeed * forward * -1f;
                rightWheel.xDrive = wheelDrive;
            }

            //turn right
            else if(dir == 3)
            {
                    wheelDrive = leftWheel.xDrive;
                    wheelDrive.targetVelocity = topSpeed * 0.1f;
                    leftWheel.xDrive = wheelDrive;

                    wheelDrive = rightWheel.xDrive;
                    wheelDrive.targetVelocity = topSpeed * -0.1f;
                    rightWheel.xDrive = wheelDrive;
            }

            //turn left
            else if(dir == 4)
            {
                    wheelDrive = leftWheel.xDrive;
                    wheelDrive.targetVelocity = topSpeed * -0.1f;
                    leftWheel.xDrive = wheelDrive;

                    wheelDrive = rightWheel.xDrive;
                    wheelDrive.targetVelocity = topSpeed * 0.1f;
                    rightWheel.xDrive = wheelDrive;
            }
        }


        private void SetMaxEnergy(float difficulty)
        {
            switch ((int) difficulty)
            {
                case 1:  maxEnergyCollectedTillEpisodeEnd = 1; break;
                case 2:  maxEnergyCollectedTillEpisodeEnd = 2; break;
                case 3:  maxEnergyCollectedTillEpisodeEnd = 3; break;
                case 4:  maxEnergyCollectedTillEpisodeEnd = 4; break;
                case 5:  maxEnergyCollectedTillEpisodeEnd = 5; break;
                case 6:  maxEnergyCollectedTillEpisodeEnd = 6; break;
                case 7:  maxEnergyCollectedTillEpisodeEnd = 7; break;
                case 8:  maxEnergyCollectedTillEpisodeEnd = 8; break;
                case 9:  maxEnergyCollectedTillEpisodeEnd = 9; break;
                case 10: maxEnergyCollectedTillEpisodeEnd = 10; break;
                case 11: maxEnergyCollectedTillEpisodeEnd = 10; break;
                default: maxEnergyCollectedTillEpisodeEnd = 1; break;
            }
        }


        //checks if box is delivered to the goal
        void OnTriggerEnter(Collider collider)
        {
            if (collider.name == "Goal" && isPickedUp)
            {
                this.OnTargetCollected();
            }
        }

        public void OnCollisionWithWall()
        {
            
            SetReward(-1);
            EndEpisode();
        }

    }
}