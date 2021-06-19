using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neural2._0
{
    public class Layer
    {
        public List<Neuron> Neurons { get; set; }
        public int CountNeurons;
        public NeuronType NeuronType;
        
        public Layer(List<Neuron> neurons, NeuronType type = NeuronType.Hidden)
        {
            Neurons = new List<Neuron>();
            for (int i = 0; i < neurons.Count; i++)
            {
                if (neurons[i].NeType != type) throw new Exception("Type not good");
                Neurons.Add(neurons[i]);

            }
            NeuronType = type;
            CountNeurons = neurons.Count;

        }
        public void AddNeuron(Neuron ne)
        {
            Neurons.Add(ne);
        }
        public List<double> GetSignals()
        {
            var res = new List<double>();
            foreach (var neuron in Neurons)
            {
                res.Add(neuron.Output);
            }
            return res;
        }
        public override string ToString()
        {
            return NeuronType.ToString();
        }
    }
}
