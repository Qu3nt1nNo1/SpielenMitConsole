﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonsterHunter
{
    struct Direction
    {
        //  possible moves
        /*  direction       x   y               how often
         *  up              0   -1      8       1   *2      2       1,2 
         *  right up        1   -1      7       0           
         *  right           1   0       6       1   *6      6       3,4,5,6,7,8
         *  right down      1   1       5       0           
         *  down            0   1       4       1   *2      2       9,10
         *  left down       -1  1       3       0           
         *  left            -1  0       2       1   *8      8       11,12,13,14,15,16,17,18 
         *  left up         -1  -1      1       0           
         *  stay            0   0       0       1   *1      1       19
         *                                              --------
         *                                                  19
         */

        // public Direction[] movement;

        public int[]
            up,
            right_up, 
            right,
            right_down,
            down,
            left_down,
            left,
            left_up,
            stay
            ;
        
    }
}
