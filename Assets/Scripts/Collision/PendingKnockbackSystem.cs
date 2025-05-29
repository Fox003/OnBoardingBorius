using Unity.Burst;
using Unity.Entities;
using Unity.Physics;
using Unity.Mathematics;
using Unity.Physics.Extensions;
using UnityEngine;

partial struct PendingKnockbackSystem : ISystem
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
            // ... your code
        
            float deltaTime = SystemAPI.Time.DeltaTime;

            foreach (var (pendingKnockback, 
                         physicsVelocity, 
                         physicsMass, 
                         movement, entity) in SystemAPI.Query<
                         RefRW<PendingKnockbackData>, 
                         RefRW<PhysicsVelocity>, RefRO<PhysicsMass>, 
                         RefRW<CharacterData>>().WithEntityAccess())
            {
                var velocity = physicsVelocity.ValueRW;
                var mass = physicsMass.ValueRO;

                if (pendingKnockback.ValueRO.Timer > 0)
                {
                    if (!SystemAPI.HasComponent<InvincibilityData>(entity))
                    {
                        ecb.AddComponent(0, entity, new InvincibilityData
                        {
                            Timer = pendingKnockback.ValueRO.Timer,
                        });
                    }
                    
                    pendingKnockback.ValueRW.Timer -= deltaTime;
                }
                else
                {
                    float3 knockback = pendingKnockback.ValueRO.KnockbackDirection * pendingKnockback.ValueRO.KnockbackForce;
                    velocity.ApplyLinearImpulse(mass, knockback);
                    physicsVelocity.ValueRW = velocity;
                    
                    ecb.RemoveComponent<PendingKnockbackData>(0, entity);
                    Debug.Log("HitStop over, applying knockback");
                }
            }
        }
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
        
    }
}
