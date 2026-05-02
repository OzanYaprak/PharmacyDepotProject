using System;
using System.Collections.Generic;
using System.Text;

namespace Persistence.Repositories.Interfaces;

/// <summary>
/// Generic entity türleri üzerinde LINQ tabanlı sorgulama yapabilmek için kullanılan arayüzdür.
/// IAsyncRepository tarafından uygulanır ve doğrudan veritabanı sorgu nesnesine erişim sağlar.
/// </summary>
/// <typeparam name="TEntity">Sorgulanacak entity türü.</typeparam>
public interface ISqlQuery<TEntity>
{
    /// <summary>
    /// İlgili entity türü için EF Core <see cref="IQueryable{T}"/> sorgu nesnesini döndürür.
    /// Where, Select, Include gibi LINQ operatörleriyle zincirlenebilir.
    /// </summary>
    IQueryable<TEntity> Query();
}
