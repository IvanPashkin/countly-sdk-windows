﻿using CountlySDK;
using CountlySDK.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace CountlySample
{
    class Program
    {
        const String serverURL = "http://try.count.ly";//put your server URL here
        const String appKey = null;//put your server APP key here       
        const bool enableDebugOpptions = true;
        const bool enableDebugOpptions = false;
        public int threadIterations = 100;
        int threadWaitStart = 100;
        int threadWaitEnd = 1000;
        int threadCount = 30;

        static void Main(string[] args)
        {           
            new Program().Run();
        }

        public void Run()
        {
            System.Console.WriteLine("Hello to the Countly sample console program");
            System.Console.WriteLine("DeviceID: " + Device.DeviceId);

            if (serverURL == null || appKey == null)
            {
                System.Console.WriteLine("");
                System.Console.WriteLine("Problem encountered, you have not set up either the serverURL or the appKey");
                System.Console.ReadKey();
                return;
            }

            Countly.IsLoggingEnabled = true;
            Countly.StartSession(serverURL, appKey, "1.234");

            System.Console.WriteLine("DeviceID: " + Device.DeviceId);

            while (true)
            {
                System.Console.WriteLine("");
                System.Console.WriteLine("Choose your option:");
                System.Console.WriteLine("1) Sample event");
                System.Console.WriteLine("2) Sample caught exception");
                System.Console.WriteLine("3) Change deviceID to a random value (create new user)");
                System.Console.WriteLine("4) Change the name of the current user");
                System.Console.WriteLine("5) Exit");

                if (enableDebugOpptions)
                {
                    System.Console.WriteLine("8) (debug) Threading test");
                }
                

                ConsoleKeyInfo cki = System.Console.ReadKey();
                System.Console.WriteLine("");

                if (cki.Key == ConsoleKey.D1)
                {
                    System.Console.WriteLine("1");
                    Countly.RecordEvent("Some event");
                }
                else if (cki.Key == ConsoleKey.D2)
                {
                    System.Console.WriteLine("2");

                    try
                    {
                        throw new Exception("This is some bad exception 3");
                    }
                    catch (Exception ex)
                    {
                        Countly.RecordException(ex.Message, ex.StackTrace);
                    }
                }
                else if (cki.Key == ConsoleKey.D3)
                {
                    System.Console.WriteLine("3");
                    Device.DeviceId = "ID-" + (new Random()).Next();
                }
                else if (cki.Key == ConsoleKey.D4)
                {
                    System.Console.WriteLine("4");
                    Countly.UserDetails.Name = "Some Username " + (new Random()).Next();
                }
                else if (cki.Key == ConsoleKey.D5)
                {
                    System.Console.WriteLine("5");
                    break;
                } else if (enableDebugOpptions && cki.Key == ConsoleKey.D8)
                {
                    System.Console.WriteLine("8");
                    System.Console.WriteLine("Running threaded debug test");
                    ThreadTest();
                }
                else
                {
                    System.Console.WriteLine("Wrong input, please try again.");
                }
            };

            Countly.EndSession();
        }

        

        void ThreadTest()
        {
            List<Thread> threads = new List<Thread>();

            for(int a = 0; a< threadCount; a++)
            {
                threads.Add(new Thread(new ThreadStart(ThreadWorkEvents)));
                //threads.Add(new Thread(new ThreadStart(ThreadWorkExceptions)));
            }

           
            for(int a = 0; a < threads.Count; a++)
            {
                threads[a].Start();
            }

            for (int a = 0; a < threads.Count; a++)
            {
                threads[a].Join();
            }

            System.Console.WriteLine("Threading test is over.");
        }

        void ThreadWorkEvents()
        {
            String[] eventKeys = new string[] { "key_1", "key_2", "key_3", "key_4", "key_5", "key_6" };

            for(int a = 0; a < threadIterations; a++)
            {
                int choice = a % 5;

                switch (choice)
                {
                    case 0:
                        Countly.RecordEvent(eventKeys[0]);
                        break;
                    case 1:
                        Countly.RecordEvent(eventKeys[1], 3);
                        break;
                    case 2:
                        Countly.RecordEvent(eventKeys[2], 3, 4);
                        break;
                    case 3:
                        Segmentation segm = new Segmentation();
                        segm.Add("foo", "bar");
                        segm.Add("anti", "dote");
                        Countly.RecordEvent(eventKeys[3], 3, segm);
                        break;
                    case 4:
                        Segmentation segm2 = new Segmentation();
                        segm2.Add("what", "is");
                        segm2.Add("world", "ending");
                        Countly.RecordEvent(eventKeys[4], 3, 4.3, segm2);
                        Countly.RecordEvent(eventKeys[5], 2, 5.3, segm2);
                        break;
                    default:
                        break;
                }

                Thread.Sleep((new Random()).Next(threadWaitStart, threadWaitEnd));
            }
        }      

        void ThreadWorkExceptions()
        {
            Exception exToUse;
            try
            {
                throw new Exception("This is some bad exception 35454");
            }
            catch (Exception ex)
            {
                exToUse = ex;
            }

            Dictionary<String, String> dict = new Dictionary<string, string>();
            dict.Add("booh", "waah");


            for (int a = 0; a < threadIterations; a++)
            {
                int choice = a % 4;

                switch (choice)
                {
                    case 0:
                        Countly.RecordException("Big error 1");
                        break;
                    case 1:                       
                        Countly.RecordException(exToUse.Message, exToUse.StackTrace);                   
                        break;
                    case 2:
                        Countly.RecordException(exToUse.Message, exToUse.StackTrace, dict);
                        break;
                    case 3:
                        Countly.RecordException(exToUse.Message, exToUse.StackTrace, dict, false);
                        break;
                    default:
                        break;
                }

                Thread.Sleep((new Random()).Next(threadWaitStart, threadWaitEnd));
            }         
        }
    }
}
