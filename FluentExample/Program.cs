using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace FluentExample
{
    class Program
    {
        static void Main(string[] args)
        {
            var ironTank = FluentFactory<Tank>
                .Instance(new Tank())
                .GiveValue(t => t.Range, 400.23M)
                .GiveValue(t => t.Side, "Me")
                .GiveValue(t => t.Type, "T48")
                .Take();




        }
    }


    // Fluent uygulamak için generic olarak farklı sınıflar kullanılarak çalışmak için 
    interface IFactory<T>
    {
        //IFactory<T> GiveValue(string PropertyName, object Value);
        IFactory<T> GiveValue(Expression<Func<T, object>>Property,object Value);
        T Take(); // New'lenmiş olarak generic nesne döndüren method imzası
    }

    
    class Factory<T> : IFactory<T>
    {
        public Factory(T Instance)
        {
            _instance = Instance;
        }

        T _instance;

        //public IFactory<T> GiveValue(string PropertyName, object Value)
        //{
        //    var pInfo = _instance.GetType().GetProperty(PropertyName);
        //    if (pInfo != null)
        //    {
        //        pInfo.SetValue(_instance, Value);
        //    }

        //    return this;
        //}

        public IFactory<T> GiveValue(Expression<Func<T, object>> Property, object Value)
        {
            PropertyInfo pInfo = null;

            if (Property.Body is MemberExpression)
            {
                pInfo = (Property.Body as MemberExpression).Member as PropertyInfo;
            }
            else
            {
                pInfo = ((Property.Body as UnaryExpression).Operand as MemberExpression).Member as PropertyInfo; 
            }

            pInfo.SetValue(_instance, Value);
            return this;
        }

        public T Take()
        {
            return _instance;
        }
    }

    // Fluent sınıfının generic olarak oluşturulması için kulllanılacak sınıf
    static class FluentFactory<T>
        where T : class, new()
    {
        public static IFactory<T> Instance(T Instance)
        {
            return new Factory<T>(Instance);
        }
    }


    // Classes
    class Player
    {
        public string NickName { get; set; }
        public int LastLavel { get; set; }
    }

    class Tank
    {
        public string Type { get; set; }
        public decimal Range { get; set; }
        public string Side { get; set; }
    }

}
