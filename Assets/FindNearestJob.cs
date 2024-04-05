using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

public partial struct FindNearestJob : IJobParallelFor
{
    [ReadOnly] public NativeArray<float3> EnemyPositions;
    [ReadOnly] public NativeArray<float3> BulletPositions;
    public NativeArray<float3> NearestEnemyPositions;

    [BurstCompile]
    public void Execute(int index) {
        float3 bulletPosition = BulletPositions[index];
        float minDistance = float.MaxValue;
        for (int i = 0; i < EnemyPositions.Length; i++) {
            var distance = math.distancesq(bulletPosition, EnemyPositions[i]);
            if (distance < minDistance) {
                minDistance = distance;
                NearestEnemyPositions[index] = EnemyPositions[i];
            }
        }
    }
}
