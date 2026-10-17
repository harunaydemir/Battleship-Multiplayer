using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace BattleshipProject
{
    // =====================================================================
    //  "KOMUTA MASASI" TEMASI
    //  Grafit zemin, pirinç (brass) vurgu, sonar camgöbeği, denizci isimleri.
    // =====================================================================
    public static class Theme
    {
        public static readonly Color Bg = Color.FromArgb(13, 18, 25);          // pencere zemini
        public static readonly Color Surface = Color.FromArgb(21, 29, 40);     // kart zemini
        public static readonly Color Edge = Color.FromArgb(40, 54, 71);        // kart kenarı
        public static readonly Color Field = Color.FromArgb(10, 15, 21);       // yazı alanları
        public static readonly Color Brass = Color.FromArgb(227, 178, 92);     // pirinç vurgu
        public static readonly Color Sonar = Color.FromArgb(79, 209, 197);     // sonar camgöbeği
        public static readonly Color Danger = Color.FromArgb(229, 72, 77);     // tehlike kırmızısı
        public static readonly Color Hit = Color.FromArgb(124, 214, 140);      // bizim isabetimiz
        public static readonly Color Text = Color.FromArgb(216, 225, 236);
        public static readonly Color Muted = Color.FromArgb(127, 142, 163);

        public static readonly Color WaterA = Color.FromArgb(14, 40, 56);
        public static readonly Color WaterB = Color.FromArgb(16, 46, 64);
        public static readonly Color WaterLine = Color.FromArgb(27, 70, 92);

        public static readonly Font Big = new Font("Segoe UI Semibold", 20f);
        public static readonly Font Heading = new Font("Segoe UI Semibold", 10.5f);
        public static readonly Font Chip = new Font("Segoe UI Semibold", 10f);
        public static readonly Font ButtonFont = new Font("Segoe UI Semibold", 9.5f);
        public static readonly Font Body = new Font("Segoe UI", 9.5f);
        public static readonly Font Small = new Font("Segoe UI", 8.5f);
        public static readonly Font Mono = new Font("Consolas", 9f, FontStyle.Bold);

        public static GraphicsPath RoundRect(Rectangle r, int radius)
        {
            int d = radius * 2;
            GraphicsPath p = new GraphicsPath();
            p.AddArc(r.X, r.Y, d, d, 180, 90);
            p.AddArc(r.Right - d, r.Y, d, d, 270, 90);
            p.AddArc(r.Right - d, r.Bottom - d, d, d, 0, 90);
            p.AddArc(r.X, r.Bottom - d, d, d, 90, 90);
            p.CloseFigure();
            return p;
        }
    }

    // =====================================================================
    //  KART: yuvarlak köşeli panel, başlık + sayaç çubuğu
    // =====================================================================
    public class Card : Panel
    {
        public string Title = "";
        public Color Accent = Theme.Brass;

        // Sağ üstteki "kalan parça" sayacı
        public int MeterValue = 0;
        public int MeterMax = 0;
        public string MeterLabel = "";
        public Color MeterColor = Theme.Sonar;

        // Kartın üzerine ek çizim yapmak isteyen (koordinat harfleri vb.)
        public Action<Graphics> PaintExtra;

        public Card()
        {
            SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw, true);
            BackColor = Theme.Surface;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.Clear(Parent != null ? Parent.BackColor : Theme.Bg);

            Rectangle r = new Rectangle(0, 0, Width - 1, Height - 1);
            using (GraphicsPath path = Theme.RoundRect(r, 10))
            using (SolidBrush fill = new SolidBrush(Theme.Surface))
            using (Pen edge = new Pen(Theme.Edge))
            {
                g.FillPath(fill, path);
                g.DrawPath(edge, path);
            }

            // Başlığın önündeki küçük vurgu çizgisi + başlık
            using (SolidBrush accent = new SolidBrush(Accent))
                g.FillRectangle(accent, 14, 11, 4, 16);
            using (SolidBrush text = new SolidBrush(Theme.Text))
                g.DrawString(Title, Theme.Heading, text, 26, 8);

            // Sayaç (kalan gemi parçası)
            if (MeterMax > 0)
            {
                SizeF ts = g.MeasureString(MeterLabel, Theme.Mono);
                using (SolidBrush muted = new SolidBrush(Theme.Muted))
                    g.DrawString(MeterLabel, Theme.Mono, muted, Width - 16 - ts.Width, 6);

                Rectangle bar = new Rectangle(Width - 16 - 120, 26, 120, 4);
                using (SolidBrush back = new SolidBrush(Theme.Field))
                    g.FillRectangle(back, bar);
                int fillW = 120 * Math.Max(0, Math.Min(MeterValue, MeterMax)) / MeterMax;
                if (fillW > 0)
                {
                    using (SolidBrush meter = new SolidBrush(MeterColor))
                        g.FillRectangle(meter, bar.X, bar.Y, fillW, bar.Height);
                }
            }

            // Başlık altı ayırıcı çizgi
            using (Pen line = new Pen(Theme.Edge))
                g.DrawLine(line, 12, 36, Width - 13, 36);

            if (PaintExtra != null) PaintExtra(g);
        }
    }

    // =====================================================================
    //  ÜST ŞERİT: pusula, başlık, mod ve durum etiketi
    // =====================================================================
    public class HeaderBar : Panel
    {
        public string ModeText = "";
        public string StatusText = "";
        public Color StatusColor = Theme.Brass;

        public HeaderBar()
        {
            SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw, true);
            BackColor = Theme.Bg;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.Clear(Theme.Bg);

            DrawCompass(g, 52, Height / 2, 22);

            using (SolidBrush brass = new SolidBrush(Theme.Brass))
                g.DrawString("AMİRAL BATTI", Theme.Big, brass, 88, 8);
            using (SolidBrush muted = new SolidBrush(Theme.Muted))
                g.DrawString("KOMUTA MERKEZİ  ·  " + ModeText, Theme.Small, muted, 91, 48);

            // Sağdaki durum etiketi
            if (StatusText.Length > 0)
            {
                SizeF sz = g.MeasureString(StatusText, Theme.Chip);
                int w = (int)sz.Width + 48;
                int h = 34;
                Rectangle chip = new Rectangle(Width - w - 24, (Height - h) / 2 - 2, w, h);
                using (GraphicsPath path = Theme.RoundRect(chip, 17))
                using (SolidBrush f = new SolidBrush(Color.FromArgb(38, StatusColor)))
                using (Pen p = new Pen(StatusColor, 1.2f))
                {
                    g.FillPath(f, path);
                    g.DrawPath(p, path);
                }
                using (SolidBrush dot = new SolidBrush(StatusColor))
                    g.FillEllipse(dot, chip.X + 14, chip.Y + h / 2 - 4, 8, 8);
                using (SolidBrush tb = new SolidBrush(StatusColor))
                    g.DrawString(StatusText, Theme.Chip, tb, chip.X + 30, chip.Y + (h - sz.Height) / 2f);
            }

            // Alt kenarda pirinç gradyan çizgi
            using (LinearGradientBrush lg = new LinearGradientBrush(
                new Rectangle(0, Height - 2, Math.Max(1, Width), 2),
                Theme.Brass, Color.FromArgb(0, Theme.Brass), LinearGradientMode.Horizontal))
            {
                g.FillRectangle(lg, 0, Height - 2, Width, 2);
            }
        }

        private static void DrawCompass(Graphics g, int cx, int cy, int r)
        {
            using (Pen ring = new Pen(Theme.Brass, 1.5f))
                g.DrawEllipse(ring, cx - r, cy - r, r * 2, r * 2);

            int s = r - 3;
            int n = 5;
            Point[] star =
            {
                new Point(cx, cy - s), new Point(cx + n, cy - n),
                new Point(cx + s, cy), new Point(cx + n, cy + n),
                new Point(cx, cy + s), new Point(cx - n, cy + n),
                new Point(cx - s, cy), new Point(cx - n, cy - n)
            };
            using (SolidBrush b = new SolidBrush(Theme.Brass))
                g.FillPolygon(b, star);

            // Kuzey ibresi kırmızı
            Point[] north = { new Point(cx, cy - s), new Point(cx + 3, cy), new Point(cx - 3, cy) };
            using (SolidBrush red = new SolidBrush(Theme.Danger))
                g.FillPolygon(red, north);

            using (SolidBrush center = new SolidBrush(Theme.Bg))
                g.FillEllipse(center, cx - 2, cy - 2, 4, 4);
        }
    }

    // =====================================================================
    //  HÜCRE: haritadaki tek kare (su, gemi parçası, isabet, karavana...)
    // =====================================================================
    public enum CellMark { None, Miss, Hit }
    public enum ShipPart { None, Stern, Mid, Bow }

    public class BoardCell : Control
    {
        public int Row;
        public int Col;
        public bool IsEnemy;     // Düşman suyu hücresi mi (sonar süpürmesi + nişan)
        public bool Hover;       // Fare üstünde mi

        // Radar/torpido alan önizlemesi için form tarafından verilir
        public Func<BoardCell, bool> PreviewTest;
        public event EventHandler HoverChanged;

        // Tüm hücreler için ortak animasyon değerleri
        public static float SweepAngle;   // sonar tarama açısı (derece)
        public static bool SweepOn;
        public static float Pulse;        // 0..1 arası nabız (patlama)

        private CellMark mark = CellMark.None;
        private bool scanned = false;
        private ShipPart part = ShipPart.None;
        private bool vertical = false;
        private int tone = 0;

        // Gemi gövde renkleri (her geminin kendi tonu)
        private static readonly Color[] Tones =
        {
            Color.FromArgb(104, 122, 145),
            Color.FromArgb(126, 112, 150),
            Color.FromArgb(98, 140, 128),
            Color.FromArgb(150, 124, 98),
            Color.FromArgb(142, 104, 116)
        };

        public CellMark Mark
        {
            get { return mark; }
            set { mark = value; Invalidate(); }
        }

        public bool Scanned
        {
            get { return scanned; }
            set { scanned = value; Invalidate(); }
        }

        public BoardCell()
        {
            SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw, true);
            SetStyle(ControlStyles.Selectable, false);
            TabStop = false;
        }

        public void SetShip(ShipPart p, bool isVertical, int shipTone)
        {
            part = p;
            vertical = isVertical;
            tone = shipTone;
            Invalidate();
        }

        public void ResetCell()
        {
            mark = CellMark.None;
            scanned = false;
            part = ShipPart.None;
            vertical = false;
            Invalidate();
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);
            Hover = true;
            Invalidate();
            if (HoverChanged != null) HoverChanged(this, EventArgs.Empty);
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            Hover = false;
            Invalidate();
            if (HoverChanged != null) HoverChanged(this, EventArgs.Empty);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            int W = Width;
            int H = Height;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            // 1) Deniz (dama düzeninde iki ton)
            Color water = ((Row + Col) % 2 == 0) ? Theme.WaterA : Theme.WaterB;
            using (SolidBrush b = new SolidBrush(water))
                g.FillRectangle(b, 0, 0, W, H);

            // 2) Sonar süpürmesi (sadece düşman suları)
            if (IsEnemy && SweepOn && Parent != null)
            {
                double cx = Parent.Width / 2.0;
                double cy = Parent.Height / 2.0;
                double dx = Left + W / 2.0 - cx;
                double dy = Top + H / 2.0 - cy;
                double ang = Math.Atan2(dy, dx) * 180.0 / Math.PI;
                if (ang < 0) ang += 360.0;
                double diff = (SweepAngle - ang + 360.0) % 360.0;
                if (diff < 70.0)
                {
                    int a = (int)((1.0 - diff / 70.0) * 60.0);
                    using (SolidBrush sb = new SolidBrush(Color.FromArgb(a, Theme.Sonar)))
                        g.FillRectangle(sb, 0, 0, W, H);
                }
            }

            // 3) Radar taramış bölge
            if (scanned && mark == CellMark.None)
            {
                using (SolidBrush sc = new SolidBrush(Color.FromArgb(46, Theme.Sonar)))
                    g.FillRectangle(sc, 0, 0, W, H);
            }

            // 4) Harita çizgileri (gemiden önce çizilir ki gövde kesilmesin)
            using (Pen line = new Pen(Theme.WaterLine))
            {
                g.DrawLine(line, W - 1, 0, W - 1, H - 1);
                g.DrawLine(line, 0, H - 1, W - 1, H - 1);
            }

            // 5) Gemi gövdesi
            if (part != ShipPart.None) DrawHull(g, W, H);

            // 6) Radar/torpido alan önizlemesi
            if (PreviewTest != null && PreviewTest(this))
            {
                using (SolidBrush pv = new SolidBrush(Color.FromArgb(64, Theme.Brass)))
                    g.FillRectangle(pv, 0, 0, W, H);
                using (Pen pp = new Pen(Color.FromArgb(170, Theme.Brass)))
                    g.DrawRectangle(pp, 1, 1, W - 3, H - 3);
            }

            // 7) İşaretler
            float mx = W / 2f;
            float my = H / 2f;
            if (mark == CellMark.Miss)
            {
                // Karavana: beyaz iğne + yayılan halka
                using (Pen ring = new Pen(Color.FromArgb(90, 235, 242, 250), 1.5f))
                    g.DrawEllipse(ring, mx - 10, my - 10, 20, 20);
                using (SolidBrush peg = new SolidBrush(Color.FromArgb(232, 232, 238, 245)))
                    g.FillEllipse(peg, mx - 5, my - 5, 10, 10);
            }
            else if (mark == CellMark.Hit)
            {
                // İsabet: nabız gibi atan ateş topu
                float r = 9f + 3f * Pulse;
                using (SolidBrush scorch = new SolidBrush(Color.FromArgb(150, 90, 15, 20)))
                    g.FillEllipse(scorch, mx - r - 1, my - r - 1, (r + 1) * 2, (r + 1) * 2);

                using (GraphicsPath gp = new GraphicsPath())
                {
                    gp.AddEllipse(mx - r, my - r, r * 2, r * 2);
                    using (PathGradientBrush pg = new PathGradientBrush(gp))
                    {
                        pg.CenterColor = Color.FromArgb(255, 255, 214, 120);
                        pg.SurroundColors = new Color[] { Color.FromArgb(0, 229, 72, 77) };
                        pg.CenterPoint = new PointF(mx, my);
                        g.FillPath(pg, gp);
                    }
                }
                using (SolidBrush core = new SolidBrush(Color.FromArgb(255, 255, 240, 200)))
                    g.FillEllipse(core, mx - 3, my - 3, 6, 6);
                using (Pen spark = new Pen(Color.FromArgb(200, 255, 170, 80), 1.5f))
                {
                    g.DrawLine(spark, mx - r - 1, my, mx + r + 1, my);
                    g.DrawLine(spark, mx, my - r - 1, mx, my + r + 1);
                }
            }

            // 8) Nişan (fare üstünde, henüz atış yapılmamış düşman karesi)
            if (IsEnemy && Hover && mark == CellMark.None)
            {
                using (Pen p = new Pen(Theme.Brass, 2f))
                {
                    int L = 7;
                    int m = 3;
                    g.DrawLine(p, m, m, m + L, m);
                    g.DrawLine(p, m, m, m, m + L);
                    g.DrawLine(p, W - 1 - m, m, W - 1 - m - L, m);
                    g.DrawLine(p, W - 1 - m, m, W - 1 - m, m + L);
                    g.DrawLine(p, m, H - 1 - m, m + L, H - 1 - m);
                    g.DrawLine(p, m, H - 1 - m, m, H - 1 - m - L);
                    g.DrawLine(p, W - 1 - m, H - 1 - m, W - 1 - m - L, H - 1 - m);
                    g.DrawLine(p, W - 1 - m, H - 1 - m, W - 1 - m, H - 1 - m - L);
                }
            }
        }

        // Geminin bu hücredeki parçasını çizer. Yatayda pruva sağda, dikeyde yukarıda.
        private void DrawHull(Graphics g, int W, int H)
        {
            GraphicsState state = g.Save();
            if (vertical)
            {
                g.TranslateTransform(W / 2f, H / 2f);
                g.RotateTransform(-90f);
                g.TranslateTransform(-W / 2f, -H / 2f);
            }

            int m = 7; // gövdenin alt/üst boşluğu
            Color baseC = Tones[tone % Tones.Length];
            Color light = ControlPaint.Light(baseC, 0.3f);
            Rectangle bounds = new Rectangle(0, m, W, H - 2 * m);

            Point[] poly;
            Point[] outline;
            switch (part)
            {
                case ShipPart.Stern: // kıç
                    poly = new Point[] { new Point(5, m + 2), new Point(W, m), new Point(W, H - m), new Point(5, H - m - 2) };
                    outline = new Point[] { new Point(W, m), new Point(5, m + 2), new Point(5, H - m - 2), new Point(W, H - m) };
                    break;
                case ShipPart.Bow: // pruva (sivri uç)
                    poly = new Point[] { new Point(0, m), new Point(W - 13, m), new Point(W - 2, H / 2), new Point(W - 13, H - m), new Point(0, H - m) };
                    outline = poly;
                    break;
                default: // gövde ortası
                    poly = new Point[] { new Point(0, m), new Point(W, m), new Point(W, H - m), new Point(0, H - m) };
                    outline = null;
                    break;
            }

            using (LinearGradientBrush lg = new LinearGradientBrush(bounds, light, baseC, LinearGradientMode.Vertical))
                g.FillPolygon(lg, poly);

            using (Pen edge = new Pen(Color.FromArgb(150, 10, 14, 20), 1.2f))
            {
                if (outline != null)
                {
                    g.DrawLines(edge, outline);
                }
                else
                {
                    g.DrawLine(edge, 0, m, W, m);
                    g.DrawLine(edge, 0, H - m, W, H - m);
                }
            }

            // Lombar
            float px = part == ShipPart.Bow ? W / 2f - 4f : W / 2f;
            using (SolidBrush win = new SolidBrush(Color.FromArgb(235, Theme.Brass)))
                g.FillEllipse(win, px - 2.5f, H / 2f - 2.5f, 5f, 5f);

            g.Restore(state);
        }
    }
}
