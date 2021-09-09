using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Threading.Tasks;

namespace yutubeTapra
{
    class Program
    {
        private static string backExeclPath = @".\table\YOUTUB以处理完成的数据.xlsx";
        static void Main(string[] args)
        {
            try
            {
                //读新表格的数据
                //Provider = Microsoft.ACE.OLEDB.12.0
                Console.WriteLine("开始读取表格中的数据内容");
                string FileFullPath = @".\table\YOUTUBE-SZ 历史汇总最新.xlsx";
                string strConn = "Provider=Microsoft.ACE.OLEDB.12.0;" + "Data Source=" + FileFullPath + ";" + "Extended Properties=Excel 8.0;";
                OleDbConnection conn = new OleDbConnection(strConn);
                conn.Open();
                string strExcel = "";
                OleDbDataAdapter myCommand = null;
                DataSet ds = null;
                strExcel = "select * from [YOUTUBE开发登记表$]";
                myCommand = new OleDbDataAdapter(strExcel, strConn);
                ds = new DataSet();
                myCommand.Fill(ds, "table1");
                var da = ds.Tables[0].Rows.Count;
                conn.Close();
                conn.Dispose();

                int daLog = 0;
                //如果有旧表格就读旧表格的数据
                if (File.Exists(backExeclPath))
                {
                    string strConnLog = "Provider=Microsoft.ACE.OLEDB.12.0;" + "Data Source=" + backExeclPath + ";" + "Extended Properties=Excel 8.0;";
                    OleDbConnection connLog = new OleDbConnection(strConnLog);
                    connLog.Open();
                    string strExcelLog = "";
                    OleDbDataAdapter myCommandLog = null;
                    DataSet dsLog = null;
                    strExcelLog = "select * from [sheet1$]";
                    myCommandLog = new OleDbDataAdapter(strExcelLog, strConnLog);
                    dsLog = new DataSet();
                    myCommandLog.Fill(dsLog, "table2");
                    int pp= dsLog.Tables[0].Rows.Count-1;
                    daLog = int.Parse(dsLog.Tables[0].Rows[pp][0].ToString()) -1;
                    connLog.Close();
                    connLog.Dispose();
                }

                //daLog = daLog + 1; //要从下一条开始
                for (int i = daLog; i < da; i++)
                {
                    if (ds.Tables[0].Rows[i]["LINK"].ToString() == "" || ds.Tables[0].Rows[i]["DATE"].ToString() == "")
                    {
                        Console.WriteLine("没有数据了 请检查！");
                        return;
                    }

                    //处理字符两边的空格
                    ds.Tables[0].Rows[i]["CREATOR"] = ds.Tables[0].Rows[i]["CREATOR"].ToString().Trim();
                    ds.Tables[0].Rows[i]["USERNAME"] = ds.Tables[0].Rows[i]["USERNAME"].ToString().Trim();
                    ds.Tables[0].Rows[i]["LINK"] = ds.Tables[0].Rows[i]["LINK"].ToString().Trim();
                    ds.Tables[0].Rows[i]["EMAIL"] = ds.Tables[0].Rows[i]["EMAIL"].ToString().Trim();
                    ds.Tables[0].Rows[i]["EMAIL1"] = ds.Tables[0].Rows[i]["EMAIL1"].ToString().Trim();

                    Console.WriteLine($"--------{(i+1).ToString()}----------");
                    //Console.WriteLine(ds.Tables[0].Rows[i]["DATE"]);
                    //Console.WriteLine(ds.Tables[0].Rows[i]["CREATOR"]);
                    //Console.WriteLine(ds.Tables[0].Rows[i]["USERNAME"]);
                    //Console.WriteLine(ds.Tables[0].Rows[i]["LINK"]);
                    //Console.WriteLine(ds.Tables[0].Rows[i]["EMAIL"]);
                    //Console.WriteLine(ds.Tables[0].Rows[i]["EMAIL1"]);

                    string IsGood = string.Empty;
                    string ChnnalId = "待处理";
                    string sliptChnnal = string.Empty;

                    string linking = "";
                    if (ds.Tables[0].Rows[i]["LINK"].ToString().Length > 32)
                    {
                        linking = ds.Tables[0].Rows[i]["LINK"].ToString().Substring(0, 32);
                    }
                    else 
                    {
                        linking = ds.Tables[0].Rows[i]["LINK"].ToString();
                    }
                  
                    if (linking != "https://www.youtube.com/channel/")
                    {
                        ChnnalId = StartSelenium.StartSeleniumGo(ds.Tables[0].Rows[i]["LINK"].ToString());
                        if (ChnnalId.Length >= 32&& ChnnalId!="no")
                        {
                            sliptChnnal = ChnnalId.Substring(32);
                        }
                        else
                        {
                            WriteDatas(ds.Tables[0], i, "此地址有问题", "有问题");
                            continue;
                        }
                        
                    }
                    else
                    {
                        sliptChnnal = ds.Tables[0].Rows[i]["LINK"].ToString().Substring(32);
                    }

                    if (sliptChnnal.Contains('/'))
                    {
                        sliptChnnal = sliptChnnal.Split('/')[0];
                    }

                    //插入数据库
                    IsGood = CreateDB.ConnsqlChnnel("https://www.youtube.com", "", sliptChnnal, ds.Tables[0].Rows[i], "https://www.youtube.com/" + sliptChnnal.Trim(), FileFullPath, i);

                    bool v = WriteDatas(ds.Tables[0], i, sliptChnnal, IsGood);
                }
            }
            catch (Exception ex)
            {
                throw new AggregateException(ex.Message);
            }
        }


        public static bool WriteDatas(DataTable da, int jk, string ChnnalId, string IsGoodDb)
        {
            IWorkbook workbookData = null;
            ISheet sheetdata = null;
            if (!File.Exists(backExeclPath))
            {
                ExeclCreate(da, jk, ChnnalId, IsGoodDb);
            }
            else
            {
                FileStream fs = new FileStream(backExeclPath, FileMode.Open, FileAccess.Read, FileShare.Write);
                IWorkbook workbook = WorkbookFactory.Create(fs);
                ISheet sheet = null;
                if (!string.IsNullOrEmpty("sheet1"))
                {
                    sheet = workbook.GetSheet("sheet1");
                    if (sheet == null)
                    {
                        sheet = workbook.GetSheetAt(0);
                    }
                }
                else
                {
                    sheet = workbook.GetSheetAt(0);
                }
                sheetdata = sheet;
                workbookData = workbook;
                fs.Close();
                fs.Dispose();
                OpenWrite(da, jk, workbookData, sheetdata, ChnnalId, IsGoodDb);
            }
            return true;
        }

        /// <summary>
        /// 创建写入
        /// </summary>
        /// <param name="da"></param>
        /// <param name="jk"></param>
        /// <returns></returns>
        public static bool ExeclCreate(DataTable da, int jk, string ChnnalId, string IsGoodDb)
        {
            FileStream stream = new FileStream(backExeclPath, FileMode.Create, FileAccess.Write);
            IWorkbook wb = new XSSFWorkbook();
            ISheet sheet = null;
            sheet = wb.CreateSheet("Sheet1");
            sheet.SetColumnWidth(1, 20 * 256);
            sheet.SetColumnWidth(2, 20 * 256);
            sheet.SetColumnWidth(3, 15 * 256);
            sheet.SetColumnWidth(4, 70 * 256);
            sheet.SetColumnWidth(5, 35 * 256);
            sheet.SetColumnWidth(6, 22 * 256);
            sheet.SetColumnWidth(7, 10 * 256);
            sheet.SetColumnWidth(8, 35 * 256);
            sheet.SetColumnWidth(9, 20 * 256);

            ICreationHelper cH = wb.GetCreationHelper();
            IRow row = sheet.CreateRow(jk);
            //row.Height=500;
            //Id
            ICell cell = row.CreateCell(0);
            cell.SetCellValue(cH.CreateRichTextString((jk+2).ToString()));
            cell.CellStyle.ShrinkToFit = true;

            //时间
            ICell cell1 = row.CreateCell(1);
            cell1.SetCellValue(cH.CreateRichTextString(da.Rows[jk]["DATE"].ToString()));
            cell1.CellStyle.ShrinkToFit = true;
            //名称
            ICell cell2 = row.CreateCell(2);
            cell2.SetCellValue(cH.CreateRichTextString(da.Rows[jk]["USERNAME"].ToString()));

            //操作人
            ICell cell13 = row.CreateCell(3);
            cell13.SetCellValue(cH.CreateRichTextString(da.Rows[jk]["CREATOR"].ToString()));

            //地址
            ICell cell4 = row.CreateCell(4);
            cell4.SetCellValue(cH.CreateRichTextString(da.Rows[jk]["LINK"].ToString()));
            //邮箱
            ICell cell5 = row.CreateCell(5);
            cell5.SetCellValue(cH.CreateRichTextString(da.Rows[jk]["EMAIL"].ToString()));

            ICell cell6 = row.CreateCell(6);
            cell6.SetCellValue(cH.CreateRichTextString(da.Rows[jk]["EMAIL1"].ToString()));

            //状态
            ICell cell7 = row.CreateCell(7);
            cell7.SetCellValue(cH.CreateRichTextString("已完成"));
            //解析后的ChnnalId
            ICell cell8 = row.CreateCell(8);
            cell8.SetCellValue(cH.CreateRichTextString(ChnnalId));

            //是否成功插入到数据库
            ICell cell19 = row.CreateCell(9);
            cell19.SetCellValue(cH.CreateRichTextString(IsGoodDb));

            wb.Write(stream);

            stream.Close();
            stream.Dispose();
            Console.WriteLine($"第{jk + 1}写入成功！ 解析后的 ChnnalId： {ChnnalId}");
            return true;
        }

        /// <summary>
        /// 打开写入
        /// </summary>
        /// <param name="da"></param>
        /// <param name="jk"></param>
        /// <returns></returns>
        public static bool OpenWrite(DataTable da, int jk, IWorkbook wb, ISheet sheet, string ChnnalId, string IsGoodDb)
        {
            FileStream stream = new FileStream(backExeclPath, FileMode.Open, FileAccess.Write);
            //IWorkbook wb = new XSSFWorkbook();
            //ISheet sheet = null;
            //sheet = wb.CreateSheet("Sheet1");

            ICreationHelper cH = wb.GetCreationHelper();
            IRow row = sheet.CreateRow(jk);

            //Id
            ICell cell = row.CreateCell(0);
            cell.SetCellValue(cH.CreateRichTextString((jk+2).ToString()));
            cell.CellStyle.ShrinkToFit = true;

            //时间
            ICell cell1 = row.CreateCell(1);
            cell1.SetCellValue(cH.CreateRichTextString(da.Rows[jk]["DATE"].ToString()));
            cell1.CellStyle.ShrinkToFit = true;
            //名称
            ICell cell2 = row.CreateCell(2);
            cell2.SetCellValue(cH.CreateRichTextString(da.Rows[jk]["USERNAME"].ToString()));

            //操作人
            ICell cell13 = row.CreateCell(3);
            cell13.SetCellValue(cH.CreateRichTextString(da.Rows[jk]["CREATOR"].ToString()));

            //地址
            ICell cell4 = row.CreateCell(4);
            cell4.SetCellValue(cH.CreateRichTextString(da.Rows[jk]["LINK"].ToString()));
            //邮箱
            ICell cell5 = row.CreateCell(5);
            cell5.SetCellValue(cH.CreateRichTextString(da.Rows[jk]["EMAIL"].ToString()));

            ICell cell6 = row.CreateCell(6);
            cell6.SetCellValue(cH.CreateRichTextString(da.Rows[jk]["EMAIL1"].ToString()));

            //状态
            ICell cell7 = row.CreateCell(7);
            cell7.SetCellValue(cH.CreateRichTextString("已完成"));
            //解析后的ChnnalId
            ICell cell8 = row.CreateCell(8);
            cell8.SetCellValue(cH.CreateRichTextString(ChnnalId));

            //是否成功插入到数据库
            ICell cell19 = row.CreateCell(9);
            cell19.SetCellValue(cH.CreateRichTextString(IsGoodDb));

            wb.Write(stream);
            stream.Close();
            stream.Dispose();
            Console.WriteLine($"第{jk + 1}写入成功！ 解析后的 ChnnalId： {ChnnalId}");
            return true;
        }


        public static bool OpenWriteYouTuBe(string backExeclPath, int i)
        {
            IWorkbook workbookData = null;
            ISheet sheetdata = null;

            FileStream fs = new FileStream(backExeclPath, FileMode.Open, FileAccess.Read, FileShare.Write);
            IWorkbook workbook = WorkbookFactory.Create(fs);
            ISheet sheet = null;
            if (!string.IsNullOrEmpty("YOUTUBE开发登记表"))
            {
                sheet = workbook.GetSheet("YOUTUBE开发登记表");
                if (sheet == null)
                {
                    sheet = workbook.GetSheetAt(0);
                }
            }
            else
            {
                sheet = workbook.GetSheetAt(0);
            }
            sheetdata = sheet;
            workbookData = workbook;
            workbook.Write(fs);
            fs.Close();
            fs.Dispose();

            FileStream stream = new FileStream(backExeclPath, FileMode.Open, FileAccess.Write);
            ICreationHelper cH = workbookData.GetCreationHelper();
            IRow row = sheet.CreateRow(i + 1);

            ICell cell = row.CreateCell(7);
            cell.SetCellValue(cH.CreateRichTextString("true"));

            workbookData.Write(stream);
            stream.Close();
            stream.Dispose();

            return true;
        }

    }
}

