﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Threading;

namespace MonsterHunter
{
    struct TextOnScreen
    {
        /// <summary>
        /// Path to the file.txt
        /// </summary>
        private static string path = "TextFiles/Welcome.txt";

        /// <summary>
        /// A place to store the result of the FileReader.
        /// </summary>
        private static string[] fileContent;

        /// <summary>
        /// Variable to store the maximum width of the ASCII text.
        /// </summary>
        private static int size = 0;

        /// <summary>
        /// Holds the symbols to be printed instead of spaces in ASCII
        /// </summary>
        private static char[] fill = new char[] { '.', ':', ',', ' ', ' ' };

        /// <summary>
        /// The list with all lines
        /// </summary>
        private static List<string> lines;

        /// <summary>
        /// The path/to/read/from/text.txt
        /// </summary>
        public string Path { get => path; set => path = value; }

        /// <summary>
        /// The symbols to fill the spaces between ASCII letters.
        /// </summary>
        public static char[] Fill { get => fill; set => fill = value; }

        /// <summary>
        /// The List object holds the lines coming from the FileReader
        /// </summary>
        public static List<string> Lines { get => lines; }

        /// <summary>
        /// Fills the Lines object with the strings from a file
        /// <remarks>
        /// Fills the lines marked with '-' in Welcome.txt with spaces
        /// </remarks>
        /// </summary>
        public static void FillTheList()
        {
            fileContent = File.ReadAllLines(path);
            lines = new List<string>();

            size = 0;
            foreach (string line in fileContent)
            {
                if (size < line.Length)
                {
                    size = line.Length;
                }

                if (line.StartsWith("-"))
                {
                    lines.Add(String.Format("{0," + size + "}", " "));
                }
                else
                {
                    lines.Add(line);
                }
            }
        }

        /// <summary>
        /// Fills the list with content from given path/to/file.txt
        /// </summary>
        /// <param name="_path">the path/to/file.txt</param>
        public static void FillTheList(string _path)
        {
            path = _path;
            FillTheList();
        }
        /// <summary>
        /// Fills the lines marked with '-' with spaces.
        /// </summary>
        /// <param name="_welcome">The text from a file</param>
        public void FillTheList(string[] _welcome)
        {
            fileContent = _welcome;
            FillTheList();

        }

        /// <summary>
        /// Print the text in the middle of the screen.
        /// </summary>
        /// <param name="_x">x-size of the screen</param>
        /// <param name="_y">size of the screeny-</param>
        public static void PrintColorBackground(int _x, int _y)
        {
            char storage = '#';

            // make some color on the screen
            for (int i = 0; i < _x; i++)
            {
                for (int j = 0; j < _y - 1; j++)
                {
                    Symbol symbol = new Symbol();
                    Console.SetCursorPosition(i, j);
                    Console.ForegroundColor = (ConsoleColor)Game.rndm.Next(1, 7);
                    storage = Fill[Game.rndm.Next(0, 4)];
                    Console.Write(storage);
                    symbol.Sign = storage;
                    symbol.Color = (int)Console.ForegroundColor;
                    Background.signs[i, j] = symbol;

                }
            }


        }


        /// <summary>
        /// Print the ASCII text at given randomPosition
        /// <remarks>...fills the spaces in text strings with random symbol
        /// and centers the text.</remarks>
        /// </summary>
        /// <param name="_lines">the lines to print</param>
        /// <param name="_x">the center of the x-axis</param>
        /// <param name="_y">the center of the y-axis</param>
        public static void PrintASCII(List<string> _lines, int _x, int _y)
        {
            Fill = new char[] { '.', ':', ',', ' ', ' ' };
            int new_x = (_x - size) / 2;
            // all line in field - ( welcome lines / 2 )
            int new_y = (_y - Window.top - Lines.Count) / 2;

            int x = new_x;
            int y = new_y;

            // print the ASCII text
            foreach (string line in _lines)
            {
                char[] symbols = new char[line.Length];
                symbols = line.ToCharArray();

                foreach (char symbol in symbols)
                {
                    Console.SetCursorPosition(x++, y);
                    Console.ForegroundColor = (ConsoleColor)Game.rndm.Next(1, 7);

                    if (symbol == ' ')
                    {
                        Console.Write(Fill[Game.rndm.Next(0, 5)]);
                    }
                    else
                    {
                        ConsoleColor store = Console.ForegroundColor;
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.Write(symbol);
                        Console.ForegroundColor = store;
                    }
                }
                x = new_x;
                y++;
            }

        }

        /// <summary>
        /// Prints the content of Welcome.txt
        /// </summary>
        public static void PrintStart()
        {
            FillTheList();
            PrintColorBackground(Window.x, Window.y);
            PrintASCII(Lines, Window.x, Window.y);
        }

        /// <summary>
        /// Prints the content of file at given path
        /// </summary>
        /// <param name="_path">path/to/file.txt</param>
        public static void PrintText(string _path)
        {
            PrintText(_path, "");
        }

        /// <summary>
        /// Prints the content of file at given path
        /// </summary>
        /// <param name="_path">path/to/file.txt</param>
        /// <param name="_text">additional text</param>
        public static void PrintText(string _path, string _text)
        {
            FillTheList(_path);
            PrintColorBackground(Window.x, Window.y);
            PrintASCII(lines, Window.x, Window.y);

        }

        /// <summary>
        /// Prints the content of the file
        /// </summary>
        /// <param name="_path">path/to/file.txt</param>
        /// <param name="_text">additional text</param>
        /// <param name="_all">true for all monster dancing</param>
        /// <returns>Timer[] with timer object of each dancing monster</returns>
        public static Timer[] PrintText(string _path, string _text, bool _all)
        {
            FillTheList(_path);
            //Console.Clear();
            PrintColorBackground(Window.x, Window.y);
            PrintASCII(lines, Window.x, Window.y);

            if (_all)
            {
                // create three asDancer;
                Dancer dancer_1 = new Dancer();
                Dancer dancer_2 = new Dancer();
                Dancer dancer_3 = new Dancer();

                dancer_1.InitDancer(Game.goble, "[ G ]");
                dancer_2.InitDancer(Game.angry, "[ A ]");
                dancer_3.InitDancer(Game.frodo, "[ F ]");

                dancer_1.asDancer.pos_x = 19;
                dancer_2.asDancer.pos_x = 50;
                dancer_3.asDancer.pos_x = 75;

                dancer_1.asDancer.pos_y = 10;
                dancer_2.asDancer.pos_y = 10;
                dancer_3.asDancer.pos_y = 10;

                AutoResetEvent danceReset = new AutoResetEvent(true);
                var dance = new Timer(dancer_1.asDancer.DanceTheMonster, danceReset, 330, 2117);
                danceReset.Set();

                AutoResetEvent danceReset2 = new AutoResetEvent(true);
                var dance2 = new Timer(dancer_2.asDancer.DanceTheMonster, danceReset2, 660, 2357);
                danceReset2.Set();

                AutoResetEvent danceReset3 = new AutoResetEvent(true);
                var dance3 = new Timer(dancer_3.asDancer.DanceTheMonster, danceReset3, 990, 2579);
                danceReset3.Set();

                return new Timer[] { dance, dance2, dance3 };

            }
            else
            {
                return new Timer[0];
            }

        }


    }
}
