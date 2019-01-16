using System;

using System.Windows.Forms;

namespace KRPDFbatch
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        /// 
        static public string gsBatchNumber;
        static public string gsReadPDF; //PDF vi ska läsa av och hitta batchnumret i
        static public string gsWritePDF; //När vi hittat en sida i gsReadPDF som har gsBatchNumber ska vi skapa en PDF av denna sida

        [STAThread]
       
        static void Main(String[] args)
        {
            gsBatchNumber = "";
            gsReadPDF = "";
            gsWritePDF = "";

           
            if (args.Length > 2)
            {
                //MessageBox.Show("Det finns " + args.Length + " argument!");
                gsBatchNumber = args[0];
                gsReadPDF = args[1];
                gsWritePDF = args[2];
                //MessageBox.Show("Argument:" + args[0] + " " + args[1] + " " + args[2]);
            }
            else
            {
                //TESTER:
               // gsBatchNumber = "F6L4849C01";
               // gsReadPDF = Application.StartupPath + @"\test\_COSU6148627410.pdf";
               
                //gsBatchNumber = "FJGJ5A";
                //gsReadPDF = Application.StartupPath + @"\test\COSU6127979391.pdf";
               
               // gsBatchNumber = "355053964";
               // gsReadPDF = Application.StartupPath + @"\test\2015-77211.pdf";

               // gsBatchNumber = "AF18525B"; 
              //  gsReadPDF = Application.StartupPath + @"\test\565727393.pdf";

               // gsWritePDF = Application.StartupPath + @"\test\skapade\" + gsBatchNumber + ".pdf";
          
                //MessageBox.Show("Alla argument är inte medskickade!"); // Args:" + args[0] + " " + args[1] + " " + args[2]);
                Application.Exit();
            }

            if (gsBatchNumber != "" && gsReadPDF != "" && gsWritePDF != "")
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new KrPdfBatchForm(gsBatchNumber, gsReadPDF, gsWritePDF));
            }
        }
    }
}
