using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine.InputSystem;

public struct InputsData : IComponentData
{
    public float2 Move;
    public bool Jump;
    public bool Dash;
    public bool Dodge;
}

public struct PlayerInputComponent : IComponentData
{
    public int DeviceID;
}

public class PlayerInputWrapper
{
    public InputSystem_Actions Actions;
    public int DeviceID;

    public PlayerInputWrapper(InputDevice device)
    {
        Actions = new InputSystem_Actions();
        Actions.devices = new[] { device }; // Bind uniquement à cette manette
        Actions.Enable();
        DeviceID = device.deviceId;
    }
}

public static class PlayerInputRegistry
{
    public static Dictionary<int, PlayerInputWrapper> Wrappers = new();
}