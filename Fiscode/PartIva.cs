using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Fiscode
{
    class PartIva
    {
        private string codicematricola = null, codiceUffPost = null;
        private string output = null;
        private HelperIVA iv = new HelperIVA();

        public PartIva(TextBox maTextBox, ListBox uffBoxItem)
        {
            codicematricola = maTextBox.Text;
            codiceUffPost = uffBoxItem.SelectedItem.ToString().Substring(0, 3);
        }

        public string calcIVA()
        {
            output += iv.add0(codicematricola);
            output += codicematricola;
            output += codiceUffPost;
            output += checkCalc();
            return output;
        }

        private char checkCalc()
        {
            int conto = 0;
            int x = 0, y = 0, z = 0, c = 0, p = 0;
            foreach(var lett in output)
            {
                if (conto % 2 == 0)
                {
                    x += Int32.Parse(output[conto].ToString());//sum digit in odd position
                }
                else
                {
                    p = Int32.Parse(output[conto].ToString());//sum digit in even position
                    y += p;
                    if (p >= 5)//count how many digits >= 5, are in even 
                        z++;
                }
                conto++;
            }

            y *= 2;
            return Convert.ToChar(((10 - ((x + y + z) % 10)) % 10) + '0');
        }
    }

    class HelperIVA
    {
        public string add0(string mat)
        {
            //return a string composed by 0 to fill the whitespaces ahead the mat code
            return new string('0', 7 - mat.Length);
        }
    }
}
