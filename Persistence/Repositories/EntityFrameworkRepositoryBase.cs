using Domain.Entities.Base;
using Domain.Entities.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Query;
using Persistence.Dynamic;
using Persistence.Dynamic.Extensions;
using Persistence.Paging;
using Persistence.Repositories.Interfaces;
using System.Collections;
using System.Linq.Expressions;
using System.Reflection;

namespace Persistence.Repositories;

public class EntityFrameworkRepositoryBase<TEntity, TEntityId, TContext> : IAsyncRepository<TEntity, TEntityId>
    where TEntity : BaseEntity<TEntityId>
    where TEntityId : notnull
    where TContext : DbContext
{
    #region Constructors and Fields

    protected readonly TContext _dbContext;

    public EntityFrameworkRepositoryBase(TContext dbContext)
    {
        _dbContext = dbContext;
    }

    #endregion

    /// <summary>
    /// Yeni bir entity'yi veritabanına ekler ve <c>CreatedDate</c> alanını otomatik olarak doldurur.
    /// </summary>
    /// <param name="entity">Eklenecek entity nesnesi.</param>
    /// <param name="cancellationToken">İşlemi iptal etmek için kullanılan token.</param>
    /// <returns>Veritabanına eklenen entity nesnesini döndürür.</returns>
    public async Task<TEntity> AddAsync(
        TEntity entity, 
        CancellationToken cancellationToken = default)
    {
        entity.CreatedDate = DateTime.UtcNow;

        await _dbContext.AddAsync(entity, cancellationToken);
        await _dbContext.SaveChangesAsync();

        return entity;
    }

    /// <summary>
    /// Birden fazla entity'yi toplu olarak veritabanına ekler ve her birinin <c>CreatedDate</c> alanını otomatik olarak doldurur.
    /// </summary>
    /// <param name="entities">Eklenecek entity koleksiyonu.</param>
    /// <returns>Veritabanına eklenen entity koleksiyonunu döndürür.</returns>
    public async Task<ICollection<TEntity>> AddRangeAsync(ICollection<TEntity> entities)
    {
        foreach (var entity in entities)
        {
            entity.CreatedDate = DateTime.UtcNow;

            await _dbContext.AddAsync(entity);
            await _dbContext.SaveChangesAsync();
        }

        return entities;
    }

    /// <summary>
    /// Verilen koşulu sağlayan herhangi bir entity'nin var olup olmadığını kontrol eder.
    /// </summary>
    /// <param name="predicate">Filtreleme koşulu. <c>null</c> verilirse tüm kayıtlar kontrol edilir.</param>
    /// <param name="withDeleted">Soft delete yapılmış kayıtların da dahil edilip edilmeyeceğini belirtir.</param>
    /// <param name="enableTracking">EF Core change tracking'in etkin olup olmadığını belirtir.</param>
    /// <param name="cancellationToken">İşlemi iptal etmek için kullanılan token.</param>
    /// <returns>Koşulu sağlayan kayıt varsa <c>true</c>, yoksa <c>false</c> döndürür.</returns>
    public async Task<bool> AnyAsync(
        Expression<Func<TEntity, bool>>? predicate = null, 
        bool withDeleted = false, 
        bool enableTracking = true, 
        CancellationToken cancellationToken = default)
    {
        IQueryable<TEntity> queryable = Query();

        if (!enableTracking) { queryable = queryable.AsNoTracking(); }
        if (withDeleted) { queryable = queryable.IgnoreQueryFilters(); }
        if (predicate != null) { queryable = queryable.Where(predicate); }

        return await queryable.AnyAsync(cancellationToken);
    }

    /// <summary>
    /// Belirtilen entity'yi siler. <paramref name="permanent"/> değerine göre hard delete veya soft delete uygulanır.
    /// </summary>
    /// <param name="entity">Silinecek entity nesnesi.</param>
    /// <param name="permanent">true → Kalıcı silme (hard delete); false → Yumuşak silme (soft delete).</param>
    /// <param name="cancellationToken">İşlemi iptal etmek için kullanılan token.</param>
    /// <returns>Silinen entity nesnesini döndürür.</returns>
    public async Task<TEntity> DeleteAsync(
        TEntity entity, 
        bool permanent = false, 
        CancellationToken cancellationToken = default)
    {
        await SetEntityAsDeletedAsync(entity, permanent, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return entity;
    }

    /// <summary>
    /// Birden fazla entity'yi toplu olarak siler. <paramref name="permanent"/> değerine göre hard delete veya soft delete uygulanır.
    /// </summary>
    /// <param name="entities">Silinecek entity koleksiyonu.</param>
    /// <param name="permanent">true → Kalıcı silme (hard delete); false → Yumuşak silme (soft delete).</param>
    /// <param name="cancellationToken">İşlemi iptal etmek için kullanılan token.</param>
    /// <returns>Silinen entity koleksiyonunu döndürür.</returns>
    public async Task<ICollection<TEntity>> DeleteRangeAsync(
        ICollection<TEntity> entities, 
        bool permanent = false, 
        CancellationToken cancellationToken = default)
    {
        await SetEntityAsDeletedAsync(entities, permanent, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return entities;
    }

    /// <summary>
    /// Belirtilen koşula uyan ilk entity'yi getirir. Kayıt bulunamazsa <c>null</c> döndürür.
    /// </summary>
    /// <param name="predicate">Filtreleme koşulu.</param>
    /// <param name="include">İlişkili entity'leri eager loading ile yüklemek için kullanılan include ifadesi.</param>
    /// <param name="withDeleted">Soft delete yapılmış kayıtların da dahil edilip edilmeyeceğini belirtir.</param>
    /// <param name="enableTracking">EF Core change tracking'in etkin olup olmadığını belirtir.</param>
    /// <param name="cancellationToken">İşlemi iptal etmek için kullanılan token.</param>
    /// <returns>Koşulu sağlayan entity veya bulunamazsa <c>null</c>.</returns>
    public async Task<TEntity?> GetAsync(
        Expression<Func<TEntity, bool>> predicate, 
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null, 
        bool withDeleted = false, 
        bool enableTracking = false, 
        CancellationToken cancellationToken = default)
    {
        IQueryable<TEntity> queryable = Query();

        if (!enableTracking) { queryable = queryable.AsNoTracking(); }
        if (include != null) { queryable = include(queryable); }
        if (withDeleted) { queryable = queryable.IgnoreQueryFilters(); }
            
        return await queryable.FirstOrDefaultAsync(predicate, cancellationToken);
    }

    /// <summary>
    /// Belirtilen koşul, sıralama ve sayfalama parametrelerine göre entity listesini getirir.
    /// </summary>
    /// <param name="predicate">Filtreleme koşulu. <c>null</c> verilirse tüm kayıtlar listelenir.</param>
    /// <param name="orderBy">Sıralama ifadesi.</param>
    /// <param name="include">İlişkili entity'leri eager loading ile yüklemek için kullanılan include ifadesi.</param>
    /// <param name="pageNumber">Sayfa numarası (0 tabanlı).</param>
    /// <param name="pageSize">Sayfada gösterilecek kayıt sayısı.</param>
    /// <param name="withDeleted">Soft delete yapılmış kayıtların da dahil edilip edilmeyeceğini belirtir.</param>
    /// <param name="enableTracking">EF Core change tracking'in etkin olup olmadığını belirtir.</param>
    /// <param name="cancellationToken">İşlemi iptal etmek için kullanılan token.</param>
    /// <returns>Sayfalanmış entity listesini içeren <see cref="Paginate{TEntity}"/> nesnesi.</returns>
    public async Task<Paginate<TEntity>> GetListAsync(
        Expression<Func<TEntity, bool>>? predicate = null, 
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null, 
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null, 
        int pageNumber = 0, int pageSize = 10, bool withDeleted = false, bool enableTracking = false, 
        CancellationToken cancellationToken = default)
    {
        IQueryable<TEntity> queryable = Query();

        if (!enableTracking) { queryable = queryable.AsNoTracking(); }
        if (include != null) { queryable = include(queryable); }
        if (withDeleted) { queryable = queryable.IgnoreQueryFilters(); }
        if (predicate != null) { queryable = queryable.Where(predicate); }
        if (orderBy != null) { return await orderBy(queryable).ToPaginateAsync(pageNumber, pageSize, cancellationToken); }
            
        return await queryable.ToPaginateAsync(pageNumber, pageSize, cancellationToken);
    }

    /// <summary>
    /// Dinamik sıralama ve filtreleme parametrelerine göre sayfalanmış entity listesini getirir.
    /// </summary>
    /// <param name="dynamic">Dinamik sıralama ve filtreleme bilgilerini içeren sorgu nesnesi.</param>
    /// <param name="predicate">Ek filtreleme koşulu. <c>null</c> verilirse yalnızca dinamik sorgu uygulanır.</param>
    /// <param name="include">İlişkili entity'leri eager loading ile yüklemek için kullanılan include ifadesi.</param>
    /// <param name="pageNumber">Sayfa numarası (0 tabanlı).</param>
    /// <param name="pageSize">Sayfada gösterilecek kayıt sayısı.</param>
    /// <param name="withDeleted">Soft delete yapılmış kayıtların da dahil edilip edilmeyeceğini belirtir.</param>
    /// <param name="enableTracking">EF Core change tracking'in etkin olup olmadığını belirtir.</param>
    /// <param name="cancellationToken">İşlemi iptal etmek için kullanılan token.</param>
    /// <returns>Sayfalanmış entity listesini içeren <see cref="Paginate{TEntity}"/> nesnesi.</returns>
    public async Task<Paginate<TEntity>> GetListByDynamicAsync(
        DynamicQuery dynamic, 
        Expression<Func<TEntity, bool>>? predicate = null, 
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null, 
        int pageNumber = 0, 
        int pageSize = 10, 
        bool withDeleted = false, 
        bool enableTracking = false, 
        CancellationToken cancellationToken = default)
    {
        IQueryable<TEntity> queryable = Query().ToDynamic(dynamic);

        if (!enableTracking) { queryable = queryable.AsNoTracking(); }
        if (include != null) { queryable = include(queryable); }
        if (withDeleted) { queryable = queryable.IgnoreQueryFilters(); }
        if (predicate != null) { queryable = queryable.Where(predicate); }
            
        return await queryable.ToPaginateAsync(pageNumber, pageSize, cancellationToken);
    }

    /// <summary>
    /// Bu entity türü için temel <see cref="IQueryable{TEntity}"/> sorgusunu döndürür.
    /// Üzerine ek LINQ sorguları zincirlenebilir.
    /// </summary>
    /// <returns>Entity kümesine ait sorgulanabilir <see cref="IQueryable{TEntity}"/> nesnesi.</returns>
    public IQueryable<TEntity> Query()
    {
        return _dbContext.Set<TEntity>().AsQueryable();
    }

    /// <summary>
    /// Mevcut bir entity'yi günceller ve <c>UpdatedDate</c> alanını otomatik olarak doldurur.
    /// </summary>
    /// <param name="entity">Güncellenecek entity nesnesi.</param>
    /// <param name="cancellationToken">İşlemi iptal etmek için kullanılan token.</param>
    /// <returns>Güncellenen entity nesnesini döndürür.</returns>
    public async Task<TEntity> UpdateAsync(
        TEntity entity, 
        CancellationToken cancellationToken = default)
    {
        entity.UpdatedDate = DateTime.UtcNow;

        _dbContext.Update(entity);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return entity;
    }

    /// <summary>
    /// Birden fazla entity'yi toplu olarak günceller ve her birinin <c>UpdatedDate</c> alanını otomatik olarak doldurur.
    /// </summary>
    /// <param name="entities">Güncellenecek entity koleksiyonu.</param>
    /// <returns>Güncellenen entity koleksiyonunu döndürür.</returns>
    public async Task<ICollection<TEntity>> UpdateRangeAsync(ICollection<TEntity> entities)
    {
        foreach (var entity in entities)
        {
            entity.UpdatedDate = DateTime.UtcNow;

            _dbContext.UpdateRange(entity);
            await _dbContext.SaveChangesAsync();
        }

        return entities;
    }

    #region Helper Methods

    /// <summary>
    /// Bir entity'yi silmek için kullanılan ana yardımcı metottur.
    /// <para>
    /// <paramref name="permanent"/> true ise entity veritabanından fiziksel olarak silinir (hard delete).
    /// false ise entity silinmez; sadece <c>DeletedDate</c> alanı doldurularak "silinmiş" olarak işaretlenir (soft delete).
    /// </para>
    /// </summary>
    /// <param name="entity">Silinecek entity nesnesi.</param>
    /// <param name="permanent">
    /// true → Kalıcı silme (kayıt veritabanından tamamen kaldırılır).<br/>
    /// false → Yumuşak silme (kayıt veritabanında kalır, sadece silinmiş olarak işaretlenir).
    /// </param>
    /// <param name="cancellationToken">İşlemi iptal etmek için kullanılan token.</param>
    private async Task SetEntityAsDeletedAsync(TEntity entity, bool permanent, CancellationToken cancellationToken)
    {
        if (!permanent)
        {
            // Soft delete yapılmadan önce entity'nin bire-bir ilişkisi var mı kontrol et.
            // Bire-bir ilişki varsa soft delete veri tutarsızlığına yol açabileceğinden exception fırlatılır.
            CheckHasEntityHaveOneToOneRelation(entity);

            // Entity'yi ve ona bağlı tüm ilişkili entity'leri soft delete olarak işaretle.
            await SetEntityAsSoftDeletedAsync(entity);
        }
        else
        {
            // permanent = true ise EF Core'a kaydı tamamen sil (hard delete) komutunu ver.
            // SaveChangesAsync() çağrısı bu metodun çağrıldığı yerde (DeleteAsync) yapılır.
            _dbContext.Remove(entity);
        }
    }

    /// <summary>
    /// Birden fazla entity'yi toplu olarak silmek için kullanılan yardımcı metottur.
    /// <para>
    /// Koleksiyondaki her entity için tek entity'yi işleyen
    /// <see cref="SetEntityAsDeletedAsync(TEntity, bool, CancellationToken)"/> metodunu çağırır.
    /// <paramref name="permanent"/> değerine göre her entity hard delete veya soft delete işlemine tabi tutulur.
    /// </para>
    /// </summary>
    /// <param name="entities">Silinecek entity koleksiyonu.</param>
    /// <param name="permanent">
    /// true → Her entity veritabanından kalıcı olarak silinir (hard delete).<br/>
    /// false → Her entity silinmiş olarak işaretlenir; veritabanında varlığını korur (soft delete).
    /// </param>
    /// <param name="cancellationToken">İşlemi iptal etmek için kullanılan token.</param>
    private async Task SetEntityAsDeletedAsync(ICollection<TEntity> entities, bool permanent, CancellationToken cancellationToken)
    {
        foreach (var entity in entities)
        {
            await SetEntityAsDeletedAsync(entity, permanent, cancellationToken);
        }
    }

    /// <summary>
    /// Verilen entity'nin bire-bir (one-to-one) ilişkisi olup olmadığını kontrol eder.
    /// <para>
    /// Bire-bir ilişkiye sahip entity'ler soft delete yapılırken veri tutarsızlığına yol açabileceğinden,
    /// böyle bir ilişki tespit edildiğinde <see cref="InvalidOperationException"/> fırlatılır.
    /// </para>
    /// </summary>
    /// <param name="entity">İlişkisi kontrol edilecek entity nesnesi.</param>
    /// <exception cref="InvalidOperationException">
    /// Entity'nin bire-bir ilişkisi varsa fırlatılır.
    /// </exception>
    private void CheckHasEntityHaveOneToOneRelation(TEntity entity)
    {
        bool hasEntityHaveOneToOneRelation =
             _dbContext
             .Entry(entity)                    // EF Core'un takip ettiği entity kaydını al.
             .Metadata.GetForeignKeys()         // Bu entity'ye ait tüm foreign key ilişkilerini getir.
             .Any(x =>
                 // DependentToPrincipal koleksiyon değilse → bu taraf "bir" (1) tarafıdır.
                 x.DependentToPrincipal?.IsCollection != true
                 // PrincipalToDependent koleksiyon değilse → karşı taraf da "bir" (1) tarafıdır.
                 // İkisi birden true olursa bu 1-1 ilişkidir.
                 && x.PrincipalToDependent?.IsCollection != true
                 // İlişkiyi tanımlayan (declaring) tür bu entity'nin kendisi değilse
                 // (yani ilişki başka bir entity üzerinden tanımlandıysa) kontrol et.
                 && x.DependentToPrincipal?.ForeignKey.DeclaringEntityType.ClrType != entity.GetType());

        if (hasEntityHaveOneToOneRelation)
        {
            // Bire-bir ilişki tespit edildi; soft delete güvenli değil, exception fırlat.
            throw new InvalidOperationException("Entity has a one-to-one relationship and cannot be soft deleted.");
        }
    }

    /// <summary>
    /// Entity'yi ve ona bağlı (Cascade ilişkili) tüm alt entity'leri soft delete olarak işaretler.
    /// <para>
    /// Soft delete, kaydı veritabanından silmek yerine <c>DeletedDate</c> alanını doldurmak demektir.
    /// Böylece kayıt veritabanında varlığını korur ancak normal sorgularda görünmez.
    /// </para>
    /// <para>
    /// Metot, entity'nin navigasyon property'lerini (ilişkili tablolar) dolaşır.
    /// Cascade veya ClientCascade silme davranışına sahip ilişkilerde bağlı entity'ler de
    /// özyinelemeli (recursive) olarak soft delete yapılır.
    /// </para>
    /// </summary>
    /// <param name="entity">Soft delete yapılacak entity. <see cref="IEntityTimeStamps"/> arayüzünü uygulamalıdır.</param>
    private async Task SetEntityAsSoftDeletedAsync(IEntityTimeStamps entity)
    {
        // Eğer entity zaten daha önce soft delete yapılmışsa (DeletedDate dolu) tekrar işlem yapma, metottan çık.
        if (entity.DeletedDate.HasValue) { return; }

        // Entity'yi "silinmiş" olarak işaretle: DeletedDate alanına şu anki UTC zamanını ata.
        // Bu alan dolu olduğu sürece global query filter tarafından sorgulardan otomatik olarak elenir.
        entity.DeletedDate = DateTime.UtcNow;

        // Bu entity'ye ait navigasyon property'lerini (ilişkili tablolar) getir.
        // Filtre koşulları:
        //   IsOnDependent: false  → yalnızca bu entity'nin "ana taraf (principal)" olduğu ilişkileri al.
        //                           Yani bu entity'nin sahip olduğu koleksiyon veya referanslar.
        //   DeleteBehavior: Cascade veya ClientCascade → silme işleminin alt kayıtlara da yansıması gereken ilişkiler.
        var navigations = _dbContext
            .Entry(entity)
            .Metadata.GetNavigations()
            .Where(x => x is { IsOnDependent: false, ForeignKey.DeleteBehavior: DeleteBehavior.ClientCascade or DeleteBehavior.Cascade })
            .ToList();

        // Cascade ilişkili her navigasyon property'sini tek tek gez.
        foreach (INavigation? navigation in navigations)
        {
            // Owned entity'ler (EF Core'un "sahipli tipler" özelliği) bağımsız tablolar değildir; bu nedenle atla.
            if (navigation.TargetEntityType.IsOwned()) { continue; }

            // PropertyInfo null ise bu navigasyon property'sine reflection ile erişilemez; atla.
            if (navigation.PropertyInfo == null) { continue; }

            // Navigasyon property'sinin bellekteki anlık değerini oku.
            // Entity daha önce Include() ile yüklendiyse bu değer dolu olacaktır (null olmayacaktır).
            object? navValue = navigation.PropertyInfo.GetValue(entity);

            if (navigation.IsCollection)
            {
                // Navigasyon bir koleksiyondur (örn. ICollection<OrderItem>, List<Address>).
                if (navValue == null)
                {
                    // Koleksiyon bellekte yüklü değil (Include yapılmamış); veritabanından sorgula.
                    IQueryable query = _dbContext.Entry(entity).Collection(navigation.PropertyInfo.Name).Query();

                    // Ham sorguyu tip-güvenli hâle getir, silinmemiş kayıtları filtrele ve listeye dönüştür.
                    navValue = await GetRelationLoaderQuery(query, navigationPropertyType: navigation.PropertyInfo.GetType()).ToListAsync();

                    // Veritabanında da ilişkili kayıt bulunamazsa bu navigasyonu atla.
                    if (navValue == null) { continue; }
                }

                // Koleksiyondaki her alt entity için bu metodu özyinelemeli (recursive) olarak çağır.
                // Böylece zincirleme ilişkilerde (A → B → C) tüm kayıtlar soft delete yapılır.
                foreach (IEntityTimeStamps navValueItem in (IEnumerable)navValue)
                {
                    await SetEntityAsSoftDeletedAsync(navValueItem);
                }
            }
            else
            {
                // Navigasyon tekil bir referanstır (örn. UserProfile, Address).
                if (navValue == null)
                {
                    // Referans bellekte yüklü değil; veritabanından tek kayıt olarak sorgula.
                    IQueryable query = _dbContext.Entry(entity).Reference(navigation.PropertyInfo.Name).Query();

                    // Ham sorguyu tip-güvenli hâle getir, silinmemiş ilk kaydı getir.
                    navValue = await GetRelationLoaderQuery(query, navigationPropertyType: navigation.PropertyInfo.GetType()).FirstOrDefaultAsync();

                    // Veritabanında da ilişkili kayıt bulunamazsa bu navigasyonu atla.
                    if (navValue == null) { continue; }
                }

                // Tekil ilişkili entity için bu metodu özyinelemeli olarak çağır.
                // navValue, IEntityTimeStamps arayüzüne cast edilerek DeletedDate atanır.
                await SetEntityAsSoftDeletedAsync((IEntityTimeStamps)navValue);
            }
        }

        // Tüm alt entity'ler işlendikten sonra bu entity'nin değişikliğini (DeletedDate) EF Core'a bildir.
        // Change Tracker güncellenir; asıl veritabanı yazma işlemi dışarıda SaveChangesAsync() ile yapılır.
        _dbContext.Update(entity);
    }

    /// <summary>
    /// Bir navigasyon property'sine ait ham <see cref="IQueryable"/> sorgusunu,
    /// belirli bir entity türü için tip-güvenli (<c>IQueryable&lt;object&gt;</c>) hâle getirir
    /// ve soft delete yapılmamış (yani <c>DeletedDate</c> alanı boş olan) kayıtları filtreler.
    /// <para>
    /// Bu metot, ilişkili entity'ler henüz belleğe yüklenmemişken (lazy load yapılmamışken)
    /// veritabanından doğrudan sorgu çekmek için kullanılır.
    /// </para>
    /// </summary>
    /// <param name="query">
    /// EF Core tarafından üretilmiş, henüz tip bilgisi olmayan ham navigasyon sorgusu.
    /// </param>
    /// <param name="navigationPropertyType">
    /// Sorgunun çalıştırılacağı navigasyon property'sinin CLR türü.
    /// Bu tür, generic <c>CreateQuery&lt;T&gt;</c> metoduna parametre olarak verilir.
    /// </param>
    /// <returns>
    /// Soft delete yapılmamış (aktif) ilişkili entity'leri döndüren tip-güvenli bir <see cref="IQueryable{T}"/>.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    /// EF Core'un <c>IQueryProvider</c>'ında <c>CreateQuery&lt;TElement&gt;</c> metodu bulunamazsa fırlatılır.
    /// </exception>
    private IQueryable<object> GetRelationLoaderQuery(IQueryable query, Type navigationPropertyType)
    {
        // Ham IQueryable'ın arkasındaki provider'ın (EF Core'un sorgu motoru) gerçek CLR türünü al.
        // Reflection ile generic metoda ulaşmak için bu türe ihtiyaç duyulur.
        Type queryProviderType = query.Provider.GetType();

        // Provider üzerindeki tüm metotlar arasından "CreateQuery" adlı ve generic olan metodu bul.
        // Ardından bulunan generic metodu, navigasyon property'sinin türüyle (navigationPropertyType) somutlaştır.
        // Örneğin: CreateQuery<OrderItem>(...) çağrısına karşılık gelir.
        // Metot bulunamazsa anlamlı bir hata fırlat.
        MethodInfo createQueryMethod = queryProviderType
                .GetMethods()
                .First(m => m is { Name: nameof(query.Provider.CreateQuery), IsGenericMethod: true })
                ?.MakeGenericMethod(navigationPropertyType)
                ?? throw new InvalidOperationException("CreateQuery<TElement> method is not found in IQueryProvider.");

        // Somutlaştırılmış CreateQuery<T> metodunu Reflection ile çağır.
        // Parametre olarak mevcut sorgunun Expression ağacını ver; bu sayede orijinal SQL koşulları korunur.
        // Sonuç IQueryable<object> türüne cast edilir (çünkü derleme zamanında tür bilinmez).
        var queryProviderQuery = (IQueryable<object>)createQueryMethod.Invoke(query.Provider, parameters: new object[] { query.Expression })!;

        // Soft delete filtresi uygula: DeletedDate değeri dolu olan (silinmiş) kayıtları sorgu dışında bırak.
        // Yalnızca aktif (silinmemiş) ilişkili kayıtlar döndürülür.
        return queryProviderQuery.Where(x => !((IEntityTimeStamps)x).DeletedDate.HasValue);
    }

    #endregion
}
