using UnityEngine;

namespace MVBP.Models;

internal class NamedSnapPoint
{
    public Vector3 LocalPosition { get; }
    public string Name { get; }

    public NamedSnapPoint(Vector3 localPosition, string name)
    {
        LocalPosition = localPosition;
        Name = name;
    }

    public NamedSnapPoint(float x, float y, float z, string name)
    {
        LocalPosition = new Vector3(x, y, z);
        Name = name;
    }

    public override string ToString()
    {
        return $"{LocalPosition} [{Name}]";
    }
}
