using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Navigation;
using System.Windows.Threading;

namespace SolvedokuWPF
{
    using TableArray = int[,];

    public static class Exts
    {
        public static void ScanXandYAxisAtCoord(this List<int> list, ref TableArray table, int x, int y)
        {
            // check on X and Y axis
            // scan in all directions
            for (int check = 0; check < 9; ++check)
            {
                ref int numXaxis = ref table[x, check];
                ref int numYaxis = ref table[check, y];

                if (numXaxis != 0)
                    list.Remove(numXaxis);

                if (numYaxis != 0)
                    list.Remove(numYaxis);
            }
        }

        public static void ScanInnerGridAtCoord(this List<int> list, ref int[,] table, int x, int y)
        {
            int startingNum = table[x, y];
            List<Tuple<int, int>> possibleCells = new();
            List<int> possibleNumbers = new() { 1, 2, 3, 4, 5, 6, 7, 8, 9 };

            // remove current grid numbers
            int gridStartX = (int)Math.Floor(x / 3d) * 3;
            int gridStartY = (int)Math.Floor(y / 3d) * 3;
            for (int gridX = gridStartX; gridX < gridStartX + 3; ++gridX)
            {
                for (int gridY = gridStartY; gridY < gridStartY + 3; ++gridY)
                {
                    ref int gridXY = ref table[gridX, gridY];
                    if (gridXY == 0)
                    {
                        possibleCells.Add(new(gridX, gridY));
                        continue;
                    }

                    possibleNumbers.Remove(gridXY);
                }
            }

            var numbersCopy = new List<int>(possibleNumbers);
            foreach (var num in numbersCopy)
            {
                var cellsCopy = new List<Tuple<int, int>>(possibleCells);
                var cellsCopyCopy = new List<Tuple<int, int>>(cellsCopy);
                foreach (var coord in cellsCopyCopy)
                {
                    // check on X and Y axis
                    for (int check = 0; check < 9; ++check)
                    {
                        ref int numXaxis = ref table[coord.Item1, check];
                        ref int numYaxis = ref table[check, coord.Item2];

                        if ((numXaxis == 0 || num != numXaxis) && (numYaxis == 0 || num != numYaxis))
                            continue;

                        // if we hit a number that's from our possible list, remove it from current possible cell
                        cellsCopy.Remove(coord);

                        if (cellsCopy.Count <= 1)
                            break;
                    }

                    if (cellsCopy.Count <= 1)
                        break;
                }

                // only one cell remaining that could fit the number currently in loop
                if (cellsCopy.Count == 1)
                {
                    if (cellsCopy.ElementAt(0) is Tuple<int, int> tuple && tuple.Item1 == x && tuple.Item2 == y)
                    {
                        list.Clear();
                        list.Add(num);
                        return;
                    }
                }
            }

            return;
        }
    
        public static Tuple<int, int> GiveCoord(this List<int> list, ref int[,] table, int x, int y)
        {
            int startingNum = table[x, y];
            List<Tuple<int, int>> possibleCells = new();
            List<int> possibleNumbers = new() { 1, 2, 3, 4, 5, 6, 7, 8, 9 };

            // remove current grid numbers
            int gridStartX = (int)Math.Floor(x / 3d) * 3;
            int gridStartY = (int)Math.Floor(y / 3d) * 3;
            for (int gridX = gridStartX; gridX < gridStartX + 3; ++gridX)
            {
                for (int gridY = gridStartY; gridY < gridStartY + 3; ++gridY)
                {
                    ref int gridXY = ref table[gridX, gridY];
                    if (gridXY == 0)
                    {
                        possibleCells.Add(new(gridX, gridY));
                        continue;
                    }

                    possibleNumbers.Remove(gridXY);
                }
            }

            var numbersCopy = new List<int>(possibleNumbers);
            foreach (var num in numbersCopy)
            {
                var cellsCopy = new List<Tuple<int, int>>(possibleCells);
                var cellsCopyCopy = new List<Tuple<int, int>>(cellsCopy);
                foreach (var coord in cellsCopyCopy)
                {
                    // check on X and Y axis
                    for (int check = 0; check < 9; ++check)
                    {
                        ref int numXaxis = ref table[coord.Item1, check];
                        ref int numYaxis = ref table[check, coord.Item2];

                        if ((numXaxis == 0 || num != numXaxis) && (numYaxis == 0 || num != numYaxis))
                            continue;

                        // if we hit a number that's from our possible list, remove it from current possible cell
                        cellsCopy.Remove(coord);

                        if (cellsCopy.Count <= 1)
                            break;
                    }

                    if (cellsCopy.Count <= 1)
                        break;
                }

                // only one cell remaining that could fit the number currently in loop
                if (cellsCopy.Count == 1)
                {
                    if (cellsCopy.ElementAt(0) is Tuple<int, int> tuple/* && tuple.Item1 == x && tuple.Item2 == y*/)
                    {
                        list.Clear();
                        list.Add(num);
                        return tuple;
                    }
                }
            }

            return null;
        }
    }

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private TableArray table =
        {
                    { 5, 3, 0, 0, 7, 0, 0, 0, 0 },
            { 6, 0, 0, 1, 9, 5, 0, 0, 0 },
            { 0, 9, 8, 0, 0, 0, 0, 6, 0 },
            { 8, 0, 0, 0, 6, 0, 0, 0, 3 },
            { 4, 0, 0, 8, 0, 3, 0, 0, 1 },
            { 7, 0, 0, 0, 2, 0, 0, 0, 6 },
            { 0, 6, 0, 0, 0, 0, 2, 8, 0 },
            { 0, 0, 0, 4, 1, 9, 0, 0, 5 },
            { 0, 0, 0, 0, 8, 0, 0, 7, 9 },/*
            { 0, 0, 0, 8, 0, 0, 0, 4, 0 },
            { 8, 0, 0, 3, 4, 0, 0, 2, 0 },
            { 4, 0, 0, 0, 0, 6, 1, 0, 8 },
            { 6, 8, 0, 0, 0, 0, 9, 0, 4 },
            { 7, 0, 0, 5, 0, 0, 0, 0, 1 },
            { 0, 0, 9, 7, 0, 0, 0, 0, 0 },
            { 0, 0, 8, 0, 0, 0, 0, 0, 9 },
            { 0, 4, 2, 0, 0, 5, 0, 8, 6 },
            { 0, 9, 0, 0, 0, 3, 4, 5, 0 },*/
        };

        public ObservableCollection<Button> Buttons { get; set; }
        private Tuple<int, int>? ovrwCell = null;
        private LogWindow logWindow;

        public void GridClick(object sender, RoutedEventArgs e)
        {
            if (!(sender is Button button) || !(button.Tag is Tuple<int, int> tag))
                return;

            //if (!Int32.TryParse(button.Content as string, out int x) || x != 0)
            //    return;

            ovrwCell = tag;
            button.FontWeight = FontWeights.Bold;

            Style style = new Style();
            style.Setters.Add(new Setter(BackgroundProperty, new SolidColorBrush(Colors.LightGoldenrodYellow)));
            button.Style = style;
        }
        
        public void ClearClick(object sender, RoutedEventArgs e)
        {
            for (int x = 0; x < 9; ++x)
            {
                for (int y = 0; y < 9; ++y)
                {
                    SetCellToValue(x, y, 0);
                }
            }
        }
        
        public void SetCellToValue(int x, int y, int value)
        {
            table[x, y] = value;
            this.Dispatcher.Invoke(() =>
            {
                if (SudokuGrid.Children.Cast<UIElement>().First(pred => Grid.GetRow(pred) == x && Grid.GetColumn(pred) == y) is Button xybutton)
                {
                    Style style = new Style();
                    style.Setters.Add(new Setter(BackgroundProperty, new SolidColorBrush(Colors.Gray)));
                    xybutton.Style = style;
                    xybutton.Content = $"{value}";
                }
            });
        }

        public bool Turn()
        {
            bool done = false;
            bool breakLoop = false;
            for (int x = 0; x < 9; ++x)
            {
                for (int y = 0; y < 9; ++y)
                {
                    if (table[x, y] != 0)
                        continue;

                    done = false;
                    List<int> possibleNumbers = new() { 1, 2, 3, 4, 5, 6, 7, 8, 9 };

                    /*// check on X and Y axis
                    // scan in all directions
                    for (int check = 0; check < 9; ++check)
                    {
                        ref int numXaxis = ref table[x, check];
                        ref int numYaxis = ref table[check, y];

                        if (numXaxis != 0)
                            possibleNumbers.Remove(numXaxis);

                        if (numYaxis != 0)
                            possibleNumbers.Remove(numYaxis);
                    }*/

                    // scan all empty boxes for in grid for remaining numbers
                    //if (possibleNumbers.Count != 1)
                    {
                    }

                    //if ((x + 1) % 3 == 0 || (y + 1) % 3 == 0)
                    {
                        var coord = possibleNumbers.GiveCoord(ref table, x, y);
                        if (coord != null)
                        {
                            SetCellToValue(coord.Item1, coord.Item2, possibleNumbers.ElementAt(0));
                            Thread.Sleep(150);
                            //PrintTable(ref table);
                            Dispatcher.Invoke(() =>
                            {
                                logWindow.LogsContainer.Insert(logWindow.LogsContainer.Count, $"Found number {possibleNumbers.ElementAt(0)}; X {coord.Item1 + 1} Y {coord.Item2 + 1}");
                            });

                            breakLoop = true;
                            break;
                            //Console.WriteLine($"Found number {possibleNumbers.ElementAt(0)}; X {x + 1} Y {y + 1}");
                        }
                    }
                }

                if (breakLoop)
                    break;
            }

            return done;
        }

        public void SolveClick(object sender, RoutedEventArgs e)
        {
            if (!(sender is Button button))
                return;

            new Thread(() =>
            {
                TableArray smallGrid = new int[3, 3];

                Stopwatch timer = new();
                timer.Start();

                while (true)
                {
                    if (Turn())
                        break;

                    if (timer.ElapsedMilliseconds > 10000)
                    {
                        this.Dispatcher.Invoke(() =>
                        {
                            MessageBox.Show("The program took more than 10 seconds while attemting to solve the puzzle.", "Could not solve.");
                            button.Content = "Solve";
                            button.IsEnabled = true;
                        });
                        return;
                    }
                    //Thread.Sleep(500);
                }

                //timer.Stop();
                //Console.WriteLine($"Solved in {timer}.");
                //Console.ReadLine();

                this.Dispatcher.Invoke(() =>
                {
                    button.Content = "Done";
                    button.IsEnabled = false;
                });
            }).Start();

            button.Content = "Solving..";
        }

        public MainWindow()
        {
            InitializeComponent();

            for (int x = 0; x < 9; ++x)
            {
                SudokuGrid.RowDefinitions.Add(new RowDefinition());
                SudokuGrid.ColumnDefinitions.Add(new ColumnDefinition());

                for (int y = 0; y < 9; ++y)
                {
                    ref var xyVal = ref table[x, y];

                    var button = new Button();
                    button.Content = $"{xyVal}";
                    button.Tag = new Tuple<int, int>(x, y);

                    button.VerticalAlignment = VerticalAlignment.Stretch;
                    button.HorizontalAlignment = HorizontalAlignment.Stretch;

                    button.Click += new RoutedEventHandler(GridClick);

                    Grid.SetColumn(button, y);
                    Grid.SetRow(button, x);
                    SudokuGrid.Children.Add(button);
                }
            }

            logWindow = new();
            logWindow.Show();
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            var tagCopy = ovrwCell;
            ovrwCell = null;

            if (tagCopy == null)
                return;

            if (e.Key < Key.D1 || e.Key > Key.D9)
                return;

            var value = e.Key - Key.D0;
            SetCellToValue(tagCopy.Item1, tagCopy.Item2, value);
            tagCopy = null;
        }

        private void NextClick(object sender, RoutedEventArgs e)
        {
            if (!(sender is Button button))
                return;

            new Thread(() =>
            {
                TableArray smallGrid = new int[3, 3];

                Stopwatch timer = new();
                timer.Start();

                //while (true)
                {
                    Turn();

                    /*if (timer.ElapsedMilliseconds > 10000)
                    {
                        this.Dispatcher.Invoke(() =>
                        {
                            MessageBox.Show("The program took more than 10 seconds while attemting to solve the puzzle.", "Could not advance.");
                            button.Content = "Solve";
                            button.IsEnabled = true;
                        });
                        return;
                    }*/
                }

                this.Dispatcher.Invoke(() =>
                {
                    button.Content = "Next";
                    button.IsEnabled = true;
                });
            }).Start();

            button.Content = "Solving..";
        }
    }
}
