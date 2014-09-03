using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using UsbUirt;
using System.Threading;
using System.IO.Ports;
using Edith.Core;


namespace Edith.Modules.FirstRemote
{
    /// <summary>
    /// Interaction logic for FirstRemote.xaml
    /// Test for Git
    /// </summary>
    public partial class FirstRemote : UserControl, IModule
    {
        public string codigo;
        private string ligar;
        public string desligar;
        public string aumvol;
        public string dimivol;
        public string aumca;
        public string dimica;
        public string mute;
        Controller mc = new Controller();
        public static string irCode = "0000 006B 0000 0022 00AB 00AE 0016 0041 0016 0041 0016 0041 0016 0016 0016 0016 0016 0016 0016 0016 0016 0016 0016 0041 0016 0041 0016 0041 0016 0016 0016 0016 0016 0016 0016 0016 0016 0016 0016 0041 0016 0016 0016 0016 0016 0041 0016 0041 0016 0016 0016 0016 0016 0041 0016 0016 0016 0041 0016 0041 0016 0016 0016 0016 0016 0041 0016 0041 0016 0016 0016 0703";
       
        private Mediator mediator;
        private int currentSelectedButtonIndex = -1;
        private bool isRunning = false;
        public event EventHandler Closed;
        private SolidColorBrush selectedBrush = (SolidColorBrush)Edith.App.Current.FindResource("IcsDarkOnFocus");
        private SolidColorBrush unselectedBorder = (SolidColorBrush)Edith.App.Current.FindResource("IcsDarkButtonBorderOffFocus");
        private SolidColorBrush unselectedBrush = new SolidColorBrush(Colors.Transparent);

        public FirstRemote()
        {
            InitializeComponent();
        }

        private static CodeFormat transmitFormat = CodeFormat.Pronto;
        private static LearnCompletedEventArgs learnCompletedEventArgs = null; ///é anulado o Evento LearnCompleted
        private static void mc_TransmitCompleted(object sender, TransmitCompletedEventArgs e)
    {
            ManualResetEvent waitEvent = e.UserState as ManualResetEvent;
            waitEvent.Set();
        
    }


     void LearnCompletedEvent(object sender, LearnCompletedEventArgs e)
        {
            System.Diagnostics.Debugger.Break();
        }

        private static void TestLearn(Controller mc, CodeFormat learnFormat, LearnCodeModifier learnCodeModifier)
        {
            learnCompletedEventArgs = null;
      //      Console.WriteLine("<Press x to abort Learn>");
            mc.Learning += new UsbUirt.Controller.LearningEventHandler(mc_Learning);
            mc.LearnCompleted += new UsbUirt.Controller.LearnCompletedEventHandler(mc_LearnCompleted);
            
            try
            {
                try
                {
                   mc.Learn(learnFormat, learnCodeModifier, 0, new TimeSpan(0, 0, 10));
                 //   mc.LearnAsync(learnFormat, learnCodeModifier, learnCompletedEventArgs);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("*** ERROR calling LearnAsync! ***");
                    throw;
                }


                   //    while (learnCompletedEventArgs == null)
                   //{

                //string numero= ;
                // string irCode = ;

              //  int ret = numero.CompareTo(irCode);

                //string s = Console.ReadLine();
                //try
                //{

                //}
                //catch (Exception ex)
                //{

                //    throw ex;
                //}
                //if (s.Length != 0 && s[0] == 'x')
                //{

                //    if (learnCompletedEventArgs == null)
                //    {
                //        Console.WriteLine("Calling LearnAsyncCancel...");
                //        mc.LearnAsyncCancel(learnCompletedEventArgs);
                //        Thread.Sleep(1000);
                //        break;
                //    }

                //    else
                //    {
                //        Console.WriteLine("<Press x to abort Learn>");
                //    }
                // }

                 //}

                //if (string.IsNullOrEmpty(irCode))
                //{
                //    Console.WriteLine("String nula ou vazia");
                //}

                if (learnCompletedEventArgs != null && learnCompletedEventArgs.Cancelled == false && learnCompletedEventArgs.Error == null)
                {
                   
                    
                        irCode = learnCompletedEventArgs.Code;
                        Console.WriteLine("...Done...IRCode = {0}", irCode);
                        transmitFormat = learnFormat;
                  
                }

            }
            finally
            {
                mc.Learning -= new UsbUirt.Controller.LearningEventHandler(mc_Learning);
                mc.LearnCompleted -= new UsbUirt.Controller.LearnCompletedEventHandler(mc_LearnCompleted);
            }
           
        

        }

        public static void mc_Learning(object sender, LearningEventArgs e)
        {
            Console.WriteLine("Learning: {0}% freq={1} quality={2}", e.Progress, e.CarrierFrequency, e.SignalQuality);
        }

        private static void mc_LearnCompleted(object sender, LearnCompletedEventArgs e)
        {
            learnCompletedEventArgs = e;
            Console.WriteLine("Learn complete. Press return to continue.");
        }

        private static void mc_Received(object sender, ReceivedEventArgs e)
        {
            Console.WriteLine("Aqui/11111");
            Console.WriteLine("Received: {0}", e.IRCode);
        }


        public void Start(Mediator mediator)
        {
            this.mediator = mediator;
            mediator.Tick += new EventHandler(mediator_Tick);
            mediator.Click += new EventHandler(mediator_Click);
            isRunning = true;
        }

        public void Stop()
        {
            mediator.Tick -= mediator_Tick;
            mediator.Click -= mediator_Click;
            isRunning = false;
        }

        public UIElement Element
        {
            get { return this; }
        }

        public ModuleType ModuleType
        {
            get 
            { 
                return Core.ModuleType.ContentModule; 
            }
        }

        public string ModuleName
        {
            get 
            {
                return Edith.Modules.FirstRemote.Resources.ModuleName; 
            }
        }

        public void GetMessage(string ID, object o)
        {
            //throw new NotImplementedException();
        }

        void mediator_Click(object sender, EventArgs e)
        {

            if (!isRunning)
                return;
            isRunning = false;
            switch (currentSelectedButtonIndex)
            {
                case -1:
                case 0:
                    isRunning = false;
                    Log.Data(DateTime.Now.ToString() + ":" + DateTime.Now.Millisecond.ToString() + "|Remote|Exit");
                    mediator.Tick -= mediator_Tick;
                    mediator.Click -= mediator_Click;
                    if (this.Closed != null)
                        Closed(this, new EventArgs());
                    return;
                case 1:

                    TestLearn(mc, CodeFormat.Pronto, LearnCodeModifier.None);
                    ligar = irCode;
                    Console.WriteLine("Aqui: " + ligar);
                //    SoundPlayer BellPlayer = new SoundPlayer(Settings.Default.PathBellsound);
                //    BellPlayer.Play();
                //    Log.Data(DateTime.Now.ToString() + ":" + DateTime.Now.Millisecond.ToString() + "|Caller|BellPlay");
                   break;
                case 2:

                    using (ManualResetEvent waitEvent = new ManualResetEvent(false))
            {
                mc.TransmitCompleted += new UsbUirt.Controller.TransmitCompletedEventHandler(mc_TransmitCompleted);
                
                try
                {
                    mc.TransmitAsync(ligar, transmitFormat, 10, TimeSpan.Zero, waitEvent);
                    Console.WriteLine("...Returned from call...");
                    if (waitEvent.WaitOne(5000, false))
                    {
                        Console.WriteLine("...IR Transmission Complete!");
                    }
                    else
                    {
                        Console.WriteLine("*** ERROR: Timeout error waiting for IR to finish!");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("*** ERROR calling TransmitAsync! ***");
                    throw;
                }
                finally
                {
                    mc.TransmitCompleted -= new UsbUirt.Controller.TransmitCompletedEventHandler(mc_TransmitCompleted);
                }
            }
                //    SoundPlayer StaffPlayer = new SoundPlayer(Settings.Default.PathStaffSound);
                //    StaffPlayer.Play();
                //    Log.Data(DateTime.Now.ToString() + ":" + DateTime.Now.Millisecond.ToString() + "|Caller|CallStaff");
                    break;
            }
            isRunning = true;
        }

        void mediator_Tick(object sender, EventArgs e)
        {
            if (!isRunning)
                return;
            currentSelectedButtonIndex++;
            if (currentSelectedButtonIndex == 7)
                currentSelectedButtonIndex = 0;
            switch (currentSelectedButtonIndex)
            {
                case 0:
                    B1.Background = B2.Background = B3.Background = B4.Background = B5.Background = B6.Background = B7.Background = unselectedBrush;
                    B1.BorderBrush = B2.BorderBrush = B3.BorderBrush = B4.BorderBrush = B5.BorderBrush = B6.BorderBrush = B7.BorderBrush = unselectedBorder;
                    B1.Background = selectedBrush;
                    B1.BorderBrush = selectedBrush;
                    break;
                case 1:
                    B1.Background = B2.Background = B3.Background = B4.Background = B5.Background = B6.Background = B7.Background = unselectedBrush;
                    B1.BorderBrush = B2.BorderBrush = B3.BorderBrush = B4.BorderBrush = B5.BorderBrush = B6.BorderBrush = B7.BorderBrush = unselectedBorder;
                    B2.Background = selectedBrush;
                    B2.BorderBrush = selectedBrush;
                    break;
                case 2:
                    B1.Background = B2.Background = B3.Background = B4.Background = B5.Background = B6.Background = B7.Background = unselectedBrush;
                    B1.BorderBrush = B2.BorderBrush = B3.BorderBrush = B4.BorderBrush = B5.BorderBrush = B6.BorderBrush = B7.BorderBrush = unselectedBorder;
                    B3.Background = selectedBrush;
                    B3.BorderBrush = selectedBrush;
                    break;
                case 3:
                    B1.Background = B2.Background = B3.Background = B4.Background = B5.Background = B6.Background = B7.Background = unselectedBrush;
                    B1.BorderBrush = B2.BorderBrush = B3.BorderBrush = B4.BorderBrush = B5.BorderBrush = B6.BorderBrush = B7.BorderBrush = unselectedBorder;
                    B4.Background = selectedBrush;
                    B4.BorderBrush = selectedBrush;
                    break;
                case 4:
                    B1.Background = B2.Background = B3.Background = B4.Background = B5.Background = B6.Background = B7.Background = unselectedBrush;
                    B1.BorderBrush = B2.BorderBrush = B3.BorderBrush = B4.BorderBrush = B5.BorderBrush = B6.BorderBrush = B7.BorderBrush = unselectedBorder;
                    B5.Background = selectedBrush;
                    B5.BorderBrush = selectedBrush;
                    break;
                case 5:
                    B1.Background = B2.Background = B3.Background = B4.Background = B5.Background = B6.Background = B7.Background = unselectedBrush;
                    B1.BorderBrush = B2.BorderBrush = B3.BorderBrush = B4.BorderBrush = B5.BorderBrush = B6.BorderBrush = B7.BorderBrush = unselectedBorder;
                    B6.Background = selectedBrush;
                    B6.BorderBrush = selectedBrush;
                    break;
                case 6:
                    B1.Background = B2.Background = B3.Background = B4.Background = B5.Background = B6.Background = B7.Background = unselectedBrush;
                    B1.BorderBrush = B2.BorderBrush = B3.BorderBrush = B4.BorderBrush = B5.BorderBrush = B6.Background = B7.BorderBrush = unselectedBorder;
                    B7.Background = selectedBrush;
                    B7.BorderBrush = selectedBrush;
                    break;
            }

        }

        //private void Image_ImageFailed(object sender, ExceptionRoutedEventArgs e)
        //{

        //}

       
    }
}
