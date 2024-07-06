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
using static Tableware.Authorization;

namespace Tableware
{
    /// <summary>
    /// Логика взаимодействия для AddChangeProduct.xaml
    /// </summary>
    public partial class AddChangeProduct : Window
    {
        public AddChangeProduct()
        {
            InitializeComponent();
        }

        public string photoPath;

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            using (var dataBase = new TradeEntities())
            {
                var cayegoryData = dataBase.ProductCategory.Select(x => x.Category).ToList();
                Category.ItemsSource = cayegoryData;
                Category.SelectedIndex = 0;

                var measurementUnit = dataBase.MeasurementUnit.Select(x => x.MeasurementUnit1).ToList();
                MeasurementUnit.ItemsSource = measurementUnit;
                MeasurementUnit.SelectedIndex = 0;

                var supplayerData = dataBase.Manufacturer.Select(x => x.Manufacturer1).ToList();
                Supplay.ItemsSource = supplayerData;
                Supplay.SelectedIndex = 0;

                if (TempData.idSelectedProduct != null)
                {
                    var productData = (from product in dataBase.Product
                                       join
                                       manufacturer in dataBase.Manufacturer on product.ProductManufacturer equals manufacturer.ManufacturerId into manufacturerGroup
                                       from manufacturer in manufacturerGroup.DefaultIfEmpty()
                                       join
                                       category in dataBase.ProductCategory on product.ProductCategory equals category.CategoryId into categoryGroup
                                       from category in categoryGroup.DefaultIfEmpty()
                                       join
                                       productMeasurementUnit in dataBase.MeasurementUnit on product.MeasurementUnit equals productMeasurementUnit.MeasurementUnitId into productMeasurementUnitGroup
                                       from productMeasurementUnit in productMeasurementUnitGroup.DefaultIfEmpty()
                                       where (product.ProductArticleNumber == TempData.idSelectedProduct)
                                       select new
                                       {
                                           product.ProductArticleNumber,
                                           product.ProductName,
                                           category.Category,
                                           product.ProductQuantityInStock,
                                           productMeasurementUnit.MeasurementUnit1,
                                           manufacturer.Manufacturer1,
                                           product.ProductCost,
                                           product.ProductPhoto,
                                           product.ProductDescription,
                                       }).FirstOrDefault();

                    IdProduct.Text = productData.ProductArticleNumber;
                    ProductName.Text = productData.ProductName;
                    Category.SelectedItem = productData.Category;
                    ProductCount.Text = productData.ProductQuantityInStock.ToString();
                    MeasurementUnit.Text = productData.MeasurementUnit1;
                    Supplay.SelectedItem = productData.Manufacturer1;
                    Cost.Text = productData.ProductCost.ToString();
                    if (productData.ProductPhoto != null)
                    {
                        byte[] photo = productData.ProductPhoto;
                        BitmapImage bitmapImage = new BitmapImage();
                        bitmapImage.BeginInit();
                        bitmapImage.StreamSource = new MemoryStream(photo);
                        bitmapImage.EndInit();

                        Photo.Source = bitmapImage;
                    }
                    Discription.Text = productData.ProductDescription;
                }
            }
        }

        private void LoadPhoto_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = @"jpg (*.jpg)|*.jpg|png (*.png)|.png|bmp (*.bmp)|*.bmp";

            if (openFileDialog.ShowDialog() == true)
            {
                BitmapImage image = new BitmapImage(new Uri(openFileDialog.FileName));
                photoPath = openFileDialog.FileName;
                Photo.Source = image;
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            using (var dataBase = new TradeEntities())
            {
                if (ProductName.Text == "" || (int.TryParse(ProductCount.Text.ToString(), out _) && ProductCount.Text == "" && Convert.ToInt32(ProductCount.Text) < 0) || (float.TryParse(Cost.Text.ToString(), out _) && Cost.Text == "" && Convert.ToInt32(Cost.Text) < 0) || Discription.Text == "")
                {
                    MessageBox.Show("Не все поля верно заполнены", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (TempData.idSelectedProduct == null)
                {
                    var product = new Product();

                    product.ProductArticleNumber = IdProduct.Text;
                    product.ProductName = ProductName.Text;
                    var idCategory = dataBase.ProductCategory.FirstOrDefault(x => x.Category == Category.Text.ToString());
                    product.ProductCategory = idCategory.CategoryId;
                    product.ProductQuantityInStock = Convert.ToInt32(ProductCount.Text);
                    var idMeasurementUnit = dataBase.MeasurementUnit.FirstOrDefault(x => x.MeasurementUnit1 == MeasurementUnit.SelectedItem.ToString());
                    product.MeasurementUnit = idMeasurementUnit.MeasurementUnitId;
                    var idSypplayer = dataBase.Manufacturer.FirstOrDefault(x => x.Manufacturer1 == Supplay.SelectedItem.ToString());
                    product.ProductManufacturer = idSypplayer.ManufacturerId;
                    product.ProductCost = Convert.ToDecimal(Cost.Text);
                    if (photoPath != null)
                    {
                        product.ProductPhoto = File.ReadAllBytes(photoPath);
                    }
                    product.ProductDescription = Discription.Text;
                    dataBase.Product.Add(product);
                    dataBase.SaveChanges();

                    MessageBox.Show("Товар был добавлен", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                    this.Close();
                }
                else
                {
                    var product = dataBase.Product.FirstOrDefault(x=>x.ProductArticleNumber == TempData.idSelectedProduct);

                    product.ProductArticleNumber = IdProduct.Text;
                    product.ProductName = ProductName.Text;
                    var idCategory = dataBase.ProductCategory.FirstOrDefault(x => x.Category == Category.Text.ToString());
                    product.ProductCategory = idCategory.CategoryId;
                    product.ProductQuantityInStock = Convert.ToInt32(ProductCount.Text);
                    var idMeasurementUnit = dataBase.MeasurementUnit.FirstOrDefault(x => x.MeasurementUnit1 == MeasurementUnit.SelectedItem.ToString());
                    product.MeasurementUnit = idMeasurementUnit.MeasurementUnitId;
                    var idSypplayer = dataBase.Manufacturer.FirstOrDefault(x => x.Manufacturer1 == Supplay.SelectedItem.ToString());
                    product.ProductManufacturer = idSypplayer.ManufacturerId;
                    product.ProductCost = Convert.ToDecimal(Cost.Text);
                    if (photoPath != null)
                    {
                        product.ProductPhoto = File.ReadAllBytes(photoPath);
                    }
                    product.ProductDescription = Discription.Text;
                    dataBase.SaveChanges();

                    MessageBox.Show("Товар был сохранен", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                    this.Close();
                }
            }
        }
    }
}
