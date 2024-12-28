using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using BUS;
using DAL.Models;


namespace GUI
{
    public partial class frmStudent : Form
    {

        private readonly StudentService _studentService;
        private readonly StudentBUS _studentBUS;    
        public frmStudent(StudentService studentService, StudentBUS studentBUS)
        {
            InitializeComponent();
            _studentService = studentService;
            _studentBUS = studentBUS;
        }

        private void frmStudent_Load(object sender, EventArgs e)
        {
            LoadStudents();
            LoadClasses();
            ConfigureListView();

            txtStudentID.TextChanged += ControlDataChanged;
            txtStudentName.TextChanged += ControlDataChanged;
            dateTimePickerDateOfBirth.ValueChanged += ControlDataChanged;
            cmbClass.SelectedIndexChanged += ControlDataChanged;
        }

        private void ConfigureListView()
        {
            listViewStudent.View = View.Details;
            listViewStudent.FullRowSelect = true;
            listViewStudent.GridLines = true;

            listViewStudent.Columns.Add("Mã SV", 100);
            listViewStudent.Columns.Add("Họ và tên", 150);
            listViewStudent.Columns.Add("Ngày sinh", 120);
            listViewStudent.Columns.Add("Lớp", 100);
        }

        private void LoadStudents()
        {
            listViewStudent.Items.Clear();
            var students = _studentService.GetAllStudents();

            foreach (var student in students)
            {
                var item = new ListViewItem(student.MaSV);
                item.SubItems.Add(student.HoTenSV);
                item.SubItems.Add(student.NgaySinh?.ToString("yyyy-MM-dd"));
                item.SubItems.Add(student.Lop?.TenLop ?? "");
                listViewStudent.Items.Add(item);
            }
        }
        private void LoadClasses()
        {
            var classes = _studentBUS.GetAllClasses();

            // Kiểm tra nếu danh sách lớp bị null hoặc rỗng
            if (classes == null || !classes.Any())
            {
                MessageBox.Show("Không có dữ liệu lớp học!", "Thông báo");
                cmbClass.DataSource = null;
                return;
            }

            // Cập nhật DataSource cho ComboBox
            cmbClass.DataSource = classes;
            cmbClass.DisplayMember = "TenLop"; // Hiển thị tên lớp
            cmbClass.ValueMember = "MaLop";   // Lấy giá trị là mã lớp
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (!ValidateInput()) return;

            var student = new SinhVien
            {
                MaSV = txtStudentID.Text,
                HoTenSV = txtStudentName.Text,
                NgaySinh = dateTimePickerDateOfBirth.Value,
                MaLop = cmbClass.SelectedValue.ToString()
            };

            try
            {
                _studentService.AddStudent(student);
                LoadStudents();
                MessageBox.Show("Thêm sinh viên thành công!", "Thông báo");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Lỗi");
            }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (!ValidateInput()) return;

            var student = new SinhVien
            {
                MaSV = txtStudentID.Text,
                HoTenSV = txtStudentName.Text,
                NgaySinh = dateTimePickerDateOfBirth.Value,
                MaLop = cmbClass.SelectedValue.ToString()
            };

            try
            {
                _studentService.UpdateStudent(student);
                LoadStudents();
                MessageBox.Show("Cập nhật sinh viên thành công!", "Thông báo");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Lỗi");
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            var studentID = txtStudentID.Text;

            if (string.IsNullOrEmpty(studentID))
            {
                MessageBox.Show("Vui lòng chọn sinh viên để xóa!", "Lỗi");
                return;
            }

            var result = MessageBox.Show(
                $"Bạn có chắc chắn muốn xóa sinh viên với ID {studentID} không?",
                "Xác nhận xóa",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                try
                {
                    _studentService.DeleteStudent(studentID);
                    LoadStudents();
                    MessageBox.Show("Xóa sinh viên thành công!", "Thông báo");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Lỗi khi xóa sinh viên: {ex.Message}", "Lỗi");
                }
            }
        }

        private bool ValidateInput()
        {
            if (string.IsNullOrWhiteSpace(txtStudentID.Text))
            {
                MessageBox.Show("Vui lòng nhập mã sinh viên!", "Lỗi");
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtStudentName.Text))
            {
                MessageBox.Show("Vui lòng nhập tên sinh viên!", "Lỗi");
                return false;
            }

            if (cmbClass.SelectedIndex == -1)
            {
                MessageBox.Show("Vui lòng chọn lớp!", "Lỗi");
                return false;
            }

            return true;
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            var searchText = txtSearch.Text;

            var students = _studentService.SearchStudents(searchText);

            listViewStudent.Items.Clear();
            foreach (var student in students)
            {
                var item = new ListViewItem(student.MaSV);
                item.SubItems.Add(student.HoTenSV);
                item.SubItems.Add(student.NgaySinh?.ToString("yyyy-MM-dd"));
                item.SubItems.Add(student.Lop?.TenLop ?? "");
                listViewStudent.Items.Add(item);
            }
        }

        private void listViewStudent_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listViewStudent.SelectedItems.Count > 0)
            {
                var selectedItem = listViewStudent.SelectedItems[0];
                txtStudentID.Text = selectedItem.SubItems[0].Text;
                txtStudentName.Text = selectedItem.SubItems[1].Text;

                if (DateTime.TryParse(selectedItem.SubItems[2].Text, out DateTime dateOfBirth))
                {
                    dateTimePickerDateOfBirth.Value = dateOfBirth;
                }
                else
                {
                    dateTimePickerDateOfBirth.Value = DateTime.Now;
                }

                var className = selectedItem.SubItems[3].Text;
                var classList = _studentBUS.GetAllClasses();
                var classObj = classList.FirstOrDefault(c => c.TenLop == className);

                if (classObj != null)
                {
                    cmbClass.SelectedValue = classObj.MaLop;
                }

                btnSave.Enabled = false;
                btnNoSave.Enabled = false;
            }
        }

        private bool isDataChanged = false;

        private void ControlDataChanged(object sender, EventArgs e)
        {
            isDataChanged = true;
            btnSave.Enabled = true;
            btnNoSave.Enabled = true;
        }
        private void btnSave_Click(object sender, EventArgs e)
        {
            if (isDataChanged)
            {
                var student = new SinhVien
                {
                    MaSV = txtStudentID.Text,
                    HoTenSV = txtStudentName.Text,
                    NgaySinh = dateTimePickerDateOfBirth.Value,
                    MaLop = cmbClass.SelectedValue.ToString()
                };

                DialogResult result = MessageBox.Show("Bạn có chắc muốn lưu thay đổi?", "Xác nhận", MessageBoxButtons.YesNo);
                if (result == DialogResult.Yes)
                {
                    try
                    {
                        _studentService.UpdateStudent(student);
                        LoadStudents();
                        MessageBox.Show("Lưu thay đổi thành công!", "Thông báo");
                        isDataChanged = false;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Lỗi khi lưu thay đổi: {ex.Message}", "Lỗi");
                    }
                }

                btnSave.Enabled = false;
                btnNoSave.Enabled = false;
            }
        }

        private void btnNoSave_Click(object sender, EventArgs e)
        {
            LoadStudents();
            MessageBox.Show("Thay đổi đã được hủy!", "Thông báo");
            btnSave.Enabled = false;
            btnNoSave.Enabled = false;
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show(
                "Bạn có chắc chắn muốn đóng ứng dụng?",
                "Xác nhận",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
           );

            if (result == DialogResult.Yes)
            {
                this.Close();
            }   
        }

        private void frmStudent_FormClosing(object sender, FormClosingEventArgs e)
        {
            DialogResult result = MessageBox.Show(
              "Bạn có chắc chắn muốn đóng ứng dụng?",
              "Xác nhận",
              MessageBoxButtons.YesNo,
              MessageBoxIcon.Question
          );

            if (result == DialogResult.No)
            {
                e.Cancel = true;
            }
        }
    }
}
