using UnityEngine;

namespace ZENN.Core
{
    public interface IMobileDragInputProvider
    {
        bool IsDragging { get; }
        Vector2 InputVector { get; }
    }
}