using Unity.Entities;
using UnityEngine;

class Character : MonoBehaviour
{
    [Header("Movement parameters")]
    public float acceleration = 2f;
    public float deceleration = 2f;
    public float maxSpeed = 2f;
    
    [Header("Dash parameters")]
    public float dashForce = 2f;
    public float dashSpeed = 2f;
    public float dashDuration = 0.5f;

    [Header("Invincibility parameters")] 
    public float DodgeInvincibilityDuration = 0.5f;
    public float HitInvincibilityDuration = 0.5f;
    
    [Header("Other")] 
    public bool movementEnabled = true;
    public float gravityForce = 2f;
}

class CharacterBaker : Baker<Character>
{
    public override void Bake(Character authoring)
    {
        var entity = GetEntity(TransformUsageFlags.Dynamic);
        
        AddComponent(entity, new CharacterData
        {
            acceleration = authoring.acceleration,
            deceleration = authoring.deceleration,
            maxSpeed = authoring.maxSpeed,
            dashForce = authoring.dashForce,
            dashSpeed = authoring.dashSpeed,
            isDashing = false,
            dashDuration = authoring.dashDuration,
            dashTimer = authoring.dashDuration,
            movementEnabled = authoring.movementEnabled,
        });
        
        AddComponent(entity, new InputsData
        {
            
        });
    }
}
