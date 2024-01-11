using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;




public class SpawnManager : MonoBehaviour
{

    [Header("Player")]
    public Transform playerspawn;
    private bool hasSpawned;
    [SerializeField] private GameObject player;//player prefab

    [Header("Enemy 1")]
    public GameObject prefab1;//first shape prefab
    public Transform enemyspawn;
    private Rigidbody2D rb;

    [Header("Enemy 2")]
    public GameObject prefab2;//first shape prefab
    public Transform enemyspawn2;
    private Rigidbody2D rb2;

    public int Count { get; set; } = 0;


    //[Header("Button Objects")]
   // public Button playerbutton;
    //public Button enemy1button;
    
    // Start is called before the first frame update
    void Start()
    {
        hasSpawned = false;
        //set's the player spawn zone
        playerspawn = this.gameObject.transform.GetChild(0);
        //enemyspawn = this.gameObject.transform.GetChild(1);
        rb = prefab1.GetComponent<Rigidbody2D>();
        rb.velocity = new Vector2(-10.0f, 0);

        rb2 = prefab2.GetComponent<Rigidbody2D>();
        rb2.velocity = new Vector2(0, 10.0f);
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 velocity = new Vector3(UnityEngine.Random.Range(-250f, 250f) * 11.5f, UnityEngine.Random.Range(-10f, 10f) * 12.5f, 0f);

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SpawnPlayer();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            InvokeRepeating("SpawnShape1", 0f, 1f);
           
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3)) 
        {
            InvokeRepeating("SpawnShape2", 0f, 1f);
        }
       
    }

    public void SpawnPlayer()
    {
      
       Instantiate(player, new Vector2(playerspawn.transform.position.x, playerspawn.transform.position.y), Quaternion.identity);
       hasSpawned = true;
      
    }
    
    public void SpawnShape1()
    {
        for (int i = 0; i < 15; i++)
        {
            //where the shape will spawn
            Vector3 position = new Vector3(enemyspawn.transform.position.x + UnityEngine.Random.Range(-10f, 10f), enemyspawn.transform.position.y + UnityEngine.Random.Range(-10f, 10f), 0f);
            Vector3 velocity = Vector3.zero;
            velocity =  Vector3.MoveTowards(velocity, playerspawn.transform.position,  15 * Time.deltaTime);
            Instantiate(prefab1, position, Quaternion.identity);
            //prefab1.gameObject.GetComponent<FirstShape>().SetVelocity(velocity * Time.deltaTime);
            prefab1.transform.Translate(velocity * Time.deltaTime, Space.World);
            Count = Count + 1;
        }

    }

    public void SpawnShape2()
    {
        for (int j = 0; j < 15; j++)
        {
            //where the shape will spawn
            Vector3 position2 = new Vector3(enemyspawn2.transform.position.x + UnityEngine.Random.Range(-10f, 10f), enemyspawn2.transform.position.y + UnityEngine.Random.Range(-10f, 10f), 0f);
            Vector3 velocity2 = new Vector3(UnityEngine.Random.Range(-250f, 250f) * 11.5f, UnityEngine.Random.Range(-10f, 10f) * 12.5f, 0f);
            Instantiate(prefab2, position2, Quaternion.identity);
            prefab2.gameObject.GetComponent<FirstShape>().SetVelocity(velocity2 * Time.deltaTime);
            Count = Count + 1;
        }
    }
}

