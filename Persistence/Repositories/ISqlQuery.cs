using System;
using System.Collections.Generic;
using System.Text;

namespace Persistence.Repositories;

// Bu arayüz, veritabanını LINQ kullanarak sorgulamak için bir yöntem sağlamak amacıyla kullanılır. IAsyncRepository arayüzü tarafından LINQ ile veritabanı sorgulama imkânı sunmak için kullanılır.
public interface ISqlQuery<TEntity>
{
    IQueryable<TEntity> Query();
}
