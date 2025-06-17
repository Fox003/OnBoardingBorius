using System;
using System.Runtime.CompilerServices;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Extensions;
using Unity.Physics.Systems;
using Unity.Transforms;
using UnityEngine;
using RaycastHit = Unity.Physics.RaycastHit;

[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
[UpdateAfter(typeof(PhysicsSystemGroup))]
partial struct CharacterMovementSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<CharacterData>();
        state.RequireForUpdate<PhysicsWorldSingleton>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        float deltaTime = SystemAPI.Time.DeltaTime;
        
        foreach (var (characterData, inputs, physicsVelocity, physicsMass, transform) in SystemAPI
                     .Query<RefRW<CharacterData>, RefRO<InputsData>, RefRW<PhysicsVelocity>, RefRO<PhysicsMass>, RefRW<LocalTransform>>())
        {
            // Si on a pas le droit de bouger, on bouge pas
            if (!characterData.ValueRO.movementEnabled)
                continue;
            
            var physicsWorld = SystemAPI.GetSingleton<PhysicsWorldSingleton>();
            var mass = physicsMass.ValueRO;
            
            var vel = physicsVelocity.ValueRW;
            
            characterData.ValueRW.desiredSpeed = characterData.ValueRO.isDashing
                ? characterData.ValueRO.dashSpeed
                : characterData.ValueRO.maxSpeed;
            
            // Verifier si la balle bouge
            bool isMoving = math.lengthsq(inputs.ValueRO.Move) > 0.0001f;
            bool isGrounded = GroundCheck(ref physicsWorld, transform.ValueRO);
            
            // Pousser la balle dans la direction des inputs
            vel.Linear += new float3(inputs.ValueRO.Move.x, 0, inputs.ValueRO.Move.y) * characterData.ValueRO.acceleration * deltaTime;
            
            // Controle de la vitesse
            float3 flatVel = new float3(vel.Linear.x, 0, vel.Linear.z);
            float flatSpeed = math.length(flatVel);

            if (flatSpeed > characterData.ValueRO.desiredSpeed)
            {
                // DAMPING PROGRESSIF pendant le knockback
                float3 slowDir = -math.normalize(flatVel);
                float excessSpeed = flatSpeed - characterData.ValueRO.desiredSpeed;

                float damping = characterData.ValueRO.knockbackDamping; // ex: 5.0f
                float3 brake = slowDir * excessSpeed * damping * deltaTime;

                vel.Linear += new float3(brake.x, 0, brake.z);
            }
            
            // Friction
            if (math.length(flatVel) > 0.001f)
            {
                var frictionVel = new float3(vel.Linear.x, 0, vel.Linear.z) * characterData.ValueRO.deceleration;
                vel.Linear -= new float3(frictionVel.x, vel.Linear.y, frictionVel.z) * deltaTime;
            }
            
            
            // Dash
            if (inputs.ValueRO.Dash && characterData.ValueRW.isDashing == false)
            {
                // Si on veut aller dans une direction
                if (math.length(inputs.ValueRO.Move) > 0)
                {
                    characterData.ValueRW.desiredSpeed = characterData.ValueRO.dashSpeed;
                    characterData.ValueRW.isDashing = true;
                    var dashForce = new float3(inputs.ValueRO.Move.x, 0, inputs.ValueRO.Move.y) * characterData.ValueRO.dashForce;
                    vel.ApplyLinearImpulse(mass, dashForce);
                }
            }
            
            // DashTimer
            if (characterData.ValueRW.isDashing)
            {
                characterData.ValueRW.dashTimer -= deltaTime;
                if (characterData.ValueRW.dashTimer <= 0)
                {
                    characterData.ValueRW.isDashing = false;
                    characterData.ValueRW.dashTimer = characterData.ValueRW.dashDuration;
                }
            }
            
            // Appliquer la vélocité résultante après tous les calculs
            physicsVelocity.ValueRW = vel;
        }
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
        
    }
    
    private static bool Raycast(PhysicsWorldSingleton physicsWorld, float3 start, float3 end, out RaycastHit hit)
    {
        var input = new RaycastInput
        {
            Start = start,
            End = end,
            Filter = CollisionFilter.Default // You can customize this
        };

        return physicsWorld.CastRay(input, out hit);
    }

    private static bool GroundCheck(ref PhysicsWorldSingleton physicsWorld, LocalTransform transform)
    {
        float3 position = transform.Position;     // Center of the player
        float height = transform.Scale;           // Assuming scale represents height

        float3 direction = math.down();           // or new float3(0, -1, 0)
        float distance = 1;           // From center to bottom

        float3 start = position;
        float3 end = start + (direction * distance);
        
        return Raycast(physicsWorld, start, end, out var hit);
    }
}