using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum Algorithms
{
    BruteForce,
    SortByX
}

/// <summary>
/// Compares two vectors on their x axis value
/// </summary>
public class XComparer : IComparer<Vector3>
{
    public int Compare(Vector3 a, Vector3 b) {
        return a.x.CompareTo(b.x);
    }
}

public class Spawner : MonoBehaviour
{
    public static Spawner Instance;

    public Algorithms UseAlgorithm = Algorithms.BruteForce;
    
    // stores width and hight of the spawn area
    public Vector2 Area;

    public GameObject EnemyPrefab;
    public GameObject BulletPrefab;
    
    public int NumEnemies;
    public int NumBullets;

    public Vector3[] EnemyPositions;

    private void Awake() {
        Instance = this; 
    }

    void Start()
    {
        EnemyPositions = new Vector3[NumBullets];
        
        // instantiate enemies at random locations within the spawn area
        for (int i = 0; i < NumEnemies; i++) {
            var inst = Instantiate(EnemyPrefab, new Vector3(Random.Range(0, Area.x), Random.Range(0, Area.y)), Quaternion.identity);
            EnemyPositions[i] = inst.transform.position;
        }

        // instantiate bullets at random locations within the spawn area
        // give each bullet a random direction and speed
        for (int i = 0; i < NumBullets; i++) {
            var inst = Instantiate(BulletPrefab, new Vector3(Random.Range(0, Area.x), Random.Range(0, Area.y)), Quaternion.identity);
            var comp = inst.GetComponent<MoveBullet>();
            comp.Speed = Random.Range(10f, 30f);
            comp.Direction = Random.insideUnitCircle.normalized;
        }
    }
    

    void Update() {
        // IMPORTANT:  this has to run before the MoveBullet.Update 
        // easiest way to achieve that is to use Project Settings > Script Execution Order
        // The reason it has to run before MoveBullet.Update is because Move.Bullet searches
        // for the closest Enemy using a binary search, which in turn requires a sorted array
        EnemyPositions = EnemyPositions.OrderBy(v => v.x).ToArray();
    }

}
