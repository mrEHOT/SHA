using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SHA
{
    public partial class mainForm : Form
    {
        public mainForm()
        {
            InitializeComponent();
        }

        private void startButton_Click(object sender, EventArgs e)
        {
            if (messageRadioButton.Checked)
            {
                byte[] data = Encoding.UTF8.GetBytes(inputTextBox.Text);

                MessageWorker.CalculateHash(data, true, data.Length);

                outputTextBox.Text = SHA.GetResultHash();

                MessageBox.Show("Вычисление хэша выполнено!", "Операция завершена", MessageBoxButtons.OK, MessageBoxIcon.Information);

                return;

            } // Выбран режим работы с текстом

            if (fileRadioButton.Checked)
            {
                if (filePathTextBox.Text.Length != 0)
                {
                    bool lastBlock = false; // Фиксируем является ли блок последним

                    var fileParameters = FileWorker.GetFileParameters(); // Получаем параметры файла, с которым работаем

                    FileInfo fileInfo = new FileInfo(fileParameters.Item1);
                    long fileLength = fileInfo.Length;

                    var infile = File.OpenRead(fileParameters.Item1); // Открываем файл для чтения

                    while (true)
                    {
                        var content = FileWorker.ReadFile(infile); // Считываем блок данных из файла (100 КБ)

                        // Фиксируем, что это последние 100 КБ или меньше
                        if (!content.Item1)
                            lastBlock = true; // Фиксируем, что блок является последним => нужен падинг

                        MessageWorker.CalculateHash(content.Item2, lastBlock, fileLength);

                        if (!content.Item1)
                        {
                            infile.Close(); // Закрываем файл, т.к. он прочитан
                            break; // В случае, когда весь файл прочитан
                        }
                    }

                    outputTextBox.Text = SHA.GetResultHash();

                    MessageBox.Show("Вычисление хэша выполнено!", "Операция завершена", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                } // Если пользователь выбрал файл, то выполняем хэширование
                else
                {
                    MessageBox.Show("Выберите файл для вычисления хэша!", "Файл не выбран", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                } // Если пользователь не выбрал файл

            } // Выбран режим работы с файлом

        } // Вычисление хэша

        private void messageRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (messageRadioButton.Checked)
            {
                // Включаем элементы управления, которые применяются в этом режиме
                inputTextBox.Enabled = true;
                outputTextBox.Enabled = true;
                startButton.Enabled = true;

                // Отключаем элементы управления, которые не применяются в этом режиме
                filePathTextBox.Enabled = false;
                selectFileButton.Enabled = false;
                clearFilePathButton.Enabled = false;
            }
        } // Программа переходит в режим работы с сообщениями

        private void fileRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (fileRadioButton.Checked)
            {
                // Включаем элементы управления, которые применяются в этом режиме
                filePathTextBox.Enabled = true;
                selectFileButton.Enabled = true;
                clearFilePathButton.Enabled = true;
                outputTextBox.Enabled = true;
                startButton.Enabled = true;

                // Отключаем элементы управления, которые не применяются в этом режиме
                inputTextBox.Enabled = false;

            }
        } // Программа переходит в режим работы с файлами

        private void selectFileButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();

            openFileDialog.Filter = "All files(*.*)|*.*";

            if (openFileDialog.ShowDialog() == DialogResult.Cancel)
                return; // Если была нажата отмена, то ничего не происходит

            // Получаем путь до выбранного файла и его короткое имя
            FileWorker.SetFileParameters(openFileDialog.FileName, openFileDialog.SafeFileName);
            filePathTextBox.Text = FileWorker.GetFileParameters().Item1;

        } // Выбор файла для вычисления хэша

        private void clearFilePathButton_Click(object sender, EventArgs e)
        {
            filePathTextBox.Clear();
            FileWorker.SetFileParameters("", "");
        } // Отмена выбора файла
    }
}
