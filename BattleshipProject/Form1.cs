using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BattleshipProject
{
    public partial class Form1 : Form
    {
        // ---------------------------------------------------------------
        //  SABİTLER
        // ---------------------------------------------------------------
        private const int GridSize = 10;
        private const int TotalShipCells = 17;   // 5 + 4 + 3 + 3 + 2
        private const int RadarSize = 4;         // Radar 4x4 alan tarar
        private const int TorpedoSize = 3;       // Torpido 3x3 alana vurur
        private const int StartRadarCount = 3;
        private const int StartTorpedoCount = 2;

        // ---------------------------------------------------------------
        //  OYUN VERİLERİ
        // ---------------------------------------------------------------
        Button[,] playerGrid = new Button[GridSize, GridSize];
        Button[,] enemyGrid = new Button[GridSize, GridSize];
        int buttonSize = 30; // Butonların genişlik ve yüksekliği (30x30 piksel)

        // Hücrelerin o anki durumu
        enum CellState { Bos, GemiVar, Vuruldu, Karavana }

        // Arka plandaki görünmez haritalar
        CellState[,] playerMap = new CellState[GridSize, GridSize];
        CellState[,] enemyMap = new CellState[GridSize, GridSize];
        Random rnd = new Random();

        // Filo: 1 adet 5'lik, 1 adet 4'lük, 2 adet 3'lük, 1 adet 2'lik gemi (toplam 17 kare)
        int[] shipSizes = { 5, 4, 3, 3, 2 };

        // Her geminin ayrı rengi olsun ki yan yana gelince karışmasınlar
        static readonly Color[] ShipColors =
        {
            Color.FromArgb(90, 105, 125),
            Color.FromArgb(105, 95, 130),
            Color.FromArgb(85, 120, 110),
            Color.FromArgb(125, 105, 85),
            Color.FromArgb(120, 90, 100)
        };

        // Yetenek hakları ve durumları
        int radarCount = StartRadarCount;
        int torpedoCount = StartTorpedoCount;
        bool isRadarActive = false;
        bool isTorpedoActive = false;

        // Online oyunda en son attığımız radarın bölgesi (sonuç gelince boyamak için)
        int lastRadarRow = 0;
        int lastRadarCol = 0;

        // Bilgisayar rakibin "vurduktan sonra çevresini dene" listesi
        List<Point> targetQueue = new List<Point>();

        bool gameOver = false;

        // ---------------------------------------------------------------
        //  AĞ (MULTIPLAYER) DEĞİŞKENLERİ
        // ---------------------------------------------------------------
        private TcpClient client;
        private TcpListener server;
        private StreamReader receive;
        private StreamWriter send;

        private bool isOnline = false; // Oyun çok oyunculu mu oynanıyor?
        private bool myTurn = false;   // Atış sırası bende mi?

        // ---------------------------------------------------------------
        //  FORM
        // ---------------------------------------------------------------
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // Designer'daki paneller form küçülünce 10. satırı kesiyordu.
            // Panelleri tam 10x10'luk hücre alanına göre kodla boyutlandırıyoruz.
            pnlPlayer.Size = new Size(GridSize * buttonSize, GridSize * buttonSize);
            pnlEnemy.Size = new Size(GridSize * buttonSize, GridSize * buttonSize);

            CreateGrids();

            // --- TERMİNAL TASARIMI (oyun çıktısı) ---
            rtbOutput.Clear(); // Tasarımcıdaki "Output" yazısını sil
            rtbOutput.ReadOnly = true;
            rtbOutput.BackColor = Color.FromArgb(20, 20, 20);
            rtbOutput.ForeColor = Color.LimeGreen;
            rtbOutput.Font = new Font("Consolas", 10, FontStyle.Bold);

            // --- SOHBET KUTUSU ---
            rtbChat.ReadOnly = true;
            rtbChat.BackColor = Color.FromArgb(30, 30, 30);
            rtbChat.ForeColor = Color.Gainsboro;
            rtbChat.Font = new Font("Segoe UI", 9);

            // Bağlantı alanları için varsayılanlar
            if (txtPort.Text.Trim().Length == 0) txtPort.Text = "5000";
            if (txtIP.Text.Trim().Length == 0) txtIP.Text = "127.0.0.1";
            txtIP.Enabled = rbClient.Checked; // IP sadece istemci modunda gerekli

            // Enter tuşuyla sohbet mesajı gönder
            txtSohbetMesaji.KeyDown += (s, ev) =>
            {
                if (ev.KeyCode == Keys.Enter)
                {
                    ev.SuppressKeyPress = true;
                    btnGonder_Click(s, EventArgs.Empty);
                }
            };

            this.FormClosing += Form1_FormClosing;

            // Offline (bilgisayara karşı) oyunla başla
            ResetGame(true);
            Log("Bilgisayara karşı oyun başladı. Düşman haritasına tıklayarak atış yap!");
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            try { if (client != null) client.Close(); } catch (Exception) { }
            try { if (server != null) server.Stop(); } catch (Exception) { }
        }

        // Oyun olaylarını alttaki çıktı kutusuna yazar
        private void Log(string text)
        {
            rtbOutput.AppendText(text + "\n");
            rtbOutput.ScrollToCaret();
        }

        // Sohbet mesajlarını sağdaki sohbet kutusuna yazar
        private void LogChat(string text)
        {
            rtbChat.AppendText(text + "\n");
            rtbChat.ScrollToCaret();
        }

        // ---------------------------------------------------------------
        //  IZGARALAR VE OYUN SIFIRLAMA
        // ---------------------------------------------------------------
        private void CreateGrids()
        {
            for (int i = 0; i < GridSize; i++)
            {
                for (int j = 0; j < GridSize; j++)
                {
                    // --- Oyuncu haritası butonu ---
                    Button btnPlayer = CreateCellButton(i, j, "btnPlayer");
                    pnlPlayer.Controls.Add(btnPlayer);
                    playerGrid[i, j] = btnPlayer;

                    // --- Düşman haritası butonu ---
                    Button btnEnemy = CreateCellButton(i, j, "btnEnemy");
                    btnEnemy.Click += EnemyGrid_Click;
                    pnlEnemy.Controls.Add(btnEnemy);
                    enemyGrid[i, j] = btnEnemy;
                }
            }
        }

        private Button CreateCellButton(int row, int col, string prefix)
        {
            Button b = new Button();
            b.Size = new Size(buttonSize, buttonSize);
            b.Location = new Point(col * buttonSize, row * buttonSize);
            b.Name = $"{prefix}_{row}_{col}";
            b.Tag = new Point(row, col); // X = satır, Y = sütun
            b.FlatStyle = FlatStyle.Flat;
            b.FlatAppearance.BorderColor = Color.CadetBlue;
            b.BackColor = Color.SteelBlue;
            b.ForeColor = Color.White;
            b.Font = new Font("Segoe UI Emoji", 10, FontStyle.Bold);
            return b;
        }

        // Haritaları, hakları ve butonları sıfırlar. Offline'da düşman gemilerini de yerleştirir.
        private void ResetGame(bool placeEnemyShips)
        {
            // Önceki turdan kalan gemi panellerini temizle ki üst üste binmesinler
            foreach (Panel old in pnlPlayer.Controls.OfType<Panel>().ToList())
            {
                pnlPlayer.Controls.Remove(old);
                old.Dispose();
            }

            for (int i = 0; i < GridSize; i++)
            {
                for (int j = 0; j < GridSize; j++)
                {
                    playerMap[i, j] = CellState.Bos;
                    enemyMap[i, j] = CellState.Bos;

                    playerGrid[i, j].BackColor = Color.SteelBlue;
                    playerGrid[i, j].Text = "";
                    playerGrid[i, j].BringToFront();

                    enemyGrid[i, j].BackColor = Color.SteelBlue;
                    enemyGrid[i, j].Text = "";
                    enemyGrid[i, j].Enabled = true;
                }
            }

            radarCount = StartRadarCount;
            torpedoCount = StartTorpedoCount;
            isRadarActive = false;
            isTorpedoActive = false;
            gameOver = false;
            targetQueue.Clear();
            pnlEnemy.Enabled = true;

            PlaceShips(playerMap, playerGrid);
            if (placeEnemyShips)
                PlaceShips(enemyMap, null);

            UpdateAbilityButtons();
        }

        // ---------------------------------------------------------------
        //  GEMİ YERLEŞTİRME
        // ---------------------------------------------------------------
        // Gemi sığıyor mu, ve başka bir gemiye (çaprazlar dahil) değmiyor mu?
        private bool CanPlaceShip(CellState[,] map, int row, int col, int length, int direction)
        {
            int endRow = direction == 0 ? row : row + length - 1;
            int endCol = direction == 0 ? col + length - 1 : col;

            if (endRow >= GridSize || endCol >= GridSize) return false;

            // Geminin kapladığı alan + çevresindeki 1 karelik boşluk tamamen boş olmalı
            for (int r = row - 1; r <= endRow + 1; r++)
            {
                for (int c = col - 1; c <= endCol + 1; c++)
                {
                    if (InBounds(r, c) && map[r, c] != CellState.Bos) return false;
                }
            }
            return true;
        }

        // Tüm filoyu (5-4-3-3-2) yerleştirmeyi dener. Sığmazsa false döner.
        // placements: her gemi için {satır, sütun, uzunluk, yön}
        private bool TryPlaceFleet(CellState[,] map, List<int[]> placements)
        {
            for (int i = 0; i < GridSize; i++)
                for (int j = 0; j < GridSize; j++)
                    map[i, j] = CellState.Bos;
            placements.Clear();

            foreach (int shipLength in shipSizes)
            {
                bool placed = false;
                for (int attempt = 0; attempt < 500 && !placed; attempt++)
                {
                    int direction = rnd.Next(2); // 0 = Yatay, 1 = Dikey
                    int startRow = rnd.Next(GridSize);
                    int startCol = rnd.Next(GridSize);

                    if (!CanPlaceShip(map, startRow, startCol, shipLength, direction))
                        continue;

                    for (int i = 0; i < shipLength; i++)
                    {
                        int r = direction == 0 ? startRow : startRow + i;
                        int c = direction == 0 ? startCol + i : startCol;
                        map[r, c] = CellState.GemiVar;
                    }
                    placements.Add(new int[] { startRow, startCol, shipLength, direction });
                    placed = true;
                }
                if (!placed) return false; // Bu dizilim tıkandı, baştan denenecek
            }
            return true;
        }

        // Filoyu yerleştirir. paintGrid verilirse gemiler oyuncu haritasında çizilir.
        private void PlaceShips(CellState[,] map, Button[,] paintGrid)
        {
            List<int[]> placements = new List<int[]>();

            // Dizilim tıkanırsa baştan dene (5-4-3-3-2 gemisi 10x10'a rahat sığar)
            while (!TryPlaceFleet(map, placements)) { }

            if (paintGrid == null) return;

            for (int k = 0; k < placements.Count; k++)
            {
                int[] sh = placements[k];
                DrawShipPanel(sh[0], sh[1], sh[2], sh[3], ShipColors[k % ShipColors.Length]);
            }
        }

        // Oyuncu haritasında gemiyi gerçek gemi şeklinde çizer (sivri pruva + her kare için bir lombar)
        private void DrawShipPanel(int startRow, int startCol, int length, int direction, Color color)
        {
            int w = direction == 0 ? length * buttonSize - 4 : buttonSize - 4;
            int h = direction == 0 ? buttonSize - 4 : length * buttonSize - 4;

            Panel pnlShip = new Panel();
            pnlShip.Size = new Size(w, h);
            pnlShip.Location = new Point(startCol * buttonSize + 2, startRow * buttonSize + 2);
            pnlShip.BackColor = color;

            // L = geminin uzun kenarı, T = kalınlığı. Pruva (sivri uç) yatayda sağda, dikeyde yukarıda.
            int L = direction == 0 ? w : h;
            int T = direction == 0 ? h : w;
            int nose = Math.Min(T * 8 / 10, L / 2);

            // Gövde köşeleri (u: gemi boyunca, v: gemi enine)
            int[,] hull = { { 0, 0 }, { L - nose, 0 }, { L, T / 2 }, { L - nose, T }, { 0, T } };
            Point[] pts = new Point[5];
            for (int i = 0; i < 5; i++)
            {
                int u = hull[i, 0], v = hull[i, 1];
                pts[i] = direction == 0 ? new Point(u, v) : new Point(v, L - u);
            }

            using (GraphicsPath path = new GraphicsPath())
            {
                path.AddPolygon(pts);
                pnlShip.Region = new Region(path); // Panel gemi şeklini alır
            }

            // Her kareye bir lombar çiz (geminin kaç kare olduğu görünsün)
            pnlShip.Paint += (sender, e) =>
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                using (Brush window = new SolidBrush(Color.FromArgb(235, 240, 245)))
                {
                    for (int i = 0; i < length; i++)
                    {
                        int u = i * buttonSize + buttonSize / 2 - 2; // Bu karenin merkezi
                        int cx = direction == 0 ? u : T / 2;
                        int cy = direction == 0 ? T / 2 : L - u;
                        e.Graphics.FillEllipse(window, cx - 4, cy - 4, 8, 8);
                    }
                }
            };

            pnlPlayer.Controls.Add(pnlShip);
            pnlShip.BringToFront();
        }

        // ---------------------------------------------------------------
        //  YARDIMCI METOTLAR
        // ---------------------------------------------------------------
        private static bool InBounds(int r, int c)
        {
            return r >= 0 && r < GridSize && c >= 0 && c < GridSize;
        }

        private static bool IsShot(CellState[,] map, int r, int c)
        {
            return map[r, c] == CellState.Vuruldu || map[r, c] == CellState.Karavana;
        }

        // Düşman haritasında bir kareyi işaretler (vurdu / karavana)
        private void MarkEnemyCell(int r, int c, bool hit)
        {
            enemyMap[r, c] = hit ? CellState.Vuruldu : CellState.Karavana;
            enemyGrid[r, c].BackColor = hit ? Color.Crimson : Color.SlateGray;
            enemyGrid[r, c].Text = hit ? "💥" : "X";
        }

        // Kendi haritamızda bir kareyi işaretler
        private void MarkOwnCell(int r, int c, bool hit)
        {
            playerMap[r, c] = hit ? CellState.Vuruldu : CellState.Karavana;
            playerGrid[r, c].BringToFront(); // Patlama geminin üstünde görünsün
            playerGrid[r, c].BackColor = hit ? Color.Crimson : Color.SlateGray;
            playerGrid[r, c].Text = hit ? "💥" : "X";
        }

        // Verilen sol üst köşeden size x size alandaki gemi parçası sayısı
        private int CountShipCells(CellState[,] map, int row, int col, int size)
        {
            int found = 0;
            for (int r = 0; r < size; r++)
                for (int c = 0; c < size; c++)
                    if (InBounds(row + r, col + c) && map[row + r, col + c] == CellState.GemiVar)
                        found++;
            return found;
        }

        // Radarın taradığı alanı düşman haritasında açık mavi ile gösterir (henüz atış yapılmamış kareler)
        private void TintRadarArea(int row, int col)
        {
            for (int r = 0; r < RadarSize; r++)
                for (int c = 0; c < RadarSize; c++)
                    if (InBounds(row + r, col + c) && !IsShot(enemyMap, row + r, col + c))
                        enemyGrid[row + r, col + c].BackColor = Color.LightSkyBlue;
        }

        private void PlaySound(string fileName)
        {
            try
            {
                string tamYol = Path.Combine(Application.StartupPath, fileName);
                if (!File.Exists(tamYol)) return; // Ses dosyası yoksa sessizce geç
                new System.Media.SoundPlayer(tamYol).Play();
            }
            catch (Exception)
            {
                // Ses çalınamasa da oyun devam etsin
            }
        }

        // Yetenek butonlarının yazısını ve açık/kapalı durumunu günceller
        private void UpdateAbilityButtons()
        {
            // Offline'da her zaman sıra bizde; online'da sadece sıra bizdeyken kullanılabilir
            bool canAct = !gameOver && (!isOnline || myTurn);

            btnRadar.Enabled = canAct && radarCount > 0;
            btnTorpedo.Enabled = canAct && torpedoCount > 0;

            btnRadar.Text = isRadarActive
                ? $"Radar İPTAL (Kalan: {radarCount})"
                : $"Radar Kullan (Kalan: {radarCount})";
            btnTorpedo.Text = isTorpedoActive
                ? $"Torpido İPTAL (Kalan: {torpedoCount})"
                : $"Torpido Kullan (Kalan: {torpedoCount})";
        }

        // ---------------------------------------------------------------
        //  OYUNCUNUN ATIŞI (DÜŞMAN HARİTASINA TIKLAMA)
        // ---------------------------------------------------------------
        private void EnemyGrid_Click(object sender, EventArgs e)
        {
            if (gameOver) return;

            Button clicked = sender as Button;
            if (clicked == null || !(clicked.Tag is Point)) return;

            Point pt = (Point)clicked.Tag;
            if (isOnline) OnlineFire(pt.X, pt.Y);
            else OfflineFire(pt.X, pt.Y);
        }

        // --- Bilgisayara karşı ---
        private void OfflineFire(int row, int col)
        {
            if (isTorpedoActive)
            {
                int hits = 0;
                for (int r = 0; r < TorpedoSize; r++)
                {
                    for (int c = 0; c < TorpedoSize; c++)
                    {
                        int rr = row + r, cc = col + c;
                        if (!InBounds(rr, cc) || IsShot(enemyMap, rr, cc)) continue;

                        bool hit = enemyMap[rr, cc] == CellState.GemiVar;
                        MarkEnemyCell(rr, cc, hit);
                        if (hit) hits++;
                    }
                }
                torpedoCount--;
                isTorpedoActive = false;
                PlaySound(hits > 0 ? "patlama.wav" : "su.wav");
                Log($"[TORPİDO RAPORU]: 3x3'lük alana ağır hasar verildi! Toplam {hits} isabet.");
            }
            else if (isRadarActive)
            {
                int found = CountShipCells(enemyMap, row, col, RadarSize);
                radarCount--;
                isRadarActive = false;
                TintRadarArea(row, col);
                Log($"[RADAR RAPORU]: Seçilen bölgede toplam {found} adet gemi parçası tespit edildi!");
            }
            else
            {
                if (IsShot(enemyMap, row, col))
                {
                    Log("Bu kareye zaten atış yaptın.");
                    return; // Sıra harcanmaz
                }

                bool hit = enemyMap[row, col] == CellState.GemiVar;
                MarkEnemyCell(row, col, hit);
                PlaySound(hit ? "patlama.wav" : "su.wav");
                Log(hit
                    ? $"Tebrikler! Düşman gemisini vurdun: Satır {row}, Sütun {col}"
                    : $"Karavana! Boşa atış yaptın: Satır {row}, Sütun {col}");
            }

            UpdateAbilityButtons();

            // Oyunu kazandıysak bilgisayar bir atış daha yapmasın
            if (CheckGameOver()) return;

            ComputerShoot();
            CheckGameOver();
        }

        // --- Online ---
        private void OnlineFire(int row, int col)
        {
            if (!myTurn)
            {
                Log("Henüz sıra sende değil! Rakibin hamlesini bekle.");
                return;
            }

            if (isTorpedoActive)
            {
                if (!SendLine($"TORPIDO_{row}_{col}")) return;
                torpedoCount--;
                isTorpedoActive = false;
                Log($"TORPİDO fırlatıldı ({row},{col})! Sonuç bekleniyor...");
            }
            else if (isRadarActive)
            {
                lastRadarRow = row;
                lastRadarCol = col;
                if (!SendLine($"RADAR_{row}_{col}")) return;
                radarCount--;
                isRadarActive = false;
                Log($"RADAR taraması yapılıyor ({row},{col})...");
            }
            else
            {
                if (IsShot(enemyMap, row, col))
                {
                    Log("Bu kareye zaten atış yaptın.");
                    return; // Sıra harcanmaz
                }
                if (!SendLine($"ATIS_{row}_{col}")) return;
                Log($"Füze fırlatıldı ({row},{col})! Sonuç bekleniyor...");
            }

            myTurn = false; // Sırayı devret
            UpdateAbilityButtons();
        }

        // ---------------------------------------------------------------
        //  YETENEK BUTONLARI
        // ---------------------------------------------------------------
        private void btnRadar_Click(object sender, EventArgs e)
        {
            if (gameOver) return;
            if (isOnline && !myTurn)
            {
                Log("Radarı sadece kendi sıranda kullanabilirsin.");
                return;
            }

            if (isRadarActive)
            {
                isRadarActive = false; // İkinci tıklama iptal eder, hak harcanmaz
                Log("Radar iptal edildi.");
            }
            else if (radarCount > 0)
            {
                isRadarActive = true;
                isTorpedoActive = false;
                Log("RADAR AKTİF! Düşman haritasında taramak istediğiniz 4x4'lük alanın sol üst köşesine tıklayın.");
            }
            UpdateAbilityButtons();
        }

        private void btnTorpedo_Click(object sender, EventArgs e)
        {
            if (gameOver) return;
            if (isOnline && !myTurn)
            {
                Log("Torpidoyu sadece kendi sıranda kullanabilirsin.");
                return;
            }

            if (isTorpedoActive)
            {
                isTorpedoActive = false; // İkinci tıklama iptal eder, hak harcanmaz
                Log("Torpido iptal edildi.");
            }
            else if (torpedoCount > 0)
            {
                isTorpedoActive = true;
                isRadarActive = false;
                Log("TORPİDO AKTİF! 3x3'lük hasar vermek istediğiniz alanın sol üst köşesine tıklayın.");
            }
            UpdateAbilityButtons();
        }

        // ---------------------------------------------------------------
        //  BİLGİSAYAR RAKİP
        // ---------------------------------------------------------------
        private void ComputerShoot()
        {
            int row = -1, col = -1;

            // 1) Önceki isabetin çevresindeki kareleri dene
            while (targetQueue.Count > 0 && row < 0)
            {
                Point p = targetQueue[0];
                targetQueue.RemoveAt(0);
                if (!IsShot(playerMap, p.X, p.Y))
                {
                    row = p.X;
                    col = p.Y;
                }
            }

            // 2) Yoksa dama tahtası düzeninde rastgele bir kare seç
            if (row < 0)
            {
                List<Point> all = new List<Point>();
                List<Point> parity = new List<Point>();
                for (int i = 0; i < GridSize; i++)
                {
                    for (int j = 0; j < GridSize; j++)
                    {
                        if (IsShot(playerMap, i, j)) continue;
                        all.Add(new Point(i, j));
                        if ((i + j) % 2 == 0) parity.Add(new Point(i, j));
                    }
                }

                List<Point> pool = parity.Count > 0 ? parity : all;
                if (pool.Count == 0) return;

                Point pick = pool[rnd.Next(pool.Count)];
                row = pick.X;
                col = pick.Y;
            }

            bool hit = playerMap[row, col] == CellState.GemiVar;
            MarkOwnCell(row, col, hit);

            if (hit)
            {
                Log($"Eyvah! Düşman gemini vurdu: Satır {row}, Sütun {col}");

                int[] dr = { -1, 1, 0, 0 };
                int[] dc = { 0, 0, -1, 1 };
                for (int k = 0; k < 4; k++)
                {
                    int nr = row + dr[k], nc = col + dc[k];
                    if (InBounds(nr, nc) && !IsShot(playerMap, nr, nc))
                        targetQueue.Add(new Point(nr, nc));
                }
            }
            else
            {
                Log($"Düşman atış yaptı ama isabet ettiremedi: Satır {row}, Sütun {col}");
            }
        }

        // ---------------------------------------------------------------
        //  OYUN SONU
        // ---------------------------------------------------------------
        // Oyun bittiyse true döner
        private bool CheckGameOver()
        {
            if (gameOver) return true;

            int seninVurdugunParca = 0;  // Rakibin haritasındaki isabetlerin
            int rakibinVurduguParca = 0; // Senin haritandaki isabetler

            for (int i = 0; i < GridSize; i++)
            {
                for (int j = 0; j < GridSize; j++)
                {
                    if (enemyMap[i, j] == CellState.Vuruldu) seninVurdugunParca++;
                    if (playerMap[i, j] == CellState.Vuruldu) rakibinVurduguParca++;
                }
            }

            if (seninVurdugunParca >= TotalShipCells)
            {
                gameOver = true;
                EndGame();
                Log("--- OYUN BİTTİ: KAZANDIN! ---");
                MessageBox.Show("Tebrikler, Kazandın! Düşmanın tüm donanmasını yok ettin! 🏆", "Oyun Bitti", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else if (rakibinVurduguParca >= TotalShipCells)
            {
                gameOver = true;
                EndGame();
                Log("--- OYUN BİTTİ: KAYBETTİN! ---");
                MessageBox.Show("Kaybettin! Tüm gemilerin batırıldı... 💀", "Oyun Bitti", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            return gameOver;
        }

        // Oyun bittiğinde haritayı ve yetenekleri kilitler
        private void EndGame()
        {
            isRadarActive = false;
            isTorpedoActive = false;
            pnlEnemy.Enabled = false;
            UpdateAbilityButtons(); // gameOver true olduğu için butonlar kapanır
        }

        // ---------------------------------------------------------------
        //  AĞ: BAĞLANTI KURMA
        // ---------------------------------------------------------------
        private async void button1_Click(object sender, EventArgs e)
        {
            Button connectButton = sender as Button;

            if (isOnline)
            {
                if (!gameOver)
                {
                    Log("Zaten bir online oyundasın.");
                    return;
                }
                CloseConnection(); // Biten oyundan sonra yeniden bağlanılabilsin
            }

            int port;
            if (!int.TryParse(txtPort.Text, out port) || port < 1 || port > 65535)
            {
                MessageBox.Show("Geçerli bir port numarası gir (1-65535).", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!rbServer.Checked && !rbClient.Checked)
            {
                MessageBox.Show("Önce Sunucu veya İstemci seçeneğini işaretle.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (connectButton != null) connectButton.Enabled = false; // Çift tıklamayı engelle

            try
            {
                if (rbServer.Checked)
                {
                    server = new TcpListener(IPAddress.Any, port);
                    server.Start();
                    Log("Sunucu başlatıldı, rakip bekleniyor...");

                    client = await server.AcceptTcpClientAsync();
                    server.Stop();

                    StartOnlineGame(true);
                }
                else
                {
                    client = new TcpClient();
                    await client.ConnectAsync(txtIP.Text.Trim(), port);

                    StartOnlineGame(false);
                }
            }
            catch (Exception ex)
            {
                CloseConnection();
                if (IsDisposed || Disposing) return; // Form kapanırken sunucu bekleme iptal edilir
                Log("Bağlantı kurulamadı: " + ex.Message);
            }
            finally
            {
                if (connectButton != null && !connectButton.IsDisposed) connectButton.Enabled = true;
            }
        }

        private void StartOnlineGame(bool isServer)
        {
            ResetGame(false); // Online'da düşman gemilerini rakip tutar, bizde yerleştirme yok

            NetworkStream stream = client.GetStream();
            receive = new StreamReader(stream);
            send = new StreamWriter(stream);
            send.AutoFlush = true;

            isOnline = true;
            myTurn = isServer; // Sunucu ilk atışı yapar

            Log(isServer
                ? "Rakip bağlandı! Oyun başlıyor. İlk atış sırası sende!"
                : "Sunucuya başarıyla bağlandınız! Oyun başlıyor. Rakibin atış yapması bekleniyor...");

            UpdateAbilityButtons();

            TcpClient myClient = client;
            StreamReader reader = receive;
            Task.Run(() => ReadLoop(myClient, reader));
        }

        private void CloseConnection()
        {
            isOnline = false;
            myTurn = false;
            try { if (client != null) client.Close(); } catch (Exception) { }
            try { if (server != null) server.Stop(); } catch (Exception) { }
        }

        // Bağlantı koptuğunda veya rakip ayrıldığında çağrılır
        private void OnDisconnected()
        {
            if (!isOnline) return;

            bool wasFinished = gameOver;
            CloseConnection();

            if (!wasFinished)
            {
                gameOver = true;
                EndGame();
                Log("--- Bağlantı koptu, oyun sona erdi. ---");
            }
            else
            {
                Log("Rakiple bağlantı kapandı.");
            }
        }

        // ---------------------------------------------------------------
        //  AĞ: MESAJ GÖNDERME / ALMA
        // ---------------------------------------------------------------
        private bool SendLine(string line)
        {
            try
            {
                send.WriteLine(line);
                return true;
            }
            catch (Exception)
            {
                OnDisconnected();
                return false;
            }
        }

        // Arka planda rakipten gelen satırları okur
        private void ReadLoop(TcpClient myClient, StreamReader reader)
        {
            try
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    string msg = line;
                    this.Invoke(new MethodInvoker(delegate
                    {
                        if (ReferenceEquals(myClient, client)) HandleMessage(msg);
                    }));
                }
            }
            catch (Exception)
            {
                // Bağlantı hatası: aşağıda kopma olarak işlenecek
            }

            try
            {
                this.Invoke(new MethodInvoker(delegate
                {
                    if (ReferenceEquals(myClient, client)) OnDisconnected();
                }));
            }
            catch (Exception)
            {
                // Form kapanmış olabilir
            }
        }

        private static bool ParseCoord(string[] parts, int index, out int r, out int c)
        {
            r = 0;
            c = 0;
            if (parts.Length < index + 2) return false;
            if (!int.TryParse(parts[index], out r) || !int.TryParse(parts[index + 1], out c)) return false;
            return InBounds(r, c);
        }

        // Rakibin atış sonucunu kendi haritamıza işler (ses çalmaz, yalnızca durumu günceller)
        private string ReceiveShot(int r, int c, out bool isNew)
        {
            isNew = false;
            switch (playerMap[r, c])
            {
                case CellState.GemiVar:
                    MarkOwnCell(r, c, true);
                    isNew = true;
                    return "ISABET";
                case CellState.Bos:
                    MarkOwnCell(r, c, false);
                    isNew = true;
                    return "KARAVANA";
                case CellState.Vuruldu:
                    return "ISABET";
                default:
                    return "KARAVANA";
            }
        }

        // Rakibin hamlesi bittikten sonra sırayı bize verir (oyun bitmediyse)
        private void FinishIncomingAttack()
        {
            if (CheckGameOver()) return;
            myTurn = true;
            UpdateAbilityButtons();
            Log("Sıra sende!");
        }

        // Rakipten gelen tek bir mesajı işler (her zaman arayüz thread'inde çalışır)
        private void HandleMessage(string msg)
        {
            try
            {
                // Sohbet mesajları önekle ayrılır; oyun komutlarıyla karışmaz
                if (msg.StartsWith("CHAT|"))
                {
                    LogChat("Rakip: " + msg.Substring(5));
                    return;
                }

                string[] p = msg.Split('_');
                int r, c;

                switch (p[0])
                {
                    // Rakip normal füze attı
                    case "ATIS":
                        {
                            if (!ParseCoord(p, 1, out r, out c)) return;

                            bool isNew;
                            string result = ReceiveShot(r, c, out isNew);
                            SendLine($"SONUC_{r}_{c}_{result}");

                            if (isNew)
                            {
                                PlaySound(result == "ISABET" ? "patlama.wav" : "su.wav");
                                Log(result == "ISABET"
                                    ? $"Eyvah! Düşman gemini vurdu: Satır {r}, Sütun {c}"
                                    : $"Düşman atış yaptı ama isabet ettiremedi: Satır {r}, Sütun {c}");
                            }
                            FinishIncomingAttack();
                            return;
                        }

                    // Rakip torpido attı (3x3)
                    case "TORPIDO":
                        {
                            if (!ParseCoord(p, 1, out r, out c)) return;

                            bool anyHit = false, anyNew = false;
                            for (int dr = 0; dr < TorpedoSize; dr++)
                            {
                                for (int dc = 0; dc < TorpedoSize; dc++)
                                {
                                    int rr = r + dr, cc = c + dc;
                                    if (!InBounds(rr, cc)) continue;

                                    bool isNew;
                                    string result = ReceiveShot(rr, cc, out isNew);
                                    if (!isNew) continue;

                                    anyNew = true;
                                    if (result == "ISABET") anyHit = true;
                                    SendLine($"SONUC_{rr}_{cc}_{result}");
                                }
                            }
                            if (anyNew) PlaySound(anyHit ? "patlama.wav" : "su.wav");
                            Log("Düşman TORPİDO kullandı!");
                            FinishIncomingAttack();
                            return;
                        }

                    // Rakip radar kullandı (4x4)
                    case "RADAR":
                        {
                            if (!ParseCoord(p, 1, out r, out c)) return;

                            int found = CountShipCells(playerMap, r, c, RadarSize);
                            SendLine($"RADARSONUC_{found}");
                            Log("Düşman RADAR kullandı!");
                            FinishIncomingAttack();
                            return;
                        }

                    // Bizim radarımızın sonucu
                    case "RADARSONUC":
                        {
                            if (p.Length < 2) return;
                            TintRadarArea(lastRadarRow, lastRadarCol);
                            Log($"[RADAR RAPORU]: Seçilen bölgede toplam {p[1]} adet gemi parçası tespit edildi!");
                            return;
                        }

                    // Bizim attığımız füze/torpidonun sonucu
                    case "SONUC":
                        {
                            if (p.Length < 4 || !ParseCoord(p, 1, out r, out c)) return;

                            bool hit = p[3] == "ISABET";
                            if (!IsShot(enemyMap, r, c))
                            {
                                MarkEnemyCell(r, c, hit);
                                PlaySound(hit ? "patlama.wav" : "su.wav");
                                Log(hit
                                    ? $"İsabet! Düşman gemisini vurdun: Satır {r}, Sütun {c}"
                                    : $"Karavana! Boşa atış yaptın: Satır {r}, Sütun {c}");
                            }
                            CheckGameOver();
                            return;
                        }

                    default:
                        // Tanınmayan mesajlar yok sayılır
                        return;
                }
            }
            catch (Exception)
            {
                // Hatalı/bozuk mesaj oyunu çökertmesin
            }
        }

        // ---------------------------------------------------------------
        //  SOHBET
        // ---------------------------------------------------------------
        private void btnGonder_Click(object sender, EventArgs e)
        {
            if (!isOnline || client == null || !client.Connected)
            {
                MessageBox.Show("Önce bağlantı kurmalısınız!", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string mesaj = txtSohbetMesaji.Text.Replace("\r", " ").Replace("\n", " ").Trim();
            if (mesaj.Length == 0) return;

            if (!SendLine("CHAT|" + mesaj)) return;
            LogChat("Sen: " + mesaj);
            txtSohbetMesaji.Clear();
        }

        // ---------------------------------------------------------------
        //  TASARIMCIDA BAĞLI OLAN OLAYLAR
        // ---------------------------------------------------------------
        // IP kutusu sadece istemci modunda açık olsun
        private void rbClient_CheckedChanged(object sender, EventArgs e)
        {
            txtIP.Enabled = rbClient.Checked;
        }

        // Designer'da bağlı, silme (hata verir)
        private void rtbOutput_TextChanged(object sender, EventArgs e)
        {
        }
    }
}