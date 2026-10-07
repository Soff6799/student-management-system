using StudentAccounting.WebApi.DTOs.Contract;

namespace StudentAccounting.WebApi.Services.Contract;

/// <summary>
/// Генератор документов договора в форматах DOCX и PDF
/// </summary>
public interface IContractDocumentGenerator
{
    byte[] GenerateDocx(ContractDocumentData data);
    byte[] GeneratePdf(ContractDocumentData data);
}