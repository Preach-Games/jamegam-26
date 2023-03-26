using System;

namespace DungeonDraws.SO
{
    [Serializable]
    public class Range<T>
    {
        public Range(T min, T max)
        {
            Min = min;
            Max = max;
        }

        public T Min;
        public T Max;
    }
}
