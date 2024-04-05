using System;
using UnityEngine;

public class MoveBullet : MonoBehaviour
{
    public Vector2 Direction;
    public float Speed;

    // for convenience. to ensure flip the direction of bullets that leave the spawn area
    Vector2 absDirection;

    private void Start() {
        absDirection = new Vector2(Mathf.Abs(Direction.x), Mathf.Abs(Direction.y));
    }

    // 
    void Search(Vector3 bulletPosition, int startIdx, int endIdx, int step, ref Vector3 nearestTarget, ref float minDistance) {
        
        for (int i = startIdx; i != endIdx; i += step) {
            
            Vector3 targetPosition = Spawner.Instance.EnemyPositions[i];
            float xDistance = Mathf.Abs(bulletPosition.x - targetPosition.x);

            if (xDistance >= minDistance) break;
            
            float distance = Vector3.Distance(targetPosition, bulletPosition);
            if (distance < minDistance) {
                minDistance = distance;
                nearestTarget = targetPosition;
            }
        }
    }

    void Update()
    {
        transform.position += Speed * Time.deltaTime * (Vector3)Direction;

        var area = Spawner.Instance.Area;
        var bulletPosition = transform.position;
        if (bulletPosition.x < 0) { Direction.x = absDirection.x; }
        if (bulletPosition.x > area.x) { Direction.x = -absDirection.x; }
        if (bulletPosition.y < 0) { Direction.y = absDirection.y; }
        if (bulletPosition.y > area.y) { Direction.y = -absDirection.y; }

        if (Spawner.Instance.UseAlgorithm == Algorithms.SortByX) {
            var enemyPositions = Spawner.Instance.EnemyPositions;

            //STEP 1: SEARCH FOR NEAREST ENEMY ON THE X AXIS
            int startIdx = Array.BinarySearch(enemyPositions, bulletPosition, new XComparer());
            // binary search will return the bitwise complement of the nearest element's index
            if (startIdx < 0) startIdx = ~startIdx;
            // handle the case when searched position is greater than all elements
            if (startIdx >= enemyPositions.Length) startIdx = enemyPositions.Length - 1;


            // STEP 2: STARTING FROM THE NEAREST ENEMY ON THE X AXIS
            // LOOP THROUGH ENEMY POSITIONS TO FIND ONE WHOSE X AXIS DISTANCE
            // IS GREATER THAN THE SHORTEST DISTANCE FOUND
            Vector3 closestEnemyPosition = enemyPositions[startIdx];
            float minDistance = Vector3.Distance(bulletPosition, closestEnemyPosition);
            Search(bulletPosition, startIdx + 1, enemyPositions.Length, +1, ref closestEnemyPosition, ref minDistance);
            Search(bulletPosition, startIdx - 1, -1, -1, ref closestEnemyPosition, ref minDistance);
            Debug.DrawLine(bulletPosition, closestEnemyPosition);
        }
        else if (Spawner.Instance.UseAlgorithm == Algorithms.BruteForce) {
            var enemyPositions = Spawner.Instance.EnemyPositions;
            float minDistance = float.MaxValue;
            Vector3 closestEnemyPosition = default;

            // loop through all enemies to find the closest
            for (int i = 0; i < Spawner.Instance.NumEnemies; i++) {
                var distance = Vector3.Distance(bulletPosition, enemyPositions[i]);
                if (distance < minDistance) {
                    minDistance = distance;
                    closestEnemyPosition = enemyPositions[i];
                }
            }
            Debug.DrawLine(bulletPosition, closestEnemyPosition);
        }
    }
}
