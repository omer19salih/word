using System;
using System.Drawing;
using System.Windows.Forms;
using System.IO;

namespace sonodev1
{
    public partial class Form2 : Form
    {
        // Kullanıcı arayüzü bileşenlerini tanımla
        private RichTextBox richTextBox1;
        private Panel formatPanel;
        private Button btnBold, btnItalic, btnUnderline, btnFont, btnColor, btnAlignLeft, btnAlignCenter, btnAlignRight;
        private Button btnBullet, btnNumbering, btnLineSpacing, btnIndent, btnOutdent;
        private Button btnInsertImage, btnInsertTable, btnInsertLink, btnInsertSymbol;

        // Form oluşturucusu, formu ve bileşenleri başlatır.
        public Form2()
        {
            InitializeComponent();
            InitializeUI();
            CreateMenu();
        }

        // Kullanıcı arayüzü elemanlarını başlatır: RichTextBox, butonlar ve olay işleyiciler.
        private void InitializeUI()
        {
            // RichTextBox'ı başlat ve ekle
            richTextBox1 = new RichTextBox { Dock = DockStyle.Fill };
            this.Controls.Add(richTextBox1);

            // Biçimlendirme panelini başlat (Metin biçimlendirme araçları için üst panel)
            formatPanel = new Panel { Dock = DockStyle.Top, Height = 40, BackColor = SystemColors.ControlLight };
            this.Controls.Add(formatPanel);

            // Biçimlendirme butonlarını başlat
            btnBold = CreateButton("K", FontStyle.Bold);
            btnItalic = CreateButton("T", FontStyle.Italic);
            btnUnderline = CreateButton("A", FontStyle.Underline);
            btnFont = CreateButton("Yazı Tipi");
            btnColor = CreateButton("Renk");
            btnAlignLeft = CreateButton("Sol", alignment: ContentAlignment.MiddleLeft);
            btnAlignCenter = CreateButton("Orta", alignment: ContentAlignment.MiddleCenter);
            btnAlignRight = CreateButton("Sağ", alignment: ContentAlignment.MiddleRight);
            btnBullet = CreateButton("Madde İşareti");
            btnNumbering = CreateButton("Numara");
            btnLineSpacing = CreateButton("Satır Aralığı");
            btnIndent = CreateButton("Girinti");
            btnOutdent = CreateButton("Çıkıntı");

            // İçerik ekleme butonlarını başlat
            btnInsertImage = CreateButton("Resim Ekle");
            btnInsertTable = CreateButton("Tablo Ekle");
            btnInsertLink = CreateButton("Bağlantı Ekle");
            btnInsertSymbol = CreateButton("Sembol Ekle");

            // Butonlara tıklama olayları ekle
            btnBold.Click += (s, e) => ToggleFontStyle(FontStyle.Bold);
            btnItalic.Click += (s, e) => ToggleFontStyle(FontStyle.Italic);
            btnUnderline.Click += (s, e) => ToggleFontStyle(FontStyle.Underline);
            btnFont.Click += btnFont_Click;
            btnColor.Click += btnColor_Click;
            btnAlignLeft.Click += (s, e) => richTextBox1.SelectionAlignment = HorizontalAlignment.Left;
            btnAlignCenter.Click += (s, e) => richTextBox1.SelectionAlignment = HorizontalAlignment.Center;
            btnAlignRight.Click += (s, e) => richTextBox1.SelectionAlignment = HorizontalAlignment.Right;
            btnBullet.Click += (s, e) => richTextBox1.SelectionBullet = !richTextBox1.SelectionBullet;
            btnIndent.Click += (s, e) => richTextBox1.SelectionIndent += 10;
            btnOutdent.Click += (s, e) => richTextBox1.SelectionIndent -= 10;
            btnInsertImage.Click += btnInsertImage_Click;
            btnInsertTable.Click += btnInsertTable_Click;
            btnInsertLink.Click += btnInsertLink_Click;
            btnInsertSymbol.Click += btnInsertSymbol_Click;

            // Format paneline butonları ekle
            formatPanel.Controls.AddRange(new Control[] {
                btnBold, btnItalic, btnUnderline, btnFont, btnColor, btnAlignLeft, btnAlignCenter, btnAlignRight,
                btnBullet, btnNumbering, btnLineSpacing, btnIndent, btnOutdent, btnInsertImage, btnInsertTable, btnInsertLink, btnInsertSymbol
            });

            // Butonların konumlarını ayarla
            int buttonX = 5;
            foreach (Control control in formatPanel.Controls)
            {
                control.Location = new Point(buttonX, 5);
                buttonX += control.Width + 5;
            }
        }

        // Butonları oluşturmak için yardımcı fonksiyon
        private Button CreateButton(string text, FontStyle style = FontStyle.Regular, ContentAlignment alignment = ContentAlignment.MiddleCenter)
        {
            Button button = new Button { Text = text, Width = 80, Font = new Font("Arial", 9, style), TextAlign = alignment };
            return button;
        }

        // Menü oluşturma
        private void CreateMenu()
        {
            MenuStrip menuStrip = new MenuStrip();
            this.MainMenuStrip = menuStrip;
            this.Controls.Add(menuStrip);

            // Dosya menüsünü oluştur
            ToolStripMenuItem fileMenu = new ToolStripMenuItem("Dosya");
            fileMenu.DropDownItems.Add("Yeni", null, (s, e) => richTextBox1.Clear());
            fileMenu.DropDownItems.Add("Aç", null, btnOpen_Click);
            fileMenu.DropDownItems.Add("Kaydet", null, btnSave_Click);
            fileMenu.DropDownItems.Add("Çıkış", null, (s, e) => Application.Exit());

            // Düzen menüsünü oluştur
            ToolStripMenuItem editMenu = new ToolStripMenuItem("Düzen");
            editMenu.DropDownItems.Add("Kes", null, (s, e) => richTextBox1.Cut());
            editMenu.DropDownItems.Add("Kopyala", null, (s, e) => richTextBox1.Copy());
            editMenu.DropDownItems.Add("Yapıştır", null, (s, e) => richTextBox1.Paste());

            // Biçim menüsünü oluştur
            ToolStripMenuItem formatMenu = new ToolStripMenuItem("Biçim");
            formatMenu.DropDownItems.Add("Yazı Tipi Seç", null, btnFont_Click);
            formatMenu.DropDownItems.Add("Yazı Rengi Seç", null, btnColor_Click);

            // Ekle menüsünü oluştur
            ToolStripMenuItem insertMenu = new ToolStripMenuItem("Ekle");
            insertMenu.DropDownItems.Add("Resim", null, btnInsertImage_Click);
            insertMenu.DropDownItems.Add("Tablo", null, btnInsertTable_Click);
            insertMenu.DropDownItems.Add("Bağlantı", null, btnInsertLink_Click);
            insertMenu.DropDownItems.Add("Sembol", null, btnInsertSymbol_Click);

            // Hakkında menüsünü oluştur
            ToolStripMenuItem aboutMenu = new ToolStripMenuItem("Hakkında", null,
                (s, e) => MessageBox.Show("Bu bir Word uygulamasıdır.\nGeliştiren: Sen 😎", "Hakkında", MessageBoxButtons.OK, MessageBoxIcon.Information));

            // Menü çubuğunu formda göster
            menuStrip.Items.AddRange(new ToolStripMenuItem[] { fileMenu, editMenu, formatMenu, insertMenu, aboutMenu });
        }

        // Yazı tipi butonuna tıklama işlevi
        private void btnFont_Click(object sender, EventArgs e)
        {
            using (FontDialog fontDialog = new FontDialog())
            {
                if (fontDialog.ShowDialog() == DialogResult.OK)
                {
                    richTextBox1.Font = fontDialog.Font;
                }
            }
        }

        // Yazı rengi butonuna tıklama işlevi
        private void btnColor_Click(object sender, EventArgs e)
        {
            using (ColorDialog colorDialog = new ColorDialog())
            {
                if (colorDialog.ShowDialog() == DialogResult.OK)
                    richTextBox1.ForeColor = colorDialog.Color;
            }
        }

        // Dosya kaydetme işlemi
        private void btnSave_Click(object sender, EventArgs e)
        {
            using (SaveFileDialog saveDialog = new SaveFileDialog
            {
                Filter = "Metin Dosyaları|*.txt|Word Dosyaları|*.doc|RTF Dosyaları|*.rtf",
                Title = "Dosya Kaydet"
            })
            {
                if (saveDialog.ShowDialog() == DialogResult.OK)
                {
                    string fileExtension = Path.GetExtension(saveDialog.FileName).ToLower();

                    if (fileExtension == ".txt")
                    {
                        // Düz metin dosyası olarak kaydet
                        File.WriteAllText(saveDialog.FileName, richTextBox1.Text);
                    }
                    else if (fileExtension == ".doc")
                    {
                        // Word dosyasına kaydetme işlemi henüz yapılmamış
                        MessageBox.Show("Word dosyasına kaydetme işlemi henüz uygulanmamış.");
                    }
                    else if (fileExtension == ".rtf")
                    {
                        // RTF dosyası olarak kaydet
                        richTextBox1.SaveFile(saveDialog.FileName, RichTextBoxStreamType.RichText);
                    }
                }
            }
        }

        // Dosya açma işlemi
        private void btnOpen_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openDialog = new OpenFileDialog
            {
                Filter = "Word Dosyaları|*.doc;*.docx|RTF Dosyaları|*.rtf|Metin Dosyaları|*.txt",
                Title = "Dosya Aç"
            })
            {
                if (openDialog.ShowDialog() == DialogResult.OK)
                {
                    string fileExtension = Path.GetExtension(openDialog.FileName).ToLower();

                    if (fileExtension == ".txt")
                    {
                        // Düz metin dosyasını aç
                        richTextBox1.Text = File.ReadAllText(openDialog.FileName);
                    }
                    else if (fileExtension == ".doc" || fileExtension == ".docx")
                    {
                        // Word dosyasını aç
                        OpenWordFile(openDialog.FileName);
                    }
                    else if (fileExtension == ".rtf")
                    {
                        // RTF dosyasını aç
                        richTextBox1.LoadFile(openDialog.FileName, RichTextBoxStreamType.RichText);
                    }
                }
            }
        }

        // Word dosyasını açma işlemi
        private void OpenWordFile(string fileName)
        {
            try
            {
                // Yeni bir Word Uygulaması örneği oluştur
                var wordApp = new Microsoft.Office.Interop.Word.Application();

                // Word dosyasını aç
                var wordDoc = wordApp.Documents.Open(fileName);

                // Word'ü görünür yap
                wordApp.Visible = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Word dosyasını açarken bir hata oluştu: " + ex.Message);
            }
        }

        // Resim ekleme işlemi
        private void btnInsertImage_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Resim Dosyaları|*.jpg;*.jpeg;*.png;*.bmp";
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                Image image = Image.FromFile(openFileDialog.FileName);
                Clipboard.SetImage(image);
                richTextBox1.Paste();
            }
        }

        
        // Tablo ekleme işlemi
        private void btnInsertTable_Click(object sender, EventArgs e)
        {
            int rows = 5;
            int cols = 3;
            string input = Microsoft.VisualBasic.Interaction.InputBox("Tablo Boyutlarını Girin (Satır x Sütun)", "Tablo Ekle", "5 x 3");

            if (!string.IsNullOrEmpty(input))
            {
                var dimensions = input.Split('x');
                if (dimensions.Length == 2 && int.TryParse(dimensions[0], out rows) && int.TryParse(dimensions[1], out cols))
                {
                    if (rows > 100 || cols > 100) // Girdi sınırlandırması
                    {
                        MessageBox.Show("Lütfen 100'den küçük satır ve sütun sayıları girin.");
                        return;
                    }

                    // Tabloyu basit metin formatında oluştur
                    string tableText = "\n";
                    for (int i = 0; i < rows; i++)
                    {
                        for (int j = 0; j < cols; j++)
                        {
                            tableText += "[ ]\t"; // Hücreyi temsil et
                        }
                        tableText += "\n";
                    }

                    // Tabloyu RichTextBox'a ekle
                    richTextBox1.SelectedText = tableText;
                }
                else
                {
                    MessageBox.Show("Geçersiz tablo boyutu. Lütfen doğru formatta girin (örneğin: 5 x 3).");
                }
            }
        }

        // Bağlantı ekleme işlemi (şu anda boş)
        private void btnInsertLink_Click(object sender, EventArgs e)
        {

            string url = Microsoft.VisualBasic.Interaction.InputBox("Bağlantı URL'sini Girin", "Bağlantı Ekle", "http://");
            string linkText = Microsoft.VisualBasic.Interaction.InputBox("Bağlantı Metnini Girin", "Bağlantı Metni", "");

            if (!string.IsNullOrEmpty(url))
            {
                if (Uri.TryCreate(url, UriKind.Absolute, out Uri validatedUri)) // URL doğrulaması
                {
                    if (string.IsNullOrEmpty(linkText))
                    {
                        linkText = url;
                    }

                    // HTML bağlantısı yerine, RichTextBox'da hiperlink ekleyelim
                    richTextBox1.SelectedText = linkText;
                    int linkStart = richTextBox1.SelectionStart;
                    int linkLength = linkText.Length;

                    // Bağlantıyı formatla
                    richTextBox1.Select(linkStart, linkLength);
                    richTextBox1.SelectionColor = Color.Blue;
                    richTextBox1.SelectionFont = new Font(richTextBox1.Font, FontStyle.Underline);

                    // Bu aşamada URL'yi eklemek için Hyperlink mantığını kullanabilirsiniz.
                    // Eğer gelişmiş özellikler kullanıyorsanız burada farklı bir yaklaşım izlenebilir.
                }
                else
                {
                    MessageBox.Show("Geçersiz URL.");
                }
            }
        }

        // Sembol ekleme işlemi (şu anda boş)
        private void btnInsertSymbol_Click(object sender, EventArgs e)
        {
            // Sembol seçimi için basit bir seçim kutusu ekleyelim
            string[] symbols = { "©", "®", "™", "€", "£", "¥", "√","@"};
            string selectedSymbol = Microsoft.VisualBasic.Interaction.InputBox("Sembol Seçin\n(Örnek:@, ©, ®, ™, €, £, ¥, √)", "Sembol Ekle", "©");

            if (Array.Exists(symbols, symbol => symbol == selectedSymbol))
            {
                // Sembolü ekle
                richTextBox1.SelectedText = selectedSymbol;
            }
            else
            {
                MessageBox.Show("Geçersiz sembol seçildi.");
            }
        }

        // Biçim stillerini değiştirme
        private void ToggleFontStyle(FontStyle style)
        {
            if (richTextBox1.SelectionFont != null)
            {
                Font currentFont = richTextBox1.SelectionFont;
                Font newFont = new Font(currentFont, currentFont.Style ^ style);
                richTextBox1.SelectionFont = newFont;
            }
        }
    }
}
