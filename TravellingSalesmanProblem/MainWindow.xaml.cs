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
            //problem jest instancją N miast o losowych koordynatach. "a problem instance π"
            //ilosc węzłów jest parametrem. Wartości krawędzi są losowe. Każde miasto ma swoje własne, losowe koordynaty. Wartości krawędzi to fizyczny dystans
            //między nimi.
            Problem problem = new Problem(30);
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
    }
}
