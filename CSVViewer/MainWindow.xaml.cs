using System;
using System.Collections.Generic;
using System.Linq;
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
using System.Windows.Shapes;
using System.Data.Odbc;
using System.Data.OleDb;
using System.Data;

namespace CSVViewer
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            string testFile = "D:\\temp\\incoming\\Rorganis20161021.csv";
            Encoding encoding = System.Text.Encoding.Default;
            DataTable data = GetDataTableFromCSVFile(testFile, false,encoding);
            statusTB.Text = data.Rows.Count.ToString();
            mainGrid.ItemsSource = data.DefaultView;
            
        }


        private static DataTable GetDataTableFromCSVFile(string csv_file_path, bool FirstLineIsHeader,Encoding encoding)
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
        //A LIRE : https://msdn.microsoft.com/fr-fr/library/office/ff965871(v=office.14).aspx
        //then : http://stackoverflow.com/questions/5182767/microsoft-ace-oledb-12-0-csv-connectionstring
        //more struggle ! http://www.powershellmagazine.com/2015/05/12/natively-query-csv-files-using-sql-syntax-in-powershell/
    }//endClass MainWindows
}//end namespace
