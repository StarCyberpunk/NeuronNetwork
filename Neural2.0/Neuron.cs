using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neural2._0
{
    public class Neuron
    {
        public List<double> Input { get; set; }
        public double Output { get; set; }
        public List<double> Weight { get; set; }
        public NeuronType NeType;
        public double Summ { get; set; }
        public double B { get; set; }
        public double Delta { get; set; }
       
        public Neuron(int input,Random rnd, NeuronType type = NeuronType.Hidden, double b = 0)
        {
            NeType = type;
            Weight = new List<double>(input);
            Input = new List<double>(input);
            B = b;
            Delta = 0;
            //метод запонения весов на каждый слой 
            //заполнение веса в инпут 

            InitWeInRandom(input,rnd);

        }

        private void InitWeInRandom(int input)
        {
            var rnd = new Random();
            for (int i = 0; i < input; i++)
            {
                 Weight.Add(rnd.NextDouble() - rnd.NextDouble());
                Input.Add(0);
            }
        }
        private void InitWeInRandom(int input,Random rnd)
        {
            for (int i = 0; i < input; i++)
            {

                if (NeType == NeuronType.Input) Weight.Add(1);
                else Weight.Add(rnd.NextDouble()-rnd.NextDouble());
                Input.Add(0);
            }
        }

        public double Activ(List<double> input)
        {
            if (input.Count != Weight.Count)
            {
                throw new Exception("Весы и инпуты не равны");
            }
            for (int j = 0; j < input.Count; j++)
            {
                Input[j] = input[j];
            }
            double sum = 0.0;
            for (int i = 0; i < input.Count; i++)
            {
                sum += input[i] * Weight[i] ;
            }
            /*SetB();*/
            
            Summ = sum;
            if (NeType != NeuronType.Input) Output = Sigmoid(sum);
            else Output = sum;
            return Output;
        }
        public void TakeDelta(double error)
        {
            if (NeType == NeuronType.Input) return;
            double summ = 0;
            for (int i = 0; i < Weight.Count; i++)
            {
                var weight = Weight[i];
                summ += error * weight;
            }
            summ = summ * (Output * (1 - Output));
            Delta=summ;
        }
        public void ReloadWeight(double LR)
        {
            for (int i = 0; i < Weight.Count; i++)
            {
                var weight = Weight[i];
                var input = Input[i];
                var newwe = weight - input * Delta * LR;
                Weight[i] = newwe;
            }
        }
        

        public void SetB(double b)
        {
            B = b;
        }
        public double Sigmoid(double x)
        {
            return 1.0 / (1.0 + Math.Pow(Math.E, -x*0.4));//to do параметр наклона 0.4 
        }
        private double SigmoidDx(double x)
        { //
            return Sigmoid(x) * (1 - Sigmoid(x));
        }
        
        
        public override string ToString()
        {
            return Output.ToString();
        }
    }
}
