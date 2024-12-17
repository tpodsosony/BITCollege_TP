using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace BITCollegeService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "ICollegeRegistration" in both code and config file together.
    [ServiceContract]
    public interface ICollegeRegistration
    {
        [OperationContract]
        void DoWork();

        bool DropCourse(int registrationId);

        int RegisterCourse(int studentId, int courseId, String notes);

        double? UpdateGrade(double grade, int registrationId, String notes);
    }
}
