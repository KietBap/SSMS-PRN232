using SMMS.Domain.Base;
using System.ComponentModel.DataAnnotations;
namespace SMMS.Domain.Entity
{
	public class HealthActivityClass : BaseEntity
	{
		public string HealthActivityId { get; set; }
		
		public string SchoolClassId { get; set; }

		public virtual HealthActivity HealthActivity { get; set; }
		public virtual SchoolClass SchoolClass { get; set; }
	}
}
