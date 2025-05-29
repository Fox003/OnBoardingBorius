using Unity.Entities;
using Unity.Mathematics;

public struct SpringData : IComponentData
{
    public float Frequency;
    public float DampingRatio;
    public float TargetPos;
    public float CurrentPos;
    public float Velocity;
    public tDampedSpringMotionParams motionParams;
}

public struct tDampedSpringMotionParams
{
    // newPos = posPosCoef*oldPos + posVelCoef*oldVel
    public float m_posPosCoef, m_posVelCoef;
    // newVel = velPosCoef*oldPos + velVelCoef*oldVel
    public float m_velPosCoef, m_velVelCoef;
};