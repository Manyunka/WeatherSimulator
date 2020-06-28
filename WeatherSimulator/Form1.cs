using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WeatherSimulator
{
    public partial class Form1 : Form
    {
        readonly Simulator simulator;
        readonly string[] states = new string[] {"Ясно", "Облачно", "Пасмурно"};
        int Time { get; set; }
        public Form1()
        {
            InitializeComponent();
            Time = 0;
            simulator = new Simulator();

            timer1.Start();

            for (int i = 0; i < 3; i++)
            {
                chart1.Series[1].Points.AddXY(i + 1, simulator.TProbs[i]);
            }
        }

        private void Timer1_Tick(object sender, EventArgs e)
        {
            Tick();
        }

        void Tick()
        {
            Time += timer1.Interval / 1000;
            simulator.Process(Time);

            pictureBox1.Image = imageList1.Images[simulator.State];
            stateLabel.Text = states[simulator.State];

            var day = Time / 24;

            timeLabel.Text = "Время: " + (day + 1) + " день, " + (Time - day * 24) + " ч.";
            stateChangeTime.Text = "Изменится в: " + (simulator.ChangeTime - day * 24) + " ч.";

            chart1.Series[0].Points.Clear();
            for (int i = 0; i < 3; i++)
            {
                chart1.Series[0].Points.AddXY(i + 1, simulator.Probabilities[i]);
            }

            averageLabel.Text = "Average: " + simulator.Average.ToString("F3") + " (error = " + Math.Round(simulator.AverageError * 100) + "%)";
            varianceLabel.Text = "Variance: " + simulator.Variance.ToString("F3") + " (error = " + Math.Round(simulator.VarianceError * 100) + "%)";

            chiSquareLabel.Text = "ChiSquared: " + simulator.ChiSquare.ToString("F3") + " <= " + simulator.CriticalValue.ToString("F3") + " is " + simulator.ChiSquareTest;
        }
    }
}
