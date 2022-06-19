using System.Collections;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Rendering;
using Random = UnityEngine.Random;

namespace CrashSample
{
    public class ParticleSystem : MonoBehaviour
    {
        [SerializeField] private ComputeShader _shader;
        [SerializeField] private int _count = 50_000;
        [SerializeField] private Mesh _particleMesh;
        [SerializeField] private Material _material;
        [SerializeField] private Color _particleColor = Color.cyan;
        [SerializeField] private float _radius = 0.1f;
        [SerializeField] private Vector2 _lifetime = new(0.5f, 1.0f);
        [SerializeField] private float _particleScale = 0.05f;

        private Vector4 ParticleColorData => new Vector4(_particleColor.r, _particleColor.g, _particleColor.b, _particleColor.a);

        private GraphicsBuffer _particleBuffer;
        private GraphicsBuffer _particlePoolBuffer;
        private GraphicsBuffer _argsBuffer;
        private GraphicsBuffer _particleDataBuffer;
        private GraphicsBuffer _particleCountBuffer;
        private ParticleData[] _particleDataStore;
        private int[] _particleCountArgs = new int[1];

        private int _initializeKernelId = -1;
        private int _updateKernelId = -1;
        private int _emitKernelId = -1;
        private bool _running = false;

        private void Awake()
        {
            _shader = Instantiate(_shader);
            _material = Instantiate(_material);

            InitializeIds();
            InitializeBuffers();
            InitializeParticles();

            Play();

            StartCoroutine(AutoEmit());
        }

        private void Update()
        {
            if (!_running) return;

            UpdateParticles();
            DrawParticles();
        }

        private void OnDestroy()
        {
            _particleBuffer?.Release();
            _particleDataBuffer?.Release();
            _particlePoolBuffer?.Release();
            _argsBuffer?.Release();
            _particleCountBuffer?.Release();
            Destroy(_shader);
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, _radius);
        }

        private IEnumerator AutoEmit()
        {
            while (true)
            {
                yield return new WaitForSeconds(0.5f);

                if (!Emit(80))
                {
                    yield break;
                }
            }
        }

        private void InitializeIds()
        {
            _initializeKernelId = _shader.FindKernel("Initialize");
            _emitKernelId = _shader.FindKernel("Emit");
            _updateKernelId = _shader.FindKernel("Update");
        }

        private void InitializeBuffers()
        {
            _particleBuffer = new GraphicsBuffer(GraphicsBuffer.Target.Structured, _count, Marshal.SizeOf<Particle>());
            _particleDataBuffer = new GraphicsBuffer(GraphicsBuffer.Target.Structured, _count, Marshal.SizeOf<ParticleData>());
            _particleDataStore = new ParticleData[_count];

            _particlePoolBuffer = new GraphicsBuffer(GraphicsBuffer.Target.Append, _count, sizeof(uint));
            _particlePoolBuffer.SetCounterValue(0);

            _particleCountBuffer = new GraphicsBuffer(GraphicsBuffer.Target.IndirectArguments, 1, sizeof(int));
            _particleCountBuffer.SetData(_particleCountArgs);

            _particleDataBuffer.SetData(_particleDataStore);

            uint[] args = new uint[]
            {
                _particleMesh.GetIndexCount(0),
                (uint)_count,
                _particleMesh.GetIndexStart(0),
                _particleMesh.GetBaseVertex(0),
                0,
            };

            _argsBuffer = new GraphicsBuffer(GraphicsBuffer.Target.IndirectArguments, 1, sizeof(uint) * args.Length);
            _argsBuffer.SetData(args);

            uint[] pool = new uint[_count];
            _particlePoolBuffer.SetData(pool);
        }

        private void InitializeParticles()
        {
            Particle[] particles = new Particle[_count];

            float lifetime = Random.Range(_lifetime.x, _lifetime.y);

            for (int i = 0; i < particles.Length; i++)
            {
                Vector3 pos = Random.insideUnitSphere;

                particles[i] = new Particle
                {
                    active = 1,
                    scale = _particleScale,
                    lifetime = lifetime,
                    age = 0,
                    position = pos,
                    basePosition = pos,
                    targetPosition = Vector3.zero,
                    oldPosition = Vector3.zero,
                    velocity = Vector3.down * 0.1f,
                    color = ParticleColorData,
                };
            }

            _particleBuffer.SetData(particles);

            _shader.SetBuffer(_initializeKernelId, "_ParticleBuffer", _particleBuffer);
            _shader.SetBuffer(_initializeKernelId, "_DeadListBuffer", _particlePoolBuffer);
            _shader.SetBuffer(_updateKernelId, "_ParticleBuffer", _particleBuffer);
            _shader.SetBuffer(_emitKernelId, "_ParticleBuffer", _particleBuffer);
            _shader.SetBuffer(_emitKernelId, "_ParticlePooBuffer", _particlePoolBuffer);
            _shader.SetBuffer(_emitKernelId, "_ParticleDataBuffer", _particleDataBuffer);

            _material.SetBuffer("_ParticleBuffer", _particleBuffer);

            _shader.Dispatch(_initializeKernelId, _count / 8, 1, 1);
        }

        private void UpdateParticles()
        {
            _shader.SetFloat("_Time", Time.time);
            _shader.SetFloat("_DeltaTime", Time.deltaTime);
            _shader.Dispatch(_updateKernelId, _count / 8, 1, 1);
        }

        private void DrawParticles()
        {
            Graphics.DrawMeshInstancedIndirect(
                _particleMesh,
                0,
                _material,
                new Bounds(transform.position, Vector3.one * 32f),
                _argsBuffer,
                0,
                null,
                ShadowCastingMode.Off,
                false,
                gameObject.layer);
        }

        public void Play()
        {
            _running = true;
        }

        public void Stop()
        {
            _running = false;
        }

        public bool Emit(int emitCount)
        {
            GraphicsBuffer.CopyCount(_particlePoolBuffer, _particleCountBuffer, 0);
            _particleCountBuffer.GetData(_particleCountArgs);
            int count = Mathf.Min(_particleCountArgs[0], emitCount);
            if (count == 0)
            {
                Debug.LogWarning("All particles are consumed.");
                return false;
            }

            for (int i = 0; i < count; i++)
            {
                _particleDataStore[i] = new ParticleData
                {
                    basePosition = transform.position + Random.insideUnitSphere * _radius,
                    velocity = Vector3.down * 0.01f,
                    color = _particleColor,
                };
            }

            _particleDataBuffer.SetData(_particleDataStore);
            _shader.Dispatch(_emitKernelId, count / 8, 1, 1);

            return true;
        }
    }
}