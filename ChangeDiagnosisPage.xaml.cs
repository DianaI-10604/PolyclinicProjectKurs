using PolyclinicProjectKurs.Context;
using PolyclinicProjectKurs.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace PolyclinicProjectKurs
{
    /// <summary>
    /// Логика взаимодействия для ChangeDiagnosisPage.xaml
    /// </summary>
    public partial class ChangeDiagnosisPage : Window
    {
        private readonly Medicalrecord _medicalRecord;
        public Medicalrecord UpdatedMedicalRecord { get; private set; }


        public ChangeDiagnosisPage()
        {
            InitializeComponent();
        }

        public ChangeDiagnosisPage(Medicalrecord medicalRecord)
        {
            InitializeComponent();
            _medicalRecord = medicalRecord;
            DiagnosisTextBox.Text = _medicalRecord.Diagnosis;
        }

        private void ConfirmButton_Click(object sender, RoutedEventArgs e)
        {
            // Получить новый диагноз из TextBox
            string newDiagnosis = DiagnosisTextBox.Text;

            if (!string.IsNullOrWhiteSpace(newDiagnosis))
            {
                // Обновить диагноз в базе данных
                using (var context = new PolycCursContext())
                {
                    var record = context.Medicalrecords.FirstOrDefault(m => m.Id == _medicalRecord.Id);
                    if (record != null)
                    {
                        record.Diagnosis = newDiagnosis;
                        context.SaveChanges();
                        MessageBox.Show("Диагноз успешно обновлен.");
                        UpdatedMedicalRecord = record; // Обновление свойства
                    }
                    else
                    {
                        MessageBox.Show("Не удалось найти запись в базе данных.");
                    }
                }

                // Закрыть окно с результатом
                DialogResult = true;
                this.Close();
            }
            else
            {
                MessageBox.Show("Введите диагноз перед подтверждением.");
            }
        }
    }
}
