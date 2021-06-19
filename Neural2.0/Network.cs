using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using System.Xml.Linq;

namespace Neural2._0
{
    public class Network
    {
        public List<Layer> Layers { get; set; }
        public Topology Topology { get; set; }
        public Network(Topology topology)
        {
            Topology = topology;
            Layers = new List<Layer>();
            CreateInputLayer();
            CreateHiddenLayer();
            CreateOutputLayer();
        }

        public Neuron FeedForward(params double[] input)
        {

            SendSignalsToInput(input);
            FeddForwardToNext();//ошибка в хидене либо 1 либо 0 из за того что все 
            return Layers[Layers.Count - 1].Neurons[0];

        }

        private Neuron MaxVal(Layer layer)
        {
            var max = Double.MinValue;
            Neuron res = null;

            for (int i = 0; i < layer.CountNeurons; i++)
            {
                if (layer.Neurons[i].Output > max)
                {
                    max = layer.Neurons[i].Output;
                    res = layer.Neurons[i];
                }
            }
            return res;
        }

        //TODO предсказание
        public void Prediction(double[,] inputs, int i)
        {
            var input = GetRow(inputs, i);
            var res = FeedForward(input).Output;
            if (res >= 0.5) Console.WriteLine("Треугольник {0}",res);
            else Console.WriteLine("Не треугольник{0}",res);


        }

        public double LearnN(double[] expected, double[] inputs, int epoch)
        {
            var error = 0.0;
            for (int i = 0; i < epoch; i++)
            {
                for (int j = 0; j < expected.Length; j++)
                {
                    var output = expected[j];
                     
                    error += MetodObRasOshibki(output, j, inputs);

                }
                /*Console.WriteLine(error);*/

            }
            var res = 0.5*error;
            return res;
        }

        public double[] GetRow(double[,] inputs, int j)
        {
            var colums = inputs.GetLength(1);
            var array = new double[colums];
            for (int i = 0; i < colums; ++i) array[i] = inputs[j, i];
            return array;
        }
        public double[] GetCol(int j, double[,] inputs)
        {
            var row = inputs.GetLength(0);
            var array = new double[row];
            for (int i = 0; i < row; ++i) array[i] = inputs[i, j];
            return array;
        }

        private double[,] Scalling(double[,] inputs)
        {
            var result = new double[inputs.GetLength(0), inputs.GetLength(1)];
            for (int column = 0; column < inputs.GetLength(1); column++)
            {
                var min = inputs[0, column];
                var max = inputs[0, column];
                for (int row = 0; row < inputs.GetLength(0); row++)
                {
                    var item = inputs[row, column];
                    if (item < min)
                    {
                        min = item;
                    }
                    if (item > max)
                    {
                        max = item;

                    }
                }
                var divi = (max - min);
                for (int row = 0; row < inputs.GetLength(0); row++)
                {
                    result[row, column] = (inputs[row, column] - min) / divi;
                }
            }
            return result;
        }
        private double[,] Normalization(double[,] inputs)
        {

            var result = new double[inputs.GetLength(0), inputs.GetLength(1)];
            for (int column = 0; column < inputs.GetLength(1); column++)
            {
                var sum = 0.0;
                for (int row = 0; row < inputs.GetLength(0); row++)
                {
                    sum += inputs[row, column];
                }
                if (inputs.GetLength(0) == 0) throw new Exception("/0");
                var average = sum / inputs.GetLength(0);
                //стандартная квадратичное отклонение нейрона.
                var error = 0.0;
                for (int row = 0; row < inputs.GetLength(0); row++)
                {
                    error += Math.Pow((inputs[row, column] - average), 2);
                }
                var standardError = Math.Sqrt(error / inputs.GetLength(0));
                for (int row = 0; row < inputs.GetLength(0); row++)
                {
                    result[row, column] = (inputs[row, column] - average) / standardError;
                }

            }
            return result;
        }
        private double MetodObRasOshibki(double expected, int nub, params double[] inputs)
        {
            var actual = FeedForward(inputs).Output;
            double diff =actual-expected;
            //только для поледнего
            foreach (var ne in Layers[Layers.Count - 1].Neurons)
            {
                diff=diff * (ne.Output * (1 - ne.Output));
                ne.Delta = diff;
            }
             for (int i = Layers.Count - 2; i >= 0; i--)
             {
                 var layer = Layers[i];
                 var prevlayer = Layers[i + 1];

                 for (int j = 0; j < layer.CountNeurons; j++)
                 {
                     var ne = layer.Neurons[j];
                     for (int k = 0; k < prevlayer.CountNeurons; k++)
                     {
                         var previousNe = prevlayer.Neurons[k];
                         var error = previousNe.Delta;
                         ne.TakeDelta(error);
                     }
                     
                 }
             }
            for (int i = 1; i < Layers.Count; i++)
            {
                var layer = Layers[i];

                for (int j = 0; j < layer.CountNeurons; j++)
                {
                    var ne = layer.Neurons[j];
                    ne.ReloadWeight(Topology.LearningRate);
                }
            }

            return diff * diff;
        }


        private void FeddForwardToNext()
        {
            for (int i = 1; i < Layers.Count; i++)
            {
                var layer = Layers[i];
                var prevSignal = Layers[i - 1].GetSignals();
                foreach (var Neuron in layer.Neurons)
                {
                    Neuron.Activ(prevSignal);
                }
            }
        }

        private void SendSignalsToInput(params double[] input)
        {
            for (int i = 0; i < input.Length; i++)
            {
                var signal = new List<double>() { input[i] };
                var neuuron = Layers[0].Neurons[i];
                neuuron.Activ(signal);
            }
        }

        private void CreateOutputLayer()
        {
            var rnd = new Random();
            var outputNeurons = new List<Neuron>();
            for (int i = 0; i < Topology.OutputCount; i++)
            {
                var neuron = new Neuron(Topology.HiddenCount, rnd, NeuronType.Output);//значение из нейронов предпоследних
                outputNeurons.Add(neuron);
            }
            var Layer = new Layer(outputNeurons, NeuronType.Output);
            Layers.Add(Layer);
        }

        private void CreateHiddenLayer()
        {
            int inpus = Topology.InputCount;
            var rnd = new Random();
            for (int i = 0; i < Topology.hiddenLayersCount; i++)
            {
                var hidden = new List<Neuron>();
                for (int j = 0; j < Topology.HiddenCount; j++)
                {
                    var neuron = new Neuron(inpus, rnd, NeuronType.Hidden);
                    hidden.Add(neuron);
                }
                var hiddenla = new Layer(hidden);
                Layers.Add(hiddenla);
                inpus = hidden.Count;
            }

        }

        private void CreateInputLayer()
        {
            var inputNeurons = new List<Neuron>();
            var rnd = new Random();
            for (int i = 0; i < Topology.InputCount; i++)
            {
                var neuron = new Neuron(1,rnd, NeuronType.Input);//должно быть входное значение возможно 
                inputNeurons.Add(neuron);
            }
            var inputLayer = new Layer(inputNeurons, NeuronType.Input);
            Layers.Add(inputLayer);
        }
        public void WriteToXml_Journal(string namefile)
        {
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.IndentChars = ("\t");
            XmlWriter writer = XmlWriter.Create(namefile, settings);
            int id2 = 1;
            writer.WriteStartElement("Weight");

            for (int i = 1; i < Layers.Count; i++)
            {
                writer.WriteStartElement("Layer");
                writer.WriteAttributeString("Id", i.ToString());
                for (int j = 0; j < Layers[i].Neurons.Count; j++)
                {
                    writer.WriteStartElement("Neuron");
                    writer.WriteAttributeString("Id", j.ToString());
                    for (int k = 0; k < Layers[i].Neurons[j].Weight.Count; k++)
                    {
                        writer.WriteStartElement("Weight");
                        writer.WriteAttributeString("Id", k.ToString());
                        writer.WriteElementString("Weight", Layers[i].Neurons[j].Weight[k].ToString());
                        writer.WriteEndElement();
                    }
                    writer.WriteEndElement();
                }
                writer.WriteValue("\t");
                writer.WriteEndElement();
                id2++;
            }
            writer.WriteEndElement();
            writer.Flush();
            writer.Close();
        }
        
        public void ReadXml_Journal(string namefile)
        {
            int l = 0;
            int n = 0;
            int w = 0;
            XmlReader xmlreader = XmlReader.Create(namefile);
            while (xmlreader.Read())
            {
                if (xmlreader.IsStartElement())
                {
                    
                    if (xmlreader.HasAttributes)
                    {
                        if (xmlreader.Name == "Layer")
                        {
                            while (xmlreader.MoveToNextAttribute())
                            {
                                l = Convert.ToInt32(xmlreader.GetAttribute(xmlreader.Name));
                            }
                            
                        }
                        if (xmlreader.Name == "Neuron")
                        {
                            while (xmlreader.MoveToNextAttribute())
                            {
                                n = Convert.ToInt32(xmlreader.GetAttribute(xmlreader.Name));
                            }
                        }
                        if (xmlreader.Name == "Weight") {
                            while (xmlreader.MoveToNextAttribute())
                            {
                                w = Convert.ToInt32(xmlreader.GetAttribute(xmlreader.Name));
                            }
                        }
                        
                        xmlreader.MoveToElement();
                    }
                    
                }
                if (xmlreader.HasValue) Layers[l].Neurons[n].Weight[w] = Convert.ToDouble(xmlreader.Value);
                
            }
            xmlreader.Close();
        }
        public List<double> Cop1to2(List<double> fi)
        {
            var se = new List<double>();
            for(int i = 0; i < fi.Count; i++)
            {
                se.Add(fi[i]);
            }
            return se;
        }
    }



        } 
    


