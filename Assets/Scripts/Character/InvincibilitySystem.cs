using Unity.Burst;
using Unity.Entities;
using Unity.Physics;

[UpdateInGroup(typeof(SimulationSystemGroup))]
partial struct InvincibilitySystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {

    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        float deltaTime = SystemAPI.Time.DeltaTime;

        foreach (var (invincibilityData, characterData, physicsCollider, entity) in SystemAPI
                     .Query<RefRW<InvincibilityData>, RefRW<CharacterData>, RefRW<PhysicsCollider>>()
                     .WithEntityAccess())
        {
            if (invincibilityData.ValueRO.Timer > 0)
            {
                invincibilityData.ValueRW.Timer -= deltaTime;
            }
            else
            {
                SystemAPI.SetComponentEnabled<InvincibilityData>(entity, false);
            }
        }
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
        
    }
}
