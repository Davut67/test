using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StuPro
{
    public class Randomizer : MonoBehaviour
    {
        // Struct 'Obstacle' defines props of the obstacles on the platform
        [System.Serializable]
        public class Obstacle
        {
            public string id;
            public GameObject obstacle;
            public ArticulationBody articulationBody;
            public bool global;
            public bool randomize;
            public bool randomizeRotation;
            public bool rotated;
            public float y;
            public float minDistanceX;
            public float minDistanceZ;
        }

        [SerializeField] List<Obstacle> obstacles = new List<Obstacle>();
        [SerializeField] List<Obstacle> randomized = new List<Obstacle>();
        [SerializeField] GameObject boundary1, boundary2;
        [SerializeField] bool demonstrationOn = false;

        void Start()
        {
            if (demonstrationOn) Time.timeScale = 5;
        }

        public void RandomizeObstacles(float difficulty)
        {
            if (demonstrationOn) difficulty = 11;

            Obstacle robot = new Obstacle();
            Obstacle target = new Obstacle();
            Obstacle dropoff = new Obstacle();
            //Obstacle wall1 = new Obstacle();
            //Obstacle wall2 = new Obstacle();
            //Obstacle wall3 = new Obstacle();
            //Obstacle wall4 = new Obstacle();
            //Obstacle wall5 = new Obstacle();

            for (int i = 0; i < obstacles.Count; i++)
            {
                switch (obstacles[i].id)
                {
                    case "AI-Bot": robot = obstacles[i]; break;
                    case "Target": target = obstacles[i]; break;
                    case "DropZone": dropoff = obstacles[i]; break;
                    //case "ZaunRW1": wall1 = obstacles[i]; break;
                    //case "ZaunRW2": wall2 = obstacles[i]; break;
                    //case "ZaunRW3": wall3 = obstacles[i]; break;
                    //case "ZaunRW4": wall4 = obstacles[i]; break;
                    //case "ZaunRW5": wall5 = obstacles[i]; break;
                }
            }

            switch (difficulty)
            {
                case 1:
                    robot.randomize = false;
                    target.randomize = false;
                    dropoff.randomize = false;
                    TeleportObstacleToPosition(robot, new Vector3(0, .5f, -3f));
                    TeleportObstacleToPosition(target, new Vector3(0, .3f, .2f));
                    TeleportObstacleToPosition(dropoff, new Vector3(0, .005f, 2));
                    RandomizeAll(1,1,1,1);
                    break;

                case 2:
                    robot.randomize = false;
                    target.randomize = false;
                    dropoff.randomize = false;
                    //wall1.randomize = true;
                    TeleportObstacleToPosition(robot, new Vector3(0, .5f, -3f));
                    TeleportObstacleToPosition(target, new Vector3(0, .3f, 0.2f));
                    TeleportObstacleToPosition(dropoff, new Vector3(Random.Range(-1f,1f), .005f, 2));
                    RandomizeAll(1,1,1,1);
                    break;

                case 3:
                    robot.randomize = false;
                    target.randomize = false;
                    dropoff.randomize = false;
                    //wall1.randomize = true;
                    //wall2.randomize = true;
                    TeleportObstacleToPosition(robot, new Vector3(0, .5f, -3f));
                    TeleportObstacleToPosition(target, new Vector3(0, .3f, 0.2f));
                    TeleportObstacleToPosition(dropoff, new Vector3(Random.Range(-2f,2f), .005f, 2));
                    RandomizeAll(1,1,1,1);
                    break;

                case 4:
                    robot.randomize = false;
                    target.randomize = false;
                    dropoff.randomize = false;
                    //wall1.randomize = true;
                    //wall2.randomize = true;
                    //wall3.randomize = true;
                    TeleportObstacleToPosition(robot, new Vector3(0, .5f, -3f));
                    TeleportObstacleToPosition(target, new Vector3(0, .3f, 0.2f));
                    TeleportObstacleToPosition(dropoff, new Vector3(Random.Range(-4f,4f), .005f, 2));
                    RandomizeAll(1,1,1,1);
                    break;

                case 5:
                    robot.randomize = false;
                    target.randomize = false;
                    dropoff.randomize = true;
                    //wall1.randomize = true;
                    //wall2.randomize = true;
                    //wall3.randomize = true;
                    //wall4.randomize = true;
                    TeleportObstacleToPosition(robot, new Vector3(0, .5f, -3f));
                    TeleportObstacleToPosition(target, new Vector3(0, .3f, 0.2f));
                    TeleportObstacleToPosition(dropoff, new Vector3(Random.Range(-6f,6f), .005f, 2));
                    RandomizeAll(1,1,1,1);
                    break;

                case 6:
                    robot.randomize = false;
                    target.randomize = false;
                    dropoff.randomize = true;
                    //wall1.randomize = true;
                    //wall2.randomize = true;
                    //wall3.randomize = true;
                    //wall4.randomize = true;
                    //wall5.randomize = true;
                    TeleportObstacleToPosition(robot, new Vector3(0, .5f, -3f));
                    TeleportObstacleToPosition(target, new Vector3(0, .3f, 0.2f));
                    RandomizeAll(1,1,.2f,.2f);
                    break;

                case 7:
                    robot.randomize = true;
                    target.randomize = false;
                    dropoff.randomize = true;
                    dropoff.global = true;
                    //wall1.randomize = true;
                    //wall2.randomize = true;
                    //wall3.randomize = true;
                    //wall4.randomize = true;
                    //wall5.randomize = true;
                    TeleportObstacleToPosition(robot, new Vector3(0, .5f, -3f));
                    TeleportObstacleToPosition(target, new Vector3(0, .3f, 0.2f));
                    RandomizeAll(1,1,.3f,.3f);
                    break;

                case 8:

                    robot.randomize = true;
                    target.randomize = true;
                    dropoff.randomize = true;
                    dropoff.global = true;
                    //wall1.randomize = true;
                    //wall2.randomize = true;
                    //wall3.randomize = true;
                    //wall4.randomize = true;
                    //wall5.randomize = true;
                    TeleportObstacleToPosition(robot, new Vector3(Random.Range(-1f, 1f), .5f, Random.Range(-1f, 1f)));
                    RandomizeAll(1, 1, .3f, .3f);

                    break;

                case 9:
                    robot.randomize = true;
                    robot.global = true;
                    target.randomize = true;
                    target.global = true;
                    dropoff.randomize = true;
                    dropoff.global = true;
                    //wall1.randomize = true;
                    //wall2.randomize = true;
                    //wall3.randomize = true;
                    //wall4.randomize = true;
                    //wall5.randomize = true;
                    TeleportObstacleToPosition(robot, new Vector3(Random.Range(-2f, 2f), .5f, Random.Range(-2f, 2f)));
                    RandomizeAll(1, 1, .5f, .5f);
                    break;

                case 10:
                    robot.randomize = true;
                    robot.global = true;
                    target.randomize = true;
                    target.global = true;
                    dropoff.randomize = true;
                    dropoff.global = true;
                    //wall1.randomize = true;
                    //wall2.randomize = true;
                    //wall3.randomize = true;
                    //wall4.randomize = true;
                    //wall5.randomize = true;
                    TeleportObstacleToPosition(robot, new Vector3(Random.Range(-4f, 4f), .5f, Random.Range(-4f, 4f)));
                    RandomizeAll(1, 1, .75f, .75f);
                    break;

                case 11:
                    robot.randomize = true;
                    robot.global = true;
                    target.randomize = true;
                    target.global = true;
                    dropoff.randomize = true;
                    dropoff.global = true;
                    //wall1.randomize = true;
                    //wall2.randomize = true;
                    //wall3.randomize = true;
                    //wall4.randomize = true;
                    //wall5.randomize = true;
                    TeleportObstacleToPosition(robot, new Vector3(Random.Range(-6f, 6f), .5f, Random.Range(-6f, 6f)));
                    RandomizeAll(1, 1, 1, 1);
                    break;

                default:
                    robot.randomize = false;
                    target.randomize = false;
                    dropoff.randomize = false;
                    TeleportObstacleToPosition(robot, new Vector3(0, .5f, -3));
                    TeleportObstacleToPosition(target, new Vector3(0, .3f, 0));
                    TeleportObstacleToPosition(dropoff, new Vector3(0, .005f, 1));
                    RandomizeAll(1,1,1,1);
                    break;
            }
        }

        private void TeleportObstacleToPosition(Obstacle obstacle, Vector3 point)
        {
            obstacle.obstacle.transform.position = transform.parent.transform.position + point;
            if (obstacle.articulationBody) obstacle.articulationBody.TeleportRoot(transform.parent.transform.position + point, Quaternion.identity);
            Debug.DrawLine(point, point + Vector3.up, Color.red, 1f);
        }

        private void RandomizeAll(float xFactor1, float xFactor2, float zFactor1, float zFactor2)
        {
            for (int i = 0; i < obstacles.Count; i++)
            {
                Obstacle obst = obstacles[i];
                
                // Continue if obstacle should not be randomized
                if (!obst.randomize) 
                {
                    randomized.Add(obst);
                    continue;
                }

                if (obst.randomizeRotation)
                {
                    int rotation = Random.Range(0, 4);
                    float dummy;

                    switch (rotation)
                    {
                        case 3: obst.obstacle.transform.rotation = Quaternion.Euler(0, 270, 0); 
                                if (!obst.rotated)
                                {
                                    obst.rotated = true;
                                    dummy = obst.minDistanceX;
                                    obst.minDistanceX = obst.minDistanceZ;
                                    obst.minDistanceZ = dummy;
                                }
                                break;
                        case 2: obst.obstacle.transform.rotation = Quaternion.Euler(0, 180, 0); 
                                if (obst.rotated)
                                {
                                    obst.rotated = false;
                                    dummy = obst.minDistanceX;
                                    obst.minDistanceX = obst.minDistanceZ;
                                    obst.minDistanceZ = dummy;
                                }
                                break;
                        case 1: obst.obstacle.transform.rotation = Quaternion.Euler(0, 90, 0);
                                if (!obst.rotated)
                                {
                                    obst.rotated = true;
                                    dummy = obst.minDistanceX;
                                    obst.minDistanceX = obst.minDistanceZ;
                                    obst.minDistanceZ = dummy;
                                }
                                break;
                        default: obst.obstacle.transform.rotation = Quaternion.Euler(0, 0, 0); 
                                if (obst.rotated)
                                {
                                    obst.rotated = false;
                                    dummy = obst.minDistanceX;
                                    obst.minDistanceX = obst.minDistanceZ;
                                    obst.minDistanceZ = dummy;
                                }
                                 break;
                    }
                }

                // Find random position for obstacle
                bool notFound;
                float x, z;
                do
                {
                    notFound = false;
                    if (!obst.global)
                    {
                        x = Random.Range(xFactor1 * boundary1.transform.position.x + obst.minDistanceX, xFactor2 * boundary2.transform.position.x - obst.minDistanceX);
                        z = Random.Range(zFactor1 * boundary1.transform.position.z + obst.minDistanceZ, zFactor2 * boundary2.transform.position.z - obst.minDistanceZ);
                    }
                    else
                    {
                        x = Random.Range(boundary1.transform.position.x + obst.minDistanceX, boundary2.transform.position.x - obst.minDistanceX);
                        z = Random.Range(boundary1.transform.position.z + obst.minDistanceZ, boundary2.transform.position.z - obst.minDistanceZ);
                    }

                    for (int j = 0; j < randomized.Count; j++)
                    {
                        Vector3 pos = randomized[j].obstacle.transform.position; 
                        if ((x > pos.x - randomized[j].minDistanceX - obst.minDistanceX && x < pos.x + randomized[j].minDistanceX + obst.minDistanceX) &&
                            (z > pos.z - randomized[j].minDistanceZ - obst.minDistanceZ && z < pos.z + randomized[j].minDistanceZ + obst.minDistanceZ))
                        {
                            notFound = true; break;
                        }
                    }
                } while(notFound);

                // Set obstacle position
                Vector3 newPosition = new Vector3(x, transform.parent.position.y + obst.y, z);
                obst.obstacle.transform.position = newPosition;
                if (obst.articulationBody) obst.articulationBody.TeleportRoot(newPosition, Quaternion.identity);
                randomized.Add(obst);
            }

            randomized.Clear();
        }
    }
}
