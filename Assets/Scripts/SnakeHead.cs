using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class SnakeHead : MonoBehaviour
{
    private float moveSpeed;
    private Transform origin, prevPart = null;
    private List<GameObject> snakeBodyPartsList;
    public Mesh mesh;
    public Material bugMat;
    private GameObject body, sphere, snakeHead;
    private RaycastHit hit;

    void Start()
    {
        sphere = GameObject.Find("Sphere"); //get dodads
        origin = new GameObject("SnakeHead Origin").transform;
        origin.parent = sphere.transform;
        transform.parent = origin;
        snakeBodyPartsList = new List<GameObject>();
        snakeBodyPartsList.Add(gameObject);
        snakeHead = GameObject.Find("Snake Head");
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.Escape)) //press escape to quit game
        {
            Application.Quit();
        }

        foreach (GameObject part in snakeBodyPartsList)
        {
            moveSpeed = 30 + (snakeBodyPartsList.Count * 2); //movespeed increased relative to snake length
            if (part == gameObject)
            {
                if (Input.GetKey("a") || Input.GetKey("d")) //snakecontrols
                {
                    var turnRate = Input.GetKey("a") ? -140 : 140;
                    part.transform.parent.Rotate(0, 0, turnRate * Time.deltaTime);
                    snakeHead.transform.localRotation = Quaternion.Lerp(snakeHead.transform.localRotation, new Quaternion (0,0,turnRate/120*0.6f, 1), 5* Time.deltaTime);
                    moveSpeed /= 2;
                }
                else
                {
                    snakeHead.transform.localRotation = Quaternion.Lerp(snakeHead.transform.localRotation, new Quaternion(0, 0, 0, 1), 5* Time.deltaTime);
                }

                part.transform.rotation = Quaternion.LookRotation(transform.forward, part.transform.position - sphere.transform.position); // rotations and movedirection through parent for cartesian translation and reletivity dependacies for the head piece
                part.transform.parent.rotation = Quaternion.AngleAxis(moveSpeed * Time.deltaTime, Vector3.Cross(part.transform.parent.position - part.transform.position, part.transform.forward)) * part.transform.parent.rotation;
            }
            else
            {
                part.transform.rotation = Quaternion.LookRotation(prevPart.transform.position, part.transform.position - sphere.transform.position); // for each other snake part
                if (prevPart != null && Vector3.Distance(part.transform.position, prevPart.position) > 0.1f)
                {
                    part.transform.parent.rotation = Quaternion.AngleAxis(moveSpeed * Time.deltaTime, Vector3.Cross(part.transform.parent.position - part.transform.position, -(prevPart.position - part.transform.position).normalized * 0.125f)) * part.transform.parent.rotation;
                }
            }
            returnToSurface(part); //rts in case of accidental physics
            prevPart = part.transform; //loop part reference control
        }
    }

    public void addBodyPart()
    {
        body = createNewGameObject(body, "Body " + snakeBodyPartsList.Count, null, mesh, bugMat, snakeBodyPartsList[snakeBodyPartsList.Count - 1].transform.position, transform.localScale, true, true);
        snakeBodyPartsList.Add(body);

        if (snakeBodyPartsList.Count > 3)
        {
            body.tag = "Body";
        }
    }

    public GameObject createNewGameObject(GameObject uGO, string Name, Transform Parent, Mesh Mesh, Material Material, Vector3 Position, Vector3 localScale, bool needsOrigin, bool needscollider)
    {
        uGO = new GameObject(Name); //setting up the undefinedgameobject

        if (needsOrigin)
        {
            origin = new GameObject("BodyPart Origin " + snakeBodyPartsList.Count).transform; //whos the daddy?
            origin.parent = sphere.transform;
            uGO.transform.parent = origin;
        }
        else
        {
            uGO.transform.parent = Parent;
        }

        uGO.gameObject.AddComponent<MeshFilter>().mesh = Mesh;
        uGO.AddComponent<MeshRenderer>().material = Material;
        uGO.transform.position = Position;
        uGO.transform.localScale = localScale;

        if (needscollider)
        {
            uGO.AddComponent<BoxCollider>().size = Vector3.one;
            uGO.GetComponent<BoxCollider>().isTrigger = true;
        }

        uGO.transform.forward = transform.forward;
        uGO.transform.rotation = transform.rotation;

        return uGO;
    }

    void returnToSurface(GameObject a) //rts in case of accidental physics
    {
        if (Vector3.Distance(a.transform.position, sphere.transform.position) > 1.05)
        {
            while (Vector3.Distance(a.transform.position, sphere.transform.position) >= 1.045)
            {
                a.transform.position = new Vector3(a.transform.position.x, a.transform.position.y, a.transform.position.z - 0.001f);
            }
        }
        else if (Vector3.Distance(a.transform.position, sphere.transform.position) < 1.04)
        {
            while (Vector3.Distance(a.transform.position, sphere.transform.position) <= 1.045)
            {
                a.transform.position = new Vector3(a.transform.position.x, a.transform.position.y, a.transform.position.z + 0.001f);
            }
        }
    }

    void OnTriggerEnter(Collider other) // reset game on fuck up
    {
        if (other.GetComponent<Collider>().tag == "Body")
        {
          SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex, LoadSceneMode.Single);
        }
    }

    public List<GameObject> getPartsList()
    {
        return snakeBodyPartsList;
    }
}