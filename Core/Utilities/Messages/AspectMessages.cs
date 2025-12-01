namespace Core.Utilities.Messages
{
    public static class AspectMessages
    {
        public static string WrongValidationType => "Wrong validation type.";
        public static string WrongLoggerType     => "Wrong Logger Type";


        // Dosya (File) İşlemleri
        public static string FileUploaded => "Dosya başarıyla yüklendi.";
        public static string FileUploadFailed => "Dosya yükleme başarısız oldu.";
        public static string FileDeleted => "Dosya başarıyla silindi.";
        public static string FileDeleteFailed => "Dosya silme başarısız oldu.";
        public static string FileUpdateFailed => "Dosya güncelleme başarısız oldu (Eski dosya silinirken veya yeni dosya yüklenirken hata oluştu).";
        public static string FileNotFound => "Belirtilen dosya bulunamadı.";
        public static string FileEmpty => "Dosya boş veya seçilmemiş.";



    }
}
