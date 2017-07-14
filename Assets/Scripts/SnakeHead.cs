using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class SnakeHead : MonoBehaviour
{
    public float growTime = 5;
    private float moveSpeed;
    private Transform origin, prevPart = null;
    private List<GameObject> snakeBodyPartsList;
    public Mesh mesh;
    public Material bugMat;
    private GameObject body, sphere;
    private RaycastHit hit;

    void Start()
    {
        sphere = GameObject.Find("Sphere");
        origin = new GameObject("SnakeHead Origin").transform;
        origin.parent = sphere.transform;
        transform.parent = origin;
        snakeBodyPartsList = new List<GameObject>();
        snakeBodyPartsList.Add(gameObject);
        StartCoroutine("addBodyPart");
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.Escape))
        {
            Application.Quit();
        }

        foreach (GameObject part in snakeBodyPartsList)
        {
            moveSpeed = 30 + (snakeBodyPartsList.Count * 2.5f);
            if (part == gameObject)
            {
                if (Input.GetKey("a") || Input.GetKey("d"))
                {
                    var turnRate = Input.GetKey("a") ? -120 : 120;
                    part.transform.parent.Rotate(0, 0, turnRate * Time.deltaTime);
                    moveSpeed /= 1.3f;
                }
                part.transform.rotation = Quaternion.LookRotation(transform.forward, part.transform.position - sphere.transform.position);
                part.transform.parent.rotation = Quaternion.AngleAxis(moveSpeed * Time.deltaTime, Vector3.Cross(part.transform.parent.position - part.transform.position, part.transform.forward)) * part.transform.parent.rotation;
            }
            else
            {
                part.transform.rotation = Quaternion.LookRotation(prevPart.transform.position, part.transform.position - sphere.transform.position);
                if (prevPart != null && Vector3.Distance(part.transform.position, prevPart.position) > 0.1f)
                {
                    part.transform.parent.rotation = Quaternion.AngleAxis(moveSpeed * Time.deltaTime, Vector3.Cross(part.transform.parent.position - part.transform.position, -(prevPart.position - part.transform.position).normalized * 0.125f)) * part.transform.parent.rotation;
                }
            }
            returnToSurface(part);
            prevPart = part.transform;
        }
    }

    private IEnumerator addBodyPart()
    {
        yield return new WaitForSeconds(1);
        body = createNewGameObject(body, "Body " + snakeBodyPartsList.Count, null, mesh, bugMat, snakeBodyPartsList[snakeBodyPartsList.Count - 1].transform.position, transform.localScale, true, true);
        snakeBodyPartsList.Add(body);
        if(snakeBodyPartsList.Count > 3)
        {
            body.tag = "Body";
        }
        yield return new WaitForSeconds(Mathf.Abs(growTime - 1));
        StartCoroutine("addBodyPart");
    }

    public GameObject createNewGameObject(GameObject uGO, string Name, Transform Parent, Mesh Mesh, Material Material, Vector3 Position, Vector3 localScale, bool needsOrigin, bool needscollider)
    {
        uGO = new GameObject(Name);
        if (needsOrigin)
        {
            origin = new GameObject("BodyPart Origin " + snakeBodyPartsList.Count).transform;
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

    void returnToSurface(GameObject a)
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

    void OnTriggerEnter(Collider other)
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