using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

class BallAuthoring : MonoBehaviour
{
    public float speed;
}

class BallAuthoringBaker : Baker<BallAuthoring>
{
    public override void Bake(BallAuthoring authoring)
    {
        Entity entity = GetEntity(TransformUsageFlags.Dynamic);
        
        AddComponent(entity, new BallData
        {
            speed = authoring.speed,
        });
    }
}
