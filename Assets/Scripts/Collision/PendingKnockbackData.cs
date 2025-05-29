using Unity.Entities;
using Unity.Mathematics;

public struct PendingKnockbackData : IComponentData
{
    public float KnockbackForce;
    public float3 KnockbackDirection;
    public float Timer;
}
