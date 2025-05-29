using Unity.Burst;
using Unity.Entities;
using UnityEngine;

public partial class AbilitySystem : SystemBase
{
    [BurstCompile]
    protected override void OnCreate()
    {
        
    }

    [BurstCompile]
    protected override void OnUpdate()
    {
        foreach (var (abilityData, invincibilityData, inputs, entity) in SystemAPI.Query<RefRW<AbilityData>, RefRW<InvincibilityData>, RefRO<InputsData>>().WithEntityAccess())
        {
            if (inputs.ValueRO.Dodge && SystemAPI.HasComponent<InvincibilityData>(entity))
            {
                // Add invincibility component
                Debug.Log("Dodging...");
            }
        }
    }
}
