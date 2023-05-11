using UnityEngine;

namespace AmazingTrack
{
    public class BlockHolesStrategy
    {
        private readonly RangeInt probability = new RangeInt(5, 15);

        private int stepCounter;

        public bool IsTimeToHole()
        {
            if (stepCounter == 0)
            {
                stepCounter = Random.Range(probability.start, probability.end);
                return true;
            }
            stepCounter--;
            return false;
        }
    }
}
