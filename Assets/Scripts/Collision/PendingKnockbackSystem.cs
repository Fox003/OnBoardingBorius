using Unity.Burst;
using Unity.Entities;
using Unity.Physics;
using Unity.Mathematics;
using Unity.Physics.Extensions;
using UnityEngine;

partial struct PendingKnockbackSystem : ISystem
{
    
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {

    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
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
                pendingKnockback.ValueRW.Timer -= deltaTime;
            }
            else
            {
                float3 knockback = pendingKnockback.ValueRO.KnockbackDirection * pendingKnockback.ValueRO.KnockbackForce;
                velocity.ApplyLinearImpulse(mass, knockback);
                physicsVelocity.ValueRW = velocity;
                
                SystemAPI.SetComponentEnabled<PendingKnockbackData>(entity, false);
                movement.ValueRW.isInKnockback = true;
                Debug.Log("HitStop over, applying knockback");
            }
        }
}

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
        
    }
}
