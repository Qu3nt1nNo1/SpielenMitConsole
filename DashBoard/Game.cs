﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace MonsterHunter
{

    struct Game
    {

        /// <summary>
        /// Stores the ConsoleKey.Key
        /// </summary>
        public static ConsoleKey choosenPlayer;

        /// <summary>
        /// Stores the key input.
        /// </summary>
        public static ConsoleKey keyPressed;

        /// <summary>
        /// Stores the Key coming from Modification Screen;
        /// </summary>
        public static ConsoleKey modInput;

        /// <summary>
        /// An instance of the PlayerModification 
        /// </summary>
        public static ScreenPlayerMod modifications = new ScreenPlayerMod();

        /// <summary>
        /// Stores, if User has selected a monster;
        /// </summary>
        public static bool choiceIsMade = false;

        /// <summary>
        /// Stores if Player is a random player
        /// </summary>
        public static bool isNotRandom = true;

        /// <summary>
        /// Stores if player is modded as boolean. 
        /// <remarks></remarks>
        /// </summary>
        public static bool isModyfied = false;


        /// <summary>
        /// Holds true to keep the main thread running;
        /// </summary>
        public static bool keepAlive = true;

        /// <summary>
        /// The enemy object when game is running
        /// </summary>
        public static Enemy enemy;

        /// <summary>
        /// The player object when game is running
        /// </summary>
        public static Player player;

        /// <summary>
        /// The winner object. We could let him dance on GameOver Screen.
        /// <remarks>Object exists, when fight is over and winner will be calculated.</remarks>
        /// </summary>
        public static Monster winner;

        /// <summary>
        /// The Random object through out our game;
        /// </summary>
        public static Random rndm = new Random();     // Make sure, we do it only once

        /// <summary>
        /// The lock object to protect console printing
        /// </summary>
        /// <example>lock (printlock){ ...the protected code }</example>
        public static object printlock = new object();

        /// <summary>
        /// An object to store the distance between player and asDancer.
        /// </summary>
        public static Distance dist = new Distance();

        /// <summary>
        /// The countdown timer object
        /// </summary>
        static Timer countdown;

        // We call this function when the timer thread callback is ready
        static AutoResetEvent autoEvent = new AutoResetEvent(true);

        /// <summary>
        /// Object to store how often Countdowntimer calls.
        /// </summary>
        static int invokeCount = 0;

        /// <summary>
        /// The maximum Countdown Time in seconds
        /// </summary>
        static int maxCount = 120;         // we want to run for given amount of seconds;

        // we store countdown here;
        // and init remaining time with maximum seconds
        /// <summary>
        /// Object to store the extant time
        /// </summary>
        static int remainTime = maxCount;

        // the string we want to print
        /// <summary>
        /// The string in the printout of the Countdown
        /// </summary>
        static String timeText;

        // this function is the callback for the timer
        /// <summary>
        /// The callback function for countdown timer
        /// </summary>
        /// <remarks>Prints the Countdown string</remarks>
        /// <param name="stateInfo">The Timer Event handle</param>
        static void PrintTime(Object stateInfo)
        {
            // yes, we can
            ++invokeCount;
            --remainTime;

            timeText = String.Format("{0,-16}{1,5} : {2}", "Time remaining",
                                        arg1: (remainTime / 60).ToString(),
                                        arg2: (remainTime % 60).ToString("D2"));

            // Clear the dashboard by printing Key.Space
            string clear = String.Format("{0,50}", " ");
            Dashboard.CenterText(2, clear);

            // print the Countdown in the center of our dashboard
            Dashboard.CenterText(2, timeText);

            // We want to send signals to waiting threads
            AutoResetEvent secondAuto = (AutoResetEvent)stateInfo;

            if (invokeCount == maxCount)    // the time has run out; 
            {
                // tell waiting threads to continue work;
                // we are done;
                secondAuto.Set();
                // Abort the Countdown Timer Thread
                KillCountdown();

                // ToDo: Funktionality when timer runs out;
                // Ex.: Check remaining stats and proclaim winner;
                // we handle this in CloseGame() ???
                CloseTheGame();

            }
        }


        /// <summary>
        /// Kill the Timer Thread when Key.L ends the game
        /// </summary>
        public static void KillCountdown()        // we clean up the thread
        {
            countdown.Dispose();
        }

        /// <summary>
        /// Start Timer Countdown event every 1000ms
        /// </summary>
        public static void StartCountdown()
        {
            try
            {
                countdown = new Timer(PrintTime, autoEvent, 2000, 1000);
                autoEvent.WaitOne();
            }
            catch (ThreadAbortException ex)
            {
                System.Diagnostics.Debug.WriteLine("Catch: StartTimer " + ex);
            }

        }

        #region Display the Statistics

        /// <summary>
        /// A place to store the player statistics (health points).
        /// </summary>
        public static Stats playerStats;    //  = new Stats();

        /// <summary>
        /// A place to store the player statistics (health points).
        /// </summary>
        public static Stats enemyStats;     //   = new Stats();

        public static void InitStats()
        {
            rounds = 0;


            if (isModyfied)
            {   // player is modified;
                // we get the mod values;
                playerStats.aPoints = Game.modifications.Attack;
                playerStats.dPoints = Game.modifications.Defense;
                playerStats.sPoints = Game.modifications.Speed;
                playerStats.hPoints = Game.player.outfit.stats.GetHPoints();
            }
            else
            {   // player has default values;
                // get values from outfit;
                playerStats.aPoints = player.outfit.stats.aPoints;
                playerStats.dPoints = player.outfit.stats.dPoints;
                playerStats.sPoints = player.outfit.stats.sPoints;
                playerStats.hPoints = player.outfit.stats.hPoints;
            }


            enemyStats.aPoints = Game.enemy.outfit.stats.aPoints;
            enemyStats.dPoints = Game.enemy.outfit.stats.dPoints;
            enemyStats.sPoints = Game.enemy.outfit.stats.sPoints;
            enemyStats.hPoints = Game.enemy.outfit.stats.GetHPoints();
        }

        public static void UpdateStats(Stats _player, Stats _enemy)
        {
            playerStats = _player;
            enemyStats = _enemy;
        }

        public static void PrintStats()
        {
            //  string.Format("{0,20}","---");
            //  string output = String.Format("Text{0,10} text{1,10}", arg1, arg2);
            //  Console.SetCursorPosition(2,1);
            //  Console.Write(output);
            String clear = String.Format("{0,20}", " ");
            int atLine = 1;
            int left = 2;
            int right = 78;
            // int health = player.outfit.stats.HPoints;
            // int otherNumber = 455;
            string playerHealth = String.Format("{0,10}{1,10}", "Health", playerStats.hPoints);
            string playerDefense = String.Format("{0,10}{1,10}", "Defense", playerStats.dPoints);

            string enemyHealth = String.Format("{0,5}{1,15}", enemyStats.hPoints, "Health");
            string enemyDefense = String.Format("{0,5}{1,15}", enemyStats.dPoints, "Defense");
            // lock (statsLock)
            // {

            lock (Game.printlock)
            {

                ConsoleColor backup = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.Green;

                Console.SetCursorPosition(left, atLine);
                Console.Write(clear);
                Console.SetCursorPosition(left, atLine);
                Console.Write(playerHealth);

                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.SetCursorPosition(left, atLine + 1);
                Console.Write(clear);
                Console.SetCursorPosition(left, atLine + 1);
                Console.Write(playerDefense);

                Console.ForegroundColor = ConsoleColor.Green;
                Console.SetCursorPosition(right, atLine);
                Console.Write(clear);
                Console.SetCursorPosition(right, atLine);
                Console.Write(enemyHealth);

                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.SetCursorPosition(right, atLine + 1);
                Console.Write(clear);
                Console.SetCursorPosition(right, atLine + 1);
                Console.Write(enemyDefense);

                Console.ForegroundColor = backup;
            }
            // }
        }

        public static void PrintStats(Stats _player, Stats _enemy)
        {
            //  string.Format("{0,20}","---");
            //  string output = String.Format("Text{0,10} text{1,10}", arg1, arg2);
            //  Console.SetCursorPosition(2,1);
            //  Console.Write(output);
            String clear = String.Format("{0,20}", " ");
            int atLine = 1;
            int left = 2;
            int right = 78;
            // int health = player.outfit.stats.HPoints;
            // int otherNumber = 455;
            string playerHealth = String.Format("{0,-10}{1,10}", "Health", _player.GetHPoints());
            string playerDefense = String.Format("{0,-10}{1,10}", "Defense", _player.dPoints);
            string playerAttack = String.Format("{0,-10}{1,10}", "Attack", _player.aPoints);

            string enemyHealth = String.Format("{0,-5}{1,15}", _enemy.GetHPoints(), "Health");
            string enemyDefense = String.Format("{0,-5}{1,15}", _enemy.dPoints, "Defense");
            string enemyAttack = String.Format("{0,-5}{1,15}", _enemy.aPoints, "Attack");

            // don't forget to update the Stats objects
            UpdateStats(_player, _enemy);

            lock (Game.printlock)
            {

                ConsoleColor backup = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.Green;

                Console.SetCursorPosition(left, atLine);
                Console.Write(clear);
                Console.SetCursorPosition(left, atLine);
                Console.Write(playerHealth);

                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.SetCursorPosition(left, atLine + 1);
                Console.Write(clear);
                Console.SetCursorPosition(left, atLine + 1);
                Console.Write(playerDefense);

                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.SetCursorPosition(left, atLine + 2);
                Console.Write(clear);
                Console.SetCursorPosition(left, atLine + 2);
                Console.Write(playerAttack);

                Console.ForegroundColor = ConsoleColor.Green;
                Console.SetCursorPosition(right, atLine);
                Console.Write(clear);
                Console.SetCursorPosition(right, atLine);
                Console.Write(enemyHealth);

                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.SetCursorPosition(right, atLine + 1);
                Console.Write(clear);
                Console.SetCursorPosition(right, atLine + 1);
                Console.Write(enemyDefense);

                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.SetCursorPosition(right, atLine + 2);
                Console.Write(clear);
                Console.SetCursorPosition(right, atLine + 2);
                Console.Write(enemyAttack);

                Console.ForegroundColor = backup;
            }
        }


        #endregion

        /// <summary>
        /// Closing the game. Display "Press any key...".
        /// <remarks>And check, who is winner</remarks>
        /// </summary>
        public static void CloseTheGame()
        {
            // stop the asDancer timer
            Enemy.enemyTimer.Dispose();
            // stop the player timer
            // check if Timer object exists
            // Timer object exists, if Program.manual = false
            if (!Program.arrowControl)
            {
                startAutoplayTimer.Dispose();

            }

            // in a close fight both can die!
            if (enemyStats.GetHPoints() <= 0 && playerStats.GetHPoints() <= 0)
            {
                winner.name = "Nobody. Everybody is DEAD!";
            }

            // who is the winner?
            if (enemyStats.GetHPoints() <= 0)
            {
                winner = player;
                // Hide the looser
                enemy.HideMonster(enemy.pos_x, enemy.pos_y);
                // Display the player again to avoid fractals
                player.PrintMonster();

                // lets dance
                // player.HideMonster(player.pos_x , player.pos_y);
                // Program.theWinner.Start();

                // DanceAsWinner(player);

            }
            else
            {
                winner = enemy as Monster;
                player.HideMonster(player.pos_x, player.pos_y);
                // Even he is hidden, we must put him aside.
                player.pos_x = 10;
                player.pos_y = 10;

                // display winner to avoid fractals
                enemy.PrintMonster();

                //Dancer dancingWinner = new Dancer();
                //dancingWinner.InitDancer(enemy.monster.outfit, "");
                //dancingWinner.asDancer.monster.pos_x = enemy.monster.pos_x;
                //dancingWinner.asDancer.monster.pos_y = enemy.monster.pos_y;

                //AutoResetEvent winnerReset = new AutoResetEvent(true);
                //Timer dancer = new Timer(dancingWinner.asDancer.monster.DanceTheMonster, winnerReset, 0, 1111);
                //winnerReset.Set();
            }


            int here = Window.y - 5 - Window.top / 2;
            lock (Game.printlock)
            {
                lock (printlock)
                {
                    // Thread.Sleep(200);
                    PrintStats(playerStats, enemyStats);
                    // Thread.Sleep(200);

                }
                Dashboard.CenterText(2, "Monster Fight took " + rounds + " rounds.", winner.outfit.designColor);
                string blanc = new string(' ', 27);
                Dashboard.CenterText(3, blanc);
                // Song.playSong = false;
                ConsoleColor red = ConsoleColor.Red;
                // Thread.Sleep(500);
                string grats = "The Winner is... " + winner.name;
                Dashboard.CenterText(here, grats, red);
                Dashboard.CenterText(++here, "Press ENTER to close game ", red);
                // Thread.Sleep(2000);
                Console.ReadLine();

            }

        }

        /// <summary>
        /// Inits a player and his asDancer
        /// </summary>
        /// <remarks>The player is random and the asDancer is different than player</remarks>
        public static void InitPlayerAndEnemy()
        {
            //  [2]...from random design
            if (!choiceIsMade)
            {
                // Implement random player monster
                int[] playerKeys = new int[] { 65, 70, 71 };
                choiceIsMade = true;
                choosenPlayer = (ConsoleKey)playerKeys[rndm.Next(0, 2)];

            }
            // switch on ConsoleKey case [A]ngry , [F]rodo , [G]oblin
            // we use constructor;
            switch (Game.choosenPlayer)
            {
                case ConsoleKey.A:
                    Game.player = new Player(theDesigns[2], theDesigns[2].designName);
                    break;
                case ConsoleKey.F:
                    Game.player = new Player(theDesigns[1], theDesigns[1].designName);
                    break;
                case ConsoleKey.G:
                    Game.player = new Player(theDesigns[0], theDesigns[0].designName);
                    break;
                default:
                    int r = rndm.Next(0, theDesigns.Length);
                    Game.player = new Player(theDesigns[r], theDesigns[r].designName);
                    break;
            }

            Console.ForegroundColor = ConsoleColor.Magenta;
            // string text = String.Format("You selected [" + choosenPlayer.ToString() + "] " + Game.player.name);
            string text = String.Format("You selected " + Game.player.name);
            Dashboard.CenterText(ConsoleColor.Red, 25, text);
            Thread.Sleep(2000);
            text = new string(' ', text.Length);
            Dashboard.CenterText(ConsoleColor.Red, 25, text);

#if DEBUG || MANUAL

            int reduction = 400;
            Game.player.outfit.stats.SetHPoints(reduction);
#endif

            Game.enemy = new Enemy(player);

            //Game.enemy.CreateEnemyFromOponent();
#if DEBUG || MANUAL

            Game.enemy.outfit.stats.SetHPoints(reduction);

#endif


            Game.enemy.PrintMonster();

            winner = new Monster();
        }

        /// <summary>
        /// Used in the while-loops to keep game running.
        /// </summary>
        public static bool play = true;

        // make a play
        // ! only cursor is moving !
        /// <summary>
        /// Start the game
        /// </summary>
        /// <remarks>Runs until Key.L is pressed</remarks>
        /// <param name="_player">The player monster</param>
        public static void PlayTheGame(Player _player)
        {
            try
            {
                /*  We receive a monster for the player with an existing design
                 */
                Game.player = _player;

                while (play)
                {
                    // we check if player is dead
                    if (Game.playerStats.GetHPoints() <= 0 || Game.player.outfit.stats.GetHPoints() <= 0)
                    {
                        // we dont want to run anymore
                        play = false;
                        // we stop the countdown
                        Game.KillCountdown();

                        // we DONT stop the asDancer, because his thread will run
                        // until asDancer is looser.
                        //StopEnemy();

                        // dont display a dead player
                        Game.player.HideMonster(Game.player.pos_x, Game.player.pos_y);

                        // leave this loop
                        Game.CloseTheGame();
                        break;

                    }
                    // player is alive
                    else
                    {
                        // we check if he is winner
                        if (Game.enemyStats.GetHPoints() <= 0)
                        {
                            // player has won;
                            // stop the clock;
                            Game.KillCountdown();
                            Game.CloseTheGame();
                            break;

                        }

                        //  we need a monster;
                        //  PrintTheMonster(pos_x, pos_y);
                        Game.player.PrintMonster(Game.player.pos_x, Game.player.pos_y);

                        ConsoleKeyInfo key;
                        key = Console.ReadKey(true);

                        // protect next steps against other threads
                        lock (printlock)
                        {
                            switch (key.Key)
                            {
                                // monster size is x = 5, y = 3
                                // min/max cursor positions are:
                                // left = 2;            // monster left is 2 pixel from middle
                                // right = x - 2 - 2;   // -2 for the monster size, -2 for the OutOfRange Exception
                                // upper = int top + 1;     // top is set when dashboard is printed;
                                // bottom = y - 1 - 1;  // -1 for the monster size, -1 for the excepiton
                                case ConsoleKey.W:
                                case ConsoleKey.UpArrow:
                                    {
                                        // if we are not at the top:
                                        // ( ! top is set when board is created !);
                                        if (Game.player.pos_y > Window.top + 1)
                                        {
                                            Game.player.HideMonster(Game.player.pos_x, Game.player.pos_y);
                                            // Console.CursorTop = Console.CursorTop - 1;
                                            Game.player.pos_y--;
                                        }
                                        break;
                                    }
                                case ConsoleKey.D:
                                case ConsoleKey.RightArrow:
                                    {
                                        // if we are not at the outer right
                                        // move right;
                                        if (Game.player.pos_x < Window.x - 4)
                                        {
                                            Game.player.HideMonster(Game.player.pos_x, Game.player.pos_y);
                                            // Console.CursorLeft += 1;
                                            Game.player.pos_x++;
                                        }
                                        break;
                                    }
                                case ConsoleKey.S:
                                case ConsoleKey.DownArrow:
                                    {
                                        // if we are not at bottom:
                                        if (Game.player.pos_y < Window.y - 3)
                                        {
                                            Game.player.HideMonster(Game.player.pos_x, Game.player.pos_y);
                                            // Console.CursorTop += 1;
                                            Game.player.pos_y++;
                                        }
                                        break;
                                    }
                                case ConsoleKey.A:
                                case ConsoleKey.LeftArrow:
                                    {
                                        // if we are not at the outer left
                                        if (Game.player.pos_x > 2)
                                        {
                                            Game.player.HideMonster(Game.player.pos_x, Game.player.pos_y);
                                            // Console.CursorLeft -= 1;
                                            Game.player.pos_x--;
                                        }
                                        break;
                                    }
                                case ConsoleKey.L:
                                    {
                                        play = false;
                                        Game.KillCountdown();
                                        // Stop the asDancer's moving
                                        Enemy.enemyTimer.Change(0, 2000);
                                        Game.CloseTheGame();
                                        break;
                                    }
                                case ConsoleKey.H:
                                case ConsoleKey.Spacebar:
                                    {
                                        // we lock this section
                                        lock (printlock)
                                        {
                                            // jump into middle of screen
                                            // player.HideMonster(player.pos_x, player.pos_y);
                                            // Center();
                                            // player.pos_x = Console.CursorLeft;
                                            // player.pos_y = Console.CursorTop;

                                            // we can fight;
                                            Game.player.Fight(Game.player);
                                            Sound.PlaySound(1, Game.player.outfit.FightSound);

                                            if (Game.dist.distance < 4)
                                            {
                                                Game.player.HitMonster(Game.playerStats, Game.enemyStats, true);

                                            }
                                        }
                                        break;
                                    }
                                default:
                                    {
                                        // Center();
                                        break;
                                    }
                            }
                        } // end of lock

                    } // end of else

                } // end of while
            }
            catch (ThreadAbortException ex)
            {

                System.Diagnostics.Debug.WriteLine("Catch: PlayTheGame " + ex);
            }


        } // end of function


        // declare 3 different types of monster...
        /// <summary>
        /// The "Goble".
        /// <remarks>The anxious monster. Medium speed, low attac, top resistant</remarks>
        /// </summary>
        public static Design goble;
        /// <summary>
        /// The "Frodo".
        /// <remarks>The ugly monster. High speed, medium attac, low resistant</remarks>
        /// </summary>
        public static Design frodo;
        /// <summary>
        /// The "Angry".
        /// <remarks>The bad monster. low speed, top attac, medium resistant</remarks>
        /// </summary>
        public static Design angry;

        // ...and declare a variable to store them
        /// <summary>
        /// An Array to store the available typ of monster.
        /// </summary>
        public static Design[] theDesigns;

        /// <summary>
        /// Init the game. 
        /// Set concole size and color.
        /// Set the layout of the monster.
        /// </summary>
        public static void InitGame()
        {
            //  set screen to default
            //  Console Settings
            Console.Clear();
            Console.WindowWidth = Window.x;
            Console.WindowHeight = Window.y;
            Console.BufferWidth = Window.x;
            Console.BufferHeight = Window.y;
            Console.BackgroundColor = ConsoleColor.Black;

            #region Monster design
            //  Monster Designs;
            /*
            |   Goble   Frodo   Angry
            |
            |   (° °)   {0.0}   [-.-]
            |
            |    ~▓~    o-▓-o    '▓'
            |    ] [     { }     U U 
            |
             */

            Game.goble = new Design
            {
                designName = "Goble",
                designColor = ConsoleColor.White,
                designElements = new string[] { "(°;°)", " ~▓~ ", " ] [ ", "O-▓-O" },
                FightSound = Sound.low,
                // top resistance: 30
                // medium speed:    888
                stats = new Stats
                {
                    hPoints = 500,
                    aPoints = 40,
                    dPoints = 30,
                    sPoints = 888,
                }
            };

            Game.frodo = new Design
            {
                designName = "Frodo",
                designColor = ConsoleColor.Yellow,
                designElements = new string[] { "{O.O}", " /▓\\ ", " { } ", "o-▓-o" },
                FightSound = Sound.mid,
                // low resistance: 10
                // high speed:  555
                stats = new Stats
                {
                    hPoints = 500,
                    aPoints = 50,
                    dPoints = 10,
                    sPoints = 555,
                }
            };

            Game.angry = new Design
            {
                designName = "Angro",
                designColor = ConsoleColor.Green,
                designElements = new string[] { "[-.-]", " '▓' ", " U U ", "0-▓-0" },
                FightSound = Sound.high,
                // medium resistance: 20
                // low speed:   1111
                stats = new Stats
                {
                    hPoints = 500,
                    aPoints = 60,
                    dPoints = 20,
                    sPoints = 1111,      // less is faster
                },
            };
            #endregion

            Game.theDesigns = new Design[] { Game.goble, Game.frodo, Game.angry };


        }


        public static Timer startAutoplayTimer;

        // static AutoResetEvent resetAutoplayTimer = new AutoResetEvent(true);

        private static int rounds;

        public static int Rounds { get => rounds; set => rounds += 1; }

        public static void StartAutoPlayerTimer(int _millis)
        {
            try
            {
                var resetAutoplayTimer = new AutoResetEvent(false);

                startAutoplayTimer = new Timer(PlayThePlayer, resetAutoplayTimer, 1000, _millis);
                // resetAutoplayTimer.WaitOne();
                resetAutoplayTimer.WaitOne();
            }
            catch (ThreadAbortException ex)
            {

                System.Diagnostics.Debug.WriteLine("Catch: StartAutoplayTimer " + ex);
            }
        }


        public static void PlayThePlayer(Object _stateInfo)
        {
            AutoResetEvent resetAutoPlayer = (AutoResetEvent)_stateInfo;

            // bool moveIsPossible = true;
            try
            {
                // System.Diagnostics.Debug.WriteLine("Player:" + System.DateTime.Now);
                // I check if one of us is dead
                if (playerStats.GetHPoints() <= 0 || enemyStats.GetHPoints() <= 0)
                {
                    // I dont want to run anymore
                    // moveIsPossible = false;
                    // startAutoplayTimer.Dispose();
                    // I stop the countdown
                    KillCountdown();

                    // I DONT stop the asDancer, because his thread will 
                    // check for his own.
                    // Enemy.StopEnemy();

                    // I check if I am a looser
                    if (playerStats.GetHPoints() <= 0)
                    {       // I am a dead player
                            // dont display a dead player
                        player.HideMonster(player.pos_x, player.pos_y);
                        // take hidden player aside
                        player.pos_x = 5;
                        player.pos_y = 10;
                        return;

                    }
                    else
                    {
                        // Enemy must be dead.
                        Enemy.StopEnemy();
                        return;
                    }

                }
                else
                {
                    // I am alive
                    //  I need a monster;

                    player.PrintMonster(player.pos_x, player.pos_y);

                    // fight first if possible, then run
                    // add 'same level check'
                    if (dist.CalcDistance_xy() < 5 && dist.CalcDistance_y() < 4)
                    {
                        lock (printlock)
                        {
                            player.Fight(player);
                            player.HitMonster(playerStats, enemyStats, true);
                        }
                    }

                    int[] nextStep = new int[2];

                    //  Done: fix strange movement
                    //  by locking the calculation;

                    // put 'nextStep...' into lock(printlock)
                    // old location

                    lock (printlock)
                    {
                        // new location
                        nextStep = Monster.GetCloser(player as Monster, enemy as Monster);

                        player.HideMonster(player.pos_x, player.pos_y);
                        player.pos_x += nextStep[0];
                        player.pos_y += nextStep[1];
                        player.PrintMonster();
                    }
                } // end of else


            } // end of try
            catch (ThreadAbortException ex)
            {
                System.Diagnostics.Debug.WriteLine("Catch: PlayTheGame " + ex);
            }

            resetAutoPlayer.Set();
        } // end of function



    }
}
