using Demo.DataBase;
using Demo.View;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
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

namespace Demo.Page
{
    /// <summary>
    /// Логика взаимодействия для ProductPage.xaml
    /// </summary>
    public partial class ProductPage : Window
    {
        DataBase.TradeEntities entities;
        private User user;
        
        public ProductPage(DataBase.TradeEntities entities, User user)
        {
            InitializeComponent();

            this.entities = entities;
            this.user = user;

            
            Binding binding1 = new Binding();
            binding1.Source = entities.ManuFacters.ToList();
            cbManuFacter.SetBinding(ComboBox.ItemsSourceProperty, binding1);
            cbManuFacter.DisplayMemberPath = "Name";

            Binding binding2 = new Binding();
            binding2.Source = entities.Providers.ToList();
            cbProvider.SetBinding(ComboBox.ItemsSourceProperty, binding2);
            cbProvider.DisplayMemberPath = "Name";

            Binding binding3 = new Binding();
            binding3.Source = entities.ProductCategories.ToList();
            cbCategory.SetBinding(ComboBox.ItemsSourceProperty, binding3);
            cbCategory.DisplayMemberPath = "Name";


            ProductBlock.DataContext = new Product();
        }

        private void buttonSave_Click(object sender, RoutedEventArgs e)
        {
            Product product = ProductBlock.DataContext as Product;
            
                
            
            if (product.ProductName.Length == 0)
            {
                MessageBox.Show("Введите наименование товара");
                return;
            }
            if (cbCategory.SelectedItem == null)
            {
                MessageBox.Show("Выберите категорию");
                return;
            }
            if (cbManuFacter.SelectedItem == null)
            {
                MessageBox.Show("Выберите производителя");
                return;
            }
            if (cbProvider.SelectedItem == null)
            {
                MessageBox.Show("Выберите поставщик");
                return;
            }



            entities.Products.Add(product);
            entities.SaveChanges();

            ProductBlock.DataContext = new Product();
            
            this.Close();
            ProductView productView = new ProductView(entities, user);
            productView.Show();
            
        }

        private void SelectImage_Click(object sender, RoutedEventArgs e)
        {
            
            Product product = ProductBlock.DataContext as Product;
            if (product == null) return;

            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Файлы изображений|*.jpg;*.jpeg;*.png;";
            openFileDialog.Multiselect = false;
            if (openFileDialog.ShowDialog() == true)
            {
                byte[] imageBytes = File.ReadAllBytes(openFileDialog.FileName);

                if (imageBytes.Length > 0)
                {
                    BitmapImage bitmapImage = new BitmapImage();
                    using (MemoryStream memoryStream = new MemoryStream(imageBytes))
                    {
                        bitmapImage.BeginInit();
                        bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                        bitmapImage.StreamSource = memoryStream;
                        bitmapImage.EndInit();
                    }

                    if (bitmapImage.PixelWidth <= 300 && bitmapImage.PixelHeight <= 200)
                    {

                        string fileName = Guid.NewGuid().ToString() + System.IO.Path.GetExtension(openFileDialog.FileName);
                        string imagePath = "/Images/" + fileName;

                        string fullPath = AppDomain.CurrentDomain.BaseDirectory + imagePath;
                        File.WriteAllBytes(fullPath, imageBytes);



                        product.ProductPhoto = imageBytes;

                        ImagePreview.Source = bitmapImage;




                        MessageBox.Show("Фотография успешно добавлена.");
 
                    }
                    else
                    {
                        MessageBox.Show("Изображение должно иметь размер не более 300x200 пикселей.");
                    }
                }
                else
                {
                    MessageBox.Show("Выбранный файл изображения пуст.");
                }
            }
        }
    }
}
