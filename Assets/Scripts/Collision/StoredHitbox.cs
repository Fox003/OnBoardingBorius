using Unity.Entities;

public struct StoredHitbox : IComponentData
{
    public BlobAssetReference<Unity.Physics.Collider> ColliderRef;
}
