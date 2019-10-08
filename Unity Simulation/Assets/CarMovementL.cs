using UnityEngine;

public class CarMovementL : MonoBehaviour {
    public float topSpeed; //top speed car will attempt to travel
    public float velocityMPH; //current velocity in MPH

    public int slowPoke = 0; //increments everytime a driver slows down
    public int lane; //0 to 3 where 0 is bottom, and 3 is top
    public Vector3 acceleration, brakes, velocity; //3D vectors for controlling car movements

    public bool carFwd = false, carUp = false, carDown = false; //flags for seeing if cars are around
    public bool goingUp, goingDown, checkCars; // if the car is actively changing lanes or looking to change lanes
    public float originalY;//stroe original y position to calculate delta y when changing lanes

    // Start is called before the first frame update
    void Start() {
        velocity.y = 0f;//start at 0
        carUp = true;//assume cars are there
        carDown = true;
        goingDown = false;//default state
        goingUp = false;
        slowPoke = 0;
        checkCars = false;
    }

    void accelerate() {
        velocity += acceleration;
    }

    void slowDown() {
        velocity += brakes;//apply the brakes vector
        if (velocity.x < 0) velocity = new Vector3(0f, velocity.y, velocity.z);//stop, dont go into reverse
    }

    void stopLaneChange() {// reset all variables, stop looking for cars
        velocity.y = 0f;
        carUp = true;
        carDown = true;
        goingDown = false;
        goingUp = false;
        slowPoke = 0;
        checkCars = false;

    }

    // FixedUpdate is called 50 times every second
    void FixedUpdate() {
        if (velocity.x < topSpeed && !carFwd && (!goingDown || !goingUp)) {//if not changing lanes and not at top speed
            accelerate();
        } 

        else if (carFwd) slowDown();//if there is a car in front, brake


        if (carUp == false && goingDown != true) {//if not going down and no car up
            if (!goingUp) {//start going up
                velocity.y = 1f;
                originalY = transform.position.y;
                lane += 1;
            }
            goingUp = true;//keep going up
        }

        if (carDown == false && goingUp != true) {//if not going up and no car down
            if (!goingDown) {//start going down
                velocity.y = -1f;
                originalY = transform.position.y;
                lane -= 1;
            }
            goingDown = true;//keep going down

        }

        if (goingUp && transform.position.y >= originalY + 2.668f) {//if done moving up
            stopLaneChange();
        }
        if (goingDown && transform.position.y <= originalY - 2.668f) {//if done moving down
            stopLaneChange();

        }
        
        velocityMPH = velocity.x * 5.246f; //visualize velocity

        transform.position += velocity * Time.deltaTime;


    }
}
