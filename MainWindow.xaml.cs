using Microsoft.Win32;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Printing;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WPF_Kutyak
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // Private fields
        ObservableCollection<Dog> dogs;
        Dictionary<int, string> nameDict;
        Dictionary<int, string[]> speciesDict;
        public MainWindow()
        {
            //Init list and dicts
            dogs = new();
            nameDict = new();
            speciesDict = new();
            //Load
            LoadDogs();
            LoadNames();
            LoadSpecies();
            InitializeComponent();
        }
        #region Loading 
        void LoadDogs()
        {
            using (StreamReader sr = new(@"./Data/Kutyak.csv"))
            {
                sr.ReadLine()!.Skip(1);
                while (!sr.EndOfStream)
                {
                    string[] line = sr.ReadLine()!.Split(';');
                    dogs.Add(new Dog(
                        int.Parse(line[0]), // ID
                        int.Parse(line[1]), // fajtaID
                        int.Parse(line[2]), // névID
                        int.Parse(line[3]), // életkor
                        DateTime.Parse(line[^1]) // utolsó orvosi ellenőrzés 
                        ));
                }
            }
        }
        void LoadNames()
        {
            using (StreamReader sr = new(@"./Data/KutyaNevek.csv"))
            {
                sr.ReadLine()!.Skip(1);
                while (!sr.EndOfStream)
                {
                    string[] line = sr.ReadLine()!.Split(';');
                    nameDict.Add(int.Parse(line[0]), line[^1]);
                }
            }
        }

        void LoadSpecies()
        {
            using (StreamReader sr = new(@"./Data/KutyaFajtak.csv"))
            {
                sr.ReadLine()!.Skip(1);
                while (!sr.EndOfStream)
                {
                    string[] line = sr.ReadLine()!.Split(';');
                    speciesDict.Add(int.Parse(line[0]), line[1..]);
                }
            }
        }
        #endregion

        #region Button Click events
        private void btn_3feladat_Click(object sender, RoutedEventArgs e)
        {
            tbl_Result.Text = $"3. feladat: KutyaNevek száma: {nameDict.Count} db";
        }


        private void btn_6feladat_Click(object sender, RoutedEventArgs e)
        {
            tbl_Result.Text = $"6. feladat: Kutyák átlag életkora: {(dogs.Average(dog => dog.Age)):F2}";
        }
        private void btn_7feladat_Click(object sender, RoutedEventArgs e)
        {
            Dog oldest = dogs.OrderByDescending(dog => dog.Age).FirstOrDefault()!;
            tbl_Result.Text = $"7. feladat: legidősebb kutya neve és fajtája: " +
                $"{nameDict[oldest.NameID]}, {speciesDict[oldest.SpeciesID][0]}";
        }
        private void btn_8feladat_Click(object sender, RoutedEventArgs e)
        {
            List<IGrouping<int, Dog>> query = dogs.Where(dog => dog.LastMedicalCheck.
            CompareTo(new DateTime(year: 2018, month: 1, day: 10)) 
            == 0) // 0 -> a kettő dátum megegyezik
                .GroupBy(dog => dog.SpeciesID)
                .ToList();
            
            tbl_Result.Text = $"8. feladat: Január 10.-én vizsgált kutyafajták:";
            query.ForEach(dog =>  tbl_Result.Text += $"\n{speciesDict[dog.Key][0]} : {dog.Count()} kutya");
        }
        private void btn_9feladat_Click(object sender, RoutedEventArgs e)
        {
            IGrouping<DateTime, Dog> busiestDay = dogs.GroupBy(dog => dog.LastMedicalCheck).
                OrderByDescending(dog => dog.Count()).First();
            tbl_Result.Text = $" 9.feladat: Legjobban leterhelt nap: " +
                $"{busiestDay.Key} : {busiestDay.Count()} kutya";

        }


        private void btn_10feladat_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog svDialog = new();
            svDialog.Filter = "Txt files (*.txt)|*.txt";
            svDialog.FileName = "Névstatisztika";
            if((bool)svDialog.ShowDialog()!)
            {
                // Még én magam se tudom, hogy ezt mégis hogy csináltam
                IGrouping<int, Dog>[] nameQuery = dogs.GroupBy(dog => dog.NameID)
                    .OrderByDescending(dog => dog.Count()).ToArray();
                using(StreamWriter sw = new(svDialog.FileName))
                {
                    foreach(var name in nameQuery)
                    {
                        sw.WriteLine($"{nameDict[name.Key]};{name.Count()}");
                    }
                }
                MessageBox.Show("szöveges fájl sikeresen létrehozva",
                    "Success",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
            }

        }
        #endregion

        private void dp_datum_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            List<Dog> DateQuery = dogs.Where(dog => dog.LastMedicalCheck == dp_datum.SelectedDate).ToList();
            dg_kutyak.ItemsSource = DateQuery;
            tbl_Result.Text = DateQuery.Count == 0 ? "Nem volt ellenőrzés ezen a napon" : 
                $"{DateQuery.Count} kutya volt ezen a napon";
           
        }

    }
}
