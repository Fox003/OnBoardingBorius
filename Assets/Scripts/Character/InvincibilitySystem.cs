using Unity.Burst;
using Unity.Entities;
using Unity.Physics;

[UpdateInGroup(typeof(SimulationSystemGroup))]
partial struct InvincibilitySystem : ISystem
{
    private EndInitializationEntityCommandBufferSystem.Singleton _ecbSingleton;
    
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<EndInitializationEntityCommandBufferSystem.Singleton>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        if (SystemAPI.TryGetSingleton<EndInitializationEntityCommandBufferSystem.Singleton>(out var ecbSingleton))
        {
            var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged).AsParallelWriter();
            float deltaTime = SystemAPI.Time.DeltaTime;

            foreach (var (invincibilityData, characterData, entity) in SystemAPI
                         .Query<RefRW<InvincibilityData>, RefRW<CharacterData>>()
                         .WithEntityAccess())
            {
                if (invincibilityData.ValueRO.Timer > 0)
                {
                    UnityEngine.Debug.Log("Disabled movement");
                    invincibilityData.ValueRW.Timer -= deltaTime;
                    characterData.ValueRW.movementEnabled = false;
                }
                else
                {
                    characterData.ValueRW.movementEnabled = true;
                    ecb.RemoveComponent<InvincibilityData>(0, entity);
                }
            }
        }
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
        
    }
}
