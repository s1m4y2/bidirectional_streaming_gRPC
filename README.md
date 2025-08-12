# 📦 Distributed CarSync System with gRPC Bidirectional Streaming

Bu proje, araçların durumlarını gerçek zamanlı olarak **birbirleriyle senkronize eden** dağıtık bir sistem örneğidir.  
gRPC’nin **Bidirectional Streaming** özelliği kullanılarak araçlar (node’lar) arasında çift yönlü veri akışı sağlanır.

---

## 🚀 Proje Hakkında

Her araç (car node), kendi durum bilgisini (örneğin "Running", "Maintenance", "Idle") diğer araçlara gönderir ve karşıdan gelen güncellemeleri alarak kendi veritabanını senkronize eder.  
Bu sayede araçlar arası durum bilgisi daima güncel ve tutarlı kalır.

---

## ✨ Öne Çıkan Özellikler

- **gRPC Bidirectional Streaming** ile gerçek zamanlı çift yönlü iletişim  
- Her node, diğer node’lara sürekli durum güncellemeleri gönderir  
- Durum değişiklikleri anlık olarak veritabanına yansıtılır  
- **Entity Framework Core** ile MSSQL üzerinde kalıcı veri yönetimi  
- Modern .NET Minimal API yapısı ile kolay genişletilebilirlik  
- Node adreslerinin konfigürasyonla kolay yönetimi  
- Kaynakları verimli kullanmak için asenkron programlama ve stream iptal takibi  

---

## 📂 Proje Klasör Yapısı

```plaintext
📦 CarSyncProject
┣ 📜 car_sync.proto            # gRPC servis ve mesaj tanımları
┣ 📂 Common                   # Ortak model ve veri erişim katmanı
┃ ┣ 📜 Car.cs                 # Araç veri modeli
┃ ┣ 📜 AppDbContext.cs        # EF Core DbContext
┣ 📂 CarNode1                 # Birinci araç node'u (örnek)
┃ ┣ 📜 Program.cs             # Uygulama başlangıcı ve yapılandırma
┃ ┣ 📜 CarSyncService.cs      # gRPC servis implementasyonu
┃ ┣ 📜 UpdateBroadcaster.cs   # Diğer node’lara bağlantı ve güncelleme yayma
┣ 📂 CarNode2                 # Diğer node’lar için benzer yapı
┣ 📂 CarNode3
```

---

## 📜 Teknolojiler ve Araçlar

- .NET 8 ve C#
- gRPC Bidirectional Streaming
- Entity Framework Core + MSSQL
- Minimal API ve Dependency Injection
- Asenkron programlama ve streaming API’leri

---

## 🔄 Çalışma Mantığı
1. Her node kendi araç durumunu DB’den okuyup diğer node’lara stream olarak gönderir.
2. Gelen güncellemeler kendi DB’sinde anında güncellenir.
3. Güncellemeler sürekli ve çift yönlü olarak akar.
4. Node’lar birbirlerinden bağımsız çalışır, ancak verileri senkron tutarlar.

---

## 🚀 Kurulum ve Çalıştırma
1. MSSQL bağlantısını appsettings.json içerisinde yapılandırın.
2. Veritabanını EF Core Migrations ile oluşturun ve başlangıç verilerini yükleyin.
3. Her node için projeyi ayrı ayrı çalıştırın (örneğin CarNode1, CarNode2, CarNode3).
4. Node’lar kendi durumlarını belirli aralıklarla diğer node’lara yayınlar.
5. Güncellemeler konsol ekranına ve veritabanına kaydedilir.

---

## 🎯 Projenin Faydaları
- Dağıtık sistemlerde gerçek zamanlı veri senkronizasyonu örneği
- gRPC Bidirectional Streaming uygulaması pratik örneği
- Gerçek dünya benzeri senaryoda Entity Framework ve asenkron iletişim kullanımı
- Microservices mimarisine uygun yapı

---

Dağıtık ve gerçek zamanlı araç senkronizasyonunu keşfetmek için ideal!

---

# 📦 GrpcChatApp

Gerçek zamanlı, çift yönlü (bidirectional) **gRPC Chat Uygulaması**.  
Birden fazla kullanıcının anlık mesajlaşabildiği, hafif ve güvenilir bir sohbet sistemi örneğidir.

---

## 📂 Proje Klasör Yapısı

```plaintext
📦 GrpcChatApp
┣ 📜 chat.proto                # gRPC servis ve mesaj tanımları
┣ 📂 Server
┃ ┣ 📜 ChatService.cs          # Çoklu client’a mesaj yayabilen chat servisi
┃ ┣ 📜 Program.cs              # gRPC server yapılandırması ve başlatılması
┣ 📂 Client
┃ ┣ 📜 Program.cs              # Kullanıcı adı alıp chat mesajlarını gönderen ve alan istemci
```

---

## 📂 Server
📜 ChatService.cs
- Bağlanan tüm client’ların stream nesnelerini thread-safe şekilde tutar.
- Client’tan gelen mesajları dinler, konsola yazar ve diğer tüm client’lara iletir.
- Mesaj gönderen client’a tekrar mesaj göndermez (loop önleme).
- Bağlantı kesildiğinde client listesinden çıkarır.
📜 Program.cs
- gRPC servisini Kestrel üzerinden hem HTTP hem HTTPS portlarında çalıştırır.
- ChatService’i kaydeder ve minimal API ile durum endpoint’i sunar.

## 📂 Client
📜 Program.cs
- Kullanıcıdan ismini alır ve sunucuya bağlanır.
- Kullanıcı mesajlarını gRPC stream üzerinden gönderir.
- Sunucudan gelen tüm mesajları asenkron olarak dinler ve ekrana yazdırır.
- Kullanıcı “bye” yazdığında sohbeti sonlandırır.

---

## 🚀 Çalıştırma
1️⃣ Server’ı başlat:
```plaintext
cd Server
dotnet run
```
2️⃣ Client’i başlat:
```plaintext
cd Client
dotnet run
```
3️⃣ İsminizi girin ve sohbete başlayın:
```plaintext
İsminizi girin: Simay
```
4️⃣ Mesaj gönderip alın:
```plaintext
[Mehmet]: Merhaba!
[Simay]: Selam Mehmet!
...
```

---

## 🎯 Özet
- gRPC Bidirectional Streaming ile çift yönlü gerçek zamanlı sohbet.
- Çoklu client yönetimi ve mesaj yayılımı.
- Thread-safe client stream yönetimi.
- HTTPS ile güvenli bağlantı (isteğe bağlı).
- Basit ve kolay genişletilebilir mimari.

