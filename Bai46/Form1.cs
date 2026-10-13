
using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Bai46
{
    public partial class Form1 : Form
    {
        private BindingList<Student> students =
            new BindingList<Student>();

        private BindingSource source = new BindingSource();

        private DataGridView dgv = new DataGridView();

        private TextBox txtId = new TextBox();
        private TextBox txtName = new TextBox();
        private TextBox txtAge = new TextBox();

        private ComboBox cboGender = new ComboBox();
        private PictureBox picAvatar = new PictureBox();

        private Button btnNew = new Button();
        private Button btnAdd = new Button();
        private Button btnUpdate = new Button();
        private Button btnDelete = new Button();
        private Button btnImage = new Button();
        private Button btnExport = new Button();

        private Label lblStatus = new Label();

        private string selectedImage = "";
        private bool isNewMode = true;
        private bool changingMode = false;

        public Form1()
        {
            InitializeComponent();

            Text = "BÀI 4.6 - QUẢN LÝ SINH VIÊN";
            StartPosition = FormStartPosition.CenterScreen;
            Size = new Size(1150, 720);
            MinimumSize = new Size(850, 600);

            Controls.Clear();

            CreateUI();
            SetupBinding();
            AddSampleData();
        }

        // ============================
        // 1. TẠO GIAO DIỆN
        // ============================

        private void CreateUI()
        {
            TableLayoutPanel layout = new TableLayoutPanel();

            layout.Dock = DockStyle.Fill;
            layout.ColumnCount = 2;
            layout.RowCount = 1;
            layout.BackColor = Color.WhiteSmoke;

            layout.ColumnStyles.Add(
                new ColumnStyle(SizeType.Percent, 35));

            layout.ColumnStyles.Add(
                new ColumnStyle(SizeType.Percent, 65));

            Controls.Add(layout);

            Panel panelLeft = new Panel();
            panelLeft.Dock = DockStyle.Fill;
            panelLeft.AutoScroll = true;
            panelLeft.BackColor = Color.White;

            layout.Controls.Add(panelLeft, 0, 0);

            Panel panelRight = new Panel();
            panelRight.Dock = DockStyle.Fill;
            panelRight.Padding = new Padding(10);

            layout.Controls.Add(panelRight, 1, 0);

            Label title = new Label();
            title.Text = "QUẢN LÝ SINH VIÊN";
            title.Font = new Font(
                "Arial", 16, FontStyle.Bold);
            title.ForeColor = Color.DarkBlue;
            title.AutoSize = true;
            title.Location = new Point(20, 20);

            panelLeft.Controls.Add(title);

            int y = 75;

            AddInput(panelLeft, "Mã sinh viên:", txtId, ref y);
            AddInput(panelLeft, "Họ và tên:", txtName, ref y);
            AddInput(panelLeft, "Tuổi:", txtAge, ref y);

            Label lblGender = new Label();
            lblGender.Text = "Giới tính:";
            lblGender.Location = new Point(20, y);
            lblGender.AutoSize = true;
            panelLeft.Controls.Add(lblGender);

            y += 25;

            cboGender.Items.AddRange(
                new object[] { "Nam", "Nữ" });

            cboGender.DropDownStyle =
                ComboBoxStyle.DropDownList;

            cboGender.Location = new Point(20, y);
            cboGender.Width = 300;
            panelLeft.Controls.Add(cboGender);

            y += 55;

            Label lblAvatar = new Label();
            lblAvatar.Text = "Ảnh đại diện:";
            lblAvatar.Location = new Point(20, y);
            lblAvatar.AutoSize = true;
            panelLeft.Controls.Add(lblAvatar);

            y += 25;

            picAvatar.Location = new Point(20, y);
            picAvatar.Size = new Size(180, 145);
            picAvatar.SizeMode = PictureBoxSizeMode.Zoom;
            picAvatar.BorderStyle = BorderStyle.FixedSingle;
            picAvatar.BackColor = Color.LightGray;

            panelLeft.Controls.Add(picAvatar);

            y += 155;

            btnImage.Text = "Chọn ảnh";
            AddButton(panelLeft, btnImage, 20, y);

            y += 50;

            btnNew.Text = "Nhập mới";
            btnAdd.Text = "Thêm";

            AddButton(panelLeft, btnNew, 20, y);
            AddButton(panelLeft, btnAdd, 180, y);

            y += 50;

            btnUpdate.Text = "Cập nhật";
            btnDelete.Text = "Xóa";

            AddButton(panelLeft, btnUpdate, 20, y);
            AddButton(panelLeft, btnDelete, 180, y);

            y += 50;

            btnExport.Text = "Xuất file CSV";
            btnExport.Width = 300;
            btnExport.Location = new Point(20, y);
            btnExport.Height = 38;
            btnExport.BackColor = Color.LightGreen;

            panelLeft.Controls.Add(btnExport);

            y += 55;

            lblStatus.Text = "Sẵn sàng";
            lblStatus.Location = new Point(20, y);
            lblStatus.AutoSize = true;
            lblStatus.ForeColor = Color.DarkGreen;
            panelLeft.Controls.Add(lblStatus);

            Label lblList = new Label();
            lblList.Text = "DANH SÁCH SINH VIÊN";
            lblList.Font = new Font(
                "Arial", 14, FontStyle.Bold);
            lblList.Dock = DockStyle.Top;
            lblList.Height = 45;
            lblList.TextAlign = ContentAlignment.MiddleCenter;
            lblList.ForeColor = Color.DarkBlue;

            panelRight.Controls.Add(lblList);

            dgv.Dock = DockStyle.Fill;
            dgv.AllowUserToAddRows = false;
            dgv.AllowUserToDeleteRows = false;
            dgv.ReadOnly = true;
            dgv.MultiSelect = false;

            dgv.SelectionMode =
                DataGridViewSelectionMode.FullRowSelect;

            dgv.AutoSizeColumnsMode =
                DataGridViewAutoSizeColumnsMode.Fill;

            dgv.RowHeadersVisible = false;
            dgv.BackgroundColor = Color.White;
            panelRight.Controls.Add(dgv);
            dgv.BringToFront();
            lblList.BringToFront();

            btnNew.Click += BtnNew_Click;
            btnAdd.Click += BtnAdd_Click;
            btnUpdate.Click += BtnUpdate_Click;
            btnDelete.Click += BtnDelete_Click;
            btnImage.Click += BtnImage_Click;
            btnExport.Click += BtnExport_Click;

            source.CurrentChanged += Source_CurrentChanged;

            // Kiểm tra dữ liệu trước khi ghi
            // vào Student qua Data Binding.
            txtId.Validating += TxtId_Validating;
            txtName.Validating += TxtName_Validating;
            txtAge.Validating += TxtAge_Validating;

            dgv.SelectionChanged += Dgv_SelectionChanged;

            FormClosed += Form1_FormClosed;
        }

        private void AddInput(
            Panel panel,
            string caption,
            TextBox textbox,
            ref int y)
        {
            Label label = new Label();
            label.Text = caption;
            label.Location = new Point(20, y);
            label.AutoSize = true;

            panel.Controls.Add(label);

            y += 25;

            textbox.Location = new Point(20, y);
            textbox.Width = 300;

            panel.Controls.Add(textbox);

            y += 55;
        }

        private void AddButton(
            Panel panel,
            Button button,
            int x,
            int y)
        {
            button.Location = new Point(x, y);
            button.Size = new Size(140, 38);
            button.BackColor = Color.LightSteelBlue;
            button.UseVisualStyleBackColor = false;

            panel.Controls.Add(button);
        }
        private void SetupBinding()
        {
            source.DataSource = students;
            dgv.DataSource = source;

            dgv.Columns["StudentId"].HeaderText = "Mã SV";
            dgv.Columns["FullName"].HeaderText = "Họ tên";
            dgv.Columns["Age"].HeaderText = "Tuổi";
            dgv.Columns["Gender"].HeaderText = "Giới tính";
            dgv.Columns["AvatarPath"].Visible = false;

            EnterNewMode();
        }
        private void ClearInputBindings()
        {
            txtId.DataBindings.Clear();
            txtName.DataBindings.Clear();
            txtAge.DataBindings.Clear();
            cboGender.DataBindings.Clear();
        }

        private void BindSelectedStudent()
        {
            changingMode = true;

            ClearInputBindings();

            txtId.DataBindings.Add(
                "Text",
                source,
                "StudentId",
                true,
                DataSourceUpdateMode.OnValidation);

            txtName.DataBindings.Add(
                "Text",
                source,
                "FullName",
                true,
                DataSourceUpdateMode.OnValidation);

            txtAge.DataBindings.Add(
                "Text",
                source,
                "Age",
                true,
                DataSourceUpdateMode.OnValidation);

            cboGender.DataBindings.Add(
                "Text",
                source,
                "Gender",
                true,
                DataSourceUpdateMode.OnValidation);

            isNewMode = false;

            btnAdd.Enabled = false;
            btnUpdate.Enabled = true;
            btnDelete.Enabled = true;

            changingMode = false;

            ShowAvatar();
        }

        private void EnterNewMode()
        {
            changingMode = true;
            isNewMode = true;

            ClearInputBindings();

            txtId.Clear();
            txtName.Clear();
            txtAge.Clear();

            cboGender.SelectedIndex = 0;

            selectedImage = "";
            SetAvatar("");

            btnAdd.Enabled = true;
            btnUpdate.Enabled = false;
            btnDelete.Enabled = false;

            lblStatus.Text = "Đang nhập sinh viên mới";

            changingMode = false;
            txtId.Focus();
        }

        private void Source_CurrentChanged(
            object sender, EventArgs e)
        {
            if (changingMode || isNewMode)
                return;

            selectedImage =
                source.Current is Student
                ? ((Student)source.Current).AvatarPath
                : "";

            ShowAvatar();
        }

        private void Dgv_SelectionChanged(
            object sender, EventArgs e)
        {
            if (changingMode || isNewMode)
                return;

            ShowAvatar();
        }
        private bool CheckInput(out int age)
        {
            age = 0;

            if (string.IsNullOrWhiteSpace(txtId.Text))
            {
                MessageBox.Show("Vui lòng nhập mã sinh viên!");
                txtId.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtName.Text))
            {
                MessageBox.Show("Vui lòng nhập họ tên!");
                txtName.Focus();
                return false;
            }

            if (!int.TryParse(txtAge.Text, out age)
                || age < 1 || age > 120)
            {
                MessageBox.Show(
                    "Tuổi phải là số nguyên từ 1 đến 120!");
                txtAge.Focus();
                return false;
            }

            if (cboGender.SelectedIndex == -1)
            {
                MessageBox.Show("Chọn giới tính!");
                return false;
            }

            return true;
        }

        private void TxtId_Validating(
            object sender, CancelEventArgs e)
        {
            if (isNewMode || changingMode) return;

            Student current = source.Current as Student;
            if (current == null) return;

            string id = txtId.Text.Trim();

            if (id == "" ||
                students.Any(x =>
                    x != current &&
                    x.StudentId.Equals(
                        id,
                        StringComparison.OrdinalIgnoreCase)))
            {
                MessageBox.Show(
                    "Mã sinh viên trống hoặc đã tồn tại!");
                e.Cancel = true;
            }
        }

        private void TxtName_Validating(
            object sender, CancelEventArgs e)
        {
            if (isNewMode || changingMode) return;

            if (string.IsNullOrWhiteSpace(txtName.Text))
            {
                MessageBox.Show("Họ tên không được để trống!");
                e.Cancel = true;
            }
        }

        private void TxtAge_Validating(
            object sender, CancelEventArgs e)
        {
            if (isNewMode || changingMode) return;

            int age;

            if (!int.TryParse(txtAge.Text, out age)
                || age < 1 || age > 120)
            {
                MessageBox.Show("Tuổi không hợp lệ!");
                e.Cancel = true;
            }
        }

        private void BtnNew_Click(
            object sender, EventArgs e)
        {
            EnterNewMode();
        }

        private void BtnAdd_Click(
            object sender, EventArgs e)
        {
            if (!isNewMode) return;

            int age;

            if (!CheckInput(out age))
                return;

            string id = txtId.Text.Trim();

            if (students.Any(x =>
                x.StudentId.Equals(
                    id,
                    StringComparison.OrdinalIgnoreCase)))
            {
                MessageBox.Show("Mã sinh viên đã tồn tại!");
                return;
            }

            Student student = new Student();

            student.StudentId = id;
            student.FullName = txtName.Text.Trim();
            student.Age = age;
            student.Gender = cboGender.Text;
            student.AvatarPath = selectedImage;

            students.Add(student);
            source.Position = students.IndexOf(student);
            BindSelectedStudent();

            lblStatus.Text = "Thêm thành công: " + student.FullName;

            MessageBox.Show("Thêm sinh viên thành công!");
        }

        private void BtnUpdate_Click(
            object sender, EventArgs e)
        {
            if (isNewMode) return;

            Student student = source.Current as Student;

            if (student == null)
            {
                MessageBox.Show("Chưa chọn sinh viên!");
                return;
            }

            int age;

            if (!CheckInput(out age))
                return;

            string id = txtId.Text.Trim();

            if (students.Any(x =>
                x != student &&
                x.StudentId.Equals(
                    id,
                    StringComparison.OrdinalIgnoreCase)))
            {
                MessageBox.Show("Mã sinh viên đã tồn tại!");
                return;
            }

            // Ghi các chỉnh sửa bằng Data Binding.
            if (!ValidateChildren())
                return;

            student.StudentId = id;
            student.FullName = txtName.Text.Trim();
            student.Age = age;
            student.Gender = cboGender.Text;
            student.AvatarPath = selectedImage;

            source.EndEdit();
            source.ResetCurrentItem();

            lblStatus.Text =
                "Đã cập nhật: " + student.FullName;

            MessageBox.Show("Cập nhật thành công!");
        }

        private void BtnDelete_Click(
            object sender, EventArgs e)
        {
            if (isNewMode) return;

            Student student = source.Current as Student;

            if (student == null)
            {
                MessageBox.Show("Chưa chọn sinh viên!");
                return;
            }

            DialogResult result = MessageBox.Show(
                "Bạn có chắc chắn muốn xóa " +
                student.FullName + "?",
                "Xác nhận xóa",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                changingMode = true;

                string name = student.FullName;
                source.RemoveCurrent();

                changingMode = false;

                if (students.Count == 0)
                {
                    EnterNewMode();
                }
                else
                {
                    BindSelectedStudent();
                }

                lblStatus.Text = "Đã xóa sinh viên: " + name;

                MessageBox.Show("Xóa thành công!");
            }
        }

        private void BtnImage_Click(
            object sender, EventArgs e)
        {
            using (OpenFileDialog dialog = new OpenFileDialog())
            {
                dialog.Title = "Chọn ảnh đại diện";

                dialog.Filter =
                    "Ảnh JPG|*.jpg;*.jpeg|" +
                    "Ảnh PNG|*.png|" +
                    "Ảnh BMP|*.bmp|" +
                    "Tất cả ảnh|*.jpg;*.jpeg;*.png;*.bmp";

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    selectedImage = dialog.FileName;

                    SetAvatar(selectedImage);
                    if (!isNewMode)
                    {
                        Student student =
                            source.Current as Student;

                        if (student != null)
                        {
                            student.AvatarPath = selectedImage;
                        }
                    }

                    lblStatus.Text = "Đã chọn ảnh đại diện";
                }
            }
        }

        private void ShowAvatar()
        {
            if (isNewMode) return;

            Student student = source.Current as Student;

            if (student == null)
            {
                selectedImage = "";
                SetAvatar("");
                return;
            }

            selectedImage = student.AvatarPath;
            SetAvatar(selectedImage);
        }

        private void SetAvatar(string path)
        {
            Image oldImage = picAvatar.Image;
            picAvatar.Image = null;

            if (oldImage != null)
                oldImage.Dispose();

            if (!File.Exists(path))
                return;

            try
            {
                using (FileStream stream = new FileStream(
                    path,
                    FileMode.Open,
                    FileAccess.Read,
                    FileShare.ReadWrite))
                {
                    using (Image image =
                        Image.FromStream(stream))
                    {
                        picAvatar.Image = new Bitmap(image);
                    }
                }
            }
            catch (Exception)
            {
                picAvatar.Image = null;
                lblStatus.Text = "Không thể đọc ảnh!";
            }
        }
        private string CsvValue(string value)
        {
            if (value == null)
                value = "";

            return "\"" +
                value.Replace("\"", "\"\"") +
                "\"";
        }

        private void BtnExport_Click(
            object sender, EventArgs e)
        {
            if (students.Count == 0)
            {
                MessageBox.Show(
                    "Danh sách sinh viên đang trống!");
                return;
            }

            using (SaveFileDialog dialog = new SaveFileDialog())
            {
                dialog.Title = "Lưu danh sách sinh viên";
                dialog.Filter = "CSV Files (*.csv)|*.csv";
                dialog.FileName = "DanhSachSinhVien.csv";

                if (dialog.ShowDialog() != DialogResult.OK)
                    return;

                try
                {
                    using (StreamWriter writer =
                        new StreamWriter(
                            dialog.FileName,
                            false,
                            new UTF8Encoding(true)))
                    {
                        writer.WriteLine(
                            "MaSV,HoTen,Tuoi,GioiTinh,AvatarPath");

                        foreach (Student s in students)
                        {
                            string line =
                                CsvValue(s.StudentId) + "," +
                                CsvValue(s.FullName) + "," +
                                s.Age.ToString() + "," +
                                CsvValue(s.Gender) + "," +
                                CsvValue(s.AvatarPath);

                            writer.WriteLine(line);
                        }
                    }

                    lblStatus.Text = "Xuất CSV thành công";

                    MessageBox.Show(
                        "Đã xuất danh sách sinh viên ra CSV!",
                        "Thành công",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(
                        "Lỗi xuất file: " + ex.Message);
                }
            }
        }

        private void AddSampleData()
        {
            students.Add(new Student
            {
                StudentId = "SV001",
                FullName = "Nguyễn Văn An",
                Age = 20,
                Gender = "Nam",
                AvatarPath = ""
            });

            students.Add(new Student
            {
                StudentId = "SV002",
                FullName = "Trần Thị Bình",
                Age = 21,
                Gender = "Nữ",
                AvatarPath = ""
            });

            students.Add(new Student
            {
                StudentId = "SV003",
                FullName = "Lê Văn Cường",
                Age = 19,
                Gender = "Nam",
                AvatarPath = ""
            });

            source.Position = 0;
            BindSelectedStudent();

            lblStatus.Text = "Đã nạp dữ liệu mẫu";
        }

        private void Form1_FormClosed(
            object sender, FormClosedEventArgs e)
        {
            Image image = picAvatar.Image;
            picAvatar.Image = null;

            if (image != null)
                image.Dispose();
        }
    }
}
