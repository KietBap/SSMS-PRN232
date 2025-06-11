using Microsoft.EntityFrameworkCore;
using SMMS.Application.DataObject.RequestObject;
using SMMS.Application.DataObject.ResponseObject;
using SMMS.Application.Services.Interfaces;
using SMMS.Domain.Entity;
using SMMS.Domain.Enum;
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
                    Status = MedicalStockStatus.Available,
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
                    Status = u.Status,
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
                    Status = MedicalIncidentStatus.Pending,
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

        public async Task<bool> UpdateIncidentStatusAsync(string id, MedicalIncidentStatus status ,string userId)
        {
            try
            {
                var incident = _repositoryManager.MedicalIncidentRepository
                    .FindByCondition(i => i.Id == id && !i.DeletedTime.HasValue, true)
                    .FirstOrDefault();

                if (incident == null)
                {
                    throw new Exception("Can not found MedicalIncident or is deleted");
                }

                incident.Status = status;
                incident.LastUpdatedBy = userId;
                incident.LastUpdatedTime = DateTime.Now;

                _repositoryManager.MedicalIncidentRepository .Update(incident);
                await _repositoryManager.SaveAsync();

                return true;
            }
            catch(Exception ex) { 
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
                    stock.Status = MedicalStockStatus.OutOfStock;
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
                    oldStock.Status = MedicalStockStatus.Available;

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
                    newStock.Status = newStock.Quantity == 0 ? MedicalStockStatus.OutOfStock : MedicalStockStatus.Available;
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
                    oldStock.Status = oldStock.Quantity == 0 ? MedicalStockStatus.OutOfStock : MedicalStockStatus.Available;
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


        //-----------------------------------------Medical Request------------------------------------------------

        public async Task<bool> CreateMedicalRequestAsync(string userId, CreateMedicalRequestRequest request)
        {
            try
            {
                var medicalRequest = new MedicalRequest
                {
                    StudentId = request.StudentId,
                    ParentId = request.ParentId,
                    UserId = userId,
                    MedicalName = request.MedicalName,
                    StartTime = request.StartTime,
                    EndTime = request.EndTime,
                    Quantity = request.Quantity,
                    Dosage = request.Dosage,
                    Notes = request.Notes,
                    Status = "Active",
                    IsCompletedToday = false,
                    CreatedTime = DateTime.Now,
                    CreatedBy = userId,
                };

                _repositoryManager.MedicalRequestRepository.Create(medicalRequest);
                await _repositoryManager.SaveAsync();

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<ListMedicalRequestResponse>> GetAllMedicalRequestsAsync()
        {
            try
            {
                var medicalRequests = _repositoryManager.MedicalRequestRepository
                    .FindByCondition(m => !m.DeletedTime.HasValue, false)
                    .Include(mr => mr.Student)
                        .ThenInclude(s => s.SchoolClass)
                    .Select(mr => new ListMedicalRequestResponse
                    {
                        Id = mr.Id,
                        StudentName = mr.Student.FullName,
                        Class = mr.Student.SchoolClass.ClassName,
                        MedicalName = mr.MedicalName,
                        Status = mr.Status,
                        StartTime = mr.StartTime,
                        EndTime = mr.EndTime,
                        Dosage = mr.Dosage,
                        IsCompletedToday = mr.IsCompletedToday,
                        LastCompletedDate = mr.LastCompletedDate
                    }).ToList();

                return medicalRequests;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<MedicalRequestResponse> GetMedicalRequestByIdAsync(string id)
        {
            try
            {
                var medicalRequest = _repositoryManager.MedicalRequestRepository
                    .FindByCondition(m => m.Id == id && !m.DeletedTime.HasValue, false)
                    .Include(mr => mr.Student)
                        .ThenInclude(s => s.SchoolClass)
                    .Include(mr => mr.Parent)
                    .Include(mr => mr.User)
                    .FirstOrDefault();

                if (medicalRequest == null)
                {
                    throw new KeyNotFoundException("Medical request not found or has been deleted.");
                }

                return new MedicalRequestResponse
                {
                    Id = medicalRequest.Id,
                    StudentId = medicalRequest.StudentId,
                    StudentName = medicalRequest.Student?.FullName ?? "N/A",
                    Class = medicalRequest.Student?.SchoolClass?.ClassName ?? "N/A",
                    ParentId = medicalRequest.ParentId,
                    ParentName = medicalRequest.Parent?.FullName ?? "N/A",
                    UserId = medicalRequest.UserId,
                    NurseName = medicalRequest.User?.FullName ?? "N/A",
                    MedicalName = medicalRequest.MedicalName,
                    Status = medicalRequest.Status,
                    StartTime = medicalRequest.StartTime,
                    EndTime = medicalRequest.EndTime,
                    Quantity = medicalRequest.Quantity,
                    Dosage = medicalRequest.Dosage,
                    Notes = medicalRequest.Notes,
                    CreatedTime = medicalRequest.CreatedTime,
                    LastCompletedDate = medicalRequest.LastCompletedDate,
                    IsCompletedToday = medicalRequest.IsCompletedToday
                };
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<bool> UpdateMedicalRequestAsync(string id, UpdateMedicalRequestRequest request, string userId)
        {
            try
            {
                var medicalRequest = _repositoryManager.MedicalRequestRepository
                    .FindByCondition(m => m.Id == id && !m.DeletedTime.HasValue, true)
                    .FirstOrDefault();

                if (medicalRequest == null)
                    throw new KeyNotFoundException("Medical request not found or has been deleted.");

                medicalRequest.MedicalName = request.MedicalName;
                medicalRequest.StartTime = request.StartTime;
                medicalRequest.EndTime = request.EndTime;
                medicalRequest.Quantity = request.Quantity;
                medicalRequest.Dosage = request.Dosage;
                medicalRequest.Notes = request.Notes;
                medicalRequest.LastUpdatedBy = userId;
                medicalRequest.LastUpdatedTime = DateTime.Now;

                _repositoryManager.MedicalRequestRepository.Update(medicalRequest);
                await _repositoryManager.SaveAsync();

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<bool> DeleteMedicalRequestAsync(string id, string userId)
        {
            try
            {
                var medicalRequest = _repositoryManager.MedicalRequestRepository
                    .FindByCondition(m => m.Id == id && !m.DeletedTime.HasValue, true)
                    .FirstOrDefault();

                if (medicalRequest == null)
                    throw new KeyNotFoundException("Medical request not found or has been deleted.");

                medicalRequest.DeletedTime = DateTimeOffset.Now;
                medicalRequest.DeletedBy = userId;

                _repositoryManager.MedicalRequestRepository.Update(medicalRequest);
                await _repositoryManager.SaveAsync();

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<DailyMedicalRequestResponse>> GetDailyMedicalRequestsAsync(DateTime date)
        {
            try
            {
                // Chỉ lấy phần ngày, bỏ qua thời gian cụ thể
                var targetDate = date.Date;

                var medicalRequests = _repositoryManager.MedicalRequestRepository
                    .FindByCondition(m => !m.DeletedTime.HasValue &&
                                          m.StartTime.Date <= targetDate &&
                                          m.EndTime.Date >= targetDate &&
                                          m.Status == "Active", false)
                    .Include(mr => mr.Student)
                        .ThenInclude(s => s.SchoolClass)
                    .Select(mr => new DailyMedicalRequestResponse
                    {
                        Id = mr.Id,
                        StudentName = mr.Student.FullName,
                        Class = mr.Student.SchoolClass.ClassName,
                        MedicalName = mr.MedicalName,
                        Dosage = mr.Dosage,
                        Quantity = mr.Quantity,
                        IsCompleted = mr.IsCompletedToday && mr.LastCompletedDate.HasValue &&
                                     mr.LastCompletedDate.Value.Date == targetDate,
                        CompletedTime = mr.LastCompletedDate.HasValue &&
                                       mr.LastCompletedDate.Value.Date == targetDate ?
                                       mr.LastCompletedDate : null,
                        Status = mr.Status,
                        Notes = mr.Notes
                    }).ToList();

                return medicalRequests;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<bool> CompleteMedicalRequestAsync(string id, string userId)
        {
            try
            {
                var medicalRequest = _repositoryManager.MedicalRequestRepository
                    .FindByCondition(m => m.Id == id && !m.DeletedTime.HasValue, true)
                    .FirstOrDefault();

                if (medicalRequest == null)
                    throw new KeyNotFoundException("Medical request not found or has been deleted.");

                var now = DateTime.Now;

                // Kiểm tra xem đã complete hôm nay chưa
                if (medicalRequest.LastCompletedDate.HasValue &&
                    medicalRequest.LastCompletedDate.Value.Date == now.Date)
                {
                    throw new InvalidOperationException("Medical request has already been completed today.");
                }

                medicalRequest.IsCompletedToday = true;
                medicalRequest.LastCompletedDate = now;
                medicalRequest.LastUpdatedBy = userId;
                medicalRequest.LastUpdatedTime = DateTimeOffset.Now;

                _repositoryManager.MedicalRequestRepository.Update(medicalRequest);
                await _repositoryManager.SaveAsync();

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<bool> UpdateMedicalRequestStatusAsync(string id, string status, string userId)
        {
            try
            {
                var medicalRequest = _repositoryManager.MedicalRequestRepository
                    .FindByCondition(m => m.Id == id && !m.DeletedTime.HasValue, true)
                    .FirstOrDefault();

                if (medicalRequest == null)
                    throw new KeyNotFoundException("Medical request not found or has been deleted.");

                medicalRequest.Status = status;
                medicalRequest.LastUpdatedBy = userId;
                medicalRequest.LastUpdatedTime = DateTime.Now;

                _repositoryManager.MedicalRequestRepository.Update(medicalRequest);
                await _repositoryManager.SaveAsync();

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<ListMedicalRequestResponse>> GetMedicalRequestsByStudentAsync(string studentId)
        {
            try
            {
                var medicalRequests = _repositoryManager.MedicalRequestRepository
                    .FindByCondition(m => !m.DeletedTime.HasValue && m.StudentId == studentId, false)
                    .Include(mr => mr.Student)
                        .ThenInclude(s => s.SchoolClass)
                    .Select(mr => new ListMedicalRequestResponse
                    {
                        Id = mr.Id,
                        StudentName = mr.Student.FullName,
                        Class = mr.Student.SchoolClass.ClassName,
                        MedicalName = mr.MedicalName,
                        Status = mr.Status,
                        StartTime = mr.StartTime,
                        EndTime = mr.EndTime,
                        Dosage = mr.Dosage,
                        IsCompletedToday = mr.IsCompletedToday,
                        LastCompletedDate = mr.LastCompletedDate
                    }).ToList();

                return medicalRequests;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<ListMedicalRequestResponse>> GetMedicalRequestsByStatusAsync(string status)
        {
            try
            {
                var medicalRequests = _repositoryManager.MedicalRequestRepository
                    .FindByCondition(m => !m.DeletedTime.HasValue && m.Status == status, false)
                    .Include(mr => mr.Student)
                        .ThenInclude(s => s.SchoolClass)
                    .Select(mr => new ListMedicalRequestResponse
                    {
                        Id = mr.Id,
                        StudentName = mr.Student.FullName,
                        Class = mr.Student.SchoolClass.ClassName,
                        MedicalName = mr.MedicalName,
                        Status = mr.Status,
                        StartTime = mr.StartTime,
                        EndTime = mr.EndTime,
                        Dosage = mr.Dosage,
                        IsCompletedToday = mr.IsCompletedToday,
                        LastCompletedDate = mr.LastCompletedDate
                    }).ToList();

                return medicalRequests;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<ListMedicalRequestResponse>> SearchMedicalRequestsAsync(string? medicalName, string? studentId, DateTime? date, string? status)
        {
            try
            {
                var query = _repositoryManager.MedicalRequestRepository
                    .FindByCondition(m => !m.DeletedTime.HasValue, false)
                    .Include(mr => mr.Student)
                        .ThenInclude(s => s.SchoolClass)
                    .AsQueryable();

                if (!string.IsNullOrEmpty(medicalName))
                {
                    query = query.Where(m => m.MedicalName.Contains(medicalName));
                }

                if (!string.IsNullOrEmpty(studentId))
                {
                    query = query.Where(m => m.StudentId == studentId);
                }

                if (date.HasValue)
                {
                    var targetDate = date.Value.Date;
                    query = query.Where(m => m.StartTime.Date <= targetDate && m.EndTime.Date >= targetDate);
                }

                if (!string.IsNullOrEmpty(status))
                {
                    query = query.Where(m => m.Status == status);
                }

                var medicalRequests = query
                    .Select(mr => new ListMedicalRequestResponse
                    {
                        Id = mr.Id,
                        StudentName = mr.Student.FullName,
                        Class = mr.Student.SchoolClass.ClassName,
                        MedicalName = mr.MedicalName,
                        Status = mr.Status,
                        StartTime = mr.StartTime,
                        EndTime = mr.EndTime,
                        Dosage = mr.Dosage,
                        IsCompletedToday = mr.IsCompletedToday,
                        LastCompletedDate = mr.LastCompletedDate
                    }).ToList();

                return medicalRequests;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<bool> ResetDailyCompletionStatusAsync()
        {
            try
            {
                var today = DateTime.Today;
                var medicalRequests = _repositoryManager.MedicalRequestRepository
                    .FindByCondition(m => !m.DeletedTime.HasValue &&
                                          m.IsCompletedToday &&
                                          m.LastCompletedDate.HasValue &&
                                          m.LastCompletedDate.Value.Date < today, true)
                    .ToList();

                foreach (var request in medicalRequests)
                {
                    request.IsCompletedToday = false;
                }

                if (medicalRequests.Any())
                {
                    await _repositoryManager.SaveAsync();
                }

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<object> GetCompletionStatusByDateAsync(DateTime date)
        {
            try
            {
                var targetDate = date.Date;

                var totalRequests = _repositoryManager.MedicalRequestRepository
                    .FindByCondition(m => !m.DeletedTime.HasValue &&
                                          m.StartTime.Date <= targetDate &&
                                          m.EndTime.Date >= targetDate &&
                                          m.Status == "Active", false)
                    .Count();

                var completedRequests = _repositoryManager.MedicalRequestRepository
                    .FindByCondition(m => !m.DeletedTime.HasValue &&
                                          m.StartTime.Date <= targetDate &&
                                          m.EndTime.Date >= targetDate &&
                                          m.Status == "Active" &&
                                          m.LastCompletedDate.HasValue &&
                                          m.LastCompletedDate.Value.Date == targetDate, false)
                    .Count();

                return new
                {
                    Date = targetDate,
                    TotalRequests = totalRequests,
                    CompletedRequests = completedRequests,
                    PendingRequests = totalRequests - completedRequests,
                    CompletionRate = totalRequests > 0 ? (double)completedRequests / totalRequests * 100 : 0
                };
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }


    }
}
