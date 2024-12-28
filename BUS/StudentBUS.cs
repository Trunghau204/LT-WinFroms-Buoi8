using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL.Models;

namespace BUS
{
    public class StudentBUS
    {
        private readonly StudentContextDB _context;
        public StudentBUS(StudentContextDB context)
        {
            _context = context;
        }

        public List<Lop> GetAllClasses()
        {
            var classes = _context.Lops
            .Where(l => !string.IsNullOrEmpty(l.MaLop) && !string.IsNullOrEmpty(l.TenLop))
            .ToList();

            if (!classes.Any())
            {
                throw new Exception("Dữ liệu lớp không hợp lệ hoặc trống.");
            }

            return classes;
        }
    }
}
