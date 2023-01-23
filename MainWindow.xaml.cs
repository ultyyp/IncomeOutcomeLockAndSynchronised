using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace IncomeOutcomeLockAndSynchronised
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        class LockTotal
        {
            decimal total = 0;
            private static readonly object _object = new object();

            public decimal Total
            {
                get { return total; }
            }

            public void AddTotal(decimal value)
            {
                lock(_object)
                {
                    total += value;
                }
            }

            public void SubstractTotal(decimal value)
            {
                lock (_object)
                {
                    total -= value;
                }
            }



        }


        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (comboBox.SelectedIndex == 0)
            {
                //decimal total = 0m;
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();
                LockTotal lt = new LockTotal();

                string incomePath = @"C:\Payments\income.txt";
                string outcomePath = @"C:\Payments\outcome.txt";

                



                var incomeTask = Task.Run(() =>
                {
                    string[] incomeLines = File.ReadAllLines(incomePath);
                    foreach (string line in incomeLines)
                    {
                        lt.AddTotal(decimal.Parse(line));
                    }
                });

                var outcomeTask = Task.Run(() =>
                {

                    string[] outcomeLines = File.ReadAllLines(outcomePath);
                    foreach (string line in outcomeLines)
                    {
                        lt.SubstractTotal(decimal.Parse(line));
                    }
                    stopwatch.Stop();



                });

                Task.WaitAll(incomeTask, outcomeTask);
                TimeSpan ts = stopwatch.Elapsed;
                IncomeBox.Text = lt.Total.ToString();
                TimeBlock.Text = "Time Elapsed: " + ts.ToString();



            }


            else if (comboBox.SelectedIndex == 1)
            {
                decimal income = 0m, outcome = 0m, total = 0m;
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();
                string incomePath = @"C:\Payments\income.txt";
                string outcomePath = @"C:\Payments\outcome.txt";
                void outcomeAction()
                {
                    string[] outcomeLines = File.ReadAllLines(outcomePath);
                    foreach (string line in outcomeLines)
                    {

                        outcome += decimal.Parse(line);
                    }
                }
                void incomeAction()
                {
                    string[] incomeLines = File.ReadAllLines(incomePath);
                    foreach (string line in incomeLines)
                    {
                        income += decimal.Parse(line);
                    }
                }
                Parallel.Invoke(incomeAction, outcomeAction);
                Parallel.Invoke(() =>
                {
                    total = income - outcome;
                    stopwatch.Stop();
                    TimeSpan ts = stopwatch.Elapsed;
                    Dispatcher.Invoke(() =>
                    {
                        IncomeBox.Text = "Income: " + total;
                        TimeBlock.Text = "Time Elapsed: " + ts;
                    });
                });
            }
        }
    }
}
