namespace Business.Constants
{
    public static class Messages
    {
        // User mesajları
        public static string UserRegistered = "Kullanıcı kaydedildi";
        public static string SuccessfulLogin = "Login Başarılı";
        public static string PasswordError = "Parola Hatası";
        public static string UserNotFound = "Kullanıcı Bulunamadı";
        public static string UserDeleted = "Kullanıcı Silindi";
        public static string UserNameExists = "Bu kullanıcı adı kullanılmaktadır.";
        public static string EmailOnUse = "Bu email kullanılmaktadır.";
        public static string UserSuspended = "Bu Kullanıcının hesabı askıya alınmıştır .";
        public static string CouldNotCreateUser = "Kullanıcı oluşturulamadı çünkü E-Posta veya Kulllanıcı Adı Başkası tarafından kullanılmakta .";
        public static string PasswordUpdated = "Şifreniz değiştirldi .";
        public static string OldPasswordIsWrong = "Mevcut şifrenizi yanlış girdiniz.";
        public static string UnAuthorizedDeleteAttempt = "Bu  hesabı silmeye yetkiniz yok";
        public static string UserSuspended2 = " Değerli kullanıcı. Şifrenizi 3 kere hatalı girdiğinizden dolayı Hesabınız 10 dk süreyle askıya alınmıştır .";
        public static string RegistrationFailed = "Kayıt oluşturma başarısız.";
        public static string ThisEmailIsNotAvailable = "Bu email adresi başka bir kullanıcıya aittir";
        public static string UserNotFoundWithMail = "Bu  emaille bir hesap bulunamadı";
        public static string UserNotFoundWithUserName = "Bu kullanıcı adıyla bir kullanıcı bulunamadı";
        public static string CouldNotFindUser = "Bu kullanıcı adına  kayıtlı kullanıcı bulunamadı";
        public static string CouldNotFindUser2 = "Bu emaile kayıtlı kulllanıcı bulunamadı";
        // Genel mesajlar (kısa)
        public static string AuthorizationDenied = "Yetkiniz yok.";
        public static string AccessTokenCreated = "AccessToken oluşturuldu";
        public static string DepartmentAdded = "Department eklendi.";
        public static string DepartmentUpdated = "Department güncellendi.";
        public static string DepartmentDeleted = "Department silindi.";
        public static string DepartmentNotFound = "Department bulunamadı.";
        public static string DepartmentListed = "Department listelendi.";

        public static string EmployeeAdded = "Employee eklendi.";
        public static string EmployeeUpdated = "Employee güncellendi.";
        public static string EmployeeDeleted = "Employee silindi.";
        public static string EmployeeNotFound = "Employee bulunamadı.";
        public static string EmployeeListed = "Employee listelendi.";

        public static string TitleAdded = "Title eklendi.";
        public static string TitleUpdated = "Title güncellendi.";
        public static string TitleDeleted = "Title silindi.";
        public static string TitleNotFound = "Title bulunamadı.";
        public static string TitleListed = "Title listelendi.";

        public static string EmployeeImageUploaded => "Çalışan fotoğrafı başarıyla yüklendi ve güncellendi.";
        public static string EmployeeImageUploadFailed => "Çalışan fotoğrafı yüklenirken bir hata oluştu.";
    }
}
