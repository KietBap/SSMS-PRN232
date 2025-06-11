
namespace SMMS.Domain.Interface.Repositories
{
	public interface IRepositoryManager
	{
		IBlogRepository BlogRepository { get; }
		IClassRepository ClassRepository { get; }
		IConselingRepository ConselingRepository { get; }
		IConsentRepository ConsentRepository { get; }
		IDocumentRepository DocumentRepository { get; }
		IHealthActivityRepository HealthActivityRepository { get; }
		IHealthCheckupRepository HealthCheckRepository { get; }
		IHealthProfileRepository HealthProfileRepository { get; }
		IMedicalIncidentRepository MedicalIncidentRepository { get; }
		IMedicalRequestRepository MedicalRequestRepository { get; }
		IMedicalStockRepository MedicalStockRepository { get; }
		IMedicalUsageRepository MedicalUsageRepository { get; }
		INotificationRepository NotificationRepository { get; }
		IRoleRepository RoleRepository { get; }
		IStudentRepository StudentRepository { get; }
		IUserRepository UserRepository { get; }
		IVaccinationCampaignRepository VaccinationCampaignRepository { get; }
		IVaccinationRecordRepository VaccinationRecordRepository { get; }
		IOtpRepository OtpRepository { get; }
		Task SaveAsync();
	}
}
