using System;
using System.Collections.Generic;

using System.Windows.Forms;
using System.IO;
using System.Text;
using iTextSharp.text.pdf;
using iTextSharp.text;

using iTextSharp.text.pdf.parser;


namespace KRPDFbatch
{
    public partial class KrPdfBatchForm : Form
    {
        public string gsBatchNumber;
        public string gsReadPDF; //PDF vi ska läsa av och hitta batchnumret i
        public string gsWritePDF; //När vi hittat en sida i gsReadPDF som har gsBatchNumber ska vi skapa en PDF av denna sida
        FileStream gfsLog;
        StreamWriter gswLog;

        public List<int> ReadPdfFile(string fileName, String searthText)
        {
            List<int> pages = new List<int>();
            if (File.Exists(fileName))
            {
                PdfReader pdfReader = new PdfReader(fileName);
                for (int page = 1; page <= pdfReader.NumberOfPages; page++)
                {
                    ITextExtractionStrategy strategy = new SimpleTextExtractionStrategy();

                    string currentPageText = PdfTextExtractor.GetTextFromPage(pdfReader, page, strategy);
                    if (currentPageText.Contains(searthText))
                    {
                        pages.Add(page);
                    }
                }
                pdfReader.Close();
            }
            return pages;
        }

        public Int32 GetPageFromPdfFile(string fileName, String searchText)
        {
            Int32 pageNumber = 0;
            if (File.Exists(fileName))
            {
                
                PDFParser pdfParser = new PDFParser(); //Från klassen PDFParser som läser bytes istället

                PdfReader pdfReader = new PdfReader(fileName);

                for (int page = 1; page <= pdfReader.NumberOfPages; page++)
                {
                    ITextExtractionStrategy strategy = new SimpleTextExtractionStrategy();
                    //ITextExtractionStrategy strategy = new LocationTextExtractionStrategy();
                    //ITextExtractionStrategy strategy = new LocationTextExtractionStrategy();

                    string currentPageText = PdfTextExtractor.GetTextFromPage(pdfReader, page,strategy);
                    //currentPageText = Encoding.UTF8.GetString(ASCIIEncoding.Convert(Encoding.Default, Encoding.UTF8, Encoding.Default.GetBytes(currentPageText)));
                    if (currentPageText.Contains(searchText))
                    {
                        //MessageBox.Show("Hittad sökt text " + searchText + " på sida " + page);
                        pageNumber = page;
                        break;
                    }
                    else
                    {
                        currentPageText = pdfParser.ExtractTextFromPDFBytes(pdfReader.GetPageContent(page));
                        currentPageText = currentPageText.Replace("\n", "");
                        currentPageText = currentPageText.Replace("\r", "");
                        currentPageText = currentPageText.Replace(" ", "");
                        if (currentPageText.Contains(searchText))
                        {
                            //MessageBox.Show("Hittad sökt text " + searchText + " på sida " + page);
                            pageNumber = page;
                            break;
                        }
                        else
                        {
                            //No

                        }
                        // MessageBox.Show(currentPageText);
                    }
                }
                pdfReader.Close();
                if (pageNumber == 0)
                {
                    gswLog.Write("Ingen sida funnen. Antal sidor:" + pdfReader.NumberOfPages + " Sökt:" + searchText);
                }
            }
            else
            {
                gswLog.Write("Hittade inte pdf " + fileName);
            }
            return pageNumber;
        }

        public void ExtractPages(string sourcePdfPath, string outputPdfPath, int startPage, int endPage)
        {
            PdfReader reader = null;
            Document sourceDocument = null;
            PdfCopy pdfCopyProvider = null;
            PdfImportedPage importedPage = null;
            try
            {
                // Intialize a new PdfReader instance with the contents of the source Pdf file:
                reader = new PdfReader(sourcePdfPath);

                // For simplicity, I am assuming all the pages share the same size
                // and rotation as the first page:
                sourceDocument = new Document(reader.GetPageSizeWithRotation(startPage));

                // Initialize an instance of the PdfCopyClass with the source 
                // document and an output file stream:
                pdfCopyProvider = new PdfCopy(sourceDocument,
                    new System.IO.FileStream(outputPdfPath, System.IO.FileMode.Create));

                sourceDocument.Open();

                // Walk the specified range and add the page copies to the output file:
                for (int i = startPage; i <= endPage; i++)
                {
                    importedPage = pdfCopyProvider.GetImportedPage(reader, i);
                    pdfCopyProvider.AddPage(importedPage);
                }

                sourceDocument.Close();
                reader.Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public KrPdfBatchForm(String sBatchNumber,String sReadPDF,String sWritePDF)
        {
            InitializeComponent();
            gsBatchNumber = sBatchNumber;
            gsReadPDF = sReadPDF;
            gsWritePDF = sWritePDF;
        }

        private void KrPdfBatchForm_Load(object sender, EventArgs e)
        {
            Int32 page = 0;
            gfsLog = new FileStream(Application.StartupPath + "\\log.txt", FileMode.OpenOrCreate, FileAccess.Write);
            gswLog = new StreamWriter(gfsLog);
            gswLog.Write("####################Log " + DateTime.Now);
            
            page = GetPageFromPdfFile(gsReadPDF, gsBatchNumber);
            if (gsReadPDF != "" && gsWritePDF != "" && page > 0)
            {
                ExtractPages(gsReadPDF, gsWritePDF, page, page);
            }
            else if (page <= 0)
            {
                gswLog.Write("Hittade ingen sida med batchnummer " + gsBatchNumber + " i PDF " + gsReadPDF + " Erhållen sida:" + page.ToString());
               
            }
            gswLog.Close();
            Application.Exit();
        }
    }
}
