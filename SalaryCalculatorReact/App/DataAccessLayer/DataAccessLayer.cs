using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using SalaryCalculatorReact.App.DataAccessLayer;
using SalaryCalculatorReact.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;

namespace SalaryCalculatorReact.App
{
    
    public class DataAccessLayer<T>:IDataAccessLayer<T> where T : class , IModelBase,  new(){
        private Context _context;
        private IModelMetadataProvider _provider;
        private readonly IMapper _mapper;
        public DataAccessLayer(Context context, IModelMetadataProvider provider,IMapper mapper)
        {
            _context = context;
            _provider = provider;
            _mapper = mapper;
        }
        private List<Field.Item> GetSelectList<M>(object expression) where M : class, IModelBase
        {
            if(expression==null) return _context.Set<M>().Select(n => new Field.Item { Key = n.Id, Text = n.Name }).ToList();
            var exp = expression as Expression<Func<M, bool>>;
            return _context.Set<M>().Where(exp).Select(n => new Field.Item { Key = n.Id, Text = n.Name }).ToList();
        }
        private List<Field> GetMetadata(T data)
        {
            var metadata = _provider.GetMetadataForType(typeof(T));
            List<Field> Fields = new List<Field>();
            foreach (var prop in metadata.Properties.Where(pm => pm.ShowForDisplay))
            {
                Type type = prop.ModelType;
                var field = new Field
                {

                    DisplayName = prop.DisplayName,
                    IsRequired = prop.IsRequired,

                };
                if (prop.TemplateHint == "ServerSide")
                {
                    field.ServerSide = true;
                }
                if (prop.IsComplexType)
                {
                    field.Name = prop.PropertyName + "Id";
                    field.Value = data?.GetType().GetProperty(field.Name).GetValue(data)?.ToString();
                    field.IsRequired = metadata.Properties.SingleOrDefault(pm => pm.PropertyName == field.Name).IsRequired;
                    field.Type = "select";
                    object[] parameters = { null };
                    if (!field.ServerSide)
                    {
                        field.Items = typeof(DataAccessLayer<T>).GetMethod(nameof(GetSelectList), BindingFlags.NonPublic | BindingFlags.Instance)
                            .MakeGenericMethod(prop.ModelType)
                            .Invoke(obj: this, parameters: parameters) as List<Field.Item>;
                    }
                }
                else
                {
                    field.Name = prop.PropertyName;
                    field.Type = prop.ModelType.Name;
                    object value = data?.GetType().GetProperty(prop.PropertyName).GetValue(data);
                    if (value is DateTime valueDatetime)
                    {
                        field.Value = valueDatetime.ToString("o");
                    }
                    else
                    {
                        if(field.Type== "Boolean" && value == null)
                        {
                            value = false;
                        }
                        field.Value = value?.ToString();
                    }
                }
                Fields.Add(field);
            }
            return Fields;
        }
        public virtual List<T> GetAll() => _context.Set<T>().AsNoTracking().ToList();
        public virtual async Task<List<T>> GetAllAsync() => await _context.Set<T>().AsNoTracking().ToListAsync();
        public virtual List<Dto> GetDtoAll<Dto>() => _context.Set<T>().AsNoTracking().ProjectTo<Dto>(_mapper.ConfigurationProvider).ToList();
        public virtual Dto GetDto<Dto>(int id) => _context.Set<T>().AsNoTracking().Where(n => n.Id == id).ProjectTo<Dto>(_mapper.ConfigurationProvider).SingleOrDefault();
        public virtual async Task<List<Dto>> GetDtoAllAsync<Dto>() => await _context.Set<T>().AsNoTracking().ProjectTo<Dto>(_mapper.ConfigurationProvider).ToListAsync();
        public virtual async Task<List<Dto>> GetDtoAllAsync<Dto>(Expression<Func<T, bool>> predicate) => await _context.Set<T>().AsNoTracking().Where(predicate).ProjectTo<Dto>(_mapper.ConfigurationProvider).ToListAsync();
        public virtual int Count() => _context.Set<T>().Count();
        public virtual List<T> AllIncluding(params Expression<Func<T, object>>[] includeProperties)
        {
            IQueryable<T> query = _context.Set<T>();
            foreach (var includeProperty in includeProperties)
            {
                query = query.AsNoTracking().Include(includeProperty);
            }
            return query.ToList();
        }
        public virtual async Task<List<T>> AllIncludingAsync(params Expression<Func<T, object>>[] includeProperties)
        {
            IQueryable<T> query = _context.Set<T>();
            foreach (var includeProperty in includeProperties)
            {
                query = query.AsNoTracking().Include(includeProperty);
            }
            return await query.ToListAsync();
        }
        public virtual List<Field> Form(int? id)
        {
            T data = null;
            if (id != null)
            {
                data = GetSingle((int)id);
            }
            return GetMetadata(data);
        }
        public virtual async Task<List<Field>> FormAsync(int? id)
        {
            T data = null;
            if (id != null)
            {
                data = await GetSingleAsync((int)id);
            }
            return GetMetadata(data);
        }
        public virtual List<Field> Form(int? id, params object[] filterExpressions)
        {
            T data = null;
            if (id != null)
            {
                data = GetSingle((int)id);
            }
            var metadata = _provider.GetMetadataForType(typeof(T));
            List<Field> Fields = new List<Field>();
            foreach (var prop in metadata.Properties.Where(pm => pm.ShowForDisplay))
            {
                Type type = prop.ModelType;
                var field = new Field
                {
                    
                    DisplayName = prop.DisplayName,
                    IsRequired = prop.IsRequired,
                    
                };
                if (prop.TemplateHint == "ServerSide")
                {
                    field.ServerSide = true;
                }
                if (prop.IsComplexType)
                {
                    field.Name = prop.PropertyName + "Id";
                    field.Value = data?.GetType().GetProperty(field.Name).GetValue(data)?.ToString();
                    field.IsRequired = metadata.Properties.SingleOrDefault(pm => pm.PropertyName == field.Name).IsRequired;
                    field.Type = "select";
                    object[] parameters = { null } ;
                    if (!field.ServerSide)
                    {
                        foreach(var exp in filterExpressions)
                        {
                            Type expGenericType;
                            try
                            {
                                 expGenericType = exp.GetType().GetGenericArguments()[0].GenericTypeArguments[0];
                            }
                            catch (Exception e)
                            {
                                System.Diagnostics.Debug.WriteLine($"The following error happened: {e.Message}");
                                System.Diagnostics.Debug.WriteLine($"Проверьте тип передоваемого фильтра");
                                break;
                                throw;
                            }
                            if (prop.ModelType == expGenericType)
                            {
                                parameters = new[] { exp };
                                break;
                            }
                        }
                        field.Items = typeof(DataAccessLayer<T>).GetMethod(nameof(GetSelectList), BindingFlags.NonPublic | BindingFlags.Instance)
                            .MakeGenericMethod(prop.ModelType)
                            .Invoke(obj: this, parameters: parameters ) as List<Field.Item>;
                    }
                }
                else
                {
                    field.Name = prop.PropertyName;
                    field.Type = prop.ModelType.Name;
                    object value = data?.GetType().GetProperty(prop.PropertyName).GetValue(data);
                    if (value is DateTime valueDatetime)
                    {
                        field.Value = valueDatetime.ToString("o");
                    }
                    else
                    { 
                        field.Value = value?.ToString();
                    }
                }
                Fields.Add(field);
            }
            return Fields;
        }
        public virtual async Task<List<Field>> FormAsync(int? id, params object[] filterExpressions)
        {
            T data = null;
            if (id != null)
            {
                data = await GetSingleAsync((int)id);
            }
            var metadata = _provider.GetMetadataForType(typeof(T));
            List<Field> Fields = new List<Field>();
            foreach (var prop in metadata.Properties.Where(pm => pm.ShowForDisplay))
            {
                Type type = prop.ModelType;
                var field = new Field
                {
                   
                    DisplayName = prop.DisplayName,
                    IsRequired = prop.IsRequired,
                   
                };
                if (prop.TemplateHint == "ServerSide")
                {
                    field.ServerSide = true;
                }
                if (prop.IsComplexType)
                {
                    field.Name = prop.PropertyName + "Id";
                    field.Value = data?.GetType().GetProperty(field.Name).GetValue(data)?.ToString();
                    field.Type = "select";
                    object[] parameters = { null };
                    if (!field.ServerSide)
                    {
                        foreach (var exp in filterExpressions)
                        {
                            Type expGenericType;
                            try
                            {
                                expGenericType = exp.GetType().GetGenericArguments()[0].GenericTypeArguments[0];
                            }
                            catch (Exception e)
                            {
                                System.Diagnostics.Debug.WriteLine($"The following error happened: {e.Message}");
                                System.Diagnostics.Debug.WriteLine($"Проверьте тип передоваемого фильтра");
                                break;
                                throw;
                            }
                            if (prop.ModelType == expGenericType)
                            {
                                parameters = new[] { exp };
                                break;
                            }
                        }
                        field.Items = typeof(DataAccessLayer<T>).GetMethod(nameof(GetSelectList), BindingFlags.NonPublic | BindingFlags.Instance)
                            .MakeGenericMethod(prop.ModelType)
                            .Invoke(obj: this, parameters: parameters) as List<Field.Item>;
                    }
                }
                else
                {
                    field.Type = prop.ModelType.Name;
                    field.Name = prop.PropertyName;
                    object value = data?.GetType().GetProperty(prop.PropertyName).GetValue(data);
                    if (value is DateTime valueDatetime)
                    {
                        field.Value = valueDatetime.ToString("o");
                    }
                    else
                    {
                        field.Value = value?.ToString();
                    }
                }
                Fields.Add(field);
            }
            return Fields;
        }
        public T GetSingle(int id) => _context.Set<T>().FirstOrDefault(x => x.Id == id);
        public T GetSingleNoTracking(int id)=> _context.Set<T>().AsNoTracking().FirstOrDefault(x => x.Id == id);
        public async Task<T> GetSingleAsync(int id) => await _context.Set<T>().FirstOrDefaultAsync(x => x.Id == id);
        public T GetSingle(Expression<Func<T, bool>> predicate) => _context.Set<T>().FirstOrDefault(predicate);
        public T GetSingle(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includeProperties)
        {
            IQueryable<T> query = _context.Set<T>();
            foreach (var includeProperty in includeProperties)
            {
                query = query.Include(includeProperty);
            }
            return query.Where(predicate).FirstOrDefault();
        }
        public virtual IEnumerable<T> FindBy(Expression<Func<T, bool>> predicate) => _context.Set<T>().AsNoTracking().Where(predicate);
        public virtual void Add(T entity)
        {
            EntityEntry dbEntityEntry = _context.Entry<T>(entity);
            _context.Set<T>().Add(entity);
        }
        public virtual async Task AddAsync(T entity)
        {
            EntityEntry dbEntityEntry = _context.Entry<T>(entity);
            await _context.Set<T>().AddAsync(entity);
        }
        public virtual void Update(T entity)
        {
            var local = _context.Set<T>()
            .Local
            .FirstOrDefault(entry => entry.Id.Equals(entity.Id));
            
            if (local!=null) 
            {
                _context.Entry(local).State = EntityState.Detached;
            }
            EntityEntry dbEntityEntry = _context.Entry<T>(entity);
            dbEntityEntry.State = EntityState.Modified;
        }
        public virtual void Delete(T entity)
        {
            EntityEntry dbEntityEntry = _context.Entry<T>(entity);
            dbEntityEntry.State = EntityState.Deleted;
        }
        public virtual void DeleteWhere(Expression<Func<T, bool>> predicate)
        {
            IEnumerable<T> entities = _context.Set<T>().Where(predicate);
            foreach (var entity in entities)
            {
                _context.Entry<T>(entity).State = EntityState.Deleted;
            }
        }
        public virtual void Commit() => _context.SaveChanges();
        public virtual async Task CommitAsync() => await _context.SaveChangesAsync();

    }
        
}
