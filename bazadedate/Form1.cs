using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Runtime.CompilerServices;
using System.Net.Configuration;

namespace bazadedate
{
    public partial class Form1 : Form
    {
        private static readonly string ConnectionString = @"Data Source=192.168.5.10;Initial Catalog=Admitere;User ID=alex;Password=12345678;Encrypt=True;TrustServerCertificate=True;";

        private ComboBox _taskCombobox;
        private ComboBox _secondaryTaskCombobox;
        private TextBox _secondaryTaskTextbox;

        public Form1()
        {
            InitializeComponent();

            _taskCombobox = new ComboBox();
            _taskCombobox.Location = new Point(25, 500);
            _taskCombobox.Size = new Size(150, 33);
            _taskCombobox.DropDownStyle = ComboBoxStyle.DropDownList;
            var items = Enumerable.Range(1, 20).Select(i => i.ToString()).ToArray();
            _taskCombobox.Items.AddRange(items);
            _taskCombobox.SelectedIndexChanged += TaskCombobox_SelectedIndexChanged;
            Controls.Add(_taskCombobox);

            _secondaryTaskCombobox = new ComboBox();
            _secondaryTaskCombobox.Location = new Point(200, 500);
            _secondaryTaskCombobox.Size = new Size(150, 33);
            _secondaryTaskCombobox.DropDownStyle = ComboBoxStyle.DropDownList;
            Controls.Add(_secondaryTaskCombobox);
            _secondaryTaskCombobox.Hide();

            _secondaryTaskTextbox = new TextBox();
            _secondaryTaskTextbox.Location = new Point(200, 500);
            _secondaryTaskTextbox.Size = new Size(150, 33);
            Controls.Add(_secondaryTaskTextbox);
            _secondaryTaskTextbox.Hide();

            var runButton = new Button();
            runButton.Location = new Point(25, 530);
            runButton.Size = new Size(100, 35);
            runButton.Text = "Executa";
            runButton.Click += RunButton_Click;
            Controls.Add(runButton);

            PreTask();
        }

        private void RunButton_Click(object sender, EventArgs e)
        {
            var task = _taskCombobox.SelectedIndex + 1;
            var secondaryTask = _secondaryTaskCombobox.SelectedIndex;
            var text = _secondaryTaskTextbox.Text;

            switch (task)
            {
                case 1:
                    if (secondaryTask == 0)
                    {
                        LoadData(@"SELECT nume, prenume, rezultat, media FROM Admitere WHERE sex = 'F' ORDER BY media DESC");
                    }
                    else
                    {
                        LoadData(@"SELECT nume, prenume, rezultat, media FROM Admitere WHERE sex = 'M' ORDER BY media DESC");
                    }
                    break;
                case 2:
                    if (secondaryTask == 0)
                    {
                        LoadData(@"SELECT TOP 5 nume, prenume, media, datan, oras FROM Admitere WHERE rezultat = 'admis' ORDER BY media DESC");
                    }
                    else
                    {
                        LoadData(@"SELECT TOP 5 nume, prenume, media, datan, oras FROM Admitere WHERE rezultat = 'admis' ORDER BY media");
                    }
                    break;
                case 3:
                    LoadData(@"SELECT nume, prenume, oras, datan, media FROM Admitere WHERE rezultat = 'admis' AND DATEADD(year, 18, datan) >= GETDATE() AND DATEADD(year, 20, datan) <= GETDATE() ORDER BY datan, nume");
                    break;
                case 4:
                    if (secondaryTask == 0)
                    {
                        LoadData(@"SELECT nume, prenume, proba1, rezultat FROM Admitere ORDER BY proba1");
                    }
                    else
                    {
                        LoadData(@"SELECT nume, prenume, proba2, rezultat FROM Admitere ORDER BY proba2");
                    }
                    break;
                case 5:
                    LoadData(@"SELECT nume, prenume, media, rezultat FROM Admitere ORDER BY nume, prenume");
                    break;
                case 6:
                    {
                        var num1 = ExecuteScalar(@"SELECT COUNT(*) FROM Admitere WHERE media >= 1 AND media <= 5.00");
                        var num2 = ExecuteScalar(@"SELECT COUNT(*) FROM Admitere WHERE media >= 5.01 AND media <= 7.00");
                        var num3 = ExecuteScalar(@"SELECT COUNT(*) FROM Admitere WHERE media >= 7.01 AND media <= 9.00");
                        var num4 = ExecuteScalar(@"SELECT COUNT(*) FROM Admitere WHERE media >= 9.01 AND media <= 10");
                        var total = ExecuteScalar(@"SELECT COUNT(*) FROM Admitere");

                        var percent1 = Math.Round(num1 / total * 100, 2);
                        var percent2 = Math.Round(num2 / total * 100, 2);
                        var percent3 = Math.Round(num3 / total * 100, 2);
                        var percent4 = Math.Round(num4 / total * 100, 2);

                        MessageBox.Show($@"1 - 5.00: {percent1}%
5.01 - 7.00: {percent2}%
7.01 - 9.00: {percent3}%
9.01 - 10.00: {percent4}%
");
                        break;
                    }
                case 7:
                    LoadData(@"SELECT nume, prenume, media, rezultat, oras FROM Admitere ORDER BY oras, nume, prenume");
                    break;
                case 8:
                    LoadData($@"SELECT ROW_NUMBER() OVER (ORDER BY nume, prenume) AS NR, PRENUME, MEDIA, REZULTAT FROM Admitere WHERE oras = '{text}' ORDER BY nume, prenume");
                    break;
                case 9:
                    LoadData(@"SELECT a.NUME, a.PRENUME, a.MEDIA, a.ORAS FROM Admitere a WHERE media = (SELECT MAX(b.media) FROM Admitere b WHERE b.oras = a.oras) ORDER BY oras");
                    break;
                case 10:
                    if (_secondaryTaskCombobox.SelectedIndex == 0)
                    {
                        LoadData(@"SELECT ID, NUME, PRENUME, SEX, PROBA1, PROBA2, MEDIA, DATAN, REZULTAT FROM Admitere WHERE oras = 'Cluj' AND rezultat = 'admis' ORDER BY media, nume, prenume");
                    }
                    else
                    {
                        LoadData(@"SELECT ID, NUME, PRENUME, SEX, PROBA1, PROBA2, MEDIA, DATAN, REZULTAT FROM Admitere WHERE oras = 'Cluj' AND rezultat = 'respins' ORDER BY media, nume, prenume");
                    }
                    break;
                case 11:
                    LoadData(@"SELECT TOP 4 NUME, PRENUME, ORAS, MEDIA FROM Admitere WHERE oras <> 'Brasov' AND rezultat = 'admis' ORDER BY media DESC, proba1 DESC");
                    break;
                case 12:
                    if (_secondaryTaskCombobox.SelectedIndex == 0)
                    {
                        LoadData(@"SELECT TOP 2 NUME, PRENUME, ORAS, MEDIA FROM Admitere WHERE oras <> 'Brasov' AND rezultat = 'admis' AND sex = 'm' ORDER BY media DESC, proba1 DESC");
                    }
                    else
                    {
                        LoadData(@"SELECT TOP 3 NUME, PRENUME, ORAS, MEDIA FROM Admitere WHERE oras <> 'Brasov' AND rezultat = 'admis' AND sex = 'f' ORDER BY media DESC, proba1 DESC");
                    }
                    break;
                case 13:
                    if (_secondaryTaskCombobox.SelectedIndex == 0)
                    {
                        LoadData(@"SELECT NUME, PRENUME, MEDIA FROM Admitere WHERE media >= 9.75 AND media <= 10 ORDER BY nume, prenume");
                    }
                    else
                    {
                        LoadData(@"SELECT NUME, PRENUME, MEDIA FROM Admitere WHERE media >= 8.50 AND media <= 9.74 ORDER BY nume, prenume");
                    }
                    break;
                case 14:
                    if (_secondaryTaskCombobox.SelectedIndex == 0)
                    {
                        LoadData(@"SELECT NUME, PRENUME, DATAN, ORAS FROM Admitere WHERE rezultat = 'respins' AND DATEADD(year, 20, datan) <= GETDATE() AND sex = 'm'");
                    }
                    else
                    {
                        LoadData(@"SELECT NUME, PRENUME, DATAN, ORAS FROM Admitere WHERE rezultat = 'admis' OR DATEADD(year, 20, datan) > GETDATE() OR sex = 'f'");
                    }
                    break;
                case 15:
                    {
                        var admisi = ExecuteScalar($@"SELECT COUNT(*) FROM Admitere WHERE rezultat = 'admis' AND oras = '{text}'");
                        var total = ExecuteScalar($@"SELECT COUNT(*) FROM Admitere WHERE oras = '{text}'");

                        var percentage = Math.Round(admisi / total * 100, 0);
                        MessageBox.Show($@"Procent admisi: {percentage}%
Total candidati: {total}");
                        break;
                    }
                case 16:
                    {
                        var num1 = Math.Round(ExecuteScalar(@"SELECT AVG(proba1) FROM Admitere WHERE rezultat = 'admis'"), 2);
                        var num2 = Math.Round(ExecuteScalar(@"SELECT AVG(proba2) FROM Admitere WHERE rezultat = 'admis'"), 2);
                        var num3 = Math.Round(ExecuteScalar(@"SELECT AVG(media) FROM Admitere WHERE rezultat = 'admis'"), 2);

                        MessageBox.Show($@"Medie proba 1: {num1}
Medie proba 2: {num2}
Media mediilor: {num3}");
                        break;
                    }
                case 17:
                    LoadData(@"SELECT NUME, PRENUME, MEDIA, ORAS from Admitere WHERE media >= 5 AND rezultat = 'respins'");
                    break;
                case 18:
                    {
                        double num;
                        if (_secondaryTaskCombobox.SelectedIndex == 0)
                        {
                            num = Math.Round(ExecuteScalar(@"SELECT AVG(media) FROM Admitere WHERE rezultat = 'admis'"), 2);
                        }
                        else
                        {
                            num = Math.Round(ExecuteScalar(@"SELECT AVG(media) FROM Admitere WHERE rezultat = 'respins'"), 2);
                        }

                        MessageBox.Show($@"Media: {num}");
                        break;
                    }
                default:
                    break;
            }
        }

        private void ShowSecondaryCombobox(object[] items)
        {
            _secondaryTaskCombobox.Items.Clear();
            _secondaryTaskCombobox.Items.AddRange(items);
            _secondaryTaskCombobox.Show();
        }

        private void TaskCombobox_SelectedIndexChanged(object sender, EventArgs e)
        {
            var task = _taskCombobox.SelectedIndex + 1;

            switch (task)
            {
                case 1:
                    ShowSecondaryCombobox(new object[] { "Fete", "Baieti" });
                    break;
                case 2:
                    ShowSecondaryCombobox(new object[] { "Cei mai buni", "Cei mai slabi" });
                    break;
                case 4:
                    ShowSecondaryCombobox(new object[] { "Proba 1", "Proba 2" });
                    break;
                case 8:
                    _secondaryTaskTextbox.Show();
                    break;
                case 10:
                    ShowSecondaryCombobox(new object[] { "Admisi", "Respinsi" });
                    break;
                case 12:
                    ShowSecondaryCombobox(new object[] { "Baieti", "Fete" });
                    break;
                case 13:
                    ShowSecondaryCombobox(new object[] { "De merit", "De studii" });
                    break;
                case 14:
                    ShowSecondaryCombobox(new object[] { "Incorporabili", "Neincorporabili" });
                    break;
                case 15:
                    _secondaryTaskTextbox.Show();
                    break;
                case 18:
                    ShowSecondaryCombobox(new object[] { "Admisi", "Respinsi" });
                    break;
                default:
                    _secondaryTaskCombobox.Hide();
                    _secondaryTaskTextbox.Hide();
                    break;
            }
        }

        private void LoadData(string sqlQuery)
        {
            using (var connection = new SqlConnection(ConnectionString))
            using (var command = new SqlCommand(sqlQuery, connection))
            using (var adapter = new SqlDataAdapter(command))
            {
                var table = new DataTable();
                adapter.Fill(table);

                dataGridView1.DataSource = null;
                dataGridView1.DataSource = table;
            }
        }

        private void ExecuteNonQuery(string sqlQuery)
        {
            using (var connection = new SqlConnection(ConnectionString))
            using (var command = new SqlCommand(sqlQuery, connection))
            {
                connection.Open();
                command.ExecuteNonQuery();
            }
        }

        private double ExecuteScalar(string sqlQuery)
        {
            using (var connection = new SqlConnection(ConnectionString))
            using (var command = new SqlCommand(sqlQuery, connection))
            {
                connection.Open();
                var result = command.ExecuteScalar();
                try
                {
                    return Convert.ToDouble(result);
                }
                catch
                {
                    return -1;
                }
            }
        }

        private void PreTask()
        {
            ExecuteNonQuery(@"UPDATE Admitere SET media = (proba1 + proba2) / 2, rezultat = 'respins'");
            ExecuteNonQuery(@"UPDATE Admitere SET rezultat = 'admis' WHERE id IN (SELECT TOP 20 id FROM Admitere WHERE proba1 >= 5 AND proba2 >= 5 ORDER BY media DESC)");
            LoadData(@"SELECT * FROM Admitere ORDER BY media DESC");
        }
    }
}
