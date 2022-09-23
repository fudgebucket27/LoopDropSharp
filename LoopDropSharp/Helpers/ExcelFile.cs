using Microsoft.Office.Interop.Excel;
using Excel = Microsoft.Office.Interop.Excel;

namespace LoopDropSharp.Helpers
{
    public class ExcelFile
    {
        public static void CreateExcelFile()
        {
            var fileName = "test";
            //Create excel app object
            Excel.Application xlSamp = new Microsoft.Office.Interop.Excel.Application();
            if (xlSamp == null)
            {
                Console.WriteLine("Excel is not Insatalled");
                Console.ReadKey();
                return;
            }

            //Create a new excel book and sheet
            Excel.Workbook xlWorkBook;
            Excel.Worksheet xlWorkSheet;
            object misValue = System.Reflection.Missing.Value;

            //Then add a sample text into first cell
            xlWorkBook = xlSamp.Workbooks.Add(misValue);
            xlWorkSheet = (Excel.Worksheet)xlWorkBook.Worksheets.get_Item(1);
            xlWorkSheet.Cells[1, 1] = "Name";
            xlWorkSheet.Cells[1, 2] = "Description";
            xlWorkSheet.Cells[1, 3] = "Owner";
            xlWorkSheet.Cells[1, 4] = "Amount";

            //Save the opened excel book to custom location
            string location = @"./" + fileName + ".xls";//Dont forget, you have to add to exist location
            xlWorkBook.SaveAs(location, Excel.XlFileFormat.xlWorkbookNormal, misValue, misValue, misValue, misValue, Excel.XlSaveAsAccessMode.xlExclusive, misValue, misValue, misValue, misValue, misValue);
            xlWorkBook.Close(true, misValue, misValue);
            xlSamp.Quit();

            //release Excel Object 
            try
            {
                System.Runtime.InteropServices.Marshal.ReleaseComObject(xlSamp);
                xlSamp = null;
            }
            catch (Exception ex)
            {
                xlSamp = null;
                Console.Write("Error " + ex.ToString());
            }
            finally
            {
                GC.Collect();
            }

        }
}
}
