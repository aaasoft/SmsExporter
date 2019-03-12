﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace SmsExporter
{
    public class MainPageModel : BindableObject
    {
        private ISmsReader reader;

        private bool _IsEnable = true;
        public bool IsEnable
        {
            get { return _IsEnable; }
            set
            {
                _IsEnable = true;
                OnPropertyChanged(nameof(IsEnable));
            }
        }

        private string _Message = "";
        public string Message
        {
            get { return _Message; }
            set
            {
                _Message = value;
                OnPropertyChanged(nameof(Message));
            }
        }

        private double _Progress = 0;
        public double Progress
        {
            get { return _Progress; }
            set
            {
                _Progress = value;
                OnPropertyChanged(nameof(Progress));
            }
        }
        public ICommand ExportCommand { get; set; }
        public ICommand TestCommand { get; set; }


        public MainPageModel(ISmsReader reader)
        {
            this.reader = reader;
            ExportCommand = new DelegateCommand() { ExecuteCommand = executeExportCommand };
            TestCommand = new DelegateCommand() { ExecuteCommand = executeTestCommand };
        }

        private void executeTestCommand(object obj)
        {
            Message = reader.GetFolderPath();
        }

        private async void executeExportCommand(object obj)
        {
            IsEnable = false;
            await Task.Run(() =>
            {
                try
                {
                    reader.CheckPermission();

                    var filePath = Path.Combine(reader.GetFolderPath(), "SmsExport-" + DateTime.Now.ToString("yyyyMMdd-HHmmss") + ".xlsx");
                    var workbook = new NPOI.XSSF.UserModel.XSSFWorkbook();
                    var sheet = workbook.CreateSheet("Sheet1");
                    //第一行
                    {
                        var row = sheet.CreateRow(0);
                        var cColumnInd = 0;

                        row.CreateCell(cColumnInd).SetCellValue("编号");
                        cColumnInd++;
                        row.CreateCell(cColumnInd).SetCellValue("号码");
                        cColumnInd++;
                        row.CreateCell(cColumnInd).SetCellValue("时间");
                        cColumnInd++;
                        row.CreateCell(cColumnInd).SetCellValue("类型");
                        cColumnInd++;
                        row.CreateCell(cColumnInd).SetCellValue("内容");
                        cColumnInd++;
                    }
                    //当前行号
                    var cRowInd = 1;

                    var totalCount = reader.GetCount();
                    var enumerator = reader.GetEnumerator();
                    var index = 0;
                    while (enumerator.MoveNext())
                    {
                        index++;
                        var item = enumerator.Current;
                        var typeStr = "";
                        switch(item.ItemType)
                        {
                            case SmsItem.SmsItemType.Send:
                                typeStr = "发送";
                                break;
                            case SmsItem.SmsItemType.Recv:
                                typeStr = "接收";
                                break;
                            default:
                                continue;
                        }
                        {
                            var cColumnInd = 0;

                            var row = sheet.CreateRow(cRowInd);
                            NPOI.SS.UserModel.ICell cell = null;

                            cell = row.CreateCell(cColumnInd);
                            cell.SetCellValue(item.Id);
                            cell.SetCellType(NPOI.SS.UserModel.CellType.String);
                            cColumnInd++;

                            cell = row.CreateCell(cColumnInd);
                            cell.SetCellValue(item.Address);
                            cell.SetCellType(NPOI.SS.UserModel.CellType.String);
                            cColumnInd++;

                            cell = row.CreateCell(cColumnInd);
                            cell.SetCellValue(item.Date.ToString("yyyy-MM-dd HH:mm:ss"));
                            cell.SetCellType(NPOI.SS.UserModel.CellType.String);
                            cColumnInd++;

                            cell = row.CreateCell(cColumnInd);
                            cell.SetCellValue(typeStr);
                            cell.SetCellType(NPOI.SS.UserModel.CellType.String);
                            cColumnInd++;

                            cell = row.CreateCell(cColumnInd);
                            cell.SetCellValue(item.Body);
                            cell.SetCellType(NPOI.SS.UserModel.CellType.String);
                            cColumnInd++;
                        }
                        cRowInd++;
                        Progress = index * 1D / totalCount;
                        Message = $"读取短信[{index} / {totalCount}]中...";
                    }
                    Message = $"正在保存到文件...";
                    using (var stream = File.OpenWrite(filePath))
                        workbook.Write(stream);
                    Message = $"已保存到{filePath}";
                }
                catch (Exception ex)
                {
                    Message = "错误," + ex;
                }
            });
            IsEnable = true;
        }
    }
}
