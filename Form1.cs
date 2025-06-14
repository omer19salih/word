using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace sonödev1
{
    public partial class Form1 : Form
    {
        private bool isDrawing = false; // Çizim yapılıp yapılmadığını kontrol eden değişken
        private Point lastPoint; // Son çizilen nokta
        private Bitmap canvas; // Çizim alanı
        private Graphics graphics; // Çizim için grafik nesnesi
        private Pen pen = new Pen(Color.Black, 3); // Kalem (renk siyah, genişlik 3)
        private bool isErasing = false; // Silme modunun aktif olup olmadığını kontrol eden değişken
        private ColorDialog colorDialog; // Renk seçici
        private string currentShape = "FreeDraw"; // Şu anki şekil (Serbest çizim olarak başlar)
        private Color fillColor = Color.Transparent; // Şekil içi renk (varsayılan olarak şeffaf)

        private List<Shape> shapes = new List<Shape>(); // Çizilen şekillerin saklandığı liste

        public Form1()
        {
            InitializeComponent();
            this.Text = "Gelişmiş Paint Uygulaması"; // Form başlığı
            this.Size = new Size(1000, 700); // Form boyutu
            this.BackColor = Color.White; // Arkaplan rengi beyaz

            // Canvas (tuval) ve grafik nesnesi başlatılıyor
            canvas = new Bitmap(this.ClientSize.Width, this.ClientSize.Height);
            graphics = Graphics.FromImage(canvas);
            graphics.Clear(Color.White); // Canvas temizleniyor
            colorDialog = new ColorDialog(); // Renk seçici başlatılıyor

            // Fare olayları için event handler'lar
            this.MouseDown += new MouseEventHandler(MouseDownHandler);
            this.MouseMove += new MouseEventHandler(MouseMoveHandler);
            this.MouseUp += new MouseEventHandler(MouseUpHandler);
            this.Paint += new PaintEventHandler(PaintHandler);

            // Menü başlatılıyor
            InitializeMenu();
        }

        private void InitializeMenu()
        {
            // MenuStrip oluşturuluyor
            MenuStrip menuStrip = new MenuStrip();
            this.MainMenuStrip = menuStrip;

            // "Dosya" menüsü oluşturuluyor ve öğeler ekleniyor
            ToolStripMenuItem fileMenuItem = new ToolStripMenuItem("Dosya");
            fileMenuItem.DropDownItems.Add("Yeni", null, NewFile_Click); // Yeni dosya
            fileMenuItem.DropDownItems.Add("Kaydet", null, btnSave_Click); // Kaydet
            menuStrip.Items.Add(fileMenuItem);

            // "Düzen" menüsü oluşturuluyor ve öğeler ekleniyor
            ToolStripMenuItem editMenuItem = new ToolStripMenuItem("Düzen");
            editMenuItem.DropDownItems.Add("Sil", null, btnClear_Click); // Silme
            menuStrip.Items.Add(editMenuItem);

            // "Şekil" menüsü oluşturuluyor ve şekil seçimleri ekleniyor
            ToolStripMenuItem shapeMenuItem = new ToolStripMenuItem("Şekil");
            shapeMenuItem.DropDownItems.Add("Serbest Çizim", null, ShapeMenu_Click); // Serbest çizim
            shapeMenuItem.DropDownItems.Add("Dikdörtgen", null, ShapeMenu_Click); // Dikdörtgen
            shapeMenuItem.DropDownItems.Add("Çember", null, ShapeMenu_Click); // Çember
            menuStrip.Items.Add(shapeMenuItem);

            // "Doldur" menüsü oluşturuluyor (şekil içi renk seçimi)
            ToolStripMenuItem fillMenuItem = new ToolStripMenuItem("Doldur");
            fillMenuItem.Click += FillColorMenu_Click; // Doldurma rengi seçimi
            menuStrip.Items.Add(fillMenuItem);

            // "Hakkımızda" menüsü oluşturuluyor ve öğe ekleniyor
            ToolStripMenuItem aboutMenuItem = new ToolStripMenuItem("Hakkımızda");
            aboutMenuItem.Click += AboutMenuItem_Click; // Hakkında bilgi
            menuStrip.Items.Add(aboutMenuItem);

            // Menü formun kontrol listesine ekleniyor
            this.Controls.Add(menuStrip);
        }

        // Doldurma rengi seçildiğinde yapılacak işlemler
        private void FillColorMenu_Click(object sender, EventArgs e)
        {
            if (colorDialog.ShowDialog() == DialogResult.OK)
            {
                fillColor = colorDialog.Color; // Seçilen rengi doldurma rengi olarak ayarla
            }
        }

        // Şekil menüsünden bir seçenek seçildiğinde yapılacak işlemler
        private void ShapeMenu_Click(object sender, EventArgs e)
        {
            var clickedItem = (ToolStripMenuItem)sender;
            currentShape = clickedItem.Text; // Seçilen şekli geçerli şekil olarak ayarla
        }

        // Yeni dosya seçildiğinde yapılacak işlemler
        private void NewFile_Click(object sender, EventArgs e)
        {
            graphics.Clear(Color.White); // Tuvali temizle
            this.Invalidate(); // Yeniden çizim iste
            shapes.Clear(); // Şekil listesini temizle
        }

        // "Hakkımızda" menüsüne tıklandığında yapılacak işlemler
        private void AboutMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Gelişmiş Paint Uygulaması\nYazar: Salih Ömer Uyar"); // Hakkında mesajı
        }

        // Renk seçimi butonuna tıklanırsa yapılacak işlemler
        private void btnColor_Click(object sender, EventArgs e)
        {
            if (colorDialog.ShowDialog() == DialogResult.OK)
            {
                pen.Color = colorDialog.Color; // Seçilen rengi kalem rengine uygula
            }
        }

        // Silme butonuna tıklanırsa yapılacak işlemler
        private void btnEraser_Click(object sender, EventArgs e)
        {
            isErasing = !isErasing; // Silme modunu değiştir
            pen.Color = isErasing ? Color.White : colorDialog.Color; // Eğer silme modunda ise beyaz renk, değilse seçilen rengi uygula
        }

        // Kalem boyutu değiştirildiğinde yapılacak işlemler
        private void trackBarSize_Scroll(object sender, EventArgs e)
        {
            pen.Width = ((TrackBar)sender).Value; // Kalem genişliğini güncelle
        }

        // Temizle butonuna tıklanırsa yapılacak işlemler
        private void btnClear_Click(object sender, EventArgs e)
        {
            graphics.Clear(Color.White); // Tuvali temizle
            this.Invalidate(); // Yeniden çizim iste
            shapes.Clear(); // Şekil listesini temizle
        }

        // Kaydet butonuna tıklanırsa yapılacak işlemler
        private void btnSave_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveDialog = new SaveFileDialog { Filter = "PNG Resmi|*.png" }; // Kaydetme penceresi
            if (saveDialog.ShowDialog() == DialogResult.OK)
            {
                canvas.Save(saveDialog.FileName); // Dosyayı seçilen yere kaydet
            }
        }

        // Fare basıldığında yapılacak işlemler
        private void MouseDownHandler(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                isDrawing = true; // Çizim başlamış
                lastPoint = e.Location; // Başlangıç noktasını kaydet
            }
        }

        // Fare hareket ettiğinde yapılacak işlemler
        private void MouseMoveHandler(object sender, MouseEventArgs e)
        {
            if (isDrawing)
            {
                if (currentShape == "FreeDraw")
                {
                    graphics.DrawLine(pen, lastPoint, e.Location); // Serbest çizim
                    lastPoint = e.Location; // Son noktayı güncelle
                }
                else if (currentShape == "Dikdörtgen")
                {
                    // Dikdörtgen çizme
                    graphics.Clear(Color.White); // Tuvali temizle
                    var width = e.X - lastPoint.X;
                    var height = e.Y - lastPoint.Y;
                    graphics.DrawRectangle(pen, lastPoint.X, lastPoint.Y, width, height);
                }
                else if (currentShape == "Çember")
                {
                    // Çember çizme
                    graphics.Clear(Color.White); // Tuvali temizle
                    var diameter = Math.Max(Math.Abs(e.X - lastPoint.X), Math.Abs(e.Y - lastPoint.Y));
                    graphics.DrawEllipse(pen, lastPoint.X, lastPoint.Y, diameter, diameter);
                }
                this.Invalidate(); // Yeniden çizim iste
            }
        }

        // Fare bırakıldığında yapılacak işlemler
        private void MouseUpHandler(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                isDrawing = false; // Çizim durdurulmuş

                // Çizim bitince şekli listeye ekle
                if (currentShape == "Dikdörtgen")
                {
                    var width = e.X - lastPoint.X;
                    var height = e.Y - lastPoint.Y;
                    shapes.Add(new Shape(currentShape, lastPoint, width, height, fillColor)); // Dikdörtgeni listeye ekle
                }
                else if (currentShape == "Çember")
                {
                    var diameter = Math.Max(Math.Abs(e.X - lastPoint.X), Math.Abs(e.Y - lastPoint.Y));
                    shapes.Add(new Shape(currentShape, lastPoint, diameter, diameter, fillColor)); // Çemberi listeye ekle
                }
            }
        }

        // Form üzerine çizim yapılması için kullanılan metot
        private void PaintHandler(object sender, PaintEventArgs e)
        {
            e.Graphics.DrawImage(canvas, Point.Empty); // Tuvali ekrana çiz

            // Şekilleri yeniden çiz
            foreach (var shape in shapes)
            {
                if (shape.Type == "Dikdörtgen")
                {
                    var rect = new Rectangle(shape.StartPoint.X, shape.StartPoint.Y, shape.Width, shape.Height);
                    if (shape.FillColor != Color.Transparent)
                    {
                        e.Graphics.FillRectangle(new SolidBrush(shape.FillColor), rect); // İçini doldur
                    }
                    e.Graphics.DrawRectangle(pen, rect); // Dikdörtgeni çiz
                }
                else if (shape.Type == "Çember")
                {
                    var diameter = shape.Width;
                    var rect = new Rectangle(shape.StartPoint.X, shape.StartPoint.Y, diameter, diameter);
                    if (shape.FillColor != Color.Transparent)
                    {
                        e.Graphics.FillEllipse(new SolidBrush(shape.FillColor), rect); // İçini doldur
                    }
                    e.Graphics.DrawEllipse(pen, rect); // Çemberi çiz
                }
            }
        }

        // Şekil sınıfı: Çizilen şekilleri saklamak için
        private class Shape
        {
            public string Type { get; set; }
            public Point StartPoint { get; set; }
            public int Width { get; set; }
            public int Height { get; set; }
            public Color FillColor { get; set; }

            public Shape(string type, Point startPoint, int width, int height, Color fillColor)
            {
                Type = type;
                StartPoint = startPoint;
                Width = width;
                Height = height;
                FillColor = fillColor;
            }
        }
    }
}
