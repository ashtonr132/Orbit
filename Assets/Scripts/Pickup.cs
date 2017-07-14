using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class Pickup : MonoBehaviour
{
    public float score = 0, lifeTime = 10;
    private GameObject sphere;
    private Text scoreText;

    private void Start()
    {
        scoreText = GameObject.Find("scoreText").GetComponent<Text>();
        sphere = GameObject.Find("Sphere");
        gameObject.GetComponent<MeshCollider>().isTrigger = true;
        StartCoroutine("pickupDecay");
       
    }

    private void Update()
    {
        GameObject.Find("pickupspotlight").transform.position = (transform.position - sphere.transform.position).normalized * 2;
        GameObject.Find("pickupspotlight").transform.rotation = Quaternion.LookRotation(-transform.position);
        List<GameObject> list = GameObject.Find("Snake Head").GetComponent<SnakeHead>().getPartsList();

            if (Vector3.Distance(sphere.transform.position, transform.position) < 1f)
            {
                transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z + 0.1f);
                transform.rotation = Quaternion.LookRotation(transform.forward, transform.position - sphere.transform.position);
            }
            transform.RotateAround(transform.position, (transform.position - sphere.transform.position).normalized, 45*Time.deltaTime);
            if (list.Count > 1)
            {
                if (Vector3.Distance(transform.position, list[0].transform.position) < 0.1f)
                {
                    transform.position = Random.onUnitSphere;
                    transform.rotation = Quaternion.LookRotation(transform.position - sphere.transform.position);
                    var go = list[list.Count - 1];
                    list.Remove(go);
                    var gop = go.transform.parent.gameObject;
                    Destroy(go);
                    Destroy(gop);
                    StopCoroutine("pickupDecay");
                    StartCoroutine("pickupDecay");
                    score++;
                    scoreText.text = "Score : " + score.ToString();
                }
            }
        }

    private IEnumerator pickupDecay()
    {
        yield return new WaitForSeconds(lifeTime);
        transform.position = Random.onUnitSphere;
        transform.rotation = Quaternion.LookRotation(transform.position - sphere.transform.position);
        StartCoroutine("pickupDecay");
    }

    public float getScore()
    {
        return score;
    }
}