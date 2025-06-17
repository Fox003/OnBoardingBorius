using Unity.Entities;

public struct CharacterData : IComponentData
{
    public float acceleration;
    public float deceleration;
    public float maxSpeed;
    public float dashSpeed;
    public bool isInKnockback;
    public float knockbackDamping;
    public float dashForce;
    public float dashDuration;
    public bool isDashing;
    public float desiredSpeed;
    public float dashTimer;
    public bool movementEnabled;
}
