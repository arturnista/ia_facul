using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLogic : MonoBehaviour
{
    public List<GameObject> snakes = new List<GameObject>();

    // Start is called before the first frame update
    public GameObject snakePrefab;
    void Start()
    {
        // Cria 5 SnakeBots em posições aleatórias
        for (int i = 0; i < 5; i++)
        {
            //Vector3 randomPosition = new Vector3(Random.Range(-50.0f, 50.0f), Random.Range(-50.0f, 50.0f), 0.0f);

            Vector3 randomPosition = new Vector3(
                Random.Range(
                    Random.Range(- 50, - 10),
                    Random.Range(10, 50)
                ),
                Random.Range(
                    Random.Range(- 50, - 10),
                    Random.Range(10, 50)
                ),
                0
            );



            GameObject newSnake = Instantiate(snakePrefab,randomPosition, Quaternion.identity) as GameObject;
            newSnake.name = "SnakeBot" + i.ToString();
            snakes.Add(newSnake);;
        }


    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
