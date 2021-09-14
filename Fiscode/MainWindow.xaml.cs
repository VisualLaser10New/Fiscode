using System;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;

namespace Fiscode
{
    /// <summary>
    /// Logica di interazione per MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Tool toolObj = new Tool();
        private string result
        {
            get => Form1.resTBOX.Text;
            set => Form1.resTBOX.Text = value;
        }

        public MainWindow()
        {
            InitializeComponent();
            //Task.WaitAll();
            initializeProvinceCombo();
        }

        private void calcFiscBTN_Click(object sender, RoutedEventArgs e)
        {
            if (!toolObj.checkTbox(new[] { nomeTBOX, cognomeTBOX, new TextBox { Text = dataPicker.Text } }))
            {
                //check if all strings are filled
                toolObj.notFilled();
                return;
            }

            //calculate the fiscal code
            CodFisc calcola = new CodFisc(
                cognomeTBOX,
                nomeTBOX,
                sessoCombo,
                provinceListB,
                dataPicker);
            result = calcola.calc();
        }

        private void calcPIBTN_Click(object sender, RoutedEventArgs e)
        {
            if (!toolObj.checkTbox(new[] { matricolaTBOX }) || !Int32.TryParse(matricolaTBOX.Text, out _) || matricolaTBOX.Text.Length > 7)
            {
                toolObj.notFilled();
                return;
            }

            PartIva calcolaIva = new PartIva(
                matricolaTBOX,
                uffprovList
                );
            result = calcolaIva.calcIVA();
        }

        private void initializeProvinceCombo()
        {
            Object s = Properties.Resources.ResourceManager.GetObject("Codicicatastali");
            string line = null;
            using (StringReader reader = new StringReader(s.ToString()))
            {
                while ((line = reader.ReadLine()) != null)
                {
                    provinceListB.Items.Add(line);
                }
            }

            //load the IVA office too
            Object r = Properties.Resources.ResourceManager.GetObject("Codiciiva");
            using (StringReader reader = new StringReader(r.ToString()))
            {
                while ((line = reader.ReadLine()) != null)
                {
                    uffprovList.Items.Add(line);
                }
            }
        }

        private void cpyBTN_Click(object sender, RoutedEventArgs e)
        {
            string toclp = result;
            Clipboard.SetText(toclp);
        }
    }

    class Tool
    {
        private static readonly string MBOXTitle = Assembly.GetExecutingAssembly().GetName().Name;
        public bool checkTbox(TextBox[] input)
        {
            //if a string in array is null or whitespaces return false
            foreach (var tb in input)
            {
                if (String.IsNullOrWhiteSpace(tb.Text))
                    return false;
            }

            return true;
        }

        public Action notFilled = () => MessageBox.Show("Compilare tutte le caselle di testo\nCon parametri validi",
             MBOXTitle,
             MessageBoxButton.OK,
             MessageBoxImage.Error);
    }
}
