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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.EntityFrameworkCore;
using PolyclinicProjectKurs.Models;
using PolyclinicProjectKurs.Context;
using System.Collections.ObjectModel;

namespace PolyclinicProjectKurs
{
    /// <summary>
    /// Логика взаимодействия для MedicalRecords.xaml
    /// </summary>
    public partial class MedicalRecords : UserControl
    {
        public ObservableCollection<Medicalrecord> MedicalRecordsCollection { get; set; }
        private User _user;

        public MedicalRecords()
        {
            InitializeComponent();
        }

        public MedicalRecords(User user)
        {
            InitializeComponent();
            _user = user;

            using (var context = new PolycCursContext())
            {
                var medicalRecordsList = context.Medicalrecords
                    .Include(m => m.Appointment)
                    .Include(m => m.Doctor)
                    .Where(m => m.UserId == _user.UserId) // Фильтр по Userid текущего пользователя
                    .ToList();

                MedicalRecordsCollection = new ObservableCollection<Medicalrecord>(medicalRecordsList);
                //выводим из базы данных записи текущего пользователя
            }

            DataContext = this;

        }
    }
}
