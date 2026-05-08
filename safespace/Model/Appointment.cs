namespace safespace.Model
{
    public class Appointment
    {
        public int Id { get; set; }
        public int PatientId { get; set; }           // معرف المريض
        public PatientProfile PatientProfile { get; set; }                // العلاقة مع المريض
        public int DoctorProfileId { get; set; }         // معرف الطبيب
        public DoctorProfile Doctor { get; set; }        // العلاقة مع الطبيب
        public DateTime Date { get; set; }                // التاريخ (بدون وقت)
        public TimeSpan StartTime { get; set; }           // وقت البدء
        public TimeSpan EndTime { get; set; }             // وقت الانتهاء (ساعة افتراضياً)
        public AppointmentStatus Status { get; set; }     // الحالة (محجوز، مكتمل، ملغي)
        public DateTime CreatedAt { get; set; }           // تاريخ الإنشاء
        public string Notes { get; set; }                  // ملاحظات (اختياري)
    }

    public enum AppointmentStatus
    {
        Scheduled,   // محجوز
        Completed,   // تمت الجلسة
        Cancelled,   // ملغي
        NoShow       // لم يحضر
    }
}
