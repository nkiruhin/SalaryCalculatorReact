using SalaryCalculatorReact.App.DataAccessLayer;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace SalaryCalculatorReact.App
{


    public interface IDataAccessLayer<T> where T : class , IModelBase, new()

    {
        /// <summary>
        /// Возращает список записей  из БД со связями 
        /// </summary>
        /// <param name="includeProperties">выражение включения</param>
        /// <returns>Список записей</returns>
        List<T> AllIncluding(params Expression<Func<T, object>>[] includeProperties);
        Task<List<T>> AllIncludingAsync(params Expression<Func<T, object>>[] includeProperties);        

        /// <summary>
        /// Возращает все записи из БД
        /// </summary>
        /// <returns></returns>
        List<T> GetAll();
        Task<List<T>> GetAllAsync();


        /// <summary>
        /// Возращает записи замапенные в тип DTO
        /// </summary>
        /// <typeparam name="Dto">DTO тип</typeparam>
        /// <returns>Список объектов DTO типа  соответствии с настройками автомаппера</returns>
        List<Dto> GetDtoAll<Dto>();
        Task<List<Dto>> GetDtoAllAsync<Dto>();
        Task<List<Dto>> GetDtoAllAsync<Dto>(Expression<Func<T,bool>> predicate);
        /// <summary>
        /// Возращает запись замапенную в тип DTO по Id
        /// </summary>
        /// <typeparam name="Dto">DTO тип</typeparam>
        /// <param name="id">Id записи</param>
        /// <returns>Объект DTO типа в соответствии с настройками автомаппера</returns>
        Dto GetDto<Dto>(int id);
        /// <summary>
        /// Количество записей в БД 
        /// </summary>
        /// <returns></returns>
        int Count();
        /// <summary>
        /// Возращает запись из БД по Id
        /// </summary>
        /// <param name="id">Id записи</param>
        /// <param name="predicate">выражение условия (Where) </param>
        /// <param name="includeProperties">связи (Include)</param>
        /// <returns></returns>
        T GetSingle(int id);
        T GetSingleNoTracking(int id);
        Task<T> GetSingleAsync(int id);
        T GetSingle(Expression<Func<T, bool>> predicate);
        T GetSingle(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includeProperties);
        /// <summary>
        /// Поиск в БД по условию
        /// </summary>
        /// <param name="predicate">выражение условия (Where)</param>
        /// <returns></returns>
        IEnumerable<T> FindBy(Expression<Func<T, bool>> predicate);
        /// <summary>
        /// Генерация шаблона формы 
        /// </summary>
        /// <param name="id">Идентификатор</param>
        /// <param name="filterExpressions">параметры типа Expression<Func<T,bool></param>
        /// <returns>Список полей</returns>
        /// 
        List<Field> Form(int? id);
        Task<List<Field>> FormAsync(int? id);
        List<Field> Form(int? id, params object[] filterExpressions);
        Task<List<Field>> FormAsync(int? id, params object[] filterExpressions);        
        /// <summary>
        /// Добавляет запись в Entity
        /// </summary>
        /// <param name="entity"></param>
        void Add(T entity);
        Task AddAsync(T entity);
        /// <summary>
        /// Обновляет запись в Entity, не коммитит
        /// </summary>
        /// <param name="entity"></param>
        void Update(T entity);
        /// <summary>
        /// Удаляет запись из Entity, не коммитит
        /// </summary>
        /// <param name="entity"></param>
        void Delete(T entity);
        /// <summary>
        /// Удаляет записи из Entity по условию, не коммитит
        /// </summary>
        /// <param name="predicate">условие (Where)</param>
        void DeleteWhere(Expression<Func<T, bool>> predicate);

        void Commit();

        Task CommitAsync();


    }

    /// <summary>
    /// Класс - поля формы
    /// </summary>
    public class Field
    {
        /// <summary>
        /// Имя
        /// </summary>
        public string Name { set; get; }
        /// <summary>
        /// Тип поля text select data и т.д.
        /// </summary>
        public string Type { set; get; }
        /// <summary>
        /// Имя отобращаемое на форме в лейбл
        /// </summary>
        public string DisplayName { set; get; }
        
        public bool IsRequired { set; get; }
        public string Value { set; get; }
        /// <summary>
        /// Список значений для select
        /// </summary>
        public List<Item> Items { set; get; }
        /// <summary>
        /// Флаг динамической подгрузки Items с сервера 
        /// </summary>
        public bool ServerSide { set; get; } = false;       
        public class Item
        {
            public int Key { set; get; }
            public string Text { set; get; }
        }
    }
}


