using NPOI.HPSF;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;

namespace netcore.Models.Excel
{
    public class ExcelHelper
    {
        #region Excel转DataTable
        public DataTable GetExcelDataTable(string filePath)
        {
            IWorkbook Workbook;
            DataTable table = new DataTable();
            try
            {
                using (FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                {
                    //XSSFWorkbook 适用XLSX格式，HSSFWorkbook 适用XLS格式
                    string fileExt = Path.GetExtension(filePath).ToLower();
                    if (fileExt == ".xls")
                    {
                        Workbook = new HSSFWorkbook(fileStream);
                    }
                    else if (fileExt == ".xlsx")
                    {
                        Workbook = new XSSFWorkbook(fileStream);
                    }
                    else
                    {
                        Workbook = null;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            int sheetNum = Workbook.NumberOfSheets;
            for (int kk = 0; kk < sheetNum; kk++)
            {
                //定位在sheet
                ISheet sheet = Workbook.GetSheetAt(kk);
                //第一行为标题行
                IRow headerRow = sheet.GetRow(kk);
                int cellCount = headerRow.LastCellNum;
                int rowCount = sheet.LastRowNum;
                if (kk == 0)
                {
                    //循环添加标题列
                    for (int i = headerRow.FirstCellNum; i < cellCount; i++)
                    {
                        DataColumn column = new DataColumn(headerRow.GetCell(i).StringCellValue);
                        table.Columns.Add(column);
                    }
                }
                //行数据
                for (int i = (sheet.FirstRowNum + 1); i <= rowCount; i++)
                {
                    IRow row = sheet.GetRow(i);
                    DataRow dataRow = table.NewRow();
                    if (row != null)
                    {
                        for (int j = row.FirstCellNum; j < cellCount; j++)
                        {
                            if (row.GetCell(j) != null)
                            {
                                dataRow[j] = GetCellValue(row.GetCell(j));
                            }
                        }
                    }
                    table.Rows.Add(dataRow);
                }
            }
            return table;
        }
        #endregion

        #region 获取单元格的值
        private static string GetCellValue(ICell cell)
        {
            if (cell == null)
            {
                return string.Empty;
            }

            switch (cell.CellType)
            {
                case CellType.Blank:
                    return string.Empty;
                case CellType.Boolean:
                    return cell.BooleanCellValue.ToString();
                case CellType.Error:
                    return cell.ErrorCellValue.ToString();
                case CellType.Numeric:
                case CellType.Unknown:
                default:
                    return cell.ToString();
                case CellType.String:
                    return cell.StringCellValue;
                case CellType.Formula:
                    try
                    {
                        HSSFFormulaEvaluator e = new HSSFFormulaEvaluator(cell.Sheet.Workbook);
                        e.EvaluateInCell(cell);
                        return cell.ToString();
                    }
                    catch
                    {
                        return cell.NumericCellValue.ToString();
                    }
            }
        }
        #endregion

        #region 读取excel转换成list集合
        /// <summary>
        /// 读取excel转换成list集合
        /// </summary>
        /// <typeparam name="T">对象</typeparam>
        /// <param name="filePath"></param>
        /// <param name="stream">文件流</param>
        /// <param name="startIndex">从第几行开始读取</param>
        /// <param name="sheetIndex">读取第几个sheet</param>
        /// <returns></returns>
        public IList<T> GetList<T>(string filePath)
            where T : class
        {
            IList<T> ts = new List<T>();
            try
            {
                IWorkbook workbook;
                using (FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                {
                    //XSSFWorkbook 适用XLSX格式，HSSFWorkbook 适用XLS格式
                    string fileExt = Path.GetExtension(filePath).ToLower();
                    if (fileExt == ".xls")
                    {
                        workbook = new HSSFWorkbook(fileStream);
                    }
                    else if (fileExt == ".xlsx")
                    {
                        workbook = new XSSFWorkbook(fileStream);
                    }
                    else
                    {
                        workbook = null;
                    }
                }
                var sheet = workbook.GetSheetAt(0);
                if (sheet != null)
                {
                    IRow firstRow = sheet.GetRow(0);
                    //一行最后一个cell的编号 即总的列数
                    int cellCount = firstRow.LastCellNum;
                    //最后一列的标号
                    int rowCount = sheet.LastRowNum;
                    for (int i = 1; i <= rowCount; ++i)
                    {
                        //获取行的数据
                        IRow row = sheet.GetRow(i);
                        if (row == null) continue; //没有数据的行默认是null　　　　　　　
                        {
                            T model = Activator.CreateInstance<T>();
                            for (int j = row.FirstCellNum; j < cellCount; ++j)
                            {
                                if (row.GetCell(j) != null)
                                {
                                    var rowTemp = row.GetCell(j);
                                    string value = GetCellValue(rowTemp);
                                    //if (rowTemp.CellType == CellType.Numeric)
                                    //{
                                    //    short format = rowTemp.CellStyle.DataFormat;
                                    //    if (format == 14 || format == 31 || format == 57 || format == 58 || format == 20)
                                    //        value = rowTemp.DateCellValue.ToString("yyyy-MM-dd");
                                    //    else
                                    //        value = rowTemp.NumericCellValue.ToString();
                                    //}
                                    //else
                                    //    value = rowTemp.ToString();
                                    //赋值
                                    foreach (System.Reflection.PropertyInfo item in typeof(T).GetProperties())
                                    {
                                        var column = item.GetCustomAttributes(true).First(x => x is ColumnAttribute) as ColumnAttribute;
                                        if (column.Index == j)
                                        {
                                            item.SetValue(model, value);
                                            break;
                                        }
                                    }
                                }
                            }
                            ts.Add(model);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {

            }
            return ts;
        }
        #endregion

        #region DataTable导出Excel：转成byte
        /// <summary>
        /// 导出Excel：转成byte
        /// </summary>
        /// <param name="dataTable">数据源：DataTable</param>
        /// <param name="tableTitle">Excel列名</param>
        /// <returns></returns>
        public byte[] Output(DataTable dataTable, string[] tableTitle)
        {
            IWorkbook workbook = new XSSFWorkbook();
            ISheet sheet = workbook.CreateSheet("sheet");
            IRow Title = null;
            IRow rows = null;
            ICellStyle cellStyle1 = workbook.CreateCellStyle();
            cellStyle1.FillForegroundColor = 22;
            cellStyle1.FillPattern = FillPattern.SolidForeground;
            for (int i = 1; i <= dataTable.Rows.Count; i++)
            {
                //创建表头
                if (i - 1 == 0)
                {
                    Title = sheet.CreateRow(0);
                    for (int k = 1; k < tableTitle.Length + 1; k++)
                    {
                        Title.CreateCell(0).SetCellValue("序号");
                        Title.CreateCell(k).SetCellValue(tableTitle[k - 1]);
                        Title.CreateCell(k).CellStyle = cellStyle1;
                    }
                    continue;
                }
                else
                {
                    rows = sheet.CreateRow(i - 1);
                    for (int j = 1; j <= dataTable.Columns.Count; j++)
                    {
                        rows.CreateCell(0).SetCellValue(i - 1);
                        rows.CreateCell(j).SetCellValue(dataTable.Rows[i - 1][j - 1].ToString());
                    }
                }
            }
            byte[] buffer = new byte[1024 * 5];
            using (MemoryStream ms = new MemoryStream())
            {
                workbook.Write(ms);
                buffer = ms.ToArray();
                ms.Close();
            }
            return buffer;
        }
        #endregion

        #region list
        public static MemoryStream Export<T>(List<T> lists, string strSheetName, string[] ColumnNames)
        {
            string[] ListColumnName = SysTool.GetPropertyNameArray<T>();
            if (ColumnNames.Length != ListColumnName.Length)
            {
                return new MemoryStream();
            }

            #region 内容
            HSSFWorkbook book = new HSSFWorkbook();
            ISheet sheet = book.CreateSheet(strSheetName);
            SummaryInformation si = PropertySetFactory.CreateSummaryInformation();
            si.ApplicationName = "NPOI";            //填加xls文件创建程序信息      
            si.LastAuthor = "后台系统";           //填加xls文件最后保存者信息      
            si.Comments = "后台系统自动创建文件";      //填加xls文件作者信息      
            //si.Title = strHead;               //填加xls文件标题信息      
            //si.Subject = strHead;              //填加文件主题信息      
            si.CreateDateTime = DateTime.Now;
            book.SummaryInformation = si;
            #endregion
            int rowIndex = 0;

            for (int j = 0; j < lists.Count; j++)
            {
                #region 新建表，填充表头，填充列头，样式
                if (rowIndex == 65535 || rowIndex == 0)
                {
                    if (rowIndex != 0)
                    {
                        sheet = book.CreateSheet(strSheetName + ((int)rowIndex / 65535).ToString());
                    }

                    #region 表头及样式
                    {
                        //IRow headerRow = sheet.CreateRow(0);
                        //headerRow.HeightInPoints = 25;
                        //headerRow.CreateCell(0).SetCellValue(strHead);
                        //ICellStyle headStyle = book.CreateCellStyle();
                        //headStyle.Alignment = HorizontalAlignment.Center;
                        //IFont font = book.CreateFont();
                        //font.FontHeightInPoints = 20;
                        //font.Boldweight = 700;
                        //headStyle.SetFont(font);

                        //headerRow.GetCell(0).CellStyle = headStyle;
                        //sheet.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(0, 0, 0, ListColumnName.Length - 1));
                    }
                    #endregion


                    #region 列头及样式
                    {
                        //HSSFRow headerRow = sheet.CreateRow(1);   
                        IRow headerRow = sheet.CreateRow(0);

                        ICellStyle headStyle = book.CreateCellStyle();
                        headStyle.Alignment = HorizontalAlignment.Center;
                        IFont font = book.CreateFont();
                        font.FontHeightInPoints = 10;
                        font.Boldweight = 700;
                        headStyle.SetFont(font);

                        for (int i = 0; i < ColumnNames.Length; i++)
                        {
                            headerRow.CreateCell(i).SetCellValue(ColumnNames[i]);
                            headerRow.GetCell(i).CellStyle = headStyle;
                            //设置列宽   
                            //sheet.SetColumnWidth(i, (arrColWidth[i] + 1) * 256);
                        }

                    }
                    #endregion

                    rowIndex = 1;
                }
                #endregion


                #region 填充内容
                IRow dataRow = sheet.CreateRow(rowIndex);
                //foreach (DataColumn column in dtSource.Columns)   
                var properties = typeof(T).GetProperties(BindingFlags.Instance | BindingFlags.Public);
                for (int i = 0; i < ListColumnName.Length; i++)
                {
                    ICell newCell = dataRow.CreateCell(i);

                    //var pi = properties.First<PropertyInfo>(x => x.Name == ListColumnName[i]);//找到相同Name字段的2种方法都可以
                    var pr = properties[i];//顺序取的
                    var value = pr.GetValue(lists[j], null);

                    string drValue = value != null ? value.ToString() : "";
                    newCell.SetCellValue(drValue);

                }
                #endregion

                rowIndex++;
            }


            using (MemoryStream ms = new MemoryStream())
            {
                book.Write(ms);
                ms.Flush();
                ms.Position = 0;
                //sheet.Dispose();   
                sheet = null;
                book = null;
                //workbook.Dispose();//一般只用写这一个就OK了，他会遍历并释放所有资源，但当前版本有问题所以只释放sheet      
                return ms;
            }
        } 
        #endregion
    }
}
