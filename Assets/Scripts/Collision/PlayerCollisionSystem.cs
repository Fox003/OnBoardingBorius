using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Extensions;
using Unity.Physics.Systems;
using UnityEngine;

[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
[UpdateAfter(typeof(PhysicsSystemGroup))]
public partial class PlayerCollisionSystem : SystemBase
{
    private EntityCommandBufferSystem _ecbSystem;
    
    protected override void OnCreate()
    {
        _ecbSystem = World.GetOrCreateSystemManaged<EndSimulationEntityCommandBufferSystem>();
    }
    
    
    protected override void OnUpdate()
    {
        var collisionWorld = SystemAPI.GetSingleton<SimulationSingleton>();
        var ecb = _ecbSystem.CreateCommandBuffer().AsParallelWriter();

        var job = new DashCollisionJob
        {
            CharacterDataLookup = SystemAPI.GetComponentLookup<CharacterData>(false),
            PhysicsVelocityLookup = SystemAPI.GetComponentLookup<PhysicsVelocity>(false),
            InvincibilityDataLookup = SystemAPI.GetComponentLookup<InvincibilityData>(false),
            PhysicsMassLookup = SystemAPI.GetComponentLookup<PhysicsMass>(true),
            CommandBuffer = ecb
        };

        Dependency = job.Schedule(collisionWorld, Dependency);
        _ecbSystem.AddJobHandleForProducer(Dependency);
    }

    public struct DashCollisionJob : ICollisionEventsJob
    {
        
        public ComponentLookup<CharacterData> CharacterDataLookup;
        public ComponentLookup<PhysicsVelocity> PhysicsVelocityLookup;
        public ComponentLookup<InvincibilityData> InvincibilityDataLookup;
        public EntityCommandBuffer.ParallelWriter CommandBuffer;
        [ReadOnly] public ComponentLookup<PhysicsMass> PhysicsMassLookup;

        public void Execute(CollisionEvent collisionEvent)
        {
            Entity a = collisionEvent.EntityA;
            Entity b = collisionEvent.EntityB;
            
            // Only process once per pair
            if (a.Index > b.Index)
            {
                Debug.Log("index of collider a was greater than b");
                return;
            }

            bool aHasData = CharacterDataLookup.HasComponent(a);
            bool bHasData = CharacterDataLookup.HasComponent(b);
            
            // Si l'entite a ou b n'a pas une des composantes qu'on doit access, return
            if (!aHasData || !bHasData) return;

            var aData = CharacterDataLookup[a];
            var bData = CharacterDataLookup[b];
            
            var aIsInvincible = InvincibilityDataLookup.HasComponent(a);
            var bIsInvincible = InvincibilityDataLookup.HasComponent(b);
        
            // Si une des deux entite est invincible, return
            if (aIsInvincible || bIsInvincible)
            {
                Debug.Log("Someone was invincible, no knockback");
                return;
            }
            
            bool aIsDashing = aData.isDashing;
            bool bIsDashing = bData.isDashing;
            
            if (aIsDashing && !bIsDashing)
            {
                KillMomentum(a, b);
                AddPendingKnockback(a, b, collisionEvent, CommandBuffer);
                
                Debug.Log("B got rocked");
            }
            else if (bIsDashing && !aIsDashing)
            {
                KillMomentum(a, b);
                AddPendingKnockback(b, a, collisionEvent, CommandBuffer);
                
                Debug.Log("A got rocked");
            }
            else if (bIsDashing && aIsDashing)
            {
                // Kill momentum
                KillMomentum(a, b);
                Debug.Log("A and B where both dashing");
            }
        }

        void AddInvincibility(Entity entity, EntityCommandBuffer.ParallelWriter ecb)
        {
            ecb.AddComponent(0, entity, new InvincibilityData
            {
                Timer = 0.2f,
            });
        }

        void AddPendingKnockback(Entity dasher, Entity target, CollisionEvent collisionEvent, EntityCommandBuffer.ParallelWriter ecb)
        {
            // Cache the pending knockback by adding a pending knockback component to the target and dasher
            // One will get the knockback force and the other the pushback
            if (!PhysicsVelocityLookup.HasComponent(target) || !PhysicsMassLookup.HasComponent(target)) return;
            
            var knockbackDir = math.normalizesafe(collisionEvent.Normal);

            float knockbackForce = 80f; // tweak as needed
            float pushBackForce = -15f;
            float3 targetKnockback = knockbackDir * knockbackForce;
            float3 dasherKnockback = knockbackDir * pushBackForce;
            
            ecb.AddComponent(0, target, new PendingKnockbackData
            {
                KnockbackDirection = knockbackDir,
                KnockbackForce = knockbackForce,
                Timer = 3f,
            });
            
            ecb.AddComponent(0, dasher, new PendingKnockbackData
            {
                KnockbackDirection = knockbackDir,
                KnockbackForce = pushBackForce,
                Timer = 3f,
            });
        }

        void KillMomentum(Entity a, Entity b)
        {
            var aVel = PhysicsVelocityLookup[a];
            var bVel = PhysicsVelocityLookup[b];
            
            aVel.Linear = new float3(0, 0, 0);
            bVel.Linear = new float3(0, 0, 0);
            
            PhysicsVelocityLookup[a] = aVel;
            PhysicsVelocityLookup[b] = bVel;
        }
     }
}