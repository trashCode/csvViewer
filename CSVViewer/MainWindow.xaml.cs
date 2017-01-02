using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.VisualBasic.FileIO;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
//using System.Windows.Shapes;
using System.Data.Odbc;
using System.Data.OleDb;
using System.Data;
using System.Diagnostics;
using Microsoft.Win32;


namespace CSVViewer
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        string currentFilePath = null;
        bool modified = false;
        public bool Modified
        {
            get { return modified; }
            set { modified = value; }
        }

        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = this;

            Encoding encoding = System.Text.Encoding.Default;
            mainGrid.LoadingRow += loadRow_Rorganis;
        }


        void loadRow_Rorganis(object sender, DataGridRowEventArgs e)
        {
            // Get the DataRow corresponding to the DataGridRow that is loading.
            DataRowView item = e.Row.Item as DataRowView;
            if (item != null)
            {
                DataRow row = item.Row;

                // Set the background color of the DataGrid row based on whatever data you like from the row
                if (row["8"].ToString() == "")
                {
                    e.Row.Background = new SolidColorBrush(Colors.IndianRed);// FromRGB(127,0,0));
                }else{
                    e.Row.Background = new SolidColorBrush(Colors.White);// FromRGB(255,255,255));
                }
            }
        }

        private static DataTable Csv2DataTable(string csv_file_path, bool FirstLineIsHeader,Encoding encoding)
        {
            DataTable csvData = new DataTable();

            try
            {
                using (TextFieldParser csvReader = new TextFieldParser(csv_file_path, encoding))
                {
                    csvReader.SetDelimiters(new string[] { ";" });
                    csvReader.HasFieldsEnclosedInQuotes = false;
                    string[] colFields = csvReader.ReadFields();

                    if (FirstLineIsHeader)
                    {
                        foreach (string column in colFields)
                        {
                            DataColumn datecolumn = new DataColumn(column);
                            datecolumn.AllowDBNull = true;
                            csvData.Columns.Add(datecolumn);
                        }
                    }
                    else
                    {
                        for (int i=0 ; i<colFields.Length ; i++)
                        {
                            DataColumn datecolumn = new DataColumn(i.ToString());
                            datecolumn.AllowDBNull = true;
                            csvData.Columns.Add(datecolumn);
                        }
                        //Nous avons deja la premiere ligne dans le colFields => on la parse
                        csvData.Rows.Add(colFields);
                    }

                    while (!csvReader.EndOfData)
                    {

                        string[] fieldData = csvReader.ReadFields();
                        //Making empty value as null
                        for (int i = 0; i < fieldData.Length; i++)
                        {
                            if (fieldData[i] == "")
                            {
                                fieldData[i] = null;
                            }
                        }

                        csvData.Rows.Add(fieldData);
                    }
                }
            }
            catch (Exception ex)
            {
            }

            return csvData;
        }

        private static string DataTable2Csv(DataTable dt, Encoding encoding)
        {
            StringBuilder sb = new StringBuilder();

            foreach (DataRow row in dt.Rows)
            {
                IEnumerable<string> fields = row.ItemArray.Select(field => field.ToString());
                sb.AppendLine(string.Join(";", fields));
            }
            return sb.ToString();
        }

        private void OpenFile(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Fichiers Csv (*.csv)|*.csv|Tous (*.*)|*.*";

            //openFileDialog.InitialDirectory = @"\\w11690100suf\APP-ACCESS\clotures";
            //openFileDialog.InitialDirectory = @"D:\temp\incoming";

            if (openFileDialog.ShowDialog() == true)
            {
                this.currentFilePath = openFileDialog.FileName;
                this.Modified = false;
                Encoding encoding = System.Text.Encoding.Default;
                DataTable data = Csv2DataTable(openFileDialog.FileName, false, encoding);
                statusTB.Text = data.Rows.Count.ToString() + " lignes chargées";
                mainGrid.ItemsSource = data.DefaultView;
            }

        }

        private void Save(object sender, RoutedEventArgs e)
        {
            File.WriteAllText(currentFilePath, DataTable2Csv(((DataView)mainGrid.ItemsSource).ToTable(), System.Text.Encoding.Default));
            Modified = false; 
        }

        private void SaveAs(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Fichiers Csv (*.csv)|*.csv|Tous (*.*)|*.*";
            saveFileDialog.FileName = Path.GetFileName(currentFilePath);

            if (saveFileDialog.ShowDialog() == true)
                File.WriteAllText(saveFileDialog.FileName, DataTable2Csv(((DataView)mainGrid.ItemsSource).ToTable(), System.Text.Encoding.Default));

            Modified = false;
            statusTB.Text = " Enregistré ";
        }

        //A LIRE : https://msdn.microsoft.com/fr-fr/library/office/ff965871(v=office.14).aspx
        //then : http://stackoverflow.com/questions/5182767/microsoft-ace-oledb-12-0-csv-connectionstring
        //more struggle ! http://www.powershellmagazine.com/2015/05/12/natively-query-csv-files-using-sql-syntax-in-powershell/
        //MVVM https://msdn.microsoft.com/en-us/magazine/dd419663.aspx

        private void About(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("CSV Viewer 1.0\n Utilitaire de visualisation des fichiers csv \n G.Fanton 2016");
        }

        private void Debug(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(Modified ? "modified = true" : "modified = false");

        }

        private void mainGrid_RowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
        {
            infoTB.Text = DateTime.Now.ToString("hh:mm:ss") + " " +e.EditAction.ToString();

            if (e.EditAction == DataGridEditAction.Commit)
                Modified = true;
                statusTB.Text = DateTime.Now.ToString("hh:mm:ss") + " " + e.EditAction.ToString();
                


        }

    }//endClass MainWindows
}//end namespace
