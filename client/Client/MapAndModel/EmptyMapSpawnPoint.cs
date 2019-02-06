using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace LoadRunnerClient.MapAndModel
{
    class EmptyMapSpawnPoint: MapSpawnPoint
    {

        public EmptyMapSpawnPoint(int x, int y)
        {
            posX = x;
            posY = y;
        }
    }
}
