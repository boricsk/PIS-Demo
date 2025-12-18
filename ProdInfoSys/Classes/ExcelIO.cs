using ClosedXML.Excel;
using ProdInfoSys.Models.FollowupDocuments;
using ProdInfoSys.Models.StatusReportModels;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace ProdInfoSys.Classes
{
    /// <summary>
    /// Provides methods for importing and exporting Excel files related to shipout plans, headcount follow-up, and
    /// generic data collections.
    /// </summary>
    /// <remarks>The ExcelIO class offers utility methods to facilitate reading from and writing to Excel
    /// files using strongly-typed collections. It supports importing shipout plan data from Excel files and exporting
    /// various data types to Excel, automatically saving the output to the user's Downloads folder. All methods assume
    /// that the input data matches the expected structure of the Excel files. This class is not thread-safe.</remarks>
    public class ExcelIO
    {
        public ExcelIO() { }

        /// <summary>
        /// Imports shipout plan data from the specified Excel file and returns a collection of shipout plan records.
        /// </summary>
        /// <param name="file">The path to the Excel file containing the shipout plan data. The file must exist and be accessible.</param>
        /// <returns>An observable collection of <see cref="ShipoutPlan"/> objects representing the imported shipout plan
        /// records. The collection will be empty if the file contains no data rows.</returns>
        /// <exception cref="InvalidOperationException">Thrown if the Excel file does not contain any worksheets.</exception>
        public ObservableCollection<ShipoutPlan> ImportShipoutPlan(string file)
        {
            ObservableCollection<ShipoutPlan> _shipoutPlan = new ObservableCollection<ShipoutPlan>();

            using var workbook = new XLWorkbook(file);
            var worksheet = workbook.Worksheets.First();
            if (worksheet != null)
            {
                var firstDataRow = 2;
                var lastRow = worksheet.LastRowUsed().RowNumber();


                for (int row = firstDataRow; row <= lastRow; row++)
                {
                    var r = worksheet.Row(row);
                    _shipoutPlan.Add(new ShipoutPlan
                    {
                        Bizonylatszam = r.Cell(1).GetString(),
                        Szam = r.Cell(2).GetString(),
                        NyitottMennyiseg = (decimal)r.Cell(3).GetDouble(),
                        EredetiKertDatum = r.Cell(4).GetDateTime(),
                        KiszallitasiDatum = r.Cell(5).GetDateTime(),
                        CustomerName = r.Cell(6).GetString(),
                        SapSoNum = r.Cell(7).GetString(),
                        SapPoNum = r.Cell(8).GetString(),
                        EgysegarAfaNelkul = r.Cell(9).GetDouble(),
                        NyitottOsszeg = (decimal)r.Cell(10).GetDouble(),
                        OrderDate = r.Cell(11).GetDateTime(),
                        SzamlazasiVevoSzam = r.Cell(12).GetString(),
                        CustomerNo = r.Cell(13).GetString(),
                        SeiCustRefNo = r.Cell(14).GetString(),
                        ETD = r.Cell(15).GetString(),
                    });
                }
                return _shipoutPlan;
            }
            else
            {
                throw new InvalidOperationException("Az Excel fájl nem tartalmaz munkalapot.");
            }
        }

        /// <summary>
        /// Exports the provided headcount follow-up data to an Excel file in the user's Downloads folder and opens the
        /// file after creation.
        /// </summary>
        /// <remarks>The exported Excel file is named with a timestamp to ensure uniqueness and is
        /// automatically opened upon successful creation. Each item in the collection is written as a row in the
        /// spreadsheet. If the collection is empty, only the header row is included in the file.</remarks>
        /// <param name="doc">The collection of headcount follow-up documents to be exported. Cannot be null.</param>
        public void ExportHeadcountFollowup(ObservableCollection<HeadCountFollowupDocument> doc)
        {

            string downloadsPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads");

            string fileName = $"Headcount followup{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";
            string fullPath = Path.Combine(downloadsPath, fileName);

            using var workbook = new XLWorkbook();
            var ws = workbook.Worksheets.Add("Headcount followup");

            // Fejléc
            ws.Cell(1, 1).Value = "Munkanap";
            ws.Cell(1, 2).Value = "Netto HC terv";
            ws.Cell(1, 3).Value = "Brutto HC terv";
            ws.Cell(1, 4).Value = "Aktuális HC";
            ws.Cell(1, 5).Value = "Alvállalkozó";
            ws.Cell(1, 6).Value = "Egyéb";
            ws.Cell(1, 7).Value = "Indirekt";
            ws.Cell(1, 8).Value = "QA Indirekt";
            ws.Cell(1, 9).Value = "TTL Indirect";
            ws.Cell(1, 10).Value = "Szabi";
            ws.Cell(1, 11).Value = "Beteg";
            ws.Cell(1, 12).Value = "Diff.";
            ws.Cell(1, 13).Value = "Tervezett munkaóra";
            ws.Cell(1, 14).Value = "Aktuális munkaóra";

            // Adatok
            for (int i = 0; i < doc.Count; i++)
            {
                ws.Cell(i + 2, 1).Value = doc[i].Workday.ToString();
                ws.Cell(i + 2, 2).Value = doc[i].NettoHCPlan;
                ws.Cell(i + 2, 3).Value = doc[i].HCPlan;
                ws.Cell(i + 2, 4).Value = doc[i].ActualHC;
                ws.Cell(i + 2, 5).Value = doc[i].Subcontactor;
                ws.Cell(i + 2, 6).Value = doc[i].Others;
                ws.Cell(i + 2, 7).Value = doc[i].Indirect;
                ws.Cell(i + 2, 8).Value = doc[i].QAIndirect;
                ws.Cell(i + 2, 9).Value = doc[i].DirectPlusIndirect;
                ws.Cell(i + 2, 10).Value = doc[i].Holiday;
                ws.Cell(i + 2, 11).Value = doc[i].AbsebseTotal;
                ws.Cell(i + 2, 12).Value = doc[i].Diff;
                ws.Cell(i + 2, 13).Value = doc[i].CalcActualSH;
                ws.Cell(i + 2, 14).Value = doc[i].CalcPlannedSH;
            }

            ws.Columns().AdjustToContents();
            workbook.SaveAs(fullPath);
            Process.Start(new ProcessStartInfo(fullPath) { UseShellExecute = true });

        }

        /// <summary>
        /// Exports the specified collection of objects to a new Excel (.xlsx) file in the user's Downloads folder,
        /// using the provided sheet name and file name prefix.
        /// </summary>
        /// <remarks>The generated Excel file will be saved in the user's Downloads folder and opened
        /// automatically after creation. Only public readable properties of type T are included as columns. Existing
        /// files with the same name will be overwritten.</remarks>
        /// <typeparam name="T">The type of objects contained in the collection to export. Each public readable property of this type will
        /// be exported as a column in the Excel sheet.</typeparam>
        /// <param name="doc">The collection of objects to export to Excel. Each object represents a row in the resulting worksheet.
        /// Cannot be null.</param>
        /// <param name="sheetName">The name of the worksheet to create in the Excel file. Cannot be null or empty.</param>
        /// <param name="filePrefix">The prefix to use for the generated Excel file name. The final file name will include this prefix and a
        /// timestamp. Cannot be null or empty.</param>
        public void ExcelExport<T>(ObservableCollection<T> doc, string sheetName, string filePrefix)
        {
            string downloadsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads");
            string fileName = $"{filePrefix} {DateTime.Now:yyyyMMdd_HHmmss}.xlsx";
            string fullPath = Path.Combine(downloadsPath, fileName);

            using var workbook = new XLWorkbook();
            var ws = workbook.Worksheets.Add(sheetName);

            var props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance).Where(p => p.CanRead).ToArray();

            // Fejléc
            for (int col = 0; col < props.Length; col++)
            {
                ws.Cell(1, col + 1).Value = props[col].Name.ToString();
                ws.Cell(1, col + 1).Style.Font.Bold = true;
                ws.Cell(1, col + 1).Style.Fill.BackgroundColor = XLColor.LightGray;
                ws.Cell(1, col + 1).Style.Border.BottomBorder = XLBorderStyleValues.Double;
                ws.Cell(1, col + 1).Style.Border.BottomBorderColor = XLColor.Black;
            }

            // Adatok
            for (int row = 0; row < doc.Count; row++)
            {
                for (int col = 0; col < props.Length; col++)
                {
                    var value = props[col].GetValue(doc[row]);
                    var type = Nullable.GetUnderlyingType(props[col].PropertyType) ?? props[col].PropertyType;

                    if (type == typeof(DateTime))
                        ws.Cell(row + 2, col + 1).Value = (DateTime)value;
                    else if (type == typeof(bool))
                        ws.Cell(row + 2, col + 1).Value = (bool)value;
                    else if (type == typeof(int))
                        ws.Cell(row + 2, col + 1).Value = Convert.ToInt32(value);
                    else if (type == typeof(double))
                        ws.Cell(row + 2, col + 1).Value = Convert.ToDouble(value);
                    else if (type == typeof(decimal))
                        ws.Cell(row + 2, col + 1).Value = Convert.ToDecimal(value);
                    else
                        ws.Cell(row + 2, col + 1).Value = value.ToString();
                }
            }

            ws.Columns().AdjustToContents();
            workbook.SaveAs(fullPath);
            Process.Start(new ProcessStartInfo(fullPath) { UseShellExecute = true });
        }
    }
}
