using Microsoft.EntityFrameworkCore;
using SMMS.Application.DataObject.RequestObject;
using SMMS.Application.DataObject.ResponseObject;
using SMMS.Application.Services.Interfaces;
using SMMS.Domain.Entity;
using SMMS.Domain.Interface.Repositories;

namespace SMMS.Application.Services.Implements
{
    public class MedicalService : IMedicalService
    {
        private readonly IRepositoryManager _repositoryManager;

        public MedicalService(IRepositoryManager repositoryManager)
        {
            _repositoryManager = repositoryManager;
        }


        //-----------------------------------------Medical Stock------------------------------------------------


        public async Task<bool> CreateMedicalStockAsync(string userId, CreateMedicalStockRequest request)
		{
            try
            {
                var medicalStock = new MedicalStock
                {
                    Name = request.Name,
                    DetailInformation = request.DetailInformation,
                    Quantity = request.Quantity,
                    ExpiryDate = request.ExpiryDate,
                    Status = "available",
                    CreatedTime = DateTime.Now,
                    CreatedBy = userId,
                };

                _repositoryManager.MedicalStockRepository.Create(medicalStock);
                await _repositoryManager.SaveAsync();

                return true;
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
		}

        public async Task<bool> DeleteMedicalStockAsync(string id, string userId)
        {
            try
            {
                var medicalStock = _repositoryManager.MedicalStockRepository
                    .FindByCondition(m => m.Id == id && !m.DeletedTime.HasValue, true)
                    .FirstOrDefault();
                if (medicalStock == null)
                {
                    throw new Exception("medicalStock is deleted or can not find");
                }

                medicalStock.DeletedBy = userId;
                medicalStock.DeletedTime = DateTime.Now;

                _repositoryManager.MedicalStockRepository.Update(medicalStock);
                await _repositoryManager.SaveAsync();

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<MedicalStockResponse> GetMedicalStockByIdAsync(string id)
        {
            try
            {
                var medicalStock = _repositoryManager.MedicalStockRepository
                    .FindByCondition(m => m.Id == id && !m.DeletedTime.HasValue, false)
                    .FirstOrDefault();

                if(medicalStock == null)
                {
                    throw new Exception("medicalStock is deleted or can not find");
                }

                return new MedicalStockResponse
                {
                    Name = medicalStock.Name,
                    DetailInformation = medicalStock.DetailInformation,
                    Quantity = medicalStock.Quantity,
                    ExpiryDate = medicalStock.ExpiryDate,
                };
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<ListMedicalStockResponse>> GetAllMedicalStockAsync()
        {
            try
            {
                var medicalstocks = _repositoryManager.MedicalStockRepository
                .FindByCondition(m => !m.DeletedTime.HasValue, false)
                .Select(u => new ListMedicalStockResponse
                {
                    Id = u.Id,
                    Name = u.Name,
                    DetailInformation = u.DetailInformation,
                    ExpiryDate = u.ExpiryDate,
                    Quantity = u.Quantity,
                }).ToList();

                return medicalstocks;
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<bool> UpdateMedicalStockAsync(string id, UpdateMedicalStockRequest model, string userId)
        {
            try
            {
                var medicalStock = _repositoryManager.MedicalStockRepository
                    .FindByCondition(m => m.Id == id && !m.DeletedTime.HasValue, true)
                    .FirstOrDefault();

                if (medicalStock == null)
                {
                    throw new Exception("medicalStock is deleted or can not find");
                }

                medicalStock.Name = model.Name;
                medicalStock.DetailInformation = model.DetailInformation;
                medicalStock.Quantity = model.Quantity;
                medicalStock.ExpiryDate = model.ExpiryDate;
                medicalStock.Status = model.Status;
                medicalStock.LastUpdatedBy = userId;
                medicalStock.LastUpdatedTime = DateTime.Now;

                _repositoryManager.MedicalStockRepository .Update(medicalStock);
                await _repositoryManager.SaveAsync();

                return true;
            }
            catch(Exception ex )
            {
                throw new Exception(ex.Message);
            }
        }


        //-----------------------------------------Medical Incident------------------------------------------------


        public async Task<bool> CreateMedicalIncidentAsync(string userId, CreateMedicalIncidentRequest request)
        {
            try
            {
                var medicalIncident = new MedicalIncident
                {
                    StudentId = request.StudentId,
                    UserId = userId,
                    Type = request.Type,
                    Description = request.Description,
                    IncidentDate = request.IncidentDate,
                    Status = "Pending",
                    CreatedTime = DateTime.Now,
                    CreatedBy = userId,
                };

                _repositoryManager.MedicalIncidentRepository.Create(medicalIncident);
                await _repositoryManager.SaveAsync();

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<bool> DeleteMedicalIncidentAsync(string id, string userId)
        {
            try
            {
                var medicalIncident = _repositoryManager.MedicalIncidentRepository
                    .FindByCondition(m => m.Id == id && !m.DeletedTime.HasValue, true)
                    .FirstOrDefault();
                if (medicalIncident == null)
                {
                    throw new Exception("medicalIncident is deleted or can not find");
                }

                medicalIncident.DeletedBy = userId;
                medicalIncident.DeletedTime = DateTime.Now;
                _repositoryManager.MedicalIncidentRepository.Update(medicalIncident);

                // xoa cac usage lien quan
                var medicalUsages = _repositoryManager.MedicalUsageRepository
                    .FindByCondition(mu => mu.MedicalIncidentId == id && !mu.DeletedTime.HasValue, true)
                    .ToList();
                foreach (var usage in medicalUsages)
                {
                    usage.DeletedBy = userId;
                    usage.DeletedTime = DateTime.Now;
                    _repositoryManager.MedicalUsageRepository.Update(usage);
                }

                await _repositoryManager.SaveAsync();

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<MedicalIncidentResponse> GetMedicalIncidentByIdAsync(string id)
        {
            try
            {
                var medicalIncident = _repositoryManager.MedicalIncidentRepository
                    .FindByCondition(m => m.Id == id && !m.DeletedTime.HasValue, false)
                    .Include(mi => mi.Student)
                        .ThenInclude(s => s.SchoolClass)
                    .Include(mi => mi.MedicalUsages.Where(mu => !mu.DeletedTime.HasValue))
                    .FirstOrDefault();

                if (medicalIncident == null)
                {
                    throw new KeyNotFoundException("Medical incident not found or has been deleted.");
                }

                return new MedicalIncidentResponse
                {
                    StudentName = medicalIncident.Student?.FullName ?? "N/A",
                    Class = medicalIncident.Student?.SchoolClass?.ClassName ?? "N/A",
                    Type = medicalIncident.Type,
                    Description = medicalIncident.Description,
                    Status = medicalIncident.Status,
                    IncidentDate = medicalIncident.IncidentDate,
                    MedicalUsages = medicalIncident.MedicalUsages?.Select(mu => new ListMedicalUsageResponse
                    {
                        Id = mu.Id,
                        MedicalName = mu.MedicalName,
                        Dosage = mu.Dosage,
                        Quantity = mu.Quantity,
                        Status = mu.Status
                    }).ToList() ?? new List<ListMedicalUsageResponse>()
                };
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }


        public async Task<List<ListMedicalIncidentResponse>> GetAllMedicalIncidentAsync(string? studentId = null)
        {
            try
            {
                var query = _repositoryManager.MedicalIncidentRepository
                    .FindByCondition(m => !m.DeletedTime.HasValue &&
                                          (string.IsNullOrEmpty(studentId) || m.StudentId == studentId), false)
                    .Include(mi => mi.Student)
                        .ThenInclude(s => s.SchoolClass);

                var medicalIncidents = query
                    .Select(u => new ListMedicalIncidentResponse
                    {
                        Id = u.Id,
                        StudentName = u.Student.FullName,
                        Class = u.Student.SchoolClass.ClassName,
                        Type = u.Type,
                        Status = u.Status,
                        IncidentDate = u.IncidentDate,
                    }).ToList();

                return medicalIncidents;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }


        public async Task<bool> UpdateMedicalIncidentAsync(string id, UpdateMedicalIncidentRequest model, string userId)
        {
            try
            {
                var medicalIncident = _repositoryManager.MedicalIncidentRepository
                    .FindByCondition(m => m.Id == id && !m.DeletedTime.HasValue, true)
                    .FirstOrDefault();

                if (medicalIncident == null)
                {
                    throw new Exception("medicalIncident is deleted or can not find");
                }

                medicalIncident.IncidentDate = model.IncidentDate;
                medicalIncident.Type = model.Type;
                medicalIncident.Status = model.Status;
                medicalIncident.Description = model.Description;
                medicalIncident.LastUpdatedBy = userId;
                medicalIncident.LastUpdatedTime = DateTime.Now;

                _repositoryManager.MedicalIncidentRepository.Update(medicalIncident);
                await _repositoryManager.SaveAsync();

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }


        //-----------------------------------------Medical Usage------------------------------------------------

        public async Task<bool> CreateMedicalUsageAsync(string userId, CreateMedicalUsageRequest request)
        {
            var stockIds = request.MedicalUsageDetails.Select(m => m.MedicalStockId).Distinct().ToList();

            // Lấy tất cả MedicalStock cần thiết một lần
            var medicalStocks = _repositoryManager.MedicalStockRepository
                .FindByCondition(ms => stockIds.Contains(ms.Id) && !ms.DeletedTime.HasValue, true)
                .ToDictionary(ms => ms.Id);

            foreach (var detail in request.MedicalUsageDetails)
            {
                if (!medicalStocks.TryGetValue(detail.MedicalStockId, out var stock))
                {
                    throw new InvalidOperationException($"Không tìm thấy thuốc với ID: {detail.MedicalStockId}");
                }

                if (stock.Quantity < detail.Quantity)
                {
                    throw new InvalidOperationException($"Thuốc '{stock.Name}' không đủ số lượng. Còn lại: {stock.Quantity}");
                }

                // Trừ số lượng thuốc và cập nhật trạng thái
                stock.Quantity -= detail.Quantity;
                if (stock.Quantity == 0)
                {
                    stock.Status = "out of stock";
                }

                // Tạo MedicalUsage mới
                var usage = new MedicalUsage
                {
                    MedicalIncidentId = request.MedicalIncidentId,
                    MedicalStockId = stock.Id,
                    Status = "Is Using",
                    MedicalName = stock.Name,
                    Dosage = detail.Dosage,
                    Quantity = detail.Quantity,
                    CreatedBy = userId,
                    CreatedTime = DateTime.Now,
                };

                _repositoryManager.MedicalUsageRepository.Create(usage);
                _repositoryManager.MedicalStockRepository.Update(stock);
            }

            await _repositoryManager.SaveAsync();
            return true;
        }

        public async Task<bool> DeleteMedicalUsageAsync(string id, string userId)
        {
            try
            {
                var medicalUsage = _repositoryManager.MedicalUsageRepository
                    .FindByCondition(m => m.Id == id && !m.DeletedTime.HasValue, true)
                    .FirstOrDefault();
                if (medicalUsage == null)
                {
                    throw new Exception("medicalUsage is deleted or can not find");
                }

                medicalUsage.DeletedBy = userId;
                medicalUsage.DeletedTime = DateTime.Now;

                _repositoryManager.MedicalUsageRepository.Update(medicalUsage);
                await _repositoryManager.SaveAsync();

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<bool> UpdateMedicalUsageAsync(string id, UpdateMedicalUsageRequest model, string userId)
        {
            var medicalUsage = _repositoryManager.MedicalUsageRepository
                .FindByCondition(m => m.Id == id && !m.DeletedTime.HasValue, true)
                .FirstOrDefault();

            if (medicalUsage == null)
                throw new KeyNotFoundException("Medical usage not found or has been deleted.");

            bool stockChanged = medicalUsage.MedicalStockId != model.MedicalStockId;
            bool quantityChanged = medicalUsage.Quantity != model.Quantity;

            if (stockChanged || quantityChanged)
            {
                // Hoàn trả thuốc cho kho cũ
                var oldStock = _repositoryManager.MedicalStockRepository
                    .FindByCondition(ms => ms.Id == medicalUsage.MedicalStockId && !ms.DeletedTime.HasValue, true)
                    .FirstOrDefault();

                if (oldStock == null)
                    throw new InvalidOperationException("Current medical stock not found.");

                oldStock.Quantity += medicalUsage.Quantity;
                if (oldStock.Quantity > 0)
                    oldStock.Status = "available";

                oldStock.LastUpdatedBy = userId;
                oldStock.LastUpdatedTime = DateTime.Now;
                _repositoryManager.MedicalStockRepository.Update(oldStock);

                if (stockChanged)
                {
                    // Trừ thuốc từ kho mới
                    var newStock = _repositoryManager.MedicalStockRepository
                        .FindByCondition(ms => ms.Id == model.MedicalStockId && !ms.DeletedTime.HasValue, true)
                        .FirstOrDefault();

                    if (newStock == null)
                        throw new InvalidOperationException("New medical stock not found.");

                    if (newStock.Quantity < model.Quantity)
                        throw new InvalidOperationException($"Thuốc '{newStock.Name}' không đủ số lượng. Còn lại: {newStock.Quantity}");

                    newStock.Quantity -= model.Quantity;
                    newStock.Status = newStock.Quantity == 0 ? "out of stock" : "available";
                    newStock.LastUpdatedBy = userId;
                    newStock.LastUpdatedTime = DateTime.Now;

                    _repositoryManager.MedicalStockRepository.Update(newStock);

                    medicalUsage.MedicalStockId = newStock.Id;
                    medicalUsage.MedicalName = newStock.Name;
                }
                else
                {
                    // Cùng kho nhưng thay đổi số lượng
                    if (oldStock.Quantity < model.Quantity)
                        throw new InvalidOperationException($"Thuốc '{oldStock.Name}' không đủ số lượng. Còn lại: {oldStock.Quantity}");

                    oldStock.Quantity -= model.Quantity;
                    oldStock.Status = oldStock.Quantity == 0 ? "out of stock" : "available";
                    oldStock.LastUpdatedBy = userId;
                    oldStock.LastUpdatedTime = DateTime.Now;

                    _repositoryManager.MedicalStockRepository.Update(oldStock);
                }
            }

            // Cập nhật thông tin của usage
            medicalUsage.Quantity = model.Quantity;
            medicalUsage.Dosage = model.Dosage;
            medicalUsage.Status = model.Status;
            medicalUsage.LastUpdatedBy = userId;
            medicalUsage.LastUpdatedTime = DateTime.Now;

            _repositoryManager.MedicalUsageRepository.Update(medicalUsage);
            await _repositoryManager.SaveAsync();

            return true;
        }


    }
}
