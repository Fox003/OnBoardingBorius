using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine.InputSystem;
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
        foreach (var (inputData, playerInput) in SystemAPI.Query<RefRW<InputsData>, RefRO<PlayerInputComponent>>())
        {
            int deviceID = playerInput.ValueRO.DeviceID;

            if (!PlayerInputRegistry.Wrappers.TryGetValue(deviceID, out var wrapper))
                continue;

            var actions = wrapper.Actions.Player;

            inputData.ValueRW.Move  = actions.Move.ReadValue<Vector2>();
            inputData.ValueRW.Jump  = actions.Jump.ReadValue<float>() > 0.5f;
            inputData.ValueRW.Dash  = actions.Attack.ReadValue<float>() > 0.5f;
            inputData.ValueRW.Dodge = actions.Interact.ReadValue<float>() > 0.5f;
        }
    }
}
