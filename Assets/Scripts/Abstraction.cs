using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;


namespace StuPro
{
    public class Abstraction : MonoBehaviour
    {
        //position of each object
        public int robotPosX = 0;
        public int robotPosZ = 0;

        public int boxPosX = 0;
        public int boxPosZ = 0;

        public int goalPosX = 0;
        public int goalPosZ = 0;

        public bool isPickedUp = false;

        public int moveD = 0; //shows the moving direction forward, backward, right or left

        //here the position of the robot before the movement is saved
        public int posX;
        public int posZ;

        //shows the direction of rotation right, left or none
        public int rotD = 0;

        //the degree of the robots rotation is saved here (typecasted)
        public int rotI;

        //like a key hit to move in a direction, gets set by agent to move (can be replaced by a key to test the moving manually)
        public int key = 0;

        //the robot for physical movement
        public RobotAI robot;

        //the abstract agent for observing and choosing the actions
        public AbstractAgent abstractAgent;

        //tiles for visualisation
        [SerializeField] public GameObject bot, box, goal; //, calc;       




        // Update is called once per frame
        void Update()
        {
            //the coordinates of the robot are typecasted to integers
            robot.AbstractionMapping();

            rotI = (int) robot.GetRot();

            //move forward
            if (key==1) //Input.GetKeyUp("u")
            {
                robot.MoveAbstract(1);

                posX = robot.robotPosX;
                posZ = robot.robotPosZ;

                moveD = 1;
                
            }

            //moves forward while the position is not changed in the forward direction
            if(moveD == 1)
            {
                robot.MoveAbstract(1);

                //If the position changes too much sideways the movement also stops
                if (robot.robotPosZ != posZ && rotD == 0 | robot.robotPosX < posX - 2 | robot.robotPosX > posX + 2)
                {
                    moveD = 0;
                }

                //moves to a side depending on the value of rotD and initiates turning back to normal
                if (robot.robotPosX != posX && rotD != 0 | robot.robotPosZ < posZ - 2 | robot.robotPosZ > posZ + 2)
                {
                    if (rotD == 1)
                    {
                        moveD = 4;

                        rotD = 0;
                    }

                    if (rotD == 2)
                    {
                        moveD = 3;

                        rotD = 0;
                    }
                }
            }

            //move backwards
            if (key==2) //Input.GetKeyUp("j")
            {
                robot.MoveAbstract(2);

                posX = robot.robotPosX;
                posZ = robot.robotPosZ;

                moveD = 2;

            }

            if (moveD == 2)
            {
                robot.MoveAbstract(2);

                if (robot.robotPosZ != posZ)
                {
                    moveD = 0;
                }
            }

            //move right
            if (key==3) //Input.GetKeyUp("k")
            {
                robot.MoveAbstract(3);

                moveD = 3;
                rotD = 1;
            }

            if (moveD == 3)
            {
                robot.MoveAbstract(3);

                rotI = (int)robot.GetRot();

                //rotates 90 degrees and initiates moving forward
                if (rotI < 271 && rotI > 269 && rotD != 0)
                {
                    moveD = 1;

                    posX = robot.robotPosX;
                    posZ = robot.robotPosZ;
                }

                //moves back to normal and stops the movement
                if (rotD == 0 && rotI < 5 && rotI > 0)
                {
                    moveD = 0;
                }
            }

            //move left
            if (key==4) //Input.GetKeyUp("h")
            {
                robot.MoveAbstract(4);

                moveD = 4;
                rotD = 2;
            }

            if (moveD == 4)
            {
                robot.MoveAbstract(4);

                rotI = (int)robot.GetRot();

                //rotates 90 degrees and initiates moving forward
                if (rotI < 91 && rotI > 89 && rotD !=0)
                {
                    moveD = 1;

                    posX = robot.robotPosX;
                    posZ = robot.robotPosZ;
                }

                //moves back to normal and stops the movement
                if (rotD == 0 && rotI < 5 && rotI > 0)
                {
                    moveD = 0;
                }
            }


            //the positions of the objects are saved here for visualisation
            robotPosX = robot.robotPosX;
            robotPosZ = robot.robotPosZ;

            boxPosX = robot.boxPosX;
            boxPosZ = robot.boxPosZ;

            goalPosX = robot.goalPosX;
            goalPosZ = robot.goalPosZ;

            //calcPosX = robot.calcStepX;
            //calcPosZ = robot.calcStepZ;

            //the positions of the tiles are set for visualisation
            bot.transform.position = new Vector3((float)robotPosX, bot.transform.position.y, (float)robotPosZ);
            box.transform.position = new Vector3((float)boxPosX, 81.4f, (float)boxPosZ);
            goal.transform.position = new Vector3((float)goalPosX, goal.transform.position.y, (float)goalPosZ);
            //calc.transform.position = new Vector3((float)calcPosX, goal.transform.position.y, (float)calcPosZ);


            //the positions of the tiles are set for visualisation in abstract training
            /*bot.transform.position = new Vector3((float)abstractAgent.agentPosX, bot.transform.position.y, (float)abstractAgent.agentPosZ);
            box.transform.position = new Vector3((float)abstractAgent.boxPosX, 81.4f, (float)abstractAgent.boxPosZ);
            goal.transform.position = new Vector3((float)abstractAgent.goalPosX, goal.transform.position.y, (float)abstractAgent.goalPosZ);*/

            //the box tile dissapears once the box is picked up
            if (robot.isPickedUp)
            {
                box.transform.position = new Vector3((float)boxPosX, 0.0f, (float)boxPosZ);
            }

            //when no movement is being processed a new descision is requested by the agent
            if (moveD == 0)
            {
                abstractAgent.RequestDecision();
            }
            else //when the agent chooses a movement by manipulating the key variable, the variable is set back to 0
            {
                key = 0;
            }

        }
    }
}
