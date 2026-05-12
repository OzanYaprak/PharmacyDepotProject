using Application.Features.Drugs.Constants;
using Application.Rules;
using CrossCuttingConcerns.Exceptions.Types;
using Domain.Entities;
using Persistence.Repositories.Drug;

namespace Application.Features.Drugs.Rules;

public class DrugBusinessRules : BaseBusinessRules
{
    #region Constructor And Fields

    private readonly IDrugRepository _drugRepository;
    public DrugBusinessRules(IDrugRepository drugRepository)
    {
        _drugRepository = drugRepository;
    }

    #endregion

    /// <summary>
    /// Insert sırasında aynı GTIN'e sahip ilaç olmamalıdır.
    /// </summary>
    public async Task GtinCannotBeDuplicatedWhenInserted(string gtin)
    {
        Drug? drug = await _drugRepository.GetAsync(predicate: d => d.GTIN == gtin);

        if (drug != null)
            throw new BusinessException(DrugMessages.GtinExists);
    }

    /// <summary>
    /// Insert sırasında aynı seri numarasına (SN) sahip ilaç olmamalıdır.
    /// </summary>
    public async Task SerialNumberCannotBeDuplicatedWhenInserted(string sn)
    {
        Drug? drug = await _drugRepository.GetAsync(predicate: d => d.SN == sn);

        if (drug != null)
            throw new BusinessException(DrugMessages.SerialNumberExists);
    }

    /// <summary>
    /// Update sırasında başka bir ilaçta aynı GTIN olamaz.
    /// </summary>
    public async Task GtinCannotBeDuplicatedWhenUpdated(Guid id, string gtin)
    {
        var dbList = await _drugRepository.GetListAsync(predicate: d => d.Id != id);
        List<Drug>? list = dbList.DataList?.ToList();

        if (list?.Any(d => d.GTIN == gtin) == true)
            throw new BusinessException(DrugMessages.GtinExists);
    }

    /// <summary>
    /// Update sırasında başka bir ilaçta aynı seri numarası (SN) olamaz.
    /// </summary>
    public async Task SerialNumberCannotBeDuplicatedWhenUpdated(Guid id, string sn)
    {
        var dbList = await _drugRepository.GetListAsync(predicate: d => d.Id != id);
        List<Drug>? list = dbList.DataList?.ToList();

        if (list?.Any(d => d.SN == sn) == true)
            throw new BusinessException(DrugMessages.SerialNumberExists);
    }

    /// <summary>
    /// Son kullanma tarihi geçmiş bir tarih olamaz.
    /// </summary>
    public Task ExpireDateCannotBeInThePast(DateTime expireDate)
    {
        if (expireDate < DateTime.UtcNow)
            throw new BusinessException(DrugMessages.ExpireDateCannotBeInThePast);

        return Task.CompletedTask;
    }

    /// <summary>
    /// Verilen ID'ye sahip ilacın veritabanında mevcut olduğunu doğrular.
    /// Yoksa NotFoundException fırlatır → HTTP 404 Not Found döndürülür.
    /// GetById, Update, Delete gibi operasyonlardan önce çağrılmalıdır.
    /// </summary>
    public async Task DrugMustExistWhenRequested(Guid id)
    {
        Drug? drug = await _drugRepository.GetAsync(predicate: d => d.Id == id);

        if (drug is null)
            throw new NotFoundException(nameof(Drug), id);
    }
}
