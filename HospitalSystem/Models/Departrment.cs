using System;

namespace HospitalSystem.Models
{
    public class Department
    {
        // ثوابت نصية لضمان كتابة أسماء الأقسام بشكل صحيح في كل المشروع
        // هذه تحل محل الـ Enum وتضمن أننا نستخدم الأسماء المذكورة في الوظيفة فقط
        public const string DEPT_OPHTHALMOLOGY = "العيون";       // عيادات داخلية
        public const string DEPT_DERMATOLOGY = "الجلد";          // قسم الجلد
        public const string DEPT_EXTERNAL_DEVICES = "الأجهزة الخارجية"; // أجهزة خارجية / جراحة خارجية

        // الخصائص
        public string Name { get; set; }

        // يمكننا إضافة وصف بسيط للنوع إذا لزم الأمر، لكن الاسم يكفي للتمييز
        public string CategoryType { get; set; } // مثال: "داخلية"، "خارجية"

        // Constructor افتراضي (مطلوب للقراءة من الملفات)
        public Department() { }

        // Constructor مخصص
        public Department(string name, string categoryType = "عام")
        {
            Name = name;
            CategoryType = categoryType;
        }

        /// <summary>
        /// تحويل كائن القسم إلى سطر نصي للحفظ في ملف departments.txt
        /// الصيغة: Name|CategoryType
        /// مثال: العيون|داخلية
        /// </summary>
        public string ToFileString()
        {
            return $"{Name}|{CategoryType}";
        }

        /// <summary>
        /// إنشاء كائن قسم من سطر نصي مقروء من الملف
        /// </summary>
        public static Department FromFileString(string line)
        {
            if (string.IsNullOrWhiteSpace(line))
                throw new FormatException("سطر بيانات القسم فارغ.");

            string[] parts = line.Split('|');

            // نتوقع جزئين: الاسم والنوع
            if (parts.Length < 1)
                throw new FormatException($"تنسيق بيانات القسم غير صحيح في السطر: {line}");

            var dept = new Department
            {
                Name = parts[0],
                // إذا كان الملف قديماً ولا يحتوي على النوع، نضع قيمة افتراضية
                CategoryType = (parts.Length > 1) ? parts[1] : "عام"
            };

            return dept;
        }

        // دالة مساعدة للتحقق مما إذا كان القسم من نوع "الأجهزة الخارجية" أو "عيادات"
        // هذا سيساعدنا لاحقاً في الـ Service للتحقق من قبول المرضى الخارجيين
        public bool IsExternalDepartment()
        {
            return Name == DEPT_EXTERNAL_DEVICES || Name.Contains("خارجية");
        }

        public bool IsInternalClinic()
        {
            return Name == DEPT_OPHTHALMOLOGY || Name == DEPT_DERMATOLOGY;
        }
    }
}