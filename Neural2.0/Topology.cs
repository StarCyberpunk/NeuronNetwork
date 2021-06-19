using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neural2._0
{
    public class Topology
    {
        public int InputCount;
        public int OutputCount;
        public int HiddenCount;
        public double LearningRate;
        public List<int> HiddenLayers;
        public int hiddenLayersCount;
        

        //входные значения и тд в нетворк 
        public Topology(int input, int output, int hidden,int hiddenLayers ,double LR)
        {

            InputCount = input;
            OutputCount = output;
            HiddenCount = hidden;
            LearningRate = LR;
            hiddenLayersCount = hiddenLayers;
            HiddenLayers = new List<int>(hiddenLayers);
           

        }
        
    }
}
