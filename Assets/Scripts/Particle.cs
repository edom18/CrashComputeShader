using UnityEngine;

namespace CrashSample
{
    public struct Particle
    {
        public int active;
        public float scale;
        public float lifetime;
        public float age;
        public Vector3 position;
        public Vector3 basePosition;
        public Vector3 targetPosition;
        public Vector3 oldPosition;
        public Vector3 velocity;
        public Vector4 color;
    }
    
    public struct ParticleData
    {
        public uint activateTypes;
        public float scale;
        public Vector4 targetPosition;
        public Vector3 basePosition;
        public Vector3 velocity;
        public Color color;
    }
}