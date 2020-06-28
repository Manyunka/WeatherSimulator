using System;

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
            TProbs = new double[] { 8.0 / 21, 19.0 / 63, 20.0 / 63 };
        }

        int N { get; set; }
        public int State { get; private set; }
        public double ChangeTime { get; private set; }
        int[] Frequency { get; set; }
        public double[] Probabilities { get; private set; }
        public double[] TProbs { get; private set; }

        public double Average { get; private set; }
        public double AverageError { get; private set; }
        public double Variance { get; private set; }
        public double VarianceError { get; private set; }

        public double ChiSquare { get; private set; }
        public bool ChiSquareTest { get; private set; }
        public double CriticalValue { get => 5.991; }

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
                    probs[j] = -Q[State, j] / Q[State, State];
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

                Statistics();
            }
        }

        void Statistics()
        {
            Average = 0;
            double E = 0;
            for (int i = 0; i < Probabilities.Length; i++)
            {
                Average += Probabilities[i] * i;
                E += TProbs[i] * i;
            }
            AverageError = Math.Abs((Average - E) / E);

            Variance = 0;
            double D = 0;
            for (int i = 0; i < Probabilities.Length; i++)
            {
                Variance += Probabilities[i] * i * i;
                D += TProbs[i] * i * i;
            }
            Variance -= Average * Average;
            D -= E * E;
            VarianceError = Math.Abs((Variance - D) / D);

            ChiSquare = 0;
            for (int i = 0; i < Probabilities.Length; i++)
            {
                ChiSquare += (Frequency[i] - N * TProbs[i]) * (Frequency[i] - N * TProbs[i]) / (N * TProbs[i]);
            }

            if (ChiSquare <= CriticalValue) ChiSquareTest = true;
            else ChiSquareTest = false;
        }
    }
}
