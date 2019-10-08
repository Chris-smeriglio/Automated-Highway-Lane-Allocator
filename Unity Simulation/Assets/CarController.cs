using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Text;
using System.IO;

public class CarController : MonoBehaviour {
    //ADJUSTABLES
    public float respawnTimeR, respawnTimeL;//Adjust to increase traffic flow (lower = faster/higher = slower)
    public float topSpeedMaxR = 60, topSpeedMinR = 40, topSpeedMaxL = 60, topSpeedMinL = 40; //in MPH. top speed a car will try to reach. randomized
    public int validLanesL, validLanesR; //valid lanes for travel

    //PUBLIC
    public GameObject carPrefabL, carPrefabR, barrierPrefab, roadPrefab;//prefabs. must be defined in main camera
    public Sprite greenCar, redCar, brownCar, orangeCar; // respectively 0, 3, 2, 1. must be defined in main camera

    //PRIVATE
    private List<GameObject> carsR = new List<GameObject>(); //list of cars going right to left
    private List<GameObject> removeCarsR = new List<GameObject>(); //list of cars to remove once past a point
    private List<GameObject> carsL = new List<GameObject>(); //list of cars going left to right
    private List<GameObject> removeCarsL = new List<GameObject>(); //list of cars to remove once past a point
    private List<GameObject> Barriers = new List<GameObject>(); //List of Barriers
    private float car1posR, car1posL, carDiffx, carDiffy; //compare car positions
    private int randLane; //set lanes
    private bool laneChangeR = false, laneChangeL = true;
    private static float scalar = (1f / 5.246f); //convert from mph
    private int k = 0, timeCount = 0; //counts fixed update. timeCount is in seconds

    //CSV VARIABLES
    private List<float[]> trafficData = new List<float[]>();
    public float[] trafficDataTemp = new float[7];//creating string variable to hold data in CSV file

    //TEXT VARIABLES
    public Text speedTextL, speedTextR, outputTextL, outputTextR, totalTextL, totalTextR; //Text variable
    private float avgSpeedL = 0, avgSpeedR = 0, throughputL = 0, throughputR = 0; //tracking avg speed of cars and throughput
    private int totalCarsL = 0, totalCarsR = 0; //tracking total amount of cars

    // Start is calld before the first frame update
    void Start() {
        StartCoroutine(carTimingR());//start spawning cars
        StartCoroutine(carTimingL());

        GameObject road1 = Instantiate(roadPrefab) as GameObject;
        road1.transform.position = new Vector3(-160.5f, 1.4f, 10f);
        Vector3 roadPos = road1.transform.position;
        for (float i = -160.5f; i < 160f; i += 19.9f) {
            GameObject roadObj = Instantiate(roadPrefab) as GameObject;
            roadPos.x += 19.9f;
            roadObj.transform.position = roadPos;
        }//spawn roads
    }//start

    void setSprite(GameObject obj)//randomly choose a sprite.
        {
        int randSprite = Random.Range(0, 4);
        switch (randSprite) {
            case 0:
                obj.GetComponent<SpriteRenderer>().sprite = greenCar;
                break;
            case 1:
                obj.GetComponent<SpriteRenderer>().sprite = redCar;
                break;
            case 2:
                obj.GetComponent<SpriteRenderer>().sprite = orangeCar;
                break;
            default:
                obj.GetComponent<SpriteRenderer>().sprite = brownCar;
                break;
        }//switch
    }//setSprite()

    float setLane(int randLane)//sets a random lane for each car 
        {
        switch (randLane) {
            case 0:
                randLane = 6;
                return -2.5572f;
            case 1:
                randLane = 6;
                return 0.1108f;
            case 2:
                randLane = 6;
                return 2.778f;
            default:
                randLane = 0;
                return 5.4468f;
        }//switch
    }//setLane()

    void Save() //Saves data to CSV file
{
        trafficDataTemp = new float[7];
        trafficDataTemp[0] = avgSpeedL;
        trafficDataTemp[1] = totalCarsL;
        trafficDataTemp[2] = throughputL;
        //skip 3, use as spacer
        trafficDataTemp[4] = avgSpeedR;
        trafficDataTemp[5] = totalCarsR;
        trafficDataTemp[6] = throughputR;
        trafficData.Add(trafficDataTemp);

        float[][] output = new float[trafficData.Count][];

        for (int i = 1; i < output.Length; i++) {
            output[i] = trafficData[i];
        }

        int length = output.GetLength(0);
        string delimiter = ",";
        StringBuilder sb = new StringBuilder();

        for (int index = 1; index < length; index++) {
            sb.AppendLine(string.Join(delimiter, output[index]));
        }

        string filePath = Application.dataPath + "/" + "L" + (validLanesL+1) + "_" + respawnTimeL + "R" + (4-validLanesR) + "_" + respawnTimeR + ".csv"; //file name

        StreamWriter outStream = File.CreateText(filePath);
        outStream.WriteLine(sb);
        outStream.Close();
    }//Save()

    void SetInfoText() { //Sets display to visualize
        speedTextL.text = "Bottom Avg Speed: " + (int)avgSpeedL + " MPH";
        speedTextR.text = "Top Avg Speed: " + (int)avgSpeedR + " MPH";
        outputTextL.text = "Bottom Throughput: " + (int)throughputL + " Cars/Hour";
        outputTextR.text = "Top Throughput: " + (int)throughputR + " Cars/Hour";
        totalTextL.text = "Total Cars (Bottom): " + totalCarsL + " Cars";
        totalTextR.text = "Total Cars (Top): " + totalCarsR + " Cars";
    }//setInfoText()

    IEnumerator carTimingL() { //time cars according to spawn rate. right now they share a spawn rate. Will change though
        while (true) {
            yield return new WaitForSeconds(respawnTimeL);
            GameObject CarObj = Instantiate(carPrefabL) as GameObject; //Create the car RIGHT TO LEFT (TOP)

            setSprite(CarObj); //set a random sprite

            CarObj.GetComponent<CarMovementL>().acceleration.x = Random.Range(0.5f, 1f);//Set Acceration Vector ->
            CarObj.GetComponent<CarMovementL>().brakes.x = -Random.Range(0.5f, 1f);//Set Brake Vector <- neg direction
            CarObj.GetComponent<CarMovementL>().topSpeed = Random.Range(topSpeedMinL * scalar, topSpeedMaxL * scalar);//Set topSpeed

            randLane = Random.Range(0, validLanesL + 1);//choose lane
            CarObj.GetComponent<CarMovementL>().lane = randLane;//set cars lane
            CarObj.transform.position = new Vector3(-175f, setLane(randLane), 0f);//set car into correct location based on lane

            carsL.Add(CarObj);
        }
    }//carTimingL()

    IEnumerator carTimingR() { //time cars according to spawn rate. right now they share a spawn rate. Will change though
        while (true) {
            yield return new WaitForSeconds(respawnTimeR);
            GameObject CarObj = Instantiate(carPrefabR) as GameObject; //Create the car RIGHT TO LEFT (TOP)

            setSprite(CarObj); //set a random sprite

            CarObj.GetComponent<CarMovementR>().acceleration.x = -Random.Range(0.5f, 1f);//Set Acceleration Vector <- neg direction
            CarObj.GetComponent<CarMovementR>().brakes.x = Random.Range(0.5f, 1f);//Set brake vector ->
            CarObj.GetComponent<CarMovementR>().topSpeed = Random.Range(-(topSpeedMaxR * scalar), -(topSpeedMinR * scalar));//set top speed

            randLane = Random.Range(validLanesR, 4);//choose lane
            CarObj.GetComponent<CarMovementR>().lane = randLane;//set cars lane
            CarObj.transform.position = new Vector3(175f, setLane(randLane), 0f);//set car into correct location based on lane

            carsR.Add(CarObj);
        }
    }//carTimingR()

    void FixedUpdate() { //runs every .02 sec
        k++;//keep track of how many updates. 1 k = .02s, 50k = 1s

/*  Utilized to test varrying spawn rates and lane allocation
       if (timeCount == 8) {
            StopAllCoroutines();//stop spawning
            for (int i = 0; i < carsL.Count; i++) {//destroy all cars L
                Destroy(carsL[i]);
            }
            for (int i = 0; i < carsR.Count; i++) {//destroy all cars R
                Destroy(carsR[i]);
            }
            carsR.Clear();//clear lists
            carsL.Clear();

            validLanesR -= 1;//adjust lanes

            if (validLanesR == 0) {//if last lane set up, reset and decrement spawn time
                respawnTimeL -= .01f;
                //respawnTimeR -= .1f;
                if(respawnTimeR <= 0f || respawnTimeL <= 0f) { //if it reaches 0, quit!
                    Application.Quit();
                }
                validLanesR = 3;
            }

            validLanesL = validLanesR - 1; //never have overlapping lanes

            StartCoroutine(carTimingR());//start spawning cars again
            StartCoroutine(carTimingL());

            timeCount = 0;//reset timer
        }*/

        for (int i = 0; i < carsR.Count; i++) { //Get car for checks

            if (k > 50) {//calculate after set time
                avgSpeedR += (carsR[i].GetComponent<CarMovementR>().velocityMPH); //Running sum of current speed L
                if (i == carsR.Count - 1) {//calculate average
                    avgSpeedR = (-avgSpeedR) / (float)i;
                    throughputR = avgSpeedR * (float)(i + 1);
                    totalCarsR = (i + 1);
                }
            }

            //if the car desires to pass 
            if (carsR[i].GetComponent<CarMovementR>().slowPoke > 200 && carsR[i].GetComponent<CarMovementR>().checkCars == false) {
                carsR[i].GetComponent<CarMovementR>().checkCars = true; //set to start looking for cars
            }

            car1posR = carsR[i].transform.position.x;//get car position

            if (car1posR <= -175f) {//if past half a mile add it to a list to remove and dont compare to other cars
                removeCarsR.Add(carsR[i]);

            } else {
                for (int j = 0; j < carsR.Count - 1; j++) {
                    if (i == j) j++;
                    carDiffx = car1posR - carsR[j].transform.position.x;
                    carDiffy = carsR[i].GetComponent<CarMovementR>().lane - carsR[j].GetComponent<CarMovementR>().lane;

                    if (carsR[i].GetComponent<CarMovementR>().checkCars == true && laneChangeR == false) {

                        //if car is 1 lane above && (  less  than 4 behind, OR less than 2.8 ahead ) OR already in highest lane
                        if ((carDiffy == 1 && (carDiffx < -4 || carDiffx < 2.8)) || (carsR[i].GetComponent<CarMovementR>().lane == 3)) {

                            //there is a car or barrier up. Dont move up
                            carsR[i].GetComponent<CarMovementR>().carUp = true;
                            carsR[i].GetComponent<CarMovementR>().checkCars = false;
                            carsR[i].GetComponent<CarMovementR>().slowPoke = 0;
                        }
                        //no car, allowed to move up
                        else {
                            carsR[i].GetComponent<CarMovementR>().carUp = false;
                            laneChangeR = true;
                        }

                        //if car is 1 lane below && ( less than 4 behind, OR less than 2.8 ahead ) OR already in lowest lane
                        if ((carDiffy == -1 && (carDiffx < -4 || carDiffx < 2.8)) || (carsR[i].GetComponent<CarMovementR>().lane == validLanesR)) {
                            //there is a car down
                            carsR[i].GetComponent<CarMovementR>().carDown = true;
                            carsR[i].GetComponent<CarMovementR>().checkCars = false;
                            carsR[i].GetComponent<CarMovementR>().slowPoke = 0;
                        }
                        //if that lane is valid
                        else {
                            carsR[i].GetComponent<CarMovementR>().carDown = false;
                            laneChangeR = true;
                        }
                    }

                    //if car is in front
                    if (carDiffx < 2.8f && carDiffx > .01f && carDiffy == 0) {
                        carsR[i].GetComponent<CarMovementR>().carFwd = true;
                        carsR[i].GetComponent<CarMovementR>().slowPoke += 1;
                        break;
                    } else carsR[i].GetComponent<CarMovementR>().carFwd = false;
                }
            }
        }
        //remove cars that are too far
        foreach (GameObject car in removeCarsR) {
            carsR.Remove(car);//remove from list
            Destroy(car);//destroy object
        }
        removeCarsR.Clear();//clear list to remove

        //Get car for checks
        for (int i = 0; i < carsL.Count; i++) {

            if (k > 50) {
                avgSpeedL += (carsL[i].GetComponent<CarMovementL>().velocityMPH); //Running sum of current speed L
                if (i == carsL.Count - 1) {
                    avgSpeedL = avgSpeedL / (float)i;
                    throughputL = avgSpeedL * (float)(i + 1);
                    totalCarsL = (i + 1);
                    SetInfoText();
                    Save();
                    avgSpeedR = 0;
                    avgSpeedL = 0;
                    k = 0;
                    timeCount++;
                }
            }
            //if the car desires to pass 
            if (carsL[i].GetComponent<CarMovementL>().slowPoke > 100 && carsL[i].GetComponent<CarMovementL>().checkCars == false) {
                carsL[i].GetComponent<CarMovementL>().checkCars = true; //set to start looking for cars
            }

            car1posL = carsL[i].transform.position.x;
            if (car1posL >= 175f) {
                removeCarsL.Add(carsL[i]);

            } else {
                for (int j = 0; j < carsL.Count - 1; j++) {
                    if (i == j) j++;
                    carDiffx = car1posL - carsL[j].transform.position.x;
                    carDiffy = carsL[i].GetComponent<CarMovementL>().lane - carsL[j].GetComponent<CarMovementL>().lane;

                    if (carsL[i].GetComponent<CarMovementL>().checkCars == true && laneChangeL == false) {
                        //if car is 1 lane above && (  less  than 4 behind, OR less than 2.8 ahead ) OR already in highest lane
                        if ((carDiffy == 1 && (carDiffx > -6 || carDiffx < 4)) || (carsL[i].GetComponent<CarMovementL>().lane == validLanesL)) {
                            //there is a car or barrier up. Dont move up
                            carsL[i].GetComponent<CarMovementL>().carUp = true;
                            carsL[i].GetComponent<CarMovementL>().checkCars = false;
                            carsL[i].GetComponent<CarMovementL>().slowPoke = 0;
                        }
                        //no car, allowed to move up
                        else {
                            laneChangeL = true;
                            carsL[i].GetComponent<CarMovementL>().carUp = false;
                        }

                        //if car is 1 lane below && ( less than 4 behind, OR less than 2.8 ahead ) OR already in lowest lane
                        if ((carDiffy == -1 && (carDiffx > -6 || carDiffx < 4)) || (carsL[i].GetComponent<CarMovementL>().lane == 0)) {
                            //there is a car down
                            carsL[i].GetComponent<CarMovementL>().carDown = true;
                            carsL[i].GetComponent<CarMovementL>().checkCars = false;
                            carsL[i].GetComponent<CarMovementL>().slowPoke = 0;
                        }
                        //no car, allowed to move down
                        else {
                            laneChangeL = true;
                            carsL[i].GetComponent<CarMovementL>().carDown = false;
                        }
                    }//check cars

                    //if car is in front
                    if (carDiffx > -2.8f && carDiffx < -.01f && carDiffy == 0) {
                        carsL[i].GetComponent<CarMovementL>().carFwd = true; //there is a car fwd
                        carsL[i].GetComponent<CarMovementL>().slowPoke += 1;
                        break;
                    } else carsL[i].GetComponent<CarMovementL>().carFwd = false; //no car fwd, approach top speed
                }//for car L 2
            }//else car L 2
        }//foor car L 1

        foreach (GameObject car in removeCarsL) {
            carsL.Remove(car);
            Destroy(car);
        }//removeCarsL
        removeCarsL.Clear();
        if ((k % 25) == 0) {//limit lane changes to 2 per second in each direction
            laneChangeR = false;
            laneChangeL = false;
        }//k%25
    }//Fixed Update
}//car Controller
