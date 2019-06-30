// Copyright 2019 Eugeny Novikov. Code under MIT license.

using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace AmazingTrack
{
    public class BlockHolesStrategy
    {
        readonly RangeInt probability = new RangeInt(5, 15);

        int stepCounter = 0;

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
