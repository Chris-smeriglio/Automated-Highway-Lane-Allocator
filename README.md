# Automated-Highway-Lane-Allocator
A project utilizing unity and C# to test the practicality on dynamically allocating freeway lanes to improve traffic.This was implemented as my senior design project for Wentworth Institute of Technology Class of 2019.
## Purpose
Traffic congestion during peak hours is an ever-growing problem that is affecting millions of people across the United States. On average, American commuters spend hundreds of hours in traffic each year costing them thousands of dollars in gas and lost productivity at work. And although several cities have taken the initiative to combat congestion, there has been no significant progress in decreasing rush hour traffic. The current “Road Zipper” method that is most commonly used across the country is helpful, but at times inefficient due to the truck’s slow speed and inability to adapt to incoming traffic. The goal of this project is to implement a system that can collect data through sensors and utilize the information to automatically adjust highway barriers to improve overall throughput.

This part of the project is the simulation that tests the practicality of the design.


## Design
There are 3 main scripts that run the simulation. Cars move in the X axis (accross the screen) and change lanes in the Y axis. The Z axis has cars at 0, and the road at 10 (behind). each script has a FixedUpdate that runs every .02s (50x/sec). Fixed update was used to apply consistant velocity and acceleration vectors. all variables are public unless specified otherwise, this is convenient as public variables can be modified in the Unity GUI. 
### Car Movement (`carMovementL` and `carMovementR`)
  - Variables
    * `Velocity`: The cars current velocity in all 3 dimensions. Initialized to 0.
    * `velocityMPH`: y value of Velocity converted to MPH.
    * `topSpeed`: Set by CarController, is the max speed that a vehicle will attempt to travel. This varies because in real life people have varying comfort levels for the speed they travel.
    * `lane`: The current lane a car is in.
    * `Acceleration`: A vector that will modify `velocity` every `FixedUpdate` if `velocity.x != topSpeed`. 
    * `slowPoke`: A counter for how long a car has been "stuck" behind another car. This increments everytime they have to slow down because a car is in front of them. it is reset what a car changes lanes.
    * `carUp, carDown, carFwd`: Flags used to tell whether a car is nearby so that the car knows wether or not it can go in that direction. these default to true, assuning a car is in all positions. set if `checkCars == true` and no cars are in the desired area. 
    * `checkCars`: set by `CarController` only when slowPoke exceeds 100. Initializes to false, not checking cars.
    * `goingUp` and `goingDown`: Flags for if the car is changing lanes. only set when `CarUp` or `carDown` is set. When true, `velocity.y = 1f` and `lane` is changed accordingly. Initializes to false, not going up or down.
  - Functions
    * `Start`: Called before first frame update. Initializes all values
    * `accelerate`: adds `acceleration` vector to the current velocity vector. called during `FixedUpdate` if the car isnt already going its top speed.
    * `slowDown`: adds `brakes` vector to the current velocity vector. called during `FixedUpdate` if the `carFwd` flag is set and it needs to slow down.
    * `stopLaneChange`: resets `slowPoke, carUp, carDown, goingUp, goingDown, velocity.y, and checkCars` to the values they were initialized with. 
### Car Controller
- Variables
  * `respawnTimeL` and `respawnTimeR`: Time in seconds between each car spawn. 1 car will spawn during the time specified here, so specifying 0.2 would spawn 5 cars in 1 second. different for each direction.
  * `topSpeedMaxR, topSpeedMinR, TopspeedMaxL, topSpeedMinL`: Velocity in MPH that the cars will attempt to travel between. this sets the `topSpeed` variable for carMovement.
  * `validLanesL` and `validLanesR`: valid lanes for travel.
  
- Spawn Cars (`carTimingL()` and `carTimingR()`)
  * spawn cars at a set interval based on the adjustable `respawnTimeL` and `respawnTimeR`
  * call `setSprite()` to randomly choose the color of a car
  * call `setLane()` to randomly assign a lanes. this depends on `validLanesL` and `validLanesR`
  * assigns `velocity` and `topSpeed` using `rand()` function
- `SetSprite(GameObject)`: randomly set the color of the car
- `SetLane(int)`: set lane of a car based on integer given. 0-3 goes bottom to top.
- `Save`: Export data to CSV. Includes average speed, total cars and a calculated throughput for each side.

## Use
### Lane Assignment
Lane assignment is done based on a 0-3 integer where 3 is the top lane of the visual and 0 is the bottom lane.
