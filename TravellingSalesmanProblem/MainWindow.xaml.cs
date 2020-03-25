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
        //problem jest instancją N miast o losowych koordynatach. "a problem instance π"
        //ilosc węzłów jest parametrem. Wartości krawędzi są losowe. Każde miasto ma swoje własne, losowe koordynaty. Wartości krawędzi to fizyczny dystans
        //między nimi.
        public Problem problem;

        //Funkcja Main. Ponieważ projekt jest napisany w WPF, jest nią MainWindow()
        public MainWindow()
        {
            InitializeComponent();  //na początku inicjalizowane jest okno
            InitializeProblem();    //problem
            Main4(problem);         //i uruchamiany jest annealing
        }

        //Inicjalizacja problemu. 
        public void InitializeProblem()
        {
            problem = new Problem(int.Parse(cityBox.Text), 1);
        }

      
        //główny algorytm annealingu. Wersja 4.
        public void Main4(Problem problem)
        {
            //Na początek czysczę trace log i wizualicję problemu.
            outputTB.Text = "";
            canvasTSP.Children.Clear();

            //zmienna do przetrzymywania traceloga. kwestie optymalizacji.
            string debugOutput = "";


            

            //Solution to klasa do initialSolution. Jest to całkowicie losowa ścieżka uzyta jako początkowa.
            Solution initialSolution = new Solution(problem);   //"Initial Solution s 0"
            //na początku best solution to initial solution. w tym momencie pojawia się pierwsze losowe przekształcenie, jest ono zawsze akceptowane.
            TwoOptSolution bestSolution = new TwoOptSolution(problem, initialSolution);
            TwoOptSolution newSolution;

            //"Initial Temperature" temperatura układu. Zwiększenie jej zwiększa ilość iteracji, ale też zwiększa szansę na to, że w Metropolis criterion 
            //zostanie wybrane gorsze rozwiązanie.
            double temperature = 0.5;
            //Cooling rate.
            double coolingRate = 0.9;



            //EXPLORATION CRITERION I COOLING SCHEME, JAK DZIAŁA
            //Exploration criterion bazowane jest na https://www.fourmilab.ch/documents/travelling/anneal/
            //Annealing dziala na bazie cykli. W każdym cyklu wykonywany jest szereg przekształceń.  | zmienna globalIterations
            //Przekształcenia dzielę na 2 kategorie
            //- udane, czyli takie gdy koszt zmniejszył się, lub rozwiązanie zostało zaakceptowane na bazie Metropolis criterion    | zmienna changes
            //- nieudane, czyli takie które nie zostało zaakceptowane
            //Każdy cykl ma 2 maksymalne ilości przekształceń do wykonania
            //Maksymalna ilość udanych przekształceń w każdym cyklu to Nmiast * 10 | zmienna minSucc
            //Maksymalna ilość nieudanych + udanych przekształceń w każdym cyklu to Nmiast * 100 | zmienna over
            //Cykl kończy się, ilość udanych przekroczy minSucc lub ilosc udanych + nieudanych przekroczy over
            //Na końcu każdego cyklu zmniejszana jest temperatura | temperature *= coolingRate
            //Algorytm kończy się, gdy na końcu cyklu ilość udanych przekształceń będzie wynosić 0

            //Podstawową operacją przekształcania jest odwracanie części ścieżki, bazujące na algorytmie 2-opt https://en.wikipedia.org/wiki/2-opt
            //W każdym nowym rozwiązaniu losowane jest X elementów połączonych ze sobą po kolei, po czym są one odwracane kolejnością


            int iterations = 0; //ilosc nowych rozwiązań
            
            int over = problem.Cities.Length * 100; //maksymalna ilość rozwiązań
            int minSucc = problem.Cities.Length * 10;   //maksymalna ilość udanych przekształceń

            int globalIterations = 0;   //ilość cykli
            int changes = 1;    //ilość udanych przekształceń


            while (changes != 0)    //nowy cykl zaczyna się, gdy w poprzednim cyklu nastąpiły jakieś udane przekształcenia
            {
                iterations = 0;
                changes = 0;
                while (iterations < over && minSucc > changes)  //właściwy annealing
                {
                    
                    newSolution = new TwoOptSolution(problem, bestSolution);    //generowane jest nowe rozwiązanie na bazie najlepszego.

                    iterations++;

                    //delta(s, s')
                    double delta = newSolution.Score - bestSolution.Score;

                    //Acceptance Criterion. W tym przypadku, Metropolis-based criteria. 
                    //W niektórych przypadkach (nie wiem skąd to się bierze, prawdopodobnie błąd zaokrągleń) Metropolis może przyjąć dokładnie 
                    //takie same rozwiązanie za lepsze. Stąd delta > 0.0000001
                    if (delta < 0 || (delta > 0.0000001 && Functions.Metropolis(problem.RNG, temperature, delta))) 
                    {
                        bestSolution = newSolution; //nowe rozwiązanie staje się najlepszym rozwiązaniem
                        changes++;                  
                    }
                }

                //Na końcu każdego cyklu zapisuję informacje o zmianach do loga.

                if (debugCM.IsChecked == true)
                {
                    //debugOutput += "Path: ";
                    //foreach (int j in bestSolution.CitySequence)
                    //{
                    //    debugOutput += j + " ";
                    //}

                    debugOutput += "    Temperature: " + temperature + "    Score: " + bestSolution.Score + "    Iteration: " + globalIterations + "    Changes: " + changes + "\n"; 
                }

                //zmniejszenie temperatury
                temperature = temperature * coolingRate;
                globalIterations++;

                //restart temperatury uważam za zbędny.
            }

            //koniec annealingu
            
            outputTB.Text += debugOutput;//wypisuję debug na ekran

            //-----------------------------
            //rysowanie
            //-----------------------------

            //dla każdego miasta dodaję czerwoną kropkę na canvas
            Ellipse[] dots = new Ellipse[bestSolution.CitySequence.Length];

            for (int i = 0; i < bestSolution.CitySequence.Length; i++)
            {
                dots[i] = new Ellipse
                {
                    Stroke = new SolidColorBrush(Colors.Red),
                    StrokeThickness = 3,
                    Height = 10,
                    Width = 10,
                    Fill = new SolidColorBrush(Colors.Red),


                    Margin = new Thickness(bestSolution.Cities[bestSolution.CitySequence[i]].X * 900, canvasTSP.Height - bestSolution.Cities[bestSolution.CitySequence[i]].Y * 900, 0, 0)
                };

                canvasTSP.Children.Add(dots[i]);
            }

            //dla każdego połączenia między miastami dodaję linie na canvas
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
                X1 = dots[dots.Length - 1].Margin.Left,
                X2 = dots[0].Margin.Left,
                Y1 = dots[dots.Length - 1].Margin.Top,
                Y2 = dots[0].Margin.Top
            };

            canvasTSP.Children.Add(lastLine);
        }


        //event handler dla przycisku reset
        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            if(dataCM.IsChecked == false)   //można zablokować generowanie nowych problemów zaznaczając opcję same dataset
            {
                problem = new Problem(int.Parse(cityBox.Text), 1);
            }
            Main4(problem);
        }

    }
}
