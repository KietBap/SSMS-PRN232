using Azure.Core;
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
		private readonly INotificationService _notificationService;

		public MedicalService(IRepositoryManager repositoryManager, INotificationService notificationService)
		{
			_repositoryManager = repositoryManager;
			_notificationService = notificationService;
		}


        //-----------------------------------------Medical Stock------------------------------------------------


        public async Task<bool> CreateMedicalStockAsync(string userId, CreateMedicalStockRequest request)
        {
            try
            {
                var stock = _repositoryManager.MedicalStockRepository
                    .FindByCondition(s => s.Name == request.Name && !s.DeletedTime.HasValue, false)
                    .FirstOrDefault();

                if (stock != null)
                {
                    throw new Exception("Stock is already exist");
                }

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
            catch (Exception ex)
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

                if (medicalStock == null)
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
            catch (Exception ex)
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

                _repositoryManager.MedicalStockRepository.Update(medicalStock);
                await _repositoryManager.SaveAsync();

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }


        //-----------------------------------------Medical Incident------------------------------------------------


        public async Task<bool> CreateMedicalIncidentAsync(string userId, CreateMedicalIncidentRequest request)
        {
            try
            {
                // Tạo Incident
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

                // Xử lý Usage
                var stockIds = request.MedicalUsageDetails.Select(m => m.MedicalStockId).Distinct().ToList();

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

                    stock.Quantity -= detail.Quantity;
                    if (stock.Quantity == 0)
                    {
                        stock.Status = MedicalStockStatus.OutOfStock;
                    }

                    var usage = new MedicalUsage
                    {
                        MedicalIncidentId = medicalIncident.Id,
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

                // Gửi thông báo cho phụ huynh
                var student = await _repositoryManager.StudentRepository
                    .FindByCondition(s => s.Id == request.StudentId && s.DeletedTime == null, false)
                    .FirstOrDefaultAsync();

                if (student != null)
                {
                    await _notificationService.CreateNotificationAsync(
                        student.ParentId,
                        "New Medical Incident",
                        $"An incident involving {student.FullName} has been reported.",
                        medicalIncident.Id
                    );
                }

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

        public async Task<bool> UpdateIncidentStatusAsync(string id, MedicalIncidentStatus status, string userId)
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

                _repositoryManager.MedicalIncidentRepository.Update(incident);
                await _repositoryManager.SaveAsync();

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }


        //-----------------------------------------Medical Usage------------------------------------------------

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
                // Lấy thông tin Parent để có ParentName và PhoneNumber
                var parent = await _repositoryManager.UserRepository
                    .FindByCondition(u => u.Id == request.ParentId && !u.DeletedTime.HasValue, false)
                    .FirstOrDefaultAsync();

                if (parent == null)
                    throw new KeyNotFoundException("Parent not found.");

                var medicalRequests = new List<MedicalRequest>();

                foreach (var item in request.MedicalRequestItems)
                {
                    var medicalRequest = new MedicalRequest
                    {
                        StudentId = request.StudentId,
                        ParentId = request.ParentId,
                        ParentName = parent.FullName,
                        PhoneNumber = parent.Phone ?? "",
                        UserId = userId,
                        MedicationName = item.MedicationName,
                        Form = item.Form,
                        Dosage = item.Dosage,
                        Route = item.Route,
                        Frequency = item.Frequency,
                        TotalQuantity = item.TotalQuantity,
                        RemainingQuantity = item.TotalQuantity, // Ban đầu bằng TotalQuantity
                        TimeToAdminister = System.Text.Json.JsonSerializer.Serialize(item.TimeToAdminister),
                        StartDate = item.StartDate,
                        EndDate = item.EndDate,
                        Notes = item.Notes,
                        Status = "Active",
                        CreatedTime = DateTimeOffset.Now,
                        CreatedBy = userId
                    };

                    medicalRequests.Add(medicalRequest);
                    _repositoryManager.MedicalRequestRepository.Create(medicalRequest);

                    // Tự động tạo các bản ghi MedicationRequestAdministration dựa trên lịch trình
                    var administrationSchedules = GenerateMedicationAdministrationSchedule(
                        medicalRequest.Id,
                        item.TimeToAdminister,
                        item.StartDate,
                        item.EndDate,
                        item.Dosage);

                    foreach (var schedule in administrationSchedules)
                    {
                        _repositoryManager.MedicationRequestAdministrationRepository.Create(schedule);
                    }
                }

                await _repositoryManager.SaveAsync();

                // Notify Parent
                var student = await _repositoryManager.StudentRepository
                    .FindByCondition(s => s.Id == request.StudentId && s.DeletedTime == null, false)
                    .FirstOrDefaultAsync();

                if (student != null)
                {
                    await _notificationService.CreateNotificationAsync(
                        request.ParentId,
                        "New Medical Request Created",
                        $"Medical request for {student.FullName} has been created with {request.MedicalRequestItems.Count} medication(s).",
                        medicalRequests.First().Id
                    );
                }

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
                var medicalRequests = await _repositoryManager.MedicalRequestRepository
                    .FindByCondition(m => !m.DeletedTime.HasValue, false)
                    .Include(mr => mr.Student)
                        .ThenInclude(s => s.SchoolClass)
                    .Include(mr => mr.MedicationRequestAdministrations)
                    .ToListAsync();

                return medicalRequests.Select(mr => new ListMedicalRequestResponse
                {
                    Id = mr.Id,
					StudentId = mr.StudentId,
					StudentName = mr.Student.FullName,
                    StudentClass = mr.Student.SchoolClass.ClassName,
                    ParentName = mr.ParentName,
                    MedicationName = mr.MedicationName,
                    Form = mr.Form,
                    Dosage = mr.Dosage,
                    Frequency = mr.Frequency,
                    TotalQuantity = mr.TotalQuantity,
                    RemainingQuantity = mr.RemainingQuantity,
                    TimeToAdminister = System.Text.Json.JsonSerializer.Deserialize<List<string>>(mr.TimeToAdminister) ?? new List<string>(),
                    StartDate = mr.StartDate,
                    EndDate = mr.EndDate,
                    Status = mr.Status,
                    CreatedTime = mr.CreatedTime,
                    TotalAdministrations = mr.MedicationRequestAdministrations.Count,
                    CompletedAdministrations = mr.MedicationRequestAdministrations.Count(a => a.WasTaken),
                    LastAdministeredAt = mr.MedicationRequestAdministrations
                        .Where(a => a.WasTaken)
                        .OrderByDescending(a => a.AdministeredAt)
                        .Select(a => a.AdministeredAt)
                        .FirstOrDefault()
                }).ToList();
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
                var medicalRequest = await _repositoryManager.MedicalRequestRepository
                    .FindByCondition(m => m.Id == id && !m.DeletedTime.HasValue, false)
                    .Include(mr => mr.Student)
                        .ThenInclude(s => s.SchoolClass)
                    .Include(mr => mr.Parent)
                    .Include(mr => mr.User)
                    .Include(mr => mr.MedicationRequestAdministrations)
                        .ThenInclude(a => a.Administrator)
                    .FirstOrDefaultAsync();

                if (medicalRequest == null)
                    throw new KeyNotFoundException("Medical request not found or has been deleted.");

                return new MedicalRequestResponse
                {
                    Id = medicalRequest.Id,
                    StudentId = medicalRequest.StudentId,
                    StudentName = medicalRequest.Student?.FullName ?? "N/A",
                    StudentClass = medicalRequest.Student?.SchoolClass?.ClassName ?? "N/A",
                    ParentId = medicalRequest.ParentId,
                    ParentName = medicalRequest.ParentName,
                    PhoneNumber = medicalRequest.PhoneNumber,
                    UserId = medicalRequest.UserId,
                    NurseName = medicalRequest.User?.FullName ?? "N/A",
                    MedicationName = medicalRequest.MedicationName,
                    Form = medicalRequest.Form,
                    Dosage = medicalRequest.Dosage,
                    Route = medicalRequest.Route,
                    Frequency = medicalRequest.Frequency,
                    TotalQuantity = medicalRequest.TotalQuantity,
                    RemainingQuantity = medicalRequest.RemainingQuantity,
                    TimeToAdminister = System.Text.Json.JsonSerializer.Deserialize<List<string>>(medicalRequest.TimeToAdminister) ?? new List<string>(),
                    StartDate = medicalRequest.StartDate,
                    EndDate = medicalRequest.EndDate,
                    Notes = medicalRequest.Notes,
                    Status = medicalRequest.Status,
                    CreatedTime = medicalRequest.CreatedTime,
                    Administrations = medicalRequest.MedicationRequestAdministrations?.Select(a => new MedicationAdministrationResponse
                    {
                        Id = a.Id,
                        AdministeredBy = a.AdministeredBy,
                        AdministratorName = a.Administrator?.FullName ?? "N/A",
                        AdministeredAt = a.AdministeredAt,
                        DoseGiven = a.DoseGiven,
                        WasTaken = a.WasTaken,
                        Notes = a.Notes
                    }).ToList() ?? new List<MedicationAdministrationResponse>()
                };
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<MedicalRequestResponse> UpdateMedicalRequestAsync(string id, UpdateMedicalRequestRequest request, string userId)
        {
            try
            {
                var medicalRequest = await _repositoryManager.MedicalRequestRepository
                    .FindByCondition(m => m.Id == id && !m.DeletedTime.HasValue, true)
                    .Include(mr => mr.Student)
                        .ThenInclude(s => s.SchoolClass)
                    .Include(mr => mr.Parent)
                    .Include(mr => mr.User)
                    .Include(mr => mr.MedicationRequestAdministrations)
                        .ThenInclude(a => a.Administrator)
                    .FirstOrDefaultAsync();

                if (medicalRequest == null)
                    throw new KeyNotFoundException("Medical request not found or has been deleted.");

                // Kiểm tra business logic
                if (request.StartDate >= request.EndDate)
                    throw new InvalidOperationException("Start date must be before end date.");

                if (request.TotalQuantity <= 0)
                    throw new InvalidOperationException("Total quantity must be greater than 0.");

                if (request.Frequency <= 0)
                    throw new InvalidOperationException("Frequency must be greater than 0.");

                // Tính toán RemainingQuantity khi TotalQuantity thay đổi
                var quantityDifference = request.TotalQuantity - medicalRequest.TotalQuantity;
                var newRemainingQuantity = medicalRequest.RemainingQuantity + quantityDifference;

                // Đảm bảo RemainingQuantity không âm
                if (newRemainingQuantity < 0)
                    throw new InvalidOperationException("Cannot reduce total quantity below the amount already administered.");

                // Cập nhật thông tin
                medicalRequest.MedicationName = request.MedicationName;
                medicalRequest.Form = request.Form;
                medicalRequest.Dosage = request.Dosage;
                medicalRequest.Route = request.Route;
                medicalRequest.Frequency = request.Frequency;
                medicalRequest.TotalQuantity = request.TotalQuantity;
                medicalRequest.RemainingQuantity = newRemainingQuantity;
                medicalRequest.TimeToAdminister = System.Text.Json.JsonSerializer.Serialize(request.TimeToAdminister);
                medicalRequest.StartDate = request.StartDate;
                medicalRequest.EndDate = request.EndDate;
                medicalRequest.Notes = request.Notes;
                medicalRequest.LastUpdatedBy = userId;
                medicalRequest.LastUpdatedTime = DateTimeOffset.Now;

                // Cập nhật status nếu cần
                if (medicalRequest.RemainingQuantity == 0)
                {
                    medicalRequest.Status = "Completed";
                }
                else if (medicalRequest.Status == "Completed" && medicalRequest.RemainingQuantity > 0)
                {
                    medicalRequest.Status = "Active";
                }

                _repositoryManager.MedicalRequestRepository.Update(medicalRequest);
                await _repositoryManager.SaveAsync();

                // Trả về response object
                return new MedicalRequestResponse
                {
                    Id = medicalRequest.Id,
                    StudentId = medicalRequest.StudentId,
                    StudentName = medicalRequest.Student?.FullName ?? "N/A",
                    StudentClass = medicalRequest.Student?.SchoolClass?.ClassName ?? "N/A",
                    ParentId = medicalRequest.ParentId,
                    ParentName = medicalRequest.ParentName,
                    PhoneNumber = medicalRequest.PhoneNumber,
                    UserId = medicalRequest.UserId,
                    NurseName = medicalRequest.User?.FullName ?? "N/A",
                    MedicationName = medicalRequest.MedicationName,
                    Form = medicalRequest.Form,
                    Dosage = medicalRequest.Dosage,
                    Route = medicalRequest.Route,
                    Frequency = medicalRequest.Frequency,
                    TotalQuantity = medicalRequest.TotalQuantity,
                    RemainingQuantity = medicalRequest.RemainingQuantity,
                    TimeToAdminister = System.Text.Json.JsonSerializer.Deserialize<List<string>>(medicalRequest.TimeToAdminister) ?? new List<string>(),
                    StartDate = medicalRequest.StartDate,
                    EndDate = medicalRequest.EndDate,
                    Notes = medicalRequest.Notes,
                    Status = medicalRequest.Status,
                    CreatedTime = medicalRequest.CreatedTime,
                    Administrations = medicalRequest.MedicationRequestAdministrations?.Select(a => new MedicationAdministrationResponse
                    {
                        Id = a.Id,
                        AdministeredBy = a.AdministeredBy,
                        AdministratorName = a.Administrator?.FullName ?? "N/A",
                        AdministeredAt = a.AdministeredAt,
                        DoseGiven = a.DoseGiven,
                        WasTaken = a.WasTaken,
                        Notes = a.Notes
                    }).ToList() ?? new List<MedicationAdministrationResponse>()
                };
            }
            catch (KeyNotFoundException)
            {
                throw;
            }
            catch (InvalidOperationException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred while updating the medical request: {ex.Message}");
            }
        }

        public async Task<bool> DeleteMedicalRequestAsync(string id, string userId)
        {
            try
            {
                var medicalRequest = await _repositoryManager.MedicalRequestRepository
                    .FindByCondition(m => m.Id == id && !m.DeletedTime.HasValue, true)
                    .Include(mr => mr.MedicationRequestAdministrations)
                    .FirstOrDefaultAsync();

                if (medicalRequest == null)
                    throw new KeyNotFoundException("Medical request not found or has been deleted.");

                // Soft delete medical request và thay đổi status
                medicalRequest.DeletedTime = DateTimeOffset.Now;
                medicalRequest.DeletedBy = userId;
                medicalRequest.Status = "Deleted";

                // Soft delete tất cả MedicationRequestAdministration liên quan
                foreach (var administration in medicalRequest.MedicationRequestAdministrations)
                {
                    if (!administration.DeletedTime.HasValue) // Chỉ xóa những bản ghi chưa bị xóa
                    {
                        administration.DeletedTime = DateTimeOffset.Now;
                        administration.DeletedBy = userId;
                        _repositoryManager.MedicationRequestAdministrationRepository.Update(administration);
                    }
                }

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
                var medicalRequests = await _repositoryManager.MedicalRequestRepository
                    .FindByCondition(m => !m.DeletedTime.HasValue && m.StudentId == studentId, false)
                    .Include(mr => mr.Student)
                        .ThenInclude(s => s.SchoolClass)
                    .Include(mr => mr.MedicationRequestAdministrations)
                    .ToListAsync();

                return medicalRequests.Select(mr => new ListMedicalRequestResponse
                {
                    Id = mr.Id,
                    StudentName = mr.Student.FullName,
                    StudentClass = mr.Student.SchoolClass.ClassName,
                    ParentName = mr.ParentName,
                    MedicationName = mr.MedicationName,
                    Form = mr.Form,
                    Dosage = mr.Dosage,
                    Frequency = mr.Frequency,
                    TotalQuantity = mr.TotalQuantity,
                    RemainingQuantity = mr.RemainingQuantity,
                    TimeToAdminister = System.Text.Json.JsonSerializer.Deserialize<List<string>>(mr.TimeToAdminister) ?? new List<string>(),
                    StartDate = mr.StartDate,
                    EndDate = mr.EndDate,
                    Status = mr.Status,
                    CreatedTime = mr.CreatedTime,
                    TotalAdministrations = mr.MedicationRequestAdministrations.Count,
                    CompletedAdministrations = mr.MedicationRequestAdministrations.Count(a => a.WasTaken),
                    LastAdministeredAt = mr.MedicationRequestAdministrations
                        .Where(a => a.WasTaken)
                        .OrderByDescending(a => a.AdministeredAt)
                        .Select(a => a.AdministeredAt)
                        .FirstOrDefault()
                }).ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<ListMedicalRequestResponse>> GetMedicalRequestsByParentAsync(string parentId)
        {
            try
            {
                var medicalRequests = await _repositoryManager.MedicalRequestRepository
                    .FindByCondition(m => !m.DeletedTime.HasValue && m.ParentId == parentId, false)
                    .Include(mr => mr.Student)
                        .ThenInclude(s => s.SchoolClass)
                    .Include(mr => mr.MedicationRequestAdministrations)
                    .ToListAsync();

                return medicalRequests.Select(mr => new ListMedicalRequestResponse
                {
                    Id = mr.Id,
                    StudentName = mr.Student.FullName,
                    StudentClass = mr.Student.SchoolClass.ClassName,
                    ParentName = mr.ParentName,
                    MedicationName = mr.MedicationName,
                    Form = mr.Form,
                    Dosage = mr.Dosage,
                    Frequency = mr.Frequency,
                    TotalQuantity = mr.TotalQuantity,
                    RemainingQuantity = mr.RemainingQuantity,
                    TimeToAdminister = System.Text.Json.JsonSerializer.Deserialize<List<string>>(mr.TimeToAdminister) ?? new List<string>(),
                    StartDate = mr.StartDate,
                    EndDate = mr.EndDate,
                    Status = mr.Status,
                    CreatedTime = mr.CreatedTime,
                    TotalAdministrations = mr.MedicationRequestAdministrations.Count,
                    CompletedAdministrations = mr.MedicationRequestAdministrations.Count(a => a.WasTaken),
                    LastAdministeredAt = mr.MedicationRequestAdministrations
                        .Where(a => a.WasTaken)
                        .OrderByDescending(a => a.AdministeredAt)
                        .Select(a => a.AdministeredAt)
                        .FirstOrDefault()
                }).ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<DailyMedicationScheduleResponse>> GetDailyMedicationScheduleAsync(DateTime date)
        {
            try
            {
                var targetDate = date.Date;

                var medicalRequests = await _repositoryManager.MedicalRequestRepository
                    .FindByCondition(m => !m.DeletedTime.HasValue &&
                                          m.StartDate.Date <= targetDate &&
                                          m.EndDate.Date >= targetDate &&
                                          m.Status == "Active", false)
                    .Include(mr => mr.Student)
                        .ThenInclude(s => s.SchoolClass)
                    .Include(mr => mr.MedicationRequestAdministrations)
                        .ThenInclude(a => a.Administrator)
                    .ToListAsync();

                return medicalRequests.Select(mr =>
                {
                    var timeToAdminister = System.Text.Json.JsonSerializer.Deserialize<List<string>>(mr.TimeToAdminister) ?? new List<string>();
                    var todayAdministrations = mr.MedicationRequestAdministrations
                        .Where(a => !a.DeletedTime.HasValue && a.AdministeredAt.Date == targetDate)
                        .ToList();

                    return new DailyMedicationScheduleResponse
                    {
                        Id = mr.Id,
                        StudentName = mr.Student?.FullName ?? "N/A",
                        StudentClass = mr.Student?.SchoolClass?.ClassName ?? "N/A",
                        MedicationName = mr.MedicationName,
                        Form = mr.Form,
                        Dosage = mr.Dosage,
                        Route = mr.Route,
                        ScheduledTimes = timeToAdminister,
                        Notes = mr.Notes,
                        TodayAdministrations = timeToAdminister.Select(time =>
                        {
                            // Parse thời gian từ string để so sánh chính xác
                            if (!TimeSpan.TryParse(time, out var scheduledTime))
                            {
                                // Nếu không parse được thời gian, trả về trạng thái mặc định
                                return new DailyAdministrationStatus
                                {
                                    AdministrationId = null,
                                    ScheduledTime = time,
                                    CompletedAt = null,
                                    DoseGiven = null,
                                    WasTaken = false,
                                    Notes = "Invalid time format",
                                    AdministratorName = null
                                };
                            }

                            var administration = todayAdministrations.FirstOrDefault(a =>
                            {
                                var adminTime = a.AdministeredAt.TimeOfDay;
                                // So sánh với độ chính xác đến phút (cho phép sai lệch 30 phút)
                                return Math.Abs((adminTime - scheduledTime).TotalMinutes) <= 30;
                            });

                            return new DailyAdministrationStatus
                            {
                                AdministrationId = administration?.Id,
                                ScheduledTime = time,
                                CompletedAt = administration?.AdministeredAt,
                                DoseGiven = administration?.DoseGiven,
                                WasTaken = administration?.WasTaken ?? false,
                                Notes = administration?.Notes,
                                AdministratorName = administration?.Administrator?.FullName
                            };
                        }).ToList()
                    };
                }).ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<bool> RecordMedicationAdministrationAsync(string userId, RecordMedicationAdministrationRequest request)
        {
            try
            {
                var medicalRequest = await _repositoryManager.MedicalRequestRepository
                    .FindByCondition(m => m.Id == request.MedicalRequestId && !m.DeletedTime.HasValue, true)
                    .FirstOrDefaultAsync();

                if (medicalRequest == null)
                    throw new KeyNotFoundException("Medical request not found or has been deleted.");

                MedicationRequestAdministration administration;

                // Nếu có AdministrationId, tìm và cập nhật bản ghi đã có
                if (!string.IsNullOrEmpty(request.AdministrationId))
                {
                    administration = await _repositoryManager.MedicationRequestAdministrationRepository
                        .FindByCondition(a => a.Id == request.AdministrationId &&
                                            a.MedicalRequestId == request.MedicalRequestId &&
                                            !a.DeletedTime.HasValue, true)
                        .FirstOrDefaultAsync();

                    if (administration == null)
                        throw new KeyNotFoundException("Administration record not found.");

                    // Cập nhật thông tin
                    administration.AdministeredBy = userId;
                    administration.AdministeredAt = request.AdministeredAt ?? DateTime.Now;
                    administration.DoseGiven = request.DoseGiven;
                    administration.WasTaken = request.WasTaken;
                    administration.Notes = request.Notes;
                    administration.LastUpdatedBy = userId;
                    administration.LastUpdatedTime = DateTimeOffset.Now;

                    _repositoryManager.MedicationRequestAdministrationRepository.Update(administration);
                }
                else
                {
                    // Tạo bản ghi mới (logic cũ)
                    administration = new MedicationRequestAdministration
                    {
                        MedicalRequestId = request.MedicalRequestId,
                        AdministeredBy = userId,
                        AdministeredAt = request.AdministeredAt ?? DateTime.Now,
                        DoseGiven = request.DoseGiven,
                        WasTaken = request.WasTaken,
                        Notes = request.Notes,
                        CreatedTime = DateTimeOffset.Now,
                        CreatedBy = userId
                    };

                    _repositoryManager.MedicationRequestAdministrationRepository.Create(administration);
                }

                // Cập nhật RemainingQuantity nếu thuốc đã được uống
                if (request.WasTaken)
                {
                    // Parse dose để lấy số lượng (ví dụ: "2 viên" -> 2)
                    var doseNumber = ExtractNumberFromDose(request.DoseGiven);
                    medicalRequest.RemainingQuantity = Math.Max(0, medicalRequest.RemainingQuantity - doseNumber);

                    if (medicalRequest.RemainingQuantity == 0)
                    {
                        medicalRequest.Status = "Completed";
                    }

                    _repositoryManager.MedicalRequestRepository.Update(medicalRequest);
                }

                await _repositoryManager.SaveAsync();

                // Notify Parent
                var student = await _repositoryManager.StudentRepository
                    .FindByCondition(s => s.Id == medicalRequest.StudentId && s.DeletedTime == null, false)
                    .FirstOrDefaultAsync();

                if (student != null && request.WasTaken)
                {
                    await _notificationService.CreateNotificationAsync(
                        medicalRequest.ParentId,
                        "Medication Administered",
                        $"{student.FullName} has been given {medicalRequest.MedicationName} - {request.DoseGiven}.",
                        administration.Id
                    );
                }

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        private int ExtractNumberFromDose(string dose)
        {
            // Extract number from dose string like "2 viên", "10ml", etc.
            var match = System.Text.RegularExpressions.Regex.Match(dose, @"\d+");
            return match.Success ? int.Parse(match.Value) : 1;
        }

        /// <summary>
        /// Tự động tạo lịch trình MedicationRequestAdministration dựa trên thời gian và tần suất uống thuốc
        /// </summary>
        /// <param name="medicalRequestId">ID của MedicalRequest</param>
        /// <param name="timeToAdminister">Danh sách thời gian uống thuốc trong ngày (VD: ["07:00", "12:00", "19:00"])</param>
        /// <param name="startDate">Ngày bắt đầu</param>
        /// <param name="endDate">Ngày kết thúc</param>
        /// <param name="dosage">Liều lượng mỗi lần uống</param>
        /// <returns>Danh sách các bản ghi MedicationRequestAdministration</returns>
        private List<MedicationRequestAdministration> GenerateMedicationAdministrationSchedule(
            string medicalRequestId,
            List<string> timeToAdminister,
            DateTime startDate,
            DateTime endDate,
            string dosage)
        {
            var administrations = new List<MedicationRequestAdministration>();

            // Lặp qua từng ngày từ startDate đến endDate
            for (var date = startDate.Date; date <= endDate.Date; date = date.AddDays(1))
            {
                // Lặp qua từng thời gian uống thuốc trong ngày
                foreach (var time in timeToAdminister)
                {
                    // Parse thời gian (VD: "07:00" -> 7 giờ 0 phút)
                    if (TimeSpan.TryParse(time, out var timeSpan))
                    {
                        var administrationDateTime = date.Add(timeSpan);

                        // Chỉ tạo lịch trình cho tương lai hoặc hôm nay
                        if (administrationDateTime >= DateTime.Now.Date)
                        {
                            var administration = new MedicationRequestAdministration
                            {
                                MedicalRequestId = medicalRequestId,
                                AdministeredBy = null, // Sẽ được cập nhật khi y tá thực hiện
                                AdministeredAt = administrationDateTime,
                                DoseGiven = null, // Sẽ được cập nhật khi y tá thực hiện với liều lượng thực tế
                                WasTaken = false, // Mặc định chưa uống
                                Notes = $"Scheduled administration at {time} - Planned dose: {dosage}",
                                CreatedTime = DateTimeOffset.Now,
                                CreatedBy = "System" // Được tạo tự động bởi hệ thống
                            };

                            administrations.Add(administration);
                        }
                    }
                }
            }

            return administrations;
        }

        public async Task<List<ListMedicalRequestResponse>> SearchMedicalRequestsAsync(string? medicationName, string? studentId, DateTime? date, string? status)
        {
            try
            {
                var query = _repositoryManager.MedicalRequestRepository
                    .FindByCondition(m => !m.DeletedTime.HasValue, false)
                    .Include(mr => mr.Student)
                        .ThenInclude(s => s.SchoolClass)
                    .Include(mr => mr.MedicationRequestAdministrations)
                    .AsQueryable();

                if (!string.IsNullOrEmpty(medicationName))
                {
                    query = query.Where(m => m.MedicationName.Contains(medicationName));
                }

                if (!string.IsNullOrEmpty(studentId))
                {
                    query = query.Where(m => m.StudentId == studentId);
                }

                if (date.HasValue)
                {
                    var targetDate = date.Value.Date;
                    query = query.Where(m => m.StartDate.Date <= targetDate && m.EndDate.Date >= targetDate);
                }

                if (!string.IsNullOrEmpty(status))
                {
                    query = query.Where(m => m.Status == status);
                }

                var medicalRequests = await query.ToListAsync();

                return medicalRequests.Select(mr => new ListMedicalRequestResponse
                {
                    Id = mr.Id,
                    StudentName = mr.Student.FullName,
                    StudentClass = mr.Student.SchoolClass.ClassName,
                    ParentName = mr.ParentName,
                    MedicationName = mr.MedicationName,
                    Form = mr.Form,
                    Dosage = mr.Dosage,
                    Frequency = mr.Frequency,
                    TotalQuantity = mr.TotalQuantity,
                    RemainingQuantity = mr.RemainingQuantity,
                    TimeToAdminister = System.Text.Json.JsonSerializer.Deserialize<List<string>>(mr.TimeToAdminister) ?? new List<string>(),
                    StartDate = mr.StartDate,
                    EndDate = mr.EndDate,
                    Status = mr.Status,
                    CreatedTime = mr.CreatedTime,
                    TotalAdministrations = mr.MedicationRequestAdministrations.Count,
                    CompletedAdministrations = mr.MedicationRequestAdministrations.Count(a => a.WasTaken),
                    LastAdministeredAt = mr.MedicationRequestAdministrations
                        .Where(a => a.WasTaken)
                        .OrderByDescending(a => a.AdministeredAt)
                        .Select(a => a.AdministeredAt)
                        .FirstOrDefault()
                }).ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<DailyCompletedMedicationSummary> GetCompletedMedicationHistoryAsync(DateTime date)
        {
            try
            {
                var targetDate = date.Date;

                // Lấy tất cả administrations đã hoàn thành trong ngày
                var completedAdministrations = await _repositoryManager.MedicationRequestAdministrationRepository
                    .FindByCondition(a => !a.DeletedTime.HasValue &&
                                        a.AdministeredBy != null && // Đã được thực hiện
                                        a.AdministeredAt.Date == targetDate, false)
                    .Include(a => a.MedicalRequest)
                        .ThenInclude(mr => mr.Student)
                            .ThenInclude(s => s.SchoolClass)
                    .Include(a => a.Administrator)
                    .OrderBy(a => a.AdministeredAt)
                    .ToListAsync();

                // Lấy tất cả medical requests active trong ngày để tính tổng số lịch
                var activeMedicalRequests = await _repositoryManager.MedicalRequestRepository
                    .FindByCondition(m => !m.DeletedTime.HasValue &&
                                        m.StartDate.Date <= targetDate &&
                                        m.EndDate.Date >= targetDate &&
                                        m.Status == "Active", false)
                    .ToListAsync();

                // Tính tổng số lịch trong ngày
                int totalScheduled = 0;
                foreach (var mr in activeMedicalRequests)
                {
                    var timeToAdminister = System.Text.Json.JsonSerializer.Deserialize<List<string>>(mr.TimeToAdminister) ?? new List<string>();
                    totalScheduled += timeToAdminister.Count;
                }

                // Tạo response objects
                var completedHistoryList = completedAdministrations.Select(a =>
                {
                    var timeToAdminister = System.Text.Json.JsonSerializer.Deserialize<List<string>>(a.MedicalRequest.TimeToAdminister) ?? new List<string>();

                    // Tìm thời gian dự kiến gần nhất với thời gian thực tế
                    var actualTime = a.AdministeredAt.TimeOfDay;
                    var closestScheduledTime = timeToAdminister
                        .Select(t => TimeSpan.TryParse(t, out var time) ? time : TimeSpan.Zero)
                        .OrderBy(t => Math.Abs((t - actualTime).TotalMinutes))
                        .FirstOrDefault();

                    var scheduledDateTime = targetDate.Add(closestScheduledTime);
                    var timeDifference = a.AdministeredAt - scheduledDateTime;

                    return new CompletedMedicationHistoryResponse
                    {
                        Id = a.Id,
                        MedicalRequestId = a.MedicalRequestId,
                        StudentId = a.MedicalRequest.StudentId,
                        StudentName = a.MedicalRequest.Student?.FullName ?? "N/A",
                        StudentClass = a.MedicalRequest.Student?.SchoolClass?.ClassName ?? "N/A",
                        MedicationName = a.MedicalRequest.MedicationName,
                        Form = a.MedicalRequest.Form,
                        Route = a.MedicalRequest.Route,
                        PlannedDosage = a.MedicalRequest.Dosage,
                        ActualDoseGiven = a.DoseGiven ?? "N/A",
                        ScheduledTime = scheduledDateTime,
                        CompletedAt = a.AdministeredAt,
                        WasTaken = a.WasTaken,
                        Notes = a.Notes,
                        AdministratorId = a.AdministeredBy ?? "N/A",
                        AdministratorName = a.Administrator?.FullName ?? "N/A",
                        Status = a.WasTaken ? "Success" : "Failed",
                        TimeDifference = timeDifference
                    };
                }).ToList();

                // Tính toán thống kê
                int totalCompleted = completedAdministrations.Count;
                int successfulAdministrations = completedAdministrations.Count(a => a.WasTaken);
                int failedAdministrations = totalCompleted - successfulAdministrations;
                double successRate = totalCompleted > 0 ? (double)successfulAdministrations / totalCompleted * 100 : 0;

                return new DailyCompletedMedicationSummary
                {
                    Date = targetDate,
                    TotalScheduled = totalScheduled,
                    TotalCompleted = totalCompleted,
                    SuccessfulAdministrations = successfulAdministrations,
                    FailedAdministrations = failedAdministrations,
                    SuccessRate = Math.Round(successRate, 2),
                    CompletedAdministrations = completedHistoryList
                };
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred while retrieving completed medication history: {ex.Message}");
            }
        }

        public async Task<DailyCompletedMedicationSummary> GetTodayCompletedMedicationHistoryAsync()
        {
            return await GetCompletedMedicationHistoryAsync(DateTime.Today);
        }
    }
}
