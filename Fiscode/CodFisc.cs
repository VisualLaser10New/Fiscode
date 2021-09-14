using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;

namespace Fiscode
{
    class CodFisc
    {
        private HelperFisc hp = new HelperFisc();
        private string nome, cognome, provincia;
        private char sesso = (char)0;
        private DateTime inputData;
        private string output;

        

        public CodFisc(TextBox tcogn, TextBox tnome, ComboBox tsesso,  ListBox tluogo, DatePicker tdata)
        {
            //constructor to initialization
            nome = tnome.Text.ToUpper();
            cognome = tcogn.Text.ToUpper();
            sesso = char.Parse(tsesso.Text);
            provincia = tluogo.SelectedItem.ToString().Substring(0, 4);
            inputData = Convert.ToDateTime(tdata.Text);
        }

        public string calc()
        {
            char meseM;
            output += COGgen();
            output += NOMgen();
            output += inputData.ToString("yy");
            hp.mesi.TryGetValue(inputData.Month, out meseM);//for the month
            output += meseM;
            output += (inputData.Day + (sesso == 'M' ? 0 : 40)).ToString();
            output += provincia;
            output += CTRgen();
            return output;
        }

        private string COGgen()
        {
            //generate the 3 chars surname
            string cognOutput = null;
            int cons = hp.contaVC(cognome, hp.consonanti);
            int voc = hp.contaVC(cognome, hp.vocali);

            hp.setLetter(ref cognOutput, cognome, hp.consonanti);//set the consonants

            if (cons < 3)
            {
                //if the consonants are not enough
                hp.setLetter(ref cognOutput, cognome, hp.vocali);//set the vowels
                if(voc + cons < 3)
                {
                    hp.xFiller(ref cognOutput, 3 - (voc + cons)); //fill with x if cons+voc aren't enough
                }
            }

            return cognOutput;
        }

        private string NOMgen()
        {
            //generate the 3 chars name
            string nomeOutput = null;
            int cons = hp.contaVC(nome, hp.consonanti);
            int voc = hp.contaVC(nome, hp.vocali);

            hp.setNomeCons(ref nomeOutput, nome, hp.consonanti, cons);
            if(cons < 3)
            {
                hp.setLetter(ref nomeOutput, nome, hp.vocali);
                if (voc + cons < 3)
                {
                    hp.xFiller(ref nomeOutput, 3- (voc+cons));
                }
            }

            return nomeOutput;
        }

        private char CTRgen()
        {
            //generate the one char check letter
            int[] number = new int[15];
            int numres = 0;

            hp.FillNUM(ref number, output); //fill the number array
            numres = number.Sum();
            return Convert.ToChar((numres % 26) + 65);
        }
    }

    class HelperFisc
    {
        //class with tool to help the fiscal code generation
        public readonly string consonanti = "BCDFGHJKLMNPQRSTVWXYZ";
        public readonly string vocali = "AEIOU";

        public readonly Dictionary<char, int> dispari = new Dictionary<char, int>
        {
            {'0', 1}, {'1', 0}, {'2', 5}, {'3', 7}, {'4', 9}, {'5', 13}, {'6', 15}, {'7', 17}, {'8', 19},
            {'9', 21}, {'A', 1}, {'B', 0}, {'C', 5}, {'D', 7}, {'E', 9}, {'F', 13}, {'G', 15}, {'H', 17},
            {'I', 19}, {'J', 21}, {'K', 2}, {'L', 4}, {'M', 18}, {'N', 20}, {'O', 11}, {'P', 3}, {'Q', 6},
            {'R', 8}, {'S', 12}, {'T', 14}, {'U', 16}, {'V', 10}, {'W', 22}, {'X', 25}, {'Y', 24}, {'Z', 23}
        };

        public readonly Dictionary<char, int> pari = new Dictionary<char, int>
        {
            {'0', 0}, {'1', 1}, {'2', 2}, {'3', 3}, {'4', 4}, {'5', 5}, {'6', 6}, {'7', 7}, {'8', 8},
            {'9', 9}, {'A', 0}, {'B', 1}, {'C', 2}, {'D', 3}, {'E', 4}, {'F', 5}, {'G', 6}, {'H', 7},
            {'I', 8}, {'J', 9}, {'K', 10}, {'L', 11}, {'M', 12}, {'N', 13}, {'O', 14}, {'P', 15}, {'Q', 16},
            {'R', 17}, {'S', 18}, {'T', 19}, {'U', 20}, {'V', 21}, {'W', 22}, {'X', 23}, {'Y', 24}, {'Z', 25}
        };

        public readonly Dictionary<int, char> mesi = new Dictionary<int, char>
        {
            {1, 'A'}, {2, 'B'}, {3, 'C'}, {4, 'D'}, {5, 'E'}, {6, 'H'},
            {7, 'L'}, {8, 'M'}, {9, 'P'}, {10, 'R'}, {11, 'S'}, {12, 'T'}
        };

        public int contaVC(string input, string vc)
        {
            //count consonants or vowels
            return input.Count(c => vc.Contains(Char.ToUpper(c)));
        }

        public void setLetter(ref string output, string NomeCognome, string Alfapattern)
        {
            //set the first 3 consonants in the output
            foreach (var lett in NomeCognome)
            {
                if (Alfapattern.Contains(lett))
                {
                    //if this letter is contained in consonants
                    output += lett;
                }

                if (output != null && output.Length == 3)
                    return;
            }
        }

        public void setNomeCons(ref string output, string nome, string Alfapattern, int nCons)
        {
            bool flag = false;
            foreach (var lett in nome)
            {
                //if consonants number is equal or greater than 4 at 3 consonant jump to next
                if (output != null && (output.Length == 1 && nCons >= 4 && flag == false && Alfapattern.Contains(lett)))
                {
                    flag = true;
                    continue;
                }

                if (Alfapattern.Contains(lett))
                {
                    output += lett;
                }

                if (output != null && output.Length == 3)
                    return;
            }
        }

        public void xFiller(ref string output, int howManyX)
        {
            //append as many 'X' as howmanyX is
            for (int i = 0; i < howManyX; i++)
            {
                output += 'X';
            }
        }

        public void FillNUM(ref int[] toFill, string fromFill)
        {
            int count = 0;
            foreach (var lett in fromFill)
            {
                if(count % 2==0)
                {
                    //if is even use the odd table
                    dispari.TryGetValue(fromFill.ElementAt(count), out toFill[count]);
                }
                else
                {
                    pari.TryGetValue(fromFill.ElementAt(count), out toFill[count]);
                }

                count++;
            }
        }

    }
}
