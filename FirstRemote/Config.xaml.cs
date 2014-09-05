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
using System.Collections.IEnumerable;

using UsbUirt;
using System.Threading;
using System.IO.Ports;

namespace Edith.Modules.FirstRemote
{
    /// <summary>
    /// Interaction logic for Config.xaml
    /// </summary>
    public partial class Config : UserControl
    {
        public event EventHandler HasChanged;
        private bool hasChanged = false;

        public string codigo;
        public string ligar;
        public string desligar;
        public string aumvol;
        public string dimivol;
        public string aumca;
        public string dimica;
        public string mute;
        Controller mc = new Controller();
        public static string irCode = "0000 006B 0000 0022 00AB 00AE 0016 0041 0016 0041 0016 0041 0016 0016 0016 0016 0016 0016 0016 0016 0016 0016 0016 0041 0016 0041 0016 0041 0016 0016 0016 0016 0016 0016 0016 0016 0016 0016 0016 0041 0016 0016 0016 0016 0016 0041 0016 0041 0016 0016 0016 0016 0016 0041 0016 0016 0016 0041 0016 0041 0016 0016 0016 0016 0016 0041 0016 0041 0016 0016 0016 0703";
        
        public Config()
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

        public void Save()
        {
            if (hasChanged)
            {
                Edith.Modules.FirstRemote.Settings.Default.Save();
            }
        }

        public string Title
        {
            get { return Edith.Modules.FirstRemote.Resources.ModuleName; }
        }

        public UIElement Element
        {
            get { return this; }
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            hasChanged = true;
            if (HasChanged != null)
            {
                HasChanged(this, new EventArgs());
            }
        }

        void LearnCompletedEvent(object sender, LearnCompletedEventArgs e)
        {
            System.Diagnostics.Debugger.Break();
        }

        private static  void TestLearn(Controller mc, CodeFormat learnFormat, LearnCodeModifier learnCodeModifier)
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

            // textBox8.Text = e.Progress.ToString();
            //e.CarrierFrequency = textBox9.Text;
            //e.SignalQuality = textBox10.Text;
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
            string chamar = (" " + e.IRCode);
            
            //   if(e.IRCode != null)
            //{
            //e.iRCode = textBox11.Text;
            //}
            //
        }

        private void ButtonR0_Click(object sender, RoutedEventArgs e)
        {
            TestLearn(mc, CodeFormat.Pronto, LearnCodeModifier.None);
            aumvol = irCode;
          //  string chamado = "";
            irCode = textBox1.Text;
            
           // textBox1.Text = ("Codigo IR:" + chamado);
           //  Console.WriteLine("Aqui: " + ligar);
            
        }

        private void ButtonR1_Click(object sender, RoutedEventArgs e)
        {

            TestLearn(mc, CodeFormat.Pronto, LearnCodeModifier.None);
            dimivol = irCode;
            irCode = textBox2.Text;
            
        }

        private void ButtonR2_Click(object sender, RoutedEventArgs e)
        {

            TestLearn(mc, CodeFormat.Pronto, LearnCodeModifier.None);
            aumca = irCode;
            irCode = textBox3.Text;

        }

        private void ButtonR3_Click(object sender, RoutedEventArgs e)
        {

            TestLearn(mc, CodeFormat.Pronto, LearnCodeModifier.None);
            dimica = irCode;
            irCode = textBox4.Text;

        }

        private void ButtonR4_Click(object sender, RoutedEventArgs e)
        {

            TestLearn(mc, CodeFormat.Pronto, LearnCodeModifier.None);
            ligar = irCode;
            irCode = textBox5.Text;

        }

        private void ButtonR5_Click(object sender, RoutedEventArgs e)
        {

            TestLearn(mc, CodeFormat.Pronto, LearnCodeModifier.None);
            desligar = irCode;
            irCode = textBox6.Text;

        }

        private void ButtonR6_Click(object sender, RoutedEventArgs e)
        {

            TestLearn(mc, CodeFormat.Pronto, LearnCodeModifier.None);
            mute = irCode;
            irCode = textBox7.Text;

        }

      

    }
}
