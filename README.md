# نظام إدارة المستشفى - C# .NET 8

نظام شامل لإدارة المستشفى مبني بلغة C# على منصة .NET 8، يعتمد على واجهة سطر أوامر (Console) ويستخدم مفاهيم البرمجة الكائنية التوجه (OOP) مع نمط المستودع (Repository Pattern) وحقن التبعيات (Dependency Injection).

## الميزات الرئيسية

### إدارة الأطباء
- تسجيل أطباء جدد (مقيمين، متعاقدين، هيئة تدريس)
- عرض جميع الأطباء
- حذف الأطباء
- حساب عدد الأطباء المقيمين
- حساب حصة الطبيب من تكاليف العلاج بناءً على نوعه وخبرته

### إدارة المرضى
- تسجيل مرضى جدد (داخليين / خارجيين)
- عرض جميع المرضى
- إدخال مريض في قسم معين
- خروج المريض (للمرضى الداخليين فقط)

### إدارة العلاجات
- تسجيل علاجات مع تفاصيل المريض والطبيب والتكلفة
- تتبع سجل العلاجات

### التقارير والإحصائيات
- تقارير الأطباء مع سجل علاجاتهم
- تقارير المرضى مع سجل علاجاتهم
- إحصائيات الأقسام ضمن فترة زمنية محددة
- علاجات المريض ضمن فترة زمنية محددة

## بنية المشروع (Architecture)

- **النماذج (Models)**: فئات Doctor، Patient، Treatment، Department مع الوراثة
- **المستودعات (Repositories)**: حفظ البيانات في ملفات نصية (doctors.txt، patients.txt، treatments.txt)
- **الخدمات (Services)**: المنطق العمليسي للأطباء والمرضى والعلاجات والتقارير
- **الواجهات (Interfaces)**: فصل واضح للمسؤوليات مثل IRepository، IDoctorService
- **حقن التبعيات (DI)**: إعداد يدوي في Program.cs

## مفاهيم OOP المستخدمة

- **الوراثة (Inheritance)**:
  - Doctor → ResidentDoctor، ContractorDoctor، FacultyDoctor
  - Patient → Inpatient، OutPatient
- **التعددية (Polymorphism)**: دوال مجردة مثل CalculateShare و CanBeDischarged
- **التجريد (Abstraction)**: واجهات للخدمات والمستودعات

## المتطلبات

- .NET 8 SDK

## كيفية التشغيل

```bash
cd HospitalSystem
dotnet run
```

## هيكل المشروع

```
HospitalSystem/
├── Models/
│   ├── Doctors/          # Doctor, ResidentDoctor, ContractorDoctor, FacultyDoctor
│   ├── Patient/          # Patient, Inpatient, OutPatient
│   ├── Treatment.cs
│   └── Department.cs
├── Repositories/
│   ├── DoctorRepository.cs
│   ├── PatientRepositories.cs
│   └── TreatmentRepository.cs
├── Services/
│   ├── DoctorService.cs
│   ├── PatientService.cs
│   ├── TreatmentService.cs
│   └── ReportService.cs
├── Interfaces/
├── Exceptions/
├── Helper/
├── Program.cs
└── HospitalSystem.csproj
```

## حفظ البيانات

يستخدم النظام ملفات نصية لتخزين البيانات:
- `doctors.txt` - سجلات الأطباء
- `patients.txt` - سجلات المرضى
- `treatments.txt` - سجلات العلاجات

## الرخصة

هذا المشروع للأغراض التعليمية.