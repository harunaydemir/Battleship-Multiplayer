# ⚓ Amiral Battı: Komuta Merkezi

C# ve Windows Forms ile geliştirilmiş, **bilgisayara karşı** ya da **ağ üzerinden bir arkadaşınla** oynayabileceğin Amiral Battı (Battleship) oyunu. Radar ve torpido gibi özel silahları, sonar süpürmeli düşman suları ve "komuta masası" temasıyla klasik oyunu biraz daha taktiksel hale getirir.

![C#](https://img.shields.io/badge/C%23-Windows%20Forms-239120)
![.NET Framework](https://img.shields.io/badge/.NET%20Framework-4.7.2-512BD4)
![Platform](https://img.shields.io/badge/Platform-Windows-0078D6)

---

## İçindekiler

- [Ekran görüntüleri](#ekran-görüntüleri)
- [Özellikler](#özellikler)
- [Oyun kuralları](#oyun-kuralları)
- [Kurulum ve çalıştırma](#kurulum-ve-çalıştırma)
- [Nasıl oynanır](#nasıl-oynanır)
- [Online oyun (iki kişilik)](#online-oyun-iki-kişilik)
- [Bilgisayar rakip nasıl düşünür](#bilgisayar-rakip-nasıl-düşünür)
- [Proje yapısı](#proje-yapısı)
- [Ağ protokolü](#ağ-protokolü)
- [Bilinen sınırlamalar ve fikirler](#bilinen-sınırlamalar-ve-fikirler)

---
## Ekran görüntüleri

<table>
  <tr>
    <td width="35.4%"><a href="https://github.com/user-attachments/assets/b8a0ca0b-2925-4261-ab20-677e6a848fcc"><img src="https://github.com/user-attachments/assets/b8a0ca0b-2925-4261-ab20-677e6a848fcc" width="100%" alt="Sunucu penceresi"></a></td>
    <td width="35.1%"><a href="https://github.com/user-attachments/assets/ebb2b223-fa39-49de-a459-c7ad2f42ad2a"><img src="https://github.com/user-attachments/assets/ebb2b223-fa39-49de-a459-c7ad2f42ad2a" width="100%" alt="İstemci penceresi"></a></td>
    <td width="29.6%"><a href="https://github.com/user-attachments/assets/862763ca-ece1-43d2-8e2c-58f2606ff269"><img src="https://github.com/user-attachments/assets/862763ca-ece1-43d2-8e2c-58f2606ff269" width="100%" alt="Oyun sonu"></a></td>
  </tr>
  <tr>
    <td align="center"><sub><b>Sunucu</b><br>radar sonucu ve telsiz</sub></td>
    <td align="center"><sub><b>İstemci</b><br>aynı oyunun karşı tarafı</sub></td>
    <td align="center"><sub><b>Oyun sonu</b><br>kazanan istemci</sub></td>
  </tr>
</table>

<sub>Görüntülerin üzerine tıklayarak tam boyutta açabilirsin. Üçü de aynı online oyundan alınmıştır: soldaki iki pencere sunucu ve istemcinin karşılıklı görünümü, sağdaki oyunun bittiği an.</sub>

---

## Özellikler

| | |
|---|---|
| 🤖 **Bilgisayara karşı mod** | Oyun açılır açılmaz oynamaya başlarsın. |
| 🌐 **Ağ üzerinden iki kişilik mod** | TCP ile sunucu/istemci bağlantısı, sırayla atış ve telsiz (sohbet). |
| 🚢 **Gerçek gemi görünümü** | Sivri pruvalı, lombarlı gövdeler. Her geminin ayrı rengi var, yatay ya da dikey durabilir. |
| 📡 **Radar** | 4×4'lük alanda kaç gemi parçası olduğunu söyler (3 hak). |
| 💣 **Torpido** | 3×3'lük alana aynı anda vurur (2 hak). |
| 🎯 **Alan önizlemesi** | Radar ya da torpido seçiliyken fareyi gezdirince etkilenecek alan işaretlenir. |
| 🧭 **Komuta masası teması** | Grafit zemin, pirinç ve sonar camgöbeği vurgular, dönen sonar süpürmesi, nabız gibi atan patlamalar. |
| 📊 **Canlı durum** | Üstte "SIRA SENDE / RAKİP ATIYOR / KAZANDIN", her iki filo için kalan parça sayacı. |
| 📝 **Renkli seyir defteri** | Oyun olayları renk koduyla yazılır. Koordinatlar `A1`–`J10` biçimindedir. |

---

## Oyun kuralları

**Filo.** Her oyuncunun 5 gemisi vardır. Gemiler rastgele yerleştirilir, birbirine **değmez** (çaprazlar dahil) ve toplam **17 kare** kaplar:

| Gemi | Uzunluk | Adet |
|---|:---:|:---:|
| Uçak gemisi | 5 | 1 |
| Muharebe gemisi | 4 | 1 |
| Kruvazör / Denizaltı | 3 | 2 |
| Muhrip | 2 | 1 |

**Atış.** Sırası gelen oyuncu rakibin haritasında bir kareye atış yapar. İsabet ya da karavana olsun, atıştan sonra **sıra rakibe geçer**.

**Özel silahlar.** Sadece **kendi sırandayken** kullanılır ve kullanınca sıra rakibe geçer:

- **Radar (3 hak):** Seçtiğin kare, 4×4'lük alanın *sol üst köşesi* olur. Alanda henüz vurulmamış kaç gemi parçası olduğunu öğrenirsin. Alan haritanın dışına taşarsa taşan kısım sayılmaz.
- **Torpido (2 hak):** Seçtiğin kare, 3×3'lük alanın *sol üst köşesi* olur. Alandaki daha önce atış yapılmamış tüm karelere aynı anda vurur.
- Düğmeye ikinci kez basarsan seçim **iptal olur ve hakkın harcanmaz**. Hak, haritada hedef seçince düşer.

**Kazanmak.** Rakibin 17 gemi karesinin hepsini vuran oyunu kazanır.

---

## Kurulum ve çalıştırma

### Gereksinimler

- Windows 10 / 11
- Visual Studio 2019 ya da 2022 ("**.NET masaüstü geliştirme**" iş yükü yüklü)
- .NET Framework 4.7.2

### Adımlar

1. Projeyi indir ya da klonla.
2. `BattleshipProject.sln` dosyasını Visual Studio ile aç.
3. `Ctrl + Shift + B` ile derle.
4. `F5` ile çalıştır.

Derlenmiş uygulama `bin\Debug\BattleshipProject.exe` içindedir.


---

## Nasıl oynanır

1. Oyun açılınca sol tarafta **kendi filonu**, ortada **düşman sularını** görürsün.
2. Düşman haritasında bir kareye tıkla. Fareyi gezdirince pirinç renkli nişan çıkar.
3. İsabet ettiysen patlama, ıskaladıysan beyaz iğne görürsün. Ardından bilgisayar atış yapar.
4. Radar ya da torpido için **Özel Silahlar** kartındaki düğmeye bas, sonra haritada hedef karesini seç.
5. Her şey **Seyir Defteri**'ne yazılır. Üst şeritte sıranın kimde olduğunu görürsün.

### Harita göstergeleri

| Görünüm | Anlamı |
|---|---|
| Beyaz iğne ve halka | Karavana, boşa atış |
| Turuncu patlama | İsabet |
| Açık mavi ton (düşman suları) | Radarın taradığı bölge |
| Pirinç rengi nişan | Fare üstündeki kare |
| Pirinç renkli geniş vurgu | Radar/torpido önizleme alanı |

---

## Online oyun (iki kişilik)

Bir kişi **sunucu**, diğeri **istemci** olur.

**Sunucu tarafı**
1. **Bağlantı** kartında "Sunucu olarak oyna"yı seç.
2. Bir port yaz (varsayılan `5000`) ve **BAĞLAN**'a bas. Rakip bağlanana kadar bekler.

**İstemci tarafı**
1. "İstemci olarak oyna"yı seç.
2. Sunucunun **IP adresini** ve aynı **port numarasını** yaz, **BAĞLAN**'a bas.

**Notlar**
- İlk atışı **sunucu** yapar.
- Aynı bilgisayarda denemek için iki kez çalıştır ve IP olarak `127.0.0.1` kullan.
- İki ayrı bilgisayarda oynarken sunucunun IP adresini Komut İstemi'nde `ipconfig` yazıp "IPv4 Adresi" satırından öğrenebilirsin.
- Windows Güvenlik Duvarı sorarsa **erişime izin ver**. Aynı yerel ağda (Wi-Fi) olmanız gerekir. İnternet üzerinden oynamak için sunucu tarafında port yönlendirme gerekir.
- **Telsiz** kutusundan rakibinle mesajlaşabilirsin (Enter ile de gönderilir).
- Oyun bitince aynı pencereden tekrar **BAĞLAN**'a basarak yeni bir online oyun başlatabilirsin.

---

## Bilgisayar rakip nasıl düşünür

Bilgisayar senin gemilerinin yerini **bilmez**. Yalnızca kendi önceki atışlarının sonucuna bakar ve iki modda çalışır:

1. **Avlanma modu:** Henüz atılmamış kareler arasından **dama tahtası düzenine** uyanlardan rastgele birini seçer. En küçük gemi 2 kare olduğu için bu düzen her gemiye mutlaka denk gelir ve tahtanın yarısını boşuna denemez.
2. **Hedefleme modu:** Bir gemiye isabet edince o karenin **yukarı, aşağı, sol ve sağındaki** karelere sırayla atış yapar. Liste bitince yeniden avlanma moduna döner.

Bilgisayar radar ve torpido kullanmaz.

---

## Proje yapısı

```
BattleshipProject/
├── Program.cs            Uygulamanın giriş noktası
├── Form1.cs              Oyun mantığı, ağ iletişimi, bilgisayar rakip, arayüz kurulumu
├── Form1.Designer.cs     Tasarımcı tarafından üretilen kontroller (elle düzenleme)
├── Theme.cs              Tema ve özel çizim sınıfları
└── BattleshipProject.csproj
```

**`Theme.cs` içindeki sınıflar**

| Sınıf | Görevi |
|---|---|
| `Theme` | Renk paleti, yazı tipleri, yuvarlak köşe yardımcısı |
| `Card` | Başlıklı, yuvarlak köşeli kart paneli + kalan parça sayacı |
| `HeaderBar` | Pusula, başlık, mod ve durum etiketi |
| `BoardCell` | Haritadaki tek kare: su, gemi gövdesi, isabet, karavana, nişan, sonar |

**`Form1.cs` ana bölümleri:** arayüz kurulumu → gemi yerleştirme → oyuncu atışı → bilgisayar rakip → oyun sonu → ağ (bağlantı, mesaj gönderme/alma) → sohbet.

Tasarımcıdaki kontroller (`pnlPlayer`, `btnRadar` vb.) aynen kullanılır, tema çalışma anında kodla uygulanır. Bu yüzden Visual Studio'nun tasarım ekranında form sade görünür, oyun açılınca asıl hali çıkar.

---

## Ağ protokolü

İki taraf TCP üzerinden, satır sonuyla biten UTF-8 metin mesajları gönderir. Koordinatlar `satır_sütun` biçiminde, 0'dan başlar.

| Mesaj | Yön | Anlamı |
|---|---|---|
| `ATIS_r_c` | atan → alan | Normal atış |
| `TORPIDO_r_c` | atan → alan | 3×3 torpido (sol üst köşe) |
| `RADAR_r_c` | atan → alan | 4×4 radar (sol üst köşe) |
| `SONUC_r_c_ISABET` / `SONUC_r_c_KARAVANA` | alan → atan | Bir karenin sonucu |
| `RADARSONUC_n` | alan → atan | Radar alanındaki gemi parçası sayısı |
| `CHAT\|metin` | ikisi de | Sohbet mesajı |

Sohbet mesajları `CHAT|` önekiyle ayrıldığı için oyun komutlarıyla karışmaz. Bozuk ya da tanınmayan mesajlar yok sayılır, oyunu çökertmez.

**Sıra mantığı:** Atan taraf mesajı gönderince sırayı bırakır. Alan taraf mesajı işleyip sonuçları gönderdikten sonra sırayı alır. Oyun bitmişse sıra değişmez.

---

## Bilinen sınırlamalar ve fikirler

Şu an olmayanlar, ileride eklenebilir:

- [ ] **Yeni Oyun butonu** (bilgisayara karşı oyunda baştan başlamak için şimdilik uygulamayı yeniden açmak gerekir)
- [ ] Gemi battığında bildirim ve batan geminin haritada belirmesi
- [ ] Oyun başında gemileri elle yerleştirme (sürükle ve döndür)
- [ ] Daha akıllı bilgisayar rakip (gemi yönünü çıkarma, batan gemiyi tanıma) ve kolay/orta/zor seçimi
- [ ] Bilgisayarın radar ve torpido kullanması
- [ ] Online modda yeniden eşleşme (rövanş) butonu

---
