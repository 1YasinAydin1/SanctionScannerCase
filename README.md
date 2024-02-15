# LibraryCase
Kütüphane Otomasyonu


Bu proje, .NET Core MVC kullanılarak geliştirilmiş bir kütüphane otomasyon sistemidir.

Projede Onion Architecture kullanılmıştır, bu da projenin katmanlı bir yapıya sahip olduğunu gösterir.
Sistem, kitapların eklenmesine, ödünç alınmasına ve iade edilmesine olanak tanır.

# Otomasyon Süreci
Kitap Ekleme: Kütüphaneye yeni bir kitap eklemek istediğinizde, öncelikle kullanıcı arayüzünden kitap bilgilerini gireceğiniz bir form açılır. Bu bilgiler arasında kitap adı, yazarı ve türü gibi detaylar bulunur. Formu doldurduktan sonra "Kitap Ekle" butonuna basarak kitabı sisteme eklersiniz. Eklenen kitaplar veritabanında saklanır.

Ödünç Alma: Bir kullanıcı kütüphaneden bir kitap ödünç almak istediğinde, öncelikle sistemdeki mevcut kitapları listeleyen bir sayfa açılır. Kullanıcı, istediği kitabı seçip "Ödünç Al" butonuna bastığında bir form açılır. Bu ekranda tc no, adı ve kitabı ne zaman geri getireceği girilmesi istenir. Ardından kullanıcı kitabı ödünç alır. Sistem, ödünç alınan kitabın durumunu günceller.

İade Etme: Kullanıcı, ödünç aldığı bir kitabı iade etmek istediğinde, "İade Et" butonunu kullanarak kitabı iade eder. Sistem, kitabın ödünç alma durumunu günceller ve  kullanıcının bir başka kitap ödünç alabilmesi için kitapları listeler.

# Proje Yapısı
Proje, aşağıdaki bileşenlerden oluşur:

Domain: Bu katman, iş mantığını ve domain modellerini içerir. Kitap, ödünç alma işlemi gibi temel işlemleri bu katmanda bulabilirsiniz.
Application: Uygulama katmanı, servislerin ve uygulama servislerinin bulunduğu yerdir. Ödünç alma ve iade etme işlemleri bu katmanda yönetilir.
EntityFrameworkCore: Altyapı katmanı, veritabanı bağlantıları ve veritabanı işlemleri gibi alt seviye işlemleri içerir.
.Core Mvc: Sunum katmanı, kullanıcı arayüzüyle etkileşime geçen controller'ları ve view'leri içerir.
