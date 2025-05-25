using System;
using Unity.Burst;
using Unity.Physics;
using Unity.Collections;

namespace Game.Scripts.Mechanics.Units.Selection
{
    [BurstCompile]
    public readonly struct OverlapCollector : ICollector<ColliderCastHit>, IDisposable
    {
        public NativeList<ColliderCastHit> Hits { get; }

        public int NumHits => Hits.Length;
        public bool EarlyOutOnFirstHit => false;
        public float MaxFraction => float.MaxValue;
        
        public OverlapCollector(int capacity = 4, Allocator allocator = Allocator.Temp)
        {
            Hits = new NativeList<ColliderCastHit>(capacity, allocator);
        }
        
        [BurstCompile]
        public bool AddHit(ColliderCastHit hit)
        {
            Hits.Add(hit);
            return true;
        }

        public void Dispose()
        {
            Hits.Dispose();
        }
    }
}