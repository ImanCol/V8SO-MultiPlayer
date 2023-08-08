using Unity.Jobs;
using UnityEngine;

public struct PositionJob : IJob
{
    public uint address;

    public int positionX;

    public int positionY;

    public int positionZ;

    public void Execute()
    {
#if DEBUG
        Debug.Log("Position address: 0x" + address.ToString("X"));
#endif
        LegacyMemoryReader.WriteInt32(address + 52, positionX);
        LegacyMemoryReader.WriteInt32(address + 56, positionY);
        LegacyMemoryReader.WriteInt32(address + 60, positionZ);
    }
}