using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Submit
{
    class Program
    {
        static void Main(string[] args)
        {
            Perceptron p = new Perceptron();                    //creating a constructor
            p.ReadData("C:\\Users\\SANJU\\Downloads\\data.csv");//Reading csv file 
            p.TrainData();                                      //training the data
            p.Output();                                         //Predicted output
            p.ClassifyPoint();                                  //To show the summary of weight
            Console.ReadLine();                                 //For the console to wait
        }
    }
}

namespace Submit
{
    class Perceptron
    {
        private static int len;             //to get the length of the file
        private static int m;               //Declared to get the Row Size
        private static int n;               //Declared to get the Column Size
        public int[,] Final;                //Declared to get data from csv in a array
        public float[,] w;                  //Decalred to get weights
        private float[,] yi_hat;            //Declared to get the predicted value
        private static float alpha = 0.1f;  //Declared to specify the learbing rate
        private static int error = 0;       //Declared to get the error value
        private static int bias = 1;        //Declared so that the linear slope will not be always in origin
        public float[,] dummy;              //dummy assignment variable
        private float[,] sum;               //to get the sum of the values of weight
        private static int iteration = 0;   //to get max iteration

        /*##The below methods are declared so assign the value from main method and declared variable in this class##*/
        public static int M { get => m; set => m = value; }
        public static int N { get => n; set => n = value; }
        public int[,] Final1 { get => Final; set => Final = value; }
        public float[,] w1 { get => w; set => w = value; }
        public float[,] Yi_hat { get => yi_hat; set => yi_hat = value; }

        /*## To read the csv file from main method##*/
        public void ReadData(string filename)
        {
            StreamReader sr = null;                         //creating a stream leader  and assigning value null
            StreamReader sr1 = null;                        // creating another stream reader to delete the column header and to convert the value to int
            string tmp = null;
            string tmp1 = null;
            string[] output = null;                         //creating a string variable to split the delimiter
            char[] charseperator = new char[] { ',' };      //delimter value assigned
            try
            {
                Console.WriteLine("The Content of the file:");
                sr = new StreamReader(filename);            //reading the File
                do                                          //creating a do while loop
                {
                    tmp = sr.ReadLine();                    //reading the 1st line of csv
                    len++;                                  //increasing the value to get the row height 
                    if (tmp == null)                        //to stop if the data table id completed
                    {
                        break;
                    }
                    output = tmp.Split(charseperator);      //splitting the read value with delimiter
                    Console.WriteLine(tmp);                 //displaying the output to screen
                } while (true);
                len = len - 1;                              //since we are using the do while(extra empty line is added).s0 -1 
                int[,] final = new int[len - 1, 4];           //declaring a int array with size 56x4
                sr1 = new StreamReader(filename);           //reading the file once again to delete the column
                tmp1 = sr1.ReadLine();                      //reading 1st line
                string[] demo = tmp1.Split(charseperator);  //seperating the value with delemiter
                var list = new List<string>(demo);          //creating a list to delete the 1st row
                list.RemoveAt(0);                           // deleteing the zeroth value in list
                list.RemoveAt(0);                           //next is to delete all four column names    
                list.RemoveAt(0);
                list.RemoveAt(0);
                string[] value = list.ToArray();            //converting the list to array
                for (int j = 0; j < len - 1; j++)             //creating the loop the read all values in csv
                {
                    tmp1 = sr1.ReadLine();
                    demo = tmp1.Split(charseperator);
                    for (int k = 0; k < 4; k++)
                    {
                        Int32.TryParse(demo[k], out final[j, k]); //converting string array to int array
                    }
                }
                Final1 = final;                             //assinging the value to main value
                m = Final1.GetLength(0);                    //to get the row size 
                n = Final1.GetLength(1);                    //to get the column size
                Console.WriteLine("\n");
                Console.WriteLine("row size {0},column size {1}", m, n);//display in console
                Console.WriteLine("\n");
                WeigthInitial();                            //initialing value of weights
                YhatIntial();                               //initialsing all values of yhat variable
                Console.WriteLine("Total number of Iteration:{0}", iteration); //display number of iteration


            }
            catch (Exception e)
            {
                Console.WriteLine("error {0}", e.Message);  //to display error message
            }
            finally
            {
                if (sr != null)                             //finally close the reader
                    sr.Close();
            }
        }

        /*## Initial intialisation of weight array value to one##  */
        public float[,] WeigthInitial()
        {
            float[,] w = new float[m, n - 2];
            for (int i = 0; i < m; i++)
            {
                for (int j = 0; j < n - 2; j++)
                {
                    w[i, j] = 1;                            //initialising the value to one
                }

            }
            w1 = w;
            return w1;                                      //return the value of w1 
        }

        /*### Intial Initialisation of yhat value to one###*/
        public float[,] YhatIntial()
        {
            float[,] yi_hat = new float[m, n - 3];
            for (int i = 0; i < m; i++)
            {
                for (int j = 0; j < n - 3; j++)
                {
                    yi_hat[i, j] = 1;
                }
            }
            //Console.WriteLine(yi_hat[m-10,n-4]);          //to display to check the values
            Yi_hat = yi_hat;
            return Yi_hat;                                  //return yhat value 
        }

        /*## creating a method to refer to Train method to pass input variable##*/
        public void TrainData()
        {
            Train(Final1, w1, Yi_hat, dummy);
        }

        /*## Creating a train method to train the model##*/
        public static void Train(int[,] Final1, float[,] w1, float[,] Yi_hat, float[,] dummy)
        {
            error = 0;                                      //initialing the error value to zero
            for (int i = 0; i < m; i++)
            {
                for (int j = 0; j < n - 3; j++)
                {
                    //yhat =(xvalue *1st weight)+(2nd xvalue * 2nd weight value)then adding bias to it
                    Yi_hat[i, n - 4] = (bias + (Final1[i, 1] * w1[i, 0]) + (Final1[i, 2] * w1[i, 1]));
                }
                if (Yi_hat[i, n - 4] >= 0)                  //if yhat value greater than zero
                {
                    Yi_hat[i, n - 4] = 1;                   //assign to one
                }
                else
                {
                    Yi_hat[i, n - 4] = 0;                   //otherwise assign to zero
                }
            }
            Iteration(Final1, w1, Yi_hat, dummy);           //call the iteration to perform more calculation
        }

        /*## To calculate weight vector and to check if the actual and predicted value is correct##*/
        public static float[,] Iteration(int[,] Final1, float[,] w1, float[,] Yi_hat, float[,] dummy)
        {
            dummy = new float[m, n - 3];                      //assigning the dummy variable size
            iteration++;                                    //increasing the number of iteration
            for (int i = 0; i < m; i++)
            {
                for (int j = 0; j < n - 3; j++)
                {
                    if (Final1[i, n - 1] == Yi_hat[i, n - 4]) //checking if the predicted and actual value
                    {
                        if (Final1[m - 1, n - 1] == Yi_hat[m - 1, n - 4])       //checking the last value(actual vs predicted) in a array
                            break;                            //if yes break the loop
                        else { }
                    }
                    else
                    {
                        //creating weight value w   w + alpha(yi - ^yi)xi
                        dummy[i, 0] = (w1[i, j] + ((alpha) * (Final1[i, 3] - Yi_hat[i, 0])) * (Final1[i, 1] * Final1[i, 2]));
                        w1[i, 0] = dummy[i, 0];
                        error++;
                    }
                }
            }
            if (error > 0)
            {
                Train(Final1, w1, Yi_hat, dummy); // tarin again
            }
            else { }
            Yi_hat = Yi_hat;
            return w1;

        }

        /*## To loop the out method##*/
        public void Output()
        {
            Out(w1, Final1, Yi_hat);
        }

        /*## Displaying the output  ##*/
        public static void Out(float[,] w1, int[,] Final1, float[,] Yi_hat)
        {
            Console.WriteLine("########values#######\n");
            Console.WriteLine("The calculated weight value is");
            for (int i = 0; i < m; i++)
            {
                for (int j = 0; j < n - 3; i++)
                {
                    if (i == 56)
                    {
                        break;
                    }
                    Console.WriteLine("row-->{0}: weight-->{1}", i + 1, w1[i, n - 4]);
                }
            }
            Console.WriteLine("***************************************************");
            Console.WriteLine("The status value in Training data vs the predicted value");
            for (int i = 0; i < m; i++)
            {
                for (int j = 0; j < n - 3; i++)
                {
                    if (i == 56)
                    {
                        break;
                    }
                    Console.WriteLine("Row-->{0} : Value-->{1} vs Row-->{2} : Value-->{3}", i + 01, Final1[i, n - 1], i + 1, Yi_hat[i, j]);
                }
            }
            Console.WriteLine("***************************************************");

        }

        /*## Displaying the weights##*/
        public void ClassifyPoint()
        {
            classy(sum, w1);            //refering the classy public method
        }
        public static void classy(float[,] sum, float[,] w1)
        {
            sum = new float[m, n - 3];
            for (int k = 0; k < m; k++)
            {
                for (int j = 0; j < n - 3; j++)
                {
                    sum[k, j] = 0;
                }
            }
            for (int i = 0; i < m; i++)
            {
                sum[0, 0] += w1[i, 0];
            }
            for (int j = 0; j < m - 28; j++)
            {
                sum[1, 0] += w1[j, 0];          //calculating the postive weights
            }
            for (int j = 29; j < m; j++)
            {
                sum[2, 0] += w1[j, 0];          //calculating negative weights
            }
            Console.WriteLine("The sum of positive weights is {0}", sum[1, 0]);
            Console.WriteLine("The Sum of negative weights is {0}", sum[2, 0]);
            Console.WriteLine("The sum of weights is {0}", sum[0, 0]);

        }
    }
}
