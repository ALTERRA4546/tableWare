using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
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
using static Tableware.Authorization;
using static Tableware.ProductPanel;

namespace Tableware
{
    /// <summary>
    /// Логика взаимодействия для ProductPanel.xaml
    /// </summary>
    public partial class ProductPanel : Window
    {
        public ProductPanel()
        {
            InitializeComponent();
        }

        public bool exitMode = false;

        public class Product
        {
            public string ProductArticleNumber { get; set; }
            public BitmapImage ProductPhoto { get; set; }
            public string ProductName { get; set; }
            public string ProductDescription { get; set; }
            public string Manufacturer1 { get; set; }
            public double ProductCost { get; set; }
            public int ProductQuantityInStock { get; set; }
        }

        List<Product> productList = new List<Product>();

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            using (var dataBase = new TradeEntities())
            {
                var manufacturerData = dataBase.Manufacturer.Select(x => x.Manufacturer1).ToList();
                manufacturerData.Add("Все производители");
                Manufacturer.ItemsSource = manufacturerData;
                Manufacturer.SelectedItem = "Все производители";

                if (TempData.enterMode != -1)
                {
                    var userData = dataBase.User.FirstOrDefault(x => x.UserID == TempData.idUser);
                    FIO.Content = userData.UserSurname + " " + userData.UserName + " " + userData.UserPatronymic;
                }

                if (TempData.enterMode != 3)
                {
                    Add.Visibility = Visibility.Collapsed;
                    Remove.Visibility = Visibility.Collapsed;
                }

                ProductList.ItemsSource = productList;
            }
        }

        private void Initialization(string find, string manufacturerFind)
        {
            productList.Clear();
            using (var dataBase = new TradeEntities())
            {
                var productData = (from product in dataBase.Product
                                   join
                                   manufacturer in dataBase.Manufacturer on product.ProductManufacturer equals manufacturer.ManufacturerId into manufacturerGroup
                                   from manufacturer in manufacturerGroup.DefaultIfEmpty()
                                   where ((find == null || product.ProductName.ToLower().Contains(find.ToLower()) || product.ProductDescription.ToLower().Contains(find.ToLower())) && (manufacturerFind == null || manufacturer.Manufacturer1 == manufacturerFind))
                                   select new
                                   {
                                       product.ProductArticleNumber,
                                       product.ProductPhoto,
                                       product.ProductName,
                                       product.ProductDescription,
                                       manufacturer.Manufacturer1,
                                       product.ProductCost,
                                       product.ProductQuantityInStock,
                                   }).ToList();

                foreach (var line in productData)
                {
                    Product product = new Product();
                    product.ProductArticleNumber = line.ProductArticleNumber;
                    if (line.ProductPhoto != null)
                    {
                        byte[] photo = line.ProductPhoto;
                        BitmapImage bitmapImage = new BitmapImage();
                        bitmapImage.BeginInit();
                        bitmapImage.StreamSource = new MemoryStream(photo);
                        bitmapImage.EndInit();
                        product.ProductPhoto = bitmapImage;
                    }
                    else
                    {
                        product.ProductPhoto = new BitmapImage(new Uri($"C:\\Users\\King Night\\source\\repos\\Tableware\\Tableware\\Image\\picture.png"));
                    }
                    product.ProductName = line.ProductName;
                    product.ProductDescription = line.ProductDescription;
                    product.Manufacturer1 = line.Manufacturer1;
                    product.ProductCost = (double)line.ProductCost;
                    product.ProductQuantityInStock = (int)line.ProductQuantityInStock;

                    productList.Add(product);
                }

                var allproduct = dataBase.Product.ToList();

                CounterProduct.Content = productData.Count + " / " + allproduct.Count;

                ProductList.Items.Refresh();
            }
        }

        private void Filter()
        {
            string find = null;
            string manufacturer = null;

            if (Find.Text != "")
                find = Find.Text;
            else
                find = null;

            if (Manufacturer.SelectedItem.ToString() != "Все производители")
                manufacturer = Manufacturer.SelectedItem.ToString();
            else
                manufacturer = null;

            Initialization(find, manufacturer);
        }

        private void Exit_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            exitMode = true;
            this.Close();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (exitMode == false)
                Application.Current.Shutdown();
        }

        
        private void Manufacturer_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Filter();
        }

        private void Find_TextChanged(object sender, TextChangedEventArgs e)
        {
            Filter();
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            TempData.idSelectedProduct = null;
            AddChangeProduct addChangeProduct = new AddChangeProduct();
            addChangeProduct.ShowDialog();
            Filter();
        }

        private void Remove_Click(object sender, RoutedEventArgs e)
        {
            var selectedProduct = ProductList.SelectedItem as Product;
            string idProduct = selectedProduct.ProductArticleNumber;

            using (var dataBase = new TradeEntities())
            {
                if (MessageBox.Show("Вы действительно хотите удалить товар", "Внимание", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {
                    var order = dataBase.OrderProduct.FirstOrDefault(x => x.ProductArticleNumber == idProduct);
                    if (order != null)
                    {
                        var product = dataBase.Product.FirstOrDefault(x => x.ProductArticleNumber == idProduct);
                        dataBase.Product.Remove(product);

                        dataBase.SaveChanges();
                        MessageBox.Show("Запись удалена", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                        Filter();
                    }
                    else
                    {
                        MessageBox.Show("Продукт есть в заказе", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                }
            }
        }

        private void ProductList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (TempData.enterMode == 3)
            {
                var selectedProduct = ProductList.SelectedItem as Product;
                if (selectedProduct != null)
                {
                    TempData.idSelectedProduct = selectedProduct.ProductArticleNumber;
                    AddChangeProduct addChangeProduct = new AddChangeProduct();
                    addChangeProduct.ShowDialog();
                    Filter();
                }
            }
        }
    }
}
