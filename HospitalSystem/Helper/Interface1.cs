using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HospitalSystem.Helper
{
    public interface IRepository<T> where T : class
    {
        List<T> GetAll();
        T GetById(int id); // قد نحتاج لتعديل هذا لاحقاً لأن الـ ID قد لا يكون كافياً مع الوراثة بدون بحث
        void Add(T item);
        void Update(T item);
        void Delete(int id);
        void SaveChanges(); // كتابة القائمة كاملة إلى الملف
        void LoadData();    // قراءة البيانات من الملف عند البدء
    }
}
