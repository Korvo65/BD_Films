using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Threading;

namespace DataBase
{
    public partial class Form1 : Form
    {
        DataWork data = new DataWork();
        string oldValue = "";
        string filename = "";
        public Form1()
        {
            InitializeComponent();
            dataGridViewTable.Rows[0].ReadOnly = true;
            dataGridViewTable.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            openFileDialog1.Filter = "Text files(*.txt)|*.txt|All files(*.*)|*.*";
            saveFileDialog1.Filter = "Text files(*.txt)|*.txt|All files(*.*)|*.*";
            panelSearchRecord.Visible = false;
            labelSave.Visible = false;
            pictureBoxAdd.Size = new Size(45, 38);
            comboBoxChoiceSort.Text = comboBoxChoiceSort.Items[0].ToString();
            InitializeTimers();
            ToolTip t = new ToolTip();
            t.SetToolTip(pictureBoxOpen, "Открыть файл (Ctrl+O)");
            t.SetToolTip(pictureBoxSave, "Сохранить файл (Ctrl+S)");
            t.SetToolTip(pictureBoxSearch, "Поиск данных");
            t.SetToolTip(pictureBoxAdd, "Добавить новые данные");
            t.SetToolTip(pictureBoxFAQ, "Информация о приложении");
        }

        private void InitializeTimers()
        {
            // Таймер для автосохранения данных в файл раз в 1 минуту
            timer1.Interval = 60000;
            timer1.Tick += new EventHandler(timer1_Tick);

            // Таймер для отображения надписи, свидетельствующей о сохранении файла
            // длительностью 3 сек
            timer2.Interval = 3000;
            timer2.Tick += new EventHandler(timer2_Tick);
        }

        // Генерация не повторяющихся id
        public ushort generateID()
        {
            Random r = new Random();
            int id = r.Next(0, 1000);
            for (int i = 0; i < data.FilmFiles.Count; i++)
            {
                FilmFile music = (FilmFile)data.FilmFiles[i];
                if (music.MovieID == id)
                {
                    i = 0;
                    id = r.Next(0, 1000);
                }
            }
            return (ushort)id;
        }

        private void btnAddRecord_Click(object sender, EventArgs e)
        {
            try
            {
                string artist = textBoxArtist.Text;
                string song = textBoxSong.Text;
                ushort year = (ushort)Convert.ToInt32(textBoxYear.Text);
                string genre = textBoxGenre.Text;
                textBoxArtist.Text = "";
                textBoxSong.Text = "";
                textBoxYear.Text = "";
                textBoxGenre.Text = "";
                FilmFile musicFile = new FilmFile(generateID(), artist, song, year, genre);
                data.AddMusicFile(musicFile);
                int n = data.FilmFiles.Count;
                FilmFile music = (FilmFile)data.FilmFiles[n - 1];
                dataGridViewTable.Rows.Add(music.MovieID, artist, song, year, genre);
                BanChangeColumn(n - 1);
                if ((!timer1.Enabled) && (filename != ""))
                {
                    timer1.Enabled = true;
                    timer1.Start();
                }
            } catch (Exception exception)
            {
                MessageBox.Show($"Ошибка: {exception.Message}");
            }
        }

        // Запретить редактировать столбец по указанному индексу
        private void BanChangeColumn(int index) =>
            dataGridViewTable.Rows[index].Cells[0].ReadOnly = true;

        private void btnClearAll_Click(object sender, EventArgs e)
        {
            if (data.FilmFiles.Count != 0)
            {
                DialogResult dialogResult = MessageBox.Show("Уверены," +
                    "что хотите удалить все элементы?", "Подтверждение", MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning);
                if (dialogResult == DialogResult.Yes)
                {
                    data.DeleteMusic();
                    dataGridViewTable.Rows.Clear();
                }
            }
        }

        private void btnClearRow_Click(object sender, EventArgs e)
        {
            int count = dataGridViewTable.Rows.Count;
            foreach (DataGridViewRow row in dataGridViewTable.SelectedRows)
            {
                int index = row.Index; // индекс выбранной строки
                if (index == count - 1) return;
                data.DeleteFilmFile(index);
                dataGridViewTable.Rows.RemoveAt(index);
            }
        }

        private void dataGridViewTable_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            int indRow = dataGridViewTable.Rows[e.RowIndex].Index;
            int indColumn = dataGridViewTable.Columns[e.ColumnIndex].Index;
            object value = dataGridViewTable.Rows[indRow].Cells[indColumn].Value;
            // Если значение не было введено, то оставляем старое
            if (value is null)
            {
                MessageBox.Show("Вы не ввели значение.");
                dataGridViewTable.Rows[indRow].Cells[indColumn].Value = oldValue;
                return;
            }
            if (indColumn == 1)
            {
                data.ChangeArtistName((string)value, indRow);
            } 
            else
            if (indColumn == 2)
            {
                data.ChangeSongTittle((string)value, indRow);
            }
            else
            if (indColumn == 3)
            {
                try
                {
                    data.ChangeYearRelease((ushort)Convert.ToInt32(value), indRow);
                } catch (Exception exception)
                {
                    MessageBox.Show($"Ошибка: {exception.Message}");
                    dataGridViewTable.Rows[indRow].Cells[indColumn].Value = oldValue;
                }
            }
            else
            if (indColumn == 4)
            {
                data.ChangeGenreSong((string)value, indRow);
            }
        }
        
        private void dataGridViewTable_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            if (dataGridViewTable.Rows[e.RowIndex].Cells[e.ColumnIndex].Value != null)
                oldValue = ((DataGridView)sender).Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString();
        }

        // Сохранение данных в файл
        private void сохранитьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (filename == "")
            {
                if (saveFileDialog1.ShowDialog() == DialogResult.Cancel)
                    return;
                filename = saveFileDialog1.FileName;
                this.Text = filename + " - База данных музыки";
            }
            // Если сохранение произошло первый раз, то запускаем 
            // таймер для автосохранения
            if (!timer1.Enabled)
            {
                timer1.Enabled = true;
                timer1.Start();
            }
            // При каждом сохранении будет появляться надпись "Сохранение..."
            labelSave.Visible = true;
            timer2.Enabled = true;
            timer2.Start();

            data.SaveToFile(filename);
        }

        // Восстановление данных из файла в таблицу и в список
        private void открытьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.Cancel)
                return;
            filename = openFileDialog1.FileName;
            this.Text = filename + " - База данных музыки";
            dataGridViewTable.Rows.Clear();
            data.OpenFile(filename);
            WriteToDataGrid();
        }

        private void создатьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if ((data.FilmFiles.Count != 0) || (filename != ""))
            {
                DialogResult dialogResult = MessageBox.Show("Уверены," +
                    "что хотите создать новый файл?" + "\r\n" +
                    "Изменения в текущем не сохранятся!", "Подтверждение", MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning);
                if (dialogResult == DialogResult.Yes)
                {
                    this.Text = "База данных музыки";
                    filename = "";
                    data.DeleteMusic();
                    dataGridViewTable.Rows.Clear();
                    timer1.Stop();
                    timer1.Enabled = false;
                    timer2.Stop();
                    timer2.Enabled = false;
                }
            }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            if ((data.FilmFiles.Count == 0) || (textBoxForSearch.Text == ""))
                return;
            dataGridViewTable.ClearSelection();
            List<int> foundElements = data.SearchMusicFile(textBoxForSearch.Text);
            if (foundElements[0] == -1)
            {
                MessageBox.Show("Ничего не удалось найти!");
                return;
            }
            dataGridViewTable.CurrentCell = dataGridViewTable[0, foundElements[0]];
            for (int i = 0; i < foundElements.Count; i++)
            {
                dataGridViewTable.Rows[foundElements[i]].Selected = true;
            }
        }

        private void pictureBoxOpen_Click(object sender, EventArgs e)
        {
            открытьToolStripMenuItem_Click(открытьToolStripMenuItem, null);
        }

        private void pictureBoxSave_Click(object sender, EventArgs e)
        {
            сохранитьToolStripMenuItem_Click(сохранитьToolStripMenuItem, null);
        }

        private void pictureBoxSearch_Click(object sender, EventArgs e)
        {
            labelArtist.Visible = false;
            labelSong.Visible = false;
            labelGenre.Visible = false;
            labelYear.Visible = false;
            textBoxArtist.Visible = false;
            textBoxSong.Visible = false;
            textBoxGenre.Visible = false;
            textBoxYear.Visible = false;
            btnAddRecord.Visible = false;
            panelSearchRecord.Visible = true;
            pictureBoxSearch.Size = new Size(45, 38);
            pictureBoxAdd.Size = new Size(35, 28);
        }

        private void pictureBoxAdd_Click(object sender, EventArgs e)
        {
            labelArtist.Visible = true;
            labelSong.Visible = true;
            labelGenre.Visible = true;
            labelYear.Visible = true;
            textBoxArtist.Visible = true;
            textBoxSong.Visible = true;
            textBoxGenre.Visible = true;
            textBoxYear.Visible = true;
            btnAddRecord.Visible = true;
            panelSearchRecord.Visible = false;
            pictureBoxSearch.Size = new Size(35, 28);
            pictureBoxAdd.Size = new Size(45, 38);
        }

        private void pictureBoxFAQ_Click(object sender, EventArgs e)
        {
            string Info = "База данных фильмов, версия 1.4" + "\n\n" +
                "Разработка/дизайн: Забелин Владислав" + "\n\n" +
                "Github: https://github.com/Korvo65" + "\n\n" +
                "2020 г.";
            MessageBox.Show(Info, "Справка", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            
        }

        private void WriteToDataGrid()
        {
            for (int i = 0; i < data.FilmFiles.Count; i++)
            {
                FilmFile music = (FilmFile)data.FilmFiles[i];
                dataGridViewTable.Rows.Add(music.MovieID, music.ProducertName,
                    music.MovieTitle, music.YearRelease, music.KinoGenre);
                BanChangeColumn(i);
            }
            // последнюю строку запрещаем редактировать
            dataGridViewTable.Rows[data.FilmFiles.Count].ReadOnly = true;
        }

        private void btnSort_Click(object sender, EventArgs e)
        {
            if (data.FilmFiles.Count != 0)
            {
                dataGridViewTable.Rows.Clear();
                if (comboBoxChoiceSort.SelectedIndex == 0)
                {
                    data.Sort(SortDirection.Ascending); 
                }
                else
                {
                    data.Sort(SortDirection.Descending);
                }
                WriteToDataGrid();
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            data.SaveToFile(filename);
            labelAutoSave.Visible = true;
            timer2.Enabled = true;
            timer2.Start();
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            if (labelAutoSave.Visible)
            {
                labelAutoSave.Visible = false;
            } else
            {
                labelSave.Visible = false;
            }
            timer2.Enabled = false;
            timer2.Stop();
        }

        private void оПрограммеToolStripMenuItem_Click(object sender, EventArgs e)
        {
            pictureBoxFAQ_Click(pictureBoxFAQ, null);
        }

        private void labelArtist_Click(object sender, EventArgs e)
        {

        }

        private void dataGridViewTable_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void textBoxArtist_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBoxYear_TextChanged(object sender, EventArgs e)
        {

        }

        private void labelGenre_Click(object sender, EventArgs e)
        {

        }

        private void textBoxGenre_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBoxSong_TextChanged(object sender, EventArgs e)
        {

        }
    }
}