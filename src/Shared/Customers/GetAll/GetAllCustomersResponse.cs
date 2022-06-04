using Archie.Shared.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Archie.Shared.Customers.GetAll;

public class GetAllCustomersResponse : List<GetAllCustomersResponse.CustomerDto>
{
    public GetAllCustomersResponse()
    {
    }

    public GetAllCustomersResponse(IEnumerable<GetAllCustomersResponse.CustomerDto> customers)
    {

    }

    public class AuditDto
    { 
        public AuditType AuditType { get; set; }
        public string Description { get; set; }
        public DateTime Timestamp { get; set; }
        public UserDto User { get; set; }

        public AuditDto(AuditType auditType, string description, DateTime timestamp, UserDto user)
        {
            AuditType = auditType;
            Description = description;
            Timestamp = timestamp;
            User = user;
        }
    }

    public class CustomerDto
    {
        public long Id { get; set; }
        public string CompanyName { get; set; }
        public Location Location { get; set; }

        public IList<AuditDto> AuditTrail { get; set; }

        public CustomerDto(long id, string companyName, Location location, IList<AuditDto> auditTrail)
        {
            Id = id;
            CompanyName = companyName;
            Location = location;
            AuditTrail = auditTrail;
        }
    }

    public class UserDto
    {
        public long Id { get; set; }
        public string FullName { get; set; }

        public UserDto(long id, string fullName)
        {
            Id = id;
            FullName = fullName;
        }
    }
}
