using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace WeatherSimulator
{
    class Simulator
    {
        readonly double[,] Q = { { -0.4, 0.3, 0.1 }, { 0.4, -0.8, 0.4 }, { 0.1, 0.4, -0.5 } };
        static readonly Random random = new Random();

        public Simulator()
        {
            N = 0;
            State = 0;
            ChangeTime = 0;
            Frequency = new int[3];
            Probabilities = new double[3];
            TProbs = new double[] { 8.0 / 21, 19.0 / 63, 20.0/63 };
        }

        int N { get; set; }
        public int State { get; private set; }
        public double ChangeTime { get; private set; }
        int[] Frequency { get; set; }
        public double[] Probabilities { get; private set; }
        public double[] TProbs { get; private set; }

        void ChangeState()
        {
            var tau = Math.Log(random.NextDouble()) / Q[State, State];
            ChangeTime += Math.Ceiling(tau);

            var alpha = random.NextDouble();
            double[] probs = new double[3];

            for (int j = 0; j < 3; j++)
            {
                if (j != State)
                {
                    probs[j] = -(Q[State, j] / Q[State, State]);
                }
                else
                {
                    probs[j] = 0;
                }
            }

            for (int j = 0; j < 3; j++)
            {
                alpha -= probs[j];

                if (alpha <= 0)
                {
                    State = j;
                    break;
                }
            }
        }

        public void Process(int time)
        {
            if (time >= ChangeTime)
            {
                ChangeState();
                Frequency[State]++;
                N++;
                for (int i = 0; i < 3; i++)
                {
                    Probabilities[i] = (double)Frequency[i] / N;
                }
            }
        }
    }
}
