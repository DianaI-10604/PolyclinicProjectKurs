﻿using System;
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
using PolyclinicProjectKurs.Models;
using PolyclinicProjectKurs.Context;

namespace PolyclinicProjectKurs
{
    public partial class RegistrationPage : Window
    {
        private string repeatpassword;
        string gender = string.Empty;
        public RegistrationPage()
        {
            InitializeComponent();
        }

        //Регистрируемся
        private void ApplyRegistration_ButtonClick(object sender, RoutedEventArgs e)
        {
            repeatpassword = repeatpasswordtext.Password;
            if (!string.IsNullOrEmpty(nametext.Text) || !string.IsNullOrEmpty(surnametext.Text) || !string.IsNullOrEmpty(patronymictext.Text) ||
                !string.IsNullOrEmpty(emailtext.Text) || !string.IsNullOrEmpty(snilstext.Text) || !string.IsNullOrEmpty(passwordtext.Password) ||
                !string.IsNullOrEmpty(repeatpasswordtext.Password))
            {
                if (passwordtext.Password == repeatpasswordtext.Password) //пароли совпадают
                {
                    using (var dbContext = new PolycCursContext())
                    {
                        if (dbContext.Users.Any(user => user.Useremail == emailtext.Text)) //проверяем существует ли введенный email
                        {
                            MessageBox.Show("Пользователь c указанным email уже существует!");
                            emailtext.Text = null;
                        }
                        else
                        {
                            if (maleradiobutton.IsChecked == true)
                            {
                                gender = "Мужской";
                            }
                            else if (femaleradiobutton.IsChecked == true)
                            {
                                gender = "Женский";
                            }

                            User newUser = new User
                            {
                                Username = nametext.Text,
                                Usersurname = surnametext.Text,
                                Userpatronymicname = patronymictext.Text,
                                Gender = gender,
                                Useremail = emailtext.Text,
                                Birthdate = DateOnly.FromDateTime(datetext.SelectedDate.GetValueOrDefault()),
                                Snils = snilstext.Text,
                                Userphone = phonetext.Text,
                                Userpassword = passwordtext.Password,
                            };

                            dbContext.Users.Add(newUser);
                            dbContext.SaveChanges(); // Сохранение изменений в базе данных

                            MessageBox.Show("Пользователь успешно зарегистрирован!");
                            UserAuth.UserAuthorized = true;

                            MainWindow w = new MainWindow(newUser, null);
                            w.Show();
                            this.Close();
                        }
                    }
                }

                else
                {
                    MessageBox.Show("Пароли не совпадают!");
                }
            }

            //Какое-то из полей пустое
            else
            {
                MessageBox.Show("Не все поля заполнены");
            }
        }

        private void SignIn_ButtonClick(object sender, RoutedEventArgs e)
        {
            AuthorizationPage page = new AuthorizationPage();
            page.Show();
            this.Close();
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Environment.Exit(0);
        }
    }
}
