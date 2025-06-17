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
partial struct PlayerCollisionSystem : ISystem
{
    
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<SimulationSingleton>();
    }
    
    
    public void OnUpdate(ref SystemState state)
    {
        var collisionWorld = SystemAPI.GetSingleton<SimulationSingleton>();

        state.Dependency = new DashCollisionJob
        {
            CharacterDataLookup = SystemAPI.GetComponentLookup<CharacterData>(false),
            PhysicsVelocityLookup = SystemAPI.GetComponentLookup<PhysicsVelocity>(false),
            InvincibilityDataLookup = SystemAPI.GetComponentLookup<InvincibilityData>(false),
            PendingKnockbackDataLookup = SystemAPI.GetComponentLookup<PendingKnockbackData>(false),
            PhysicsMassLookup = SystemAPI.GetComponentLookup<PhysicsMass>(true),
        }.Schedule(collisionWorld, state.Dependency);
    }

    public struct DashCollisionJob : ICollisionEventsJob
    {
        
        public ComponentLookup<CharacterData> CharacterDataLookup;
        public ComponentLookup<PhysicsVelocity> PhysicsVelocityLookup;
        public ComponentLookup<InvincibilityData> InvincibilityDataLookup;
        public ComponentLookup<PendingKnockbackData> PendingKnockbackDataLookup;
        [ReadOnly] public ComponentLookup<PhysicsMass> PhysicsMassLookup;

        public void Execute(CollisionEvent collisionEvent)
        {
            Entity a = collisionEvent.EntityA;
            Entity b = collisionEvent.EntityB;
            
            // Only process once per pair
            if (a.Index > b.Index)
            {
                return;
            }
            
            bool aHasVelocityComponent = PhysicsVelocityLookup.HasComponent(a);
            bool bHasVelocityComponent = PhysicsVelocityLookup.HasComponent(b);
            
            bool aHasMassComponent = PhysicsMassLookup.HasComponent(a);
            bool bHasMassComponent = PhysicsMassLookup.HasComponent(b);
            
            bool aHasCharacterComponent = CharacterDataLookup.HasComponent(a);
            bool bHasCharacterComponent = CharacterDataLookup.HasComponent(b);
            
            // Si l'entite a ou b n'a pas une des composantes qu'on doit access, return
            if (!aHasVelocityComponent || !bHasVelocityComponent ||
                !aHasMassComponent || !bHasMassComponent ||
                !aHasCharacterComponent || !bHasCharacterComponent)
            {
                return;
            }

            var aData = CharacterDataLookup[a];
            var bData = CharacterDataLookup[b];
            
            var aIsInvincible = InvincibilityDataLookup.IsComponentEnabled(a);
            var bIsInvincible = InvincibilityDataLookup.IsComponentEnabled(b);
        
            // Si une des deux entite est invincible, return
            if (aIsInvincible || bIsInvincible)
            {
                Debug.Log("Someone was invincible, no knockback");
                return;
            }
            
            bool aIsDashing = aData.isDashing;
            bool bIsDashing = bData.isDashing;

            float hitLagDuration = 0.2f;
            if (aIsDashing && !bIsDashing)
            {
                KillMomentum(a, b);
                AddPendingKnockback(a, b, collisionEvent, hitLagDuration, PendingKnockbackDataLookup);
                EnableInvincibility(b, 0.6f, InvincibilityDataLookup);
                
                Debug.Log("B got rocked");
            }
            else if (bIsDashing && !aIsDashing)
            {
                KillMomentum(a, b);
                AddPendingKnockback(b, a, collisionEvent, hitLagDuration, PendingKnockbackDataLookup);
                EnableInvincibility(a, 0.6f, InvincibilityDataLookup);
                
                Debug.Log("A got rocked");
            }
            else if (bIsDashing && aIsDashing)
            {
                // Kill momentum
                KillMomentum(a, b);
                Debug.Log("A and B where both dashing");
            }
        }

        void EnableInvincibility(Entity entity, float invincivilityDuration, ComponentLookup<InvincibilityData> invincibilityDataLookup)
        {
            invincibilityDataLookup[entity] = new InvincibilityData
            {
                Timer = invincivilityDuration
            };
            
            invincibilityDataLookup.SetComponentEnabled(entity, true);
        }

        void AddPendingKnockback(Entity dasher, Entity target, CollisionEvent collisionEvent, float HitLagDuration, ComponentLookup<PendingKnockbackData> pendingKnockbackLookup)
        {
            // Cache the pending knockback by adding a pending knockback component to the target and dasher
            // One will get the knockback force and the other the pushback
            if (!PhysicsVelocityLookup.HasComponent(target) || !PhysicsMassLookup.HasComponent(target)) return;
            
            var normal = collisionEvent.Normal;
            
            if (collisionEvent.EntityA == dasher)
            {
                // Normal goes from B -> A, so we flip it to go from A -> B
                normal = -math.normalizesafe(normal);
            }
            else if (collisionEvent.EntityB == dasher)
            {
                // Normal is already in the right direction
                normal = math.normalizesafe(normal);
            }

            float knockbackForce = 6f; // tweak as needed
            float pushBackForce = 2.5f;
            float3 targetKnockback = normal * knockbackForce;
            float3 dasherKnockback = -normal * pushBackForce;
            
            pendingKnockbackLookup[dasher] = new PendingKnockbackData
            {
                KnockbackDirection = dasherKnockback,
                KnockbackForce = pushBackForce,
                Timer = HitLagDuration
            };
            
            pendingKnockbackLookup[target] = new PendingKnockbackData
            {
                KnockbackDirection = targetKnockback,
                KnockbackForce = knockbackForce,
                Timer = HitLagDuration
            };
            
            pendingKnockbackLookup.SetComponentEnabled(target, true);
            pendingKnockbackLookup.SetComponentEnabled(dasher, true);
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