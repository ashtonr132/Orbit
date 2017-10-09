using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class Pickup : MonoBehaviour
{
    private float score = 0, newGForce;
    private GameObject sphere, sHead, asteroid;
    private Text scoreText;
    private Vector3 fVector;

    private void Start()
    {
        scoreText = GameObject.Find("scoreText").GetComponent<Text>(); //grab bits needed for calcs
        sphere = GameObject.Find("Sphere");
        sHead = GameObject.Find("Snake");
        asteroid = GameObject.Find("Asteroid");
    }

    private void Update()
    {
        
        GameObject.Find("pickupspotlight").transform.position = (transform.position - sphere.transform.position).normalized * 2;
        GameObject.Find("pickupspotlight").transform.rotation = Quaternion.LookRotation(-transform.position); //grab and move spotlight
        List<GameObject> list = sHead.GetComponent<SnakeHead>().getPartsList(); //getting snake length pre calc
        RaycastHit hit;
        Physics.Raycast(transform.position, sphere.transform.position, out hit);

        if (Vector3.Distance(hit.point, transform.position) >= 1.05) //doGravity
        {
            newGForce = 9.81f * (sphere.GetComponent<Rigidbody>().mass * gameObject.GetComponent<Rigidbody>().mass / Vector3.Distance(sphere.transform.position, transform.position));
            fVector = (sphere.transform.position - transform.position).normalized * newGForce;
            gameObject.GetComponent<Rigidbody>().AddForce(fVector);
        }
        else
        {
            gameObject.GetComponent<Rigidbody>().isKinematic = true;
        }

        if (Vector3.Distance(transform.position, list[0].transform.position) < 0.175f || Vector3.Distance(transform.position, sphere.transform.position) < 0.175f) //proximity for giving points, is it eaten?
        {
            sHead.GetComponent<SnakeHead>().addBodyPart();
            score++;
            scoreText.text = "Score : " + score.ToString();
            transform.position = Random.onUnitSphere * 3; //respawn loca in space
            gameObject.GetComponent<Rigidbody>().isKinematic = false; //unfreeze to fall
            gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero; //reset velo so that the asteroid doesnt fuck off into space
        }
    }

    public float getScore()
    {
        return score;
    }

    public void setScore(float into)
    {
        score = score + into;
    }
}