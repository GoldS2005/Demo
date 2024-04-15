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

namespace Demo.View
{
    /// <summary>
    /// Логика взаимодействия для Authorization.xaml
    /// </summary>
    public partial class Authorization : Window
    {
        private readonly DataBase.TradeEntities entities;
        private DataBase.User user;
        private bool isRequireCaptcha;
        private string captchaCode;
        private readonly string captchaSymbols = "ABCDEFGHIKLMNOPQRSTVXYZ1234567890";
        private readonly Random random;
        private readonly double letterWidth = 40;
        
        public Authorization()
        {
            InitializeComponent();

            entities = new DataBase.TradeEntities();
            random = new Random(Environment.TickCount);

            
        }

        private void OnSingIn(object sender, RoutedEventArgs e)
        {
            if (isRequireCaptcha && captchaCode.ToLower() == tbCaptcha.Text.Trim())
            {
                MessageBox.Show("DSKSDAKSDASDFASF");
                return;
            }

            string login = tbLogin.Text.Trim();
            string password = tbPassword.Password.Trim();

            if (login.Length < 1 || password.Length < 1)
            {
                MessageBox.Show("Необходимо ввести логин");
                return;
            }

            user = entities.Users.Where(u => u.UserLogin == login && u.UserPassword == password).FirstOrDefault();
            if (user == null)
            {
                MessageBox.Show("Некорректный логин/пароль");
                GenerateCaptcha();
                tbLogin.Clear();
                tbPassword.Clear();
                return;
                
            }
            

            if (isRequireCaptcha)
            {
                isRequireCaptcha = false;
            }

            ProductView productView = new ProductView(entities, user);
            switch (user.Role.RoleName)
            {

                case "Администратор":
                    productView.Owner = this;
                    productView.Show();
                    Hide();
                    break;
                case "Менеджер":
                    productView.Owner = this;
                    productView.Show();
                    Hide();
                    productView.addButton.IsEnabled = false;
                    productView.deleteButton.IsEnabled = false;
                    productView.redacButton.IsEnabled = false;
                    break;
                case "Клиент":

                    productView.Owner = this;
                    productView.Show();
                    Hide();
                    productView.addButton.IsEnabled = false;
                    productView.deleteButton.IsEnabled = false;
                    productView.redacButton.IsEnabled = false;
                    break;

                   

            }
        }

        private void GenerateCaptcha()
        {
            captchaCode = GetNewCaptchaCode();

            for(int i = 0; i < captchaCode.Length; i++)
            {
                AddCharToCanvas(i, captchaCode[i]);
            }
            GenerateNoize();
        }

        private string GetNewCaptchaCode()
        {
            canvas.Children.Clear();
            string code = "";

            for (int i = 0; i < 4; i++)
            {
                code += captchaSymbols[random.Next(captchaSymbols.Length)];
            }

            return code;
        }

        private void AddCharToCanvas(int index, char ch)
        {
            Label label = new Label();
            label.FontSize = random.Next(32, 42);
            label.FontWeight = FontWeights.Black;
            label.Foreground = GetRandomBrush();
            label.Width = letterWidth;
            label.Height = 60;
            //label.BorderBrush = Brushes.Black;
            //label.BorderThickness = new Thickness(1);
            label.HorizontalContentAlignment = HorizontalAlignment.Center;
            label.VerticalContentAlignment = VerticalAlignment.Center;
            label.RenderTransformOrigin = new Point(0.5, 0.5);
            label.RenderTransform = new RotateTransform(random.Next(-20, 15));
            label.Content = ch.ToString();

            canvas.Children.Add(label);

            int startPosition = (int)((canvas.ActualWidth / 2) - (letterWidth * 4 / 2));

            Canvas.SetLeft(label, startPosition + (index * letterWidth));
            Canvas.SetTop(label, random.Next(-10, 10));
        }

        private void GenerateNoize()
        {
            for (int i = 1; i < 100; i++)
            {
                
                double x = random.NextDouble() * canvas.ActualWidth;
                double y = random.NextDouble() * canvas.ActualHeight;

                int radius = random.Next(4, 10);
                Ellipse ellipse = new Ellipse
                {
                    Width = radius,
                    Height = radius,
                    Fill = GetRandomBrush((byte)random.Next(100, 180)),
                    Stroke = Brushes.Transparent
                };

                canvas.Children.Add(ellipse);

                Canvas.SetLeft(ellipse, x);
                Canvas.SetTop(ellipse, y);

                
            }

            int lineCount = random.Next(2, 5);
            for (int i = 0; i < lineCount; i++)
            {
                Line line = new Line();
                

                line.X1 = random.Next(100, 120);
                line.Y1 = random.Next(10, 54);
                line.X2 = random.Next(260, 280);
                line.Y2 = random.Next(10, 54);
                line.Stroke = GetRandomBrush();
                line.StrokeThickness = random.Next(2, 4);

                canvas.Children.Add(line);




            }
        }

        private SolidColorBrush GetRandomBrush(byte alpha = 255)
        {
            return new SolidColorBrush(Color.FromArgb(alpha, (byte)random.Next(256), (byte)random.Next(256), (byte)random.Next(256)));
        }

    }
}
