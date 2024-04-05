using iTextSharp.text;
using Profisys_Zadanie.Class;
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

namespace Profisys_Zadanie.ListOfDocuments
{
    /// <summary>
    /// Interaction logic for DetailDocumentWindow.xaml
    /// </summary>
    public partial class DetailDocumentWindow : Window
    {
        public Profisys_Zadanie.Class.Document DetailDocument { get; set; }

        public DetailDocumentWindow(Profisys_Zadanie.Class.Document detailDoc)
        {
            InitializeComponent();
            DetailDocument = detailDoc;
            this.DataContext = DetailDocument;
            BruttoValueTxt.Text = DetailDocument.CalculateTotalPrice().ToString();
        }

        private void PrintBtn_Click(object sender, RoutedEventArgs e)
        {
            // Ukrycie elementów UI, które nie powinny być widoczne na wydruku
            var firstRow = MainGrid.Children.Cast<UIElement>().Where(x => Grid.GetRow(x) == 0);
            foreach (var element in firstRow)
            {
                element.Visibility = Visibility.Collapsed;
            }

            Dispatcher.BeginInvoke(new Action(() =>
            {
                RenderAndGeneratePDF();
                // Przywrócenie widoczności elementów UI
                foreach (var element in firstRow)
                {
                    element.Visibility = Visibility.Visible;
                }
            }), System.Windows.Threading.DispatcherPriority.Background);
        }

        private void RenderAndGeneratePDF()
        {
            MainGrid.UpdateLayout();

            int dpi = 96;
            RenderTargetBitmap renderTargetBitmap = new RenderTargetBitmap(
                (int)(MainGrid.ActualWidth * dpi / 96.0),
                (int)(MainGrid.ActualHeight * dpi / 96.0),
                dpi, dpi,
                PixelFormats.Pbgra32);
            renderTargetBitmap.Render(MainGrid);

            PngBitmapEncoder pngImage = new PngBitmapEncoder();
            pngImage.Frames.Add(BitmapFrame.Create(renderTargetBitmap));
            MemoryStream imageStream = new MemoryStream();
            pngImage.Save(imageStream);
            imageStream.Position = 0;

            using (MemoryStream memoryStream = new MemoryStream())
            {
                iTextSharp.text.Document document = new iTextSharp.text.Document(iTextSharp.text.PageSize.A4, 25, 25, 30, 30);
                iTextSharp.text.pdf.PdfWriter writer = iTextSharp.text.pdf.PdfWriter.GetInstance(document, memoryStream);
                document.Open();

                iTextSharp.text.Image image = iTextSharp.text.Image.GetInstance(imageStream.ToArray());
                image.ScaleToFit(document.PageSize.Width - document.LeftMargin - document.RightMargin, document.PageSize.Height - document.TopMargin - document.BottomMargin);
                image.Alignment = iTextSharp.text.Image.ALIGN_CENTER | iTextSharp.text.Image.ALIGN_MIDDLE;

                document.Add(image);
                document.Close();

                File.WriteAllBytes("Dokument.pdf", memoryStream.ToArray());
            }

            string filePath = System.IO.Path.Combine(Environment.CurrentDirectory, "Dokument.pdf");
            System.Diagnostics.Process.Start("explorer.exe", filePath);
        }



        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void ExitBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

    }
}
