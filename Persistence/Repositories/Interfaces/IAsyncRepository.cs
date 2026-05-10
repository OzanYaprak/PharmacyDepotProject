using Domain.Entities.Base;
using Microsoft.EntityFrameworkCore.Query;
using Persistence.Dynamic;
using Persistence.Paging;
using System.Linq.Expressions;

namespace Persistence.Repositories.Interfaces;

/// <summary>
/// Tüm entity'ler için generic asenkron repository arayüzü.
/// CRUD işlemleri ve sayfalama/filtreleme gibi ortak veritabanı operasyonlarını tanımlar.
/// </summary>
/// <typeparam name="TEntity">İşlem yapılacak entity tipi; <see cref="BaseEntity{TEntityId}"/>'den türetilmelidir.</typeparam>
/// <typeparam name="TEntityId">Entity'nin birincil anahtar tipi; null olamaz.</typeparam>
public interface IAsyncRepository<TEntity, TEntityId> : ISqlQuery<TEntity>
    where TEntity : BaseEntity<TEntityId>
    where TEntityId : notnull
{
    /// <summary>
    /// Belirtilen koşulu sağlayan tek bir entity'yi asenkron olarak getirir.
    /// </summary>
    /// <param name="predicate">Filtreleme koşulu.</param>
    /// <param name="include">İlişkili verileri yüklemek için include ifadesi (opsiyonel).</param>
    /// <param name="withDeleted">Soft-delete edilmiş kayıtların da dahil edilip edilmeyeceği.</param>
    /// <param name="enableTracking">EF Core change tracking'in devre dışı bırakılıp bırakılmayacağı.</param>
    /// <param name="cancellationToken">İptal belirteci.</param>
    /// <returns>Koşulu sağlayan entity; bulunamazsa <c>null</c>.</returns>
    Task<TEntity?> GetAsync(
        Expression<Func<TEntity, bool>> predicate,
        Func<IQueryable<TEntity>,
        IIncludableQueryable<TEntity, object>>? include = null,
        bool withDeleted = false,
        bool enableTracking = false,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Koşul, sıralama ve sayfalama destekli entity listesini asenkron olarak getirir.
    /// </summary>
    /// <param name="predicate">Filtreleme koşulu (opsiyonel).</param>
    /// <param name="orderBy">Sıralama ifadesi (opsiyonel).</param>
    /// <param name="include">İlişkili verileri yüklemek için include ifadesi (opsiyonel).</param>
    /// <param name="pageNumber">Sayfa numarası (0 tabanlı).</param>
    /// <param name="pageSize">Sayfa başına kayıt sayısı.</param>
    /// <param name="withDeleted">Soft-delete edilmiş kayıtların dahil edilip edilmeyeceği.</param>
    /// <param name="enableTracking">EF Core change tracking'in devre dışı bırakılıp bırakılmayacağı.</param>
    /// <param name="cancellationToken">İptal belirteci.</param>
    /// <returns>Sayfalanmış entity listesi.</returns>
    Task<Paginate<TEntity>> GetListAsync(
        Expression<Func<TEntity, bool>>? predicate = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null,
        int pageNumber = 0,
        int pageSize = 10,
        bool withDeleted = false,
        bool enableTracking = false,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Dinamik sorgu (filtreleme/sıralama) desteğiyle sayfalanmış entity listesini getirir.
    /// </summary>
    /// <param name="dynamic">Dinamik filtre ve sıralama parametrelerini içeren nesne.</param>
    /// <param name="predicate">Ek filtreleme koşulu (opsiyonel).</param>
    /// <param name="include">İlişkili verileri yüklemek için include ifadesi (opsiyonel).</param>
    /// <param name="pageNumber">Sayfa numarası (0 tabanlı).</param>
    /// <param name="pageSize">Sayfa başına kayıt sayısı.</param>
    /// <param name="withDeleted">Soft-delete edilmiş kayıtların dahil edilip edilmeyeceği.</param>
    /// <param name="enableTracking">EF Core change tracking'in devre dışı bırakılıp bırakılmayacağı.</param>
    /// <param name="cancellationToken">İptal belirteci.</param>
    /// <returns>Sayfalanmış entity listesi.</returns>
    Task<Paginate<TEntity>> GetListByDynamicAsync(
        DynamicQuery dynamic,
        Expression<Func<TEntity, bool>>? predicate = null,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null,
        int pageNumber = 0,
        int pageSize = 10,
        bool withDeleted = false,
        bool enableTracking = false,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Belirtilen koşulu sağlayan herhangi bir kaydın var olup olmadığını asenkron olarak kontrol eder.
    /// </summary>
    /// <param name="predicate">Kontrol koşulu (opsiyonel; belirtilmezse tabloda herhangi bir kayıt aranır).</param>
    /// <param name="withDeleted">Soft-delete edilmiş kayıtların dahil edilip edilmeyeceği.</param>
    /// <param name="enableTracking">EF Core change tracking'in etkin olup olmayacağı.</param>
    /// <param name="cancellationToken">İptal belirteci.</param>
    /// <returns>Koşulu sağlayan kayıt varsa <c>true</c>, yoksa <c>false</c>.</returns>
    Task<bool> AnyAsync(
        Expression<Func<TEntity, bool>>? predicate = null,
        bool withDeleted = false,
        bool enableTracking = true,
        CancellationToken cancellationToken = default);

    /// <summary>Tek bir entity'yi veritabanına asenkron olarak ekler.</summary>
    Task<TEntity> AddAsync(TEntity entity, CancellationToken cancellationToken = default);

    /// <summary>Birden fazla entity'yi veritabanına asenkron olarak toplu ekler.</summary>
    Task<ICollection<TEntity>> AddRangeAsync(ICollection<TEntity> entities);

    /// <summary>Mevcut bir entity'yi asenkron olarak günceller.</summary>
    Task<TEntity> UpdateAsync(TEntity entity, CancellationToken cancellationToken = default);

    /// <summary>Birden fazla entity'yi asenkron olarak toplu günceller.</summary>
    Task<ICollection<TEntity>> UpdateRangeAsync(ICollection<TEntity> entities);

    /// <summary>
    /// Bir entity'yi asenkron olarak siler.
    /// </summary>
    /// <param name="entity">Silinecek entity.</param>
    /// <param name="permanent">
    /// <c>true</c> ise fiziksel silme (hard delete) yapılır;
    /// <c>false</c> ise soft-delete uygulanır.
    /// </param>
    /// <param name="cancellationToken">İptal belirteci.</param>
    Task<TEntity> DeleteAsync(TEntity entity, bool permanent = false, CancellationToken cancellationToken = default);

    /// <summary>
    /// Birden fazla entity'yi asenkron olarak toplu siler.
    /// </summary>
    /// <param name="entities">Silinecek entity koleksiyonu.</param>
    /// <param name="permanent">
    /// <c>true</c> ise fiziksel silme (hard delete) yapılır;
    /// <c>false</c> ise soft-delete uygulanır.
    /// </param>
    /// <param name="cancellationToken">İptal belirteci.</param>
    Task<ICollection<TEntity>> DeleteRangeAsync(ICollection<TEntity> entities, bool permanent = false, CancellationToken cancellationToken = default);
}
