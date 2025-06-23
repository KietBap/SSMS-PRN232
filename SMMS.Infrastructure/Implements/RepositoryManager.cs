using SMMS.Domain.Interface.Repositories;
using SMMS.Infrastructure.Context;


namespace SMMS.Infrastructure.Implements
{
	public class RepositoryManager : IRepositoryManager
	{
		private readonly DatabaseContext _context;
		private IBlogRepository _blogRepository;
		private IClassRepository _classRepository;
		private IConselingRepository _conselingRepository;
		private IConsentRepository _consentRepository;
		private IHealthActivityRepository _healthActivityRepository;
		private IHealthCheckupRepository _healthCheckupRepository;
		private IHealthProfileRepository _healthProfileRepository;
		private IMedicalIncidentRepository _medicalIncidentRepository;
		private IMedicalRequestRepository _medicalRequestRepository;
		private IMedicationRequestAdministrationRepository _medicationRequestAdministrationRepository;
		private IMedicalStockRepository _medicalStockRepository;
		private IMedicalUsageRepository _medicalUsageRepository;
		private INotificationRepository _notificationRepository;
		private IRoleRepository _roleRepository;
		private IStudentRepository _studentRepository;
		private IUserRepository _userRepository;
		private IVaccinationCampaignRepository _vaccinationCampaignRepository;
		private IVaccinationRecordRepository _vaccinationRecordRepository;


		public RepositoryManager(DatabaseContext context)
		{
			_context = context;
			
			_userRepository = new UserRepository(context);
			_blogRepository = new BlogRepository(context);
			_classRepository = new ClassRepository(context);
			_conselingRepository = new ConselingRepository(context);
			_consentRepository = new ConsentRepository(context);
			_healthActivityRepository = new HealthActivityRepository(context);
			_healthCheckupRepository = new HealthCheckupRepository(context);
			_healthProfileRepository = new HealthProfileRepository(context);
			_medicalIncidentRepository = new MedicalIncidentRepository(context);
			_medicalRequestRepository = new MedicalRequestRepository(context);
			_medicationRequestAdministrationRepository = new MedicationRequestAdministrationRepository(context);
			_medicalStockRepository = new MedicalStockRepository(context);
			_medicalUsageRepository = new MedicalUsageRepository(context);
			_notificationRepository = new NotificationRepository(context);
			_roleRepository = new RoleRepository(context);
			_studentRepository = new StudentRepository(context);
			_vaccinationCampaignRepository = new VaccinationCampaignRepository(context);
			_vaccinationRecordRepository = new VaccinationRecordRepository(context);
		}

		public IUserRepository UserRepository { get { return _userRepository; } }

		public IBlogRepository BlogRepository { get { return _blogRepository; } }

		public IClassRepository ClassRepository { get { return _classRepository; } }

		public IConselingRepository ConselingRepository { get { return _conselingRepository; } }

		public IConsentRepository ConsentRepository { get { return _consentRepository; } }

		public IHealthActivityRepository HealthActivityRepository { get { return _healthActivityRepository; } }

		public IHealthCheckupRepository HealthCheckRepository { get { return _healthCheckupRepository; } }

		public IHealthProfileRepository HealthProfileRepository { get { return _healthProfileRepository; } }

		public IMedicalIncidentRepository MedicalIncidentRepository { get { return _medicalIncidentRepository; } }

		public IMedicalRequestRepository MedicalRequestRepository { get { return _medicalRequestRepository; } }

		public IMedicationRequestAdministrationRepository MedicationRequestAdministrationRepository { get { return _medicationRequestAdministrationRepository; } }

		public IMedicalStockRepository MedicalStockRepository { get { return _medicalStockRepository; } }

		public IMedicalUsageRepository MedicalUsageRepository { get { return _medicalUsageRepository; } }

		public INotificationRepository NotificationRepository { get { return _notificationRepository; } }

		public IRoleRepository RoleRepository { get { return _roleRepository; } }

		public IStudentRepository StudentRepository { get { return _studentRepository; } }

		public IVaccinationCampaignRepository VaccinationCampaignRepository { get { return _vaccinationCampaignRepository; } }

		public IVaccinationRecordRepository VaccinationRecordRepository { get { return _vaccinationRecordRepository; } }

		public Task SaveAsync() => _context.SaveChangesAsync();
	}
}
