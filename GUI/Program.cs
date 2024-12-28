using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using BUS;
using DAL.Models;

namespace GUI
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            var dbContext = new StudentContextDB();

            var studentService = new StudentService(dbContext);
            var studentBUS = new StudentBUS(dbContext);

            Application.Run(new frmStudent(studentService, studentBUS));
        }
    }
}
