using DiagramDesigner;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System;
using Microsoft.Win32;
using System.IO;

namespace DemoApp
{
    public partial class Window1 : Window
    {
        private Window1ViewModel window1ViewModel;

        public Window1()
        {
            InitializeComponent();

            window1ViewModel = new Window1ViewModel();
            this.DataContext = window1ViewModel;
            CommandBindings.AddRange(window1ViewModel.CommandBindings);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ItemsControl ic = ((MyDesigner.FindName("DemoDiagramControl") as DiagramControl).FindName("DesignerScrollViewer") as ScrollViewer).FindName("DesignerItemsControl") as ItemsControl;
            var itemsPanel = ic.ItemsPanel;
            var canvas = (Canvas)itemsPanel.LoadContent();
            Rect bounds = VisualTreeHelper.GetDescendantBounds(canvas);
            double dpi = 96d;

            // Create a RenderTargetBitmap based on the size of the canvas
            var renderBitmap = new RenderTargetBitmap(1000, 1000, 96d, 96d, PixelFormats.Pbgra32);

            // Render the canvas onto the RenderTargetBitmap
            renderBitmap.Render(canvas);

            // Create a PngBitmapEncoder and add the RenderTargetBitmap to it
            var pngEncoder = new PngBitmapEncoder();
            pngEncoder.Frames.Add(BitmapFrame.Create(renderBitmap));

            

            SaveFileDialog saveFileDialog = new()
            {
                Filter = "PNG File (*.png)|*.png|All Files (*.*)|*.*",
                Title = "Save diagram"
            };
            if (saveFileDialog.ShowDialog() == true)
            {
                try
                {
                    // Create a FileStream to save the PNG image
                    using (var fileStream = new FileStream(saveFileDialog.FileName, FileMode.Create))
                    {
                        // Save the PNG image to the FileStream
                        pngEncoder.Save(fileStream);
                    }
                }
                catch (Exception err)
                {
                    MessageBox.Show(err.ToString(), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }

        }
    }
}
