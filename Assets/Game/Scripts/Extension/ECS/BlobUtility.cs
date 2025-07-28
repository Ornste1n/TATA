using Game.Scripts.Mechanics.Units.General.Components;
using Unity.Entities;

namespace Game.Scripts.Extension.ECS
{
    public static class BlobUtility
    {
        public static bool TryGetUnit(ref UnitBlobRoot root, uint id, out UnitBlob result)
        {
            ref BlobArray<UnitBlob> units = ref root.Units;
            int left = 0, right = units.Length - 1;

            while (left <= right)
            {
                int mid = (left + right) / 2;
                uint currentId = units[mid].Id;

                if (currentId == id)
                {
                    result = units[mid];
                    return true;
                }
                else if (currentId < id)
                    left = mid + 1;
                else
                    right = mid - 1;
            }

            result = default;
            return false;
        }
    }
}