using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL.Models;

namespace BUS
{
    public class StudentService
    {
        private readonly StudentContextDB _context;

        public StudentService(StudentContextDB context)
        {
            _context = context;
        }

        public List<SinhVien> GetAllStudents()
        {
            return _context.SinhViens.Include("Lop").ToList();
        }

        public void AddStudent(SinhVien sv)
        {
            if (_context.SinhViens.Any(s => s.MaSV == sv.MaSV))
                throw new Exception("Mã sinh viên đã tồn tại!");

            _context.SinhViens.Add(sv);
            _context.SaveChanges();
        }

        public void UpdateStudent(SinhVien sv)
        {
            var existingStudent = _context.SinhViens.Find(sv.MaSV);
            if (existingStudent == null)
                throw new Exception("Không tìm thấy sinh viên!");

            existingStudent.HoTenSV = sv.HoTenSV;
            existingStudent.NgaySinh = sv.NgaySinh;
            existingStudent.MaLop = sv.MaLop;
            _context.SaveChanges();
        }

        public void DeleteStudent(string MaSV)
        {
            var student = _context.SinhViens.Find(MaSV);
            if (student == null)
                throw new Exception("Không tìm thấy sinh viên!");

            _context.SinhViens.Remove(student);
            _context.SaveChanges();
        }

        public List<SinhVien> SearchStudents(string searchText)
        {
            return _context.SinhViens
                .Include("Lop")
                .Where(s => s.HoTenSV.ToLower().Contains(searchText.ToLower()))
                .ToList();
        }
    }
}
