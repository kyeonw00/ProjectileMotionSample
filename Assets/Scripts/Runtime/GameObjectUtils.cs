using UnityEngine;

public static class GameObjectUtils
{
    /// <summary> <paramref name="container"/>에 <paramref name="containee"/>가 포함되었는지 체크합니다. </summary>
    /// <param name="container"></param>
    /// <param name="containee"></param>
    /// <returns><paramref name="container"/>에 <paramref name="containee"/>가 포함되어있다면 TRUE를 리턴합니다.</returns>
    public static bool ContainsMask(this LayerMask container, LayerMask containee)
    {
        return (container.value & containee) != 0;
    }
}