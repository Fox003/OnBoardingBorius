using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Vector2 = UnityEngine.Vector2;

public partial class InputsSystem : SystemBase
{
    private InputSystem_Actions _inputs;
    
    protected override void OnCreate()
    {
        _inputs = new InputSystem_Actions();
        _inputs.Enable();
    }

    
    protected override void  OnUpdate()
    {
        foreach (RefRW<InputsData> data in SystemAPI.Query<RefRW<InputsData>>())
        {
            data.ValueRW.Move = _inputs.Player.Move.ReadValue<Vector2>();
            data.ValueRW.Jump = _inputs.Player.Jump.ReadValue<float>() > 0.5f;
            data.ValueRW.Dash = _inputs.Player.Attack.ReadValue<float>() > 0.5f;
            data.ValueRW.Dodge = _inputs.Player.Interact.ReadValue<float>() > 0.5f;
        }
    }
}
