using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

namespace TravellingSalesmanProblem
{
    /// <summary>
    /// Logika interakcji dla klasy MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            Main2();
        }


        public void Main1()
        {
            //problem jest instancją N miast o losowych koordynatach. "a problem instance π"
            //ilosc węzłów jest parametrem. Wartości krawędzi są losowe. Każde miasto ma swoje własne, losowe koordynaty. Wartości krawędzi to fizyczny dystans
            //między nimi.
            Problem problem = new Problem(7);
            //Solution jest losowym rozwiazaniem. Porządek odwiedzonych miast jest całkowicie losowy niezależnie od temperatury. Nie daje to zbyt dobrych rezultatów,
            //ale jest proste w implementacji.
            Solution initialSolution = new Solution(problem);   //"Initial Solution s 0"
            Solution bestSolution = initialSolution;
            Solution newSolution;

            //"Initial Temperature" temperatura układu. Zwiększenie jej zwiększa ilość iteracji, ale też zwiększa szansę na to, że w Metropolis criterion 
            //zostanie wybrane gorsze rozwiązanie.
            double temperature = 100;
            //Cooling Scheme. W tym przypadku wykladniczy. Im większa dokładność do 1, tym więcej iteracji.
            double coolingRate = 0.99;
            //iteration count
            int iterations = 0;
            //dobór początkowych parametrów temperature, cooling rate i stopping criterion jest w tym przypadku wyłącznie intuicyjny, bazowałem na
            //ilości iteracji i wartości Score.
            while (temperature > 0.0001) //Stopping Criterion. W tym przypadku, stała wartość temperatury.
            {

                //Exploration Criterion. W tym przypadku, całkowicie losowa nowa ścieżka.
                //W przypadku takiego Exploration Criterion, annealing nie jest zbyt wydajny. Wkrótcę wyślę nową, poprawioną wersję programu.
                //Na razie jednak, ze względu na zbliżający się deadline taki musi wystarczyć.
                //Poprawny Exploration Criterion, powinien bazować na zmianach w ścieżce w zależności od temperatury.
                //I mniejsza temperatura, zmiany powinny być mniejsze. Wysoka temperatura powinna praktycznie losować na nowo całe rozwiązanie.
                newSolution = new Solution(problem);

                iterations++;

                //delta(s, s')
                double delta = newSolution.Score - bestSolution.Score;

                //Acceptance Criterion. W tym przypadku, Metropolis-based criteria. Na zajęciach było wspomniane, że takiego musimy użyć.
                if (delta < 0 || (delta > 0 && Math.Exp(-delta / temperature) > problem.RNG.NextDouble()))
                {
                    bestSolution = newSolution;

                    //debug output. Dla każdego nowego bestSolution, wypisuję jego dane.
                    outputTB.Text += "Path: ";
                    foreach (int j in bestSolution.CitySequence)
                    {
                        outputTB.Text += j + " ";
                    }
                    outputTB.Text += "    Temperature: " + temperature + "    Score: " + bestSolution.Score + "    Iteration: " + iterations + "\n";
                }
                //Cooling Scheme. 
                temperature = temperature * coolingRate;

            }
        }

        public void Main2()
        {
            outputTB.Text = "";
            canvasTSP.Children.Clear();


            //problem jest instancją N miast o losowych koordynatach. "a problem instance π"
            //ilosc węzłów jest parametrem. Wartości krawędzi są losowe. Każde miasto ma swoje własne, losowe koordynaty. Wartości krawędzi to fizyczny dystans
            //między nimi.
            Problem problem = new Problem(20);
            //Solution jest losowym rozwiazaniem.
            Solution initialSolution = new Solution(problem);   //"Initial Solution s 0"
            SwapSolution bestSolution = new SwapSolution(problem, initialSolution);
            SwapSolution newSolution;

            //"Initial Temperature" temperatura układu. Zwiększenie jej zwiększa ilość iteracji, ale też zwiększa szansę na to, że w Metropolis criterion 
            //zostanie wybrane gorsze rozwiązanie.
            double temperature = 50;
            //Cooling Scheme. W tym przypadku wykladniczy. Im większa dokładność do 1, tym więcej iteracji.
            double coolingRate = 0.999;
            //iteration count
            int iterations = 0;
            //dobór początkowych parametrów temperature, cooling rate i stopping criterion jest w tym przypadku wyłącznie intuicyjny, bazowałem na
            //ilości iteracji i wartości Score.
            while (temperature > 0.0001) //Stopping Criterion. W tym przypadku, stała wartość temperatury.
            {

                //Nowy, ulepszony exploration criterion. W każdym rozwiązaniu miasta są losowo zamieniane ze sobą. Im większa temperatura, tym więcej nastąpi
                //zamian. Maksymalna ilość zamian w jednym rozwiązaniu to (Nmiast * 10). Minimalna ilość to 1. Przy niskiej temperaturze, W każdym nowym
                //rozwiązaniu następuje wyłącznie jedna zamiana. Daje to o wiele bardziej dokładnie wyniki, niż tasowanie ścieżki.
                newSolution = new SwapSolution(problem, bestSolution, temperature);

                iterations++;

                //delta(s, s')
                double delta = newSolution.Score - bestSolution.Score;

                //Acceptance Criterion. W tym przypadku, Metropolis-based criteria. Na zajęciach było wspomniane, że takiego musimy użyć.
                if (delta < 0 || (delta > 0.1 && Math.Exp(-delta / temperature) > problem.RNG.NextDouble()))
                {
                    bestSolution = newSolution;

                    //debug output. Dla każdego nowego bestSolution, wypisuję jego dane.
                    outputTB.Text += "Path: ";
                    foreach (int j in bestSolution.CitySequence)
                    {
                        outputTB.Text += j + " ";
                    }
                    outputTB.Text += "    Temperature: " + temperature + "    Score: " + bestSolution.Score + "    Iteration: " + iterations + "\n";
                }
                //Cooling Scheme. 
                temperature = temperature * coolingRate;

            }


            //rysowanie
            Ellipse[] dots = new Ellipse[bestSolution.CitySequence.Length];

            for(int i = 0; i<bestSolution.CitySequence.Length; i++)
            {
                dots[i] = new Ellipse
                {
                    Stroke = new SolidColorBrush(Colors.Red),
                    StrokeThickness = 3,
                    Height = 10,
                    Width = 10,
                    Fill = new SolidColorBrush(Colors.Red),


                    Margin = new Thickness(bestSolution.Cities[bestSolution.CitySequence[i]].X * 10, canvasTSP.Height - bestSolution.Cities[bestSolution.CitySequence[i]].Y * 10, 0, 0)
                };

                canvasTSP.Children.Add(dots[i]);
            }

            Line[] lines = new Line[bestSolution.CitySequence.Length];

            for (int i = 0; i < bestSolution.CitySequence.Length - 1; i++)
            {
                Thickness margin1 = dots[i].Margin;
                Thickness margin2 = dots[i + 1].Margin;
                lines[i] = new Line
                {
                    Stroke = Brushes.Black,
                    StrokeThickness = 1,
                    X1 = margin1.Left,
                    X2 = margin2.Left,
                    Y1 = margin1.Top,
                    Y2 = margin2.Top
                };

                canvasTSP.Children.Add(lines[i]);
            }

            Line lastLine = new Line
            {
                Stroke = Brushes.Black,
                StrokeThickness = 1,
                X1 = dots[dots.Length-1].Margin.Left,
                X2 = dots[0].Margin.Left,
                Y1 = dots[dots.Length - 1].Margin.Top,
                Y2 = dots[0].Margin.Top
            };

            canvasTSP.Children.Add(lastLine);

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Main2();
        }
    }
}
