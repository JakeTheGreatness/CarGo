using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(NNet))]
public class CarController : MonoBehaviour
{
    private Vector3 startPosition, startRotation;
    private NNet network;
    //accelartaion and turning variables
    [Range(-1f,1f)]
    public float acel,turn;
    //haw long it has been since the time started
    public float timeSinceStart = 0f;

    //How well an enity does 
    [Header("Fitness")]
    //network inputs
    public float overallFitness;
    public float distanceMultipler = 1.4f;
    public float averageSpeedMultiplier = 0.2f;
    public float sensorMultiplier = 0.1f;

    [Header("Network Options")]
    public int LAYERS = 1;
    public int NEURONS = 10;

    //used to calculate fitness
    private Vector3 lastPosition;
    private float totalDistanceTravelled;
    private float averageSpeed;

    //Distance values
    private float aSensor,bSensor,cSensor;

    private void Awake() {
        startPosition = transform.position;
        startRotation = transform.eulerAngles;
        network = GetComponent<NNet>();

        
    }

    public void ResetWithNetwork (NNet net)
    {
        network = net;
        Reset();
    }

    
    //Ever time car fails this function will be used
    public void Reset() {

        timeSinceStart = 0f;
        totalDistanceTravelled = 0f;
        averageSpeed = 0f;
        lastPosition = startPosition;
        overallFitness = 0f;
        transform.position = startPosition;
        transform.eulerAngles = startRotation;
    }

    private void OnCollisionEnter (Collision collision) {
        Death();
    }

    private void FixedUpdate() {

        InputSensors();
        lastPosition = transform.position;


        (acel, turn) = network.RunNetwork(aSensor, bSensor, cSensor);


        MoveCar(acel,turn);

        timeSinceStart += Time.deltaTime;

        CalculateFitness();



    }

    private void Death ()
    {
        GameObject.FindObjectOfType<GeneticManager>().Death(overallFitness, network);
    }

    private void CalculateFitness() {
        //set last postion to current postion
        totalDistanceTravelled += Vector3.Distance(transform.position,lastPosition);
        averageSpeed = totalDistanceTravelled/timeSinceStart;

       overallFitness = (totalDistanceTravelled*distanceMultipler)+(averageSpeed * averageSpeedMultiplier) +(((aSensor+bSensor+cSensor)/3)*sensorMultiplier);

        if (timeSinceStart > 20 && overallFitness < 40) {
            Death();
        }

        if (overallFitness >= 1000) {
            Death();
        }

    }

    private void InputSensors() {
        // directions of the car
        Vector3 a = (transform.forward+transform.right);
        Vector3 b = (transform.forward);
        Vector3 c = (transform.forward-transform.right);

        Ray r = new Ray(transform.position,a);
        RaycastHit hit;

        if (Physics.Raycast(r, out hit)) {
            aSensor = hit.distance/20;
            Debug.DrawLine(r.origin, hit.point, Color.red);
        }

        r.direction = b;
      
        if (Physics.Raycast(r, out hit)) {
            bSensor = hit.distance/20;
            Debug.DrawLine(r.origin, hit.point, Color.red);
        }

        r.direction = c;

        if (Physics.Raycast(r, out hit)) {
            cSensor = hit.distance/20;
            Debug.DrawLine(r.origin, hit.point, Color.red);
        }

    }

    private Vector3 inp;
    public void MoveCar (float v, float h) {
        //converts input relitive to car
        inp = Vector3.Lerp(Vector3.zero,new Vector3(0,0,v*11.4f),0.02f);
        inp = transform.TransformDirection(inp);
        transform.position += inp;
      
        transform.eulerAngles += new Vector3(0, (h*90)*0.02f,0);
    }

}
