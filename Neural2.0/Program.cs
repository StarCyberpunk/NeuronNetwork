using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Neural2._0
{
    class Program
    {
        static void Main(string[] args)
        {
           
            
            
            var circlesPath = @"C:\Users\Год\Desktop\shapes\circles";
            var trianglesPath = @"C:\Users\Год\Desktop\shapes\triangles";
            var squaresPath= @"C:\Users\Год\Desktop\shapes\squares";
            var Test = @"C:\Users\Год\Desktop\shapes\podborka";
            var Test2 = @"C:\Users\Год\Desktop\shapes\podborka\Test";
            var converter = new PictureConverter();
            var Files = Directory.GetFiles(@"C:\Users\Год\Desktop\shapes\triangles"); 
            var testImageInputPAR = converter.Convert(Files[0]);
            Topology top = new Topology(testImageInputPAR.Count, 1, testImageInputPAR.Count / 16,2, 0.42);
            Network net = new Network(top);

            Console.WriteLine("Learn? \t Enter: Yes \t Backspace= No");
            if (Console.ReadKey().Key == ConsoleKey.Enter)
            {
                double[,] trianglesInputs = GetData(trianglesPath, converter, testImageInputPAR, net);
                double[,] CirInputs = GetData(circlesPath, converter, testImageInputPAR, net);
                double[,] SqrInputs = GetData(squaresPath, converter, testImageInputPAR, net);
                for (int i = 0; i < 80; i++)
                {
                    double[] itp = net.GetRow(trianglesInputs, i);
                    double[] bad = net.GetRow(CirInputs, i);
                    double[] bad2 = net.GetRow(SqrInputs, i);
                     net.LearnN(new double[] { 1.0 }, itp, 100);
                    if (i % 2 == 0) net.LearnN(new double[] { 0.0 }, bad, 100);
                    else net.LearnN(new double[] { 0.0 }, bad2, 100);

                }

                Console.WriteLine("Save Weight? \t Enter: Yes \t Backspace= No");
                if (Console.ReadKey().Key == ConsoleKey.Enter) net.WriteToXml_Journal("out.xml");

            }
            else net.ReadXml_Journal("out.xml");


            var test = Directory.GetFiles(@"C:\Users\Год\Desktop\shapes\podborka");
            var test12 = converter.Convert(test[0]);
            var test21 = converter.Convert(test[1]);
            var test31 = converter.Convert(test[2]);
            double[,] trianglesTest = GetData(Test, converter, test12, net);
            net.Prediction(trianglesTest,0);            
            net.Prediction(trianglesTest,1);
            net.Prediction(trianglesTest, 2);
            var test2= Directory.GetFiles(@"C:\Users\Год\Desktop\shapes\podborka\Test");
            double[,] trianglesTest2 = GetData(Test2, converter, test12, net);
            for (int i = 0; i < test2.Length; i++)
            {
                var test221 = converter.Convert(test2[i]);
                net.Prediction(trianglesTest2, i);
                Console.WriteLine(test2[i]);
            }
            Console.ReadKey();


            
        }
        private static double[,] GetData(string Path, PictureConverter converter, List<int> testImageInput, Network net)
        {
            var Image = Directory.GetFiles(Path);
            var size = Image.Length;
            var Inputs = new double[size,testImageInput.Count];
            for (int i = 0; i < size; i++)
            {
                var image = converter.Convert(Image[i]);
                for (int j = 0; j < image.Count; j++)
                {
                    Inputs[i, j] = image[j];

                }

            }
            return Inputs;

        }
        
    }
}
