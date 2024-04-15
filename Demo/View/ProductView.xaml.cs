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
using System.Collections.ObjectModel;
using System.ComponentModel;
using Microsoft.Win32;
using System.IO;
using Demo.DataBase;
using System.Globalization;
using System.Windows.Navigation;
using Demo.Page;
using Demo.Edit;

namespace Demo.View
{
    /// <summary>
    /// Логика взаимодействия для ProductView.xaml
    /// </summary>

    public class SortItem
    {
        public string Text { get; set; }
        public SortDescription Description { get; set; }
    }

    public partial class ProductView : Window
    {
        private readonly DataBase.TradeEntities entities;
        private readonly DataBase.ManuFacter allManuFacter;
        private User user;
        
        
       

        public DataBase.ManuFacter SelectedManuFacter { get; set; }
        public SortItem SelectedSort { get; set; }
        public ObservableCollection<DataBase.Product> Products { get; set; }
        public ObservableCollection<DataBase.ManuFacter> ManuFacters { get; set; }

        public ObservableCollection<SortItem> SortItems { get; set; }
        public ProductView(DataBase.TradeEntities entities1, User user)
        {
            InitializeComponent();

            entities = entities1;
            this.user = user;
            allManuFacter = new DataBase.ManuFacter() { ID = 0, Name = "Все производители" };

            Products = new ObservableCollection<DataBase.Product>(entities.Products);
            ManuFacters = new ObservableCollection<DataBase.ManuFacter>(entities.ManuFacters);
            ManuFacters.Insert(0, allManuFacter);

            SortItems = new ObservableCollection<SortItem>()
            {
                new SortItem() {Text = "Сортировать по возрастанию цены", Description = new SortDescription() { PropertyName = "ProductCost", Direction = ListSortDirection.Ascending} },
                new SortItem() {Text = "Сортировать по убыванию цены", Description = new SortDescription() { PropertyName = "ProductCost", Direction = ListSortDirection.Descending} }
            };

            

            DataContext = this;


        }

        private void Window_Closed(object sender, EventArgs e)
        {
            if (Owner != null)
            {
                Owner.Show();
            }
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {


            ApplyFilter();
        }

        private void ApplyFilter()
        {
            var searchString = tbSearch.Text.Trim().ToLower();


            var view = CollectionViewSource.GetDefaultView(lvProducts.ItemsSource);
            if (view == null) return;

            view.Filter = (object o) =>
            {
                var product = o as DataBase.Product;
                if (product == null) return false;



                if (searchString.Length > 0)
                {
                    if (!(product.ProductName.ToLower().Contains(searchString) ||
                     product.ProductDescription.ToLower().Contains(searchString) ||
                     product.ProductCategory1.Name.ToLower().Contains(searchString) ||
                     product.ManuFacter.Name.ToLower().Contains(searchString) ||
                     product.Provider.Name.ToLower().Contains(searchString)))
                    {
                        return false;
                    }


                }

                if (SelectedManuFacter != null && SelectedManuFacter != allManuFacter)
                {
                    if (product.ManuFacter != SelectedManuFacter)
                    {
                        return false;
                    }
                }

                return true;
            };
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ApplyFilter();
        }

        private void ApplySort()
        {
            var view = CollectionViewSource.GetDefaultView(lvProducts.ItemsSource);
            if (view == null) return;

            view.SortDescriptions.Clear();

            if (SelectedSort != null)
            {
                view.SortDescriptions.Add(SelectedSort.Description);
            }



        }

        private void ComboBox_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {
            ApplySort();
        }

        

        

        private void deleteButton_Click(object sender, RoutedEventArgs e)
        {
            
            if (lvProducts.SelectedItems.Count > 0)
            {
                var selectedProduct = lvProducts.SelectedItems[0] as Product;

                if (selectedProduct != null)
                {
                   
                    
                    Products.Remove(selectedProduct);
                    entities.Products.Remove(selectedProduct);
                    entities.SaveChanges();
                    
                    
                }
            }
        }

        private void addButton_Click(object sender, RoutedEventArgs e)
        {
        
            ProductPage productPage = new ProductPage(entities, user);
            productPage.Owner = this;
            productPage.Show();
            Hide();
       
        }

       
        
           
            
                

        private void redacButton_Click(object sender, RoutedEventArgs e)
        { 
            if (lvProducts.SelectedItems.Count > 0)
            {
               var selectedProduct = lvProducts.SelectedItems[0] as Product;

               if (selectedProduct != null)
               {
                        
                    ProductEdit productEdit = new ProductEdit(entities, user, selectedProduct);
                    productEdit.Owner = this;
                    productEdit.Show();
                    Hide(); 
               }
            }
                
        }
            
        
    }
}
