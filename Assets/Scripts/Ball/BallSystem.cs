using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;

partial struct BallSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<BallData>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        BallJob job = new BallJob
        {
            DeltaTime = SystemAPI.Time.DeltaTime,
        };

        job.ScheduleParallel();
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
        
    }

    public partial struct BallJob : IJobEntity
    {
        public float DeltaTime;
        private void Execute(ref BallData ball, ref LocalTransform transform)
        {
            
        }
    }
}
