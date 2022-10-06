using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;
using PdfSharp.Drawing.Layout;
using PdfSharp;
using PdfSharp.Pdf.AcroForms;
using PdfSharp.Pdf.Advanced;
using System.IO;
using PdfSharp.Pdf.Annotations;
using System.Drawing.Text;
using Pdfer.Common;

namespace Pdfer.Services
{
    public static class PdfService
    {
        //注册字体
        static System.Text.EncodingProvider p = System.Text.CodePagesEncodingProvider.Instance;


        public static void PdfDivision(string srcPath, string dstPath)
        {
            Encoding.RegisterProvider(p);
            using (var fileStream = File.OpenRead(srcPath))
            {
                var inputDocument = PdfReader.Open(fileStream, PdfDocumentOpenMode.Import);
                var pageNo = 0;
                foreach (var page in inputDocument.Pages)
                {
                    pageNo++;
                    var outputDocument = new PdfDocument();
                    //outputDocument.PageLayout = PdfPageLayout.SinglePage;
                    var newPage = outputDocument.AddPage(page);
                    var gfx = XGraphics.FromPdfPage(newPage);

                    if (!Directory.Exists(dstPath))
                    {
                        Directory.CreateDirectory(dstPath);
                    }

                    outputDocument.Save(dstPath + @"\\Page-" + pageNo + ".pdf");
                }
            }
        }

        public static void PdfCombine(string[] srcPathFiles, string dstPathFile)
        {
            Encoding.RegisterProvider(p);
            var outputDocument = new PdfDocument();
            foreach (var spf in srcPathFiles)
            {
                using (var fileStream = File.OpenRead(spf))
                {
                    var inputDocument = PdfReader.Open(fileStream, PdfDocumentOpenMode.Import);
                    var pageNo = 0;
                    foreach (var page in inputDocument.Pages)
                    {
                        pageNo++;
                        var newPage = outputDocument.AddPage(page);
                    }
                }
            }
            outputDocument.Save(dstPathFile);
        }
        public static void PdfGetInfo(string srcPathFile, out List<Tuple<int, double, double>> pageInfoCollection)
        {

            Encoding.RegisterProvider(p);
            pageInfoCollection = new List<Tuple<int, double, double>>();
            var outputDocument = new PdfDocument();
            using (var fileStream = File.OpenRead(srcPathFile))
            {
                var inputDocument = PdfReader.Open(fileStream, PdfDocumentOpenMode.Import);
                int pageNo = 0;
                foreach (var page in inputDocument.Pages)
                {
                    pageNo++;
                    var pageInfo = new Tuple<int, double, double>(pageNo, page.Width, page.Height);
                    pageInfoCollection.Add(pageInfo);
                }

            }
        }

        public static void PdfSplicling(string[] srcPathFiles, string dstPathFile, SpliclingParam param)
        {
            var maxRow = param.MaxRow;
            var left = param.Left;
            var right = param.Right;
            var top = param.Top;
            var bottom = param.Bottom;
            //字体相关
            System.Text.EncodingProvider ppp = System.Text.CodePagesEncodingProvider.Instance;
            Encoding.RegisterProvider(ppp);
            PrivateFontCollection pfcFonts = new PrivateFontCollection();
            string strFontPath = Environment.GetFolderPath(Environment.SpecialFolder.Windows) + "/Fonts/SIMFANG.TTF";
            pfcFonts.AddFontFile(strFontPath);



            var outputDocument = new PdfDocument();
            outputDocument.PageLayout = PdfPageLayout.SinglePage;

            int leftFile = srcPathFiles.Count();
            int currentFile = 0;
            int pageRow = 0;
            //var newPage = outputDocument.AddPage();
            //var gfx = XGraphics.FromPdfPage(newPage);
            //newPage.Size = PageSize.A4;

            var tempOutputDocument_A = new PdfDocument();
            var tempPage_X = tempOutputDocument_A.AddPage();

            foreach (var file in srcPathFiles)
            {
                leftFile--;
                currentFile++;
                using (var fileSteam = File.OpenRead(file))
                {
                    var fileExtName = file.Substring(file.Length - 4, 4);
                    if (fileExtName.Equals(".pdf", StringComparison.CurrentCultureIgnoreCase))
                    {

                        var inputDocument = PdfReader.Open(fileSteam, PdfDocumentOpenMode.Import);
                        int leftPage = inputDocument.PageCount;
                        foreach (PdfPage page in inputDocument.Pages)
                        {

                            leftPage--;

                            if (pageRow < maxRow)
                            {
                                pageRow++;
                            }
                            else
                            {
                                pageRow = 0;
                                pageRow++;
                            }

                            var height = page.Height - top - bottom;
                            var startPointY = height * (pageRow - 1);

                            if (pageRow == 1 && currentFile != 1)
                            {
                                tempPage_X = tempOutputDocument_A.InsertPage(0);
                                tempPage_X.Width = page.Width - left - right;
                                tempPage_X.Height = (page.Height - top - bottom) * maxRow;
                            }
                            else
                            {
                                tempPage_X.Width = page.Width - left - right;
                                tempPage_X.Height = (page.Height - top - bottom) * maxRow;
                            }



                            //step 1 : Page->PdfFile temp 1
                            var stream_A = new MemoryStream();
                            tempOutputDocument_A.PageLayout = PdfPageLayout.SinglePage;


                            var tempPage_A = tempOutputDocument_A.AddPage(page);
                            tempPage_A.Width = page.Width;
                            tempPage_A.Height = page.Height;
                            tempOutputDocument_A.Save(stream_A);

                            var tempOutputDocument_B = new PdfDocument();
                            tempOutputDocument_B.AddPage(page);
                            MemoryStream Stream_Img = new MemoryStream();
                            tempOutputDocument_B.Save(Stream_Img);


                            //tempOutputDocument_A.Save(string.Format(@"C:\Users\26712\Desktop\2\temp1(AddPage后).pdf"));


                            //step 2 : temp 1 cut top and bottom -> Pdf temp 2

                            var tempPage_B = tempOutputDocument_A.AddPage();
                            tempPage_B.Width = page.Width;
                            tempPage_B.Height = page.Height;
                            TrimMargins(ref tempPage_B, top, bottom, left, right);
                            tempOutputDocument_A.Save(string.Format(@"C:\Users\26712\Desktop\2\temp2(newPage2裁剪后).pdf"));


                            //step 3 : temp 2 write Img
                            XImage tempImg = XImage.FromStream(Stream_Img);
                            XPdfForm temp = XPdfForm.FromFile(@"C:\Users\26712\Desktop\2\temp1(AddPage后).pdf");
                            var tempBox = new XRect(0, 0, tempImg.PointWidth, page.Height);
                            var tempGfx = XGraphics.FromPdfPage(tempPage_B);
                            tempGfx.DrawImage(tempImg, tempBox);
                            foreach (PdfAnnotation item in tempPage_A.Annotations)
                            {
                                tempPage_B.Annotations.Add(item.Clone() as PdfAnnotation);
                            }

                            var stream_B = new MemoryStream();
                            tempOutputDocument_A.Save(stream_B);
                            tempOutputDocument_A.Save(string.Format(@"C:\Users\26712\Desktop\2\temp3(temp2绘制后).pdf"));

                            var tempOutputDocument_C = new PdfDocument();
                            var tempPage_C = tempOutputDocument_C.AddPage();
                            tempPage_C.Width = page.Width - left - right;
                            tempPage_C.Height = page.Height - top - bottom;
                            TrimMargins(ref tempPage_B, top, bottom, left, right);
                            var Gfx_Img_Cut = XGraphics.FromPdfPage(tempPage_C);
                            Gfx_Img_Cut.DrawImage(tempImg, -left, -top);
                            MemoryStream Stream_Img_Cut = new MemoryStream();
                            tempOutputDocument_C.Save(Stream_Img_Cut);
                            tempOutputDocument_C.Save(string.Format(@"C:\Users\26712\Desktop\2\Stream_Img_Cut.pdf"));

                            //step 4 : temp 2 -> newPage ( 从Y轴起始位置开始绘制 )
                            XImage img = XImage.FromStream(Stream_Img_Cut);
                            foreach (PdfAnnotation item in tempPage_B.Annotations)
                            {
                                var x1 = item.Rectangle.X1 -  left;
                                var x2 = item.Rectangle.X2 -  left;
                                var y1 = item.Rectangle.Y1 + ((maxRow - pageRow) * height - bottom);
                                var y2 = item.Rectangle.Y2 + ((maxRow - pageRow) * height - bottom);
                                var xR = new XRect(x1, y1, x2 - x1, y2 - y1);
                                PdfRectangle rectangle = new PdfRectangle(xR);
                                var item_x = item.Clone();
                                item_x.Elements.SetRectangle("/Rect", rectangle);
                                tempPage_X.Annotations.Add(item_x as PdfAnnotation);
                            }
                            var gfx_X = XGraphics.FromPdfPage(tempPage_X);
                            gfx_X.DrawImage(img, 0, startPointY);

                            tempOutputDocument_A.Save(string.Format(@"C:\Users\26712\Desktop\2\temp4(temp3绘制后).pdf"));

                            tempOutputDocument_A.Pages.RemoveAt(1);
                            tempOutputDocument_A.Pages.RemoveAt(1);
                            MemoryStream stream_C = new MemoryStream();
                            tempOutputDocument_A.Save(stream_C);

                            if (pageRow == maxRow || (leftPage == 0 && leftFile == 0))
                            {
                                var inputDocument_X = PdfReader.Open(stream_C, PdfDocumentOpenMode.Import);
                                outputDocument.AddPage(inputDocument_X.Pages[0]);
                                tempOutputDocument_A.Pages.RemoveAt(0);
                            }
                        }
                    }
                }


            }
            var fieldsArry = new PdfArray();
            foreach (var item in outputDocument.Internals.GetAllObjects())
            {
                if (item is PdfDictionary)
                {
                    var ft = (item as PdfDictionary).Elements[PdfAcroField.Keys.FT];
                    if (ft != null && ft is PdfName && (ft as PdfName).Value == " / Sig")
                    {
                        fieldsArry.Elements.Add(PdfInternals.GetReference(item));
                    }
                }
            }
            var dic = new PdfDictionary(outputDocument);
            dic.Elements[PdfAcroForm.Keys.Fields] = fieldsArry;
            dic.Elements[PdfAcroForm.Keys.SigFlags] = new PdfInteger(3);
            outputDocument.Internals.AddObject(dic);
            outputDocument.Internals.Catalog.Elements["/AcroForm"] = PdfInternals.GetReference(dic);
            //outputDocument.Save(@"C:\Users\kingdee\Desktop\2.pdf");
            outputDocument.Save(dstPathFile);
        }

        public static void TrimMargins(ref PdfPage tranPage, double topCut, double bottomCut, double left, double right)
        {
            var topCutXunit = XUnit.FromPoint(topCut);
            var bottomCutXunit = XUnit.FromPoint(bottomCut);
            tranPage.TrimMargins.Top = -(topCutXunit);
            tranPage.TrimMargins.Bottom = -(bottomCutXunit / 2) + (topCutXunit / 2);
            tranPage.TrimMargins.Left = -left;
            tranPage.TrimMargins.Right = -(right / 2) + (left / 2);
        }
    }

}
