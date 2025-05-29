using Unity.Entities;
using Unity.Mathematics;

public struct InputsData : IComponentData
{
    public float2 Move;
    public bool Jump;
    public bool Dash;
    public bool Dodge;
}
