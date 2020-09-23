using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnakeMovement : MonoBehaviour
{
    public List<Transform> bodyParts = new List<Transform>();

    public Transform head;

    // Start is called before the first frame update
    void Start()
    {
        head = transform;
    }

    // Update is called once per frame
    void Update()
    {
        if (tag == "Player")
        {
            MouseRotationSnake();
            if (Input.GetMouseButtonDown(0))
            {
                speed = speedRunning;
                isRunning = true;
            }
            if (Input.GetMouseButtonUp(0))
            {
                speed = speedWalking;
                isRunning = false;
            }
        }
    }


    private Vector3 direction;
    void MouseRotationSnake()
    {

        direction = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
        direction.z = 0.0f;

        float angle = Mathf.Atan2(direction.x, direction.y) * Mathf.Rad2Deg;
        Quaternion rotation = Quaternion.AngleAxis(-angle, Vector3.forward);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, speed * Time.deltaTime);
    }

    public float speed = 3.5f;
    public float speedWalking = 3.5f, speedRunning = 7.0f;
    public float currentRotation = 0.0f;
    public float rotationSensitivity = 300.0f;

    public GameObject Eyes;
    void FixedUpdate()
    {
        if (tag == "Player")
        {
            PlayerBotBehavior();
        }
        else
        {
            DummyBotBehavior();
        }

        int nParts = head.GetComponent<SnakeMovement>().bodyParts.Count;
        gameObject.GetComponent<SpriteRenderer>().sortingOrder = nParts;
        Eyes.GetComponent<SpriteRenderer>().sortingOrder = nParts + 1;
    }

    public bool isRunning = false;
    void PlayerBotBehavior()
    {
        MoveForwardMouse();
        CameraFollow();
        SpawnOrbManager();
        
    }

    void DummyBotBehavior()
    {
        MoveForward();
        ChangeDirection();
        SpawnOrbManager();
    }


    void MoveForwardMouse()
    {
        transform.position = Vector2.MoveTowards(transform.position, Camera.main.ScreenToWorldPoint(Input.mousePosition), speed * Time.deltaTime);
    }

    void MoveForward()
    {
        float angle = Mathf.Atan2(direction.x, direction.y) * Mathf.Rad2Deg;
        Quaternion rotation = Quaternion.AngleAxis(-angle, Vector3.forward);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, speed * Time.deltaTime);

        transform.position = Vector2.MoveTowards(transform.position, randomPoint, speed * Time.deltaTime);
    }

    void Rotation()
    {
        transform.rotation = Quaternion.Euler(new Vector3(transform.rotation.x, transform.rotation.y, currentRotation));
    }

    [Range(0.0f, 1.0f)]
    public float smoothTime = 0.05f;

    void CameraFollow()
    {

        Transform camera = GameObject.FindGameObjectWithTag("MainCamera").gameObject.transform;
        Vector3 cameraVelocity = Vector3.zero;
        camera.position = Vector3.SmoothDamp(camera.position, new Vector3(gameObject.transform.position.x, gameObject.transform.position.y, -10), ref cameraVelocity, smoothTime);
    }

    public Transform bodyObject;
    void OnTriggerEnter2D(Collider2D other)
    {
  
        if (other.gameObject.transform.tag == "Body")
        {
          
            if (transform.parent.name != other.gameObject.transform.parent.name)
            {
                    for (int i = 0; i < bodyParts.Count; i++)
                    {
                        Destroy(bodyParts[i].gameObject);
                        Destroy(bodyParts[i]);
                    }
                    Destroy(this.gameObject);

            }
           
        }

        if (other.transform.tag == "Orb")
        {

            Destroy(other.gameObject);

            if (bodyParts.Count == 0)
            {
                Vector3 currentPos = transform.position;
                
                Transform newBodyPart = Instantiate(bodyObject, currentPos, Quaternion.identity) as Transform;
                newBodyPart.parent = transform.parent;
                bodyParts.Add(newBodyPart.transform);
            }
            else
            {
                Vector3 currentPos = bodyParts[bodyParts.Count - 1].position;
               
                Transform newBodyPart = Instantiate(bodyObject, currentPos, Quaternion.identity) as Transform;
                newBodyPart.parent = transform.parent;

                this.bodyParts.Add(newBodyPart.transform);
            }
        }
    }

    void OnCollisionEnter2D(Collision2D other)
    {
       
    }

    //Gerenciamento do spawn dos orbs 
    public float spawnOrbEveryXSeconds = 1;
    public GameObject orbPrefab;
    void SpawnOrbManager()
    {
        StartCoroutine("CallEveryFewSeconds", spawnOrbEveryXSeconds);
    }
    IEnumerator CallEveryFewSeconds(float x)
    {
        yield return new WaitForSeconds(x);
        StopCoroutine("CallEveryFewSeconds");
        Vector3 randomNewOrbPosition = new Vector3(
                Random.Range(
                    Random.Range(transform.position.x - 10, transform.position.x - 5),
                    Random.Range(transform.position.x + 5, transform.position.x + 10)
                ),
                Random.Range(
                    Random.Range(transform.position.y - 10, transform.position.y - 5),
                    Random.Range(transform.position.y + 5, transform.position.y + 10)
                ),
                0
            );
        GameObject newOrb = Instantiate(orbPrefab, randomNewOrbPosition, Quaternion.identity) as GameObject;
        GameObject orbParent = GameObject.Find("Orbs");
        newOrb.transform.parent = orbParent.transform;
      
    }


    //IA mais burra de todas 
    public float changeDirEveryXSeconds = 1;
    Vector3 randomPoint;
    void ChangeDirection()
    {
        StartCoroutine("UpdateDirEveryXSeconds", changeDirEveryXSeconds);
    }
    IEnumerator UpdateDirEveryXSeconds(float x)
    {
        yield return new WaitForSeconds(x);
        StopCoroutine("UpdateDirEveryXSeconds");
        randomPoint = new Vector3(
                Random.Range(
                    Random.Range(transform.position.x - 10, transform.position.x - 5),
                    Random.Range(transform.position.x + 5, transform.position.x + 10)
                ),
                Random.Range(
                    Random.Range(transform.position.y - 10, transform.position.y - 5),
                    Random.Range(transform.position.y + 5, transform.position.y + 10)
                ),
                0
            );
        direction = randomPoint - transform.position;
        direction.z = 0.0f;


    }

}
