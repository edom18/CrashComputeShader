struct Particle
{
    int active;
    float scale;
    float lifetime;
    float age;
    float3 position;
    float3 basePosition;
    float3 targetPosition;
    float3 oldPosition;
    float3 velocity;
    float4 color;
};

struct ParticleData
{
    uint activateTypes;
    float scale;
    float4 targetPosition;
    float3 basePosition;
    float3 velocity;
    float4 color;
};