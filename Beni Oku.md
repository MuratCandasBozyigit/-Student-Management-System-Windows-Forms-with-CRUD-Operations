Hocam aşagıda uygulamaya ait kendi hostumdaki veritaban'ı ilgisi ve login olurken gireceğiniz bilgiler yazıyor Teşekkür ederim şimfiden kolay gelsin sizlere.


""""""""""""""""""""
Login Bilgileri: admin / admin
SqlConnection sql = new SqlConnection("Server=217.195.207.215\\MSSQLSERVER2019;Database=dunyani1_obs;User Id=obs;Password=2013061Murat;TrustServerCertificate=True;");
""""""""""""""""""""

Öğrenci Bilgi Sistemi Projesi
Bu proje, C# ve SQL Server kullanılarak geliştirilmiş bir öğrenci bilgi sistemi uygulamasıdır. Proje, öğrenci bilgilerini, dersleri ve notları yönetmeyi amaçlar. Aşağıda, uygulamanın temel özelliklerini ve işleyişini detaylı bir şekilde bulabilirsiniz.

Giriş Ekranı
Uygulama başlatıldığında kullanıcıdan Kullanıcı Adı ve Şifre istenmektedir. Eğer yanlış giriş yapılırsa, kullanıcıya bir uyarı mesajı gösterilir. Doğru bilgilerle giriş yapıldığında ana menüye yönlendirilirsiniz.

Ana Menü
Ana menüde şu başlıklar bulunmaktadır:

Öğrenci Yönetimi
Ders Yönetimi
Not Yönetimi
Öğrenci Yönetimi
Öğrenci Ekleme: Öğrenci bilgileri eksiksiz bir şekilde girilmelidir. Eksik bilgiyle öğrenci eklenmeye çalışıldığında bir hata mesajı görüntülenir.
Öğrenci Listesi: Tüm öğrenciler listelenir ve listeyi güncellemek için "Güncelle" butonuna tıklanabilir.
Öğrenci Düzenleme ve Silme: Öğrencinin bilgilerine çift tıklayarak düzenleme yapabilir veya öğrenciyi silebilirsiniz. Ayrıca, öğrencinin cinsiyetine göre oranları görüntüleyebilirsiniz.
ID ile Öğrenci Arama: Öğrenci ID'si girilerek öğrenci bilgilerine hızlıca ulaşılabilir. Geçersiz ID girildiğinde "Bulunamadı" hatası alınır.
Ders Yönetimi
Ders Ekleme: Ders adı, saat bilgisi ve diğer ders bilgileri girilerek veri tabanına yeni bir ders eklenebilir.
Ders Silme: Ders ID'si girilerek ders silinebilir.
Ders Düzenleme: Mevcut dersler listeden seçilerek düzenlenebilir.
Ders Geçişi: Dersler arasında geçiş yapmak için yön butonları kullanılabilir.
Not Yönetimi
Not Ekleme: Seçilen öğrenciye seçilen dersten not eklenebilir. Aynı öğrenci, aynı dersi birden fazla alabilir ancak her dersten yalnızca iki not alabilir. Aynı öğrenciye aynı dersi birden fazla kez vermek mümkün değildir ve bu durumda hata mesajı gösterilir.
Not Silme: Ders notları, "Kaldır" butonuyla silinebilir.
Dersin Ortalama Notu: Her dersin ortalama notu hesaplanabilir ve görüntülenebilir.
Öğrenci Notları: Öğrencilerin aldığı tüm dersler ve notları görüntülenebilir.
Veri Tabanı Kullanımı
Bu projede SQL Server veritabanı kullanılmıştır ve ADO.NET ile veri tabanı bağlantıları yapılmıştır. Windows Forms üzerinde veri tabanı işlemleri gerçekleştirilerek, bu teknolojiyi daha verimli bir şekilde kullanmaya çalıştım.

Geliştirme Konuları ve Kullanılan Teknolojiler:
Dil: C#
Veritabanı: SQL Server
Teknoloji: ADO.NET, Windows Forms
Veritabanı İlişkileri: Öğrenciler, dersler ve notlar arasındaki ilişkiler doğru şekilde modellenmiştir.