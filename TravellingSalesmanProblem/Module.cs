using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TravellingSalesmanProblem
{

    //City przechowuje koordynaty X i Y dla każdego miasta. Posiada także statyczną metodę Distance, która liczy dystans pomiędzy dwoma miastami.
    public class City
    {
        public double X { get; }
        public double Y { get; }

        public City(double _X, double _Y)
        {
            X = _X;
            Y = _Y;
        }

        public static double Distance(City _pierwsze, City _drugie)
        {
            return Math.Sqrt((_drugie.X - _pierwsze.X) * (_drugie.X - _pierwsze.X) + (_drugie.Y - _pierwsze.Y) * (_drugie.Y - _pierwsze.Y));
        }

    }

    //Klasa problem posiada tablicę miast, generator liczb losowych i wektor odległości. 
    //RNG jest jeden na cały program, kwestie optymalizacji.
    public class Problem
    {
        public City[] Cities { get; }
        public Random RNG { get; }
        public double[,] DistanceArray { get; }

        public Problem(int _cityCount, int multiplier)
        {
            Cities = new City[_cityCount];
            RNG = new Random();
            for (int i = 0; i < _cityCount; i++)
            {
                Cities[i] = new City(RNG.NextDouble() * multiplier, RNG.NextDouble() * multiplier);
            }


            DistanceArray = new double[_cityCount, _cityCount];

            for (int i = 0; i < _cityCount; i++)
            {
                for (int j = 0; j < _cityCount; j++)
                {
                    DistanceArray[i, j] = City.Distance(Cities[i], Cities[j]);
                }
            }
        }

    }

    //Losowe rozwiązanie. CitySequence to tablica miast poszeregowanych wg kolejności odwiedzin. Score to dlugość całej ścieżki
    public class Solution
    {
        Problem problem;
        public int[] CitySequence { get; internal set; }
        public double Score { get; internal set; }

        public Solution(Problem _problem)
        {
            problem = _problem;
            CitySequence = new int[problem.Cities.Length];
            GenerateRandomSolution();
            CalculateScore();
        }

        private void GenerateRandomSolution()   //generuje losową ścieżkę
        {
            int max = CitySequence.Length;
            CitySequence = Enumerable.Range(0, max).OrderBy(c => problem.RNG.Next()).ToArray();
            int value = CitySequence[0];
            for (int i = 1; i < CitySequence.Length; i++)
            {
                if (CitySequence[i] == 0)
                {
                    CitySequence[0] = 0;
                    CitySequence[i] = value;
                }
            }
        }

        private void CalculateScore()   //liczy wynik na podtawie odległości miast
        {
            double suma = 0;
            for (int i = 0; i < CitySequence.Length - 1; i++)
            {
                suma += City.Distance(problem.Cities[CitySequence[i]], problem.Cities[CitySequence[i + 1]]);
            }
            suma += City.Distance(problem.Cities[CitySequence[CitySequence.Length - 1]], problem.Cities[CitySequence[0]]);
            Score = suma;
        }

    }

    //rozwiązanie bazujące na algortytmie 2-opt. Każde rozwiązanie wykonuje tylko jedno przekształcenie.
    public class TwoOptSolution
    {
        Problem problem;
        public int[] CitySequence { get; internal set; }
        public double Score { get; internal set; }

        public City[] Cities
        {
            get
            {
                return problem.Cities;
            }
        }

        public TwoOptSolution(Problem _problem, Solution _initialSolution)  //konstruktor kopiujący 
        {
            problem = _problem;
            CitySequence = new int[problem.Cities.Length];
            Array.Copy(_initialSolution.CitySequence, CitySequence, CitySequence.Length);

            CalculateScore();
        }

        public TwoOptSolution(Problem _problem, TwoOptSolution _bestSolution)   //konstruktor kopiujący
        {
            problem = _problem;
            CitySequence = new int[problem.Cities.Length];
            Array.Copy(_bestSolution.CitySequence, CitySequence, CitySequence.Length);

            ReversePath();

            CalculateScore();
        }

        private void ReversePath()  //odwraca część scieżki
        {
            int first = problem.RNG.Next(CitySequence.Length);
            int last = problem.RNG.Next(CitySequence.Length);

            int holder;
            if(first > last)
            {
                holder = first;
                first = last;
                last = holder;
            }

            Array.Reverse(CitySequence, first, last - first);

        }
        

        private void CalculateScore()   //liczy dlugość ścieżki na podstawie wektora odległości
        {
            double suma = 0;
            for (int i = 0; i < CitySequence.Length - 1; i++)
            {
                //suma += City.Distance(problem.Cities[CitySequence[i]], problem.Cities[CitySequence[i + 1]]);
                suma += problem.DistanceArray[CitySequence[i], CitySequence[i + 1]];
            }
            //suma += City.Distance(problem.Cities[CitySequence[CitySequence.Length - 1]], problem.Cities[CitySequence[0]]);
            suma += problem.DistanceArray[CitySequence[CitySequence.Length - 1], CitySequence[0]];
            Score = suma;
        }
    }

    //metropolis
    public static class Functions
    {
        public static bool Metropolis(Random _rng, double temperature, double delta)
        {
            return Math.Exp(-delta / temperature) > _rng.NextDouble();
        }
    }

}
