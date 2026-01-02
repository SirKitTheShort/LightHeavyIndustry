
/*
using System;
using Unity.Entities;

namespace LightHeavyIndustry.Components
{
    /// <summary>
    /// Buffer component that stores which chimney/smoke prefabs have been permanently removed
    /// from a Light Industry building.
    /// 
    /// NOTE: Serialization not yet implemented - chimneys will reappear after save/load.
    /// This will be added once the core functionality is working.
    /// </summary>
    [InternalBufferCapacity(0)]
    public struct RemovedChimneyPrefab : IBufferElementData, IQueryTypeParameter, IEquatable<RemovedChimneyPrefab>
    {
        /// <summary>
        /// The prefab entity that should be permanently hidden (chimney, smoke, etc.)
        /// </summary>
        public Entity m_ChimneyPrefab;

        /// <summary>
        /// Initializes a new instance of the <see cref="RemovedChimneyPrefab"/> struct.
        /// </summary>
        /// <param name="chimneyPrefab">The prefab entity to remove.</param>
        public RemovedChimneyPrefab(Entity chimneyPrefab)
        {
            m_ChimneyPrefab = chimneyPrefab;
        }

        /// <inheritdoc/>
        public bool Equals(RemovedChimneyPrefab other)
        {
            return m_ChimneyPrefab.Equals(other.m_ChimneyPrefab);
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            return m_ChimneyPrefab.GetHashCode();
        }
    }
}

*/